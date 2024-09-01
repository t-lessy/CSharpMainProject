using Model.Runtime;
using Model.Runtime.ReadOnly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.BuffsAndDebuffs
{
    public enum EffectType
    {
        Attack,
        Move
    }
    public abstract class Effect

    {
        private IReadOnlyUnit _effectOwner;
        private float _effectDuration;
        private float _modifier;
        private EffectType _effectType;

        public float EffectDuration { get { return _effectDuration; }  set { _effectDuration = value; } }
        public float Modifier { get { return _modifier; } set { _modifier = value; } }
        public EffectType EffectType { get { return _effectType; } }
        public Effect(IReadOnlyUnit unit, EffectType effectType)
        {
            _effectOwner = unit;
            _effectType = effectType;
        }
    }
}
