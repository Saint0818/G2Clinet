using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameStruct;

public struct TCloneMesh {
	public GameObject Source;
	public int MaterialKind;
	public float DeltaTime;
	public float Timer;
	public int Count;
}

public class EffectManager : MonoBehaviour
{
	public float CloneLiveTime = 1;
	public GameObject ObjPool;
	private static EffectManager instance = null;
	private static string[] GameEffects = {"ThreePointEffect", "TwoPointEffect", "ShockEffect", "BlockEffect", "DunkEffect", "StealEffect",  "ThreeLineEffect", "ThrowInLineEffect", "DoubleClick01", "DoubleClick02"};
	private bool GameEffectLoaded = false;

	private List<TCloneMesh> cloneMeshs = new List<TCloneMesh>();
	private List<Material> materials = new List<Material>();
	private List<int> triangles = new List<int>();
	private List<GameObject> cloneObjects = new List<GameObject>();
	
	private Dictionary<string, GameObject> effectList = new Dictionary<string, GameObject>();

	void Awake() {

		Material mat = Resources.Load("Effect/Materials/CloneMesh0") as Material;
		if (mat)
			materials.Add(mat);

		ObjPool = new GameObject ();
		ObjPool.name = "ObjPool";
		ObjPool.transform.parent = gameObject.transform;
		ObjPool.SetActive (false);
	}

	public static EffectManager Get {
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

	void Update() {
		for (int i = cloneMeshs.Count-1; i >= 0; i--) {
			if (cloneMeshs[i].Count > 0) {
				TCloneMesh cm = cloneMeshs[i];
				cm.Timer += Time.deltaTime;
				if (cm.Timer >= cm.DeltaTime) {
					cm.Timer -= cm.DeltaTime;
					cm.Count--;

					cloneMesh(cm.Source, cm.MaterialKind, cm.DeltaTime, cm.Count);
				}
				
				cloneMeshs[i] = cm;
			} else {
				cloneMeshs.RemoveAt(i);
			}
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
			if(obj == null)
				obj = (GameObject)Resources.Load("Effect/Skill/" + effectName, typeof(GameObject));
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

	public GameObject PlayEffectFollowBallOwner(string effectName, Vector3 position)
	{
		if (GameData.Setting.Effect || IsCheckSpecial(effectName)) {
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

				particles.transform.localPosition = position;
				particles.transform.localScale = Vector3.one;				
				return particles;
			}
		}
		
		return null;
	}

	public GameObject PlayEffect(string effectName, Vector3 position, GameObject parent = null, GameObject followObj = null, float lifeTime = 0) {
		if (GameData.Setting.Effect || IsCheckSpecial(effectName)) {
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

	public bool IsCheckSpecial(string name)
	{
		string[] strs = new string[]{"SelectMe", "SelectA", "SelectB", "MoveTo", "BallHolder", "SkillSign", "SkillSign01", "PassMe", "PassA", "PassB", "SkillSign1", "SkillSign2", "SkillSign101", "SkillSign201"};

		for(int i = 0; i < strs.Length; i++)
		{
			if(strs[i] == name)
				return true;
		}
	
		return false;
	}

	private GameObject getCloneObject() {
		for (int i = 0; i < cloneObjects.Count; i ++)
			if (!cloneObjects[i].activeInHierarchy) {
				cloneObjects[i].SetActive(true);
				cloneObjects[i].GetComponent<AutoHide>().HideTime = CloneLiveTime;
				return cloneObjects[i];
		}

		GameObject obj = new GameObject();
		obj.name = "CloneMesh" + cloneObjects.Count.ToString();

		AutoHide autoHide = obj.AddComponent<AutoHide>();
		autoHide.HideTime = CloneLiveTime;

		obj.AddComponent<MeshFilter> ();
		obj.AddComponent<MeshRenderer> ();
		cloneObjects.Add(obj);

		return obj;
	}

	private void CloneObj(Mesh ClonMesh, GameObject source, int materialKind)
	{
		GameObject obj = getCloneObject();
		obj.transform.position = source.transform.position;
		obj.transform.eulerAngles = source.transform.eulerAngles;
		//obj.layer = source.layer;

		MeshFilter meshFilter = obj.GetComponent<MeshFilter> ();
		if (meshFilter) {
			meshFilter.mesh.Clear();
			meshFilter.mesh = ClonMesh;
		}

		if (materialKind < materials.Count) 
			obj.GetComponent<MeshRenderer> ().material = materials[materialKind];
	}

	private void cloneMesh(GameObject source, int materialKind = 0, float deltaTime = 0.5f, int count = 4) {
		if (source) {
			SkinnedMeshRenderer[] skinnMeshRenders = source.GetComponentsInChildren<SkinnedMeshRenderer> ();
			if (skinnMeshRenders.Length > 0) {
				CombineInstance[] combine = new CombineInstance[skinnMeshRenders.Length];
				
				for(int skinnMeshNum = 0; skinnMeshNum < skinnMeshRenders.Length; skinnMeshNum++) {
					triangles.Clear();

					Mesh mesh = new Mesh();
					skinnMeshRenders[skinnMeshNum].BakeMesh(mesh);

					for(int subNum = 0 ; subNum<mesh.subMeshCount;subNum++)
						triangles.AddRange(mesh.GetTriangles(subNum).ToList());

					mesh.SetTriangles(triangles.ToArray(),0);
					mesh.subMeshCount = 1;

					combine[skinnMeshNum].mesh = mesh;
				}
				
				Mesh Commesh = new Mesh();
				Commesh.CombineMeshes(combine,true,false);
				CloneObj(Commesh, source, materialKind);
			}
		}
	}

	public void CloneMesh(GameObject source, int materialKind = 0, float deltaTime = 0.5f, int count = 4) {
		if (source) {
			TCloneMesh cm = new TCloneMesh();
			cm.Source = source;
			cm.MaterialKind = materialKind;
			cm.DeltaTime = deltaTime;
			cm.Count = count;

			cloneMeshs.Add(cm);
		}
	}
}
