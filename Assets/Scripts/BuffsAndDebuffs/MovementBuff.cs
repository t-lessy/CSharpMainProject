using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Runtime;

namespace Assets.Scripts.BuffsAndDebuffs
{
    public class MovementBuff : Effect
    {
        public MovementBuff(Unit _unit) : base(_unit, EffectType.Move)
        {
            Modifier = 2f;
            EffectDuration = 30f;
        }
    }
}
