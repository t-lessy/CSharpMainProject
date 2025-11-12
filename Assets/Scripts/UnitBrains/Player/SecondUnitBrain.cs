using GluonGui.Dialog;
using Model.Runtime;
using Model.Runtime.Projectiles;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Utilities;
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

        private Vector2Int priorityTarget; 
        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            if (GetTemperature() < overheatTemperature)
            {
                IncreaseTemperature();
            }
            else
            {
                return;
            }

            for (int i = 0; i < GetTemperature(); i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
        }

        public override Vector2Int GetNextStep()
        {
            if (priorityTarget == Vector2Int.zero || GetReachableTargets().Contains(priorityTarget))
            {
                return base.GetNextStep();
            }
            else
            {
                Vector2Int currentPosition = base.GetNextStep();
                return currentPosition.CalcNextStepTowards(priorityTarget);
            }
        }

        protected override List<Vector2Int> SelectTargets()
        {
            var allTargets = GetAllTargets();
            List<Vector2Int> result = new List<Vector2Int>();

            if (!allTargets.Any())
            {
                var allBases = runtimeModel.RoMap.Bases;

                foreach (var baseObj in allBases)
                {
                    if (!IsPlayerUnitBrain)
                    {
                        result.Add(baseObj);
                        break;
                    }
                }
            }

            float minDistance = float.MaxValue;
            foreach (Vector2Int target in allTargets)
            {
                float distance = DistanceToOwnBase(target);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    priorityTarget = target;
                }

            }

            List<Vector2Int> results = GetReachableTargets();

            if (results.Contains(priorityTarget))
            {
                result.Add(priorityTarget);
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