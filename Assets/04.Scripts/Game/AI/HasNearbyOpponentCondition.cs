
using UnityEngine;

namespace AI
{
    /// <summary>
    /// 檢查有幾個人靠近某位球員.
    /// </summary>
    public class HasNearbyOpponentCondition : Condition
    {
        /// <summary>
        /// 幾個人靠近此球員時, 條件式會判定為 true.
        /// </summary>
        private readonly int mNearNum;

        private Vector2 mFocusPos = Vector2.zero;
        private Vector2 mOtherPos = Vector2.zero;

        public HasNearbyOpponentCondition(AISkillJudger parent, int value) : base(parent)
        {
            mNearNum = value;
        }

        public override bool IsValid()
        {
            if(mNearNum <= 0) // 0: 表示關閉此檢查.
                return true;

            float skillDis = GameData.DSkillData[Parent.FocusPlayer.Attribute.ActiveSkill.ID].Distance(Parent.FocusPlayer.Attribute.ActiveSkill.Lv);

            int inRangeCount = 0;
            for(int i = 0; i < Parent.Players.Length; i++)
            {
                if(Parent.Players[i].Team != Parent.FocusPlayer.Team)
                {
                    mFocusPos.Set(Parent.FocusPlayer.transform.position.x, Parent.FocusPlayer.transform.position.z);
                    mOtherPos.Set(Parent.Players[i].transform.position.x, Parent.Players[i].transform.position.z);

                    if(skillDis >= Vector2.Distance(mFocusPos, mOtherPos))
                        ++inRangeCount;
                }
            }

            return inRangeCount >= mNearNum;
        }
    }

}
