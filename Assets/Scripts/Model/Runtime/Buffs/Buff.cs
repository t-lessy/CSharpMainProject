using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Model.Runtime.Buffs
{
    /// <summary>
    /// Базовый абстрактный бафф/дебафф: хранит оставшуюся продолжительность и множитель.
    /// В методе Tick уменьшается Duration и возвращается true, если эффект истёк.
    /// </summary>
    public abstract class Buff
    {
        public float Duration { get; private set; }
        public float Modifier { get; }

        protected Buff(float duration, float modifier)
        {
            Duration = duration;
            Modifier = modifier;
        }

        // Уменьшить Duration на deltaTime. 
        //Вернуть true, если Duration ≤ 0 (эффект завершился).
     
        public bool Tick(float deltaTime)
        {
            Duration -= deltaTime;
            return Duration <= 0f;
        }
    }
}