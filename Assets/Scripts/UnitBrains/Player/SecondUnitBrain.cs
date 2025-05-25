using Model;
using Model.Runtime.Projectiles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;
        private List<Vector2Int> _targetsToMove = new();

        private static int _unitCounter = 0;
        private readonly int _maxTargetUnit = 3;
        private readonly int _unitId;

        public SecondUnitBrain()
        {
            _unitId = _unitCounter++;
        }

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            if (GetTemperature() >= OverheatTemperature)
                return;

            IncreaseTemperature();

            for (int i = 0; i < GetTemperature(); i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
        }

        public override Vector2Int GetNextStep()
        {
            if (_targetsToMove.Count == 0 || HasTargetsInRange())
                return unit.Pos;

            return unit.Pos.CalcNextStepTowards(_targetsToMove.First());
        }

        protected override List<Vector2Int> SelectTargets()
        {
            var result = new List<Vector2Int>();
            var allTargets = GetAllTargets().ToList();
            var playerBase = runtimeModel.RoMap.Bases[RuntimeModel.PlayerId];
            var enemyBase = runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId];

            var mostDangerousTargets = allTargets
                .Where(target => target != playerBase)
                .ToList();

            SortByDistanceToOwnBase(mostDangerousTargets);

            if (mostDangerousTargets.Count > 0)
            {
                var availableTargets = mostDangerousTargets.Take(_maxTargetUnit).ToList();
                var targetIndex = _unitId % availableTargets.Count;
                var selectedTarget = availableTargets[targetIndex];

                var targetsToCheck = availableTargets
                    .OrderBy(t => t == selectedTarget ? 0 : 1)
                    .ToList();

                foreach (var target in targetsToCheck)
                {
                    if (IsTargetInRange(target))
                    {
                        _targetsToMove.Clear();
                        result.Add(target);
                        break;
                    }
                }


                if (result.Count == 0)
                {
                    _targetsToMove = new List<Vector2Int> { selectedTarget };
                }
            }
            else
            {
                if (IsTargetInRange(enemyBase))
                    result.Add(enemyBase);
                else
                    _targetsToMove = new List<Vector2Int> { enemyBase };
            }

            return result;
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {              
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown/10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}