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
        private int Unit;
        private static int UnitPlus = 0;
        private const int MaxUnit = 2;
        private Vector2Int Arack;

        public SecondUnitBrain()
        {
            Unit = UnitPlus;
            UnitPlus++;
        }

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            float temperature = GetTemperature();
            if (temperature >= overheatTemperature)
            {
                return;
            }
            for (int i = 0; i < temperature; i++)  // исправлено: i < temperature, чтобы цикл был корректным
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }

            IncreaseTemperature();
        }

        public override Vector2Int GetNextStep()
        {
            if (Arack == default)
                return unit.Pos;
            if (GetReachableTargets().Contains(Arack))
                return unit.Pos;

            return unit.Pos.CalcNextStepTowards(Arack);
        }

        protected override List<Vector2Int> SelectTargets()
        {
            var result = GetReachableTargets();
            var allTargets = GetAllTargets().ToList();

            if (allTargets.Any())
            {
                var sorted = allTargets.OrderBy(DistanceToOwnBase).ToList();
                int res = Unit % MaxUnit;
                Arack = res < sorted.Count ? sorted[res] : sorted.Last();
            }
            else
            {
                Arack = runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId];
            }

            if (IsTargetInRange(Arack))
            {
                result.Add(Arack);
            }
            return result;
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {
                _cooldownTime += deltaTime;
                float t = _cooldownTime / OverheatCooldown;
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                    _temperature = 0f;
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
            if (_temperature >= OverheatTemperature)
                _overheated = true;
        }
    }
}

//дите, чтобы методы, которые переопределяете, существовал