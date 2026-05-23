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
        private List<Vector2Int> _targetsToChase = new List<Vector2Int>();


        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////           
            if (GetTemperature() >= overheatTemperature)
            {
                return;
            }

            int shotsCount = GetTemperature() + 1;

            for (int i = 0; i < shotsCount; i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }

            IncreaseTemperature();
            ///////////////////////////////////////
        }

        public override Vector2Int GetNextStep()
        {
            if (_targetsToChase.Count > 0)
            {
                return unit.Pos.CalcNextStepTowards(_targetsToChase[0]);
            }
            else
            {
                return unit.Pos;
            }
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 4
            ///////////////////////////////////////
            List<Vector2Int> result = GetReachableTargets();
            var allTargets = GetAllTargets();
            var enemyBase = runtimeModel.RoMap.Bases[IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId];

            List<Vector2Int> unreachableTargets = new List<Vector2Int>();

            float minDistance = float.MaxValue;
            Vector2Int nearestTarget = Vector2Int.zero;
            bool targetFound = false;


            foreach (Vector2Int target in allTargets)
            {
                float distance = DistanceToOwnBase(target);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestTarget = target;
                    targetFound = true;
                    
                }
            }

            result.Clear();
            _targetsToChase.Clear();

            if (targetFound)
            {
                if (IsTargetInRange(nearestTarget))
                    result.Add(nearestTarget);
                else
                    _targetsToChase.Add(nearestTarget);
            }
            else
            {
                if (IsTargetInRange(enemyBase))
                    result.Add(enemyBase);
                else
                    _targetsToChase.Add(enemyBase);
            }
                
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