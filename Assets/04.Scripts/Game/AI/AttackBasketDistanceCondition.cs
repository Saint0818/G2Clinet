using UnityEngine;

namespace AI
{
    /// <summary>
    /// �ˬd���a��i���x�ت��Z��, �Y�O�p�������檺�ƭ�, �N�P�_�� true.
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
            if(mMaxDistance <= 0) // 0: ����������ˬd.
                return true;

            mFocusPos.Set(Parent.FocusPlayer.transform.position.x, Parent.FocusPlayer.transform.position.z);

            var basketPos = CourtMgr.Get.ShootPoint[Parent.FocusPlayer.Team.GetHashCode()].transform.position;
            mBasketPos.Set(basketPos.x, basketPos.z);

            return Vector2.Distance(mFocusPos, mBasketPos) <= mMaxDistance;
        }
    }
}

