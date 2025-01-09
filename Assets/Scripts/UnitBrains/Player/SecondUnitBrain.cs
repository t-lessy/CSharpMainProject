using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Runtime.Projectiles;
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

        private List<Vector2Int> _targetsOutsideReach = new List<Vector2Int>();

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            /////////////////////////////////////// 
            float temp = GetTemperature();
            if (temp >= overheatTemperature) return;
            else IncreaseTemperature();
            for (int i = 0; i <= temp; i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
            ///////////////////////////////////////
        }

        public override Vector2Int GetNextStep()
        {
            // Если есть цели вне зоны досягаемости, двигаемся к ближайшей
            if (_targetsOutsideReach.Count > 0)
            {
                Vector2Int target = _targetsOutsideReach[0];
                return unit.Pos.CalcNextStepTowards(target);
            }

            // Если целей нет или цель в зоне досягаемости, остаемся на месте
            return unit.Pos;
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////           
            List<Vector2Int> result = new List<Vector2Int>();
            _targetsOutsideReach.Clear();

            // Получаем все цели
            List<Vector2Int> allTargets = GetAllTargets().ToList();

            if (allTargets.Count > 0)
            {
                // Находим ближайшую цель к нашей базе
                Vector2Int mostDangerousTarget = allTargets[0];
                float minDistance = DistanceToOwnBase(mostDangerousTarget);

                foreach (Vector2Int target in allTargets)
                {
                    float distance = DistanceToOwnBase(target);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        mostDangerousTarget = target;
                    }
                }

                // Проверяем, находится ли цель в зоне досягаемости
                if (IsTargetInRange(mostDangerousTarget))
                {
                    result.Add(mostDangerousTarget);
                }
                else
                {
                    _targetsOutsideReach.Add(mostDangerousTarget);
                }
            }
            else
            {
                // Если целей нет, добавляем базу противника
                int enemyBaseId = IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId;
                Vector2Int enemyBasePosition = runtimeModel.RoMap.Bases[enemyBaseId];
                result.Add(enemyBasePosition);
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