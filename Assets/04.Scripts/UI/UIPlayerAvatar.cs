using System;
using UnityEngine;
using GameStruct;

public enum EUIPlayerMode
{
	UIPlayerInfo,
	UIAvatarFitted,
    UIShop
}

public class UIPlayerAvatar : KnightSingleton<UIPlayerAvatar>
{
    private EUIPlayerMode uiMode;
	private Camera camera3d;
	private GameObject playerModel;
    private TLoadParameter loadParam;
   
	void Awake()
	{
        loadParam = new TLoadParameter(ELayer.UIPlayer, "PlayerModel", false, true);
		Init3DCamera ();
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
			camera3d.nearClipPlane = 0.1f;
			camera3d.farClipPlane = 1000;
			camera3d.depth = 2;
			if(camera3d.gameObject.GetComponent<UICamera>() == null)
				camera3d.gameObject.AddComponent<UICamera>();
		}
	}

    private void setManLocation(EUIPlayerMode mode) {
        playerModel.transform.parent = gameObject.transform;
        playerModel.transform.localScale = Vector3.one;
        switch (mode) {
            case EUIPlayerMode.UIPlayerInfo:
                playerModel.transform.localPosition = new Vector3 (3f, -1.7f, -3);
                break;
            case EUIPlayerMode.UIAvatarFitted:
                playerModel.transform.localPosition = new Vector3 (3, -1.7f, -3);
                break;
            case EUIPlayerMode.UIShop:
                playerModel.transform.localPosition = new Vector3 (2.92f, -1.8f, -3);
                break;
        }
    }

    public void ShowUIPlayer(EUIPlayerMode mode, int bodyType, TAvatar avatars)
	{
		gameObject.SetActive (true);
        TAvatarLoader.Load(bodyType, avatars, ref playerModel, gameObject, loadParam);
       
        uiMode = mode;
        setManLocation(mode);
	}

	public void ChangeAvatar(int bodyType, TAvatar equipAvatar)
	{
        TAvatarLoader.Load(bodyType, equipAvatar, ref playerModel, gameObject, loadParam);
        setManLocation(uiMode);
	}

	/// <summary>
	/// 介面上人物的開關
	/// 使用介面 : UIPlayerInfo、UIAvatarFitted、UIShop
	/// </summary>
	/// <value><c>true</c> if enable; otherwise, <c>false</c>.</value>
	public bool Enable
	{
		set{gameObject.SetActive (value);}
	}
}

