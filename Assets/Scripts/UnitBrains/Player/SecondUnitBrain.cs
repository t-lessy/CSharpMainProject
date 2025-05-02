using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Model;


//using System.Diagnostics;
using Model.Runtime.Projectiles;
using UnityEditor.Graphs;
using UnityEngine;
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

        private List<Vector2Int> unreachebleTarget = new();
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

            List<Vector2Int> unreachebleTarget = SelectTargets();
            List<Vector2Int> allTargets = SelectTargets();
            List<Vector2Int> result = SelectTargets();

            Vector2Int position = unit.Pos;
            Vector2Int Nextposition = Vector2Int.right;

            //return base.GetNextStep();

            // Если нет достижимых целей вообще, идем к недостижимой цели
            if (result==null || result.Count==0)
            {
                Debug.Log(" Если нет достижимых целей вообще, идем к недостижимой цели");
                foreach(var target in unreachebleTarget)
                {
                    
                    return position.CalcNextStepTowards(target);

                }
               
            }
            // Если цели в рэнджэ атаки есть, остаемся на месте
            else if (result != null && result.Count > 0)
            {
                Debug.Log(" Если цели в рэнджэ атаки есть, остаемся на месте");
                return unit.Pos;
            }
         
            return position.CalcNextStepTowards(runtimeModel.RoMap.Bases[IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId]);



        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            List<Vector2Int> allTargets =  GetAllTargets().ToList();
            
            List<Vector2Int> result = new();
            

            unreachebleTarget.Clear();

            int enemyID = IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId;
            Vector2Int enemyBase = runtimeModel.RoMap.Bases[enemyID];

            var MinimumDistance = float.MaxValue;

            if ( allTargets.Count > 1)
            {
                Vector2Int IntermediateVector = new Vector2Int { };
                foreach (var mainTarget in allTargets)
                {
                    if (MinimumDistance > DistanceToOwnBase(mainTarget))
                    {
                        MinimumDistance = DistanceToOwnBase(mainTarget);
                        
                        IntermediateVector = mainTarget;
                    }
                }
                if (IsTargetInRange(IntermediateVector))
                {
                    result.Clear();
                    result.Add(IntermediateVector);
                }
                else
                {
       
                    unreachebleTarget.Add(IntermediateVector);
                }   
                
            }
            else if(IsTargetInRange(enemyBase))
            {
                
                result.Add(enemyBase);
               
            }
            else
            {
                unreachebleTarget.Add(enemyBase);
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