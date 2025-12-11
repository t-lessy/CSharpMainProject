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
        public readonly List<Vector2Int> _currTarget = new();

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
            if (_currTarget.Count > 0)
            {
                Vector2Int Target = _currTarget[0];
            }
            else
            {
                Target = unit.Pos;
            }
            return IsItPossibleToShootTarget(Target) ? unit.Pos : CalcNextStepTowards(Target);
        }
        
        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            var result = new List<Vector2Int>();
            float lowestDistance = float.MaxValue;
            Vector2Int nearestTarget = Vector2Int.zero;
            foreach (var target in GetAllTargets())
            {
                if (DistanceToOwnBase(target) < lowestDistance)
                {
                    lowestDistance = DistanceToOwnBase(target);
                    nearestTarget = target;
                }
            }
            _currTarget.Clear();
            if (lowestDistance < float.MaxValue)
            {
                _currTarget.Add(nearestTarget);
                if (IsItPossibleToShootTarget(nearestTarget) == true)
                {
                    result.Add(nearestTarget);
                }
                
            }
            else
            {
                result.Add(runtimeModel.RoMap.Bases[IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerID]);
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