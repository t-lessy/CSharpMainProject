using System;
using System.Collections.Generic;
using Model;
using Model.Runtime.Projectiles;
using UnityEngine;
using Utilities;
using static Codice.CM.Common.CmCallContext;

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

        private List<Vector2Int> unreachableTargets = new(); //недостижимые цели
        private Vector2Int? currentTarget; //текущая цель
        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            float temperature = GetTemperature();

            if (temperature >= overheatTemperature)
            {
                return;
            }

            for (int i = 0; i < temperature; i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }

            IncreaseTemperature();

        }

        public override Vector2Int GetNextStep()
        {
            Vector2Int position = Vector2Int.zero;
            Vector2Int nextPosition = Vector2Int.right;
            position = position.CalcNextStepTowards(nextPosition);

            if (currentTarget == null || GetReachableTargets().Contains(currentTarget.Value))
            {
                return position;
            }

            return position.CalcNextStepTowards(currentTarget.Value);
        }

        protected override List<Vector2Int> SelectTargets()
        {
            List<Vector2Int> allTargets = (List<Vector2Int>)GetAllTargets();
            List<Vector2Int> result = new();
            unreachableTargets.Clear();

            if (allTargets.Count > 0)
            {
                Vector2Int closestTarget = allTargets[0];
                int minDistance = (int)DistanceToOwnBase(closestTarget);

                foreach (var target in allTargets)
                {
                    int distance = (int)DistanceToOwnBase(target);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestTarget = target;
                    }
                }

                currentTarget = closestTarget;
                if (GetReachableTargets().Contains(closestTarget))
                {
                    result.Add(closestTarget);
                }
                else
                {
                    unreachableTargets.Add(closestTarget);
                }
            }
            else
            {
                int enemyId = IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId;
                Vector2Int enemyBase = runtimeModel.RoMap.Bases[enemyId];
                currentTarget = enemyBase;
                unreachableTargets.Add(enemyBase);
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
