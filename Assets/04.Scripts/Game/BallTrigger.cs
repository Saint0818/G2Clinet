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
		if (UIGame.Visible){
			if (other.gameObject.tag == "Player") 
			{

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
	}

	void Update()
	{
		gameObject.transform.localPosition = Vector3.zero;
	}

	void FixedUpdate ()
	{	

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

	public void SetFollowObject(GameObject obj) {
		followObject = obj;
	}
}