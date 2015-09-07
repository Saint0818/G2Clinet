
using JetBrains.Annotations;
using UnityEngine;

namespace AI
{
    public class BitConverter
    {
        /// <summary>
        /// 有限制的原因是不希望因為使用者的錯誤輸入, 造成一次 new 太大的記憶體.
        /// </summary>
        private const int MaxBitsNum = 50;

        [CanBeNull]
        public static bool[] Convert(string bitString)
        {
            if(bitString.Length >= MaxBitsNum)
            {
                Debug.LogErrorFormat("Parameter is too long, Length:{0}", bitString.Length);
                return null;
            }

            bool[] bits = new bool[bitString.Length];
            int[] ints = new int[bitString.Length];
            for(int i = 0; i < bitString.Length; ++i)
            {
                if(!int.TryParse(bitString[bitString.Length - 1 - i].ToString(), out ints[i]))
                {
                    Debug.LogErrorFormat("Parse bool fail. char:{0}, index:{1}", bitString[i], i);
                    return null;
                }

                bits[i] = System.Convert.ToBoolean(ints[i]);
            }

            return bits;
        }
    }
} // end of the namespace AI

