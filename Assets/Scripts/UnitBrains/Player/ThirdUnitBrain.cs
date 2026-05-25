using System.Collections.Generic;
using UnityEngine;


namespace UnitBrains.Player
{
    public class ThirdUnitBrain : DefaultPlayerUnitBrain
    {
        private enum BehemothState
        {
            Moving,
            SwitchingToAttack,
            SwitchingToMove,
            Attacking
        }

        public override string TargetUnitName => "Ironclad Behemoth";
        private BehemothState _state = BehemothState.Moving;
        private float _switchTimer = 0f;
        private const float SwitchDuration = 1f;

        public override void Update(float deltaTime, float time)
        {
            base.Update(deltaTime, time);
            Debug.Log($"Behemoth Update: dt={deltaTime}, state={_state}, timer={_switchTimer}");
            switch (_state)
            {
                case BehemothState.Moving:
                    if (HasTargetsInRange())
                    {
                        _state = BehemothState.SwitchingToAttack;
                    }
                    break;

                case BehemothState.SwitchingToAttack:
                    _switchTimer += deltaTime;
                    if (_switchTimer >= SwitchDuration)
                    {
                        _state = BehemothState.Attacking;
                        _switchTimer = 0f;
                    }
                    break;

                case BehemothState.SwitchingToMove:
                    _switchTimer += deltaTime;
                    if (_switchTimer >= SwitchDuration)
                    { 
                        _state = BehemothState.Moving;
                        _switchTimer = 0f;
                    }
                    break;

                case BehemothState.Attacking:
                    if (!HasTargetsInRange())
                    {
                        _state = BehemothState.SwitchingToMove;
                    }
                    break;
            }
        }

        public override Vector2Int GetNextStep()
        {
            if (_state != BehemothState.Moving)
                return unit.Pos;

            return base.GetNextStep();
        }

        protected override List<Vector2Int> SelectTargets()
        {
            if (_state != BehemothState.Attacking)
                return new List<Vector2Int>();

            return base.SelectTargets();
        }
    }
}