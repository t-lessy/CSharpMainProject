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

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float currentTemp = GetTemperature();

            if (currentTemp >= OverheatTemperature)
            {
                return;
            }

            IncreaseTemperature();

            int projectileCount = Mathf.Min((int)currentTemp + 1, 3);

            for (int i = 0; i < projectileCount; i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
        }

        public override Vector2Int GetNextStep()
        {
            return base.GetNextStep();
        }

        protected override List<Vector2Int> SelectTargets()
        {
            List<Vector2Int> allTargets = GetReachableTargets();
            List<Vector2Int> result = new List<Vector2Int>();

            if (allTargets.Count == 0)
                return result;



            Vector2Int closestToBase = result[0];
            float minDistanceToBase = DistanceToOwnBase(result[0]);

            for (int i = 1; i < result.Count; i++)
            {
                float distanceToBase = DistanceToOwnBase(result[i]);
                if (distanceToBase < minDistanceToBase)
                {
                    minDistanceToBase = distanceToBase;
                    closestToBase = result[i];
                }
            }

            result.Add(closestToBase);
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