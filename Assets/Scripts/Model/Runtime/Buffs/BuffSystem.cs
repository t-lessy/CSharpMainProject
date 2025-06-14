using Model.Runtime;
using System.Collections.Generic;
#if UNITY_EDITOR
using System.Linq;
#endif
using UnityEngine;
using Utilities;

namespace Assets.Scripts.Model.Runtime.Buffs
{
    public class BuffSystem
    {
        private readonly TimeUtil _timeUtil;
        private readonly Dictionary<Unit, List<Buff>> _buffs = new();
        private bool _running;

        public BuffSystem(TimeUtil timeUtil)
        {
            _timeUtil = timeUtil;
            _timeUtil.AddFixedUpdateAction(UpdateAll);
        }


        public void AddBuff(Unit unit, Buff buff)
        {
            if (!_buffs.TryGetValue(unit, out var list))
                _buffs[unit] = list = new List<Buff>();
            list.Add(buff);
        }


        public float GetMoveModifier(Unit unit)
        {
            if (!_buffs.TryGetValue(unit, out var list))
                return 1f;

            float net = list.OfType<HasteMovementBuff>().Sum(b => b.Modifier - 1f)
                      - list.OfType<SlowMovementDebuff>().Sum(b => 1f - b.Modifier);
            return Mathf.Max(0f, 1f + net);
        }

        public float GetAttackModifier(Unit unit)
        {
            if (!_buffs.TryGetValue(unit, out var list))
                return 1f;

            float net = list.OfType<HasteAttackBuff>().Sum(b => b.Modifier - 1f)
                      - list.OfType<SlowAttackDebuff>().Sum(b => 1f - b.Modifier);
            return Mathf.Max(0f, 1f + net);
        }

        private void UpdateAll(float deltaTime)
        {
            foreach (var key in _buffs.Keys.ToList())
            {
                var list = _buffs[key];
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i].Tick(deltaTime))
                        list.RemoveAt(i);
                }
                if (list.Count == 0)
                    _buffs.Remove(key);
            }
        }

        public void StartProcessing()
        {
            if (_running) return;
            _timeUtil.AddFixedUpdateAction(UpdateAll);
            _running = true;
        }

        public void StopProcessing()
        {
            if (!_running) return;
            _timeUtil.RemoveFixedUpdateAction(UpdateAll);
            _running = false;
        }
#if UNITY_EDITOR
        /// Возвращает инфо о ближайшем Haste-баффе на юните
        public (bool has, float timeLeft, float multiplier) GetHasteAttackInfo(Unit u)
        {
            if (_buffs.TryGetValue(u, out var list))
            {
                var haste = list.OfType<HasteAttackBuff>().FirstOrDefault();
                if (haste != null)
                    return (true, haste.Duration, haste.Modifier);
            }
            return (false, 0f, 1f);
        }
#endif
    }
}