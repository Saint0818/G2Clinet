using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct;

public class TAvatarLoader : MonoBehaviour {
	private static string[] avatarPartStr = {"B", "H", "M", "C", "P", "S", "A", "Z"};
	private static Material materialSource;
	private static Dictionary<string, MeshRenderer> meshRendererCache = new Dictionary<string, MeshRenderer>();
	private static Dictionary<string, SkinnedMeshRenderer> skinnedMeshRendererCache = new Dictionary<string, SkinnedMeshRenderer>();
	private static Dictionary<string, GameObject> bodyCache = new Dictionary<string, GameObject>();
	private static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();

	void Awake () {
		if (materialSource == null)
			materialSource = Resources.Load("Character/Materials/Material_0") as Material;
				
		if (meshRendererCache == null)
			meshRendererCache = new Dictionary<string, MeshRenderer>();

		if (skinnedMeshRendererCache == null)
			skinnedMeshRendererCache = new Dictionary<string, SkinnedMeshRenderer>();
		
		if (bodyCache == null)
			bodyCache = new Dictionary<string, GameObject>();

		if (textureCache == null)
			textureCache = new Dictionary<string, Texture2D>();
	}

	private static GameObject loadBody(string path)
	{
		if (bodyCache.ContainsKey(path))
			return bodyCache[path];
		else
		{
			GameObject obj = Resources.Load(path) as GameObject;
			if (obj)
			{
				bodyCache.Add(path, obj);
				return obj;
			}
			else
			{
				//download form server
				return null;
			}
		}
	}

	private static SkinnedMeshRenderer loadSkinnedMeshRenderer(string path)
	{
		if (skinnedMeshRendererCache.ContainsKey(path))
			return skinnedMeshRendererCache[path];
		else
		{
			GameObject obj = Resources.Load(path) as GameObject;
			if (obj)
			{
				SkinnedMeshRenderer smr = obj.GetComponentInChildren<SkinnedMeshRenderer> ();
				if (smr != null)
					skinnedMeshRendererCache.Add (path, smr);

				return smr;
			}
			else
			{
				//download form server
				return null;
			}
		}
	}

	private static MeshRenderer loadMeshRenderer(string path)
	{
		if (meshRendererCache.ContainsKey(path))
			return meshRendererCache[path];
		else
		{
			GameObject obj = Resources.Load(path) as GameObject;
			if (obj)
			{
				MeshRenderer smr = obj.GetComponentInChildren<MeshRenderer> ();
				if (smr != null)
					meshRendererCache.Add (path, smr);

				return smr;
			}
			else
			{
				//download form server
				return null;
			}
		}
	}

	private static Texture2D loadTexture(string path)
	{
		if (textureCache.ContainsKey(path))
		{
			return textureCache[path];
		}
		else
		{
			Texture2D obj = Resources.Load(path) as Texture2D;
			if (obj)
			{
				textureCache.Add(path, obj);
				return obj;
			}
			else 
			{
				//download form server
				return null;
			}
		}
	}

	private IEnumerator AsyncSetAvatar(int bodyType, TAvatar avatars) {
		int[] avatarIndex = new int[] { avatars.Body, avatars.Hair, avatars.MHandDress, avatars.Cloth, avatars.Pants, avatars.Shoes, avatars.AHeadDress, avatars.ZBackEquip };
		string path = string.Format("Prefab/Player/PlayerModel_{0}", bodyType); 
		string texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", bodyType, "B", "0", avatars.Body);
		GameObject resObj = null;
		Material matObj = materialSource;
		SkinnedMeshRenderer bodySMR = null;

		//Load body
		if (bodyCache.ContainsKey(path))
			resObj = bodyCache[path];
		else {
			ResourceRequest request = Resources.LoadAsync(path);
			yield return request;
			resObj = request.asset as GameObject;
			//bodyCache.Add (path, resObj);
		}

		if (resObj) {
			GameObject avatarPartGO = Instantiate (resObj) as GameObject;
			avatarPartGO.transform.parent = transform;

			Texture2D texture = null;
			if (textureCache.ContainsKey (texturePath))
				texture = textureCache [texturePath];
			else {
				ResourceRequest request = Resources.LoadAsync (texturePath);
				yield return request;
				texture = request.asset as Texture2D;
				//textureCache.Add (texturePath, texture);
			}

			bodySMR = avatarPartGO.GetComponentInChildren<SkinnedMeshRenderer> ();
			if (texture != null) {
				matObj = Instantiate (materialSource);
				matObj.SetTexture ("_MainTex", texture);
			}
		}

		for (int i = 1; i < 6; i++) {
			if (avatarIndex[i] > 0) {
				int avatarBody = avatarIndex[i] / 1000;
				int avatarBodyTexture = avatarIndex[i] % 1000;
				string avatarPart = avatarPartStr[i];
				path = string.Format("Character/PlayerModel_{0}/Model/{0}_{1}_{2}", bodyType, avatarPart, avatarBody);
				texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", bodyType, avatarPart, avatarBody, avatarBodyTexture);

				SkinnedMeshRenderer smr = null;

				if (skinnedMeshRendererCache.ContainsKey(path))
					smr = skinnedMeshRendererCache[path];
				else {
					ResourceRequest request = Resources.LoadAsync(path);
					yield return request;
					resObj = request.asset as GameObject;
					smr = resObj.GetComponentInChildren<SkinnedMeshRenderer> ();
					//skinnedMeshRendererCache.Add (path, smr);
				}

				if (smr != null) {
					SkinnedMeshRenderer avatarSMR = Instantiate(smr) as SkinnedMeshRenderer;

					Texture2D texture = null;
					if (textureCache.ContainsKey(texturePath))
						texture = textureCache[texturePath];
					else {
						ResourceRequest request = Resources.LoadAsync(texturePath);
						yield return request;
						texture = request.asset as Texture2D;
						//textureCache.Add (texturePath, texture);
					}

					if (texture != null) {
						matObj = Instantiate(materialSource);
						matObj.SetTexture("_MainTex", texture);
						avatarSMR.material = matObj;
						avatarSMR.transform.parent = transform;
					}
				}
			}
		}

		if (bodySMR != null && bodySMR.sharedMesh != null) {
			SkinnedMeshRenderer[] meshFilters = GetComponentsInChildren<SkinnedMeshRenderer> ();
			CombineInstance[] combine = new CombineInstance[meshFilters.Length];
			int count = 0;
			while (count < meshFilters.Length) {
				combine [count].mesh = meshFilters [count].sharedMesh;
				combine [count].transform = meshFilters [count].transform.localToWorldMatrix;
				meshFilters [count].gameObject.SetActive (false);
				count++;
			}

			bodySMR.sharedMesh.CombineMeshes (combine);
		}
	}

	public void LoadAvatar(int bodyType, TAvatar avatars) {
		StartCoroutine (AsyncSetAvatar (bodyType, avatars));
	}
}
