using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;

namespace UnitBrains.Player
{
    public class ThirdUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Ironclad Behemoth";

        private enum UnitState
        {
            Moving,
            Attacking,
            Switching
        }

        private UnitState _currentState = UnitState.Moving;
        private UnitState _nextState;
        private float _switchTimer;

        private const float SwitchDuration = 1f;

        public override void Update(float deltaTime, float time)
        {
            base.Update(deltaTime, time);

      
            bool canAttack = base.SelectTargets().Any();

            UnitState desiredState = canAttack
                ? UnitState.Attacking
                : UnitState.Moving;

            if (_currentState != desiredState && _currentState != UnitState.Switching)
            {
                Debug.Log($"k: {_currentState} -> {desiredState}");
                _currentState = UnitState.Switching;
                _nextState = desiredState;
                _switchTimer = 0.5f;
            }

            if (_currentState == UnitState.Switching)
            {
                _switchTimer += deltaTime;
                Debug.Log($"IN {_switchTimer:F2}/{SwitchDuration}");

                if (_switchTimer >= SwitchDuration)
                {
                    _currentState = _nextState;
                    Debug.Log($" S: {_currentState}");
                }
            }
        }

        protected override List<Vector2Int> SelectTargets()
        {
            if (_currentState != UnitState.Attacking)
            {
                Debug.Log($"ATTACK BLOCKED= {_currentState}");
                return new List<Vector2Int>();
            }

            Debug.Log("[j");
            return base.SelectTargets();
        }

        public override Vector2Int GetNextStep()
        {
            if (_currentState != UnitState.Moving)
            {
                Debug.Log($"[e = {_currentState}");
                return unit.Pos;
            }

            Debug.Log("D");
            return base.GetNextStep();
        }
    }
}