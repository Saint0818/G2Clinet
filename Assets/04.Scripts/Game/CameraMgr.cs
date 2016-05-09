using DG.Tweening;
using GameEnum;
using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

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
    public int CourtMode = ECourtMode.Full;
    public ECameraTest TestCameraMode = ECameraTest.None;
    public bool IsOpenColorfulFX = true;

    //Game const
    private Shake mShake;
    private const float safeZ = 16.5f;
	private const float safeZRateAdd = 1.5f;
	private const float safeZRateMinus = 1.5f;
	private const float focusOffsetBuffer = 3f;
	private const float overRangeRotationSpeed = 10f;
    private float groupOffsetSpeed = 0.005f;
	private const float zoomNormal = 30;
	private const float zoomRange = 20;
	private float zoomTime = 1;
	private const float focusOffsetSpeed = 0.8f;

    private float[] focusStopPoint = new float[]{ 21f, -25f };
    private float cameraRotationSpeed = 2f;
    private float cameraOffsetSpeed = 0.1f;
	private Vector2 blankAera = new Vector2(-2, 3.5f);
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
    private Vector3 jumpBallRotate = new Vector3(12.5f, 90, 0);
    private Vector3 endShowPos = new Vector3(-25f, 8, 0);
    private Vector3 endShowRotate = new Vector3(0, 90, 0);
    private GameObject cameraGroupObj;
    private GameObject cameraRotationObj;
    private GameObject cameraOffsetObj;
    private Camera cameraFx;
    private Camera cameraSkill;
    private Camera cameraPlayerInfo;
    private GameObject cameraSkillCenter;
    private Animator cameraAnimator;

    private GameObject focusTargetOne;
    private GameObject focusTargetTwo;
    private GameObject focusTarget;
    private ECameraSituation situation = ECameraSituation.Loading;
    public bool IsBallOnFloor = false;
    public bool IsLongPass = false;
    private bool isStartRoom = false;
    private GameObject skiller;

    private bool isOverCamera = false;
    private float distanceZ;
    private Vector3 focusSecondPos;
    private Vector2 focusLimitX;
    private Vector2 focusLimitZ;

    public EZoomType CrtZoom = EZoomType.Normal;
    private GameObject showCamera;
    public GameObject SkillDCTarget;
    public GameObject DoubleClickDCBorn;
    [CanBeNull]private Animator showAnimatorControl;
    private Vector2 smothHight = Vector2.zero;
    private float plusZ = 0;
    private int skillEventKind = 0;
    public bool IsTee = false;

    //TestTool
    private GameObject cameraOffsetAeraObj;
    private GameObject focusMoveAeraObj;
    private GameObject focusStopAeraObj;

    void Awake()
    {
        if (cameraGroupObj == null)
        {
            cameraGroupObj = Instantiate(Resources.Load("Prefab/Stadium/Camera")) as GameObject;
            cameraGroupObj.name = "CameraGroup";

            cameraWithBasketBallCourtRate = new Vector2(cameraMoveAera.x / basketballCourt.x, cameraMoveAera.y / basketballCourt.y);
            initCamera();
            InitTestTool();
        }
    }

    void OnDestroy() {
        if (cameraGroupObj)
            Destroy(cameraGroupObj);

        if (cameraFx != null && cameraFx.gameObject)
            Destroy(cameraFx.gameObject);
        
        if (cameraSkill != null && cameraSkill.gameObject)
            Destroy(cameraSkill.gameObject);

        if (cameraPlayerInfo != null && cameraPlayerInfo.gameObject)
            Destroy(cameraPlayerInfo.gameObject);

        if (showCamera != null && showCamera.gameObject)
            Destroy(showCamera.gameObject);
    }

    public override string ResName
    {
        get
        {
            return "CameraMgr";
        }
    }

    public void PlayShake()
    {
        mShake.Play();
	    AudioMgr.Get.PlaySound(SoundType.SD_DunkNormal);
    }

    private void setHalfCourtCamera()
    {
        if (CourtMode == ECourtMode.Half)
        {
            cameraOffsetObj.transform.localPosition = new Vector3(0, 10.35f, -15);
            cameraOffsetObj.transform.eulerAngles = Vector3.zero;
            cameraRotationObj.transform.localPosition = Vector3.zero;
            cameraRotationObj.transform.eulerAngles = new Vector3(21.15f, 0, 0);
            cameraFx.fieldOfView = 35;
        } else
            cameraFx.fieldOfView = 25;

        if (cameraAnimator)
            cameraAnimator.enabled = CourtMode != ECourtMode.Half;
    }

    public void SetCourtCamera(string scene)
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
            /*
			if (!IsOpenColorfulFX && cameraFx) 
			{
				if (cameraFx.GetComponent<CC_Sharpen> ())
					cameraFx.GetComponent<CC_Sharpen> ().enabled = false;

				if (cameraFx.GetComponent<CC_Convolution3x3> ())
					cameraFx.GetComponent<CC_Convolution3x3> ().enabled = false;

				if (cameraFx.GetComponent<CC_RadialBlur> ())
					cameraFx.GetComponent<CC_RadialBlur> ().enabled = false;
			}*/

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
        ShowCourtCamera(true);
    }

    public void FinishGame()
    {
        cameraAnimator.SetTrigger("FinishGame");
    }
	
    public void SetEndShowSituation()
    {
        cameraAnimator.SetTrigger("GameWin");
        cameraPlayerInfo.gameObject.SetActive(false);
        cameraGroupObj.transform.localPosition = Vector3.zero;
        cameraRotationObj.transform.localPosition = endShowPos;
        cameraRotationObj.transform.localEulerAngles = endShowRotate;
    }

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
                ShowCourtCamera(false);
            } else
                ShowCourtCamera(true);
        } 
    }

    public void ShowCameraEnable(bool isEnable)
    {
		InitShowCameraAnimator();
        if (isEnable)
            showAnimatorControl.SetTrigger("ShowTrigger");
        else
            showAnimatorControl.SetTrigger("CloseTrigger");
    }

    public void ShowCourtCamera(bool isShow)
    {
        cameraGroupObj.SetActive(isShow);
    }

    public Camera CourtCamera
    {
        get { return cameraFx; }
    }

    public Animator CourtCameraAnimator
    {
        get { return cameraAnimator; }
    }

    public void ShowPlayerInfoCamera(bool isShow)
    {
        cameraPlayerInfo.gameObject.SetActive(isShow);
    }

    public void InitCamera(ECameraSituation s)
    {
        InitShowCameraAnimator();
        SetCameraSituation(s);
        initCamera();
    }

    private void initCamera()
    {
        if (cameraGroupObj)
        {
            if (cameraOffsetObj == null)
                cameraOffsetObj = cameraGroupObj.transform.FindChild("Offset").gameObject;
            
            if (cameraRotationObj == null)
                cameraRotationObj = cameraGroupObj.transform.FindChild("Offset/Rotation").gameObject;
            
            if (cameraFx == null)
                cameraFx = cameraRotationObj.transform.GetComponentInChildren<Camera>();

            if (!cameraOffsetObj.GetComponent<Shake>())
                mShake = cameraOffsetObj.AddComponent<Shake>();
            
            cameraRotationObj.transform.position = startPos;
            smothHight.y = startPos.y;
            cameraOffsetPos = cameraGroupObj.transform.position;        
        }

        cameraRotationObj.transform.localPosition = jumpBallPos;
        cameraRotationObj.transform.localEulerAngles = jumpBallRotate;
        cameraGroupObj.transform.localPosition = Vector3.zero;
        ShowCourtCamera(false);
        if (cameraPlayerInfo)
            cameraPlayerInfo.gameObject.SetActive(false);
        
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

        if (TestCameraMode == ECameraTest.RGB) {
            cameraOffsetAeraObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cameraOffsetAeraObj.GetComponent<Collider>().enabled = false;
            cameraOffsetAeraObj.name = "ColorR";
            cameraOffsetAeraObj.transform.parent = cameraGroupObj.transform;
            cameraOffsetAeraObj.transform.position = new Vector3(startPos.x, -0.4f, 0);
            cameraOffsetAeraObj.transform.localScale = new Vector3(offsetLimit[0].x - offsetLimit[1].x, 1, offsetLimit[0].z - offsetLimit[1].z);
            cameraOffsetAeraObj.GetComponent<Renderer>().material = Resources.Load("Materials/CameraOffsetAera_M") as Material;
        
            focusStopAeraObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            focusStopAeraObj.GetComponent<Collider>().enabled = false;
            focusStopAeraObj.name = "ColorO";

            focusStopAeraObj.transform.localScale = new Vector3(cameraMoveAera.x, 1, 0.1f);
            focusStopAeraObj.GetComponent<Renderer>().material = Resources.Load("Materials/FocusStopAera_M") as Material;

            SetTestToolPosition();

            cameraOffsetAeraObj.GetComponent<Renderer>().enabled = TestCameraMode == ECameraTest.RGB;
            focusMoveAeraObj.GetComponent<Renderer>().enabled = TestCameraMode == ECameraTest.RGB;
            focusStopAeraObj.GetComponent<Renderer>().enabled = TestCameraMode == ECameraTest.RGB;
            focusTargetOne.GetComponent<Renderer>().enabled = true;
            focusTargetTwo.GetComponent<Renderer>().enabled = true;

            r.material = Resources.Load("Materials/FocusAera_M") as Material;
        } else {
            focusTargetOne.GetComponent<Renderer>().enabled = false;
            focusTargetTwo.GetComponent<Renderer>().enabled = false;
            focusTarget.GetComponent<Renderer>().enabled = false;
            r.enabled = false;
        }
    }

    private bool rotationByJoysticker() {
        if (GameData.Setting.GameRotation && situation != ECameraSituation.Skiller)
            return true;
        else
            return false;
    }

    private void SetTestToolPosition()
    {
        if (TestCameraMode == ECameraTest.RGB)
        {
            if (situation == ECameraSituation.Self)
            {
                focusMoveAeraObj.transform.position = new Vector3(blankAera.x, -0.4f, blankAera.y);
                focusStopAeraObj.transform.position = new Vector3(0, -0.4f, focusStopPoint[0]);    
            }
            else
            {
                focusMoveAeraObj.transform.position = new Vector3(blankAera.x, -0.4f, -blankAera.y);
                focusStopAeraObj.transform.position = new Vector3(0, -0.4f, focusStopPoint[1]);
            }
        }
    }

    public void SetFocus(FocusSensor.FocusSensorMode sensorMode)
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

    void Update()
    {
        if (CourtMode == ECourtMode.Full && 
		   (situation == ECameraSituation.Self ||
        	situation == ECameraSituation.Npc || 
			situation == ECameraSituation.Skiller)) {
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
            } else 
            if (CrtZoom == EZoomType.Out)
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
        if (situation != ECameraSituation.Skiller && situation != ECameraSituation.JumpBall && Time.timeScale > 0)
            cameraGroupObj.transform.position = Vector3.Lerp(cameraGroupObj.transform.position, groupOffsetPoint[situation.GetHashCode()], groupOffsetSpeed);
       
        CameraOffset();
        CameraFocus();
    }

    private void CameraOffset()
    {
        if (situation == ECameraSituation.Skiller)
            return;
        
        cameraOffsetRate.x = (11.5f - focusTargetOne.transform.position.x) / cameraMoveAera.x;
        cameraOffsetRate.z = (22.5f - focusTargetOne.transform.position.z) / cameraMoveAera.y;
        if (cameraOffsetRate.x < 0)
            cameraOffsetRate.x = 0;
        else 
        if (cameraOffsetRate.x > 1)
            cameraOffsetRate.x = 1;
        
        if (cameraOffsetRate.z < 0)
            cameraOffsetRate.z = 0;
        else 
        if (cameraOffsetRate.z > 1)
            cameraOffsetRate.z = 1;

        cameraOffsetPos.x = offsetLimit[0].x - (cameraOffsetRate.x * (offsetLimit[0].x - offsetLimit[1].x));

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

            } else 
            if (distanceZ < -safeZ)
            {
                isOverCamera = true;
                plusZ = safeZRateAdd * (distanceZ + safeZ);
            }
        }
        
        switch (situation)
        {
            case ECameraSituation.Self: 
                cameraOffsetPos.z = offsetLimit[0].z - (cameraOffsetRate.z * (offsetLimit[0].z - offsetLimit[1].z)) - plusZ;
                break;
            case ECameraSituation.Npc:
                cameraOffsetPos.z = offsetLimit[0].z - (cameraOffsetRate.z * (offsetLimit[0].z - offsetLimit[1].z)) + plusZ;
                break;
            default :
                cameraOffsetPos.z = offsetLimit[0].z - (cameraOffsetRate.z * (offsetLimit[0].z - offsetLimit[1].z));
                break;
        }

        if (rotationByJoysticker()) {
            cameraGroupObj.transform.position = Vector3.zero;
            if (CourtMgr.Get.RealBall.State == EPlayerState.Shooting) 
                cameraOffsetPos.z = CourtMgr.Get.RealBall.transform.position.z;
            else
            if (GameController.Get.Joysticker != GameController.Get.BallOwner)
                cameraOffsetPos.z = (GameController.Get.Joysticker.transform.position.z + CourtMgr.Get.RealBall.transform.position.z) / 2;
            else
                cameraOffsetPos.z = GameController.Get.Joysticker.transform.position.z;
            
            if (cameraOffsetPos.z < -8)
                cameraOffsetPos.z = -8;
            else
            if (cameraOffsetPos.z > 8)
                cameraOffsetPos.z = 8;
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
                cameraRotationObj.transform.localEulerAngles = jumpBallRotate;
                break;

            case ECameraSituation.Self:
            case ECameraSituation.Npc:
                Lookat(Vector3.zero);
                if (rotationByJoysticker())
					cameraRotationObj.transform.localEulerAngles = jumpBallRotate;
				else
                	cameraRotationObj.transform.localEulerAngles = new Vector3(restrictedAreaAngle.x, cameraRotationObj.transform.localEulerAngles.y, restrictedAreaAngle.z);
                
				break;

            case ECameraSituation.Skiller:
                focusTargetOne.transform.position = new Vector3(skiller.transform.position.x, skiller.transform.position.y + 2, skiller.transform.position.z);
                Lookat(Vector3.zero);
                break;
        }
    }

	private Vector3 tempPos;
	public void LookatByPause (Vector3 lookPos, Vector3 pos) {
		tempPos = lookPos;
		CourtCamera.transform.DOMove(pos, 0.2f).OnUpdate(PauseCameraLookAt).SetUpdate(UpdateType.Normal, true);
	}

	public void PauseCameraLookAt () {
		CourtCamera.transform.LookAt(tempPos);
	}

	public void GamePause () {
		CourtCameraAnimator.enabled = false;
		CourtCamera.fieldOfView = 20;
	}

	public void GameContinue () {
		CourtCameraAnimator.enabled = true;
		CourtCamera.fieldOfView = 25;
		CourtCamera.transform.DOMove(Vector3.zero , 0.1f);
	}

	public void Lookat(Vector3 pos)
    {
        Vector3 v1;
        float sp;
        Vector3 dir = focusTarget.transform.position - cameraRotationObj.transform.position;
        Quaternion rot = Quaternion.LookRotation(dir);

        if (GameController.Get.BallOwner && GameController.Get.BallOwner != GameController.Get.Joysticker)
            v1 = new Vector3(GameController.Get.BallOwner.transform.position.x, 0, GameController.Get.BallOwner.transform.position.z);
        else
            v1 = new Vector3(CourtMgr.Get.RealBall.transform.position.x, 0, CourtMgr.Get.RealBall.transform.position.z);

        Vector3 BarycentreV3 = new Vector3((cameraFx.gameObject.transform.position.x + GameController.Get.transform.position.x + v1.x) * 1 / 3,
                               0, 
                               (cameraFx.gameObject.transform.position.z + GameController.Get.transform.position.z + v1.z) * 1 / 3);

        if (BarycentreV3.x > focusLimitX.x)
            BarycentreV3.x = focusLimitX.x;
        else 
		if (BarycentreV3.x < focusLimitX.y)
            BarycentreV3.x = focusLimitX.y;  
        
        if (BarycentreV3.z > focusLimitZ.x)
            BarycentreV3.z = focusLimitZ.x;
        else 
		if (BarycentreV3.z < focusLimitZ.y)
            BarycentreV3.z = focusLimitZ.y;
        
        focusTargetTwo.transform.position = Vector3.Lerp(focusTargetTwo.transform.position, BarycentreV3, focusOffsetBuffer * Time.deltaTime);

        if (isOverCamera)
        {
            if (Vector3.Distance(focusTarget.transform.position, focusTargetTwo.transform.position) > 0.1f)
                focusTarget.transform.position = Vector3.Lerp(focusTarget.transform.position, BarycentreV3, focusOffsetBuffer * Time.deltaTime);
           
			sp = overRangeRotationSpeed * Time.deltaTime;
            Vector3 dirMidle = focusTarget.transform.position - cameraRotationObj.transform.position;
            Quaternion rotMidle = Quaternion.LookRotation(dirMidle);
            cameraRotationObj.transform.rotation = Quaternion.Lerp(cameraRotationObj.transform.rotation, rotMidle, sp);
        }
        else
        {
            if (Vector3.Distance(focusTarget.transform.position, focusTargetOne.transform.position) > 0.1f)
            {
                focusTarget.transform.position = Vector3.Lerp(focusTarget.transform.position, focusTargetOne.transform.position, focusOffsetBuffer * Time.deltaTime);
                dir = focusTarget.transform.position - cameraRotationObj.transform.position;
                rot = Quaternion.LookRotation(dir);
            }

            sp = cameraRotationSpeed * Time.deltaTime;
        }

        cameraRotationObj.transform.rotation = Quaternion.Lerp(cameraRotationObj.transform.rotation, rot, sp);
    }

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
                else 
				if (pos.x < focusLimitX.y)
                    pos.x = focusLimitX.y;  

                if (pos.z > focusLimitZ.x)
                    pos.z = focusLimitZ.x;
                else 
				if (pos.z < focusLimitZ.y)
                    pos.z = focusLimitZ.y;
			
                break;

            case ECameraSituation.Npc:
                if (GameController.Get.BallOwner)
                {
                    pos.x = GameController.Get.BallOwner.gameObject.transform.position.x * cameraWithBasketBallCourtRate.x + blankAera.x;
                    pos.z = GameController.Get.BallOwner.gameObject.transform.position.z * cameraWithBasketBallCourtRate.y - blankAera.y;
                }
                else
                {
                    pos.x = CourtMgr.Get.RealBall.transform.position.x * cameraWithBasketBallCourtRate.x + blankAera.x;
                    pos.z = CourtMgr.Get.RealBall.transform.position.z * cameraWithBasketBallCourtRate.y - blankAera.y;
                }

                if (pos.x > focusLimitX.x)
                    pos.x = focusLimitX.x;
                else 
                if (pos.x < focusLimitX.y)
                    pos.x = focusLimitX.y;  
                
                if (pos.z > focusLimitZ.x)
                    pos.z = focusLimitZ.x;
                else 
                if (pos.z < focusLimitZ.y)
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
        LayerMgr.Get.ReSetLayerRecursively(CourtMgr.Get.RealBall.gameObject, "Default", "RealBall");
        switch (skillEventKind)
        {
            case 0://reset self  layer
            case 1:
                LayerMgr.Get.ReSetLayerRecursively(executePlayer.PlayerRefGameObject, "Player", "PlayerModel", "(Clone)");
                break;
            case 2://reset all player's layer
                GameController.Get.SetAllPlayerLayer("Player");
                break;
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

	private PlayerBehaviour executePlayer;

	public void SkillShowActive(PlayerBehaviour player, int kind = 0, float t = 1.5f)
    {
		executePlayer = player;
        skillEventKind = kind;
        cameraFx.enabled = false;
        cameraSkill.gameObject.SetActive(true);
		cameraSkillCenter.transform.position = player.PlayerRefGameObject.transform.position;
        cameraSkill.gameObject.transform.parent = cameraSkillCenter.transform;
        switch (kind)
        {
            case 0: //rotate
            case 1://take self
				TweenFOV.Begin(cameraSkill.gameObject, 0.3f, 15);
				cameraSkill.gameObject.transform.DOLookAt(player.PlayerRefGameObject.transform.position + new Vector3(0, 2, 0), 0.5f).SetEase(Ease.Linear);
				cameraSkillCenter.transform.DOLocalRotate(cameraSkillCenter.transform.eulerAngles + new Vector3(0, 360, 0), t , RotateMode.WorldAxisAdd).SetEase(Ease.Linear).OnUpdate(LootAtPlayer).OnComplete(StopSkill);

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
		cameraSkill.gameObject.transform.LookAt(executePlayer.PlayerRefGameObject.transform.position + new Vector3(0, 2, 0));
    }

    public void ResetCamera()
    {
		executePlayer.StopSkill();
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

    public void ShowEnd()
    {
        showCamera.SetActive(false);
    }

    private void InitShowCameraAnimator()
    {
		string name = string.Format("InGameStartShow_{0}", SceneMgr.Get.CurrentSceneNo);
		
		if (showCamera)
		{
			if (showCamera.name == name) 
				return;
			else 
			{
				Destroy (showCamera);
				showCamera = null;	
			}
		}
		
        if (showCamera == null)
        {
            string path = string.Format("Prefab/Camera/"+ name);
            Object obj = Resources.Load(path);
            if (!obj) {
                Debug.LogError("Missing Prefab : " + path);
                path =  string.Format("Prefab/Camera/InGameStartShow_{0}", 0);
                obj = Resources.Load(path);
                if (!obj)
                    showCamera = Instantiate(obj) as GameObject;
            }

            if (obj) {
                showCamera = Instantiate(obj) as GameObject;
			    showCamera.name = name;
                showAnimatorControl = showCamera.GetComponent<Animator>();
            }
        }
    }
}
