using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model.Runtime.Buffs
{
    public class BuffSystem : MonoBehaviour
    {
        private Dictionary<Unit, List<Buff>> _buffs = new();

        public static BuffSystem Create()
        {
            var go = new GameObject("BuffSystem");
            DontDestroyOnLoad(go);
            return go.AddComponent<BuffSystem>();
        }

        public void AddBuffToUnit(Unit unit, Buff buff)
        {
            if (!_buffs.ContainsKey(unit))
                _buffs[unit] = new List<Buff>();

            _buffs[unit].Add(buff);
        }

        public List<Buff> GetActiveBuffs(Unit unit)
        {
            if (_buffs.ContainsKey(unit))
                return _buffs[unit];

            return new();
        }
        
        public void Clear()
        {
            _buffs = new();
        }

        private void Start() => StartCoroutine(ProcessBuffs(0.1f));

        private IEnumerator ProcessBuffs(float time)
        {
            while (true)
            {
                foreach (var unit in _buffs.Keys)
                {
                    List<Buff> toRemoveBuffs = new();
                    foreach (var buff in _buffs[unit])
                    {
                        buff.Duration -= time;
                        if (buff.Duration <= 0)
                            toRemoveBuffs.Add(buff);
                    }

                    // Remove expired buffs
                    foreach (var toRemoveBuff in toRemoveBuffs)
                        _buffs[unit].Remove(toRemoveBuff);
                }

                yield return new WaitForSeconds(time);
            }
        }
    }
}