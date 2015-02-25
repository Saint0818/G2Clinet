using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour
{
	private static EffectManager instance = null;
	private static string[] GameEffects = {"ThreePointEffect", "TwoPointEffect", "ShockEffect", "BlockEffect", "DunkEffect", "StealEffect"};
	private bool GameEffectLoaded = false;
	private Shake mShake;
	private Dictionary<string, GameObject> effectList = new Dictionary<string, GameObject>();
	public FollowGameObject SelectEffectScript;

	void Awake()
	{
		mShake = CameraMgr.Get.GetUICamera().gameObject.AddComponent<Shake>();
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

			GameObject SelectEffectobj = GameObject.Instantiate(LoadEffect("SelectEffect")) as GameObject;
			SelectEffectScript = SelectEffectobj.GetComponent<FollowGameObject>();
		}
	}

	public void PlayEffect(string effectName, Vector3 position, GameObject parent = null, Transform followObjPos = null)
	{
//		if (!TeamManager.Effect)
//			return;

		GameObject obj = LoadEffect(effectName);
		
		if(obj != null)
		{
			GameObject particles = (GameObject)Instantiate(obj);
			particles.transform.position = position;
			particles.name = effectName;

			if(effectName == "Skill170" && followObjPos)
			{
//				DrawLine line = particles.GetComponentInChildren<DrawLine>();
//				line.SetOriginPos(followObjPos);
			}

			if(particles.particleSystem == null) {
				ParticleSystem ps =	particles.GetComponentInChildren<ParticleSystem>();
				if (ps)
					ps.Play();
			}
			else
				particles.particleSystem.Play();

			if (parent)
				particles.transform.parent = parent.transform;
			else
				particles.transform.position = position;

			particles.transform.localScale = Vector3.one;
		}
	}

	public void PlayEffect(string effectName, GameObject parent, Vector3 localPos, float lifeTime = -1)
	{
//		if (!TeamManager.Effect)
//			return;

		GameObject obj = LoadEffect(effectName);
		
		if(obj != null)
		{
			GameObject particles = (GameObject)Instantiate(obj);

			if(lifeTime > 0)
			{
				AutoDestoryEffect autoDestory = particles.GetComponentInChildren<AutoDestoryEffect>();

				if(!autoDestory)
					autoDestory = particles.AddComponent<AutoDestoryEffect>();

				autoDestory.SetDestoryTime = lifeTime;
			}

			if(parent)
			{
				particles.transform.parent = parent.transform;
				particles.transform.localPosition = Vector3.zero;
			}

			particles.transform.localScale = Vector3.one;
			particles.transform.localPosition = localPos;
			particles.transform.localEulerAngles = Vector3.zero;
			
			if(particles.particleSystem == null) {
				ParticleSystem ps = particles.GetComponentInChildren<ParticleSystem>();
				if (ps)
					ps.Play();
			}
			else
				particles.particleSystem.Play();
		}
	}

	public void Play1633Effect(Vector3 worldPos, Vector3 scale, GameObject[] targets, int lifeTime)
	{
//		if (!TeamManager.Effect)
//			return;
		
		GameObject obj = LoadEffect("Skill1633");

		if(obj){
			GameObject particles = (GameObject)Instantiate(obj);
			particles.name = "Skill1633";
			particles.transform.position = worldPos;
			particles.transform.localScale = scale;

			if(lifeTime > 0)
			{
				AutoDestoryEffect autoDestory = particles.GetComponentInChildren<AutoDestoryEffect>();
				if(!autoDestory)
					autoDestory = particles.AddComponent<AutoDestoryEffect>();
				
				autoDestory.SetDestoryTime = lifeTime;
			}
			
			if(particles.particleSystem == null){
				ParticleSystem ps = particles.GetComponentInChildren<ParticleSystem>();
				if (ps)
					ps.Play();
			} else
				particles.particleSystem.Play();

//			SkillBlackHole skill = particles.GetComponent<SkillBlackHole>();
//			
//			if(skill)
//				skill.InitTargets(targets);
		}
	}
}
