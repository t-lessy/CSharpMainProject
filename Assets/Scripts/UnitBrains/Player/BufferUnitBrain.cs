using BuffSystem;
using Model.Runtime.Projectiles;
using Model.Runtime.ReadOnly;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace UnitBrains.Player
{
    public class BufferUnit : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Buffer";
        private bool _Pause = false;
        private float _Pause_HalfASecond = 0.5f;
        private float _Pause_FixedTime = 0f;

        protected void Pause()
        {
            _Pause = true;
            _Pause_FixedTime = Time.time;
        }

        public override void Update(float deltaTime, float time)
        {
            if (_Pause)
            {
                if ((Time.time - _Pause_FixedTime) >= _Pause_HalfASecond)
                {
                    _Pause = false;
                }
                else
                {
                    return;
                }
            }
            var targetToBuff = this.unit.FindUnitByPosition(SelectTargetToBuff()[0]);
            if (targetToBuff != null && _Pause != true)
            {
                this.unit._buffManager.AddBuff(targetToBuff, BuffType.MoveSpeed, 10f, -0.1f);
                this.unit._buffManager.AddBuff(targetToBuff, BuffType.AttackSpeed, 10f, -0.1f);
                Pause();
            }
        }

        public override Vector2Int GetNextStep()
        {
            if (_Pause)
            {
                return unit.Pos;
            }
            return base.GetNextStep();
        }

        protected List<Vector2Int> SelectTargetToBuff()
        {
            var alliedUnits = GetUnitsInRadius(this.unit.Config.AttackRange, true);
            if (alliedUnits.Count > 0)
            {
                var sortedUnits = alliedUnits
                .OrderBy(u => Vector2Int.Distance(u.Pos, this.unit.Pos))
                .ToList();

                foreach (var unit in sortedUnits)
                {
                    var target = this.unit.FindUnitByPosition(unit.Pos);
                    if (this.unit._buffManager.HasAnyBuff(target) == false)
                    {
                        return new List<Vector2Int> { unit.Pos };
                    }
                }
            }
            return null;
        }

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            //AddProjectileToList(CreateProjectile(forTarget), intoList); убрали возможность атаковать
        }
    }
}