using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Runtime.Projectiles;
using UnityEngine;
using UnityEngine.UIElements;
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

        List<Vector2Int> possibleTargets = new List<Vector2Int>();
        List<Vector2Int> result = new List<Vector2Int>();
        List<Vector2Int> unReachableTargets = new List<Vector2Int>();

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////
            ///

            float currentTemperature = GetTemperature();

            if (currentTemperature >= overheatTemperature) {
                return;
            }
            
            for (int i=0; i <= currentTemperature; i++ ) {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
            

            IncreaseTemperature();
            ///////////////////////////////////////
        }

        public override Vector2Int GetNextStep()
        {
            Vector2Int position = new Vector2Int();
            Vector2Int targetPosition = new Vector2Int();


            if (result.Count > 0) {

                position = unit.Pos;
            }
            else if (unReachableTargets.Count() > 0) {

                    targetPosition = unReachableTargets [0];
                    position = unit.Pos.CalcNextStepTowards(targetPosition); ;
            }
        
            return position;

        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            List<Vector2Int> possibleTargets = GetAllTargets().ToList();

            float closestDistance = float.MaxValue;
            Vector2Int active_target = new Vector2Int();
            
            unReachableTargets.Clear();
            result.Clear();

            foreach (Vector2Int target in possibleTargets) {

                float distance = DistanceToOwnBase(target);

                if (distance < closestDistance) {

                    closestDistance = distance;
                    active_target = target;
                    unReachableTargets.Add(active_target);

                    if (GetReachableTargets().Contains(active_target)) {
                        result.Add(active_target);
                    } 
                    
                }

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