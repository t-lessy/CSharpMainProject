using Model.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace Assets.Scripts.Model.Runtime.Buffs
{
    /// <summary>
    /// Сервис, который через TimeUtil.FixedUpdate тикает все баффы
    /// и выдаёт единые модификаторы для движения и атаки.
    /// </summary>
    public class BuffSystem
    {
        private readonly TimeUtil _timeUtil;
        private readonly Dictionary<Unit, List<Buff>> _buffs = new();
        private bool _running;

        public BuffSystem(TimeUtil timeUtil)
        {
            _timeUtil = timeUtil;
            // каждый FixedUpdate будет вызывать UpdateAll
            _timeUtil.AddFixedUpdateAction(UpdateAll);
        }

        /// <summary>Добавить бафф или дебафф конкретному юниту.</summary>
        public void AddBuff(Unit unit, Buff buff)
        {
            if (!_buffs.TryGetValue(unit, out var list))
                _buffs[unit] = list = new List<Buff>();
            list.Add(buff);
        }

        /// <summary>
        /// Итоговый модификатор скорости передвижения: 
        /// 1f = без изменений, >1f = ускорение, <1f = замедление.
        /// </summary>
        public float GetMoveModifier(Unit unit)
        {
            if (!_buffs.TryGetValue(unit, out var list))
                return 1f;

            // для каждого HasteMovementBuff берём (Modifier−1), 
            // для каждого SlowMovementDebuff — (1−Modifier), складываем и прибавляем к 1
            float net = list.OfType<HasteMovementBuff>().Sum(b => b.Modifier - 1f)
                      - list.OfType<SlowMovementDebuff>().Sum(b => 1f - b.Modifier);
            return Mathf.Max(0f, 1f + net);
        }

        /// <summary>
        /// Итоговый модификатор скорости атаки: 
        /// 1f = без изменений, >1f = ускорение, <1f = замедление.
        /// </summary>
        public float GetAttackModifier(Unit unit)
        {
            if (!_buffs.TryGetValue(unit, out var list))
                return 1f;

            float net = list.OfType<HasteAttackBuff>().Sum(b => b.Modifier - 1f)
                      - list.OfType<SlowAttackDebuff>().Sum(b => 1f - b.Modifier);
            return Mathf.Max(0f, 1f + net);
        }

        /// <summary>
        /// Уменьшаем Duration у всех баффов и удаляем истёкшие.
        /// </summary>
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
    }
}