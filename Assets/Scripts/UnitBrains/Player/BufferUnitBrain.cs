using Assets.Scripts.UnitBrains;
using Assets.Scripts.UnitBrains.Buffs;
using Assets.Scripts.UnitBrains.Pathfinding;
using Codice.CM.Client.Differences.Merge;
using Model;
using Model.Runtime;
using Model.Runtime.Projectiles;
using PlasticGui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnitBrains.Pathfinding;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;
using View;
using static UnityEngine.GraphicsBuffer;

namespace UnitBrains.Player
{
    public class BufferUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Mystic Guardian";
        private bool _pause = false;
        private float _pauseTime = 0f;
        private readonly BuffSystem _buffSystem = ServiceLocator.Get<BuffSystem>();
        private readonly VFXView _vfxView = ServiceLocator.Get<VFXView>();
        public override BaseUnitPath ActivePath => _activePath;
        private BaseUnitPath _activePath = null;

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
        }

        public override Vector2Int GetNextStep()
        {
            var target = SelectTargets()[0];
            if (_pause || IsTargetInRange(target))
            {
                return unit.Pos;
            }
            else
            {
                _activePath = new AStarUnitPath(runtimeModel, unit.Pos, target);
                return _activePath.GetNextStepFrom(unit.Pos);
            }
        }
        public override void Update(float deltaTime, float time)
        {
            var target = SelectTargets()[0];
            if (!_pause && IsTargetInRange(target))
            {
                if (CanSetBuff(target))
                {
                    _pause = true;
                }
            }

            if (_pause)
            {
                _pauseTime += Time.deltaTime * 10;
                if (_pauseTime >= 0.5f)
                {
                    _pause = false;
                    _pauseTime = 0;
                }
            }

            base.Update(deltaTime, time);
        }
        private bool CanSetBuff(Vector2Int pos)
        {
            Unit targetUnit = (Unit)runtimeModel.RoPlayerUnits
                .FirstOrDefault(u => u.Pos == pos);

            if (targetUnit == null || targetUnit == unit)
            {
                return false;
            }

            _buffSystem.AddBuff(targetUnit, new AccelerationAttack());
            _vfxView.PlayVFX(targetUnit.Pos, VFXView.VFXType.BuffApplied);
            return true;

        }
        protected override List<Vector2Int> SelectTargets()
        {
            var allies = (List<Unit>)runtimeModel.RoPlayerUnits;

            var target = allies
                .Select(u => new
                {
                    Unit = u,
                    BuffDuration = _buffSystem.ContainsKey(u) ? _buffSystem.GetBuff(u).Duration : 0f,
                    HealthLoss = u.Config.MaxHealth - u.Health
                })
                .Where(u => u.Unit != unit)
                .OrderBy(x => x.BuffDuration)
                .ThenBy(x => x.HealthLoss)
                .FirstOrDefault();
            if (target != null)
                return new List<Vector2Int>() { target.Unit.Pos };
            else
                return new List<Vector2Int> { unit.Pos };
        }
        public string PlayerBuffsToString()
        {
            string output = "";
            int count = 0;
            foreach (Unit unit in runtimeModel.RoPlayerUnits)
            {
                if (_buffSystem.ContainsKey(unit))
                {
                    count++;
                    var buff = _buffSystem.GetBuff(unit);
                    output += $"{count}. Unit with pos {unit.Pos} has buff: duration {buff.Duration}, move {buff.MoveModifier}, attack {buff.AttackModifier}.\n";
                }
            }
            output = $"Total {count} Buffs.\n" + output;
            return output;
        }
    }
}