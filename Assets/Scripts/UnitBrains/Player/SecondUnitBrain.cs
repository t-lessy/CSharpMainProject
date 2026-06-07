using Model.Runtime.Projectiles;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

            if (GetTemperature() >= overheatTemperature)
                return;
            else
            {
                for (int a = 0; a <= GetTemperature(); ++a)
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
            return base.GetNextStep();
        }

        ///////////////////////////////////////
        // Homework 1.4 (1st block, 4rd module)
        ///////////////////////////////////////
        protected override List<Vector2Int> SelectTargets()
        {
            List<Vector2Int> result = GetReachableTargets();
            while (result.Count > 1)
            {
                Vector2Int ClosestTarget = result[0]; 
                var minDistance = DistanceToOwnBase(ClosestTarget);

                for (int i = 1; i < result.Count; i++)
                {
                    Vector2Int currentTarget = result[i];
                    var currentDistance = DistanceToOwnBase(currentTarget);

                    if (currentDistance < minDistance)
                    {
                        minDistance = currentDistance;
                        ClosestTarget = currentTarget;
                    }

                }
                    result.Clear();
                    result.Add(ClosestTarget);
            }
            return result;

        }



            ///////////////////////////////////////
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


