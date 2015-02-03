using UnityEngine;
using System.Collections;

public class CameraMgr : KnightSingleton<CameraMgr>
{
    private bool isOpenAutoFocus = false;
    private bool isRealBall = false;
    private int situationIndex = 0;
	public Camera UI2DCamera;
    public Camera uiCam;
    public CamMode crrentMode = CamMode.None;
    private GameObject uiCamGp;
    private GameObject uiCamOffset;
    private GameObject curFocus;
    private Moba_Camera mobaCamera;
//    private AmplifyColorEffect mapColor;
	private int uiBuildKind = 0;
	public float speed = 1.5f;
	private float trunTime = 0f;
	private Vector3 StartPos = new Vector3(-11f,1.32f,14.68034f);
	private Vector3 MoveToBuildPos = new Vector3(-8.5f, 5.5f, -7f);
//	public AnaglyphizerC anaglyphizer;
	public bool IsOpenAutoStereoscopic3D = false;
	
    public enum CamMode
    {
        None,
        ReadyToGame,
        UI,
        VerticalInGame,
        HorizontalInGame,
		RecordShoot3Game
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

    #region Monobehaviors
    void Awake()
    {
		UI2DCamera = GameObject.Find("UI2D/2DCamera").camera;
        if (uiCamGp == null)
        {
            uiCamGp = Instantiate(Resources.Load("Prefab/Moba")) as GameObject;
            uiCamGp.transform.parent = gameObject.transform;

            uiCamOffset = uiCamGp.transform.FindChild("MobaOffset").gameObject;
            uiCam = uiCamGp.GetComponentInChildren<Camera>();
//			anaglyphizer = uiCam.GetComponent<AnaglyphizerC>();
//			anaglyphizer.enabled = IsOpenAutoStereoscopic3D;

			if(uiCam && uiCam.animation)
				uiCam.animation.Stop();

            mobaCamera = uiCamGp.GetComponentInChildren<Moba_Camera>();
//            mapColor = uiCam.gameObject.AddComponent<AmplifyColorEffect>();
//            mapColor.MaskTexture = (Texture2D)Resources.Load("Stadiums/Color/Mask") as Texture2D;
        }
            
        SetCamMode(CamMode.UI);
    }
    #endregion

    public void InitUICam()
    {
        if (uiCamGp == null)
        {
            Debug.LogError("UICamGp is null, check please");
            return;
        }

        uiCamGp.transform.localPosition = Vector3.zero;
        uiCamGp.transform.localEulerAngles = Vector3.zero;

        uiCamOffset.transform.localPosition = Vector3.zero;
        uiCamOffset.transform.localEulerAngles = Vector3.zero;

//      uiCam.fieldOfView = 60;
//      uiCam.transform.localPosition = new Vector3 (-14, 4.4f, 13.29699f);
//      uiCam.transform.localEulerAngles = new Vector3 (3.7f, 90, 0);

        uiCam.fieldOfView = 35;
        uiCam.transform.localPosition = new Vector3(-11f, 1.32f, 14.8f);
        uiCam.transform.localEulerAngles = new Vector3(5.25f, 90, 0);
        uiCam.farClipPlane = 130;
         
        if (mobaCamera != null) 
            mobaCamera.Stop = true;
    }

    public void InitVerticalInGame()
    {
        if (uiCamGp == null)
        {
            Debug.LogError("UICamGp is null, check please");
            return;
        }
		uiCamGp.transform.position = new Vector3 (0, 0, -9.773501f);
		uiCamGp.transform.localEulerAngles = new Vector3(0, 90, 0);
        uiCamOffset.transform.localPosition = new Vector3(0, 8.15f, -17);
        
        uiCam.fieldOfView = 30;
        uiCam.transform.localPosition = new Vector3(-30, 3, 5.8f);
        uiCam.transform.localEulerAngles = new Vector3(22.5f, 60, 0);
        uiCam.farClipPlane = 130;
    }

    public void InitHorizontalInGameCam()
    {
        if (uiCamGp == null)
        {
            Debug.LogError("UICamGp is null, check please");
            return;
        }

        uiCamGp.transform.position = Vector3.zero;
        uiCamGp.transform.localEulerAngles = new Vector3(0, 90, 0);
        uiCamOffset.transform.localPosition = new Vector3(0, 8.15f, -17);

        uiCam.fieldOfView = 30;
        uiCam.transform.localPosition = new Vector3(0, 1.5f, -7.5f);
        uiCam.transform.localEulerAngles = new Vector3(17, 0, 0);
        uiCam.farClipPlane = 130;
        curFocus = null;
    }

	public void SwitchAIView(bool ai) 
	{
		switch (crrentMode)
		{
			case CamMode.VerticalInGame:
				if (!ai)
					uiCam.transform.localPosition = new Vector3(-30, 3, 5.8f);
				else
					uiCam.transform.localPosition = new Vector3(-25, 0.5f, 8);

				break;
			case CamMode.HorizontalInGame:
				if (!ai)
					uiCam.transform.localPosition = new Vector3(0, 1.5f, -7.5f);
				else
					uiCam.transform.localPosition = new Vector3(0, -1, -1);

			break;
		}
	}

	public void InitShoot3GameCam()
	{
		if (uiCamGp == null)
		{
			Debug.LogError("UICamGp is null, check please");
			return;
		}
		
		uiCamGp.transform.position = Vector3.zero;
		uiCamGp.transform.localEulerAngles = new Vector3(0, 90, 0);
		uiCamOffset.transform.localPosition = new Vector3(0, 8.15f, -17);

		uiCam.fieldOfView = 30;
		uiCam.transform.localPosition = new Vector3(10, 1, 10);
		uiCam.transform.localEulerAngles = new Vector3(18, -72, 0);
		curFocus = null;
	}

    public Camera GetUICamera()
    {
        return uiCam;
    }

    public void SetCamMode(CamMode cammode)
    {
        crrentMode = cammode;

        switch (crrentMode)
        {
            case CamMode.None:
                break;
            case CamMode.ReadyToGame:  
				uiCamGp.transform.position = Vector3.zero;
				uiCamGp.transform.localEulerAngles = new Vector3(0, 90, 0);
				uiCamOffset.transform.localPosition = new Vector3(0, 8.15f, -17);
				
				uiCam.fieldOfView = 30;
				uiCam.transform.localPosition = new Vector3(0, 1.5f, -7.5f);
				uiCam.transform.localEulerAngles = new Vector3(17, 0, 0);
				uiCam.farClipPlane = 130;
                break;
            case CamMode.UI:
                isOpenAutoFocus = false;
                uiCamGp.SetActive(true);
                mobaCamera.enabled = false;
                InitUICam();
//				mapColor.enabled = false;
                break;
            case CamMode.VerticalInGame:
//				mapColor.enabled = true;
                uiCamGp.SetActive(true);
                mobaCamera.enabled = false;
                InitVerticalInGame();
                break;
            case CamMode.HorizontalInGame:
//				mapColor.enabled = true;
                uiCamGp.SetActive(true);
                mobaCamera.enabled = false;
                InitHorizontalInGameCam();
				uiCam.cullingMask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("RealBall");
                break;
		case CamMode.RecordShoot3Game:
				isOpenAutoFocus = false;
//				mapColor.enabled = true;
				uiCamGp.SetActive(true);
				mobaCamera.enabled = false;
				InitShoot3GameCam();
				break;
        }
    }

    public void SetCameraColor(int color)
    {
//        if(color > 0)
//        {
//            string colorName = string.Format("Color_{0}", color);
//
//			if(!mapColor.LutTexture || mapColor.LutTexture.name != colorName)
//                mapColor.LutTexture = (Texture2D)Resources.Load(string.Format("Stadiums/Color/Color_{0}", color)) as Texture2D;
//        }
//        else
//            mapColor.LutTexture = null;
    }

    public void SetUICamPosFromSrollView(Vector3 srollViewLocalPos)
    {
        if (uiCam == null || crrentMode != CamMode.UI)
        {
            return;
        }

        Vector3 uiCamPos = uiCam.transform.localPosition;
		
		if ((14.68f + ((251538 - srollViewLocalPos.x) * 0.000007f)) <= 14.68f) 
			uiCam.transform.localPosition = new Vector3 (uiCamPos.x, uiCamPos.y, 14.68f + ((251538 - srollViewLocalPos.x) * 0.000012f));
		else 
			uiCam.transform.localPosition = new Vector3(uiCamPos.x, uiCamPos.y, 14.68f + ((251538 - srollViewLocalPos.x) * 0.000002f));
    }

    public void SetUICamTargetTransform(Transform trans)
    {
        mobaCamera.settings.lockTargetTransform = trans;
    }

    public GameObject GetTouchPlayer()
    {
        GameObject result = null;

        Ray ray = uiCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int mask = 1 << 9;
        if (Physics.Raycast(ray, out hit, 100, mask))
        if (hit.collider && hit.collider.tag == "Player")
            result = hit.collider.gameObject;

        return result;
    }

    public void SetMobaCamera(bool isStop, Vector3 localPos, Vector3 eulerAng, Transform lockTargetTransform, float lockRotationY)
    {
        if (mobaCamera == null)
            Debug.LogError("mobaCamera is null");

        mobaCamera.Stop = isStop;
        mobaCamera.transform.localPosition = localPos;
        mobaCamera.transform.eulerAngles = eulerAng;
        mobaCamera.settings.lockTargetTransform = lockTargetTransform;
        mobaCamera.LockRotationY = lockRotationY;
    }

    public void SetMobaOffset(Vector3 localPos, Vector3 eulerAngles)
    {
        uiCamOffset.transform.localPosition = localPos;
        uiCamOffset.transform.eulerAngles = eulerAngles;
    }

    public void SetSceneCamAnglesPos(Vector3 angle, Vector3 pos)
    {
        uiCam.transform.localEulerAngles = angle;
        uiCam.transform.localPosition = pos;
    }

    public void SetMobaStop(bool flag)
    {
        if (mobaCamera == null)
        {
            Debug.LogError("mobaCamera is null");
            return;
        }

        mobaCamera.Stop = flag;
    }

    public void SetFocus(FocusSensor.FocusSensorMode sensorMode, GameObject sensorObj, GameObject ball, bool isReal)
    {
        if (crrentMode == CamMode.HorizontalInGame || crrentMode == CamMode.VerticalInGame)
        {

            isRealBall = isReal;
            
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
                    curFocus = ball;
                    isOpenAutoFocus = true;
                    break;
            }
        }
    }

    public void SetGameSituation(int index)
    {
        situationIndex = index;
    }

    private Transform ballTransform;

	void FixedUpdate()
    {
        if (crrentMode == CamMode.HorizontalInGame)
        {
            if (curFocus == null)
                return;
            else
                HorizontalCameraHandle();
        } else if (crrentMode == CamMode.VerticalInGame)
        {
            if (curFocus == null)
                return;
            else
                VerticalCameraHandle(); 
        }
		if (crrentMode == CamMode.UI) {
			if(uiBuildKind == 1)
				uiCam.transform.localPosition = Vector3.Slerp(uiCam.transform.localPosition, MoveToBuildPos, 0.01f);
			else if(uiBuildKind == 2)
			{
				uiCam.transform.localPosition = Vector3.Slerp(uiCam.transform.localPosition, StartPos, 0.1f);

				if(trunTime > 0)
				{
					trunTime -= Time.deltaTime;

					if(trunTime <= 0)
					{
						trunTime = 0;
						uiBuildKind = 0;
						uiCam.transform.localPosition = StartPos;
					}
				}
			}
			else
				return;
		}


    }

    private void HorizontalCameraHandle()
    {
        if (isOpenAutoFocus)
        {
            if (situationIndex == 1)
            {
                if (isRealBall)
                {
                    if (curFocus == null)
                        return;
                    
                    if (curFocus.transform.position.y > 6)
                        FollowLookAtComputing(new Vector3(curFocus.transform.position.x, 6, curFocus.transform.position.z));
                    else
                        FollowLookAtComputing(curFocus.transform.position);
                } else
                {
                    FollowLookAtComputing(new Vector3(curFocus.transform.position.x, 0, curFocus.transform.position.z));
                }
                
            } else if (situationIndex == 2 || situationIndex == 4 || situationIndex == 6 || situationIndex == 8)
            {  
                FollowLookAtComputing(curFocus.transform.position);
            } else
            {
                if (isRealBall)
                {
                    if (curFocus.transform.position.y > 6)
                        FollowLookAtComputing(new Vector3(curFocus.transform.position.x, 6, curFocus.transform.position.z));
                    else
                        FollowLookAtComputing(curFocus.transform.position);
                } else
                    FollowLookAtComputing(new Vector3(curFocus.transform.position.x, 0, curFocus.transform.position.z));
            }
        } else
        {
            FollowLookAtComputing(curFocus.transform.position);
        }
    }

    private void VerticalCameraHandle()
    {
        FollowCoumputing(curFocus.transform.position);
    }

    private void FollowLookAtComputing(Vector3 targetPos)
    {
        Quaternion rotation = Quaternion.LookRotation(targetPos - uiCam.transform.position, Vector3.up);
		uiCam.transform.rotation = Quaternion.Slerp(uiCam.transform.rotation, rotation, Time.deltaTime * speed);
    }

    private void FollowCoumputing(Vector3 targetPos)
    {
        Vector3 formPos = uiCamGp.transform.position;
        Vector3 toPos = formPos;

        if (targetPos.z - 10 > 0)
            toPos.z = 0;
        else if (targetPos.z - 10 < -18)
            toPos.z = -18f;
        else
            toPos.z = targetPos.z - 10;

		uiCamGp.transform.position = Vector3.Slerp(formPos, toPos, 0.1f);
    }

    public override string ResName
    {
        get
        {
            return "CameraMgr";
        }
    }

	public void RotateToBasket() {
		uiCam.animation["MoveToBasket"].speed = 1.0f;
		uiCam.animation.Play();
		uiBuildKind = 1;
	}

	public void UIBuildBackToMain()
	{
		uiCam.animation["MoveToBasket"].time = uiCam.animation["MoveToBasket"].clip.length;  
		uiCam.animation["MoveToBasket"].speed = -1.0f; 
		uiCam.animation.Play();

		uiBuildKind = 2;
		trunTime = 2;
	}
}
