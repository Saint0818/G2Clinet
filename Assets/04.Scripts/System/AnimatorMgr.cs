using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameEnum;

public struct TAnimatorItem
{
	public EAnimatorState Type;
	public int StateNo;
}

public static class StateChecker {
	private static bool isInit = false;
	public static Dictionary<EPlayerState, bool> StopStates = new Dictionary<EPlayerState, bool>();
	public static Dictionary<EPlayerState, bool> ShootStates = new Dictionary<EPlayerState, bool>();
	public static Dictionary<EPlayerState, bool> ShowStates = new Dictionary<EPlayerState, bool>();
	public static Dictionary<EPlayerState, bool> LoopStates = new Dictionary<EPlayerState, bool>();
	public static Dictionary<EPlayerState, bool> PassStates = new Dictionary<EPlayerState, bool>();
	
	public static void InitState() {
		if (!isInit) {
			isInit = true;
			
			ShootStates.Add(EPlayerState.Shoot0, true);
			ShootStates.Add(EPlayerState.Shoot1, true);
			ShootStates.Add(EPlayerState.Shoot2, true);
			ShootStates.Add(EPlayerState.Shoot3, true);
			ShootStates.Add(EPlayerState.Shoot4, true);
			ShootStates.Add(EPlayerState.Shoot5, true);
			ShootStates.Add(EPlayerState.Shoot6, true);
			ShootStates.Add(EPlayerState.Shoot7, true);
			ShootStates.Add(EPlayerState.Shoot20, true);
			ShootStates.Add(EPlayerState.TipIn, true);
			
			StopStates.Add(EPlayerState.Block0, true);
			StopStates.Add(EPlayerState.Block1, true);
			StopStates.Add(EPlayerState.Block2, true);
			StopStates.Add(EPlayerState.Block20, true);
			StopStates.Add(EPlayerState.BlockCatch, true);
			StopStates.Add(EPlayerState.CatchFlat, true);
			StopStates.Add(EPlayerState.CatchFloor, true);
			StopStates.Add(EPlayerState.CatchParabola, true);
			StopStates.Add(EPlayerState.Alleyoop, true);
			StopStates.Add(EPlayerState.Elbow0, true);
			StopStates.Add(EPlayerState.Elbow1, true);
			StopStates.Add(EPlayerState.Elbow2, true);
			StopStates.Add(EPlayerState.Elbow20, true);
			StopStates.Add(EPlayerState.Elbow21, true);
			StopStates.Add(EPlayerState.FakeShoot, true);
			StopStates.Add(EPlayerState.HoldBall, true);
			StopStates.Add(EPlayerState.GotSteal, true);
			StopStates.Add(EPlayerState.Pass0, true);
			StopStates.Add(EPlayerState.Pass2, true);
			StopStates.Add(EPlayerState.Pass1, true);
			StopStates.Add(EPlayerState.Pass3, true);
			StopStates.Add(EPlayerState.Pass4, true);
			StopStates.Add(EPlayerState.Pass50, true);
			StopStates.Add(EPlayerState.Push0, true);
			StopStates.Add(EPlayerState.Push1, true);
			StopStates.Add(EPlayerState.Push2, true);
			StopStates.Add(EPlayerState.Push20, true);
			StopStates.Add(EPlayerState.Pick0, true);
			StopStates.Add(EPlayerState.Pick1, true);
			StopStates.Add(EPlayerState.Pick2, true);
			StopStates.Add(EPlayerState.Steal0, true);
			StopStates.Add(EPlayerState.Steal1, true);
			StopStates.Add(EPlayerState.Steal2, true);
			StopStates.Add(EPlayerState.Steal20, true);
			StopStates.Add(EPlayerState.Rebound0, true);
			StopStates.Add(EPlayerState.Rebound20, true);
			StopStates.Add(EPlayerState.ReboundCatch, true);
			StopStates.Add(EPlayerState.TipIn, true);
			StopStates.Add(EPlayerState.Intercept0, true);
			StopStates.Add(EPlayerState.Intercept1, true);
			StopStates.Add(EPlayerState.MoveDodge0, true);
			StopStates.Add(EPlayerState.MoveDodge1, true);
			StopStates.Add(EPlayerState.Buff20, true);
			StopStates.Add(EPlayerState.Buff21, true);
			
			StopStates.Add(EPlayerState.Show1, true);
			StopStates.Add(EPlayerState.Show101, true);
			StopStates.Add(EPlayerState.Show102, true);
			StopStates.Add(EPlayerState.Show103, true);
			StopStates.Add(EPlayerState.Show104, true);
			StopStates.Add(EPlayerState.Show201, true);
			StopStates.Add(EPlayerState.Show202, true);
			StopStates.Add(EPlayerState.Show1001, true);
			StopStates.Add(EPlayerState.Show1003, true);
			StopStates.Add(EPlayerState.KnockDown0, true);
			StopStates.Add(EPlayerState.KnockDown1, true);
			
			StopStates.Add(EPlayerState.Ending0, true);
			StopStates.Add(EPlayerState.Ending10, true);
			
			ShowStates.Add(EPlayerState.Show1, true);
			ShowStates.Add(EPlayerState.Show101, true);
			ShowStates.Add(EPlayerState.Show102, true);
			ShowStates.Add(EPlayerState.Show103, true);
			ShowStates.Add(EPlayerState.Show104, true);
			ShowStates.Add(EPlayerState.Show201, true);
			ShowStates.Add(EPlayerState.Show202, true);
			ShowStates.Add(EPlayerState.Show1001, true);
			ShowStates.Add(EPlayerState.Show1003, true);
			
			LoopStates.Add(EPlayerState.Idle,true);
			LoopStates.Add(EPlayerState.Run0,true);
			LoopStates.Add(EPlayerState.Run1,true);
			LoopStates.Add(EPlayerState.Run2,true);
			LoopStates.Add(EPlayerState.Defence0,true);
			LoopStates.Add(EPlayerState.Defence1,true);
			LoopStates.Add(EPlayerState.Dribble0,true);
			LoopStates.Add(EPlayerState.Dribble1,true);
			LoopStates.Add(EPlayerState.Dribble2,true);
			LoopStates.Add(EPlayerState.Dribble3,true);
			LoopStates.Add(EPlayerState.RunningDefence, true);
			
			PassStates.Add(EPlayerState.Pass0, true);
			PassStates.Add(EPlayerState.Pass1, true);
			PassStates.Add(EPlayerState.Pass2, true);
			PassStates.Add(EPlayerState.Pass3, true);
			PassStates.Add(EPlayerState.Pass4, true);
			PassStates.Add(EPlayerState.Pass5, true);
			PassStates.Add(EPlayerState.Pass6, true);
			PassStates.Add(EPlayerState.Pass7, true);
			PassStates.Add(EPlayerState.Pass8, true);
			PassStates.Add(EPlayerState.Pass9, true);
			PassStates.Add(EPlayerState.Pass50, true);
		}
	}
}

public class AnimatorMgr : KnightSingleton<AnimatorMgr>
{
	public static Dictionary<EAnimatorState, Dictionary<EPlayerState, int>> AnimtorStatesType = new Dictionary<EAnimatorState, Dictionary<EPlayerState, int>>();
    public static Dictionary<EAnimatorState, bool> States = new Dictionary<EAnimatorState, bool>();

    //強制動作：必須等待強制動作做完之後，才能接下一個loop sate，避免loop state太快轉換下一個state,例如fall > Idle
    //必須配合PlayerBehaviour.ReadyToNextState使用
    public static Dictionary<EPlayerState, bool> ForciblyStates = new Dictionary<EPlayerState, bool>();

	private TAnimatorItem ani = new TAnimatorItem();

    void Awake()
    {
        //DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 指定每個EPlayerState 要傳的參數編號 for Animator
    /// </summary>
	public void InitAnimtorStatesType()
	{
		foreach (EAnimatorState item in Enum.GetValues(typeof(EAnimatorState)))
		{
			if (!AnimtorStatesType.ContainsKey(item))
			{
				AnimtorStatesType.Add(item, new Dictionary<EPlayerState, int>());
				
				switch (item)
				{
				case EAnimatorState.Idle:
					AnimtorStatesType [item].Add(EPlayerState.Idle, 0);
					break;
				case EAnimatorState.Run:
					AnimtorStatesType [item].Add(EPlayerState.Run0, 0);
					AnimtorStatesType [item].Add(EPlayerState.Run1, 1);
					AnimtorStatesType [item].Add(EPlayerState.Run2, 2);
					AnimtorStatesType [item].Add(EPlayerState.RunningDefence, 0);
					break;
				case EAnimatorState.Push:
					AnimtorStatesType [item].Add(EPlayerState.Push0, 0);
					AnimtorStatesType [item].Add(EPlayerState.Push1, 1);
					AnimtorStatesType [item].Add(EPlayerState.Push2, 2);
					AnimtorStatesType [item].Add(EPlayerState.Push20, 20);
					break;
				case EAnimatorState.Rebound:
					AnimtorStatesType [item].Add(EPlayerState.Rebound0, 0);
					AnimtorStatesType [item].Add(EPlayerState.Rebound20, 20);
					break;
				case EAnimatorState.Defence:
					AnimtorStatesType [item].Add(EPlayerState.Defence0, 0);
					AnimtorStatesType [item].Add(EPlayerState.Defence1, 1);
					break;
				case EAnimatorState.Steal:
					AnimtorStatesType [item].Add(EPlayerState.Steal0, 0);
					AnimtorStatesType [item].Add(EPlayerState.Steal1, 1);
					AnimtorStatesType [item].Add(EPlayerState.Steal2, 2);
					AnimtorStatesType [item].Add(EPlayerState.Steal20, 20);
					break;
				case EAnimatorState.Buff:
					AnimtorStatesType [item].Add(EPlayerState.Buff20, 20);
					AnimtorStatesType [item].Add(EPlayerState.Buff21, 21);
					break;
				case EAnimatorState.Block:
					AnimtorStatesType [item].Add(EPlayerState.Block0, 0);
					AnimtorStatesType [item].Add(EPlayerState.Block1, 1);
					AnimtorStatesType [item].Add(EPlayerState.Block2, 2);
					AnimtorStatesType [item].Add(EPlayerState.Block20, 20);
					break;
				case EAnimatorState.Catch:
					AnimtorStatesType [item].Add(EPlayerState.CatchFlat, 0);
					AnimtorStatesType [item].Add(EPlayerState.CatchFloor, 2);
					AnimtorStatesType [item].Add(EPlayerState.CatchParabola, 1);
					break;
				case EAnimatorState.Pick:
					AnimtorStatesType [item].Add(EPlayerState.Pick0, 0);
					AnimtorStatesType [item].Add(EPlayerState.Pick1, 1);
					AnimtorStatesType [item].Add(EPlayerState.Pick2, 2);
					break;
				case EAnimatorState.Intercept:
					AnimtorStatesType [item].Add(EPlayerState.Intercept0, 0);
					AnimtorStatesType [item].Add(EPlayerState.Intercept1, 1);
					break;
				case EAnimatorState.BlockCatch:
					AnimtorStatesType [item].Add(EPlayerState.BlockCatch, 0);
					break;
				case EAnimatorState.HoldBall:
					AnimtorStatesType [item].Add(EPlayerState.HoldBall, 0);
					break;
				case EAnimatorState.Dribble:
					AnimtorStatesType [item].Add(EPlayerState.Dribble0, 0);   
					AnimtorStatesType [item].Add(EPlayerState.Dribble1, 1);   
					AnimtorStatesType [item].Add(EPlayerState.Dribble2, 2);   
					AnimtorStatesType [item].Add(EPlayerState.Dribble3, 3);   
					break;
				case EAnimatorState.MoveDodge:
					AnimtorStatesType [item].Add(EPlayerState.MoveDodge0, 0);
					AnimtorStatesType [item].Add(EPlayerState.MoveDodge1, 1);
					break;
				case EAnimatorState.Elbow:
					AnimtorStatesType [item].Add(EPlayerState.Elbow0, 0);
					AnimtorStatesType [item].Add(EPlayerState.Elbow1, 1);
					AnimtorStatesType [item].Add(EPlayerState.Elbow2, 2);
					AnimtorStatesType [item].Add(EPlayerState.Elbow20, 20);
					AnimtorStatesType [item].Add(EPlayerState.Elbow21, 21);
					break;
				case EAnimatorState.Layup:
					AnimtorStatesType [item].Add(EPlayerState.Layup0, 0);
					AnimtorStatesType [item].Add(EPlayerState.Layup1, 1);
					AnimtorStatesType [item].Add(EPlayerState.Layup2, 2);
					AnimtorStatesType [item].Add(EPlayerState.Layup3, 3);
					break;
				case EAnimatorState.TipIn:
					AnimtorStatesType [item].Add(EPlayerState.TipIn, 0);
					break;
				case EAnimatorState.Shoot:
					AnimtorStatesType [item].Add(EPlayerState.Shoot0, 0);
					AnimtorStatesType [item].Add(EPlayerState.Shoot1, 1);
					AnimtorStatesType [item].Add(EPlayerState.Shoot2, 2);
					AnimtorStatesType [item].Add(EPlayerState.Shoot3, 3);
					AnimtorStatesType [item].Add(EPlayerState.Shoot4, 4);
					AnimtorStatesType [item].Add(EPlayerState.Shoot5, 5);
					AnimtorStatesType [item].Add(EPlayerState.Shoot6, 6);
					AnimtorStatesType [item].Add(EPlayerState.Shoot7, 7);
					AnimtorStatesType [item].Add(EPlayerState.Shoot20, 20);
					break;
				case EAnimatorState.Dunk:
					AnimtorStatesType [item].Add(EPlayerState.Dunk0, 0);
					AnimtorStatesType [item].Add(EPlayerState.Dunk1, 1);
					AnimtorStatesType [item].Add(EPlayerState.Dunk2, 2);
					AnimtorStatesType [item].Add(EPlayerState.Dunk3, 3);
					AnimtorStatesType [item].Add(EPlayerState.Dunk4, 4);
					AnimtorStatesType [item].Add(EPlayerState.Dunk5, 5);
					AnimtorStatesType [item].Add(EPlayerState.Dunk6, 6);
					AnimtorStatesType [item].Add(EPlayerState.Dunk7, 7);
					AnimtorStatesType [item].Add(EPlayerState.Dunk20, 20);
					AnimtorStatesType [item].Add(EPlayerState.Dunk21, 21);
					AnimtorStatesType [item].Add(EPlayerState.Dunk22, 22);
					break;
					
				case EAnimatorState.FakeShoot:
					AnimtorStatesType [item].Add(EPlayerState.FakeShoot, 0);
					break;
				case EAnimatorState.End:
					AnimtorStatesType [item].Add(EPlayerState.Ending0, 0);
					AnimtorStatesType [item].Add(EPlayerState.Ending10, 10);
					break;
				case EAnimatorState.Fall:
					AnimtorStatesType [item].Add(EPlayerState.Fall0, 0);
					AnimtorStatesType [item].Add(EPlayerState.Fall1, 1);
					AnimtorStatesType [item].Add(EPlayerState.Fall2, 2);
					break;
				case EAnimatorState.GotSteal:
					AnimtorStatesType [item].Add(EPlayerState.GotSteal, 0);
					break;
				case EAnimatorState.KnockDown:
					AnimtorStatesType [item].Add(EPlayerState.KnockDown0, 0);
					AnimtorStatesType [item].Add(EPlayerState.KnockDown1, 1);
					break;
				case EAnimatorState.Pass:
					AnimtorStatesType [item].Add(EPlayerState.Pass0, 0);
					AnimtorStatesType [item].Add(EPlayerState.Pass1, 1);
					AnimtorStatesType [item].Add(EPlayerState.Pass2, 2);
					AnimtorStatesType [item].Add(EPlayerState.Pass3, 3);
					AnimtorStatesType [item].Add(EPlayerState.Pass4, 4);
					AnimtorStatesType [item].Add(EPlayerState.Pass5, 5);
					AnimtorStatesType [item].Add(EPlayerState.Pass6, 6);
					AnimtorStatesType [item].Add(EPlayerState.Pass7, 7);
					AnimtorStatesType [item].Add(EPlayerState.Pass8, 8);
					AnimtorStatesType [item].Add(EPlayerState.Pass9, 9);
					AnimtorStatesType [item].Add(EPlayerState.Pass50, 50);
					break;
				case EAnimatorState.JumpBall:
					AnimtorStatesType [item].Add(EPlayerState.JumpBall, 0);
					break;
				case EAnimatorState.Show:
					AnimtorStatesType [item].Add(EPlayerState.Show1, 1);
					AnimtorStatesType [item].Add(EPlayerState.Show1001, 1001);
					AnimtorStatesType [item].Add(EPlayerState.Show1003, 1003);
					AnimtorStatesType [item].Add(EPlayerState.Show101, 101);
					AnimtorStatesType [item].Add(EPlayerState.Show102, 102);
					AnimtorStatesType [item].Add(EPlayerState.Show103, 103);
					AnimtorStatesType [item].Add(EPlayerState.Show104, 104);
					AnimtorStatesType [item].Add(EPlayerState.Show201, 201);
					AnimtorStatesType [item].Add(EPlayerState.Show202, 202);
					break;
				case EAnimatorState.Alleyoop:
					AnimtorStatesType [item].Add(EPlayerState.Alleyoop, 0);
					break;
				}
			}
		}

        InitForciblyStates();
	}

    private void InitForciblyStates()
    {
        foreach (EAnimatorState item in Enum.GetValues(typeof(EAnimatorState)))
        {
            switch (item)
            {
                case EAnimatorState.Push:
                case EAnimatorState.Rebound:
                case EAnimatorState.Block:
                case EAnimatorState.Steal:
                case EAnimatorState.Buff:
                case EAnimatorState.MoveDodge:
                case EAnimatorState.Elbow:
                case EAnimatorState.Layup:
                case EAnimatorState.TipIn:
                case EAnimatorState.Shoot:
                case EAnimatorState.Dunk:
                case EAnimatorState.FakeShoot:
                case EAnimatorState.Fall:
                case EAnimatorState.GotSteal:
                case EAnimatorState.KnockDown:
                case EAnimatorState.Pass:
                case EAnimatorState.JumpBall:
                case EAnimatorState.Alleyoop:
                    if(AnimtorStatesType.ContainsKey(item))
                        foreach (KeyValuePair<EPlayerState, int> state in AnimtorStatesType[item])
                        {
                            if (!ForciblyStates.ContainsKey(state.Key))
                            {
                                ForciblyStates.Add(state.Key, true);
                            }
                        }
                    break;
            }
        }
    }
        
    public bool IsForciblyStates(EPlayerState state)
    {
        return ForciblyStates.ContainsKey(state);
    }
	
	public TAnimatorItem GetAnimatorStateType(EPlayerState playerState)
	{
		try {
			foreach (EAnimatorState item in Enum.GetValues(typeof(EAnimatorState))){
				if (AnimtorStatesType.ContainsKey (item) && AnimtorStatesType[item].ContainsKey(playerState)){
					ani.Type = item;
					ani.StateNo = AnimtorStatesType[item][playerState];
					return ani;
				}
			}

			Debug.Log("Animator state not found " + playerState.ToString());
			ani.Type = EAnimatorState.Idle;
			ani.StateNo = 0;
			return ani;
		} catch (Exception e) {
			Debug.Log(e.Message);
			ani.Type = EAnimatorState.Idle;
			ani.StateNo = 0;
			return ani;
		}
	}

	public bool IsLoopState(EPlayerState state)
	{
		return StateChecker.LoopStates.ContainsKey (state); 
	}
}

