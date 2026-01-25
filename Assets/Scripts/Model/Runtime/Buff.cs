using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Runtime
{
    public abstract class Buff
    {
        // Оставшаяся длительность эффекта в секундах
        public float Duration { get; set; }

        // пример модифи\катора скорости/урона
        // 0.5f
        // 1.0f = нормальная скорость(без эффекта)
        // 2.0f
        public float Modifier { get; set; }

        protected Buff(float duration, float modifier)
        {
            Duration = duration;
            Modifier = modifier;
        }

        public abstract string GetDescription();
    }
}