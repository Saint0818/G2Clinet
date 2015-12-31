using System;
using Chronos;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

public enum ETimerKind
{
	Default,
	Self0,
	Self1,
	Self2,
	Npc0,
	Npc1,
	Npc2
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
        if (!GameStart.Get.IsOpenChronos)
            return;
        
		CrtTime = value;
		Timekeeper.instance.Clock(key.ToString()).localTimeScale = CrtTime;
		if(GameController.Get.GamePlayers.Count > 1 && key == ETimerKind.Self0) {
			if(value == 0)
			{
				GameController.Get.GamePlayers[0].transform.DOPause();
			}
			else
			{
				GameController.Get.GamePlayers[0].transform.DOPlay();
			}
		}
        if(GameController.Get.GamePlayers.Count > 2 && key == ETimerKind.Self1) {
			if(value == 0)
			{
				GameController.Get.GamePlayers[1].transform.DOPause();
			}
			else
			{
				GameController.Get.GamePlayers[1].transform.DOPlay();
			}
		}
        if(GameController.Get.GamePlayers.Count > 3 && key == ETimerKind.Self2) {
			if(value == 0)
			{
				GameController.Get.GamePlayers[2].transform.DOPause();
			}
			else
			{
				GameController.Get.GamePlayers[2].transform.DOPlay();
			}
		}
		if(GameController.Get.GamePlayers.Count > 4 && key == ETimerKind.Npc0) {
			if(value == 0)
			{
				GameController.Get.GamePlayers[3].transform.DOPause();
			}
			else
			{
				GameController.Get.GamePlayers[3].transform.DOPlay();
			}
		}
        if(GameController.Get.GamePlayers.Count > 5 && key == ETimerKind.Npc1) {
			if(value == 0)
			{
				GameController.Get.GamePlayers[4].transform.DOPause();
			}
			else
			{
				GameController.Get.GamePlayers[4].transform.DOPlay();
			}
		}
        if(GameController.Get.GamePlayers.Count > 6 && key == ETimerKind.Npc2) {
			if(value == 0)
			{
				GameController.Get.GamePlayers[5].transform.DOPause();
			}
			else
			{
				GameController.Get.GamePlayers[5].transform.DOPlay();
			}
		}
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

