using UnityEngine;

namespace AI
{
    public static class TransformExtension
    {
        /// <summary>
        /// target 是否在 source +Z 軸的扇形範圍內.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="fanDis"></param>
        /// <param name="fanAngle"></param>
        /// <returns> true: target 在 source 的扇形範圍內. </returns>
        public static bool IsInFanArea(this Transform source, Vector3 target, float fanDis, float fanAngle)
        {
            var betweenAngle = G2.MathUtils.FindAngle(source, target);
            var betweenDis = G2.MathUtils.Find2DDis(source.position, target);

            return Mathf.Abs(betweenAngle) <= fanAngle * 0.5f && betweenDis <= fanDis;
        }
    } // end of the class.
} // end of the namespace.


