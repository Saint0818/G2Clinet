using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WeightedRandomization
{
    /// <summary>
    /// <para>這是 Asset Store 上找到的亂數器. 使用概念是先給亂數器某個符號出現的機率, 
    /// 符號可以是 enum, int, class 等等, 機率是百分比. </para>
    /// 
    /// <para> 使用者自己必須要保證全部符號的權重總和必須是 100%, 否則機率會有錯誤. </para>
    /// 
    /// <para> 我個人不建議使用這個, 因為使用者必須要保證權重總和是 100%, 建議改用 AI.WeightedRandomizer,
    /// 就不用保證權重總和要 100%. </para>
    /// </summary>
    /// <typeparam name="T"> int, enum, class </typeparam>
    /// <remarks>
    /// 使用方法:
    /// <list type="number">
    /// <item> new instance. </item>
    /// <item> Call AddOrUpdateWeight() 加入亂數符號. </item>
    /// <item> Call GetNext() 取出亂數符號. </item>
    /// </list>
    /// </remarks>
    public class WeightedRandomizer<T> 
    {
        public IRandomizationProvider Provider { get; set; }

        private bool mAdjusted;
        private List<WeightedChance<T>> mWeights = new List<WeightedChance<T>>();

        private readonly Pool<WeightedChance<T>> mPool;

        public WeightedRandomizer()
        {
            mPool = new Pool<WeightedChance<T>>(() => new WeightedChance<T>(), 
                                                weightChance => weightChance.Clear());
        }

        /// <summary>
        /// 設定某個符號出現的機率.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="weight"> 出現機率, 這必須是 (0, 1] 範圍的數值. </param>
        public void AddOrUpdateWeight(T symbol, float weight)
        {
            if(weight < 0 || weight > 1)
            {
                Debug.LogWarningFormat("weighted must be [0, 1]. Symbol:{0}, Weight:{1}", symbol, weight);
                return;
            }

            if(Math.Abs(weight) < float.Epsilon) // weight == 0
                return;

            WeightedChance<T> existing = mWeights.FirstOrDefault(x => Equals(x.Symbol, symbol));
            if(existing == null)
            {
                var weightChance = mPool.CreateOrGet();
                if(weightChance == null)
                {
                    Debug.LogErrorFormat("Pool is full, ignore Symbol:{0}, Value:{1}", symbol, weight);
                    return;
                }
                weightChance.Symbol = symbol;
                weightChance.Weight = weight;
                mWeights.Add(weightChance);
            }                            
            else            
                existing.Weight = weight;

            mAdjusted = false; 
        }

        public bool IsEmpty()
        {
            return mWeights.Count == 0;
        }

        public void Remove(T value)
        {
            WeightedChance<T> existing = mWeights.FirstOrDefault(x => Equals(x.Symbol, value));
            if(existing != null)
            {
                mPool.Free(existing);
                mWeights.Remove(existing);
                mAdjusted = false;
            }
        }

        public void Clear()
        {
            for(int i = 0; i < mWeights.Count; i++)
            {
                mPool.Free(mWeights[i]);
            }
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

            if(weightSum <= 0.98m || weightSum >= 1.02m)
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
                return mWeights[mWeights.Count - 1].Symbol;
            }
            return item.Symbol;
        }
    }
}
