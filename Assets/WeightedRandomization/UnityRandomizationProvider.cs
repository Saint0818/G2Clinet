using UnityEngine;

namespace WeightedRandomization
{
    public class UnityRandomizationProvider : IRandomizationProvider
    {        
        public static IRandomizationProvider Default { get { return new UnityRandomizationProvider(); } }        

        public double NextRandomValue()
        {
            return Random.Range(0f, 1f); 
        }
    }
}