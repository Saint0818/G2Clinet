
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

        /// <summary>
        /// 將 "100111" 轉換成 int array. index 0 就是 bit 0, index 1 就是 bit 1, 已此類推.
        /// </summary>
        /// <param name="bitString"></param>
        /// <returns></returns>
        [CanBeNull]
        public static int[] Convert([NotNull] string bitString)
        {
			if(bitString == null)
			{
				Debug.LogError("Parameter is Null");
				return null;
			}

			if(bitString.Length >= MaxBitsNum)
            {
                Debug.LogErrorFormat("Parameter is too long, Length:{0}", bitString.Length);
                return null;
            }

            int[] ints = new int[bitString.Length];
            for(int i = 0; i < bitString.Length; ++i)
            {
                if(!int.TryParse(bitString[bitString.Length - 1 - i].ToString(), out ints[i]))
                {
                    Debug.LogErrorFormat("Parse int fail. char:{0}, index:{1}", bitString[i], i);
                    return null;
                }
            }

            return ints;
        }
    }
} // end of the namespace AI

