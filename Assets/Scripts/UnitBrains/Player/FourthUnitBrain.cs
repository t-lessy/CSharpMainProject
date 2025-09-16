using Model;
using Model.Config;
using Model.Runtime.Projectiles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using View;

namespace UnitBrains.Player
{
    public class FourthUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "PlayerUbit4";
        private enum MovementState
        {
            Moving,
            Standing,
            TransitioningToMove,
            TransitioningToStand
        }

        private MovementState _currentState = MovementState.Moving;
        private float _stateChangeTime = 0f;
        private float _lastBuffTime = 0f;
        private const float TransitionDuration = 0.5f;
        private const float BuffCooldown = 5f;

        private VFXView _vfxView;

        public FourthUnitBrain()
        {
            _vfxView = ServiceLocator.Get<VFXView>();
        }

        public override void Update(float deltaTime, float time)
        {
            if (_currentState == MovementState.TransitioningToMove || _currentState == MovementState.TransitioningToStand)
            {
                if (time - _stateChangeTime >= TransitionDuration)
                {
                    if (_currentState == MovementState.TransitioningToMove) 
                        _currentState = MovementState.Moving;
                    else 
                        _currentState = MovementState.Standing;
                }

                return;
            }

            if (_currentState == MovementState.Moving && HasTargetsInRange())
            {
                _currentState = MovementState.TransitioningToStand;
                _stateChangeTime = time;
            }
            else if (_currentState == MovementState.Standing && !HasTargetsInRange())
            {
                _currentState = MovementState.TransitioningToMove;
                _stateChangeTime = time;
            }

            if (time - _lastBuffTime >= BuffCooldown)
            {
                ApplyBuffsToAllies();
                _lastBuffTime = time;
            }
        }

        private void ApplyBuffsToAllies()
        {
            var buffsSystem = ServiceLocator.Get<BuffsSys>();
            if (buffsSystem == null)
                return;

            var alliesInRange = GetUnitsInRadius(unit.Config.AttackRange, true);
            if (alliesInRange == null || alliesInRange.Count == 0)
                return;

            foreach (var ally in alliesInRange)
            {
                if (ally.Config.IsPlayerUnit == unit.Config.IsPlayerUnit)
                {
                    bool hasBuffs = buffsSystem.Effects.ContainsKey((Model.Runtime.Unit)ally);
                    if (!hasBuffs)
                    {
                        var buff = new BuffsSys.Buff(8f, 10f, 10f);
                        buffsSystem.AddBuff((Model.Runtime.Unit)ally, buff);

                        var vfxView = ServiceLocator.Get<VFXView>();
                        vfxView.PlayVFX(ally.Pos, VFXView.VFXType.BuffApplied);

                        _currentState = MovementState.TransitioningToStand;
                        _stateChangeTime = Time.time;
                        return;
                    }
                    else
                    {
                        Debug.Log($"⏩ [{unit.Pos}] союзник {ally.Pos} уже под баффом");
                    }
                }
            }
        }

        public new List<BaseProjectile> GetProjectiles() => new();

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        { 

        }

        public override Vector2Int GetNextStep()
        {
            if (_currentState != MovementState.Moving)
            {
                return unit.Pos;
            }

            return base.GetNextStep();
        }
    }
}