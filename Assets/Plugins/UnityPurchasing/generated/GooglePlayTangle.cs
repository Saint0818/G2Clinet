#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("qywfMj9kXM530R1yqTirFekJjVp9z0xvfUBLRGfLBcu6QExMTEhNTvLBCRlR2PwBFPJ/LKrjdmevK47ptf4w9rPCFwXu3irw/yKdLHprmuUhF3IVg+mB5G3z6blx0Au+OvyMaPkkOFWp2hnn8kRpL7Fzdwe3Zz2YzKWo1WtVa5odb9eSJkZXYv8ljMQzmwUQxlaOfZa56LaMFZwq78KTPWjtFZWDQr925mn/5MYaFp7lOyA3evmHJMAJu8WvfqgtdercGEPlzcU8lTdSkYkkkE4+WU0MQF8nZhadus9MQk19z0xHT89MTE2eZpcUhy5oqNK+i6wTt7HHKSXn9L5FCCarC/K+OOBxx7gAbqSi3VLO300auRcANiEgKPjvYvJIQk9OTE1M");
        private static int[] order = new int[] { 9,9,4,8,5,13,7,11,10,12,13,12,13,13,14 };
        private static int key = 77;

        public static byte[] Data() {
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
