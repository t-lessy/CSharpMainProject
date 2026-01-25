using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Runtime
{
    // Ускорение движ-я
    public class MovementSpeedBuff : Buff
    {
        public MovementSpeedBuff(float duration, float modifier) : base(duration, modifier) { }

        public override string GetDescription()
        {
            return $"Movement Speed Buff: x{Modifier} для {Duration:F1}s";
        }
    }

    // Замедление движ-я
    public class MovementSlowDebuff : Buff
    {
        public MovementSlowDebuff(float duration, float modifier) : base(duration, modifier) { }

        public override string GetDescription()
        {
            return $"Movement Slow: x{Modifier} для {Duration:F1}s";
        }
    }

    // Ускорение атаки
    public class AttackSpeedBuff : Buff
    {
        public AttackSpeedBuff(float duration, float modifier) : base(duration, modifier) { }

        public override string GetDescription()
        {
            return $"Attack Speed Buff: x{Modifier} для {Duration:F1}s";
        }
    }

    // Замедление атаки
    public class AttackSlowDebuff : Buff
    {
        public AttackSlowDebuff(float duration, float modifier) : base(duration, modifier) { }

        public override string GetDescription()
        {
            return $"Attack Slow: x{Modifier} для {Duration:F1}s";
        }
    }
}