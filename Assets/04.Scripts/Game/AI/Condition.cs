

namespace AI
{
    /// <summary>
    /// <para> 要搭配 AISkillJuder 一起使用. 這表示是一個判斷. </para>
    /// </summary>
    public abstract class Condition
    {
        public AISkillJudger Parent
        {
            get { return mParent; }
        }

        private readonly AISkillJudger mParent;

        protected Condition(AISkillJudger parent)
        {
            mParent = parent;
        }

        public abstract void Init(object value);

        /// <summary>
        /// 判斷式的結果.
        /// </summary>
        /// <returns></returns>
        public abstract bool IsValid();
    }
}


