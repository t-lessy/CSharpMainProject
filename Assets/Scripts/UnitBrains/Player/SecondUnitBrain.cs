using Model;
using Model.Runtime.Projectiles;
using PlasticPipe.PlasticProtocol.Messages;
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
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////
            if (GetTemperature() >= overheatTemperature)
            {
                return;
            }
            else
            {
                IncreaseTemperature();
                for (int i = 0; i<_temperature; i++)   
                {
                    var projectile = CreateProjectile(forTarget);
                    AddProjectileToList(projectile, intoList);
                }

            }

            ///////////////////////////////////////
        }

        List<Vector2Int> NextEnemyUnderDistance = new List<Vector2Int>();
        public override Vector2Int GetNextStep()
        {
            if (NextEnemyUnderDistance.Count > 0 )
            {
                var target = NextEnemyUnderDistance[0];
                return unit.Pos.CalcNextStepTowards(target);


            }
            else
            {
                return unit.Pos;
            }

            //return base.GetNextStep();

        }
        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            /*/
            Куча текста режeт глаза. 
            /*/

            List<Vector2Int> result = new List<Vector2Int>();

            Vector2Int Enemy = runtimeModel.RoMap.Bases[IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId];

            IEnumerable<Vector2Int> possibleTarget = GetAllTargets(); 
            Vector2Int NearEnemy = Enemy;

            if (possibleTarget.Count() != 0)
            {
                float NearestEnemyToOwnBase = float.MaxValue;

                foreach (var targets in possibleTarget)
                {
                    var DistanceEnemyToOwnBase = DistanceToOwnBase(targets);
                    if (DistanceEnemyToOwnBase < NearestEnemyToOwnBase)
                    {
                        NearestEnemyToOwnBase = DistanceEnemyToOwnBase;
                        NearEnemy = targets;
                    }
                }
                Enemy = NearEnemy;
            }
            if (GetReachableTargets().Contains(Enemy))
            {
                result.Add(Enemy);
            }
            else
            {
                NextEnemyUnderDistance.Clear();
                NextEnemyUnderDistance.Add(Enemy);
            }

            return result;

            /*/
            Гы. Я учусь на XYZ!!!!
            /*/
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