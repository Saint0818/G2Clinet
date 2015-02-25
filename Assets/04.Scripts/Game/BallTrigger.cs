using UnityEngine;
using System.Collections;

public class BallTrigger : MonoBehaviour
{
	private GameObject followObject;
	private BoxCollider box;
	private Rigidbody parentobjRigidbody;
	private GameObject HintObject;
	private bool passing = false;

//	private float onFloorTime = 0;

	void Awake()
	{
		parentobjRigidbody = gameObject.transform.parent.transform.gameObject.rigidbody;
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
		if (UIGame.Visible){
			if (other.gameObject.tag == "Player") 
			{

			}
			else 
			if (other.gameObject.tag == "Floor") 
			{
				GameController.Get.ShootController = null;
//				followObject = null;
//				UIGame.Get.Game.ExShooter = null;
//				UIGame.Get.Game.RushToBall(1);	
			} else 
			if (other.gameObject.tag == "Wall") 
			{
				EffectManager.Get.PlayEffect("BallTouchWall", gameObject.transform.position);
			}
		}
	}

	public void Falling()
	{
		parentobjRigidbody.AddForce (new Vector3 (0, -100, 0));
	}

	public bool PassBall()
	{
		if (!passing && GameController.Get.Catcher) {
			passing = true;
			SceneMgr.Get.SetBallState(PlayerState.Pass);
			SceneMgr.Get.RealBall.rigidbody.velocity = GameFunction.GetVelocity(SceneMgr.Get.RealBall.transform.position, GameController.Get.Catcher.DummyBall.transform.position, Random.Range(40, 60));	
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
				if (Vector3.Distance (SceneMgr.Get.RealBall.gameObject.transform.position, GameController.Get.Catcher.transform.position) > GameController.Get.PickBallDis){
					parentobjRigidbody.gameObject.transform.position = Vector3.Lerp(parentobjRigidbody.gameObject.transform.position, GameController.Get.Catcher.transform.position, 0.1f);
				}else{
					GameController.Get.SetBall(GameController.Get.Catcher);
					GameController.Get.Catcher.DelPass();
					GameController.Get.Catcher = null;
					passing = false;
				}
			}
			else
				passing = false;
		}
	}
}