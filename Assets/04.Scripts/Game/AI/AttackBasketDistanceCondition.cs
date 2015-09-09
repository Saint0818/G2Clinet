using UnityEngine;

namespace AI
{
    /// <summary>
    /// 檢查玩家到進攻籃框的距離, 若是小於企劃表格的數值, 就判斷為 true.
    /// </summary>
    public class AttackBasketDistanceCondition : Condition
    {
        private float mMaxDistance;

        private Vector2 mFocusPos = Vector2.zero;
        private Vector2 mBasketPos = Vector2.zero;

        public AttackBasketDistanceCondition(AISkillJudger parent) : base(parent)
        {
        }

        public override void Init(object value)
        {
            mMaxDistance = (float)value;

            if (mMaxDistance < 0)
                Debug.LogWarningFormat("AttackBasketDistanceCondition, value({0}) must be great than zero.", value);
        }

        public override bool IsValid()
        {
            if(mMaxDistance <= 0) // 0: 表示關閉此檢查.
                return true;

            mFocusPos.Set(Parent.FocusPlayer.transform.position.x, Parent.FocusPlayer.transform.position.z);

            var basketPos = CourtMgr.Get.ShootPoint[Parent.FocusPlayer.Team.GetHashCode()].transform.position;
            mBasketPos.Set(basketPos.x, basketPos.z);

            return Vector2.Distance(mFocusPos, mBasketPos) <= mMaxDistance;
        }
    }
}

