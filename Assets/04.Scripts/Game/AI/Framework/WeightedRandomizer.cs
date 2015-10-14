using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AI
{
    public class WeightedChance<T>
    {
        public T Symbol;
        public float Value;

        public void Clear()
        {
            Value = 0;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Symbol, Value);
        }
    }

    /// <summary>
    /// 可針對每個 Symbol 下不同權重的亂數產生器.
    /// </summary>
    /// <typeparam name="T"> enum, int, class. </typeparam>
    /// <remarks>
    /// 使用說明:
    /// <list type="number">
    /// <item> new instance. </item>
    /// <item> Call AddOrUpdate() 多次, 設定每個 Symbol 不同的權重. </item>
    /// <item> Call GetNext() 取出亂數 Symbol. </item>
    /// </list>
    /// </remarks>
    public class WeightedRandomizer<T>
    {
        private readonly WeightedRandomization.WeightedRandomizer<T> mRandomizer = new WeightedRandomization.WeightedRandomizer<T>();

        private readonly List<WeightedChance<T>> mWeights = new List<WeightedChance<T>>();

        private readonly Pool<WeightedChance<T>> mPool;

        public WeightedRandomizer()
        {
            mPool = new Pool<WeightedChance<T>>(() => new WeightedChance<T>(), 
                                                weightChance => weightChance.Clear());
        }

        private readonly StringBuilder mBuilder = new StringBuilder();
        public override string ToString()
        {
            mBuilder.Remove(0, mBuilder.Length);

            if(mWeights.Count == 0)
                return "Empty";

            for(int i = 0; i < mWeights.Count; i++)
            {
                mBuilder.AppendFormat("{0}, ", mWeights[i]);
            }
            return mBuilder.ToString();
        }

        public void AddOrUpdate(T symbol, float weight)
        {
            if(weight <= 0)
                throw new ArgumentException("weight cannot have a zero chance.");

            WeightedChance<T> existing = mWeights.FirstOrDefault(x => Equals(x.Value, weight));
            if(existing == null)
            {
                var weightChance = mPool.CreateOrGet();
                if(weightChance == null)
                {
                    Debug.LogErrorFormat("Pool is full, ignore Symbol:{0}, Value:{1}", symbol, weight);
                    return;
                }
                weightChance.Symbol = symbol;
                weightChance.Value = weight;
                mWeights.Add(weightChance);
            }
            else
                existing.Value = weight;

            updateWeights();
        }

        private void updateWeights()
        {
            float sum = mWeights.Sum(weight => weight.Value);

            foreach (var weight in mWeights)
            {
                mRandomizer.AddOrUpdateWeight(weight.Symbol, weight.Value/sum);
            }
        }

        public bool IsEmpty()
        {
            return mRandomizer.IsEmpty();
        }

        public T GetNext()
        {
            return mRandomizer.GetNext();
        }

        public void Remove(T symbol)
        {
            WeightedChance<T> existing = mWeights.FirstOrDefault(x => Equals(x.Symbol, symbol));
            if(existing != null)
            {
                mWeights.Remove(existing);
                mPool.Free(existing);
            }
            mRandomizer.Remove(symbol);
        }

        public void Clear()
        {
            for(int i = 0; i < mWeights.Count; i++)
            {
                mPool.Free(mWeights[i]);
            }
            mWeights.Clear();

            mRandomizer.Clear();
        }
    } // end of the class WeightedRandomizer.

    public class WeightedRandomizerProfiler
    {
        private enum EType
        {
            Shooting,
            Block,
            Pass,
            Steal
        }

        public static void RunTest1()
        {
            WeightedRandomizer<int> randomizer = new WeightedRandomizer<int>();
            randomizer.AddOrUpdate(100, 5);
            randomizer.AddOrUpdate(200, 20);
            randomizer.AddOrUpdate(300, 40);
            randomizer.AddOrUpdate(400, 35);
            int count100 = 0;
            int count200 = 0;
            int count300 = 0;
            int count400 = 0;
            for (int i = 0; i < 10000; i++)
            {
                var symbol = randomizer.GetNext();
                if (symbol == 100)
                    ++count100;
                else if (symbol == 200)
                    ++count200;
                else if (symbol == 300)
                    ++count300;
                else if (symbol == 400)
                    ++count400;
                else
                    Debug.LogWarningFormat("Unknown Symbol:{0}", symbol);
            }
            Debug.LogFormat("Count100:{0}, Count200:{1}, Count300:{2}, Count400:{3}", count100, count200, count300, count400);
        }

        public static void RunTest2()
        {
            WeightedRandomizer<EType> randomizer = new WeightedRandomizer<EType>();
            randomizer.AddOrUpdate(EType.Shooting, 65); // 65/160 = 0.40625
            randomizer.AddOrUpdate(EType.Block, 20); // 20/160 = 0.125
            randomizer.AddOrUpdate(EType.Pass, 40); // 40/160 = 0.25
            randomizer.AddOrUpdate(EType.Steal, 35); // 35/160 = 0.21875
            int countShooting = 0;
            int countBlock = 0;
            int countPass = 0;
            int countSteal = 0;
            for (int i = 0; i < 10000; i++)
            {
                var symbol = randomizer.GetNext();
                if (symbol == EType.Shooting)
                    ++countShooting;
                else if (symbol == EType.Block)
                    ++countBlock;
                else if (symbol == EType.Pass)
                    ++countPass;
                else if (symbol == EType.Steal)
                    ++countSteal;
                else
                    Debug.LogWarningFormat("Unknown Symbol:{0}", symbol);
            }
            // Shooting:4069, Block:1255, Pass:2488, Steal:2188
            Debug.LogFormat("Shooting:{0}, Block:{1}, Pass:{2}, Steal:{3}", countShooting, countBlock, countPass, countSteal);
        }

        public static void RunTest3()
        {
            WeightedRandomizer<EType> randomizer = new WeightedRandomizer<EType>();
            randomizer.AddOrUpdate(EType.Shooting, 61); // 61/139 = 0.4388489208633094
            randomizer.AddOrUpdate(EType.Block, 29); // 29/139 = 0.2086330935251799
            randomizer.AddOrUpdate(EType.Pass, 12); // 12/139 = 0.0863309352517986
            randomizer.AddOrUpdate(EType.Steal, 37); // 37/139 = 0.2661870503597122
            int countShooting = 0;
            int countBlock = 0;
            int countPass = 0;
            int countSteal = 0;
            for (int i = 0; i < 1000000; i++)
            {
                var symbol = randomizer.GetNext();
                if (symbol == EType.Shooting)
                    ++countShooting;
                else if (symbol == EType.Block)
                    ++countBlock;
                else if (symbol == EType.Pass)
                    ++countPass;
                else if (symbol == EType.Steal)
                    ++countSteal;
                else
                    Debug.LogWarningFormat("Unknown Symbol:{0}", symbol);
            }
            // Shooting:4375, Block:2061, Pass:918, Steal:2646
            Debug.LogFormat("Shooting:{0}, Block:{1}, Pass:{2}, Steal:{3}", countShooting, countBlock, countPass, countSteal);
        }
    }
} // end of the namespace AI.


