using System;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using GameEnum;

public enum ETimerKind
{
    Default = 0,
    Self0 = 1,
    Self1 = 2,
    Self2 = 3,
    Npc0 = 4,
    Npc1 = 5,
    Npc2 = 6
}

public class TimerMgr : KnightSingleton<TimerMgr>
{
    public ETimerKind SeleckKind = ETimerKind.Default;
    public float CrtTime = 1f;
    private Vector3 ballvelocity = Vector3.zero;
    private bool tempGravity = true;
    public bool IsPause = false;
    public PlayerBehaviour TimeController;
    private Dictionary<ETimerKind, float> timeRecorder = new Dictionary<ETimerKind, float>();

    void Start()
    {
        CrtTime = 1f;
    }

    /// <summary>
    /// 遊戲結束，時間回復
    /// </summary>
    public void ResetTime()
    {
        for (int i = 0; i < GameController.Get.GamePlayers.Count; i++)
			GameController.Get.GamePlayers[i].Pause = false;
        PauseBall(false);
//        foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind)))
//            ChangeTime(item, 1);
    }

    /// <summary>
    /// 用於放完技能時，時間回復
    /// </summary>
    /// <param name="player">Player.</param>
    public void ResetTime(ref PlayerBehaviour player)
    {
        for (int i = 0; i < GameController.Get.GamePlayers.Count; i++)
			GameController.Get.GamePlayers[i].Pause = false;

        PauseBall(false);
    }


    public float GetTime(ETimerKind key)
    {
//        if (GameStart.Get.IsOpenChronos)
//            return Timekeeper.instance.Clock(key.ToString()).localTimeScale;
//        else
            return 1;
    }

    public void ChangeTime(ETimerKind key, float value)
    {
//        if (!GameStart.Get.IsOpenChronos && CrtTime == 0)
//            return;

		if (value == 0)
			CrtTime = GameConst.Min_TimePause;	
		else
			CrtTime = value;
    }

    public Dictionary<EAnimatorState, bool> LoopStates = new Dictionary<EAnimatorState, bool>();

    public void PauseTimeByUseSkill(float releaseTime, Action callback)
	{
        for (int i = 0; i < GameController.Get.GamePlayers.Count; i++)
            GameController.Get.GamePlayers[i].Pause = true;

        PauseBall(true);
        StartCoroutine(DelayTime( releaseTime, callback));
	}
				
	public void CamEvent(float releaseTime,float speed)
	{
		StartCoroutine(DelayTime( releaseTime, ResetTime));
	}

    private IEnumerator DelayTime(float t, Action callback)
	{
        yield return new WaitForSeconds (t);
        if (callback != null)
            callback();
	}

    public void PauseBall(bool isPase)
    {
        if (IsPause == isPase)
            return;
        else
        {
            if (isPase)
            {
                ballvelocity = CourtMgr.Get.RealBallCompoment.MoveVelocity;
                tempGravity = CourtMgr.Get.RealBallCompoment.Gravity;
                CourtMgr.Get.RealBallCompoment.Gravity = false;
                CourtMgr.Get.RealBallObj.transform.DOPause();
            }
            else
            {
				if(!tempGravity && GameController.Get.IsReboundTime) {
					//因為球在跑動作的時候沒有Gravity，所以用IsReboundTime去判斷是否在做籃筐動作
					//如果進來就是不把重力恢復(恢復的話球變沒重力)
				} else {
					if(GameController.Get.BallOwner == null) {
						CourtMgr.Get.RealBallCompoment.Gravity = tempGravity;
						CourtMgr.Get.RealBallCompoment.MoveVelocity = ballvelocity;
						CourtMgr.Get.RealBallObj.transform.DOPlay();
					}
				}
            }
            IsPause = isPase;
        }
    }
}

