namespace WeightedRandomization
{
    public class SystemRandomizationProvider : IRandomizationProvider
    {
        private readonly System.Random rand = new System.Random();

        public static IRandomizationProvider Default { get { return new SystemRandomizationProvider(); } }

        public double NextRandomValue()
        {
            return rand.NextDouble(); 
        }
    }
}
