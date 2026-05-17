using System.Collections.Generic;
using Model.Runtime;
using Effects;
using UnityEngine;

namespace Controller
{
    public class BuffSystemController
    {
        // Для хранения баффов рекомендуется использовать словарь. 
		// Словарь юнит - бафф
		public Dictionary<Unit, List<Buff>> buffs = new Dictionary<Unit, List<Buff>>();

		public float GetMoveDelayModifier(Unit unit) 
        {
            return buffs[unit].moveDelayModifier;
        }

		public float GetAttackDelayModifier(Unit unit) 
        {
            return buffs[unit].attackDelayModifier;
        }

		public void AddBuff(Unit unit)
		{
			buffs.Add(unit, increaseSpeedBuff);
		}
    }
}