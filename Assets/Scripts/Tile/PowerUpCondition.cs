namespace Tile
{
    [System.Serializable]
    public class PowerUpCondition
    {
        public string name;
        public int conditionCount;


        public PowerUpCondition(string name, int conditionCount)
        {
            this.name = name;
            this.conditionCount = conditionCount;
        }

        public void SetCondition(string name, int conditionCount)
        {
            this.name = name;
            this.conditionCount = conditionCount;
        }

        public bool IsConditionMet(int cCount)
        {
            return cCount >= conditionCount;
        }
    }
}