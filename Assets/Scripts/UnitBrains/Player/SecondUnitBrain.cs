using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Runtime.Projectiles;
using UnityEngine;

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
        
        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////
            if(GetTemperature() >= overheatTemperature) return;
            for(int i = 0; i <= GetTemperature(); i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
            IncreaseTemperature();
            ///////////////////////////////////////
        }

        public override Vector2Int GetNextStep()
        {
            return base.GetNextStep();
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////

            List<Vector2Int> targetsInRange = new();
            List<Vector2Int> targetsOutRange = new();

            List<Vector2Int> allTargets = GetAllTargets().ToList();
            allTargets.OrderBy(DistanceToOwnBase);

            var enemyBase = runtimeModel.RoMap.Bases[IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId];

            targetsInRange.Clear();
            targetsOutRange.Clear();

            if (allTargets.Any())
            {
                foreach (var target in allTargets)
                {
                    if (IsTargetInRange(target))
                    {
                        targetsInRange.Add(target);
                    }
                    else
                    {
                        targetsOutRange.Add(target);
                    }
                }
            }
            else
            {
                allTargets.Add(enemyBase);
            }

            List<Vector2Int> result = GetReachableTargets();
            float minDistanse = float.MaxValue;
            Vector2Int criticalTarget = Vector2Int.zero;
            if(minDistanse != float.MaxValue)
                result.Add(criticalTarget);
            return result;
            ///////////////////////////////////////
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