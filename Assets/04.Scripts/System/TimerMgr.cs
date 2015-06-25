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
//		GameController.Get.Joysticker.SetTimerKey(SeleckKind);
		CrtTime = value;
//		Debug.Log("key : " + key.ToString () + "Time : " + value);
		Timekeeper.instance.Clock(key.ToString()).localTimeScale = CrtTime;
//		GameController.Get.Joysticker.SetTimerTime(CrtTime);
	}

	void OnGUI()
	{
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			ChangeTime(SeleckKind, 0);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			ChangeTime(SeleckKind, 0.5f);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			ChangeTime(SeleckKind, 1);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			ChangeTime(SeleckKind, 2);
		}
	}
}

