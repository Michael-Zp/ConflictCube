namespace Engine.Inputs
{
    public struct AxisData
    {
        public string PositiveKey;
        public string NegativeKey;

        public AxisData(string positiveKey, string negativeKey)
        {
            PositiveKey = positiveKey;
            NegativeKey = negativeKey;
        }
    }
}
