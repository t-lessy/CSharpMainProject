using System;
using UnityEngine;

namespace Assets.Scripts.Model.Runtime
{
    public abstract class Buff
    {
        public float Duration { get; set; }
        public float MoveSpeedModifier { get; protected set; } = 1f;
        public float AttackSpeedModifier { get; protected set; } = 1f;

        protected Buff(float duration)
        {
            if (duration <= 0f)
                throw new ArgumentException("Продолжительность должна быть больше, чем 0", nameof(duration));

            Duration = duration;
        }

        protected void ValidateMoveSpeedModifier(float modifier, string buffType)
        {
            if (modifier <= 0f)
                throw new ArgumentException($"{buffType}: Параметр MoveSpeedModifier должен быть больше, чем 0", nameof(modifier));
        }

        protected void ValidateAttackSpeedModifier(float modifier, string buffType)
        {
            if (modifier <= 0f)
                throw new ArgumentException($"{buffType}: Параметр AttackSpeedModifier должен быть больше, чем 0", nameof(modifier));
        }

    }

    public class MoveSpeedBuff : Buff
    {
        public MoveSpeedBuff(float duration, float modifier) : base(duration)
        {
            ValidateMoveSpeedModifier(modifier, nameof(MoveSpeedBuff));

            if (modifier < 1f)
                throw new ArgumentException("Параметр MoveSpeedBuff должен быть больше или равен 1.0", nameof(modifier));

            MoveSpeedModifier = modifier;
        }
    }

    public class AttackSpeedBuff : Buff
    {
        public AttackSpeedBuff(float duration, float modifier) : base(duration)
        {
            ValidateMoveSpeedModifier(modifier, nameof(AttackSpeedBuff));

            if (modifier < 1f)
                throw new ArgumentException("Параметр AttackSpeedBuff должен быть больше или равен 1.0", nameof(modifier));

            AttackSpeedModifier = modifier;
        }
    }

    public class MoveSlowDebuff : Buff
    {
        public MoveSlowDebuff(float duration, float modifier) : base(duration)
        {
            ValidateMoveSpeedModifier(modifier, nameof(MoveSlowDebuff));

            if (modifier > 1f)
                throw new ArgumentException("Параметр MoveSlowDebuff должен быть меньше или равен 1.0", nameof(modifier));

            MoveSpeedModifier = modifier;
        }
    }

    public class AttackSlowDebuff : Buff
    {
        public AttackSlowDebuff(float duration, float modifier) : base(duration)
        {
            ValidateMoveSpeedModifier(modifier, nameof(AttackSlowDebuff));

            if (modifier > 1f)
                throw new ArgumentException("Параметр AttackSlowDebuff должен быть меньше или равен 1.0", nameof(modifier));

            AttackSpeedModifier = modifier;
        }
    }    
}