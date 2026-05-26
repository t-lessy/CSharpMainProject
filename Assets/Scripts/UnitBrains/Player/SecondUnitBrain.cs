using Model.Runtime;
using Model.Runtime.Projectiles;
using System.Collections.Generic;
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
            
            // Homework 1.3 (1st block, 3rd module)
                     
            //проверяем, не перегрето ли оружие
            if (GetTemperature() >= OverheatTemperature)
            {
                return; //оружие перегрелось, прерываем
            }

            // Определяем количество снарядов (минимум 1)
            int projectileCount = Mathf.Max(1, Mathf.FloorToInt(GetTemperature() + 1));

            //Создание нужного количества снарядов
            for (int i = 0; i < projectileCount; i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }

            // Увеличиваем температуру
            IncreaseTemperature();

            // Проверяем перегрев после выстрела
            if (GetTemperature() >= OverheatTemperature)
            {
                _overheated = true;
                _cooldownTime = Time.time + OverheatCooldown;
            }
        }

        public override Vector2Int GetNextStep()
        {
            return base.GetNextStep();
        }

        protected override List<Vector2Int> SelectTargets()
        {
            
            // Homework 1.4 (1st block, 4rd module)
            
            // Получаем список всех целей, до которых может достать юнит
            List<Vector2Int> result = GetReachableTargets();

            // Пока в списке больше 1 цели - удаляем последнюю
            while (result.Count > 1)
            {
                result.RemoveAt(result.Count - 1); // Удаляем элемент с последним индексом
            }

            // Возвращаем список (в котором остался максимум 1 элемент)
            return result;

            // Находим ближайшую к нашей базе цель
            Vector2Int closestTarget = result[0];
            float closestDistance = DistanceToOwnBase(closestTarget);

            // Перебираем всех остальных врагов
            for (int i = 1; i < result.Count; i++)
            {
                Vector2Int target = result[i];
                float distance = DistanceToOwnBase(target);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = target;
                }
            }

            // Очищаем список и добавляем только ближайшую цель
            result.Clear();
            result.Add(closestTarget);

            return result;

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