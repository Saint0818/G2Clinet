using UnityEngine;
using System.Collections;

public class AvatarAnimationTest : MonoBehaviour {
	public static AvatarAnimationTest Get;
	private Animator animator;

	public string[] AnimationName = {"Block","BlockCatch","BlockCatchDown","Catch","Dash","DashDribble","DefenceMove","DefenceStay","Dunk",
									"GoSteal","Move","MoveDribble","Pass","Pick","Push","ReboundCatch","ReboundDown","Reboundjump","Shoot",
									"ShootDown","ShootStay","Steal","Stop","ThrowIn","Walk"};

	void Awake(){
		Get = this;
		animator = gameObject.GetComponent<Animator>();
	}

	public void Play(string name){
		animator.SetTrigger(name);
	}
}
