using System;
using Chronos;
using DG.Tweening;
using UnityEngine;

public enum ETimerKind
{
	Default,
	Player0,
	Player1,
	Player2,
	Player3,
	Player4,
	Player5,
	Ball
}

public class TimerMgr : KnightSingleton<TimerMgr>
{
	public Clock playerClock;
	public ETimerKind SeleckKind = ETimerKind.Default;
	public float CrtTime = 1f;

	void Start()
	{
		playerClock = Timekeeper.instance.Clock(SeleckKind.ToString());
		CrtTime = 1f;
	}

	public void ChangeTime(ETimerKind key, float value)
	{
		CrtTime = value;
		Timekeeper.instance.Clock(key.ToString()).localTimeScale = CrtTime;
		if(key == ETimerKind.Ball){
			Timekeeper.instance.Clock(key.ToString()).DOPause();
//			LogMgr.Get.Log("DOPause ball");
		}
	}

	private Vector3 ballvelocity = Vector3.zero;
	public bool IsPause = false;

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

