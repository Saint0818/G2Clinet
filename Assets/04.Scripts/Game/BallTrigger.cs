using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BallTrigger : MonoBehaviour
{
	private Rigidbody ParentRigidbody;
	private GameObject followObject;
	private BoxCollider box;
	private GameObject HintObject;
	private bool passing = false;
	private GameObject Parabolatarget;
	private float Parabolaspeed = 20;    
	private float ParaboladistanceToTarget; 
	private bool Parabolamove = true;  

//	private float onFloorTime = 0;

	void Awake()
	{
		ParentRigidbody = gameObject.transform.parent.transform.gameObject.GetComponent<Rigidbody>();
		box = gameObject.GetComponent<BoxCollider>();
//		HintObject = GameObject.Find("MoveTo");
//		HintObject.SetActive(false);
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
		if (!passing && GameController.Get.Catcher) {
			passing = true;

			SceneMgr.Get.SetBallState(PlayerState.PassFlat);
			float dis = Vector3.Distance(GameController.Get.Catcher.DummyBall.transform.position, SceneMgr.Get.RealBall.transform.position);
			float time = dis / (GameConst.BasicMoveSpeed * GameConst.AttackSpeedup * Random.Range(4, 6));
			Parabolatarget = GameController.Get.Catcher.DummyBall;	
			ParaboladistanceToTarget = Vector3.Distance(SceneMgr.Get.RealBall.transform.position, Parabolatarget.transform.position);

			switch(Kind)
			{
			case 0:
				SceneMgr.Get.RealBall.transform.DOMove(GameController.Get.Catcher.DummyBall.transform.position, time).OnComplete(PassEnd).SetEase(Ease.Linear);
				break;
			case 2:
				Vector3 [] pathay = new Vector3[2];
				pathay[0] = GetMiddlePosition(GameController.Get.BallOwner.transform.position, GameController.Get.Catcher.DummyBall.transform.position);
				pathay[1] = GameController.Get.Catcher.DummyBall.transform.position;
				SceneMgr.Get.RealBall.transform.DOPath(pathay, time).OnComplete(PassEnd).SetEase(Ease.Linear);
				break;
			case 1:
			case 3:
				Parabolamove = true;							  
				StartCoroutine(Parabola());
				break;
			}

			if(Vector3.Distance(SceneMgr.Get.RealBall.transform.position, GameController.Get.Catcher.DummyBall.transform.position) > 15f)
				CameraMgr.Get.IsLongPass = true;

			return true;
		}else
			return false;
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
		GameController.Get.SetEndPass();
		passing = false;
		CameraMgr.Get.IsLongPass = false;
	}

	void Update()
	{
		gameObject.transform.localPosition = Vector3.zero;
	}	    

	IEnumerator Parabola()  
	{  		
		while (Parabolamove)  
		{  
			Vector3 targetPos = Parabolatarget.transform.position;  
			SceneMgr.Get.RealBall.transform.LookAt(targetPos);  
			float angle = Mathf.Min(1, Vector3.Distance(SceneMgr.Get.RealBall.transform.position, targetPos) / ParaboladistanceToTarget) * 45;  
			SceneMgr.Get.RealBall.transform.rotation = SceneMgr.Get.RealBall.transform.rotation * Quaternion.Euler(Mathf.Clamp(-angle, -42, 42), 0, 0);  
			float currentDist = Vector3.Distance(SceneMgr.Get.RealBall.transform.position, Parabolatarget.transform.position);  
			if (currentDist < 0.5f){
				Parabolamove = false;  
				PassEnd();
			}  				
			SceneMgr.Get.RealBall.transform.Translate(Vector3.forward * Mathf.Min(Parabolaspeed * Time.deltaTime, currentDist));  
			yield return null;  
		}  
	}
}