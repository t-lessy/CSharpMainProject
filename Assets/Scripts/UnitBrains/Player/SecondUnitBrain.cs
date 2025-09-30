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

        private static int _counter = 0;
        private int _unitNumber = 0;
        private const int maxTargetNumber = 3;

        private List<Vector2Int> _unreachableTargets = new();

        public SecondUnitBrain()
        {
            _unitNumber = _counter++;
        }

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////
            if(GetTemperature() >= overheatTemperature) return;
            for(int i = 0; i <= GetTemperature(); i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
            IncreaseTemperature();
            ///////////////////////////////////////
        }

        public override Vector2Int GetNextStep()
        {
            if (_unreachableTargets.Count > 0)
            {
                Vector2Int target = _unreachableTargets[0];
                return unit.Pos.CalcNextStepTowards(target);
            }
            return unit.Pos;
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////


            List<Vector2Int> result = new List<Vector2Int>();
            List<Vector2Int> allTargets = GetAllTargets().ToList();
            _unreachableTargets.Clear();

            if (!allTargets.Any())
            {
                var enemyBase = runtimeModel.RoMap.Bases[IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId];
                result.Add(enemyBase);
                return result;
            }

            SortByDistanceToOwnBase(allTargets);
            List<Vector2Int> limitedTargets = allTargets.Take(maxTargetNumber).ToList();
            int targetIndex = _unitNumber % limitedTargets.Count;

            Vector2Int selectedTargets = allTargets[targetIndex];

            if (IsTargetInRange(selectedTargets))
            {
                result.Add(selectedTargets);
            }
            else
            {
                _unreachableTargets.Add(selectedTargets);
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