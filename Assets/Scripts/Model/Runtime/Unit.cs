using System.Collections.Generic;
using System.Linq;
using Model.Config;
using Model.Runtime.Projectiles;
using Model.Runtime.ReadOnly;
using UnitBrains;
using UnitBrains.Pathfinding;
using UnityEngine;
using Utilities;

namespace Model.Runtime
{
    public class Unit : IReadOnlyUnit
    {
        private Commander _commander;
        public UnitConfig Config { get; }
        public Vector2Int Pos { get; private set; }
        public int Health { get; private set; }
        public bool IsDead => Health <= 0;
        public BaseUnitPath ActivePath => _brain?.ActivePath;
        public IReadOnlyList<BaseProjectile> PendingProjectiles => _pendingProjectiles;

        private readonly List<BaseProjectile> _pendingProjectiles = new();
        private IReadOnlyRuntimeModel _runtimeModel;
        private BaseUnitBrain _brain;

        private float _nextBrainUpdateTime = 0f;
        private float _nextMoveTime = 0f;
        private float _nextAttackTime = 0f;
        private EffectSystem _effectSystem;

        public Unit(UnitConfig config, Vector2Int startPos, Commander commander)
        {
            Config = config;
            Pos = startPos;
            Health = config.MaxHealth;
            _commander = commander;
            _brain = UnitBrainProvider.GetBrain(config);
            _brain.SetUnit(this);
            _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();
            _effectSystem = ServiceLocator.Get<EffectSystem>();
        }
        public Commander GetCommander()
        {
            return _commander;
        }
        public void Update(float deltaTime, float time)
        {
            
            if (IsDead)
                return;

            float moveFactor = _effectSystem.GetMoveSpeedFactor(this);
            float attackFactor = _effectSystem.GetAttackSpeedFactor(this);

            moveFactor = Mathf.Max(0.01f, moveFactor);
            attackFactor = Mathf.Max(0.01f, attackFactor);

            float actualMoveDelay = Config.MoveDelay / moveFactor;
            float actualAttackDelay = Config.AttackDelay / attackFactor;


            if (_nextBrainUpdateTime < time)
            {
                _nextBrainUpdateTime = time + Config.BrainUpdateInterval;
                _brain.Update(deltaTime, time);
            }

            if (_nextMoveTime < time)
            {
                _nextMoveTime = time + actualMoveDelay;
                Move();
            }

            if (_nextAttackTime < time && Attack())
            {
                _nextAttackTime = time + actualAttackDelay;
            }
        }

        private bool Attack()
        {
            var projectiles = _brain.GetProjectiles();
            if (projectiles == null || projectiles.Count == 0)
                return false;

            _pendingProjectiles.AddRange(projectiles);
            return true;
        }

        private void Move()
        {
            var targetPos = _brain.GetNextStep();
            var delta = targetPos - Pos;
            if (delta.sqrMagnitude > 2)
            {
                Debug.LogError($"Brain for unit {Config.Name} returned invalid move: {delta}");
                return;
            }

            if (_runtimeModel.RoMap[targetPos] ||
                _runtimeModel.RoUnits.Any(u => u.Pos == targetPos))
            {
                return;
            }

            Pos = targetPos;
        }

        public void ClearPendingProjectiles()
        {
            _pendingProjectiles.Clear();
        }

        public void TakeDamage(int projectileDamage)
        {
            Health -= projectileDamage;
        }
    }
}