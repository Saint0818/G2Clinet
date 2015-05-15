using UnityEngine;
using System.Collections;

public class CameraMgr : KnightSingleton<CameraMgr>
{
	//Game const
	private float groupOffsetSpeed = 0.1f;
	private float zoomNormal = 25;
	private float blankAera = 3.2f;
	private float lockedFocusAngle = 30f;
	private float focusOffsetSpeed = 0.8f;
	private float focusSmoothSpeed = 0.02f;
	private float[] focusStopPoint = new float[]{21f, -21f};

	private float cameraRotationSpeed = 10f;
	private float cameraOffsetSpeed = 0.1f;
	private Vector2 cameraWithBasketBallCourtRate;
	private Vector2 cameraMoveAera = new Vector2(23f, 27.5f); 
	private Vector3 cameraOffsetRate = Vector3.zero;
	private Vector2 basketballCourt = new Vector2(23f, 34.5f);
	private Vector3 restrictedAreaAngle = new Vector3(14f, 1, 0);
	private Vector3 cameraOffsetPos = Vector3.zero;
	private Vector3 startPos = new Vector3(-17.36f, 8f, 0.67f);
	private Vector3[] groupOffsetPoint = new Vector3[]{new Vector3(0, 0, -6.625f), new Vector3(0, 0, 7.625f)};
	private Vector3[] offsetLimit = new Vector3[]{new Vector3(-13f, 0, 1.63f), new Vector3(-30f, 0, -1.63f)};

	private GameObject cameraGroupObj;
	private GameObject cameraRotationObj;
	private GameObject cameraOffsetObj;

	private Camera cameraFx;
	private Camera cameraPlayer;

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
			cameraRotationObj.gameObject.SetActive(value);
		}
		get
		{
			return cameraRotationObj.gameObject.activeInHierarchy;
		}
	}

	void Awake()
	{
		if (cameraGroupObj == null)
		{
			cameraGroupObj = Instantiate(Resources.Load("Prefab/Stadium/Camera")) as GameObject;
			cameraGroupObj.name = "CameraGroup";

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

    public Camera CourtCamera
    {
		get {return cameraFx;}
    }

	private void InitCamera()
	{
		if (cameraGroupObj) {
			cameraOffsetObj = cameraGroupObj.gameObject.transform.FindChild("Offset").gameObject;
			cameraRotationObj = cameraGroupObj.gameObject.transform.FindChild("Offset/Rotation").gameObject;
			cameraFx = cameraRotationObj.gameObject.transform.GetComponentInChildren<Camera>();
			
			cameraRotationObj.transform.position = startPos;
			smothHight.x = startPos.y;
			cameraOffsetPos = cameraGroupObj.transform.position;		
		}

		cameraFx.farClipPlane = 500;
		cameraFx.fieldOfView = zoomNormal;
		cameraFx.cullingMask = 1 << LayerMask.NameToLayer("Player") | 
							   1 << LayerMask.NameToLayer("Default") | 
							   1 << LayerMask.NameToLayer("RealBall") | 
							   1 << LayerMask.NameToLayer("Scene");

		cameraGroupObj.transform.localPosition = Vector3.zero;
		cameraRotationObj.transform.localPosition = startPos;
		cameraRotationObj.transform.LookAt(Vector3.zero);

		focusTarget = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		focusTarget.GetComponent<Collider>().enabled = false;
		focusTarget.name = "focusPos";
		focusTarget.transform.position = SceneMgr.Get.RealBall.transform.position;
	}

	private void InitTestTool()
	{
		if (GameStart.Get.TestCameraMode == CameraTest.RGB) {
			cameraOffsetAeraObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cameraOffsetAeraObj.GetComponent<Collider>().enabled = false;
			cameraOffsetAeraObj.name = "ColorR";
			cameraOffsetAeraObj.transform.parent = cameraGroupObj.transform;
			cameraOffsetAeraObj.transform.position = new Vector3 (startPos.x, -0.4f, 0);
			cameraOffsetAeraObj.transform.localScale = new Vector3(offsetLimit[0].x - offsetLimit[1].x, 1, offsetLimit[0].z - offsetLimit[1].z);
			cameraOffsetAeraObj.GetComponent<Renderer>().material = Resources.Load ("Materials/CameraOffsetAera_M") as Material;

			focusMoveAeraObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			focusMoveAeraObj.GetComponent<Collider>().enabled = false;
			focusMoveAeraObj.name = "ColorG";

			focusMoveAeraObj.transform.localScale = new Vector3 (cameraMoveAera.x, 1, cameraMoveAera.y);
			focusMoveAeraObj.GetComponent<Renderer>().material = Resources.Load ("Materials/FocusAera_M") as Material;
		
			focusStopAeraObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			focusStopAeraObj.GetComponent<Collider>().enabled = false;
			focusStopAeraObj.name = "ColorO";

			focusStopAeraObj.transform.localScale = new Vector3(cameraMoveAera.x, 1, 0.1f);
			focusStopAeraObj.GetComponent<Renderer>().material = Resources.Load ("Materials/FocusStopAera_M") as Material;

			SetTestToolPosition();

			cameraOffsetAeraObj.GetComponent<Renderer> ().enabled = GameStart.Get.TestCameraMode == CameraTest.RGB;
			focusMoveAeraObj.GetComponent<Renderer> ().enabled = GameStart.Get.TestCameraMode == CameraTest.RGB;
			focusStopAeraObj.GetComponent<Renderer> ().enabled =  GameStart.Get.TestCameraMode == CameraTest.RGB;
			focusTarget.GetComponent<Renderer>().enabled = true;
		} else
			focusTarget.GetComponent<Renderer>().enabled = false;
	}

	private void SetTestToolPosition()
	{
		if (GameStart.Get.TestCameraMode == CameraTest.RGB) {
			if (curTeam == TeamKind.Self) {
				focusMoveAeraObj.transform.position = new Vector3(0, -0.4f, blankAera);
				focusStopAeraObj.transform.position = new Vector3 (0, -0.4f, focusStopPoint [0]);	
			} else {
				focusMoveAeraObj.transform.position = new Vector3(0, -0.4f, -blankAera);
				focusStopAeraObj.transform.position = new Vector3 (0, -0.4f, focusStopPoint [1]);
			}
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
		cameraGroupObj.transform.position = Vector3.Lerp(cameraGroupObj.transform.position, groupOffsetPoint[curTeam.GetHashCode()], groupOffsetSpeed);
		CameraOffset();
		CameraFocus ();
		if(cameraPlayer && GameController.Get.Joysticker != null) {
			cameraPlayer.gameObject.transform.LookAt(GameController.Get.Joysticker.gameObject.transform);
		}
    }

	private Vector2 smothHight = Vector2.zero;

	private void CameraOffset()
	{
		float boardZ = -1 * (cameraMoveAera.y / 2) + blankAera;
		float computZ = 0;
//
//		if (focusTarget.transform.position.z < boardZ)
//			computZ = 0;
//		else

		cameraOffsetRate.x = (11.5f - focusTarget.transform.position.x) / cameraMoveAera.x;
//				Mathf.Sqrt (Mathf.Pow (11.5f - focusTarget.transform.position.x, 2) + Mathf.Pow (22.5f - focusTarget.transform.position.z, 2));
//		Debug.Log ("cameraOffsetRate.x : " + (22.5f - focusTarget.transform.position.z));

			
		cameraOffsetRate.z = (22.5f - focusTarget.transform.position.z) / cameraMoveAera.y;
		if(cameraOffsetRate.x < 0)
			cameraOffsetRate.x = 0;
		else if(cameraOffsetRate.x > 1)
			cameraOffsetRate.x = 1;
		
		if(cameraOffsetRate.z < 0)
			cameraOffsetRate.z = 0;
		else if(cameraOffsetRate.z > 1)
			cameraOffsetRate.z = 1;

		cameraOffsetPos.x = offsetLimit[0].x - (cameraOffsetRate.x * (offsetLimit[0].x - offsetLimit[1].x));

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

		cameraOffsetPos.y = smothHight.y;
		cameraOffsetPos.z = offsetLimit[0].z - (cameraOffsetRate.z * (offsetLimit[0].z - offsetLimit[1].z));
		cameraRotationObj.transform.localPosition = Vector3.Lerp(cameraRotationObj.transform.localPosition, cameraOffsetPos, cameraOffsetSpeed);
	}

	private void CameraFocus()
	{
		focusObjectOffset (curTeam.GetHashCode());
		switch (curTeam) {
		case TeamKind.Self:
			if (focusTarget.transform.position.z < focusStopPoint[curTeam.GetHashCode()]) {
				Lookat(focusTarget, Vector3.zero);
				cameraRotationObj.transform.localEulerAngles = new Vector3(restrictedAreaAngle.x, cameraRotationObj.transform.localEulerAngles.y, restrictedAreaAngle.z);
			}
			else
			{
				float angle = Mathf.LerpAngle(cameraRotationObj.transform.localEulerAngles.y, lockedFocusAngle, focusSmoothSpeed);
				cameraRotationObj.transform.localEulerAngles =  new Vector3(restrictedAreaAngle.x, angle, restrictedAreaAngle.z);
			}
			break;
		case TeamKind.Npc:

			if (focusTarget.transform.position.z > focusStopPoint[curTeam.GetHashCode()]) {
				Lookat(focusTarget, Vector3.zero);
				cameraRotationObj.transform.localEulerAngles = new Vector3(restrictedAreaAngle.x, cameraRotationObj.transform.localEulerAngles.y, restrictedAreaAngle.z);
			}
			else
			{
				float angle = Mathf.LerpAngle(cameraRotationObj.transform.localEulerAngles.y, 180 - lockedFocusAngle, focusSmoothSpeed);
				cameraRotationObj.transform.localEulerAngles =  new Vector3(restrictedAreaAngle.x, angle, restrictedAreaAngle.z);
			}
			break;
		}
	}
	
	public void setSplitScreen(){
		RenderTexture tex = Resources.Load("UI/PlayerCameraTexture", typeof(RenderTexture)) as RenderTexture;
		cameraPlayer = Instantiate(cameraFx);
		cameraPlayer.name = "PlayerCamera";
		cameraPlayer.transform.position = cameraFx.transform.position;
		cameraPlayer.targetTexture = tex;
	}

	private void Lookat(GameObject obj, Vector3 pos)
	{
		Vector3 dir = obj.transform.position - cameraRotationObj.transform.position;
		Quaternion rot = Quaternion.LookRotation(dir);
		cameraRotationObj.transform.rotation = Quaternion.Lerp(cameraRotationObj.transform.rotation, rot, cameraRotationSpeed * Time.deltaTime);
	}

	private void focusObjectOffset(int team)
	{
		Vector3 pos;
		pos.x = SceneMgr.Get.RealBall.transform.position.x;
		pos.y = 0;

		if (GameController.Get.IsStart && GameController.Get.BallOwner) {
			pos.x = GameController.Get.BallOwner.gameObject.transform.position.x;
			if (GameController.Get.BallOwner.Team == 0)
				pos.z = GameController.Get.BallOwner.gameObject.transform.position.z * cameraWithBasketBallCourtRate.y + blankAera;
			else
				pos.z = GameController.Get.BallOwner.gameObject.transform.position.z * cameraWithBasketBallCourtRate.y - blankAera;
		} else {
			if(team == 0)
				pos.z = SceneMgr.Get.RealBall.transform.position.z * cameraWithBasketBallCourtRate.y + blankAera;
			else
				pos.z = SceneMgr.Get.RealBall.transform.position.z * cameraWithBasketBallCourtRate.y - blankAera;
		}

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
