using UnitBrains.Player;
using UnityEngine;

namespace Utilities.BuffsService
{
    public class DoubleShoot: IBuff<SecondUnitBrain>
    {
        public bool CanApplyBuff(SecondUnitBrain unitBrain)
        {
            
            return true;
        }

        public void ApplyBuff(SecondUnitBrain unitBrain)
        {
            unitBrain.EnableDoubleShoot();
        }

        public void RemoveBuff(SecondUnitBrain unitBrain)
        {
            Debug.Log($"{unitBrain}: Double shoot disabled.");
        }
    }
}