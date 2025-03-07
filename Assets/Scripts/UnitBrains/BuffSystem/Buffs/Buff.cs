using System;
using System.Collections.Generic;
using Model.Runtime;
using UnityEngine;

namespace UnitBrains.BuffSystem
{
    public abstract class Buff<T>
    {
        public abstract string Name { get; }
        public float Duration { get; protected set; }
        public bool IsExpired => Duration <= 0;
        
        public Buff(float duration)
        {
            Duration = duration;
        }
        public abstract void Add(T unit);
        public abstract void Remove(T unit);
        public abstract bool CanApply(T unit);
        
        public void Update(float deltaTime)
        {
            if (Duration > 0)
            {
                Duration -= deltaTime;
            }
        }
    }
}