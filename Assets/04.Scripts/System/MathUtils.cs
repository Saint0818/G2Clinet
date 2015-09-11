
using UnityEngine;

namespace G2
{
    public class MathUtils
    {
        public static float GetAngle(Transform t1, Transform t2)
        {
            Vector3 relative = t1.InverseTransformPoint(t2.position);
            return Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
        }
    }
}


