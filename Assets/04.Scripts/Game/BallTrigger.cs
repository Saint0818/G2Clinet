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
		if (UIGame.Get.Game.Catcher) {
			passing = true;
			UIGame.Get.Game.SetBallState(PlayerState.Pass);
			SceneMgr.Inst.RealBall.rigidbody.velocity = GameFunction.GetVelocity(SceneMgr.Inst.RealBall.transform.position, UIGame.Get.Game.Catcher.DummyBall.transform.position, Random.Range(40, 60));	
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
			if (UIGame.Get.Game.Catcher) {
				if (Vector3.Distance (parentobjRigidbody.gameObject.transform.position, UIGame.Get.Game.Catcher.transform.position) > UIGame.Get.Game.PickBallDis){
					parentobjRigidbody.gameObject.transform.position = Vector3.Lerp(parentobjRigidbody.gameObject.transform.position, UIGame.Get.Game.Catcher.transform.position, 0.1f);
				}else{
					UIGame.Get.Game.SetBall(UIGame.Get.Game.Catcher);
					UIGame.Get.Game.Catcher.DelPass();
					UIGame.Get.Game.Catcher = null;
					passing = false;
				}
			}
			else
				passing = false;
		}
	}
}