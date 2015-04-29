using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BallTrigger : MonoBehaviour
{
	private Rigidbody ParentRigidbody;
	private GameObject followObject;
	private BoxCollider box;
	private GameObject HintObject;
	private Vector3 Parabolatarget;
	private float Parabolaspeed = 20;    
	private float ParaboladistanceToTarget; 
	private bool Parabolamove = true;  
	private bool Passing = false;
	private int PassKind = -1;
	private float PassCheckTime = 0;

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
	
	void OnTriggerEnter(Collider other) {
		if (GameController.Visible){
			if (other.gameObject.CompareTag("Player"))
			{

			}
			else if (other.gameObject.CompareTag("Floor")) 
			{
				GameController.Get.BallOnFloor();
			} 
			else if (other.gameObject.CompareTag("Wall"))
			{
				EffectManager.Get.PlayEffect("BallTouchWall", gameObject.transform.position);
			}
		}
	}

	public void Falling()
	{
		ParentRigidbody.AddForce (new Vector3 (0, -100, 0));
	}
	
	public bool PassBall(int Kind = 0)
	{
		if (GameController.Get.Catcher && GameController.Get.BallOwner != null && GameController.Get.IsPassing == false) {
			GameController.Get.Passer = GameController.Get.BallOwner;
			GameController.Get.SetBallOwnerNull();

			Passing = true;
			GameController.Get.IsPassing = true;
			PassCheckTime = Time.time + 2.5f;
			PassKind = Kind;
			if( Vector3.Distance(GameController.Get.Passer.transform.position, GameController.Get.Catcher.transform.position) > 15f)
				CameraMgr.Get.IsLongPass = true;

			SceneMgr.Get.SetBallState(PlayerState.PassFlat);
			float dis = Vector3.Distance(GameController.Get.Catcher.DummyBall.transform.position, SceneMgr.Get.RealBall.transform.position);
			float time = dis / (GameConst.BasicMoveSpeed * GameConst.AttackSpeedup * Random.Range(4, 6));

			switch(Kind)
			{
			case 0:
				SceneMgr.Get.RealBall.transform.DOMove(GameController.Get.Catcher.DummyBall.transform.position, time).OnComplete(PassEnd).SetEase(Ease.Linear).OnUpdate(PassUpdate);
				break;
			case 2:
				Vector3 [] pathay = new Vector3[2];
				pathay[0] = GetMiddlePosition(GameController.Get.Passer.transform.position, GameController.Get.Catcher.DummyBall.transform.position);
				pathay[1] = GameController.Get.Catcher.DummyBall.transform.position;
				SceneMgr.Get.RealBall.transform.DOPath(pathay, time).OnComplete(PassEnd).SetEase(Ease.Linear).OnUpdate(PassUpdate);
				break;
			case 1:
			case 3:
				Parabolamove = true;							  
				StartCoroutine(Parabola());
				break;
			}

			return true;
		}else
			return false;
	}

	private void PassUpdate()
	{
		if (GameController.Get.Catcher != null && GameController.Get.Passer != null) 
		{
			float dis = Vector3.Distance(SceneMgr.Get.RealBall.transform.position, GameController.Get.Catcher.transform.position);  
			if (dis < 3.5f && Passing)
			{
				Passing = false;
				if(PassKind == 0)
					GameController.Get.Catcher.AniState (PlayerState.CatchFlat, GameController.Get.Passer.transform.position);		
				else if(PassKind == 2)
					GameController.Get.Catcher.AniState (PlayerState.CatchFloor, GameController.Get.Passer.transform.position);	
			} 
		
			if(GameController.Get.Passer != null)
				GameController.Get.Passer.rotateTo(GameController.Get.Catcher.transform.position.x, GameController.Get.Catcher.transform.position.z); 
		}
	}
	
	private Vector3 GetMiddlePosition(Vector3 p1, Vector3 p2){
		Vector3 Result = Vector3.zero;
		Result.x = (p1.x + p2.x) / 2; 
		Result.y = 0; 
		Result.z = (p1.z + p2.z) / 2; 
		return Result;
	}

	void LateUpdate()
	{
		if (gameObject.activeInHierarchy) {
			if (gameObject.transform.position.y < 0)
				gameObject.transform.position = new Vector3(gameObject.transform.position.x, 0.3f, gameObject.transform.position.z);

			if (Mathf.Abs(gameObject.transform.position.x) > 20)
			    gameObject.transform.position = Vector3.zero;

			if (Mathf.Abs(gameObject.transform.position.z) > 20)
			    gameObject.transform.position = Vector3.zero;
		}
	}

	public void PassEnd(){
		PassCheckTime = 0;
		GameController.Get.SetEndPass();
		CameraMgr.Get.IsLongPass = false;
	}

	void FixedUpdate()
	{
		gameObject.transform.localPosition = Vector3.zero;
		if(GameController.Get.IsPassing && PassCheckTime > 0 && Time.time >= PassCheckTime)
		{
			PassCheckTime = 0;
			GameController.Get.Catcher = null;
			GameController.Get.IsPassing = false;
		}
	}	    

	IEnumerator Parabola()  
	{  		
		while (Parabolamove && GameController.Get.Catcher != null && GameController.Get.Passer != null)  
		{  
			float [] disAy = new float[4];
			disAy [0] = Vector3.Distance(GameController.Get.Catcher.DefPointAy[DefPointKind.FrontSteal.GetHashCode()].position, GameController.Get.Passer.transform.position);
			disAy [1] = Vector3.Distance(GameController.Get.Catcher.DefPointAy[DefPointKind.BackSteal.GetHashCode()].position, GameController.Get.Passer.transform.position);
			disAy [2] = Vector3.Distance(GameController.Get.Catcher.DefPointAy[DefPointKind.RightSteal.GetHashCode()].position, GameController.Get.Passer.transform.position);
			disAy [3] = Vector3.Distance(GameController.Get.Catcher.DefPointAy[DefPointKind.LeftSteal.GetHashCode()].position, GameController.Get.Passer.transform.position);
			int Index = MinIndex(disAy);
			
			Parabolatarget = new Vector3(GameController.Get.Catcher.DefPointAy[4 + Index].position.x, 0, GameController.Get.Catcher.DefPointAy[4 + Index].position.z);
			ParaboladistanceToTarget = Vector3.Distance(SceneMgr.Get.RealBall.transform.position, Parabolatarget);

			Vector3 targetPos = Parabolatarget;  
			SceneMgr.Get.RealBall.transform.LookAt(targetPos);  
			float angle = Mathf.Min(1, Vector3.Distance(SceneMgr.Get.RealBall.transform.position, targetPos) / ParaboladistanceToTarget) * 45;  
			SceneMgr.Get.RealBall.transform.rotation = SceneMgr.Get.RealBall.transform.rotation * Quaternion.Euler(Mathf.Clamp(-angle, -42, 42), 0, 0);  
			float currentDist = Vector3.Distance(SceneMgr.Get.RealBall.transform.position, Parabolatarget);  			 			

			Vector3 pos;

			if (currentDist <= 3.4f){
				Parabolamove = false;  
				PassEnd();
			}else if(currentDist <= 6 && Passing) 
			{
				Passing = false;
				pos = Vector3.forward * Mathf.Min(Parabolaspeed * Time.deltaTime, currentDist);
				SceneMgr.Get.SetRealBallOffset(pos);
				GameController.Get.Catcher.AniState(PlayerState.CatchParabola, GameController.Get.Passer.transform.position);
			}else
			{
				pos = Vector3.forward * Mathf.Min(Parabolaspeed * Time.deltaTime, currentDist);
				SceneMgr.Get.SetRealBallOffset(pos);
			}

			yield return null;  
		}  
	}

	private int MinIndex(float[] floatAy)
	{
		int Result = 0;
		
		float Min = floatAy [0];
		
		for (int i = 1; i < floatAy.Length; i++)
		{
			if (floatAy [i] < Min)
			{
				Min = floatAy [i];
				Result = i;
			}
		}

		return Result;	
	}
}