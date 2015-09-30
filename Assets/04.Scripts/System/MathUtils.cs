
using UnityEngine;

namespace G2
{
    public class MathUtils
    {
        /// <summary>
        /// <seealso cref="http://answers.unity3d.com/questions/15822/how-to-get-the-positive-or-negative-angle-between.html"/>
        /// <para> You can use Mathf.Atan2 to compute the angle between 2D vectors. This only works 
        /// for 2d rotations though, however often that's all you need even in a 3d game. </para>
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static float GetAngle(Transform t1, Transform t2)
        {
            Vector3 relative = t1.InverseTransformPoint(t2.position);
            return Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
        }

        public static float Find2DDis(Vector3 v1, Vector3 v2)
        {
            v1.y = 0;
            v2.y = 0;
            return Vector3.Distance(v1, v2);
        }
    }
}


