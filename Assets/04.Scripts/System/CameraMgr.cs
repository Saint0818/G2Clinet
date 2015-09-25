using UnityEngine;
using System.Collections;
using DG.Tweening;
using GameEnum;
using GamePlayEnum;

public enum EZoomType
{
    Normal,
    In,
    Out
}

public enum ECameraSituation
{
    Loading = -3,
    Show = -2,
    JumpBall = -1,
    Self = 0,
    Npc = 1,
    Skiller = 2,
    Finish = 3
}

public class CameraMgr : KnightSingleton<CameraMgr>
{
    //Game const
    private Shake mShake;
    private float safeZ = 15f;
    private float safeZRateAdd = 1.5f;
    private float safeZRateMinus = 1.5f;
	private float focusOffsetBuffer = 3f;
	private float overRangeRotationSpeed = 10f;
    private float groupOffsetSpeed = 0.005f;
    private float zoomNormal = 30;
    private float zoomRange = 20;
    private float zoomTime = 1;
    public Vector2 blankAera = new Vector2(-2, 3.5f);
//    private float lockedFocusAngle = 100f;
//    private float lockedTeeFocusAngle = 50f;
    private float focusOffsetSpeed = 0.8f;
    private float[] focusStopPoint = new float[]{21f, -25f};
    private float cameraRotationSpeed = 2f;
    private float cameraOffsetSpeed = 0.1f;
    private Vector2 cameraWithBasketBallCourtRate;
    private Vector2 cameraMoveAera = new Vector2(14f, 25f);
    private Vector3 cameraOffsetRate = Vector3.zero;
    private Vector2 basketballCourt = new Vector2(21f, 30.5f);
    private Vector3 restrictedAreaAngle = new Vector3(14f, 1, 0);
    private Vector3 cameraOffsetPos = Vector3.zero;
    private Vector3 startPos = new Vector3(-17.36f, 10f, 0.67f);
    private Vector3[] groupOffsetPoint = new Vector3[]
    {
        new Vector3(0, 0, -8f),
        new Vector3(0, 0, 8f)
    };
    private Vector3[] offsetLimit = new Vector3[]
    {
        new Vector3(-12f, 0, 1.63f),
        new Vector3(-31f, 0, -1.63f)
    };
    private Vector3 jumpBallPos = new Vector3(-25f, 8, 0);
    private Vector3 jumpBallRoate = new Vector3(12.5f, 90, 0);
    private GameObject cameraGroupObj;
    private GameObject cameraRotationObj;
    private GameObject cameraOffsetObj;
    private Camera cameraFx;
    private Camera cameraSkill;
	private Camera cameraPlayerInfo;
    private GameObject cameraSkillCenter;
    private Animator cameraAnimator;

//  private Camera cameraPlayer;

    private GameObject focusTargetOne;
    private GameObject focusTargetTwo;
	private GameObject focusTarget;
    private ECameraSituation situation = ECameraSituation.Loading;
    public bool IsBallOnFloor = false;
    public bool IsLongPass = false;
    private bool isStartRoom = false;
    private GameObject skiller;

    //TestTool
    private GameObject cameraOffsetAeraObj;
    private GameObject focusMoveAeraObj;
    private GameObject focusStopAeraObj;
    public EZoomType CrtZoom = EZoomType.Normal;
    private GameObject showCamera;
    public Animator ShowAnimatorControl;
    public GameObject[] CharacterPos = new GameObject[6];
    public GameObject SkillDCTarget;
    public GameObject DoubleClickDCBorn;
    private Vector2 smothHight = Vector2.zero;
    private float plusZ = 0;
    private int skillEventKind = 0;

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
            initCamera();
            InitTestTool();

            showCamera = Instantiate(Resources.Load("Prefab/Camera/InGameStartShow_0")) as GameObject;
            ShowAnimatorControl = showCamera.GetComponent<Animator>();

            if (showCamera)
                for (int i = 0; i < CharacterPos.Length; i++)
                    CharacterPos [i] = showCamera.transform.FindChild(string.Format("CharacterPos/{0}", i)).gameObject;
        }
    }

    public void PlayShake()
    {
        mShake.Play();
        AudioMgr.Get.PlaySound(SoundType.SD_Dunk);
    }

    private void setHalfCourtCamera()
    {
        if (GameStart.Get.CourtMode == ECourtMode.Half)
        {
            cameraOffsetObj.transform.localPosition = new Vector3(0, 10.35f, -15);
            cameraOffsetObj.transform.eulerAngles = Vector3.zero;
            cameraRotationObj.transform.localPosition = Vector3.zero;
            cameraRotationObj.transform.eulerAngles = new Vector3(21.15f, 0, 0);
            cameraFx.fieldOfView = 35;
        } else 
            cameraFx.fieldOfView = 25;

        if (cameraAnimator) 
            cameraAnimator.enabled = GameStart.Get.CourtMode != ECourtMode.Half;
    }

    public void SetCourtCamera(ESceneName scene)
    {
        if (cameraFx && cameraFx.name != scene.ToString())
        {
            Destroy(cameraFx.gameObject);
            GameObject obj = Instantiate(Resources.Load(string.Format("Prefab/Camera/Camera_{0}", scene.ToString()))) as GameObject;
            cameraFx = obj.GetComponent<Camera>();
            cameraFx.gameObject.transform.parent = cameraRotationObj.transform;
            cameraFx.gameObject.transform.localPosition = Vector3.zero;
            cameraFx.gameObject.transform.localEulerAngles = Vector3.zero;
            cameraFx.gameObject.name = scene.ToString();
            cameraAnimator = cameraFx.GetComponent<Animator>();

            cameraSkill = (Instantiate(Resources.Load("Prefab/Camera/Camera_Skill")) as GameObject).GetComponent<Camera>();
            cameraSkill.gameObject.transform.parent = cameraRotationObj.transform;
            cameraSkill.gameObject.transform.localPosition = Vector3.zero;
            cameraSkill.gameObject.transform.localEulerAngles = Vector3.zero;
            cameraSkill.gameObject.SetActive(false);

			cameraPlayerInfo = (Instantiate(Resources.Load("Prefab/Camera/Camera_PlayerInfo")) as GameObject).GetComponent<Camera>();
			cameraPlayerInfo.gameObject.transform.parent = cameraRotationObj.transform;
			cameraPlayerInfo.gameObject.transform.localPosition = Vector3.zero;
			cameraPlayerInfo.gameObject.transform.localEulerAngles = Vector3.zero;
			cameraPlayerInfo.gameObject.SetActive(false);

            cameraSkillCenter = new GameObject();
            cameraSkillCenter.name = "CameraSkillCenter";

            Transform tEnd = obj.transform.FindChild("DC_Pos/End_Point");
            if (tEnd)
                SkillDCTarget = tEnd.gameObject;

            Transform tDC = obj.transform.FindChild("DC_Pos/DC_Point");
            if (tDC)
                DoubleClickDCBorn = tDC.gameObject;
        }

        setHalfCourtCamera();
    }
    
    public void PlayGameStartCamera()
    {
        cameraAnimator.SetTrigger("InGameStart");
    }

    public void FinishGame()
    {
        cameraAnimator.SetTrigger("FinishGame");
    }

    public void SetSelectRoleCamera()
    {
        if (cameraFx)
        {
            Destroy(cameraFx.gameObject);
            GameObject obj = Instantiate(Resources.Load("Prefab/Camera/Camera_SelectRole")) as GameObject;
            cameraFx = obj.GetComponent<Camera>();
            cameraFx.gameObject.transform.localPosition = Vector3.zero;
            cameraFx.gameObject.transform.localEulerAngles = Vector3.zero;
            cameraFx.gameObject.name = "Camera_SelectRole";
        }
    }

    public bool IsTee = false;

    public void SetCameraSituation(ECameraSituation s, bool isTee = false)
    {
        IsTee = isTee;
        situation = s;
        SetTestToolPosition();

        if (s != ECameraSituation.Finish)
        {
            if (s == ECameraSituation.Show)
            {
                ShowCameraEnable(true);
                cameraGroupObj.SetActive(false);
            } else
                cameraGroupObj.SetActive(true);
        }
    }

    public void ShowCameraEnable(bool isEnable)
    {
        if (isEnable)
            showCamera.GetComponent<Animator>().SetTrigger("ShowTrigger");
        else
            showCamera.GetComponent<Animator>().SetTrigger("CloseTrigger");
    }

    public Camera CourtCamera
    {
        get { return cameraFx;}
    }

	public Animator CourtCameraAnimator {
		get {return cameraAnimator; }
	}

	public void ShowPlayerInfoCamera (bool isShow) {
		cameraPlayerInfo.gameObject.SetActive(isShow);
	}

    public void InitCamera(ECameraSituation s)
    {
        SetCameraSituation(s);
        initCamera();
    }

    private void initCamera()
    {
        if (cameraGroupObj)
        {
            cameraOffsetObj = cameraGroupObj.transform.FindChild("Offset").gameObject;
            cameraRotationObj = cameraGroupObj.transform.FindChild("Offset/Rotation").gameObject;
            cameraFx = cameraRotationObj.transform.GetComponentInChildren<Camera>();

            if (!cameraOffsetObj.GetComponent<Shake>())
                mShake = cameraOffsetObj.AddComponent<Shake>();
            
            cameraRotationObj.transform.position = startPos;
            smothHight.y = startPos.y;
            cameraOffsetPos = cameraGroupObj.transform.position;        
        }

        cameraGroupObj.transform.localPosition = Vector3.zero;
        cameraRotationObj.transform.localPosition = jumpBallPos;
        cameraRotationObj.transform.localEulerAngles = jumpBallRoate;
        setHalfCourtCamera();

        if (focusTargetOne == null)
        {
            focusTargetOne = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            focusTargetOne.GetComponent<Collider>().enabled = false;
            focusTargetOne.name = "focusTargetOne";
        }

        if (focusTargetTwo == null)
        {
            focusTargetTwo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            focusTargetTwo.GetComponent<Collider>().enabled = false;
            focusTargetTwo.name = "focusTargetTwo";
        }

		if (focusTarget == null)
		{
			focusTarget = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			focusTarget.GetComponent<Collider>().enabled = false;
			focusTarget.name = "focusTarget";
		}
		
		if (CourtMgr.Get.RealBall)
			focusTargetOne.transform.position = CourtMgr.Get.RealBall.transform.position;
    }

    private void InitTestTool()
    {
        focusMoveAeraObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        focusMoveAeraObj.GetComponent<Collider>().enabled = false;
        focusMoveAeraObj.transform.localScale = new Vector3(cameraMoveAera.x, 1, cameraMoveAera.y);
        focusMoveAeraObj.name = "ColorG";
        Renderer r = focusMoveAeraObj.GetComponent<Renderer>();

        if (GameStart.Get.TestCameraMode == ECameraTest.RGB)
        {
            cameraOffsetAeraObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cameraOffsetAeraObj.GetComponent<Collider>().enabled = false;
            cameraOffsetAeraObj.name = "ColorR";
            cameraOffsetAeraObj.transform.parent = cameraGroupObj.transform;
            cameraOffsetAeraObj.transform.position = new Vector3(startPos.x, -0.4f, 0);
            cameraOffsetAeraObj.transform.localScale = new Vector3(offsetLimit [0].x - offsetLimit [1].x, 1, offsetLimit [0].z - offsetLimit [1].z);
            cameraOffsetAeraObj.GetComponent<Renderer>().material = Resources.Load("Materials/CameraOffsetAera_M") as Material;
        
            focusStopAeraObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            focusStopAeraObj.GetComponent<Collider>().enabled = false;
            focusStopAeraObj.name = "ColorO";

            focusStopAeraObj.transform.localScale = new Vector3(cameraMoveAera.x, 1, 0.1f);
            focusStopAeraObj.GetComponent<Renderer>().material = Resources.Load("Materials/FocusStopAera_M") as Material;

            SetTestToolPosition();

            cameraOffsetAeraObj.GetComponent<Renderer>().enabled = GameStart.Get.TestCameraMode == ECameraTest.RGB;
            focusMoveAeraObj.GetComponent<Renderer>().enabled = GameStart.Get.TestCameraMode == ECameraTest.RGB;
            focusStopAeraObj.GetComponent<Renderer>().enabled = GameStart.Get.TestCameraMode == ECameraTest.RGB;
            focusTargetOne.GetComponent<Renderer>().enabled = true;
            focusTargetTwo.GetComponent<Renderer>().enabled = true;

            r.material = Resources.Load("Materials/FocusAera_M") as Material;
        } else
        {
            focusTargetOne.GetComponent<Renderer>().enabled = false;
            focusTargetTwo.GetComponent<Renderer>().enabled = false;
			focusTarget.GetComponent<Renderer>().enabled = false;
            r.enabled = false;
        }
    }

    private void SetTestToolPosition()
    {
        if (GameStart.Get.TestCameraMode == ECameraTest.RGB)
        {
            if (situation == ECameraSituation.Self)
            {
                focusMoveAeraObj.transform.position = new Vector3(blankAera.x, -0.4f, blankAera.y);
                focusStopAeraObj.transform.position = new Vector3(0, -0.4f, focusStopPoint [0]);    
            } else
            {
                focusMoveAeraObj.transform.position = new Vector3(blankAera.x, -0.4f, -blankAera.y);
                focusStopAeraObj.transform.position = new Vector3(0, -0.4f, focusStopPoint [1]);
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
        if (GameStart.Get.CourtMode == ECourtMode.Full && (situation == ECameraSituation.Self || 
            situation == ECameraSituation.Npc || situation == ECameraSituation.Skiller))
        {
            ZoomCalculation();
            HorizontalCameraHandle();
        }
    }

    private void ZoomCalculation()
    {
        if (isStartRoom)
        {
            if (CrtZoom == EZoomType.In)
            {
                cameraFx.fieldOfView = Mathf.Lerp(cameraFx.fieldOfView, zoomRange, zoomTime);

                if (Mathf.Approximately(cameraFx.fieldOfView, zoomRange))
                    isStartRoom = false;
            } else if (CrtZoom == EZoomType.Out)
            {
                cameraFx.fieldOfView = Mathf.Lerp(cameraFx.fieldOfView, zoomNormal, zoomTime);

                if (Mathf.Approximately(cameraFx.fieldOfView, zoomNormal))
                    isStartRoom = false;
            }
        }
    }

    private void HorizontalCameraHandle()
    {
        //GroupOffset
        if (situation != ECameraSituation.Skiller && situation != ECameraSituation.JumpBall)
            cameraGroupObj.transform.position = Vector3.Lerp(cameraGroupObj.transform.position, groupOffsetPoint [situation.GetHashCode()], groupOffsetSpeed);

        CameraOffset();
        CameraFocus();
//      if(cameraPlayer && GameController.Get.Joysticker != null) {
//          cameraPlayer.gameObject.transform.LookAt(GameController.Get.Joysticker.gameObject.transform);
//      }
    }

    private bool isOverCamera = false;
    private float distanceZ;

    private void CameraOffset()
    {
        if (situation == ECameraSituation.Skiller)
            return;
        cameraOffsetRate.x = (11.5f - focusTargetOne.transform.position.x) / cameraMoveAera.x;
        cameraOffsetRate.z = (22.5f - focusTargetOne.transform.position.z) / cameraMoveAera.y;
        if (cameraOffsetRate.x < 0)
            cameraOffsetRate.x = 0;
        else if (cameraOffsetRate.x > 1)
            cameraOffsetRate.x = 1;
        
        if (cameraOffsetRate.z < 0)
            cameraOffsetRate.z = 0;
        else if (cameraOffsetRate.z > 1)
            cameraOffsetRate.z = 1;

        cameraOffsetPos.x = offsetLimit [0].x - (cameraOffsetRate.x * (offsetLimit [0].x - offsetLimit [1].x));

        if (GameController.Get.Shooter)
        {
            float h;

            if (CourtMgr.Get.RealBall.transform.position.y > 10f)
                h = 2;
            else 
                h = (CourtMgr.Get.RealBall.transform.position.y / 10f) * 2f;

            smothHight = Vector2.Lerp(smothHight, new Vector2(0, startPos.y + h), 0.1f);
        } else
            smothHight = Vector2.Lerp(smothHight, new Vector2(0, startPos.y), 0.1f);

        cameraOffsetPos.y = smothHight.y;
        plusZ = 0;
        isOverCamera = false;
        distanceZ = 0;

        if (!GameController.Get.Joysticker.IsBallOwner)
        {
            if (GameController.Get.BallOwner)
                distanceZ = Vector3.Distance(GameController.Get.BallOwner.transform.position, GameController.Get.Joysticker.transform.position);
            else
                distanceZ = Vector3.Distance(CourtMgr.Get.RealBall.transform.position, GameController.Get.Joysticker.transform.position);

            if (distanceZ > safeZ)
            {
                isOverCamera = true;
                plusZ = safeZRateMinus * (distanceZ - safeZ);

            } else if (distanceZ < -safeZ)
            {
                isOverCamera = true;
                plusZ = safeZRateAdd * (distanceZ + safeZ);
            }
        }
        
        switch (situation)
        {
            case ECameraSituation.Self: 
                cameraOffsetPos.z = offsetLimit [0].z - (cameraOffsetRate.z * (offsetLimit [0].z - offsetLimit [1].z)) - plusZ;
                break;
            case ECameraSituation.Npc:
                cameraOffsetPos.z = offsetLimit [0].z - (cameraOffsetRate.z * (offsetLimit [0].z - offsetLimit [1].z)) + plusZ;
                break;
            default :
                cameraOffsetPos.z = offsetLimit [0].z - (cameraOffsetRate.z * (offsetLimit [0].z - offsetLimit [1].z));
                break;
        }

        cameraRotationObj.transform.localPosition = Vector3.Lerp(cameraRotationObj.transform.localPosition, cameraOffsetPos, cameraOffsetSpeed);
    }

    private void CameraFocus()
    {
        focusObjectOffset();

        switch (situation)
        {
            case ECameraSituation.JumpBall:
                cameraRotationObj.transform.localPosition = jumpBallPos;
                cameraRotationObj.transform.localEulerAngles = jumpBallRoate;
                break;
            case ECameraSituation.Self:

			Lookat(Vector3.zero);
			cameraRotationObj.transform.localEulerAngles = new Vector3(restrictedAreaAngle.x, cameraRotationObj.transform.localEulerAngles.y, restrictedAreaAngle.z);

//                if (!IsTee)
//                {
////                    if (focusTargetOne.transform.position.z < focusStopPoint [situation.GetHashCode()])
////                    {
//                    Lookat(Vector3.zero);
//                    cameraRotationObj.transform.localEulerAngles = new Vector3(restrictedAreaAngle.x, cameraRotationObj.transform.localEulerAngles.y, restrictedAreaAngle.z);
////                    } else
////                    {
////                        float angle = Mathf.LerpAngle(cameraRotationObj.transform.localEulerAngles.y, lockedFocusAngle, focusSmoothSpeed);
////                        cameraRotationObj.transform.localEulerAngles = new Vector3(restrictedAreaAngle.x, angle, restrictedAreaAngle.z);
////                    }
//                } else
//                {
//                    float angle = Mathf.LerpAngle(cameraRotationObj.transform.localEulerAngles.y, lockedTeeFocusAngle, focusSmoothSpeed);
//                    cameraRotationObj.transform.localEulerAngles = new Vector3(restrictedAreaAngle.x, angle, restrictedAreaAngle.z);
//                }

                break;
            case ECameraSituation.Npc:
			Lookat(Vector3.zero);
			cameraRotationObj.transform.localEulerAngles = new Vector3(restrictedAreaAngle.x, cameraRotationObj.transform.localEulerAngles.y, restrictedAreaAngle.z);
//                if (!IsTee)
//                {
////                    if (focusTargetOne.transform.position.z > focusStopPoint [situation.GetHashCode()])
////                    {
//                    Lookat(Vector3.zero);
//                    cameraRotationObj.transform.localEulerAngles = new Vector3(restrictedAreaAngle.x, cameraRotationObj.transform.localEulerAngles.y, restrictedAreaAngle.z);
////                    } else
////                    {
////                        float angle = Mathf.LerpAngle(cameraRotationObj.transform.localEulerAngles.y, 180 - lockedFocusAngle, focusSmoothSpeed);
////                        cameraRotationObj.transform.localEulerAngles = new Vector3(restrictedAreaAngle.x, angle, restrictedAreaAngle.z);
////                    }
//                } else
//                {
//                    float angle = Mathf.LerpAngle(cameraRotationObj.transform.localEulerAngles.y, 180 - lockedTeeFocusAngle, focusSmoothSpeed);
//                    cameraRotationObj.transform.localEulerAngles = new Vector3(restrictedAreaAngle.x, angle, restrictedAreaAngle.z);
//                }
                break;

            case ECameraSituation.Skiller:
                focusTargetOne.transform.position = new Vector3(skiller.transform.position.x, skiller.transform.position.y + 2, skiller.transform.position.z);
                Lookat(Vector3.zero);
                break;
        }
    }

    private Vector3 focusSecondPos;

    private void Lookat(Vector3 pos)
    {
		Vector3 v1;
		float sp;
		Vector3 dir = focusTarget.transform.position - cameraRotationObj.transform.position;
		Quaternion rot = Quaternion.LookRotation(dir);

		if (GameController.Get.BallOwner && GameController.Get.BallOwner != GameController.Get.Joysticker)
            v1 = new Vector3(GameController.Get.BallOwner.transform.position.x, 0, GameController.Get.BallOwner.transform.position.z);
		else
			v1 = new Vector3(CourtMgr.Get.RealBall.transform.position.x, 0, CourtMgr.Get.RealBall.transform.position.z);

        Vector3 BarycentreV3 = new Vector3((cameraFx.gameObject.transform.position.x + GameController.Get.transform.position.x + v1.x) * 1/3,
                                   0, 
                                   (cameraFx.gameObject.transform.position.z + GameController.Get.transform.position.z + v1.z) * 1/3);

        if (BarycentreV3.x > focusLimitX.x)
            BarycentreV3.x = focusLimitX.x;
        else if (BarycentreV3.x < focusLimitX.y)
            BarycentreV3.x = focusLimitX.y;  
        
        if (BarycentreV3.z > focusLimitZ.x)
            BarycentreV3.z = focusLimitZ.x;
        else if (BarycentreV3.z < focusLimitZ.y)
            BarycentreV3.z = focusLimitZ.y;
        
		focusTargetTwo.transform.position = Vector3.Lerp(focusTargetTwo.transform.position, BarycentreV3, focusOffsetBuffer * Time.deltaTime);

        if (isOverCamera)
        {
			if(Vector3.Distance(focusTarget.transform.position, focusTargetTwo.transform.position) > 0.1f)
			{
				focusTarget.transform.position = Vector3.Lerp(focusTarget.transform.position, BarycentreV3, focusOffsetBuffer * Time.deltaTime);
			}
			sp = overRangeRotationSpeed * Time.deltaTime;
			Vector3 dirMidle = focusTarget.transform.position - cameraRotationObj.transform.position;
            Quaternion rotMidle = Quaternion.LookRotation(dirMidle);

			cameraRotationObj.transform.rotation = Quaternion.Lerp(cameraRotationObj.transform.rotation, rotMidle, sp);
//			cameraRotationObj.transform.rotation = Quaternion.Lerp(rot, rotMidle, sp);
        } else
        {
			if(Vector3.Distance(focusTarget.transform.position, focusTargetOne.transform.position) > 0.1f)
			{
				focusTarget.transform.position = Vector3.Lerp(focusTarget.transform.position, focusTargetOne.transform.position, focusOffsetBuffer * Time.deltaTime);
				dir = focusTarget.transform.position - cameraRotationObj.transform.position;
				rot = Quaternion.LookRotation(dir);
			}
			sp = cameraRotationSpeed * Time.deltaTime;

//			cameraRotationObj.transform.rotation = Quaternion.Lerp(cameraRotationObj.transform.rotation, rot, );
        }

		cameraRotationObj.transform.rotation = Quaternion.Lerp(cameraRotationObj.transform.rotation, rot, sp);
    }

    private Vector2 focusLimitX;
    private Vector2 focusLimitZ;

    private void focusObjectOffset()
    {
        Vector3 pos = Vector3.zero;
        pos.x = CourtMgr.Get.RealBall.transform.position.x;
        pos.y = 0;

        focusLimitX = new Vector2(focusMoveAeraObj.transform.position.x + focusMoveAeraObj.transform.localScale.x / 2,
                                          focusMoveAeraObj.transform.position.x - focusMoveAeraObj.transform.localScale.x / 2);

        focusLimitZ = new Vector2(focusMoveAeraObj.transform.position.z + focusMoveAeraObj.transform.localScale.z / 2,
                                          focusMoveAeraObj.transform.position.z - focusMoveAeraObj.transform.localScale.z / 2);

        switch (situation)
        {
            case ECameraSituation.JumpBall:
                pos = Vector3.zero;
                break;
            case ECameraSituation.Self:
                if (GameController.Get.BallOwner)
                {
                    pos.x = GameController.Get.BallOwner.gameObject.transform.position.x * cameraWithBasketBallCourtRate.x + blankAera.x;
                    pos.z = GameController.Get.BallOwner.gameObject.transform.position.z * cameraWithBasketBallCourtRate.y + blankAera.y;

                } else
                {
                    pos.x = CourtMgr.Get.RealBall.transform.position.x * cameraWithBasketBallCourtRate.x + blankAera.x;
                    pos.z = CourtMgr.Get.RealBall.transform.position.z * cameraWithBasketBallCourtRate.y + blankAera.y;
                }

                if (pos.x > focusLimitX.x)
                    pos.x = focusLimitX.x;
                else if (pos.x < focusLimitX.y)
                    pos.x = focusLimitX.y;  

                if (pos.z > focusLimitZ.x)
                    pos.z = focusLimitZ.x;
                else if (pos.z < focusLimitZ.y)
                    pos.z = focusLimitZ.y;
                break;

            case ECameraSituation.Npc:
                if (GameController.Get.BallOwner)
                {
                    pos.x = GameController.Get.BallOwner.gameObject.transform.position.x * cameraWithBasketBallCourtRate.x + blankAera.x;
                    pos.z = GameController.Get.BallOwner.gameObject.transform.position.z * cameraWithBasketBallCourtRate.y - blankAera.y;
                } else
                {
                    pos.x = CourtMgr.Get.RealBall.transform.position.x * cameraWithBasketBallCourtRate.x + blankAera.x;
                    pos.z = CourtMgr.Get.RealBall.transform.position.z * cameraWithBasketBallCourtRate.y - blankAera.y;
                }

                if (pos.x > focusLimitX.x)
                    pos.x = focusLimitX.x;
                else if (pos.x < focusLimitX.y)
                    pos.x = focusLimitX.y;  
                
                if (pos.z > focusLimitZ.x)
                    pos.z = focusLimitZ.x;
                else if (pos.z < focusLimitZ.y)
                    pos.z = focusLimitZ.y;
                break;

            case ECameraSituation.Skiller:
                pos = skiller.transform.position;
                break;
        }

        focusTargetOne.transform.position = Vector3.Slerp(focusTargetOne.transform.position, pos, focusOffsetSpeed);
    }
    
    private void resetSKillLayer()
    {
        GameFunction.ReSetLayerRecursively(CourtMgr.Get.RealBall, "Default", "RealBall");
        switch (skillEventKind)
        {
            case 0://reset self  layer
            case 1:
                GameFunction.ReSetLayerRecursively(GameController.Get.Joysticker.gameObject, "Player", "PlayerModel", "(Clone)");
                break;
            case 2://reset all player's layer
                GameController.Get.SetAllPlayerLayer("Player");
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
        skiller = player;
        SetCameraSituation(ECameraSituation.Skiller);
    }

    public void SkillShowActive(int kind = 0, float t = 1.5f)
    {
        skillEventKind = kind;
        cameraFx.enabled = false;
        cameraSkill.gameObject.SetActive(true);
        cameraSkillCenter.transform.position = GameController.Get.Joysticker.transform.position;
        cameraSkill.gameObject.transform.parent = cameraSkillCenter.transform;
        switch (kind)
        {
            case 0: //rotate
            case 1://take self
                TweenFOV.Begin(cameraSkill.gameObject, 0.3f, 15);
                cameraSkill.gameObject.transform.DOLookAt(GameController.Get.Joysticker.transform.position + new Vector3(0, 2, 0), 0.5f).SetEase(Ease.Linear);
                if (kind == 0)  
                    cameraSkillCenter.transform.DOLocalRotate(cameraSkillCenter.transform.eulerAngles + new Vector3(0, 360, 0), (t - 0.3f), RotateMode.WorldAxisAdd).SetEase(Ease.Linear).OnUpdate(LootAtPlayer).OnComplete(StopSkill);
                else
                    Invoke("StopSkill", (t - 0.3f));

                break;
            case 2://take all player
                TweenFOV.Begin(cameraSkill.gameObject, 0.3f, 45);
                Invoke("StopSkill", (t - 0.3f));
                break;
        }
    }

    public void StopSkill()
    {
        TweenFOV.Begin(cameraSkill.gameObject, 0.3f, 25);
        cameraSkill.gameObject.transform.parent = cameraRotationObj.transform;
        cameraSkill.gameObject.transform.DOLocalMove(Vector3.zero, 0.3f);
        cameraSkill.gameObject.transform.DOLocalRotate(Vector3.zero, 0.3f).OnComplete(ResetCamera);
    }

    public void LootAtPlayer()
    {
        cameraSkill.gameObject.transform.LookAt(GameController.Get.Joysticker.transform.position + new Vector3(0, 2, 0));
    }

    public void ResetCamera()
    {
        GameController.Get.Joysticker.StopSkill();
        cameraFx.enabled = true;
        cameraSkill.gameObject.SetActive(false);
        cameraSkill.gameObject.transform.localEulerAngles = Vector3.zero;
        cameraSkill.gameObject.transform.localPosition = Vector3.zero;
        resetSKillLayer();
    }

    public void SetRoomMode(EZoomType z, float t)
    {
        CrtZoom = z;
        zoomTime = t;
        isStartRoom = true;
    }
}
