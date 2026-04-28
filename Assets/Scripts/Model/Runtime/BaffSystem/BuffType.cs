using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuffSystem
{
    public enum BuffType
    {
        AttackSpeed,
        MoveSpeed
    }
    public class Buff
    {
        public BuffType Type { get; }
        public float Duration { get; private set; }
        public float Modifier { get; }

        public Buff(BuffType type, float duration, float modifier)
        {
            Type = type;
            Duration = duration;
            Modifier = modifier;
        }

        public void DecreaseDuration(float deltaTime)
        {
            Duration -= deltaTime;
        }

        public bool IsExpired() => Duration <= 0;
    }
}
