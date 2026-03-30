using Model.Runtime.ReadOnly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Model.Runtime
{
    public class EffectSystem : MonoBehaviour
    {
       
        private readonly Dictionary<IReadOnlyUnit, List<GameEffect>> _effects = new();
        private readonly TimeUtil _timeUtil;

        public EffectSystem()
        {
            _timeUtil = ServiceLocator.Get<TimeUtil>();
            _timeUtil.AddUpdateAction(OnUpdate);
        }

        ~EffectSystem()
        {
            _timeUtil.RemoveUpdateAction(OnUpdate);
        }

        public void AddEffect(IReadOnlyUnit unit, GameEffect effect)
        {
            if (!_effects.ContainsKey(unit))
            {
                _effects[unit] = new List<GameEffect>();
            }
            _effects[unit].Add(effect);
        }

        public float GetMoveSpeedFactor(IReadOnlyUnit unit)
        {
            float factor = 1f;
            if (_effects.TryGetValue(unit, out var effects))
            {
                foreach (var effect in effects)
                {
                    if (effect is MovementEffect moveEffect)
                    {
                        factor *= moveEffect.SpeedMultiplier;
                    }
                }
            }
            return factor;
        }

        public float GetAttackSpeedFactor(IReadOnlyUnit unit)
        {
            float factor = 1f;
            if (_effects.TryGetValue(unit, out var effects))
            {
                foreach (var effect in effects)
                {
                    if (effect is AttackRateEffect attackEffect)
                    {
                        factor *= attackEffect.RateMultiplier;
                    }
                }
            }
            return factor;
        }

        private void OnUpdate(float deltaTime)
        {
            foreach (var unitEffects in _effects.Values)
            {
                for (int i = unitEffects.Count - 1; i >= 0; i--)
                {
                    var effect = unitEffects[i];
                    effect.TimeLeft -= deltaTime;
                    if (effect.TimeLeft <= 0)
                    {
                        unitEffects.RemoveAt(i);
                    }
                }
            }
        }
    }

    public abstract class GameEffect
    {
        public float TimeLeft { get; set; }

        public GameEffect(float duration)
        {
            TimeLeft = duration;
        }
    }

    public class MovementEffect : GameEffect
    {
        public float SpeedMultiplier { get; }

        public MovementEffect(float duration, float multiplier) : base(duration)
        {
            SpeedMultiplier = multiplier;
        }
    }

    public class AttackRateEffect : GameEffect
    {
        public float RateMultiplier { get; }

        public AttackRateEffect(float duration, float multiplier) : base(duration)
        {
            RateMultiplier = multiplier;
        }
    }
}
   