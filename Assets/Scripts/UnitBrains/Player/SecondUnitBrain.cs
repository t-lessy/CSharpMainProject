using Model;
using Model.Runtime.Projectiles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

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
            if (GetTemperature() >= overheatTemperature)
            {
                return;
            }
            for (int i = 0; i <= GetTemperature(); i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
            IncreaseTemperature();
        }

        public override Vector2Int GetNextStep()
        {
            List<Vector2Int> targets = SelectTargets();
            Vector2Int currentPosition = unit.Pos; //D1 Текущая позиция юнита из поля unit, подсмотрел из BaseUnitBrain

            if (targets.Count == 0) return currentPosition;

            Vector2Int target = targets[0];

            if (IsTargetInRange(target)) return currentPosition; //D2 метод IsTargetInRange также подсмотрел в BaseUnitBrain

            return currentPosition.CalcNextStepTowards(target); //E - сли цель вне зоны атаки - метод CalcNextStepTowards
        }

        // Вспомогательный метод для расчёта расстояния (если нет доступного в проекте)
        private float Distance(Vector2Int a, Vector2Int b)
        {
            return Vector2Int.Distance(a, b);
        }


        Vector2Int ClosestUnreachableTarget; //Шаг В.2 - создание новой переменной для хранения целей, к которым нужно идти, но которые вне зоны досягаемости.
        protected override List<Vector2Int> SelectTargets()
        {
            List<Vector2Int> AllTargets = GetAllTargets().ToList(); //Шаг А. Замена достижиых целей на все цели. List<Vector2Int> result = GetReachableTargets();

            if (AllTargets.Count == 0)
            {
                int enemyPlayerId = IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId; //С1 Получаем айди противника
                Vector2Int enemyBasePosition = runtimeModel.RoMap.Bases[enemyPlayerId]; //С2 Получаем координаты базы противника
                return new List<Vector2Int> { enemyBasePosition };
            }

            List<Vector2Int> ReachableTargets = GetReachableTargets(); // Шаг В.1 Создаем список всех достижимых целей для дальнеших проверок.
            List<Vector2Int> result = new List<Vector2Int>();

            Vector2Int minTarget = AllTargets[0];
            float minDistance = DistanceToOwnBase(minTarget);

            foreach (Vector2Int target in AllTargets)
            {
                float distance= DistanceToOwnBase(target);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minTarget = target;
                    }
            }

            if (ReachableTargets.Contains(minTarget)) //Проверка на достижиомость цели
            {
                result.Add(minTarget); 
            }

            else
            {
                ClosestUnreachableTarget = minTarget; //В противном случае записываем в переменную как самую опасную цель
            }

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