using UnityEngine;
using System;
using System.Collections;
using Chronos;

public enum ETimerKind
{
	Default,
	Player0,
	Player1,
	Player2,
	Player3,
	Player4,
	Player5
}

public class TimerMgr : KnightSingleton<TimerMgr>
{
	public Clock playerClock;
	public ETimerKind SeleckKind = ETimerKind.Default;
	public float CrtTime = 1f;

	void Start()
	{
		playerClock = Timekeeper.instance.Clock(SeleckKind.ToString());
	}

	public void ChangeTime(ETimerKind key, float value)
	{
		CrtTime = value;
		Timekeeper.instance.Clock(key.ToString()).localTimeScale = CrtTime;
	}
}

