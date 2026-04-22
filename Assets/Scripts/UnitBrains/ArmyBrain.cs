namespace UnitBrains
{
    public class ArmyBrain
    {
        private static ArmyBrain _instance;
        private ArmyBrain() {}

        public static ArmyBrain GetInstance()
        {
            if (_instance == null)
                _instance = new ArmyBrain();
        
            return _instance;
        }
    } 
}