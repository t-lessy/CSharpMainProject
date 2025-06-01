using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Runtime.Projectiles;
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
        
        public List<Vector2Int> unReachableTarget = new List<Vector2Int>();
        public List<Vector2Int> resultForShoot = new List<Vector2Int>();
        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            /////////////////////////////////////////
            //// Homework 1.3 (1st block, 3rd module)
            /////////////////////////////////////////           
            var projectile = CreateProjectile(forTarget);
            AddProjectileToList(projectile, intoList);
            /////////////////////////////////////////

            //IncreaseTemperature();
            //if (!_overheated)
            //{

            //}

        }

        public override Vector2Int GetNextStep()
        {
           Vector2Int unitPos = unit.Pos;

            if (unReachableTarget.Count == 1)
            {
                Vector2Int currentTarget = unReachableTarget[0];
                if (!IsTargetInRange(currentTarget))
                {
                    return unitPos.CalcNextStepTowards(currentTarget);
                }

            }
            return base.GetNextStep();
        }

        protected override List<Vector2Int> SelectTargets()
        {

        //    protected override List<Vector2Int> SelectTargets()
        //{
        //    List<Vector2Int> result = GetReachableTargets();

        //    if (result.Count == 0)
        //    {
        //        return result; 
        //    }

        //    Vector2Int nearestEnemy = result[0];
        //    float nearestEnemyDistance = DistanceToOwnBase(nearestEnemy);

        //    foreach (var target in result)
        //    {
        //        float enemyDistance = DistanceToOwnBase(target);
        //        if (enemyDistance < nearestEnemyDistance)
        //        {
        //            nearestEnemy = target;
        //            nearestEnemyDistance = enemyDistance; 
        //        }
        //    }
        //    return new List<Vector2Int> { nearestEnemy };
        //}

        /////////////////////////////////////////
        //// Homework 1.4 (1st block, 4rd module)
        /////////////////////////////////////////
        //List<Vector2Int> result = GetReachableTargets();
        //while (result.Count > 1)
        //{
        //    result.RemoveAt(result.Count - 1);
        //}
        //return result;
        /////////////////////////////////////////
        ///
            resultForShoot.Clear();
            List<Vector2Int> result = GetAllTargets().ToList();

            if (result.Count == 1)
            {
                var baseEnemy = runtimeModel.RoMap.Bases[
                    IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId];

                if (IsTargetInRange(baseEnemy))
                {
                    return ListSender(resultForShoot, baseEnemy);
                }
                else
                {
                    UnreachedListSender(baseEnemy);
                }

            }
            else
            {
                Vector2Int nearestEnemy = result[0];
                float nearestEnemyDistance = DistanceToOwnBase(nearestEnemy);
                foreach (var target in result)
                {
                    float enemyDistance = DistanceToOwnBase(target);
                    if (enemyDistance < nearestEnemyDistance)
                    {
                        nearestEnemy = target;
                        nearestEnemyDistance = enemyDistance;
                    }
                }
                if (IsTargetInRange(nearestEnemy))
                {
                    return ListSender(resultForShoot, nearestEnemy);
                }
                else
                {
                    UnreachedListSender(nearestEnemy);
                }

            }
            return resultForShoot;
        }

        private List<Vector2Int> ListSender(List<Vector2Int> shootList, Vector2Int target)
        {
            shootList.Clear();
            shootList.Add(target);
            return shootList;
        }
        private void UnreachedListSender(Vector2Int target)
        {
            unReachableTarget.Clear();
            unReachableTarget.Add(target);
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