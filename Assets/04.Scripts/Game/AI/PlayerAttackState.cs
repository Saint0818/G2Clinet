using System;
using System.Collections.Generic;
using AI;
using G2;
using GamePlayEnum;
using GamePlayStruct;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 球員在進攻狀態.
/// </summary>
public class PlayerAttackState : State<EPlayerAIState, EGameMsg>
{
    public override EPlayerAIState ID
    {
        get { return EPlayerAIState.Attack; }
    }

    private TTacticalData mTactical;
    private readonly PlayerBehaviour mPlayer;
    private AISkillJudger mSkillJudger;

    /// <summary>
    /// 進攻狀態會做的行為. 這會根據情況, 用亂數的方式找出一個來做.
    /// </summary>
    private enum EAction
    {
        None, Dunk, Shoot2, Shoot3, Pass, Push, Elbow, FakeShoot,
        MoveDodge // 轉身運球(會閃過對手).
    }
    private readonly WeightedRandomizer<EAction> mRandomizer = new WeightedRandomizer<EAction>();
    /// <summary>
    /// 發生某件事情時, 對應的動作.
    /// </summary>
    private readonly Dictionary<EAction, CommonDelegateMethods.Action> mProbabilityActions = new Dictionary<EAction, CommonDelegateMethods.Action>();

    private readonly PlayerAI mPlayerAI;

    public PlayerAttackState([NotNull]PlayerAI playerAI, [NotNull] PlayerBehaviour player)
    {
        mPlayer = player;
        mPlayerAI = playerAI;

        mProbabilityActions.Add(EAction.Dunk, doShoot);
        mProbabilityActions.Add(EAction.Shoot2, doShoot);
        mProbabilityActions.Add(EAction.Shoot3, doShoot);
        mProbabilityActions.Add(EAction.Pass, doPass);
//        mProbabilityActions.Add(EProbability.Push, doPush);
        mProbabilityActions.Add(EAction.Elbow, doElbow);
        mProbabilityActions.Add(EAction.FakeShoot, doFakeShoot);
        mProbabilityActions.Add(EAction.MoveDodge, doMoveDodge);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="players"> 該場比賽中, 全部的球員. </param>
    public void Init([NotNull]PlayerBehaviour[] players)
    {
        mSkillJudger = new AISkillJudger(mPlayer, players, true);
    }

    public override void Enter(object extraInfo)
    {
        if(GameData.DSkillData.ContainsKey(mPlayer.Attribute.ActiveSkill.ID))
        {
            TSkillData skill = GameData.DSkillData[mPlayer.Attribute.ActiveSkill.ID];
            mSkillJudger.SetCondition(skill.Situation, mPlayer.Attribute.AISkillLv);
        }
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        if(!mPlayer.AIing)
            return;

        if(mSkillJudger.IsMatchCondition() && mPlayer.CanUseActiveSkill)
        {
            GameController.Get.DoSkill(mPlayer);
            return;
        }

        if(!mPlayer.IsAllShoot)
        {
//            bool isDoingAction;
            if(isBallOwner())
//                isDoingAction = tryDoShooting();
                tryDoShooting();
            else
//                isDoingAction = tryDoPush();
                tryDoPush();

//            GameController.Get.AIAttack(mPlayer);
//            if(!isDoingAction)
                GameController.Get.AIMove(mPlayer, ref mTactical);
        }
    }

    private bool isBallOwner()
    {
        return GameController.Get.BallOwner == mPlayer;
    }

    public override void HandleMessage(Telegram<EGameMsg> msg)
    {
        if(msg.Msg == EGameMsg.CoachOrderAttackTactical)
        {
            mTactical = (TTacticalData)msg.ExtraInfo;
//            Debug.LogFormat("HandleMessage, Tactical:{0}", mTactical);
        }
    }

    private bool tryDoShooting()
    {
        if(!GameController.Get.BallOwner)
            return false;

        var probability = randomProbability();
        if(probability == EAction.None)
            return false;

        mProbabilityActions[probability]();

        return true;
    }

    private EAction randomProbability()
    {
        mRandomizer.Clear();

        float shootPointDis = MathUtils.Find2DDis(mPlayer.transform.position,
                                    CourtMgr.Get.ShootPoint[mPlayer.Team.GetHashCode()].transform.position);

        var threat = mPlayerAI.Team.HasDefPlayer(mPlayer, GameConst.ThreatDistance, GameConst.ThreatAngle);

        // 是否可以灌籃.
        if (shootPointDis <= GameConst.DunkDistance &&
//            (dunkRate || mPlayer.CheckAnimatorSate(EPlayerState.HoldBall)) &&
            mPlayer.CheckAnimatorSate(EPlayerState.HoldBall) /*&& Team.IsInUpfield(mPlayer)*/)
            mRandomizer.AddOrUpdate(EAction.Dunk, mPlayer.Attr.DunkRate);
        //            GameController.Get.AIFakeShoot(mPlayer);

        // 是否可以投 2 分球.
        if (shootPointDis <= GameConst.TwoPointDistance &&
//                (GameController.Get.HasDefPlayer(mPlayer.DefPlayer, 1.5f, 40) == 0 || shootRate || mPlayer.CheckAnimatorSate(EPlayerState.HoldBall)) &&
//                GameController.Get.HasDefPlayer(mPlayer.DefPlayer, 1.5f, 40) == 0 &&
//                mPlayerAI.Team.HasDefPlayer(mPlayer, GameConst.ThreatDistance, GameConst.ThreatAngle) == 0 &&
                threat != Team.EFindPlayerResult.InFront &&
                mPlayer.CheckAnimatorSate(EPlayerState.HoldBall)
                /*Team.IsInUpfield(mPlayer)*/)
            //            GameController.Get.AIFakeShoot(mPlayer);
            mRandomizer.AddOrUpdate(EAction.Shoot2, mPlayer.Attr.PointRate2);

        // 是否可以投 3 分球.
        if(shootPointDis <= GameConst.TreePointDistance + 1 &&
//                (GameController.Get.HasDefPlayer(mPlayer.DefPlayer, 3.5f, 40) == 0 || shoot3Rate || mPlayer.CheckAnimatorSate(EPlayerState.HoldBall)) &&
//                GameController.Get.HasDefPlayer(mPlayer.DefPlayer, 3.5f, 40) == 0 &&
//                mPlayerAI.Team.HasDefPlayer(mPlayer, GameConst.ThreatDistance, GameConst.ThreatAngle) == 0 &&
                threat != Team.EFindPlayerResult.InFront &&
                mPlayer.CheckAnimatorSate(EPlayerState.HoldBall)
                /*Team.IsInUpfield(mPlayer)*/)
            //            GameController.Get.AIFakeShoot(mPlayer);
            mRandomizer.AddOrUpdate(EAction.Shoot3, mPlayer.Attr.PointRate3);

        // 是否可以做假動作
        if(shootPointDis <= GameConst.TreePointDistance + 1 &&
           !mPlayer.CheckAnimatorSate(EPlayerState.HoldBall) && 
//           GameController.Get.HasDefPlayer(mPlayer, GameConst.BlockDistance, 40) != 0)
//           mPlayerAI.Team.HasDefPlayer(mPlayer, GameConst.ThreatDistance, GameConst.ThreatAngle) != 0)
           threat == Team.EFindPlayerResult.InFront)
        {
            mRandomizer.AddOrUpdate(EAction.FakeShoot, GameConst.FakeShootRate);
        }

        // 是否可以用 Elbow 攻擊對方.
        PlayerBehaviour defPlayer;
        //        if(elbowRate && GameController.Get.IsInUpfield(mPlayer) && 
        if(GameController.Get.IsInUpfield(mPlayer) &&
           GameController.Get.HaveDefPlayer(mPlayer, GameConst.StealBallDistance, 90, out defPlayer) != 0 &&
           mPlayer.CoolDownElbow == 0 && !mPlayer.CheckAnimatorSate(EPlayerState.Elbow))
        {
            mRandomizer.AddOrUpdate(EAction.Elbow, mPlayer.Attr.ElbowingRate);
            //            if (mPlayer.DoPassiveSkill(ESkillSituation.Elbow, defPlayer.transform.position))
            //            {
            //                GameController.Get.coolDownPass = 0;
            //                mPlayer.CoolDownElbow = Time.time + 3;
            //                GameController.Get.RealBallFxTime = 1f;
            //                CourtMgr.Get.RealBallFX.SetActive(true);
            //            }
        }

        // 是否可以傳球.
        //        if((passRate || mPlayer.CheckAnimatorSate(EPlayerState.HoldBall)) && 
        if ((mPlayer.CheckAnimatorSate(EPlayerState.HoldBall)) &&
           GameController.Get.coolDownPass == 0 && /*!IsShooting && !IsDunk &&*/
           !mPlayer.CheckAnimatorSate(EPlayerState.Elbow)/* && BallOwner.AIing*/)
            //            GameController.Get.AIPass(mPlayer);
            mRandomizer.AddOrUpdate(EAction.Pass, mPlayer.Attr.PassRate);

        // 是否可以轉身運球過人.
        if (mPlayer.IsHaveMoveDodge && GameController.Get.CoolDownCrossover == 0 && mPlayer.CanMove &&
            GameController.Get.HasDefPlayer(mPlayer.DefPlayer, 1.5f, 40) == 0)
        {
            mRandomizer.AddOrUpdate(EAction.MoveDodge, mPlayer.MoveDodgeRate);
            //            if(Random.Range(0, 100) <= mPlayer.MoveDodgeRate)
            //                mPlayer.DoPassiveSkill(ESkillSituation.MoveDodge);
        }

        if(mRandomizer.IsEmpty())
            return EAction.None;
        return mRandomizer.GetNext();
    }

    private bool tryDoPush()
    {
        // 參數 player 並未持球, 所以只能做 Push 被動技.
        // 這裡的企劃規則是, 附近的敵對球員必須是 Idle 狀態時, 才會真的執行推人行為.
//        var nearPlayer = GameController.Get.hasNearPlayer(mPlayer, GameConst.StealBallDistance, false);
        var nearPlayer = mPlayerAI.Team.FindNearestOpponentPlayer(mPlayerAI.transform.position);
        bool pushRate = Random.Range(0, 100) < mPlayer.Attr.PushingRate;
//        if(nearPlayer && pushRate && mPlayer.CoolDownPush == 0)
        if(nearPlayer && pushRate && Math.Abs(mPlayer.CoolDownPush) < float.Epsilon &&
           nearPlayer.GetComponent<PlayerBehaviour>().CheckAnimatorSate(EPlayerState.Idle))
        {
            if(mPlayer.DoPassiveSkill(ESkillSituation.Push0, nearPlayer.transform.position))
            {
                mPlayer.CoolDownPush = Time.time + GameConst.CoolDownPushTime;
                return true;
            }
        }

        return false;
    }

    private void doShoot()
    {
        GameController.Get.DoShoot();
    }

    private void doFakeShoot()
    {
        mPlayer.AniState(EPlayerState.FakeShoot, CourtMgr.Get.ShootPoint[mPlayer.Team.GetHashCode()].transform.position);
    }

    private void doElbow()
    {
        PlayerBehaviour defPlayer;
        GameController.Get.HaveDefPlayer(mPlayer, GameConst.StealBallDistance, 90, out defPlayer);
        if(mPlayer.DoPassiveSkill(ESkillSituation.Elbow, defPlayer.transform.position))
        {
            GameController.Get.coolDownPass = 0;
            mPlayer.CoolDownElbow = Time.time + 3;
            GameController.Get.RealBallFxTime = 1f;
            CourtMgr.Get.RealBallFX.SetActive(true);
        }
    }

//    private void doPush()
//    {
//    }

    private void doPass()
    {
        GameController.Get.AIPass(mPlayer);
    }

    private void doMoveDodge()
    {
        mPlayer.DoPassiveSkill(ESkillSituation.MoveDodge);
    }
}
