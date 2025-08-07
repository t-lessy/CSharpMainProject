using System.Collections.Generic;
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

        private const float SwitchStateDurationSec = 1f;
        private UnitState _currentState = UnitState.Moving;
        private UnitState _nextState = UnitState.Moving;
        private float _switchStartTime = 0f;

        public override void Update(float deltaTime, float time)
        {
            base.Update(deltaTime, time);

            if (_currentState == UnitState.Switching)
            {
                if (time - _switchStartTime >= SwitchStateDurationSec)
                {
                    _currentState = _nextState;
                }
                return;
            }

            bool hasTargets = HasTargetsInRange();

            if (_currentState == UnitState.Moving && hasTargets)
            {
                StartSwitching(UnitState.Attacking, time);
            }
            else if (_currentState == UnitState.Attacking && !hasTargets)
            {
                StartSwitching(UnitState.Moving, time);
            }
        }

        public override Vector2Int GetNextStep()
        {
            return _currentState != UnitState.Moving
                ? unit.Pos
                : base.GetNextStep();
        }

        protected override List<Vector2Int> SelectTargets()
        {
            return _currentState != UnitState.Attacking
                ? new List<Vector2Int>()
                : base.SelectTargets();
        }

        private void StartSwitching(UnitState newState, float currentTime)
        {
            _currentState = UnitState.Switching;
            _nextState = newState;
            _switchStartTime = currentTime;
        }
    }
}
