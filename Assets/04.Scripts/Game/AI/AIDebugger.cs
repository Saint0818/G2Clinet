using UnityEngine;

namespace AI
{
    public class AIDebugger : KnightSingleton<AIDebugger>
    {
        public Vector2 StartPos = new Vector2(200, 100);
        public float YInterval = 60;

        public bool ShowStatus;
    } // end of the class.
} // end of the namespace.


