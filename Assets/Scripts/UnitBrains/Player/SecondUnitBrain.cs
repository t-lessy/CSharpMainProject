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
            return base.GetNextStep();
        }

        protected override List<Vector2Int> SelectTargets()
        {
            List<Vector2Int> result = GetReachableTargets();
            if (result.Count == 0)
            {
                return result; //Список возвращается если не будет целей
            }
            Vector2Int minTarget = result[0]; //Беру первое попавшееся значение из листа result за эталон
            float minDistance = DistanceToOwnBase(minTarget); //Беру в качестве эталона расстояние до базы от первой же цели

            foreach (Vector2Int target in result)//Цикл для каждой цели target из листа result:
            {
                float distance= DistanceToOwnBase(target);//Берётся значение каждой из цели до базы
                if (distance < minDistance) //если расстояние distance до базы меньше эталонного
                {
                    minDistance=distance; //присваивается новое миниальное расстояние
                    minTarget=target; //берется ближайшая цель на основе расстояния до базы
                }
            }
            result.Clear(); 
            result.Add(minTarget); // Очищаем весь лист result и добавляем в него только ближайшую
            return result; // Возврат итогового списка

            //while (result.Count > 1)
            //{
            //    result.RemoveAt(result.Count - 1);
            //}
            //return result;
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