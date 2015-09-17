using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WeightedRandomization
{
    public class WeightedRandomizer<T>
    {
        public IRandomizationProvider Provider { get; set; }

        private bool mAdjusted;
        private List<WeightedChance<T>> mWeights = new List<WeightedChance<T>>();

        public void AddOrUpdateWeight(T value, float weight)
        {
            if(weight <= 0)
                throw new ArgumentException("weighted value cannot have a 0% chance.");

            if(weight > 1)
                throw new ArgumentException("weighted value cannot have a 101% chance.");

            WeightedChance<T> existing = mWeights.FirstOrDefault(x => Equals(x.Value, value));
            if (existing == null)            
                mWeights.Add(new WeightedChance<T> { Value = value, Weight = weight });                            
            else            
                existing.Weight = weight;

            mAdjusted = false; 
        }

        public void Remove(T value)
        {
            WeightedChance<T> existing = mWeights.FirstOrDefault(x => Equals(x.Value, value));
            if(existing != null)
            {
                mWeights.Remove(existing);
                mAdjusted = false;
            }
        }

        public void Clear()
        {
            mWeights.Clear();
            mAdjusted = false; 
        }
     
        /// <summary>
        /// Determines the adjusted weights for all items in the collection. 
        /// This will be called automatically if GetNext is called after there are changes
        /// to the weights collection. 
        /// </summary>
        public void CalculateAdjustedWeights()
        {
            var sorted = mWeights.OrderBy(x => x.Weight).ToList(); // ascending order.
            decimal weightSum = 0; 
            for (int i = 0; i < sorted.Count; i++)
            {                
                weightSum += (decimal)sorted[i].Weight;               
                if (i == 0)
                    sorted[i].AdjustedWeight = sorted[i].Weight;
                else
                    sorted[i].AdjustedWeight = sorted[i].Weight + sorted[i - 1].AdjustedWeight;                
            }

            if(weightSum != 1.0m)
//                throw new InvalidOperationException("The weights of all items must add up to 1.0 ");
                Debug.LogWarningFormat("The weights of all items don't equal 1.0, weightSum:{0}", weightSum);

            mWeights = mWeights.OrderBy(x => x.AdjustedWeight).ToList();            

            mAdjusted = true; 
        }

        /// <summary>
        /// Return a value based on the weights provided. 
        /// </summary>
        /// <returns></returns>
        public T GetNext()
        {            
            if(Provider == null)
                Provider = UnityRandomizationProvider.Default;

            if(!mAdjusted)
                CalculateAdjustedWeights();
                        
            double d = Provider.NextRandomValue();            
            var item = mWeights.FirstOrDefault(x => d <= x.AdjustedWeight);

            if(item == null)
            {
                Debug.LogWarning("Item is null");
                return mWeights[mWeights.Count - 1].Value;
            }
            return item.Value;
        }
    }
}
