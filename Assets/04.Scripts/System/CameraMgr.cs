using UnityEngine;
using System.Collections;

public class CameraMgr : KnightSingleton<CameraMgr>
{
	private float offsetSpeed = 0.1f;
	private float focusSpeed = 3f;
	private float zoomIn = 28;
	private float zoomNormal = 35f;
	private float zoomOut = 42;
	private float zoomTo = 35f;

	private Vector2 zoomVertor2 = new Vector2 (35f, 0);
	private Vector3 offsetVertor3 = Vector3.zero;
	private Vector3 focusVertor3 = Vector3.zero;
	private Vector3 startPos = new Vector3(-18, 7.5f, 0);
	private Vector3 focusLimit = new Vector3(12, 0, 20);
	private Vector3 focusRate;

	private Vector3[] offsetPos = new Vector3[]{new Vector3(0, 0, -2.5f), new Vector3(0, 0, 2.5f)};
	private TeamKind curTeam = TeamKind.Self;
	private GameObject uiCamOffset;

	public bool IsBallOnFloor = false;
	public bool IsLongPass = false;
	public Camera uiCam;
	public GameObject[] OffsetPos = new GameObject[2];
	private GameObject focusObject;

	private enum ZoomType
	{
		In,
		Out,
		Normal
	}

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
			focusObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			focusObject.collider.enabled = false;
			focusObject.name = "focusPos";
			focusObject.renderer.enabled = false;

			focusRate = new Vector3(focusLimit.x / 26, 0, focusLimit.z / 36);
			focusObject.transform.position = new Vector3(SceneMgr.Get.RealBall.transform.position.x * focusRate.x, SceneMgr.Get.RealBall.transform.position.y, SceneMgr.Get.RealBall.transform.position.z * focusRate.z);

			uiCamOffset = Instantiate(Resources.Load("Prefab/Camera")) as GameObject;
			uiCam = uiCamOffset.GetComponentInChildren<Camera>();
			uiCam.transform.position = startPos;
			offsetVertor3 = uiCamOffset.transform.position;

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

	public void AddScore(int team)
	{
		SetZoom(ZoomType.In);
	}

    public Camera GetUICamera()
    {
        return uiCam;
    }

	private void InitCamera()
	{
		uiCam.farClipPlane = 130;
		uiCam.fieldOfView = zoomNormal;
		uiCam.cullingMask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("RealBall");

		uiCamOffset.transform.localPosition = Vector3.zero;
		uiCam.transform.localPosition = startPos;
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
		offsetVertor3 = Vector3.Lerp(uiCamOffset.transform.position, OffsetPos[curTeam.GetHashCode()].transform.position, offsetSpeed);
		uiCamOffset.transform.position = new Vector3 (0, 0, offsetVertor3.z);

		//FocusComputing
		if (GameController.Get.ShootController) {
			focusVertor3 = Vector3.Slerp (focusObject.transform.position, SceneMgr.Get.ShootPoint [GameController.Get.ShootController.Team.GetHashCode ()].transform.position, 0.1f);
			focusObject.transform.position = new Vector3(focusVertor3.x, 2, focusVertor3.z);
			SetZoom(ZoomType.In);
		}
		else
		{
			focusObject.transform.position = Vector3.Slerp(focusObject.transform.position, new Vector3 (SceneMgr.Get.RealBall.transform.position.x * 0.5f, 1.5f, SceneMgr.Get.RealBall.transform.position.z * 0.55f), 0.1f);

			if( Vector3.Distance(SceneMgr.Get.RealBall.gameObject.transform.position, CameraMgr.Get.uiCam.gameObject.transform.position) > 16)
				SetZoom(ZoomType.Normal);
			else
				SetZoom(ZoomType.Out);
		}

		//ZoomComputing
		zoomVertor2 = Vector2.Lerp(new Vector2 (uiCam.fieldOfView, 0), new Vector2 (zoomTo, 0), 0.1f);
		uiCam.fieldOfView = zoomVertor2.x;

		Quaternion rotation = Quaternion.LookRotation(focusObject.transform.position - uiCam.transform.position, Vector3.up);

		if (IsLongPass) {
			uiCam.transform.rotation = Quaternion.Slerp (uiCam.transform.rotation, rotation, Time.deltaTime * (focusSpeed + 3f));
		}
		else
			uiCam.transform.rotation = Quaternion.Slerp(uiCam.transform.rotation, rotation, Time.deltaTime * focusSpeed);
    }

	private void SetZoom(ZoomType type)
	{
		switch (type) {
			case ZoomType.In:
				if(zoomTo != zoomIn)
					zoomTo = zoomIn;
				break;
			case ZoomType.Normal:
				if(zoomTo != zoomNormal)
					zoomTo = zoomNormal;
				break;
			case ZoomType.Out:
				if(zoomTo != zoomOut)
					zoomTo = zoomOut;
				break;
		}
	}

    public override string ResName
    {
        get
        {
            return "CameraMgr";
        }
    }
}
