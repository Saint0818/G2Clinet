using UnityEngine;
using System.Collections;

public class AutoDestoryAudio : MonoBehaviour 
{
	public float DestoryDelayTime = 0;
	private bool startDestory = false;

	public float SetDestoryTime
	{
		get{return DestoryDelayTime;}
		set
		{
			DestoryDelayTime = value;
			if(DestoryDelayTime > 0)
				startDestory = true;
		}
	}

	void Update()
	{
		if (startDestory) 
		{
			DestoryDelayTime -= Time.deltaTime;
			if(DestoryDelayTime <= 0)
			  GameObject.Destroy(this.gameObject);
		} 
	}
}
