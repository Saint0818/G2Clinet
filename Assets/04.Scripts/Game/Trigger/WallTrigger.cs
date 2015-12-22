using UnityEngine;
using System.Collections;

public enum SaveWall
{
	WallA,
	WallB,
	WallR,
	WallL,
	WallT,
	WallD,
	WallC1,
	WallC2,
	WallC3,
	WallC4
}

public class WallTrigger : MonoBehaviour
{
	private GameObject followObject;
	public SaveWall WallType = SaveWall.WallA;

	void OnTriggerEnter(Collider collisionInfo) {

		if((collisionInfo.CompareTag("Player") && collisionInfo.gameObject.layer != LayerMask.NameToLayer("UI3D")) || collisionInfo.CompareTag("RealBall"))
		{
			bool flag = collisionInfo.gameObject.GetComponent<Rigidbody>().isKinematic;
			
			if(collisionInfo.gameObject.GetComponent<Rigidbody>().isKinematic == false)
			{
				collisionInfo.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
				collisionInfo.gameObject.GetComponent<Rigidbody>().isKinematic = true;
			}
			
			Vector3 pos = collisionInfo.gameObject.transform.position;
			
			if(pos.y < 0)
				pos.y = 0;
			
			switch(WallType)
			{
			case SaveWall.WallA:
				pos.z = -17f;
				break;
			case SaveWall.WallB:
				pos.z = 17f;
				break;
			case SaveWall.WallL:
				pos.x = -9f;
				break;
			case SaveWall.WallR:
				pos.x = 9f;
				break;
			case SaveWall.WallD:
				pos.y = 0;
				break;
			case SaveWall.WallC1:
				pos.x = 9f;
				pos.z = 17f;
				break;
			case SaveWall.WallC2:
				pos.x = -9f;
				pos.z = 17f;
				break;
			case SaveWall.WallC3:
				pos.x = -9f;
				pos.z = -17f;
				break;
			case SaveWall.WallC4:
				pos.x = 9f;
				pos.z = -17f;
				break;

			}
			
			collisionInfo.gameObject.transform.position = pos;
			collisionInfo.gameObject.GetComponent<Rigidbody>().isKinematic = flag;
		}

	}

}
