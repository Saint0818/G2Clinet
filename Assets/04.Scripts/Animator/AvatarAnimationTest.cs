using UnityEngine;
using System.Collections;

public class AvatarAnimationTest : MonoBehaviour {
	public static AvatarAnimationTest Get;
	private Animator animator;

	void Awake(){
		Get = this;
		animator = gameObject.GetComponent<Animator>();
	}

	public void Play(string name){
		animator.SetTrigger(name);
	}
}
