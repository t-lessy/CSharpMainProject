namespace Utilities.BuffsService
{
    public interface IBuff <BaseUnitBrain>
    {
        bool CanApplyBuff(BaseUnitBrain unit);
        void ApplyBuff(BaseUnitBrain unit);
        void RemoveBuff(BaseUnitBrain unit);
    }
}