using UnityEngine;
using System.Collections;

public delegate bool OnSkillDCComplete();

public class SkillDCMove : MonoBehaviour {
	public static SkillDCMove instance = null;
	public GameObject Born;
	public GameObject Target;
	public bool IsMove = false;
	private float distance;
	void Awake(){
		instance = this;
	}
	void FixedUpdate() {
		if(IsMove){
			distance = Vector3.Distance(transform.position, Target.transform.position);
			transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, 10 * Time.deltaTime);
			if(distance <= 0.2f){
				if(GameController.Get.onSkillDCComplete != null)
					GameController.Get.onSkillDCComplete();
				gameObject.SetActive(false);
				transform.position = Born.transform.position;
			}
		}
	}
}
