using UnityEngine;
using System.Collections;

public class CameraMgr : KnightSingleton<CameraMgr>
{
	private float offsetSpeed = 0.1f;
	private float focusSpeed = 4f;
	private Vector3 startPos = new Vector3(-17.5f, 7, 8);
	private Vector3 focusLimit = new Vector3(10, 0, 20);
	private Vector3 focus;
	private Vector3[] offsetPos = new Vector3[]{new Vector3(0, 0, -2.5f), new Vector3(0, 0, 2.5f)};
	private TeamKind curTeam = TeamKind.Self;
	private GameObject uiCamOffset;

	public Camera uiCam;
	public GameObject[] OffsetPos = new GameObject[2];
	private GameObject focusPos;

	public bool UICamVisible
	{
		set
		{
			uiCam.gameObject.SetActive(value);
		}
		get
		{
			return uiCam.gameObject.activeInHierarchy;
		}
	}

	void Awake()
	{
		if (uiCamOffset == null)
		{
			focusPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			focusPos.collider.enabled = false;
			focusPos.name = "focusPos";

			focus = new Vector3(focusLimit.x / 26, 0, focusLimit.z / 36);
			Debug.Log("focus" + focus);
			focusPos.transform.position = new Vector3(SceneMgr.Inst.RealBall.transform.position.x * focus.x, SceneMgr.Inst.RealBall.transform.position.y, SceneMgr.Inst.RealBall.transform.position.z * focus.z);

			uiCamOffset = Instantiate(Resources.Load("Prefab/Camera")) as GameObject;
			uiCam = uiCamOffset.GetComponentInChildren<Camera>();
			uiCam.transform.position = startPos;

			for(int i = 0; i < OffsetPos.Length; i++)
			{
				OffsetPos[i] = new GameObject();
				OffsetPos[i].transform.position = offsetPos[i];
				OffsetPos[i].name = "CameraOffsetPos" + i;
			}

			InitCamera();
		}
	}

	public void SetTeamCamera(TeamKind team)
	{
		curTeam = team;
	}

    public Camera GetUICamera()
    {
        return uiCam;
    }

	private void InitCamera()
	{
		uiCam.farClipPlane = 130;
		uiCam.fieldOfView = 30;
		uiCam.cullingMask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("RealBall");

		uiCamOffset.transform.localPosition = Vector3.zero;
		uiCam.transform.localPosition = new Vector3 (-14.5f, 7, 1.8f);
	}

    public void SetFocus(FocusSensor.FocusSensorMode sensorMode, GameObject sensorObj, GameObject ball, bool isReal)
    { 
	    switch (sensorMode)
	    {
	        case FocusSensor.FocusSensorMode.Center:
	        case FocusSensor.FocusSensorMode.LeftTop:
	        case FocusSensor.FocusSensorMode.RightTop:
	        case FocusSensor.FocusSensorMode.Left:
	        case FocusSensor.FocusSensorMode.LeftUp:
	        case FocusSensor.FocusSensorMode.LeftDown:
	        case FocusSensor.FocusSensorMode.Right:
	        case FocusSensor.FocusSensorMode.RightUp:
	        case FocusSensor.FocusSensorMode.RightDown:
	        case FocusSensor.FocusSensorMode.CenterDown:
	        case FocusSensor.FocusSensorMode.CenterUp:
	            break;
	    }
    }

	void FixedUpdate()
    {
        HorizontalCameraHandle();
    }

    private void HorizontalCameraHandle()
    {
		//OffsetComputing
		uiCamOffset.transform.localPosition = Vector3.Slerp(uiCamOffset.transform.localPosition, OffsetPos[curTeam.GetHashCode()].transform.localPosition, offsetSpeed);

		//ZoomComputing

//		Debug.LogError ("Ball dis : " + Vector3.Distance (SceneMgr.Inst.RealBall.transform.position, uiCamOffset.transform.position));
		//FocusComputing
		if (UIGame.Get.Game.ShootController)
			focusPos.transform.position = Vector3.Slerp (focusPos.transform.position, SceneMgr.Inst.ShootPoint [UIGame.Get.Game.ShootController.Team.GetHashCode ()].transform.position, 0.1f);
		else
			focusPos.transform.position = new Vector3 (SceneMgr.Inst.RealBall.transform.position.x * 0.5f, 1.5f, SceneMgr.Inst.RealBall.transform.position.z * 0.55f);

		Quaternion rotation = Quaternion.LookRotation(focusPos.transform.position - uiCam.transform.position, Vector3.up);
		uiCam.transform.rotation = Quaternion.Slerp(uiCam.transform.rotation, rotation, Time.deltaTime * focusSpeed);
    }

    public override string ResName
    {
        get
        {
            return "CameraMgr";
        }
    }
}
