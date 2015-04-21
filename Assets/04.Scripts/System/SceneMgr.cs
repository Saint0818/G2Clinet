using UnityEngine;
using System.Collections;
using System;
using RootMotion.FinalIK;

public class SceneMgr : KnightSingleton<SceneMgr>
{
	private bool isPve = true;
	private int attackDirection = 0;
    private int crtStadiumIndex = -1;
    private int crtBasketIndex = -1;
    private int crtLogoIndex = -1;
    private int crtFloorIndex = -1;
    private int crtLineIndex = -1;
	private int crtSkyIndex = -1;

	//RealBall
	public GameObject RealBall;
	public InteractionObject RealBallInteractionObject;
	private SphereCollider realBallCollider;
	public Rigidbody RealBallRigidbody;
	public BallTrigger RealBallTrigger;
	public GameObject RealBallFX;

    private GameObject crtStadium;
    private GameObject crtBasket;
	private GameObject crtLine;
    private GameObject crtLogo;
    private GameObject crtFloor;
    private GameObject crtCollider;
	private GameObject[] pveBasketAy = new GameObject[2];
	private GameObject[] BuildBasket = new GameObject[2];
	private GameObject[] BuildDummyAy = new GameObject[2];
	private Vector3[] animPos = new Vector3[2];
	private Vector3[] animRotate = new Vector3[2];
	private LightmapData[] lightmapData = new LightmapData[1];

	public GameObject[] DunkPoint = new GameObject[2];
	public GameObject[] DunkJumpPoint = new GameObject[2];
	public GameObject[] Hood = new GameObject[2];
    public GameObject[] ShootPoint = new GameObject[2];
	public GameObject[] MissPoint = new GameObject[2];
	public ScoreTrigger[,] BasketEntra = new ScoreTrigger[2, 2];
	public GameObject[,] Distance3Pos = new GameObject[2,5];
	public AutoFollowGameObject BallShadow;
	public GameObject[] CameraHood = new GameObject[2];
	public Material BasketMaterial;

    void Awake()
    {
		lightmapData[0] = new LightmapData();
		Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), LayerMask.NameToLayer ("RealBall"));
		Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("Shooter"), LayerMask.NameToLayer ("RealBall"));
		Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("Shooter"), LayerMask.NameToLayer ("BasketCollider"));
		Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), LayerMask.NameToLayer ("Shooter"));
        InitLineGroup();
        CheckCollider();
		RealBall = GameObject.Instantiate (Resources.Load ("Prefab/RealBall")) as GameObject;
		RealBallFX = RealBall.transform.FindChild ("BallFX").gameObject;
		RealBallTrigger = RealBall.GetComponentInChildren<BallTrigger>();
		RealBall.name = "ReallBall";
		realBallCollider = RealBall.GetComponent<SphereCollider>();
		RealBallRigidbody = RealBall.GetComponent<Rigidbody>();
		//IK
		RealBallInteractionObject = RealBall.GetComponent<InteractionObject>();

	}
    private void InitLineGroup()
    {
		crtLine = Instantiate(Resources.Load("Prefab/Line")) as GameObject;
		crtLogo = crtLine.transform.FindChild("Logo").gameObject;
    }

    public void CheckCollider()
    {
        if (crtCollider == null)
        {
            crtCollider = Instantiate(Resources.Load("Prefab/StadiumCollider")) as GameObject;
            crtCollider.transform.parent = gameObject.transform;

            Hood[0] = GetGameObjtInCollider(string.Format("{0}/HoodA", crtCollider.name));
            Hood[1] = GetGameObjtInCollider(string.Format("{0}/HoodB", crtCollider.name)); 
			ShootPoint[0] = GetGameObjtInCollider(string.Format("{0}/HoodA/ShootPoint", crtCollider.name));
            ShootPoint[1] = GetGameObjtInCollider(string.Format("{0}/HoodB/ShootPoint", crtCollider.name));
			MissPoint[0] = GetGameObjtInCollider(string.Format("{0}/MissPos/A", crtCollider.name));
            MissPoint[1] = GetGameObjtInCollider(string.Format("{0}/MissPos/B", crtCollider.name));
            DunkPoint[0] = GetGameObjtInCollider(string.Format("{0}/DunkL/Point", crtCollider.name));
			DunkPoint[1] = GetGameObjtInCollider(string.Format("{0}/DunkR/Point", crtCollider.name));
			DunkJumpPoint[0] = GetGameObjtInCollider(string.Format("{0}/DunkL/JumpPoint", crtCollider.name));
			DunkJumpPoint[1] = GetGameObjtInCollider(string.Format("{0}/DunkR/JumpPoint", crtCollider.name));
            CameraHood[0] = GetGameObjtInCollider(string.Format("{0}/CameraHood/A", crtCollider.name));
            CameraHood[1] = GetGameObjtInCollider(string.Format("{0}/CameraHood/B", crtCollider.name));
			BasketEntra[0, 0] = GetGameObjtInCollider(string.Format("{0}/HoodA/Entra", crtCollider.name)).GetComponent<ScoreTrigger>();
			BasketEntra[0, 1] = GetGameObjtInCollider(string.Format("{0}/HoodA/Sale", crtCollider.name)).GetComponent<ScoreTrigger>();
			BasketEntra[0, 1].IntTrigger = 1;
			BasketEntra[1, 0] = GetGameObjtInCollider(string.Format("{0}/HoodB/Entra", crtCollider.name)).GetComponent<ScoreTrigger>();
			BasketEntra[1, 1] = GetGameObjtInCollider(string.Format("{0}/HoodB/Sale", crtCollider.name)).GetComponent<ScoreTrigger>();
			BasketEntra[1, 1].IntTrigger = 1;
			BallShadow = GetGameObjtInCollider(string.Format("{0}/BallShadow", crtCollider.name)).GetComponent<AutoFollowGameObject>();
			BallShadow.gameObject.SetActive(false);

//			if(TeamManager.Team.BasketLv >= 0)
//			{
//				string filename;
//
//				if(TeamManager.Team.BasketLv == 0)
//					filename = string.Format("BasketLv_{0}", 1);
//				else
//					filename = string.Format("BasketLv_{0}", TeamManager.Team.BasketLv);
//
//				for(int i = 0; i < BuildBasket.Length; i++)
//				{
//					BuildBasket[i] =  LoadBuildBasket(TeamManager.Team.BasketLv);
//					BuildBasket[i].transform.parent = crtCollider.transform;
//					if(i == 0)
//						BuildBasket[i].transform.localPosition = new Vector3(0, 0, 20.1f);
//					else
//					{
//						BuildBasket[i].transform.localPosition = new Vector3(0, 0, -20.1f);
//						BuildBasket[i].transform.localEulerAngles = new Vector3(0, 180, 0);
//					}
//					BuildBasket[i].name = filename;
//					BuildBasket[i].SetActive(false);
//				}
//			}

			for(int i = 0; i < Distance3Pos.GetLength(0); i++)
				for(int j = 0; j < Distance3Pos.GetLength(1); j++)
					Distance3Pos[i, j] = GetGameObjtInCollider(string.Format("{0}/Distance3/{1}/Distance3_{2}", crtCollider.name, i, j));
        }
    }

	private void switchGameobj(ref GameObject obj1, ref GameObject obj2) {
		GameObject obj = obj1;
		obj1 = obj2;
		obj2 = obj;
	}

	public void SwitchDirection(int direction) {
		if (attackDirection != direction) {
			attackDirection = direction;

			switchGameobj(ref Hood[0], ref Hood[1]);
			switchGameobj(ref ShootPoint[0], ref ShootPoint[1]);
			switchGameobj(ref MissPoint[0], ref MissPoint[1]);
			switchGameobj(ref DunkPoint[0], ref DunkPoint[1]);
			switchGameobj(ref CameraHood[0], ref CameraHood[1]);
		}
	} 

    private GameObject GetGameObjtInCollider(string path)
    {
        GameObject go = GameObject.Find(path);
        if (go == null)
        {
            Debug.LogError("Can not find GameObject  Path : " + path);
        }

        return go;
    }
    
    public void ChangeStadium(int stadiumIndex, int color)
    {
        if (crtStadiumIndex == stadiumIndex)
        {
            return;
        }

        if (crtStadium != null)
        {
            Destroy(crtStadium);
            crtStadium = null;
        }

        crtStadium = Instantiate(Resources.Load(string.Format("Prefab/Stadium/Stadium_{0}", stadiumIndex))) as GameObject;
        crtStadium.transform.parent = gameObject.transform;
        crtStadiumIndex = stadiumIndex;

        ChangeLightMapping(crtStadiumIndex);
//		CameraMgr.Inst.SetCameraColor(color);
    }

    public void ChangeLightMapping(int index)
    {
		lightmapData[0].lightmapFar = null;
		if (LightmapSettings.lightmaps != null && LightmapSettings.lightmaps.Length > 0 &&
		    LightmapSettings.lightmaps[0].lightmapFar)
			LightmapSettings.lightmaps[0].lightmapFar = null;

		lightmapData[0].lightmapFar = (Texture2D)Resources.Load(string.Format("Stadiums/Lightmap/Stadium_{0}", index)) as Texture2D;
		LightmapSettings.lightmaps = lightmapData;
    }

    public void ChangeFloor(int floorIndex)
    {
        if (crtFloorIndex == floorIndex)
            return;

        crtFloorIndex = floorIndex;

        if (floorIndex == -1)
        {
            if (crtFloor)
            {
                Destroy(crtFloor);
                crtFloor = null;
            }

            return;
        }

        if (crtFloor != null)
        {
            Destroy(crtFloor);
            crtFloor = null;
        }

        crtFloor = Instantiate(Resources.Load(string.Format("Prefab/Floor/Floor_{0}", floorIndex))) as GameObject;
        crtFloor.transform.parent = gameObject.transform;
    }

    public void ChangeBasket(int basketIndex)
    {
		isPve = true;
		for (int i = 0; i < pveBasketAy.Length; i++) {
			if(pveBasketAy[i])
				pveBasketAy[i].SetActive(true);
			
			if(BuildBasket[i])
				BuildBasket[i].SetActive(false);
		}

        if (crtBasketIndex == basketIndex)
        {
            return;
        }

        if (crtBasket != null)
        {
            Destroy(crtBasket);
            crtBasket = null;
        }
    
        crtBasket = Instantiate(Resources.Load(string.Format("Prefab/Basket/Basket_{0}", basketIndex))) as GameObject;
        pveBasketAy[0] = crtBasket.transform.FindChild("Left/Basket_0").gameObject;
		pveBasketAy[1] = crtBasket.transform.FindChild("Right/Basket_1").gameObject;
		animPos[0] = pveBasketAy[0].transform.localPosition;
		animPos[1] = pveBasketAy[1].transform.localPosition;
		animRotate[0] = pveBasketAy[0].transform.localEulerAngles;
		animRotate[1] = pveBasketAy[1].transform.localEulerAngles;

        crtBasket.transform.parent = gameObject.transform;
        crtBasketIndex = basketIndex;
    }

	public void SetBasketBallState(PlayerState state, Transform dummy = null){
		switch(state){
			case PlayerState.BasketActionSwish:
				realBallCollider.enabled = false;
				RealBallTrigger.SetBoxColliderEnable(true);
			break;
			case PlayerState.BasketActionSwishEnd:
				realBallCollider.enabled = true;
			break;
			case PlayerState.BasketAnimationStart:
				realBallCollider.enabled = false;
				RealBallRigidbody.useGravity = false;
				RealBallRigidbody.isKinematic = true;
				RealBallTrigger.SetBoxColliderEnable(false);
				RealBall.transform.parent = dummy;
				RealBall.transform.localScale = Vector3.one;
				RealBall.transform.localPosition = Vector3.zero;
				RealBall.transform.eulerAngles = dummy.eulerAngles;
				break;
			case PlayerState.BasketActionEnd:
				realBallCollider.enabled = true;
				RealBallRigidbody.useGravity = true;
				RealBallRigidbody.isKinematic = false;
				RealBallTrigger.SetBoxColliderEnable(true);
				RealBall.transform.parent = null;
				RealBall.transform.localScale = Vector3.one;
				RealBall.transform.eulerAngles = dummy.eulerAngles;	
				RealBallRigidbody.AddRelativeForce(new Vector3(1,0,0)*  70,ForceMode.Impulse);
				break;
		}

	}

	public void SetBallState(PlayerState state, PlayerBehaviour player = null)
	{
		switch(state)
		{
			case PlayerState.Dribble:
				realBallCollider.enabled = false;
				RealBall.transform.parent = player.DummyBall.transform;
				RealBallRigidbody.useGravity = false;
				RealBallRigidbody.isKinematic = true;
				RealBall.transform.localEulerAngles = Vector3.zero;
				RealBall.transform.localScale = Vector3.one;
				RealBallTrigger.SetBoxColliderEnable(false);
				RealBallFX.gameObject.SetActive(false);
				RealBall.transform.localPosition = Vector3.zero;
				break;

			case PlayerState.Shooting: 
				realBallCollider.enabled = true;
				RealBall.transform.parent = null;
				RealBallRigidbody.isKinematic = false;
				RealBallRigidbody.useGravity = true;
				RealBallTrigger.SetBoxColliderEnable(true);
				RealBall.transform.localScale = Vector3.one;
				break;

			case PlayerState.PassFlat: 
			case PlayerState.PassFloor: 
			case PlayerState.PassParabola: 
			case PlayerState.Tee: 
				realBallCollider.enabled = true;
				RealBall.transform.parent = null;
				RealBallRigidbody.isKinematic = true;
				RealBallRigidbody.useGravity = false;
				RealBallTrigger.SetBoxColliderEnable(true);
				RealBall.transform.localScale = Vector3.one;
				break;

			case PlayerState.Steal:
				realBallCollider.enabled = true;
				RealBall.transform.parent = null;
				RealBallRigidbody.isKinematic = false;
				RealBallRigidbody.useGravity = true;
				RealBallTrigger.SetBoxColliderEnable(true);
				RealBall.transform.localScale = Vector3.one;
				
				//				Vector3 v = GameFunction.CalculateNextPosition(RealBall.transform.position, RealBallRigidbody.velocity, 0.5f);
				Vector3 v = RealBall.transform.forward * -1;
				RealBallRigidbody.velocity = GameFunction.GetVelocity(RealBall.transform.position, v, 60);
				RealBallFX.gameObject.SetActive(true);
			break;
			
		case PlayerState.Block: 
				GameController.Get.Shooter = null;
				GameController.Get.Passer = null;
				realBallCollider.enabled = true;
				RealBall.transform.parent = null;
				RealBallRigidbody.isKinematic = false;
				RealBallRigidbody.useGravity = true;
				RealBallTrigger.SetBoxColliderEnable(true);
				RealBall.transform.localScale = Vector3.one;
				v = RealBall.transform.forward * -1;
				RealBallRigidbody.velocity = GameFunction.GetVelocity(RealBall.transform.position, v, 60);
				RealBallFX.gameObject.SetActive(true);
				break;

			case PlayerState.Dunk:
				realBallCollider.enabled = true;
				RealBallFX.gameObject.SetActive(false);
				RealBall.transform.localScale = Vector3.one;
				break;

			case PlayerState.DunkBasket:
				realBallCollider.enabled = true;
				RealBallRigidbody.useGravity = true;
				RealBallRigidbody.isKinematic = false;
				RealBallTrigger.SetBoxColliderEnable(true);
				RealBall.transform.parent = null;
				RealBall.transform.localScale = Vector3.one;

				if(player)
					RealBall.transform.position = player.DummyBall.transform.position;

				RealBallRigidbody.AddForce(Vector3.down * 2000);
				break;

			case PlayerState.Reset:
				SceneMgr.Get.RealBall.transform.parent = null;
				SceneMgr.Get.RealBall.transform.localPosition = new Vector3(0, 5, 0);
				SceneMgr.Get.RealBallRigidbody.isKinematic = true;
				SceneMgr.Get.RealBallRigidbody.useGravity = false;
				SceneMgr.Get.RealBallTrigger.SetBoxColliderEnable(true);
				SceneMgr.Get.RealBallFX.gameObject.SetActive(true);
			break;

			case PlayerState.Start:
				SceneMgr.Get.RealBall.transform.localPosition = new Vector3 (0, 5, 0);
				SceneMgr.Get.RealBall.GetComponent<Rigidbody>().isKinematic = false;
				SceneMgr.Get.RealBall.GetComponent<Rigidbody>().useGravity = true;
				break;

			case PlayerState.HoldBall:
			case PlayerState.PickBall:
				realBallCollider.enabled = false;
				RealBall.transform.parent = player.DummyBall.transform;
				RealBallRigidbody.useGravity = false;
				RealBallRigidbody.isKinematic = true;
				RealBall.transform.localEulerAngles = Vector3.zero;
				RealBall.transform.localScale = Vector3.one;
				RealBallTrigger.SetBoxColliderEnable(false);
				RealBallFX.gameObject.SetActive(false);
				RealBall.transform.localPosition = Vector3.zero;
				break;
				
		}
	}

	public void SetRealBallPosition(Vector3 pos) {
		RealBall.transform.position = pos;
	}

	public void SetRealBallOffset(Vector3 pos)
	{
		RealBall.transform.Translate (pos);
	}

	public void ResetBasketEntra() {
		for (int i = 0; i < 2; i ++)
			for (int j = 0; j < 2; j ++)
				BasketEntra[i, j].Into = false;
	}

    public void PlayDunk(int team)
    {
        Animation animation;
		string animationName;
		AnimationClip clip;
		animationName = "Dunk_0";

        if (team == 0)
        {
			animation = pveBasketAy[0].GetComponent<Animation>();
			Hood[0].gameObject.SetActive(true);
        } else
        {
			if(isPve)
			{
				animation = pveBasketAy[1].GetComponent<Animation>();
			}
			else
			{
				animation = BuildBasket[1].GetComponent<Animation>();
			}
           
			Hood[1].gameObject.SetActive(true);
		}

		animation.Play (animationName);
    }

	public void PlayShoot(int team)
	{
		Animation animation;
		string animationName;
		AnimationClip clip;
		animationName = "Shot_0";
		
		if (team == 0)
		{
			animation = pveBasketAy[0].GetComponent<Animation>();
			Hood[0].gameObject.SetActive(true);
		} else
		{
			if(isPve)
			{
				animation = pveBasketAy[1].GetComponent<Animation>();
			}
			else
			{
				animation = BuildBasket[1].GetComponent<Animation>();
			}
			
			Hood[1].gameObject.SetActive(true);
		}
		
		animation.Play (animationName);
	}

    public void PlayBasketEffect(int teamIndex, int index)
    {
        Animation animation;
        string animationName = string.Empty;

        if (teamIndex == 1)
        {
			if(isPve)
			{
				animation = pveBasketAy[0].GetComponent<Animation>();
				animationName = string.Format("BasketL{0}", index);
			}
            else
			{
				animation = BuildBasket[0].GetComponent<Animation>();
				if(index == 161)
					animationName = "Basket162";
			}
        
			Hood[0].gameObject.SetActive(false);
        } else
        {
			if(isPve)
			{
				animation = pveBasketAy[1].GetComponent<Animation>();
				animationName = string.Format("BasketR{0}", index);
			}
			else
			{
				animation = BuildBasket[1].GetComponent<Animation>();
				if(index == 161)
					animationName = "Basket162";
			}
           
			Hood[1].gameObject.SetActive(false);
        }

		StartCoroutine("Reset");
		animation.wrapMode = WrapMode.Once;
        animation.Play(animationName);
    }

	IEnumerator Reset()
	{  
		yield return new WaitForSeconds(3f);

		if (isPve) {
			for (int i = 0; i < 2; i ++) {
				if (pveBasketAy [i]) {
					pveBasketAy [i].transform.localEulerAngles = animRotate [i];
					pveBasketAy [i].transform.localPosition = animPos [i];
				}
			}
		} else {
			for (int i = 0; i < 2; i ++) {
				if (BuildDummyAy[i]) {
					BuildDummyAy[i].transform.localPosition = Vector3.zero;
					BuildDummyAy[i].transform.localEulerAngles = new Vector3(-90, 0, 0);
				}
			}
		}

		Hood[0].gameObject.SetActive(true);
		Hood[1].gameObject.SetActive(true);
	}  

    public void ChangeLine(int lineIndex)
    {
        if (crtLineIndex == lineIndex)
        {
            return;
        }

		if (!crtLine)
            InitLineGroup();

        Texture tex = Resources.Load(string.Format("Textures/Court/Line{0}", lineIndex)) as Texture;
		crtLine.GetComponent<Renderer>().material.mainTexture = tex;
    }

    public void ChangeLogo(int logoIndex)
    {
        if (crtLogoIndex == logoIndex)
        {
            return;
        }

        if (!crtLogo)
        {
            InitLineGroup();
        }

        Texture tex = Resources.Load(string.Format("Textures/Logo/Logo{0}", logoIndex)) as Texture;
        crtLogo.GetComponent<Renderer>().material.mainTexture = tex;
    }

	public GameObject GetGameObjectInColliderGp(string name)
    {
        GameObject result = null;
        result = crtCollider.transform.FindChild(name).gameObject;
        return result;
    }

    public void ChangeLevel(int stageNo)
    {
		int no = 1;
		int color = 0;
		int floorIndex = 0;
		int basketIndex = 0;

//		if (TeamManager.DStages.ContainsKey(stageNo)){
//			no = TeamManager.DStages [stageNo].Stadium;
//			color = TeamManager.DStages [stageNo].ColorKind;
//			floorIndex = TeamManager.DStages [stageNo].Floor;
//			basketIndex = TeamManager.DStages [stageNo].Basket;
//		}

		//TODO: Temp-------
		floorIndex = 0;
		basketIndex = 0;
		no = 3;
		CameraMgr.Get.SetTeamCamera(0);
		RealBall.transform.localPosition = new Vector3 (0, 5, 0);

		RealBall.GetComponent<Rigidbody>().isKinematic = true;
		RealBall.GetComponent<Rigidbody>().useGravity = false;

		//------------------

//		ChangeStadium(no, color);
        ChangeFloor(floorIndex);
        ChangeBasket(basketIndex);
        CheckCollider();

		DateTime dt = DateTime.Now;
		if(dt.Hour >= 6 && dt.Hour <= 18)
			ChangeSky(0);
		else
			ChangeSky(1);
    }

	public void ChangeSpecialScene(int Stadium, int ColorKind, int Floor, int Basket, int SkyIndex){
		ChangeStadium(Stadium, ColorKind);
		ChangeFloor(Floor);
		ChangeBasket(Basket);
		CheckCollider();
		ChangeSky (SkyIndex);
	}

	public void ChangeSky(int SkyIndex){
		if (crtSkyIndex != SkyIndex) {
			crtSkyIndex = SkyIndex;
//			Material mat = TextureManager.GetSkyMaterial (SkyIndex);
//			if(mat != null)
//				RenderSettings.skybox = mat;
		}
	}

	public void SetBuildBasket (int leftBasktet, int rightBasket)
	{
		isPve = false;
		for(int i = 0; i < BuildBasket.Length; i++)
		{
			if(pveBasketAy[i] != null){
				pveBasketAy[i].gameObject.SetActive (false);

				if(BuildBasket[i])
					BuildBasket[i].SetActive(true);
				
				string name; 
				int lv;
		
				if(i == 0)
				{
					lv = leftBasktet;
					pveBasketAy[0] = BuildBasket[0].gameObject;
					
				}
				else
				{
					lv = rightBasket;
					pveBasketAy[1] = BuildBasket[1].gameObject;
				}
				
				name = string.Format("BasketLv_{0}", lv);

				if(BuildBasket[i].name != name)
				{
					Destroy(BuildBasket[i]);
					BuildBasket[i] = null;
					
					BuildBasket[i] = LoadBuildBasket(lv);
					BuildBasket[i].name = name;
					BuildBasket[i].transform.parent = crtCollider.transform;
					
					if(i == 0)
						BuildBasket[i].transform.localPosition = new Vector3(0, 0, 20.1f);
					else
					{
						BuildBasket[i].transform.localPosition = new Vector3(0, 0, -20.1f);
						BuildBasket[i].transform.localEulerAngles = new Vector3(0, 180, 0);
					}
				}

				BuildDummyAy[i] = BuildBasket[i].transform.FindChild("Dummy01").gameObject;
			}
		}

		crtBasketIndex = -1;
	}

	public void BasketLvUp(int lv)
	{
		if(pveBasketAy[1])
			pveBasketAy[1].gameObject.SetActive (false);

		if(BuildBasket[1])
			BuildBasket[1].gameObject.SetActive (false);

		string filename = string.Format ("BasketLv_{0}", lv);

		if (BuildBasket [1].name != filename){
			Destroy(BuildBasket [1]);
			BuildBasket [1] = null;
			BuildBasket [1] = LoadBuildBasket(lv);
			BuildBasket [1].name = filename;
			BuildBasket [1].transform.localPosition =  new Vector3(0, 0, -20.1f);
			BuildBasket [1].transform.localEulerAngles =  new Vector3(0, 180, 0);
		}

//		EffectManager.Get.PlayEffect("BasketUpgrate", new Vector3(0, 0, -20.1f));
	}

	public GameObject LoadBuildBasket(int lv)
	{
		string path;
		GameObject go;
		UnityEngine.Object obj;

		path = string.Format("Prefab/PlayerBasket/BasketLv_{0}", lv);
		obj = Resources.Load(path);

		if(obj == null)
		{
//			UIHint.Get.ShowHint("Basketlv is out of range or perfab not found, lv : " + lv , Color.red);
			obj = Resources.Load("Prefab/PlayerBasket/BasketLv_0");
		}
				
		go = Instantiate(obj) as GameObject;
		return go;
	}
}
