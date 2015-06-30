using UnityEngine;
using System.Collections;

public enum EZoomType
{
	Normal,
	In,
	Out
}

public class CameraMgr : KnightSingleton<CameraMgr>
{
	//Game const
	private Shake mShake;
	private float groupOffsetSpeed = 0.1f;
	private float zoomNormal = 25;
	private float zoomRange = 20;
	private float zoomTime = 1;
	private float blankAera = 3.2f;
	private float lockedFocusAngle = 30f;
	private float lockedTeeFocusAngle = 50f;
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
	private Vector3 jumpBallPos = new Vector3(-25f, 8, 0);
	private Vector3 jumpBallRoate= new Vector3(12.5f, 90, 0);

	private GameObject cameraGroupObj;
	private GameObject cameraRotationObj;
	private GameObject cameraOffsetObj;

	private Camera cameraFx;
//	private Camera cameraPlayer;

	private GameObject focusTarget;
	private ETeamKind curTeam = ETeamKind.Self;
	
	public bool IsBallOnFloor = false;
	public bool IsLongPass = false;
	private bool isStartRoom = false;
	private GameObject skiller;

	//TestTool
	private GameObject cameraOffsetAeraObj;
	private GameObject focusMoveAeraObj;
	private GameObject focusStopAeraObj;

	public EZoomType CrtZoom = EZoomType.Normal;

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

	public void PlayShake() {
		mShake.Play();
		AudioMgr.Get.PlaySound (SoundType.Dunk);
	}

	public void SetCourtCamera(SceneName scene)
	{
		if(cameraFx && cameraFx.name != scene.ToString()){
			Destroy(cameraFx.gameObject);
			GameObject obj = Instantiate(Resources.Load(string.Format("Prefab/Camera/Camera_{0}", scene.ToString()))) as GameObject;
			cameraFx = obj.GetComponent<Camera>();
			cameraFx.gameObject.transform.parent = cameraRotationObj.transform;
			cameraFx.gameObject.transform.localPosition = Vector3.zero;
			cameraFx.gameObject.transform.localEulerAngles = Vector3.zero;
			cameraFx.gameObject.name = scene.ToString();
		}
	}

	public void SetSelectRoleCamera()
	{
		if(cameraFx){
			Destroy(cameraFx.gameObject);
			GameObject obj = Instantiate(Resources.Load("Prefab/Camera/Camera_SelectRole")) as GameObject;
			cameraFx = obj.GetComponent<Camera>();
			cameraFx.gameObject.transform.localPosition = Vector3.zero;
			cameraFx.gameObject.transform.localEulerAngles = Vector3.zero;
			cameraFx.gameObject.name = "Camera_SelectRole";
		}
	}

	public bool IsTee = false;

	public void SetTeamCamera(ETeamKind team, bool isTee = false)
	{
		IsTee = isTee;
		curTeam = team;
		SetTestToolPosition();
	}

    public Camera CourtCamera
    {
		get {return cameraFx;}
    }

	public void InitCamera(ETeamKind team)
	{
		SetTeamCamera (team);
		InitCamera ();
	}

	private void InitCamera()
	{
		if (cameraGroupObj) {
			cameraOffsetObj = cameraGroupObj.gameObject.transform.FindChild("Offset").gameObject;
			cameraRotationObj = cameraGroupObj.gameObject.transform.FindChild("Offset/Rotation").gameObject;
			cameraFx = cameraRotationObj.gameObject.transform.GetComponentInChildren<Camera>();

			if(!cameraOffsetObj.gameObject.GetComponent<Shake>())
				mShake = cameraOffsetObj.gameObject.AddComponent<Shake>();
			
			cameraRotationObj.transform.position = startPos;
			smothHight.y = startPos.y;
			cameraOffsetPos = cameraGroupObj.transform.position;		
		}

		cameraGroupObj.transform.localPosition = Vector3.zero;
		cameraRotationObj.transform.localPosition = jumpBallPos;
		cameraRotationObj.transform.localEulerAngles = jumpBallRoate;

		if (focusTarget == null) {
			focusTarget = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			focusTarget.GetComponent<Collider> ().enabled = false;
			focusTarget.name = "focusPos";
		}

		if (CourtMgr.Get.RealBall)
			focusTarget.transform.position = CourtMgr.Get.RealBall.transform.position;
	}

	private void InitTestTool()
	{
		if (GameStart.Get.TestCameraMode == ECameraTest.RGB) {
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

			cameraOffsetAeraObj.GetComponent<Renderer> ().enabled = GameStart.Get.TestCameraMode == ECameraTest.RGB;
			focusMoveAeraObj.GetComponent<Renderer> ().enabled = GameStart.Get.TestCameraMode == ECameraTest.RGB;
			focusStopAeraObj.GetComponent<Renderer> ().enabled =  GameStart.Get.TestCameraMode == ECameraTest.RGB;
			focusTarget.GetComponent<Renderer>().enabled = true;
		} else
			focusTarget.GetComponent<Renderer>().enabled = false;
	}

	private void SetTestToolPosition()
	{
		if (GameStart.Get.TestCameraMode == ECameraTest.RGB) {
			if (curTeam == ETeamKind.Self) {
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
		if (SceneMgr.Get.CurrentScene != SceneName.SelectRole && curTeam != ETeamKind.JumpBall) {
			ZoomCalculation();
			HorizontalCameraHandle();
		}
    }

	private void ZoomCalculation()
	{
		if (isStartRoom) {
			if (CrtZoom == EZoomType.In){
				cameraFx.fieldOfView = Mathf.Lerp(cameraFx.fieldOfView , zoomRange, zoomTime);

				if (Mathf.Approximately(cameraFx.fieldOfView, zoomRange))
					isStartRoom = false;
			}
			else if (CrtZoom == EZoomType.Out){
				cameraFx.fieldOfView = Mathf.Lerp(cameraFx.fieldOfView , zoomNormal, zoomTime);

				if (Mathf.Approximately(cameraFx.fieldOfView, zoomNormal))
					isStartRoom = false;
			}
		}
	}

    private void HorizontalCameraHandle()
    {
		//GroupOffset
		if(curTeam != ETeamKind.Skiller && curTeam != ETeamKind.JumpBall)
			cameraGroupObj.transform.position = Vector3.Lerp(cameraGroupObj.transform.position, groupOffsetPoint[curTeam.GetHashCode()], groupOffsetSpeed);

		CameraOffset();
		CameraFocus ();
//		if(cameraPlayer && GameController.Get.Joysticker != null) {
//			cameraPlayer.gameObject.transform.LookAt(GameController.Get.Joysticker.gameObject.transform);
//		}
    }

	private Vector2 smothHight = Vector2.zero;

	private void CameraOffset()
	{
		if (curTeam == ETeamKind.Skiller)
			return;
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

		cameraOffsetPos.x = offsetLimit[0].x - (cameraOffsetRate.x * (offsetLimit[0].x - offsetLimit[1].x));

		if (GameController.Get.Shooter) {
			float h;

			if(CourtMgr.Get.RealBall.transform.position.y > 10f)
				h = 2;
			else 
				h = (CourtMgr.Get.RealBall.transform.position.y / 10f) * 2f;

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
		focusObjectOffset ();

		switch (curTeam) {

		case ETeamKind.JumpBall:
			cameraRotationObj.transform.localPosition = jumpBallPos;
			cameraRotationObj.transform.localEulerAngles = jumpBallRoate;
//				new Vector3(restrictedAreaAngle.x, cameraRotationObj.transform.localEulerAngles.y, restrictedAreaAngle.z);
			break;
		case ETeamKind.Self:
			if(!IsTee){
				if (focusTarget.transform.position.z < focusStopPoint[curTeam.GetHashCode()]) {
					Lookat(focusTarget, Vector3.zero);
					cameraRotationObj.transform.localEulerAngles = new Vector3(restrictedAreaAngle.x, cameraRotationObj.transform.localEulerAngles.y, restrictedAreaAngle.z);
				}
				else
				{
					float angle = Mathf.LerpAngle(cameraRotationObj.transform.localEulerAngles.y, lockedFocusAngle, focusSmoothSpeed);
					cameraRotationObj.transform.localEulerAngles =  new Vector3(restrictedAreaAngle.x, angle, restrictedAreaAngle.z);
				}
			}
			else{
				float angle = Mathf.LerpAngle(cameraRotationObj.transform.localEulerAngles.y, lockedTeeFocusAngle, focusSmoothSpeed);
				cameraRotationObj.transform.localEulerAngles =  new Vector3(restrictedAreaAngle.x, angle, restrictedAreaAngle.z);
			}

			break;
		case ETeamKind.Npc:
			if(!IsTee){
				if (focusTarget.transform.position.z > focusStopPoint[curTeam.GetHashCode()]) {
					Lookat(focusTarget, Vector3.zero);
					cameraRotationObj.transform.localEulerAngles = new Vector3(restrictedAreaAngle.x, cameraRotationObj.transform.localEulerAngles.y, restrictedAreaAngle.z);
				}
				else
				{
					float angle = Mathf.LerpAngle(cameraRotationObj.transform.localEulerAngles.y, 180 - lockedFocusAngle, focusSmoothSpeed);
					cameraRotationObj.transform.localEulerAngles =  new Vector3(restrictedAreaAngle.x, angle, restrictedAreaAngle.z);
				}
			}
			else
			{
				float angle = Mathf.LerpAngle(cameraRotationObj.transform.localEulerAngles.y, 180 - lockedTeeFocusAngle, focusSmoothSpeed);
				cameraRotationObj.transform.localEulerAngles =  new Vector3(restrictedAreaAngle.x, angle, restrictedAreaAngle.z);
			}
			break;

		case ETeamKind.Skiller:
			focusTarget.transform.position = new Vector3(skiller.transform.position.x , skiller.transform.position.y + 2, skiller.transform.position.z);
			Lookat(focusTarget, Vector3.zero);
			break;
		}
	}

	private void Lookat(GameObject obj, Vector3 pos)
	{
		Vector3 dir = obj.transform.position - cameraRotationObj.transform.position;
		Quaternion rot = Quaternion.LookRotation(dir);
		cameraRotationObj.transform.rotation = Quaternion.Lerp(cameraRotationObj.transform.rotation, rot, cameraRotationSpeed * Time.deltaTime);
	}

	private void focusObjectOffset()
	{
		Vector3 pos = Vector3.zero;
		pos.x = CourtMgr.Get.RealBall.transform.position.x;
		pos.y = 0;

		switch (curTeam) {
			case ETeamKind.JumpBall:
				pos = Vector3.zero;
				break;
			case ETeamKind.Self:
					if(GameController.Get.BallOwner){
						pos.x = GameController.Get.BallOwner.gameObject.transform.position.x;
						pos.z = GameController.Get.BallOwner.gameObject.transform.position.z * cameraWithBasketBallCourtRate.y + blankAera;
					}
					else
					{
						pos.z = CourtMgr.Get.RealBall.transform.position.z * cameraWithBasketBallCourtRate.y + blankAera;
					}
					break;
				case ETeamKind.Npc:
					if(GameController.Get.BallOwner){
						pos.x = GameController.Get.BallOwner.gameObject.transform.position.x;
						pos.z = GameController.Get.BallOwner.gameObject.transform.position.z * cameraWithBasketBallCourtRate.y - blankAera;
					}
					else
					{
						pos.z = CourtMgr.Get.RealBall.transform.position.z * cameraWithBasketBallCourtRate.y - blankAera;
					}
					break;
				case ETeamKind.Skiller:
					pos = skiller.transform.position;
					break;
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

	public GameObject GetTouch(int Layer)
	{
		GameObject result = null;
		RaycastHit hit;

		if (Physics.Raycast(cameraFx.ScreenPointToRay(Input.mousePosition), out hit, 100, 1 << Layer) && hit.collider)
			result = hit.collider.gameObject;
		
		return result;
	}

	public void SkillShow(GameObject player)
	{
		curTeam = ETeamKind.Skiller;
		skiller = player;
	}

	public void SetRoomMode(EZoomType z, float t)
	{
		CrtZoom = z;
		zoomTime = t;
		isStartRoom = true;
	}
}
