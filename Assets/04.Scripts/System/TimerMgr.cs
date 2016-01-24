using System;
using Chronos;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

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
    public bool IsPause = false;

	void Start()
	{
		CrtTime = 1f;
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

		checkPlayerIsUseActive(key);
	}

	private void checkPlayerIsUseActive (ETimerKind key) {
		bool isNeedRecover = false;
		if (GameController.Get.GamePlayers.Count > 1 && key == ETimerKind.Self0 && GameController.Get.GamePlayers[0].IsUseActiveSkill) {
			isNeedRecover = true;
		}

		if (GameController.Get.GamePlayers.Count > 2 && key == ETimerKind.Self1 && GameController.Get.GamePlayers[1].IsUseActiveSkill) {
			isNeedRecover = true;
		}

		if(GameController.Get.GamePlayers.Count > 3 && key == ETimerKind.Self2 && GameController.Get.GamePlayers[2].IsUseActiveSkill) {
			isNeedRecover = true;
		}

		if(GameController.Get.GamePlayers.Count > 4 && key == ETimerKind.Npc0 && GameController.Get.GamePlayers[3].IsUseActiveSkill) {
			isNeedRecover = true;
		}

		if(GameController.Get.GamePlayers.Count > 5 && key == ETimerKind.Npc1 && GameController.Get.GamePlayers[4].IsUseActiveSkill) {
			isNeedRecover = true;
		}

		if(GameController.Get.GamePlayers.Count > 6 && key == ETimerKind.Npc2 && GameController.Get.GamePlayers[5].IsUseActiveSkill) {
			isNeedRecover = true;
		}

		if(isNeedRecover)
			Timekeeper.instance.Clock(key.ToString()).localTimeScale = 1;
	}

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
                timer.SetRecording(34, 2);
                timer.recordTransform = false;
                timer.rigidbody.useGravity = true;
            }  
        }
    }

	public void PauseTime(bool isPase)
	{
		float time = (isPase == true ? 0 : 1);
		foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind)))
			ChangeTime (item, time);

		PauseBall(isPase);
	}

	public void PauseBall (bool isPase) {
		if (IsPause == isPase)
			return;
		else
		{
			if(isPase)
			{
				ballvelocity = CourtMgr.Get.RealBallVelocity;
				CourtMgr.Get.RealBallRigidbody.isKinematic = true;
				CourtMgr.Get.RealBall.transform.DOPause();
			}
			else
			{
				CourtMgr.Get.RealBallRigidbody.isKinematic = false;
				CourtMgr.Get.RealBallVelocity = ballvelocity;
				CourtMgr.Get.RealBall.transform.DOPlay();
			}
			IsPause = isPase;
		}
	}
}

