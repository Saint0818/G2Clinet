#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("EJOdkqIQk5iQEJOTkhEH3A7B9JruVYWkW+1aRoBwsRh2Ye2+8PolBNu8hLSkdlZEkJOp2EGZQf1olEDZM7q1g8gdIXFAL82cioN0mdkwLuli+b3gTN/bDX/cZ7FWCAQPtHtRqvdh2d6KOlGnHFqXt8hP+lJd+TY55FXnhnfn6Oyl/kpyR2ByMdxoysOEBCpUtWNk3B054c5H5WXe0iKc9WD+YuvYzVDVfZUrtzKIRlWXLgztMJwYWGkdj60wMBr98vadVbQuTOiiEJOwop+Um7gU2hRln5OTk5eSkdAffr09I4Br8hIUOZXmqRIYViFdk4HHRaMpcZhsx6ngfrb0LF415uA0e4hnG7KmaUwVotxkzkC0FVC5/ngXPAqL7FZJVZCRk5KT");
        private static int[] order = new int[] { 1,5,12,9,6,7,8,8,11,9,11,13,13,13,14 };
        private static int key = 146;

        public static byte[] Data() {
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
