using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Runtime.Projectiles;
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
        private List<Vector2Int> futureTargets = new();

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////        

            // Implement Temperature Overheating
            int curentTemperature = GetTemperature();
            if (curentTemperature >= overheatTemperature)
            {
                return;
            }
            IncreaseTemperature();

            // Implement Projectile Boost
            for (int projectileBoostIndex = 0; projectileBoostIndex <= curentTemperature; projectileBoostIndex++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }

            ///////////////////////////////////////
        }

        public override Vector2Int GetNextStep()
        {
            if (this.futureTargets.Count > 0)
            {
                Vector2Int target = this.futureTargets[0];
                if (IsTargetInRange(target))
                {
                    return unit.Pos;
                }
                Vector2Int currentPosition = unit.Pos;
                return currentPosition.CalcNextStepTowards(target);
            }
            return unit.Pos;
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            List<Vector2Int> result = new();
            IEnumerable<Vector2Int> allTargets = GetAllTargets();
            this.futureTargets.Clear();

            if (allTargets.Count() > 0)
            {
                Vector2Int nearestToBaseTarget = allTargets.First();
                float distance = float.MaxValue;

                foreach (Vector2Int target in allTargets)
                {
                    float currentTargetDistance = DistanceToOwnBase(target);
                    if (currentTargetDistance < distance)
                    {
                        distance = currentTargetDistance;
                        nearestToBaseTarget = target;
                    }
                }
                this.futureTargets.Add(nearestToBaseTarget);
                bool isNearestToBaseTargetInRange = IsTargetInRange(nearestToBaseTarget);
                if (isNearestToBaseTargetInRange)
                {
                    result.Add(nearestToBaseTarget);
                }
            }
            else
            {
                Vector2Int enemyBase = runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId];
                this.futureTargets.Add(enemyBase);
                result.Add(enemyBase);
            }
            return result;
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown / 10);
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
            if (_overheated) return (int)OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}