using UnityEngine;
using System.Collections;

public class BlockCatchTrigger: MonoBehaviour {
	private PlayerBehaviour blocker;
	private BoxCollider boxCollider;
	private bool isOpen = false;

	void Awake()
	{
		boxCollider = gameObject.AddComponent<BoxCollider> ();
		boxCollider.size = new Vector3(1, 1.5f, 2);
		boxCollider.center = new Vector3 (0.3f, 0, 0);
		boxCollider.isTrigger = true;
	}

	void Start()
	{
		blocker = gameObject.transform.parent.gameObject.GetComponent<PlayerBehaviour>();
	}

	public void SetEnable(bool flag)
	{
		isOpen = flag;
		boxCollider.enabled = isOpen;
	}

	void OnTriggerEnter(Collider other) {
		if (isOpen && GameController.Visible && other.gameObject.CompareTag ("RealBall")) {
			if(blocker == null)
			{
				blocker = gameObject.transform.parent.gameObject.GetComponent<PlayerBehaviour>();
			}
			else{
				if(GameController.Get.Shooter == null){
					blocker.IsPerfectBlockCatch = true;
				}
				else if(GameController.Get.Shooter && GameController.Get.Shooter != blocker)
					blocker.IsPerfectBlockCatch = true;
			}
		}
	}
}
