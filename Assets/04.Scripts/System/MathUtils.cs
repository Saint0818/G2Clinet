
using JetBrains.Annotations;
using UnityEngine;

namespace G2
{
    public class MathUtils
    {
        /// <summary>
        /// <seealso cref="http://answers.unity3d.com/questions/15822/how-to-get-the-positive-or-negative-angle-between.html"/>
        /// <para> You can use Mathf.Atan2 to compute the angle between 2D vectors. This only works 
        /// for 2d rotations though, however often that's all you need even in a 3d game. </para>
        /// <para> 以 source 的 +Z 軸為基準, 算出某點和 source +Z 軸的夾角. , 會得到 0 ~ 90 度; 當,</para>
        /// 夾角說明:
        /// <list type="number">
        /// <item> target 在 source 的 +X and +Z 區塊時: return 0 ~ 90. </item>
        /// <item> target 在 source 的 +X and -Z 區塊時: return 90 ~ 180. </item>
        /// <item> target 在 source 的 -X and +Z 區塊時: return 0 ~ -90. </item>
        /// <item> target 在 source 的 -X and -Z 區塊時: return -90 ~ -180. </item>
        /// </list>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float FindAngle(Transform source, Vector3 target)
        {
			Vector3 relative = source.InverseTransformPoint(target);
			return Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
		}

//        public static float GetAngle([NotNull]Transform source, Transform target)
//        {
//            Vector3 relative = source.InverseTransformPoint(target.position);
//            return Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
//        }

        public static float Find2DDis(Vector3 v1, Vector3 v2)
        {
            v1.y = 0;
            v2.y = 0;
            return Vector3.Distance(v1, v2);
        }

        public static Vector2 Convert2D(Vector3 value)
        {
            Vector2 v = Vector2.zero;
            v.Set(value.x, value.z);
            return v;
        }
    }
}


