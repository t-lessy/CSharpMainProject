using System.Collections.Generic;
using Model.Runtime.Projectiles;
using UnityEngine;
using System.Linq;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 4f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;
        List<Vector2Int> result = new List<Vector2Int>(); //цели в зоне досягаемости 
        List<Vector2Int> outOfRangeTargets = new List<Vector2Int>(); //цели вне зоне досягаемости 

        private static int Counter = 0; // счетчик 
        private int UnitNumber; // номер юнита 
        private const int MaxTargets = 3;
        private bool isInitialized = false;


        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;

            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////           

            //Если нынешняя температура больше или равна темпераратуры перегрева -> Прерываем метод
            if (GetTemperature() >= overheatTemperature)
                return;

            for (int i = 0; i < GetTemperature(); i++)
            {
                // Создаем снаряд
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }

            IncreaseTemperature(); // повышаем температуру до перегрева

            ///////////////////////////////////////
        }
        public override Vector2Int GetNextStep()
        {
            // Проверка, есть ли цели в зоне досягаемости 
            if (result.Count > 0)
            {
                // Берем первую цель из result
                Vector2Int targetPosInRange = result.First();

                // Если цель в пределах области атаки
                if (IsTargetInRange(targetPosInRange))
                {
                    return unit.Pos; // стоим и атакуем(так как цель в пределах досягаемости)
                }
                else
                {
                    // Если цель не в пределах области атаки, двигаемся к ней чтобы атаковать 
                    return CalcNextStepTowards(unit.Pos, targetPosInRange);
                }
            }
            // Если есть цели в зоне не досягаемости
            else if (outOfRangeTargets.Count > 0)
            {
                // Берем первую цель из outOfRangeTargets 
                Vector2Int targetPos = outOfRangeTargets.First();

                // Если цель не в пределах досягаемости, двигаемся к ней
                return CalcNextStepTowards(unit.Pos, targetPos);
            }

            // Если нет целей вообще, стоим на месте
            return unit.Pos;
        }

        protected override List<Vector2Int> SelectTargets()
        {
            if (!isInitialized)
            {
                UnitNumber = Counter;
                Debug.Log("Номер юнита: " + UnitNumber);
                Counter = (Counter + 1) % MaxTargets;
                isInitialized = true;
            }

            List<Vector2Int> allTargets = GetAllTargets().ToList();
            result.Clear(); // Очищаем список, чтобы вложить новую ближайшую  цель
            outOfRangeTargets.Clear();

            float minDistance = float.MaxValue;
            Vector2Int nearTarget = Vector2Int.zero; // коардинаты для ближайшей цели изначально задаем как (0,0)

            foreach (Vector2Int target in allTargets) // Проходим по всем существующим целям и ищем ближайшую
            {
                float distance = DistanceToOwnBase(target);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearTarget = target; // Устанавливаем ближайшую цель
                }
            }

            if (minDistance < float.MaxValue)
            {
                result.Add(nearTarget); //Добавлям в очищенный список новую цель
            }
            else
            {
                outOfRangeTargets.Add(nearTarget);
            }
            if (result.Count == 0) // если нет целей 
            {
                // Получаем базу противника
                Vector2Int enemyBase = runtimeModel.RoMap.Bases[IsPlayerUnitBrain ? Model.RuntimeModel.BotPlayerId : Model.RuntimeModel.PlayerId];
                result.Add(enemyBase);
            }
            // Сортируем цели по расстоянию до базы
            SortByDistanceToOwnBase(allTargets);
            // Выбираем цель для текущего юнита на основе его номера
            int targetIndex = UnitNumber % allTargets.Count;  // Остаток от деления на количество целей
            result.Add(allTargets[targetIndex]);  // Добавляем выбранную цель

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

        // Метод для расчета следующего шага
        private Vector2Int CalcNextStepTowards(Vector2Int currentPos, Vector2Int targetPos)
        {
            // Вычисляем разницу между текущей позицией и целевой
            Vector2Int direction = targetPos - currentPos;

            // Нормализуем направление (если расстояние > 0)
            if (direction.x != 0) direction.x = direction.x > 0 ? 1 : -1;
            if (direction.y != 0) direction.y = direction.y > 0 ? 1 : -1;

            // Возвращаем следующую позицию
            return currentPos + direction;
        }
    }
}