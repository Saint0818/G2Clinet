using UnityEngine;
using System.Collections;

public class BallTrigger : MonoBehaviour
{
	private GameObject followObject;
	private BoxCollider box;
	private Rigidbody parentobjRigidbody;
	private GameObject HintObject;

	private float onFloorTime = 0;

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
//		if (other.gameObject.name == "Skill163(Clone)") {
//			TrackGameObject go = other.gameObject.GetComponent<TrackGameObject>();
//				if(go && go.isWork)
//					parentobjRigidbody.AddForce(other.gameObject.transform.forward * 100);
//
//			Destroy(other.gameObject);
//		}

		if (UIGame.Visible){
			if (other.gameObject.tag == "Player") 
			{
				if (parentobjRigidbody.isKinematic == false) 
					UIGame.Get.Game.SetBall (other.gameObject);
			}
			else 
			if (other.gameObject.tag == "Floor") 
			{
				followObject = null;
//				UIGame.Get.Game.ExShooter = null;
//				UIGame.Get.Game.RushToBall(1);	
			} else 
			if (other.gameObject.tag == "Wall") 
			{
				EffectManager.Get.PlayEffect("BallTouchWall", gameObject.transform.position);
			}
		}
//		else if (UIRecordGame.Visible){
//			if (other.gameObject.tag == "Player") 
//			{
//				if (parentobjRigidbody.isKinematic == false) {
//					UIRecordGame.Get.Game.SetBall (other.gameObject, true);
//				}
//			}
//			else if (other.gameObject.tag == "Floor") 
//			{		
//				if(!RecordGameController.next && UIRecordGame.Get.Game.ballController == null)
//					UIRecordGame.Get.Game.SetNext();
//			}
//			else if (other.gameObject.tag == "Wall") 
//			{		
//				if(!RecordGameController.next && UIRecordGame.Get.Game.ballController == null)
//					UIRecordGame.Get.Game.SetNext();
//			}
//		}
	}

	void Update()
	{
		gameObject.transform.localPosition = Vector3.zero;
	}

	void FixedUpdate ()
	{	
		followTarget();

		if (gameObject.transform.position.y <= 0.2) {
			onFloorTime += Time.deltaTime;
			if (onFloorTime >= 1) {
//				HintObject.SetActive(true);
				SetBoxColliderEnable(true);
			}
		} else {
//			onFloorTime = 0;
//			if (HintObject.activeInHierarchy) 
//				HintObject.SetActive(false);
		}
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

	private void followTarget() {
		if (parentobjRigidbody && gameObject.activeInHierarchy && !parentobjRigidbody.isKinematic &&
		    followObject && followObject.activeInHierarchy) {
				float dis = Vector3.Distance(gameObject.transform.position, followObject.transform.position);
				if (dis <= 2)
					UIGame.Get.Game.SetBall (followObject);
				else
				if (dis <= 8 && dis > 2) {
					dis = 8 - dis;

				float moveSpeed = dis * 10;
				Vector3 delta = followObject.transform.position - transform.position;
				parentobjRigidbody.AddForce(delta * moveSpeed); 
			}
		}
	}

	public void SetFollowObject(GameObject obj) {
		followObject = obj;
	}
}