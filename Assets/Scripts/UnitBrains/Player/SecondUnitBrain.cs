using System.Collections.Generic;
using Model.Runtime.Projectiles;
using PlasticGui.WorkspaceWindow;
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
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)

            int currentTemperature = GetTemperature();
            if (currentTemperature < overheatTemperature)
            {
                for(int shot = 0; shot <= currentTemperature; shot++)
                {
                    var projectile = CreateProjectile(forTarget);
                    AddProjectileToList(projectile, intoList);
                }
                IncreaseTemperature();
            }
            else return;
            

            ///////////////////////////////////////           
            
            ///////////////////////////////////////
        }

        public override Vector2Int GetNextStep()
        {
            Vector2Int result = new Vector2Int();
            Vector2Int DangerousTargetOutOfRange = new Vector2Int();

            List <IEnumerable<Vector2Int>> ListOfAllTargets = (List<IEnumerable<Vector2Int>>)GetAllTargets();

            if(ListOfAllTargets.Count > 0)
            {
                float minDistance = float.MaxValue;
                Vector2Int closestTarget = new Vector2Int();

                foreach (var target in GetAllTargets())
                {
                    float distanceToBase = DistanceToOwnBase(target);

                    if (distanceToBase < minDistance)
                    {
                        minDistance = distanceToBase;
                        closestTarget = target;
                    }

                }
                if (IsTargetInRange(closestTarget)) 
                {
                    result = closestTarget;
                    return result;
                }
                else
                {
                    Vector2Int currentPosition = Vector2Int.zero;
                    DangerousTargetOutOfRange = closestTarget;

                    currentPosition = currentPosition.CalcNextStepTowards(DangerousTargetOutOfRange);
                    return DangerousTargetOutOfRange;
                 }

            }
            else
            {
                Vector2Int enemyBase = runtimeModel.RoMap.Bases[RuntimeModel.PlayerId];
                return enemyBase;
            }
           
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////

            float minDistance = float.MaxValue;
            Vector2Int closestTarget = new Vector2Int();

            List<Vector2Int> result = GetReachableTargets();

            foreach (Vector2Int target in result)
            {
                float distanceToBase = DistanceToOwnBase(target);

                if(distanceToBase < minDistance)
                {
                    minDistance = distanceToBase;
                    closestTarget = target;
                }

            }

            //Исправление ошибок (ДЗ 4)

            //result.Insert(0, closestTarget);

            ////Код, который был в ДЗ изначально (думала, что его нужно оставить)
            //while (result.Count > 1)
            //{
            //    result.RemoveAt(result.Count - 1);
            //}
            
            //Исправления в соответствии с комментарием проверяющего
            result.Clear();
            result.Add(closestTarget);
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