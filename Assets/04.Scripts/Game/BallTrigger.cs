using UnityEngine;
using System.Collections;

public class BallTrigger : MonoBehaviour
{
	private GameObject followObject;
	private BoxCollider box;
	private Rigidbody parentobjRigidbody;
	private GameObject HintObject;

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

	public void PassBall(GameObject from , GameObject to)
	{
		followObject = to;
		PlayerBehaviour p = followObject.GetComponent<PlayerBehaviour>();
		if(p)
			SceneMgr.Inst.RealBall.rigidbody.velocity = GameFunction.GetVelocity(SceneMgr.Inst.RealBall.transform.position, p.DummyBall.transform.position, Random.Range(40, 60));
		else
			SceneMgr.Inst.RealBall.rigidbody.velocity = GameFunction.GetVelocity(SceneMgr.Inst.RealBall.transform.position, followObject.transform.position, Random.Range(40, 60));

//		if(UIGame.Get.Game.Catcher)
//		{
//			float ang = GameFunction.ElevationAngle(SceneMgr.Inst.RealBall.transform.position, UIGame.Get.Game.Catcher.transform.position);                                                                                                                           
//			float shootAng = 30 + ang;
//			shootAng = Mathf.Clamp(shootAng, 40, 60);
//			
//			Vector3 v = UIGame.Get.Game.Catcher.transform.position;
//			if (new Vector3(UIGame.Get.Game.Catcher.TargetPos.x, 2, UIGame.Get.Game.Catcher.TargetPos.y) != Vector3.zero && UIGame.Get.Game.Catcher.rigidbody.velocity != Vector3.zero)
//			{
//				v = GameFunction.calculateTrajectory(UIGame.Get.Game.Catcher.transform.position, 
//				                        UIGame.Get.Game.Catcher.TargetPos, 
//				                        UIGame.Get.Game.Catcher.rigidbody.velocity, 
//				                        Vector3.Distance(UIGame.Get.Game.Catcher.transform.position, transform.position));
//			}
//			
//			v.y = 1;
//			Vector3 v2 = GameFunction.GetVelocity(SceneMgr.Inst.RealBall.transform.position, v, shootAng);
//			
//			if (UIGame.Visible && (int)UIGame.Get.Game.situation > 2)
//			{
//				v2.x *= 3;
//				v2.y = 0.3f;
//				v2.z *= 3;
//			} else
//			{
//				float dis = Vector3.Distance(v, transform.position);
//				if (dis <= 13)
//				{
//					RaycastHit hit;  
//					LayerMask mask = 1 << LayerMask.NameToLayer("Player");
//					Vector3 fwd = transform.TransformDirection(Vector3.forward);
//					bool flag = false;
//					if (Physics.Raycast(transform.position, fwd, out hit, 40, mask))
//					{
//						flag = hit.collider.gameObject.GetComponent<PlayerBehaviour>() != UIGame.Get.Game.Catcher;
//					}
//					
//					if (!flag)
//					{
//						v2.x *= 3;
//						v2.y = 0.3f;
//						v2.z *= 3;
//					}
//				}
//			}
//			
//			SceneMgr.Inst.RealBall.rigidbody.velocity = v2;
//		}
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

		if (followObject) {
			if (Vector3.Distance (parentobjRigidbody.gameObject.transform.position, followObject.transform.position) > UIGame.Get.Game.PickBallDis)
				parentobjRigidbody.gameObject.transform.position = Vector3.Lerp(parentobjRigidbody.gameObject.transform.position, followObject.transform.position, 0.1f);
			else
			{
				followObject = null;
				UIGame.Get.Game.SetBall(UIGame.Get.Game.Catcher);
				UIGame.Get.Game.Passing = false;
			}
		}
	}
	
}