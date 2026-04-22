using Model;
using Utilities;

namespace UnitBrains
{
    public class ArmyBrain
    {
        private readonly TimeUtil _timeUtil;
        private readonly IReadOnlyRuntimeModel _runtimeModel;
        
        private static ArmyBrain _instance;
        private ArmyBrain() {
            _timeUtil = ServiceLocator.Get<TimeUtil>();
            _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();
        }

        public static ArmyBrain GetInstance()
        {
            if (_instance == null)
                _instance = new ArmyBrain();
        
            return _instance;
        }
    } 
}