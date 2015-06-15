using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CourtMgr : KnightSingleton<CourtMgr>
{
	private bool isPve = true;
	private int attackDirection = 0;
	private int crtBasketIndex = -1;
	private GameObject crtBasket;

	//RealBall
	public GameObject RealBall;
	private SphereCollider realBallCollider;
	public Rigidbody RealBallRigidbody;
	public BallTrigger RealBallTrigger;
	public GameObject RealBallFX;
	public EPlayerState RealBallState;

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
	public GameObject[] Walls = new GameObject[2];
	public ScoreTrigger[,] BasketEntra = new ScoreTrigger[2, 2];
	public GameObject[,] Distance3Pos = new GameObject[2,5];
	public Animator[] BasketHoopAnimator = new Animator[2];
	public Transform[] BasketHoop = new Transform[2];
	public Transform[] BasketHoopDummy = new Transform[2];
	public GameObject Effect;

	public AutoFollowGameObject BallShadow;
	public GameObject[] CameraHood = new GameObject[2];
	public Material BasketMaterial;
	public BallCurve RealBallCurve;
	public UILabel[] Scoreboards = new UILabel[2];

	public Dictionary<string, Vector3> BasketShootPosition = new Dictionary<string, Vector3>();
	public Dictionary<int, List<string>> BasketAnimationName = new Dictionary<int, List<string>>(); 
	public Dictionary<int, List<string>> BasketAnimationNoneState = new Dictionary<int, List<string>>();

	private void InitBasket(RuntimeAnimatorController controller){
		AnimationClip[] clip = controller.animationClips;
		List<string> scoreName = new List<string>();
		List<string> noScoreName = new List<string>();
		if(clip.Length > 0) {
			for (int i=0; i<clip.Length; i++) {
				if(clip[i].name.Contains("BasketballAction_")){
					string[] nameSplit = clip[i].name.Split("_"[0]);
					int num = int.Parse(nameSplit[1]);
					if(num < 100) 
						scoreName.Add(clip[i].name);
					else
						noScoreName.Add(clip[i].name);
					
				}
			}
		}
		
		//Get Basket Every Range Animation
		//Score
		List<string> BasketScoreAnimationStateRightWing = new List<string>();
		List<string> BasketScoreAnimationStateRight = new List<string>();
		List<string> BasketScoreAnimationStateCenter = new List<string>();
		List<string> BasketScoreAnimationStateLeft = new List<string>();
		List<string> BasketScoreAnimationStateLeftWing = new List<string>();
		if(scoreName.Count > 0){
			for(int i=0; i<scoreName.Count; i++) {
				string[] nameSplit = scoreName[i].Split("_"[0]);
				//RightWing
				for (int j=0; j<GameConst.AngleScoreRightWing.Length; j++){
					if(GameConst.AngleScoreRightWing[j].Equals(nameSplit[1]))
						BasketScoreAnimationStateRightWing.Add(scoreName[i]);
				}
				//Right
				for (int j=0; j<GameConst.AngleScoreRight.Length; j++){
					if(GameConst.AngleScoreRight[j].Equals(nameSplit[1]))
						BasketScoreAnimationStateRight.Add(scoreName[i]);
				}
				//Center
				for (int j=0; j<GameConst.AngleScoreCenter.Length; j++){
					if(GameConst.AngleScoreCenter[j].Equals(nameSplit[1]))
						BasketScoreAnimationStateCenter.Add(scoreName[i]);
				}
				//Left
				for (int j=0; j<GameConst.AngleScoreLeft.Length; j++){
					if(GameConst.AngleScoreLeft[j].Equals(nameSplit[1]))
						BasketScoreAnimationStateLeft.Add(scoreName[i]);
				}
				//LeftWing
				for (int j=0; j<GameConst.AngleScoreLeftWing.Length; j++){
					if(GameConst.AngleScoreLeftWing[j].Equals(nameSplit[1]))
						BasketScoreAnimationStateLeftWing.Add(scoreName[i]);
				}
			}
			BasketAnimationName.Add((int)EBasketDistanceAngle.ShortRightWing, arrayIntersection(GameConst.DistanceScoreShort, BasketScoreAnimationStateRightWing));
			BasketAnimationName.Add((int)EBasketDistanceAngle.MediumRightWing, arrayIntersection(GameConst.DistanceScoreMedium, BasketScoreAnimationStateRightWing));
			BasketAnimationName.Add((int)EBasketDistanceAngle.LongRightWing, arrayIntersection(GameConst.DistanceScoreLong, BasketScoreAnimationStateRightWing));
			BasketAnimationName.Add((int)EBasketDistanceAngle.ShortRight, arrayIntersection(GameConst.DistanceScoreShort, BasketScoreAnimationStateRight));
			BasketAnimationName.Add((int)EBasketDistanceAngle.MediumRight, arrayIntersection(GameConst.DistanceScoreMedium, BasketScoreAnimationStateRight));
			BasketAnimationName.Add((int)EBasketDistanceAngle.LongRight, arrayIntersection(GameConst.DistanceScoreLong, BasketScoreAnimationStateRight));
			BasketAnimationName.Add((int)EBasketDistanceAngle.ShortCenter, arrayIntersection(GameConst.DistanceScoreShort, BasketScoreAnimationStateCenter));
			BasketAnimationName.Add((int)EBasketDistanceAngle.MediumCenter, arrayIntersection(GameConst.DistanceScoreMedium, BasketScoreAnimationStateCenter));
			BasketAnimationName.Add((int)EBasketDistanceAngle.LongCenter, arrayIntersection(GameConst.DistanceScoreLong, BasketScoreAnimationStateCenter));
			BasketAnimationName.Add((int)EBasketDistanceAngle.ShortLeft, arrayIntersection(GameConst.DistanceScoreShort, BasketScoreAnimationStateLeft));
			BasketAnimationName.Add((int)EBasketDistanceAngle.MediumLeft, arrayIntersection(GameConst.DistanceScoreMedium, BasketScoreAnimationStateLeft));
			BasketAnimationName.Add((int)EBasketDistanceAngle.LongLeft, arrayIntersection(GameConst.DistanceScoreLong, BasketScoreAnimationStateLeft));
			BasketAnimationName.Add((int)EBasketDistanceAngle.ShortLeftWing, arrayIntersection(GameConst.DistanceScoreShort, BasketScoreAnimationStateLeftWing));
			BasketAnimationName.Add((int)EBasketDistanceAngle.MediumLeftWing, arrayIntersection(GameConst.DistanceScoreMedium, BasketScoreAnimationStateLeftWing));
			BasketAnimationName.Add((int)EBasketDistanceAngle.LongLeftWing, arrayIntersection(GameConst.DistanceScoreLong, BasketScoreAnimationStateLeftWing));
		}
		//No Score
		List<string> BasketNoScoreAnimationStateRightWing = new List<string>();
		List<string> BasketNoScoreAnimationStateRight = new List<string>();
		List<string> BasketNoScoreAnimationStateCenter = new List<string>();
		List<string> BasketNoScoreAnimationStateLeft = new List<string>();
		List<string> BasketNoScoreAnimationStateLeftWing = new List<string>();
		if(noScoreName.Count > 0) {
			for(int i=0; i<noScoreName.Count; i++) {
				string[] nameSplit = noScoreName[i].Split("_"[0]);
				//RightWing
				for (int j=0; j<GameConst.AngleNoScoreRightWing.Length; j++){
					if(GameConst.AngleNoScoreRightWing[j].Equals(nameSplit[1]))
						BasketNoScoreAnimationStateRightWing.Add(noScoreName[i]);
				}
				//Right
				for (int j=0; j<GameConst.AngleNoScoreRight.Length; j++){
					if(GameConst.AngleNoScoreRight[j].Equals(nameSplit[1]))
						BasketNoScoreAnimationStateRight.Add(noScoreName[i]);
				}
				//Center
				for (int j=0; j<GameConst.AngleNoScoreCenter.Length; j++){
					if(GameConst.AngleNoScoreCenter[j].Equals(nameSplit[1]))
						BasketNoScoreAnimationStateCenter.Add(noScoreName[i]);
				}
				//Left
				for (int j=0; j<GameConst.AngleNoScoreLeft.Length; j++){
					if(GameConst.AngleNoScoreLeft[j].Equals(nameSplit[1]))
						BasketNoScoreAnimationStateLeft.Add(noScoreName[i]);
				}
				//LeftWing
				for (int j=0; j<GameConst.AngleNoScoreLeftWing.Length; j++){
					if(GameConst.AngleNoScoreLeftWing[j].Equals(nameSplit[1]))
						BasketNoScoreAnimationStateLeftWing.Add(noScoreName[i]);
				}
			}
			BasketAnimationNoneState.Add((int)EBasketDistanceAngle.ShortRightWing, arrayIntersection(GameConst.DistanceNoScoreShort, BasketNoScoreAnimationStateRightWing));
			BasketAnimationNoneState.Add((int)EBasketDistanceAngle.MediumRightWing, arrayIntersection(GameConst.DistanceNoScoreMedium, BasketNoScoreAnimationStateRightWing));
			BasketAnimationNoneState.Add((int)EBasketDistanceAngle.LongRightWing, arrayIntersection(GameConst.DistanceNoScoreLong, BasketNoScoreAnimationStateRightWing));
			BasketAnimationNoneState.Add((int)EBasketDistanceAngle.ShortRight, arrayIntersection(GameConst.DistanceNoScoreShort, BasketNoScoreAnimationStateRight));
			BasketAnimationNoneState.Add((int)EBasketDistanceAngle.MediumRight, arrayIntersection(GameConst.DistanceNoScoreMedium, BasketNoScoreAnimationStateRight));
			BasketAnimationNoneState.Add((int)EBasketDistanceAngle.LongRight, arrayIntersection(GameConst.DistanceNoScoreLong, BasketNoScoreAnimationStateRight));
			BasketAnimationNoneState.Add((int)EBasketDistanceAngle.ShortCenter, arrayIntersection(GameConst.DistanceNoScoreShort, BasketNoScoreAnimationStateCenter));
			BasketAnimationNoneState.Add((int)EBasketDistanceAngle.MediumCenter, arrayIntersection(GameConst.DistanceNoScoreMedium, BasketNoScoreAnimationStateCenter));
			BasketAnimationNoneState.Add((int)EBasketDistanceAngle.LongCenter, arrayIntersection(GameConst.DistanceNoScoreLong, BasketNoScoreAnimationStateCenter));
			BasketAnimationNoneState.Add((int)EBasketDistanceAngle.ShortLeft, arrayIntersection(GameConst.DistanceNoScoreShort, BasketNoScoreAnimationStateLeft));
			BasketAnimationNoneState.Add((int)EBasketDistanceAngle.MediumLeft, arrayIntersection(GameConst.DistanceNoScoreMedium, BasketNoScoreAnimationStateLeft));
			BasketAnimationNoneState.Add((int)EBasketDistanceAngle.LongLeft, arrayIntersection(GameConst.DistanceNoScoreLong, BasketNoScoreAnimationStateLeft));
			BasketAnimationNoneState.Add((int)EBasketDistanceAngle.ShortLeftWing, arrayIntersection(GameConst.DistanceNoScoreShort, BasketNoScoreAnimationStateLeftWing));
			BasketAnimationNoneState.Add((int)EBasketDistanceAngle.MediumLeftWing, arrayIntersection(GameConst.DistanceNoScoreMedium, BasketNoScoreAnimationStateLeftWing));
			BasketAnimationNoneState.Add((int)EBasketDistanceAngle.LongLeftWing, arrayIntersection(GameConst.DistanceNoScoreLong, BasketNoScoreAnimationStateLeftWing));
		}
		
		
		//Get Basket Animation InitPosition
		if(GameData.BasketShootPosition != null && GameData.BasketShootPosition.Length > 0) {
			for(int i=0; i<GameData.BasketShootPosition.Length; i++) {
				Vector3 position = new Vector3(GameData.BasketShootPosition[i].ShootPositionX, GameData.BasketShootPosition[i].ShootPositionY, GameData.BasketShootPosition[i].ShootPositionZ);
				BasketShootPosition.Add(GameData.BasketShootPosition[i].AnimationName, position);
			}
		}
	}
	
	private List<string> arrayIntersection(string[] list1, List<string> list2) {
		List<string> list = new List<string>();
		for (int i=0; i<list1.Length; i++) {
			string nameSplit = "BasketballAction_"+list1[i];
			if(list2.Contains(nameSplit)) {
				list.Add(nameSplit);
			}
		}
		return list;
	}

	private void InitScoreboard()
	{
		Scoreboards [1] = GameObject.Find ("Scoreboard/Left").GetComponent<UILabel>();

		if (Scoreboards [1])
			Scoreboards [1].text = "0";

		Scoreboards [0] = GameObject.Find ("Scoreboard/Right").GetComponent<UILabel>();

		if(Scoreboards [0])
			Scoreboards [0].text = "0";

		EffectEnable(GameData.Setting.Effect);
	}

	public void EffectEnable(bool enable)
	{
		if (Effect == null)
			Effect = GameObject.Find ("Effect");

		if (Effect)
			Effect.SetActive (enable);
	}

    void Awake()
    {
		lightmapData[0] = new LightmapData();
		Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), LayerMask.NameToLayer ("RealBall"));
		Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("Shooter"), LayerMask.NameToLayer ("RealBall"));
		Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("Shooter"), LayerMask.NameToLayer ("BasketCollider"));
		Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), LayerMask.NameToLayer ("Shooter"));
		Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("Shooter"), LayerMask.NameToLayer ("Shooter"));
        
        CheckCollider();
	}

	public void InitCourtScene()
	{
		CloneReallBall();
		CheckCollider();
		ChangeBasket(2);
		CameraMgr.Get.SetTeamCamera(0);
		CameraMgr.Get.SetCourtCamera (SceneName.Court_0);
		InitScoreboard ();
		UIGame.UIShow (true);
	}

	public void CloneReallBall()
	{
		if (RealBall == null) {
			RealBall = GameObject.Instantiate (Resources.Load ("Prefab/Stadium/RealBall")) as GameObject;
			RealBallFX = RealBall.transform.FindChild ("BallFX").gameObject;
			RealBallTrigger = RealBall.GetComponentInChildren<BallTrigger> ();
			RealBall.name = "ReallBall";
			realBallCollider = RealBall.GetComponent<SphereCollider> ();
			RealBallRigidbody = RealBall.GetComponent<Rigidbody> ();

			if(RealBallCurve == null || RealBallCurve.gameObject == null){
				GameObject obj = GameObject.Instantiate (Resources.Load ("Prefab/Stadium/BallCurve")) as GameObject;
				RealBallCurve = obj.GetComponent<BallCurve> ();
			}
		}

		if (RealBall) {
			RealBall.transform.localPosition = new Vector3 (0, 7, 0);
			RealBall.GetComponent<Rigidbody> ().isKinematic = true;
			RealBall.GetComponent<Rigidbody> ().useGravity = false;
		}
	}

    public void CheckCollider()
    {
        if (crtCollider == null)
        {
			crtCollider = Instantiate(Resources.Load("Prefab/Stadium/StadiumCollider")) as GameObject;
            crtCollider.transform.parent = gameObject.transform;

			Walls[0] = GetGameObjtInCollider(string.Format("{0}/Wall/Wall/WallA", crtCollider.name));
			Walls[1] = GetGameObjtInCollider(string.Format("{0}/Wall/Wall/WallB", crtCollider.name)); 
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
		GameObject go = GameObject.Find (path);
		if (go == null) {
				Debug.LogError ("Can not find GameObject  Path : " + path);
		}

		return go;
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
    
		crtBasket = Instantiate(Resources.Load(string.Format("Prefab/Stadium/Basket/Basket_{0}", basketIndex))) as GameObject;
		pveBasketAy[0] = crtBasket.transform.FindChild("Left/Basket").gameObject;
		pveBasketAy[1] = crtBasket.transform.FindChild("Right/Basket").gameObject;
		animPos[0] = pveBasketAy[0].transform.localPosition;
		animPos[1] = pveBasketAy[1].transform.localPosition;
		animRotate[0] = pveBasketAy[0].transform.localEulerAngles;
		animRotate[1] = pveBasketAy[1].transform.localEulerAngles;
		
		crtBasket.transform.parent = gameObject.transform;
		crtBasketIndex = basketIndex;
		
		BasketHoop[0] = crtBasket.transform.FindChild("Left/BasketballAction");
		BasketHoop[1] = crtBasket.transform.FindChild("Right/BasketballAction");
		
		BasketHoopAnimator[0] = BasketHoop[0].gameObject.GetComponent<Animator>();
		BasketHoopAnimator[1] = BasketHoop[1].gameObject.GetComponent<Animator>();
		
		BasketHoopDummy[0] = BasketHoop[0].FindChild("DummyHoop");
		BasketHoopDummy[1] = BasketHoop[1].FindChild("DummyHoop");
		
		InitBasket(BasketHoopAnimator[0].runtimeAnimatorController);
	}

	public void RealBallPath(int team, string animationName) {
		if(!GameController.Get.IsReset){
			switch(animationName) {
			case "ActionEnd":
				SetBasketBallState(EPlayerState.BasketActionEnd, BasketHoopDummy[team]);
				break;
			case "ActionNoScoreEnd":
				SetBasketBallState(EPlayerState.BasketActionNoScoreEnd, BasketHoopDummy[team]);
				SetBallState(EPlayerState.Rebound);
				break;
			case "BasketNetPlay":
				PlayShoot(team);
				RealBallRigidbody.velocity = Vector3.zero;
				break;
			}
		}
	}
	
	public void SetBasketBallState(EPlayerState state, Transform dummy = null){
		if(!GameController.Get.IsReset){
			switch(state){
			case EPlayerState.BasketActionSwish:
				Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("BasketCollider"), LayerMask.NameToLayer ("RealBall"), true);
				RealBallTrigger.SetBoxColliderEnable(true);
				break;
			case EPlayerState.BasketActionSwishEnd:
				Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("BasketCollider"), LayerMask.NameToLayer ("RealBall"), false);
				RealBallRigidbody.velocity = Vector3.zero;
				RealBallRigidbody.AddForce(Vector3.down * 70);
				break;
			case EPlayerState.BasketAnimationStart:
				RealBallRigidbody.useGravity = false;
				RealBallRigidbody.isKinematic = true;
				RealBallTrigger.SetBoxColliderEnable(false);
				RealBall.transform.parent = dummy;
				RealBall.transform.localScale = Vector3.one;
				RealBall.transform.localPosition = Vector3.zero;
				RealBall.transform.eulerAngles = dummy.eulerAngles;
				break;
			case EPlayerState.BasketActionEnd:
				RealBallRigidbody.useGravity = true;
				RealBallRigidbody.isKinematic = false;
				RealBallTrigger.SetBoxColliderEnable(true);
				RealBall.transform.parent = null;
				RealBall.transform.localScale = Vector3.one;
				RealBall.transform.eulerAngles = dummy.eulerAngles;	
				RealBallRigidbody.AddForce(Vector3.down * 50);
				GameController.Get.Passer = null;
				break;
			case EPlayerState.BasketActionNoScoreEnd:
				RealBallRigidbody.useGravity = true;
				RealBallRigidbody.isKinematic = false;
				RealBallTrigger.SetBoxColliderEnable(true);
				RealBall.transform.parent = null;
				RealBall.transform.localScale = Vector3.one;
				RealBall.transform.eulerAngles = dummy.eulerAngles;	
				RealBallRigidbody.AddRelativeForce(new Vector3(1,0,0)* 70,ForceMode.Impulse);
				GameController.Get.Passer = null;
				break;
			}
		}
	}

	public void SetBallState(EPlayerState state, PlayerBehaviour player = null)
	{
		RealBallState = state;
		switch(state)
		{
			case EPlayerState.Dribble0:
				realBallCollider.enabled = false;
				if (player)
					RealBall.transform.parent = player.DummyBall.transform;

				RealBallRigidbody.useGravity = false;
				RealBallRigidbody.isKinematic = true;
				RealBall.transform.localEulerAngles = Vector3.zero;
				RealBall.transform.localScale = Vector3.one;
				RealBallTrigger.SetBoxColliderEnable(false);
				RealBallFX.gameObject.SetActive(false);
				RealBall.transform.localPosition = Vector3.zero;
				break;

			case EPlayerState.Shoot0: 
			case EPlayerState.Shoot1: 
			case EPlayerState.Shoot2: 
			case EPlayerState.Shoot3: 
			case EPlayerState.Shoot4: 
			case EPlayerState.Shoot5: 
			case EPlayerState.Shoot6: 
			case EPlayerState.Shoot7: 
			case EPlayerState.Layup0: 
			case EPlayerState.Layup1: 
			case EPlayerState.Layup2: 
			case EPlayerState.Layup3: 
			case EPlayerState.TipIn: 
				realBallCollider.enabled = true;
				RealBall.transform.parent = null;
				RealBallRigidbody.isKinematic = false;
				RealBallRigidbody.useGravity = true;
				RealBallTrigger.SetBoxColliderEnable(true);
				RealBall.transform.localScale = Vector3.one;
				break;

			case EPlayerState.Pass0: 
			case EPlayerState.Pass2: 
			case EPlayerState.Pass1: 
			case EPlayerState.Pass3: 
			case EPlayerState.Pass4: 
			case EPlayerState.Pass5: 
			case EPlayerState.Pass6: 
			case EPlayerState.Pass7: 
			case EPlayerState.Pass8: 
			case EPlayerState.Pass9: 
				realBallCollider.enabled = true;
				RealBall.transform.parent = null;
				RealBallRigidbody.isKinematic = true;
				RealBallRigidbody.useGravity = false;
				RealBallTrigger.SetBoxColliderEnable(true);
				RealBall.transform.localScale = Vector3.one;
				break;

			case EPlayerState.Steal:
				realBallCollider.enabled = true;
				RealBall.transform.parent = null;
				RealBallRigidbody.isKinematic = false;
				RealBallRigidbody.useGravity = true;
				RealBallTrigger.SetBoxColliderEnable(true);
				RealBall.transform.localScale = Vector3.one;
				GameController.Get.Passer = null;
				
				//				Vector3 v = GameFunction.CalculateNextPosition(RealBall.transform.position, RealBallRigidbody.velocity, 0.5f);
				Vector3 v = RealBall.transform.forward * -1;
				if(player != null)					
					v = player.transform.forward * 10;
				RealBallRigidbody.velocity = v;//GameFunction.GetVelocity(RealBall.transform.position, v, 60);
				RealBallFX.gameObject.SetActive(true);
			break;
			case EPlayerState.JumpBall:
				if(!GameController.Get.IsJumpBall)
				{
					GameController.Get.IsJumpBall = true;
					realBallCollider.enabled = true;
					RealBall.transform.parent = null;
					RealBallRigidbody.isKinematic = false;
					RealBallRigidbody.useGravity = true;
					RealBallTrigger.SetBoxColliderEnable(true);
					RealBall.transform.localScale = Vector3.one;
					GameController.Get.Passer = null;

					if(player != null)	
						v = player.transform.forward * -5;
					else
						v = RealBall.transform.forward * -1;
					
					RealBallRigidbody.velocity = v;
					RealBallFX.gameObject.SetActive(true);
				}
				break;
			
			case EPlayerState.Block: 
				GameController.Get.Shooter = null;
				GameController.Get.Passer = null;
				realBallCollider.enabled = true;
				RealBall.transform.parent = null;
				RealBallRigidbody.isKinematic = false;
				RealBallRigidbody.useGravity = true;
				RealBallTrigger.SetBoxColliderEnable(true);
				RealBall.transform.localScale = Vector3.one;
				v = RealBall.transform.forward * -1;
				if(player != null)					
					v = player.transform.forward * 10;
				RealBallRigidbody.velocity = v;//GameFunction.GetVelocity(RealBall.transform.position, v, 60);
				RealBallFX.gameObject.SetActive(true);
				break;

			case EPlayerState.Dunk0:
				realBallCollider.enabled = true;
				RealBallFX.gameObject.SetActive(false);
				RealBall.transform.localScale = Vector3.one;
				break;

			case EPlayerState.DunkBasket:
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

			case EPlayerState.Reset:
				RealBall.transform.parent = null;
				RealBall.transform.position = new Vector3(0, 7, 0);
				RealBallRigidbody.isKinematic = true;
				RealBallRigidbody.useGravity = false;
				RealBallRigidbody.velocity = Vector3.zero;
				RealBallTrigger.SetBoxColliderEnable(true);
				RealBallFX.gameObject.SetActive(true);
				break;

			case EPlayerState.Start:
				RealBall.transform.localPosition = new Vector3 (0, 6, 0);
				RealBallRigidbody.isKinematic = false;
				RealBallRigidbody.useGravity = true;
				RealBallRigidbody.AddForce(Vector3.up * 3500);
				break;

			case EPlayerState.HoldBall:
			case EPlayerState.PickBall0:
				realBallCollider.enabled = false;
				if (player)
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

	public void SetScoreboards(int team, int score)
	{
		if (Scoreboards [team] == null) {
			InitScoreboard();		
		}

		if(Scoreboards [team])
			Scoreboards [team].text = score.ToString();
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

    public void PlayDunk(int team, int stageNo)
    {
        Animation animation;
		string animationName;
//		AnimationClip clip;
		animationName = string.Format("Dunk_{0}", stageNo);

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

		if(animation[animationName])
			animation.Play (animationName);

		AudioMgr.Get.PlaySound (SoundType.Dunk);
    }

	public void PlayShoot(int team)
	{
		Animation animation;
		string animationName;
//		AnimationClip clip;
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

	public GameObject GetGameObjectInColliderGp(string name)
    {
        GameObject result = null;
        result = crtCollider.transform.FindChild(name).gameObject;
        return result;
    }
}

