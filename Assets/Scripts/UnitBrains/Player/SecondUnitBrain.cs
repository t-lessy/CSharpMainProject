using System.Collections.Generic;
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
        public List<Vector2Int> result { get; set; }

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////
            if (GetTemperature()<OverheatTemperature)
            {
                for (int i = 0; i <= GetTemperature(); i++)
                {
                    var projectile = CreateProjectile(forTarget);
                    AddProjectileToList(projectile, intoList);
                }
                IncreaseTemperature(); 
             
            }
        }
                ///////////////////////////////////////

        public override Vector2Int GetNextStep()
        {
            if (result.Count > 0)
            {
                Vector2Int currentTarget = result[0];
                if (IsItPossibleToShootTarget(currentTarget) == false)
                {
                    return unit.Pos.CalcNextStepTowards(currentTarget);
                }
                
            }
            return base.GetNextStep();
        }
        
        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            result = GetAllTargets().ToList();
            List<Vector2Int> UnattainableTargets = new List<Vector2Int>();
            List<Vector2Int> ReachableTargets = GetReachableTargets();
            float lowestDistance = float.MaxValue;
            Vector2Int nearestTarget = new Vector2Int();
            foreach (var target in result)
            {
                if (DistanceToOwnBase(target) < lowestDistance)
                {
                    lowestDistance = DistanceToOwnBase(target);
                    nearestTarget = target;
                }
            }
            if (result.Count > 0)
            {
                if (IsItPossibleToShootTarget(nearestTarget) == true)
                {
                    result.Clear();
                    result.Add(nearestTarget);
                    return result;
                }
                else
                {
                   GetNextStep();
                }
                
            }
            else
            {
                var baseEnemy = runtimeModel.RoMap.Bases[IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerID];
                result.Add(baseEnemy);
            }

            return result;
            
            ///////////////////////////////////////
        }

        public bool IsItPossibleToShootTarget(Vector2Int mostDangerousEnemy)
        {
            List<Vector2Int> rectTargets = GetReachableTargets();
            return rectTargets.Contains(mostDangerousEnemy);
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