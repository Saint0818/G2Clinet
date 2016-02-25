using System;
using Chronos;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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
        foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind)))
            TimerMgr.Get.ChangeTime(item, 1);

        TimeController = null;
    }

    /// <summary>
    /// 用於放完技能時，時間回復
    /// </summary>
    /// <param name="player">Player.</param>

    public void ResetTime(ref PlayerBehaviour player)
    {
        if (TimeController == player || player == null)
        {
            ResetTime();
        }
    }

    /// <summary>
    /// 設定時間主控權
    /// </summary>
    /// <param name="player">Player.</param>
    public void SetTimeController(ref PlayerBehaviour player)
    {
        if (player)
            TimeController = player;
    }

    public float GetTime(ETimerKind key)
    {
        if (GameStart.Get.IsOpenChronos)
            return Timekeeper.instance.Clock(key.ToString()).localTimeScale;
        else
            return 1;
    }

    public void ChangeTime(ETimerKind key, float value)
    {
        if (!GameStart.Get.IsOpenChronos && CrtTime == 0)
            return;

        CrtTime = value;
        Timekeeper.instance.Clock(key.ToString()).localTimeScale = CrtTime;

//        checkPlayerIsUseActive(key); 
    }

    /// <summary>
    /// 技能施放，時間控制
    /// </summary>
    /// <param name="key">Key.</param>
    /// <param name="value">Value.</param>
    /// <param name="user">User.</param>
    public void ChangeTime(ETimerKind key, float value, ref PlayerBehaviour user)
    {
        if (user != TimeController)
            return;
        
        ChangeTime(key, value);
    }

//    private void checkPlayerIsUseActive(ETimerKind key)
//    {
//        bool isNeedRecover = false;
//        if (GameController.Get.GamePlayers.Count > 1 && key == ETimerKind.Self0 && GameController.Get.GamePlayers[0].IsUseActiveSkill)
//        {
//            isNeedRecover = true;
//        }
//
//        if (GameController.Get.GamePlayers.Count > 2 && key == ETimerKind.Self1 && GameController.Get.GamePlayers[1].IsUseActiveSkill)
//        {
//            isNeedRecover = true;
//        }
//
//        if (GameController.Get.GamePlayers.Count > 3 && key == ETimerKind.Self2 && GameController.Get.GamePlayers[2].IsUseActiveSkill)
//        {
//            isNeedRecover = true;
//        }
//
//        if (GameController.Get.GamePlayers.Count > 4 && key == ETimerKind.Npc0 && GameController.Get.GamePlayers[3].IsUseActiveSkill)
//        {
//            isNeedRecover = true;
//        }
//
//        if (GameController.Get.GamePlayers.Count > 5 && key == ETimerKind.Npc1 && GameController.Get.GamePlayers[4].IsUseActiveSkill)
//        {
//            isNeedRecover = true;
//        }
//
//        if (GameController.Get.GamePlayers.Count > 6 && key == ETimerKind.Npc2 && GameController.Get.GamePlayers[5].IsUseActiveSkill)
//        {
//            isNeedRecover = true;
//        }
//
//        if (isNeedRecover)
//            Timekeeper.instance.Clock(key.ToString()).localTimeScale = 1;
//    }

    public Dictionary<EAnimatorState, bool> LoopStates = new Dictionary<EAnimatorState, bool>();

    public void SetTimerKey(ETimerKind key, ref GameObject obj)
    {
        if (GameStart.Get.IsOpenChronos)
        {
            Timeline timer = obj.GetComponent<Timeline>();

            if (timer == null)
            {
                timer = obj.AddComponent<Timeline>();
            }

            if (timer)
            {
                timer.mode = TimelineMode.Global;
                timer.globalClockKey = key.ToString();

                //timer.SetRecording(34, 2);
                //timer.recordTransform = false;
                timer.rigidbody.useGravity = true;
            }  
        }
    }

    private bool Useing = false;

    public void PauseTimeByUseSkill(float releaseTime, Action callback)
	{
        foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind)))
            ChangeTime(item, 0);
//        }
//        else
//        {
//            foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind)))
//            {
//                if (timeRecorder.ContainsKey(item))
//                    timeRecorder[item] = GetTime(item);
//                else
//                    timeRecorder.Add(item, GetTime(item));
//
//                ChangeTime(item, 0);
//            }
//        }

        PauseBall(true);

//        if (Useing == false)
//            Useing = true;
     
        StartCoroutine(DelayTime( releaseTime, callback));
	}
				
	public void CamEvent(float releaseTime,float speed)
	{
		foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind)))
			ChangeTime(item, speed);

		StartCoroutine(DelayTime( releaseTime, ResetTime));
	}

    private IEnumerator DelayTime(float t, Action callback)
	{
        yield return new WaitForSeconds (t);
        if (callback != null)
            callback();
	}

    public void PauseTime(bool isPase)
    {
        float time;

        foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind)))
        {
            if (isPase)
            {
                time = 0;
                if (timeRecorder.ContainsKey(item))
                {
                    timeRecorder[item] = GetTime(item);
                }
                else
                {
                    timeRecorder.Add(item, GetTime(item));
                }
            }
            else
                time = timeRecorder[item]; 

            ChangeTime(item, time);
        }

        PauseBall(isPase);
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
                CourtMgr.Get.RealBallObj.transform.DOPause();
            }
            else
            {
                CourtMgr.Get.RealBallCompoment.Gravity = tempGravity;
                CourtMgr.Get.RealBallCompoment.MoveVelocity = ballvelocity;
                CourtMgr.Get.RealBallObj.transform.DOPlay();
            }
            IsPause = isPase;
        }
    }
}

