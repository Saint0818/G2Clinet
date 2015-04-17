using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct;

public class EffectManager : MonoBehaviour
{
	private static EffectManager instance = null;
	private static string[] GameEffects = {"ThreePointEffect", "TwoPointEffect", "ShockEffect", "BlockEffect", "DunkEffect", "StealEffect",
											"ThreeLineEffect", "ThrowInLineEffect"};
	private bool GameEffectLoaded = false;
	private Shake mShake;
	private Dictionary<string, GameObject> effectList = new Dictionary<string, GameObject>();

	void Awake()
	{
		mShake = CameraMgr.Get.CourtCamera.gameObject.AddComponent<Shake>();
	}

	public void PlayShake()
	{
		mShake.Play();
	}

	public static EffectManager Get
	{
		get
		{
			if(instance == null)
			{
				GameObject go = new GameObject("EffectManager");
				instance = go.AddComponent<EffectManager>();
			}
			return instance;
		}
	}

	private GameObject LoadEffect(string effectName) {
		GameObject obj = null;
		if(effectList.ContainsKey(effectName))
			obj = effectList[effectName];
		else {
			obj = (GameObject) Resources.Load("Prefab/Effect/"+effectName, typeof(GameObject));
			if(obj == null)
				obj = (GameObject)Resources.Load("Effect/" + effectName, typeof(GameObject));
			effectList.Add(effectName, obj);
		}

		return obj;
	}

	public void LoadGameEffect() {
		if (!GameEffectLoaded) {
			GameEffectLoaded = true;
			for (int i = 0; i < GameEffects.Length; i ++) 
				LoadEffect(GameEffects[i]);
		}
	}

	public GameObject PlayEffect(string effectName, Vector3 position, GameObject parent = null, GameObject followObj = null, float lifeTime = 0)
	{
		if (GameSetting.Effect) {
			GameObject obj = LoadEffect(effectName);
			
			if(obj != null) {
				GameObject particles = (GameObject)Instantiate(obj);
				particles.transform.position = position;
				particles.SetActive(true);
				particles.name = effectName;

				if(particles.GetComponent<ParticleSystem>() == null) {
					ParticleSystem ps =	particles.GetComponentInChildren<ParticleSystem>();
					if (ps)
						ps.Play();
				} else
					particles.GetComponent<ParticleSystem>().Play();

				if (lifeTime > 0)
				{
					AutoDestoryEffect autoDestory = particles.GetComponentInChildren<AutoDestoryEffect>();
					
					if(!autoDestory)
						autoDestory = particles.AddComponent<AutoDestoryEffect>();
					
					autoDestory.SetDestoryTime = lifeTime;
				}

				if (followObj) {
					FollowGameObject fo = particles.AddComponent<FollowGameObject>();
					fo.SetTarget(followObj);
					particles.transform.localPosition = position;
				} else
				if (parent) {
					particles.transform.parent = parent.transform;
					particles.transform.localPosition = position;
				} else
					particles.transform.position = position;

				particles.transform.localScale = Vector3.one;

				return particles;
			}
		}

		return null;
	}
}
