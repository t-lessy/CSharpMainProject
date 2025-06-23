using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Model;


//using System.Diagnostics;
using Model.Runtime.Projectiles;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;
using static UnityEditor.PlayerSettings;
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

        private List<Vector2Int> unreachableTarget = new();
        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////
            ///

            if (GetTemperature() < overheatTemperature)
            {
                for (int i = 0; i < _temperature + 1; i++)
                {
                    var projectile = CreateProjectile(forTarget);
                    AddProjectileToList(projectile, intoList);
                }
                IncreaseTemperature();
            }
        }
            ///////////////////////////////////////
            ///

        public override Vector2Int GetNextStep()
        {


            
            List<Vector2Int> allTargets = SelectTargets();
            List<Vector2Int> result = SelectTargets();

            Vector2Int position = unit.Pos;
            Vector2Int Nextposition = Vector2Int.right + Vector2Int.up;

            if (result.Count != 0) //Если список достижимых целей не пуст стоим на месте
            {
                
                
                    Debug.Log("Цель в радиусе атаки");
                    return unit.Pos;
                


            }
            else if (result.Count == 0 && unreachableTarget.Count != 0) //Если список достижимых целей пуст,а недостижымых не пуст,то идем к недостижимым 
            {
                
                
                    Debug.Log("Целей в радиусе атаки нет,иду к ближайшей");
                    return position.CalcNextStepTowards(unreachableTarget[0]);
                
            }
            return position.CalcNextStepTowards(runtimeModel.RoMap.Bases[IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId]);




        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////\
            int enemyID = IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId;
            Vector2Int enemyBase = runtimeModel.RoMap.Bases[enemyID];
            
            List< Vector2Int > allTargets = GetAllTargets().ToList();
            List<Vector2Int> result = new();

            

            var MinimumDistanceToBase = float.MaxValue;

            if (allTargets.Count >= 1)
            {
                Vector2Int IntermediateVector = new Vector2Int { };
                foreach (var mainTarget in allTargets)
                {
                    var currentDistance = DistanceToOwnBase(mainTarget);
                    if (MinimumDistanceToBase > currentDistance)
                    {
                        MinimumDistanceToBase = DistanceToOwnBase(mainTarget);

                        IntermediateVector = mainTarget;
                    }
                }
                if (IsTargetInRange(IntermediateVector))
                {
                    
                    result.Add(IntermediateVector);
                }
                else
                {

                    unreachableTarget.Add(IntermediateVector);
                }
            }
            else 
            {

                unreachableTarget.Add(enemyBase);
            }
            return result;



            //unreachebleTarget.Clear();

            //var MinimumDistance = float.MaxValue;

            //if ( allTargets.Count > 1)
            //{
            //    Vector2Int IntermediateVector = new Vector2Int { };
            //    foreach (var mainTarget in allTargets)
            //    {
            //        if (MinimumDistance > DistanceToOwnBase(mainTarget))
            //        {
            //            MinimumDistance = DistanceToOwnBase(mainTarget);

            //            IntermediateVector = mainTarget;
            //        }
            //    }
            //    if (IsTargetInRange(IntermediateVector))
            //    {
            //        result.Clear();
            //        result.Add(IntermediateVector);
            //    }
            //    else
            //    {

            //        unreachebleTarget.Add(IntermediateVector);
            //    }   

            //}
            //else if(IsTargetInRange(enemyBase))
            //{

            //    result.Add(enemyBase);

            //}
            //else
            //{
            //    unreachebleTarget.Add(enemyBase);
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