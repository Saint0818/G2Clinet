using System;
using UnityEngine;
using GameStruct;

public enum EUIPlayerMode
{
	UIPlayerInfo,
	UIAvatarFitted
}

public class UIPlayerMgr : KnightSingleton<UIPlayerMgr>
{
	private Camera camera3d;
	private GameObject avatar;

	void Awake()
	{
		Init3DCamera ();
		InitAvatar ();
	}

	private void Init3DCamera()
	{
		if (camera3d == null){
			GameObject go = new GameObject();
			go.name = "camera3d";
			camera3d = go.AddComponent<Camera>();
			go.transform.parent = gameObject.transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localEulerAngles = new Vector3 (0, -180, 0);
			LayerMgr.Get.SetLayer(go, ELayer.UI);

			camera3d.clearFlags = CameraClearFlags.Nothing;
			camera3d.cullingMask = (1 << LayerMask.NameToLayer ("UIPlayer")); 
			camera3d.orthographic = true;
			camera3d.orthographicSize = 2.41f;
			camera3d.nearClipPlane = 0.3f;
			camera3d.farClipPlane = 1000;
			camera3d.depth = 2;
		}
	}

	private void InitAvatar()
	{
		if(avatar == null){
			avatar = new GameObject ();
			avatar.name = "UIPlayer";
		}
	}

	public void ShowUIPlayer(EUIPlayerMode mode)
	{
		gameObject.SetActive (true);
		ModelManager.Get.SetAvatarByItem(ref avatar, GameData.Team.Player.Items, GameData.Team.Player.BodyType, EAnimatorType.AvatarControl, false);
		avatar.transform.parent = gameObject.transform;
		avatar.transform.localScale = Vector3.one;

		switch (mode) {
			case EUIPlayerMode.UIPlayerInfo:
				avatar.transform.localPosition = new Vector3 (2.62f, -2.16f, -2);
				break;
			case EUIPlayerMode.UIAvatarFitted:
				avatar.transform.localPosition = new Vector3 (-3, -1.7f, -2);
				break;
		}
		LayerMgr.Get.SetLayerRecursively(avatar, "UIPlayer");
	}

	public void ChangeAvatar(TAvatar equipAvatar)
	{
		ModelManager.Get.SetAvatar(ref avatar, equipAvatar, GameData.Team.Player.BodyType, EAnimatorType.AvatarControl, false);
		avatar.transform.parent = gameObject.transform;
		avatar.transform.localScale = Vector3.one;
		avatar.transform.localPosition = new Vector3 (-3, -1.7f, -1);
		LayerMgr.Get.SetLayerRecursively(avatar, "UIPlayer");
	}

	public bool Enable
	{
		set{gameObject.SetActive (value);}
	}
}

