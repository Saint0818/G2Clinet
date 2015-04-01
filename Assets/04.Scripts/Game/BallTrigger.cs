using UnityEngine;
using System.Collections;

public class BallTrigger : MonoBehaviour
{
	private Rigidbody ParentRigidbody;
	private GameObject followObject;
	private BoxCollider box;
	private GameObject HintObject;
	private bool passing = false;

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

	public bool PassBall()
	{
		if (!passing && GameController.Get.Catcher) {
			passing = true;
			SceneMgr.Get.SetBallState(PlayerState.Pass);
			SceneMgr.Get.RealBall.GetComponent<Rigidbody>().velocity = GameFunction.GetVelocity(SceneMgr.Get.RealBall.transform.position, GameController.Get.Catcher.DummyBall.transform.position, Random.Range(40, 60));	
			if(Vector3.Distance(SceneMgr.Get.RealBall.transform.position, GameController.Get.Catcher.DummyBall.transform.position) > 15f)
				CameraMgr.Get.IsLongPass = true;

			return true;
		}else
			return false;
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

	void Update()
	{
		gameObject.transform.localPosition = Vector3.zero;

		if(passing){
			if (GameController.Get.Catcher) {
				if (Vector3.Distance (SceneMgr.Get.RealBall.gameObject.transform.position, GameController.Get.Catcher.transform.position) > GameConst.PickBallDistance){
					ParentRigidbody.gameObject.transform.position = Vector3.Lerp(ParentRigidbody.gameObject.transform.position, GameController.Get.Catcher.transform.position, 0.1f);
				}else{
					GameController.Get.SetEndPass();
					passing = false;
					CameraMgr.Get.IsLongPass = false;
				}
			}
			else
				passing = false;
		}
	}
}