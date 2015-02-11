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

		p1.Stop(true);

		if (isAi)
			p1.startColor = Color.gray;
		else
			p1.startColor = new Color(0.43f,1,0.93f,1);

		p1.Play(true);
	}

	public void InitCom()
	{
		p1 = gameObject.transform.FindChild ("View/1").GetComponent<ParticleSystem>();
	}

	// Update is called once per frame
	void Update () {
		if (target) {
			gameObject.transform.position = new Vector3(target.transform.position.x , 0, target.transform.position.z);	
		}
	}
}
