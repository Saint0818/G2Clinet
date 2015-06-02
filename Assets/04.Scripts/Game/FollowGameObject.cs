using UnityEngine;
using System.Collections;

public class FollowGameObject : MonoBehaviour {
	private bool isAI = false;
	public ParticleSystem p1;
	private GameObject target;

	public void SetTarget(GameObject obj)
	{
		target = obj;
		InitCom();
	}

	public void SetParticleColor(bool isAi)
	{
		if (isAI == isAi)
			return;

		isAI = isAi;

		if (!p1)
			InitCom ();

		if (p1) {
			p1.Stop(true);

			if (isAi)
				p1.startColor = Color.gray;
			else
				p1.startColor = new Color(0.43f,1,0.93f,1);

			p1.Play(true);
		}
	}

	public void InitCom()
	{
		Transform obj = gameObject.transform.FindChild ("View/1");
		if (obj)
			p1 = obj.GetComponent<ParticleSystem>();
	}

	void FixedUpdate () {
		if (target) 
			gameObject.transform.position = new Vector3(target.transform.position.x , 0.05f, target.transform.position.z);	
	}
}
