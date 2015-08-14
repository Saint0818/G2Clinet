using UnityEngine;
using System.Collections;

public class AutoDestoryEffect : MonoBehaviour 
{
	public float DestoryDelayTime = 0;
	public bool OnlyDeactivate;
	private ParticleSystem effect;
	private bool startDestory = false;
	public bool IsNeedPause = true;

	void OnEnable()
	{
		effect = gameObject.GetComponent<ParticleSystem> ();

		if (effect == null)
			effect = gameObject.GetComponentInChildren<ParticleSystem> ();

		if (DestoryDelayTime == 0)
			StartCoroutine ("CheckIfAlive");
		else
			startDestory = true;
	}

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

	IEnumerator CheckIfAlive ()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.5f);
			if(effect && !effect.IsAlive(true))
			{
				if(OnlyDeactivate)
				{
					#if UNITY_3_5
					this.gameObject.SetActiveRecursively(false);
					#else
					this.gameObject.SetActive(false);
					#endif
				}
				else
					GameObject.Destroy(this.gameObject);
				break;
			}
		}
	}

	void Update()
	{
		if (startDestory) 
		{
			DestoryDelayTime -= Time.deltaTime * TimerMgr.Get.CrtTime;
			if(DestoryDelayTime <= 0)
			  GameObject.Destroy(this.gameObject);
		} 
		if(IsNeedPause) {
			if(TimerMgr.Get.CrtTime == 0)
			{
				effect.Pause(true);
			}
			else
			{
				if(effect.isPaused)
					effect.Play(true);
			}
		}
	}
}
