using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UnitBrains.Pathfinding;
using Assets.Scripts.Utilities;
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
        Effect speedBuff = new Effect("speed_boost", "Буст скорости", EffectType.Buff, 10f, 1.5f);
        Effect attackSpeedBuff = new Effect("speed_boost", "Буст скорости атаки", EffectType.Buff, 10f, 1.5f);

        public static List<Unit> Group=new();
        public static List<Unit> AlLUnitPlayer = new();
        public static int UnitCounter=0;// Счетчик юнитов
        public static int NumberUnit { get; set; }// Номер юнита
        public const int MaxUnitInGroup = 3;// Макс количество юнитов в группе

        public UnitConfig Config { get; }
        public Vector2Int Pos { get; private set; }
        public int Health { get; private set; }
        public bool IsDead => Health <= 0;
        public BaseUnitPath ActivePath => _brain?.ActivePath;
        public IReadOnlyList<BaseProjectile> PendingProjectiles => _pendingProjectiles;

        private readonly List<BaseProjectile> _pendingProjectiles = new();
        private IReadOnlyRuntimeModel _runtimeModel;
        private BaseUnitBrain _brain;
       
        private EffectSystem _effectSystem;
        private Coordinator _coordunator;
        private float _nextBrainUpdateTime = 0f;
        private float _nextMoveTime = 0f;
        private float _nextAttackTime = 0f;
        
        public Unit(UnitConfig config, Vector2Int startPos,Coordinator coordinator)
        {
            Group.Clear();
            Config = config;
            Pos = startPos;
            Health = config.MaxHealth;
            _brain = UnitBrainProvider.GetBrain(config);
            _brain.SetUnit(this);

            _effectSystem=ServiceLocator.Get<EffectSystem>();
            _coordunator = coordinator;
           
            //coordinator= ServiceLocator.Get<Coordinator>();
            _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();
            if(config.IsPlayerUnit && config.Name!="Command Buffer")
            {
                AlLUnitPlayer.Add(this);// добавление в группу

            }
            if (config.IsPlayerUnit )// если юнит игрока
            {
                if (_runtimeModel != null)
                Group.Add(this);// добавление в группу
                
                UnitCounter++;// Добавление к счету юнита +1
                NumberUnit = UnitCounter;// Присвоение  юниту номера
                Debug.Log($"Номер юнита {NumberUnit}");

            }
            if (config.Name == "Sky Serpent")
            {
                _effectSystem.AddEffect(this,speedBuff);
            }
           
           
        }
        private bool CanMove(float time)
        {
            float moveDelay = Config.MoveDelay / _effectSystem.GetMoveSpeedModifier(this);
            return _nextMoveTime < time;
        }

        private bool CanAttack(float time)
        {
            float attackDelay = Config.AttackDelay / _effectSystem.GetAttackSpeedModifier(this);
            return _nextAttackTime < time;
        }
        public void Update(float deltaTime, float time)
        {
            if (IsDead)
                return;
            // Движение и атака с учетом модификаторов
            if (CanMove(time)) Move();

            if (CanAttack(time)) Attack();

            if (_nextBrainUpdateTime < time)
            {
                _nextBrainUpdateTime = time + Config.BrainUpdateInterval;
                _brain.Update(deltaTime, time);
            }
            
            if (_nextMoveTime < time)
            {
                _nextMoveTime = time + Config.MoveDelay;
                Move();
            }
            
            if (_nextAttackTime < time && Attack())
            {
                _nextAttackTime = time + Config.AttackDelay;
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
