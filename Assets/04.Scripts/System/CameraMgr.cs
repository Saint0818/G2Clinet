using UnityEngine;
using System.Collections;

public class CameraMgr : KnightSingleton<CameraMgr>
{
	private float groupOffsetSpeed = 0.1f;
	private float zoomNormal = 30f;
	private float blankAera = 5.5f;
	private float lockedFocusAngle = 40f;
	private float focusOffsetSpeed = 0.3f;
	private float focusSmoothSpeed = 0.02f;
	private float[] focusStopPoint = new float[]{12f, -12f};

	private float cameraRotationSpeed = 6f;
	private float cameraOffsetSpeed = 0.1f;
	private Vector2 cameraWithBasketBallCourtRate;
	private Vector2 cameraMoveAera = new Vector2(23f, 34.5f); 
	private Vector3 cameraOffsetRate = Vector3.zero;
	private Vector2 basketballCourt = new Vector2(23f, 34.5f);
	private Vector3 restrictedAreaAngle = new Vector3(15, 1, 0);
	private Vector3 cameraOffset = Vector3.zero;
	private Vector3 startPos = new Vector3(-17.36f, 8f, 0.67f);
	private Vector3[] groupOffsetPoint = new Vector3[]{new Vector3(0, 0, -6.625f), new Vector3(0, 0, 6.625f)};
//	private Vector3[] offsetLimit = new Vector3[]{new Vector3(-12.6f, 0, 1.63f), new Vector3(-22.25f, 0, -1.63f)};
	private Vector3[] offsetLimit = new Vector3[]{new Vector3(-12f, 0, 1.63f), new Vector3(-26f, 0, -1.63f)};

	private GameObject cameraGroup;
	private GameObject cameraRotation;
	private Camera cameraFx;

	private GameObject focusTarget;
	private TeamKind curTeam = TeamKind.Self;
	
	public bool IsBallOnFloor = false;
	public bool IsLongPass = false;

	//TestTool
	private GameObject cameraOffsetAeraObj;
	private GameObject focusMoveAeraObj;
	private GameObject focusStopAeraObj;
	
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
			cameraRotation.gameObject.SetActive(value);
		}
		get
		{
			return cameraRotation.gameObject.activeInHierarchy;
		}
	}

	void Awake()
	{
		if (cameraGroup == null)
		{
			cameraGroup = Instantiate(Resources.Load("Prefab/Camera")) as GameObject;
			cameraGroup.name = "CameraGroup";

			cameraWithBasketBallCourtRate = new Vector2(cameraMoveAera.x / basketballCourt.x, cameraMoveAera.y / basketballCourt.y);
			InitCamera();
			InitTestTool();
		}
	}

	public void SetTeamCamera(TeamKind team)
	{
		curTeam = team;
		SetTestToolPosition();
	}

    public Camera GetUICamera()
    {
        return cameraFx;
    }

	private void InitCamera()
	{
		if (cameraGroup) {
			cameraRotation = cameraGroup.gameObject.transform.FindChild("Offset/Rotation").gameObject;
			cameraFx = cameraRotation.gameObject.transform.GetComponentInChildren<Camera>();
			
			cameraRotation.transform.position = startPos;
			smothHight.x = startPos.y;
			cameraOffset = cameraGroup.transform.position;		
		}


		cameraFx.farClipPlane = 130;
		cameraFx.fieldOfView = zoomNormal;
		cameraFx.cullingMask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("RealBall");

		cameraGroup.transform.localPosition = Vector3.zero;
		cameraRotation.transform.localPosition = startPos;

		focusTarget = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		focusTarget.collider.enabled = false;
		focusTarget.name = "focusPos";
		focusTarget.renderer.enabled = false;
		focusTarget.transform.position = SceneMgr.Get.RealBall.transform.position;
		cameraRotation.transform.LookAt(Vector3.zero) ;
	}

	private void InitTestTool()
	{
		focusTarget.renderer.enabled = true;
		cameraOffsetAeraObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cameraOffsetAeraObj.collider.enabled = false;
		cameraOffsetAeraObj.name = "ColorR";
		cameraOffsetAeraObj.transform.parent = cameraGroup.transform;
		cameraOffsetAeraObj.transform.position = new Vector3 (startPos.x, -0.4f, 0);
		cameraOffsetAeraObj.transform.localScale = new Vector3(offsetLimit[0].x - offsetLimit[1].x, 1, offsetLimit[0].z - offsetLimit[1].z);
		cameraOffsetAeraObj.renderer.material = Resources.Load ("Materials/CameraOffsetAera_M") as Material;

		focusMoveAeraObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
		focusMoveAeraObj.collider.enabled = false;
		focusMoveAeraObj.name = "ColorG";

		focusMoveAeraObj.transform.localScale = new Vector3 (cameraMoveAera.x, 1, cameraMoveAera.y);
		focusMoveAeraObj.renderer.material = Resources.Load ("Materials/FocusAera_M") as Material;
	
		focusStopAeraObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
		focusStopAeraObj.collider.enabled = false;
		focusStopAeraObj.name = "ColorO";

		focusStopAeraObj.transform.localScale = new Vector3(cameraMoveAera.x, 1, 0.1f);
		focusStopAeraObj.renderer.material = Resources.Load ("Materials/FocusStopAera_M") as Material;

		SetTestToolPosition();
	}

	private void SetTestToolPosition()
	{
		if (curTeam == TeamKind.Self) {
			focusMoveAeraObj.transform.position = new Vector3(0, -0.4f, blankAera);
			focusStopAeraObj.transform.position = new Vector3 (0, -0.4f, focusStopPoint [0]);	
		} else {
			focusMoveAeraObj.transform.position = new Vector3(0, -0.4f, -blankAera);
			focusStopAeraObj.transform.position = new Vector3 (0, -0.4f, focusStopPoint [1]);
		}
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
		//GroupOffset
		cameraGroup.transform.position = Vector3.Lerp(cameraGroup.transform.position, groupOffsetPoint[curTeam.GetHashCode()], groupOffsetSpeed);
		CameraOffset();
		CameraFocus ();
    }

	private Vector2 smothHight = Vector2.zero;

	private void CameraOffset()
	{
		cameraOffsetRate.x = (11.5f - focusTarget.transform.position.x) / cameraMoveAera.x;
		cameraOffsetRate.z = (22.5f - focusTarget.transform.position.z) / cameraMoveAera.y;
		if(cameraOffsetRate.x < 0)
			cameraOffsetRate.x = 0;
		else if(cameraOffsetRate.x > 1)
			cameraOffsetRate.x = 1;
		
		if(cameraOffsetRate.z < 0)
			cameraOffsetRate.z = 0;
		else if(cameraOffsetRate.z > 1)
			cameraOffsetRate.z = 1;

		cameraOffset.x = offsetLimit[0].x - (cameraOffsetRate.x * (offsetLimit[0].x - offsetLimit[1].x));

		if (GameController.Get.Shooter) {
			float h;

			if(SceneMgr.Get.RealBall.transform.position.y > 10f)
				h = 2;
			else 
				h = (SceneMgr.Get.RealBall.transform.position.y / 10f) * 2f;

			smothHight = Vector2.Lerp (smothHight, new Vector2 (0, startPos.y + h), 0.1f);
		}
		else
			smothHight = Vector2.Lerp(smothHight, new Vector2(0, startPos.y), 0.1f);

		cameraOffset.y = smothHight.y;
		cameraOffset.z = offsetLimit[0].z - (cameraOffsetRate.z * (offsetLimit[0].z - offsetLimit[1].z));
		cameraRotation.transform.localPosition = Vector3.Lerp(cameraRotation.transform.localPosition, cameraOffset, cameraOffsetSpeed);
	}

	private void CameraFocus()
	{
		focusObjectOffset (curTeam.GetHashCode());
		switch (curTeam) {
		case TeamKind.Self:
			if (focusTarget.transform.position.z < focusStopPoint[curTeam.GetHashCode()]) {
				Lookat(focusTarget);
				cameraRotation.transform.localEulerAngles = new Vector3(restrictedAreaAngle.x, cameraRotation.transform.localEulerAngles.y, restrictedAreaAngle.z);
			}
			else
			{
				float angle = Mathf.LerpAngle(cameraRotation.transform.localEulerAngles.y, lockedFocusAngle, focusSmoothSpeed);
				cameraRotation.transform.localEulerAngles =  new Vector3(restrictedAreaAngle.x, angle, restrictedAreaAngle.z);
			}
			break;
		case TeamKind.Npc:

			if (focusTarget.transform.position.z > focusStopPoint[curTeam.GetHashCode()]) {
				Lookat(focusTarget);
				cameraRotation.transform.localEulerAngles = new Vector3(restrictedAreaAngle.x, cameraRotation.transform.localEulerAngles.y, restrictedAreaAngle.z);
			}
			else
			{
				float angle = Mathf.LerpAngle(cameraRotation.transform.localEulerAngles.y, 180 - lockedFocusAngle, focusSmoothSpeed);
				cameraRotation.transform.localEulerAngles =  new Vector3(restrictedAreaAngle.x, angle, restrictedAreaAngle.z);
			}
			break;
		}
	}

	private void Lookat(GameObject obj)
	{
		Vector3 dir = obj.transform.position - cameraRotation.transform.position;
		Quaternion rot = Quaternion.LookRotation(dir);
		cameraRotation.transform.rotation = Quaternion.Lerp(cameraRotation.transform.rotation, rot, cameraRotationSpeed * Time.deltaTime);
	}

	private void focusObjectOffset(int team)
	{
		Vector3 pos;
		pos.x = SceneMgr.Get.RealBall.transform.position.x;
		pos.y = 0;

		if(team == 0)
			pos.z = SceneMgr.Get.RealBall.transform.position.z * cameraWithBasketBallCourtRate.y + blankAera;
		else
			pos.z = SceneMgr.Get.RealBall.transform.position.z * cameraWithBasketBallCourtRate.y - blankAera;

		focusTarget.transform.position = Vector3.Slerp(focusTarget.transform.position, pos, focusOffsetSpeed);
	}

    public override string ResName
    {
        get
        {
            return "CameraMgr";
        }
    }
}
