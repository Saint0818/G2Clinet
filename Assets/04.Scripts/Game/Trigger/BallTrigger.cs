using UnityEngine;
using System.Collections;
using DG.Tweening;
using JetBrains.Annotations;

public class BallTrigger : MonoBehaviour
{
	private Rigidbody ParentRigidbody;
	private GameObject followObject;
	private BoxCollider box;
	private GameObject HintObject;
	private Vector3 Parabolatarget;
	private float ParaboladistanceToTarget; 
	private bool Parabolamove = false;  
	private bool Passing = false;
	public static int PassKind = -1;
	private float ParabolaTime = 0;
	private float ParabolaDis = 0;
	private TBallCurve BallHeight;
	private bool isAutoRotate = false;

	void Awake()
	{
		ParentRigidbody = gameObject.transform.parent.transform.gameObject.GetComponent<Rigidbody>();
		box = gameObject.GetComponent<BoxCollider>();
	}

	public void SetBoxColliderEnable(bool isShow)
	{
		if(box)
			box.enabled = isShow;
	}

	private bool touchPlayer(Collider other, bool isEnter)
    {
        // -1: 沒碰到 trigger, 0: TriggerTop, 1:TriggerFR, 3:TriggerBR, 5:TriggerFinger, 6:TriggerSteal.
        int dir = -1;

        // -1: 
		int team = -1;

        // 
		int index = -1;
		if(other.gameObject.name == "TriggerTop")
			dir = 0;
		else if(other.gameObject.name == "TriggerFR")
			dir = 1;
		else if(other.gameObject.name == "TriggerBR")
			dir = 3;
		else if(other.gameObject.name.Contains("TriggerFinger"))
        {
            // 球和 Player 接觸.
            dir = 5;
			int.TryParse(other.gameObject.name[0].ToString(), out team); 
			int.TryParse(other.gameObject.name[1].ToString(), out index); 
		}
        else if(other.gameObject.name == "TriggerSteal")
			dir = 6;

		if (dir > -1)
        {
			if (team > -1 && index > -1 && GameController.Get.Situation == EGameSituation.JumpBall)
            {
				GameController.Get.BallTouchPlayer(team * 3 + index, dir, isEnter);
			}
            else if(other.gameObject.transform.parent && other.gameObject.transform.parent.parent)
            {
				PlayerBehaviour player = other.gameObject.transform.parent.parent.GetComponent<PlayerBehaviour>();
				if(player)
                {
					if(isEnter)
					{
						if(GameController.Get.PassingStealBall(player, dir)) 
							GameController.Get.BallTouchPlayer(player, dir, true);
					}
                    else
						GameController.Get.BallTouchPlayer(player, dir, false);
					return true;
				}
			}
		}

		return false;
	}
	
    [UsedImplicitly]
	void OnTriggerEnter(Collider other)
    {
		if(touchPlayer(other, true))
        {

		}
        else if(other.gameObject.CompareTag("Floor")) 
		{
			AudioMgr.Get.PlaySound (SoundType.SD_dribble);
			GameController.Get.BallOnFloor();
			IsAutoRotate = false;
		} 
		else if(other.gameObject.CompareTag("Wall"))
		{
			EffectManager.Get.PlayEffect("BallTouchWall", gameObject.transform.position);
		}
	}

	void OnTriggerStay(Collider other) {
		touchPlayer(other, false);
	}

	public void Falling()
	{
		ParentRigidbody.AddForce (new Vector3 (0, -100, 0));
	}
	
	public bool PassBall(int kind = 0)
	{
		if(GameController.Get.Catcher && GameController.Get.BallOwner != null && 
           GameController.Get.IsPassing == false)
        {
			IsAutoRotate = true;
			GameController.Get.Passer = GameController.Get.BallOwner;
			GameController.Get.SetBallOwnerNull();

			Passing = true;
			GameController.Get.IsPassing = true;
//			PassCheckTime = Time.time + 2.5f;
			PassKind = kind;
			if( Vector3.Distance(GameController.Get.Passer.transform.position, GameController.Get.Catcher.transform.position) > 15f)
				CameraMgr.Get.IsLongPass = true;

			CourtMgr.Get.SetBallState (EPlayerState.Pass0);
			float dis = Vector3.Distance (GameController.Get.Catcher.CatchBallPoint.transform.position, CourtMgr.Get.RealBall.transform.position);
			float time = dis / (GameConst.AttackSpeedup * Random.Range (3, 5));

			switch(kind)
			{
			case 2:
				Vector3 [] pathay = new Vector3[2];
				pathay[0] = GetMiddlePosition(GameController.Get.Passer.transform.position, GameController.Get.Catcher.CatchBallPoint.transform.position);
				pathay[1] = GameController.Get.Catcher.CatchBallPoint.transform.position;
				CourtMgr.Get.RealBall.transform.DOPath(pathay, time * 1/GameController.Get.Passer.Timer.timeScale).OnComplete(PassEnd).SetEase(Ease.Linear).OnUpdate(PassUpdate);
				CourtMgr.Get.RealBall.transform.DOPunchRotation(new Vector3(0, 0, 720), time * 1/GameController.Get.Passer.Timer.timeScale, 10, 1);
				break;
			case 1:
				ParabolaTime = 0;
				Parabolamove = true;
				Parabolatarget = CourtMgr.Get.RealBall.transform.position;
				ParabolaDis = Vector3.Distance(CourtMgr.Get.RealBall.transform.position, GameController.Get.Catcher.transform.position); 
				switch(GameController.Get.Passer.Attribute.BodyType)
				{
				case 0:
					BallHeight = CourtMgr.Get.RealBallCurve.Ball_Type0;
					break;
				case 1:
					BallHeight = CourtMgr.Get.RealBallCurve.Ball_Type1;
					break;
				default:
					BallHeight = CourtMgr.Get.RealBallCurve.Ball;
					break;
				}
				break;
			case 3:
				GameController.Get.Catcher.AniState (EPlayerState.CatchFlat, GameController.Get.Passer.transform.position);	
//				PassEnd();
				break;

			case 99://Alleyoop
				CourtMgr.Get.RealBall.transform.DOMove(GameController.Get.Catcher.CatchBallPoint.transform.position, GameConst.AlleyoopPassTime * 1/GameController.Get.Passer.Timer.timeScale).OnComplete(PassEnd).SetEase(Ease.Linear).OnUpdate(PassUpdate);
				CourtMgr.Get.RealBall.transform.DOPunchRotation(new Vector3(0, 0, 720), time * 1/GameController.Get.Passer.Timer.timeScale, 10, 1);
				break;

			default:
				CourtMgr.Get.RealBall.transform.DOMove(GameController.Get.Catcher.CatchBallPoint.transform.position, time * 1/GameController.Get.Passer.Timer.timeScale).OnComplete(PassEnd).SetEase(Ease.Linear).OnUpdate(PassUpdate);
				CourtMgr.Get.RealBall.transform.DOPunchRotation(new Vector3(0, 0, 720), time * 1/GameController.Get.Passer.Timer.timeScale, 10, 1);
				break;
			}

			return true;
		}else
			return false;
	}
	
	private void CalculationParabolaMove()
	{
		if (Parabolamove)
		{
			if(GameController.Get.Catcher && GameController.Get.Passer)
			{
				IsAutoRotate = true;

				ParabolaTime += Time.deltaTime * GameController.Get.Passer.Timer.timeScale; 
				float X = 0;
				float Z = 0;

				if(ParabolaDis < 8){
					Parabolatarget.y =  CourtMgr.Get.RealBallCurve.ShortBall.aniCurve.Evaluate(ParabolaTime);
					X = ((GameController.Get.Catcher.transform.position.x - Parabolatarget.x) / CourtMgr.Get.RealBallCurve.ShortBall.LifeTime) * ParabolaTime;
					Z = ((GameController.Get.Catcher.transform.position.z - Parabolatarget.z) / CourtMgr.Get.RealBallCurve.ShortBall.LifeTime) * ParabolaTime;
				}else{
					Parabolatarget.y =  BallHeight.aniCurve.Evaluate(ParabolaTime);
					X = ((GameController.Get.Catcher.transform.position.x - Parabolatarget.x) / CourtMgr.Get.RealBallCurve.Ball.LifeTime) * ParabolaTime;
					Z = ((GameController.Get.Catcher.transform.position.z - Parabolatarget.z) / CourtMgr.Get.RealBallCurve.Ball.LifeTime) * ParabolaTime;
				}

				if (Parabolatarget.y < 0)
					Parabolatarget.y = 0.5f;
				
				CourtMgr.Get.RealBall.transform.position = new Vector3(Parabolatarget.x + X, Parabolatarget.y, Parabolatarget.z + Z);
				
				if(ParabolaDis < 8){
					if (ParabolaTime >= CourtMgr.Get.RealBallCurve.ShortBall.LifeTime)
					{
						if(GameController.Get.BallOwner == null)				
							CourtMgr.Get.SetBallState(EPlayerState.Steal0, GameController.Get.Passer);
						
						Parabolamove = false;
					}
				}else{
					if (ParabolaTime >= CourtMgr.Get.RealBallCurve.Ball.LifeTime)
					{
						if(GameController.Get.BallOwner == null)				
							CourtMgr.Get.SetBallState(EPlayerState.Steal0, GameController.Get.Passer);
						
						Parabolamove = false;
					}
				}
				
				PassUpdate();
			}else
				Parabolamove = false;
		}
	}
	
	private void PassUpdate()
	{
		if (GameController.Get.Passer && GameController.Get.Passer.Timer.timeScale == 0)
		 return;

		if (GameController.Get.Catcher != null && GameController.Get.Passer != null) 
		{
			if((Parabolamove && ParabolaTime >= 0.2f) || !Parabolamove && GameController.Get.Catcher != null)
			{
				float dis = Vector3.Distance(CourtMgr.Get.RealBall.transform.position, GameController.Get.Catcher.transform.position);  

				if(PassKind == 1){
					if(ParabolaDis < 8)
					{
						if ((dis < 5) && Passing)
						{
							Passing = false;
							if(PassKind == 0)
								GameController.Get.Catcher.AniState (EPlayerState.CatchFlat, GameController.Get.Passer.transform.position);		
							else if(PassKind == 2)
								GameController.Get.Catcher.AniState (EPlayerState.CatchFloor, GameController.Get.Passer.transform.position);	
							else
								GameController.Get.Catcher.AniState(EPlayerState.CatchParabola, GameController.Get.Passer.transform.position);
						}else if(dis <= 2.8f)
						{
							Parabolamove = false;
							PassEnd();
						}
					}
					else
					{
						if(dis < 6 && Passing)
						{
							Passing = false;
							if(PassKind == 0)
								GameController.Get.Catcher.AniState (EPlayerState.CatchFlat, GameController.Get.Passer.transform.position);		
							else if(PassKind == 2)
								GameController.Get.Catcher.AniState (EPlayerState.CatchFloor, GameController.Get.Passer.transform.position);	
							else
								GameController.Get.Catcher.AniState(EPlayerState.CatchParabola, GameController.Get.Passer.transform.position);
						}
                        else if(dis <= 2.5f)
						{
							Parabolamove = false;
							PassEnd();
						}
					}
				} else if(PassKind == 99) {
					if(!GameController.Get.Catcher.IsAlleyoopState)
						PassEnd();
				} else{
					if ((dis < 8) && Passing)
					{
						Passing = false;
						if(PassKind == 0)
							GameController.Get.Catcher.AniState (EPlayerState.CatchFlat, GameController.Get.Passer.transform.position);		
						else if(PassKind == 2)
							GameController.Get.Catcher.AniState (EPlayerState.CatchFloor, GameController.Get.Passer.transform.position);	
						else
							GameController.Get.Catcher.AniState(EPlayerState.CatchParabola, GameController.Get.Passer.transform.position);
					}
				}

				switch(PassKind)
				{
					case 0:
					case 1:
					case 2:
					case 3:
					if(GameController.Get.Passer != null && GameController.Get.Catcher != null)
						GameController.Get.Passer.RotateTo(GameController.Get.Catcher.transform.position.x, GameController.Get.Catcher.transform.position.z); 
					break;
					default:
						break;
				}

			}
		}
	}
	
	private Vector3 GetMiddlePosition(Vector3 p1, Vector3 p2){
		Vector3 Result = Vector3.zero;
		Result.x = (p1.x + p2.x) / 2; 
		Result.y = 0; 
		Result.z = (p1.z + p2.z) / 2; 
		return Result;
	}

	public void PassEnd(){
		GameController.Get.SetEndPass();
		CameraMgr.Get.IsLongPass = false;
	}

	void FixedUpdate()
	{
		CalculationParabolaMove();
		gameObject.transform.localPosition = Vector3.zero;

		if (IsAutoRotate) {
			ParentRigidbody.gameObject.transform.Rotate (ParentRigidbody.gameObject.transform.forward * -10);
		}
	}

	public bool IsAutoRotate 
	{
		get{ return isAutoRotate;}
		set{isAutoRotate = value;}
	}
}