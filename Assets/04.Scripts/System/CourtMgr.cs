using System.Collections;
using System.Collections.Generic;
using AI;
using Chronos;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using GameEnum;

public class CourtMgr : KnightSingleton<CourtMgr>
{
    public GameObject RefGameObject;
    private bool isPve = true;
    private int attackDirection = 0;
    private int crtBasketIndex = -1;
    private GameObject crtBasket;

    //RealBall
    private bool isBallOffensive = false;
//Shoot and Dunk (true)
    public GameObject RealBallObj;
    private GameObject mRealBallSFX;
    public EPlayerState RealBallState;
    private bool isRealBallInAcitve = false;
    private readonly CountDownTimer mRealBallSFXTimer = new CountDownTimer(1);
    // 特效顯示的時間. 單位: 秒.

    private GameObject crtCollider;
    private GameObject[] pveBasketAy = new GameObject[2];
    private GameObject[] BuildBasket = new GameObject[2];
    private GameObject[] BuildDummyAy = new GameObject[2];
    private Vector3[] animPos = new Vector3[2];
    private Vector3[] animRotate = new Vector3[2];
    private GameObject spotlight;

    public GameObject[] DunkPoint = new GameObject[2];
    public GameObject[] DunkJumpPoint = new GameObject[2];
    public GameObject[] Hood = new GameObject[2];

    /// <summary>
    /// index 0: 玩家進攻的籃框; index 1: 對手進攻的籃框.(這對應到 ETeamKind 的數值)
    /// </summary>
    public GameObject[] ShootPoint = new GameObject[2];
    public GameObject[] EffectPoint = new GameObject[2];
    public GameObject[] MissPoint = new GameObject[2];
    public GameObject[] Walls = new GameObject[2];
    public ScoreTrigger[,] BasketEntra = new ScoreTrigger[2, 2];
    public AirBallTrigger[] BasketAirBall = new AirBallTrigger[2];
    public GameObject[,] Distance3Pos = new GameObject[2, 5];
    public Animator[] BasketHoopAnimator = new Animator[2];
    public Transform[] BasketHoop = new Transform[2];
    public Transform[] BasketHoopDummy = new Transform[2];
    public GameObject[] BasketRangeCenter = new GameObject[2];
    public GameObject EffectHigh;
    public GameObject EffectMedium;
	private BasketAnimation[] basketAnimation = new BasketAnimation[2];
	private BasketAnimation[] basketActionAnimation = new BasketAnimation[2];
	public string BasketAnimationName = "BasketballAction_1";
		
    public CircularSectorMeshRenderer SkillRangeOfAction;
    public GameObject SkillArrowOfAction;
    private UITexture textureArrow;
    public Transform[] EndPlayerPosition = new Transform[6];

    public AutoFollowGameObject BallShadow;
    public GameObject[] CameraHood = new GameObject[2];
    public Material BasketMaterial;
    public BallCurve RealBallCurve;
    //	public UILabel[] Scoreboards = new UILabel[2];

    public Dictionary<string, Vector3> DBasketShootWorldPosition = new Dictionary<string, Vector3>();
    public Dictionary<int, List<string>> DBasketAnimationName = new Dictionary<int, List<string>>();
    public Dictionary<int, List<string>> DBasketAnimationNoneState = new Dictionary<int, List<string>>();

	private int scoreTeam = 0;
	private bool isSwishIn = false;
    public RealBall RealBallCompoment;

    public void ChangeBasketByLobby(GameObject obj)
    {
        pveBasketAy[0] = obj;
    }

    public void InitBasket(RuntimeAnimatorController controller)
    {
        AnimationClip[] clip = controller.animationClips;
        List<string> scoreName = new List<string>();
        List<string> noScoreName = new List<string>();
        if (clip.Length > 0)
        {
            for (int i = 0; i < clip.Length; i++)
            {
                if (clip[i].name.Contains("BasketballAction_"))
                {
                    string[] nameSplit = clip[i].name.Split("_"[0]);
                    int num = int.Parse(nameSplit[1]);
                    if (num < 100)
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
        if (scoreName.Count > 0)
        {
            for (int i = 0; i < scoreName.Count; i++)
            {
                string[] nameSplit = scoreName[i].Split("_"[0]);
                //RightWing
                for (int j = 0; j < GameConst.AngleScoreRightWing.Length; j++)
                {
                    if (GameConst.AngleScoreRightWing[j].Equals(nameSplit[1]))
                        BasketScoreAnimationStateRightWing.Add(scoreName[i]);
                }
                //Right
                for (int j = 0; j < GameConst.AngleScoreRight.Length; j++)
                {
                    if (GameConst.AngleScoreRight[j].Equals(nameSplit[1]))
                        BasketScoreAnimationStateRight.Add(scoreName[i]);
                }
                //Center
                for (int j = 0; j < GameConst.AngleScoreCenter.Length; j++)
                {
                    if (GameConst.AngleScoreCenter[j].Equals(nameSplit[1]))
                        BasketScoreAnimationStateCenter.Add(scoreName[i]);
                }
                //Left
                for (int j = 0; j < GameConst.AngleScoreLeft.Length; j++)
                {
                    if (GameConst.AngleScoreLeft[j].Equals(nameSplit[1]))
                        BasketScoreAnimationStateLeft.Add(scoreName[i]);
                }
                //LeftWing
                for (int j = 0; j < GameConst.AngleScoreLeftWing.Length; j++)
                {
                    if (GameConst.AngleScoreLeftWing[j].Equals(nameSplit[1]))
                        BasketScoreAnimationStateLeftWing.Add(scoreName[i]);
                }
            }

            DBasketAnimationName.Clear();
            DBasketAnimationName.Add((int)EBasketDistanceAngle.ShortRightWing, arrayIntersection(GameConst.DistanceScoreShort, BasketScoreAnimationStateRightWing));
            DBasketAnimationName.Add((int)EBasketDistanceAngle.MediumRightWing, arrayIntersection(GameConst.DistanceScoreMedium, BasketScoreAnimationStateRightWing));
            DBasketAnimationName.Add((int)EBasketDistanceAngle.LongRightWing, arrayIntersection(GameConst.DistanceScoreLong, BasketScoreAnimationStateRightWing));
            DBasketAnimationName.Add((int)EBasketDistanceAngle.ShortRight, arrayIntersection(GameConst.DistanceScoreShort, BasketScoreAnimationStateRight));
            DBasketAnimationName.Add((int)EBasketDistanceAngle.MediumRight, arrayIntersection(GameConst.DistanceScoreMedium, BasketScoreAnimationStateRight));
            DBasketAnimationName.Add((int)EBasketDistanceAngle.LongRight, arrayIntersection(GameConst.DistanceScoreLong, BasketScoreAnimationStateRight));
            DBasketAnimationName.Add((int)EBasketDistanceAngle.ShortCenter, arrayIntersection(GameConst.DistanceScoreShort, BasketScoreAnimationStateCenter));
            DBasketAnimationName.Add((int)EBasketDistanceAngle.MediumCenter, arrayIntersection(GameConst.DistanceScoreMedium, BasketScoreAnimationStateCenter));
            DBasketAnimationName.Add((int)EBasketDistanceAngle.LongCenter, arrayIntersection(GameConst.DistanceScoreLong, BasketScoreAnimationStateCenter));
            DBasketAnimationName.Add((int)EBasketDistanceAngle.ShortLeft, arrayIntersection(GameConst.DistanceScoreShort, BasketScoreAnimationStateLeft));
            DBasketAnimationName.Add((int)EBasketDistanceAngle.MediumLeft, arrayIntersection(GameConst.DistanceScoreMedium, BasketScoreAnimationStateLeft));
            DBasketAnimationName.Add((int)EBasketDistanceAngle.LongLeft, arrayIntersection(GameConst.DistanceScoreLong, BasketScoreAnimationStateLeft));
            DBasketAnimationName.Add((int)EBasketDistanceAngle.ShortLeftWing, arrayIntersection(GameConst.DistanceScoreShort, BasketScoreAnimationStateLeftWing));
            DBasketAnimationName.Add((int)EBasketDistanceAngle.MediumLeftWing, arrayIntersection(GameConst.DistanceScoreMedium, BasketScoreAnimationStateLeftWing));
            DBasketAnimationName.Add((int)EBasketDistanceAngle.LongLeftWing, arrayIntersection(GameConst.DistanceScoreLong, BasketScoreAnimationStateLeftWing));
        }
        //No Score
        List<string> BasketNoScoreAnimationStateRightWing = new List<string>();
        List<string> BasketNoScoreAnimationStateRight = new List<string>();
        List<string> BasketNoScoreAnimationStateCenter = new List<string>();
        List<string> BasketNoScoreAnimationStateLeft = new List<string>();
        List<string> BasketNoScoreAnimationStateLeftWing = new List<string>();
        if (noScoreName.Count > 0)
        {
            for (int i = 0; i < noScoreName.Count; i++)
            {
                string[] nameSplit = noScoreName[i].Split("_"[0]);
                //RightWing
                for (int j = 0; j < GameConst.AngleNoScoreRightWing.Length; j++)
                {
                    if (GameConst.AngleNoScoreRightWing[j].Equals(nameSplit[1]))
                        BasketNoScoreAnimationStateRightWing.Add(noScoreName[i]);
                }
                //Right
                for (int j = 0; j < GameConst.AngleNoScoreRight.Length; j++)
                {
                    if (GameConst.AngleNoScoreRight[j].Equals(nameSplit[1]))
                        BasketNoScoreAnimationStateRight.Add(noScoreName[i]);
                }
                //Center
                for (int j = 0; j < GameConst.AngleNoScoreCenter.Length; j++)
                {
                    if (GameConst.AngleNoScoreCenter[j].Equals(nameSplit[1]))
                        BasketNoScoreAnimationStateCenter.Add(noScoreName[i]);
                }
                //Left
                for (int j = 0; j < GameConst.AngleNoScoreLeft.Length; j++)
                {
                    if (GameConst.AngleNoScoreLeft[j].Equals(nameSplit[1]))
                        BasketNoScoreAnimationStateLeft.Add(noScoreName[i]);
                }
                //LeftWing
                for (int j = 0; j < GameConst.AngleNoScoreLeftWing.Length; j++)
                {
                    if (GameConst.AngleNoScoreLeftWing[j].Equals(nameSplit[1]))
                        BasketNoScoreAnimationStateLeftWing.Add(noScoreName[i]);
                }
            }

            DBasketAnimationNoneState.Clear();
            DBasketAnimationNoneState.Add((int)EBasketDistanceAngle.ShortRightWing, arrayIntersection(GameConst.DistanceNoScoreShort, BasketNoScoreAnimationStateRightWing));
            DBasketAnimationNoneState.Add((int)EBasketDistanceAngle.MediumRightWing, arrayIntersection(GameConst.DistanceNoScoreMedium, BasketNoScoreAnimationStateRightWing));
            DBasketAnimationNoneState.Add((int)EBasketDistanceAngle.LongRightWing, arrayIntersection(GameConst.DistanceNoScoreLong, BasketNoScoreAnimationStateRightWing));
            DBasketAnimationNoneState.Add((int)EBasketDistanceAngle.ShortRight, arrayIntersection(GameConst.DistanceNoScoreShort, BasketNoScoreAnimationStateRight));
            DBasketAnimationNoneState.Add((int)EBasketDistanceAngle.MediumRight, arrayIntersection(GameConst.DistanceNoScoreMedium, BasketNoScoreAnimationStateRight));
            DBasketAnimationNoneState.Add((int)EBasketDistanceAngle.LongRight, arrayIntersection(GameConst.DistanceNoScoreLong, BasketNoScoreAnimationStateRight));
            DBasketAnimationNoneState.Add((int)EBasketDistanceAngle.ShortCenter, arrayIntersection(GameConst.DistanceNoScoreShort, BasketNoScoreAnimationStateCenter));
            DBasketAnimationNoneState.Add((int)EBasketDistanceAngle.MediumCenter, arrayIntersection(GameConst.DistanceNoScoreMedium, BasketNoScoreAnimationStateCenter));
            DBasketAnimationNoneState.Add((int)EBasketDistanceAngle.LongCenter, arrayIntersection(GameConst.DistanceNoScoreLong, BasketNoScoreAnimationStateCenter));
            DBasketAnimationNoneState.Add((int)EBasketDistanceAngle.ShortLeft, arrayIntersection(GameConst.DistanceNoScoreShort, BasketNoScoreAnimationStateLeft));
            DBasketAnimationNoneState.Add((int)EBasketDistanceAngle.MediumLeft, arrayIntersection(GameConst.DistanceNoScoreMedium, BasketNoScoreAnimationStateLeft));
            DBasketAnimationNoneState.Add((int)EBasketDistanceAngle.LongLeft, arrayIntersection(GameConst.DistanceNoScoreLong, BasketNoScoreAnimationStateLeft));
            DBasketAnimationNoneState.Add((int)EBasketDistanceAngle.ShortLeftWing, arrayIntersection(GameConst.DistanceNoScoreShort, BasketNoScoreAnimationStateLeftWing));
            DBasketAnimationNoneState.Add((int)EBasketDistanceAngle.MediumLeftWing, arrayIntersection(GameConst.DistanceNoScoreMedium, BasketNoScoreAnimationStateLeftWing));
            DBasketAnimationNoneState.Add((int)EBasketDistanceAngle.LongLeftWing, arrayIntersection(GameConst.DistanceNoScoreLong, BasketNoScoreAnimationStateLeftWing));
        }

        //Get Basket Animation InitPosition
        DBasketShootWorldPosition.Clear();
        if (GameData.BasketShootPosition != null && GameData.BasketShootPosition.Length > 0)
        {
            for (int i = 0; i < GameData.BasketShootPosition.Length; i++)
            {
                Vector3 position = new Vector3(GameData.BasketShootPosition[i].ShootPositionX, GameData.BasketShootPosition[i].ShootPositionY, GameData.BasketShootPosition[i].ShootPositionZ);

                BasketHoopDummy[0].localPosition = position;
                DBasketShootWorldPosition.Add("0_" + GameData.BasketShootPosition[i].AnimationName, BasketHoopDummy[0].position);
                BasketHoopDummy[1].localPosition = position;

                if (GameStart.Get.CourtMode == ECourtMode.Full)
                    DBasketShootWorldPosition.Add("1_" + GameData.BasketShootPosition[i].AnimationName, BasketHoopDummy[1].position);
                else
                    DBasketShootWorldPosition.Add("1_" + GameData.BasketShootPosition[i].AnimationName, BasketHoopDummy[0].position);
            }
            BasketHoopDummy[0].localPosition = Vector3.zero;
            BasketHoopDummy[1].localPosition = Vector3.zero;
        }

    }

    private List<string> arrayIntersection(string[] list1, List<string> list2)
    {
        List<string> list = new List<string>();
        for (int i = 0; i < list1.Length; i++)
        {
            string nameSplit = "BasketballAction_" + list1[i];
            if (list2.Contains(nameSplit))
            {
                list.Add(nameSplit);
            }
        }
        return list;
    }

    public void InitScoreboard(bool isEnable = false)
    {
        InitBallShadow();
        EffectEnable((QualityType)GameData.Setting.Quality);
    }

    public void EffectEnable(QualityType type)
    {
        if (EffectHigh)
            EffectHigh.SetActive(type == QualityType.High);
		
        if (EffectMedium)
            EffectMedium.SetActive(type >= QualityType.Medium);
    }

    [UsedImplicitly]
    void Awake()
    {
        RefGameObject = gameObject;
        mRealBallSFXTimer.TimeUpListener += HideBallSFX;

        CheckCollider();
    }

    [UsedImplicitly]
    private void FixedUpdate()
    {
        mRealBallSFXTimer.Update(Time.deltaTime);
    }

    public void InitEffect()
    {
        EffectMedium = GameObject.Find("Effect/Medium");
        EffectHigh = GameObject.Find("Effect/High");		
    }

    public void InitCourtScene()
    {
        CloneReallBall();
        CheckCollider();
        //ChangeBasket(2);
        InitScoreboard();
        InitEffect();

        if (!SkillRangeOfAction)
            SkillRangeOfAction = Instantiate(Resources.Load("Effect/RangeOfAction") as GameObject).GetComponent<CircularSectorMeshRenderer>();

        if (!SkillArrowOfAction)
        {
            SkillArrowOfAction = Instantiate(Resources.Load("Effect/SkillArea_Arrow") as GameObject);
            SkillArrowOfAction.SetActive(false);
            Transform t = SkillArrowOfAction.transform.FindChild("Scale/SpriteSkillAreaArrow");
            if (t != null)
                textureArrow = t.GetComponent<UITexture>();
        }

        CameraMgr.Get.SetCameraSituation(ECameraSituation.Loading);
    }

    public void ShowRangeOfAction(bool isShow, Transform parent = null, float degree = 0, float dis = 0, float euler = 0)
    {
        SkillRangeOfAction.transform.parent = parent;
        //LayerMgr.Get.SetLayer(SkillRangeOfAction.gameObject, ELayer.UIPlayerInfo);
        if (parent)
            SkillRangeOfAction.transform.localPosition = new Vector3(0, 0.1f, 0);
        SkillRangeOfAction.transform.localEulerAngles = new Vector3(0, euler, 0);
        SkillRangeOfAction.ChangeValue(degree, dis);
        SkillRangeOfAction.RefGameObject.SetActive(isShow);
    }

    public void ShowArrowOfAction(bool isShow, Transform parent = null, float dis = 0)
    {
        SkillArrowOfAction.transform.parent = parent;
        if (parent)
            SkillArrowOfAction.transform.localPosition = new Vector3(-1, 0.1f, 0);
        SkillArrowOfAction.transform.localEulerAngles = Vector3.zero;
        if (textureArrow)
            textureArrow.SetRect(0, 0, 200, dis * 100);
        SkillArrowOfAction.SetActive(isShow);
    }

    public void RangeOfActionPosition(Vector3 position)
    {
        SkillRangeOfAction.transform.position = new Vector3(position.x, 0.1f, position.z);
    }

    public void RangeOfActionEuler(float euler)
    {
        SkillRangeOfAction.transform.localEulerAngles = new Vector3(0, euler, 0);
    }

    public void ShowEnd(bool isImmediately = false)
    {
        CameraMgr.Get.SetCameraSituation(ECameraSituation.JumpBall);

        if (isImmediately)
            CameraMgr.Get.ShowCameraEnable(false);
    }

//    void OnGUI()
//    {
//        if (Input.GetKeyDown(KeyCode.B))
//            CameraMgr.Get.PlayGameStartCamera();
//    }

	private void InitBasketDelegate () {
		for (int i = 0; i < basketAnimation.Length; i++) {
		    basketAnimation[i] = pveBasketAy[i].GetComponent<BasketAnimation>();
			if ( basketAnimation[i] != null) {
				basketAnimation[i].AnimationEventDel += AnimationEvent;
				basketAnimation[i].PlayEffectDel += PlayEffect;
				basketAnimation[i].PlayShakeDel += PlayShake;
				basketAnimation[i].PlayActionSoundDel += PlayActionSound;
			}
		}

		for (int i = 0; i < basketActionAnimation.Length; i++) {
			basketActionAnimation[i] = BasketHoop[i].GetComponent<BasketAnimation>();
			if ( basketActionAnimation[i] != null) {
				basketActionAnimation[i].AnimationEventDel += AnimationEvent;
				basketActionAnimation[i].PlayEffectDel += PlayEffect;
				basketActionAnimation[i].PlayShakeDel += PlayShake;
				basketActionAnimation[i].PlayActionSoundDel += PlayActionSound;
			}
		}
	}

	public void AnimationEvent(int Team, AnimationEvent aniEvent) {
		string animationName = aniEvent.stringParameter;
		int index = aniEvent.intParameter;
		RealBallPath(Team, animationName, index);
	}

	public void PlayEffect(int Team, AnimationEvent aniEvent) {
		float duration = aniEvent.floatParameter;
		int eventKind = aniEvent.intParameter;
		string effectName = aniEvent.stringParameter;
		PlayBasketEffect(Team, effectName, eventKind, duration);
	}

	public void PlayShake (int Team, AnimationEvent aniEvent) {
		CameraMgr.Get.PlayShake ();
	}

	public void PlayActionSound (int Team, AnimationEvent aniEvent) {
		AudioMgr.Get.PlaySound(aniEvent.stringParameter);
	}

	public void AirBallMgr (int Team) {
		BasketAirBall[Team].Into = true;
		GameController.Get.ShowShootSate(false, Team);
	}

	public void ScoreMgr (int Team, int IntTrigger) {
		if(IntTrigger == 0 && !BasketEntra[Team, 0].Into) {
			if (GameController.Visible) {
				if(!GameController.Get.IsDunk && !GameController.Get.IsAlleyoop && !GameController.Get.IsPassing &&
					GameController.Get.BasketSituation != EBasketSituation.AirBall) {
					BasketEntra[Team, 0].Into = true;
					RealBallTrigger.IsAutoRotate = false;
					RealBallDoMoveFinish();
					switch (GameController.Get.BasketSituation) {
					case EBasketSituation.Swish:
						if(GameStart.Get.IsDebugAnimation && GameController.Visible){
							Debug.LogWarning("RealBall Swish IN:"+ Time.time);
							GameController.Get.shootSwishTimes++;
						}
						isSwishIn = true;
						SetBasketState(EPlayerState.BasketActionSwish, Team);
						break;
					case EBasketSituation.Score:
					case EBasketSituation.NoScore:
						if(GameStart.Get.IsDebugAnimation && GameController.Visible) {
							Debug.LogWarning("RealBall IN:"+ BasketAnimationName);
							string[] nameSplit = BasketAnimationName.Split("_"[0]);
							if(int.Parse(nameSplit[1]) < 100)
								GameController.Get.shootTimes ++ ;
						}

						SetBasketState(EPlayerState.BasketAnimationStart, Team);
						if(BasketHoopAnimator[Team] != null ){
							if(BasketAnimationName != string.Empty)
								BasketHoopAnimator[Team].SetTrigger(BasketAnimationName);
						}
						break;
					default:
						SetBasketState(EPlayerState.BasketActionSwish, Team);
						break;
					}
				}
			}
		} else  if (IntTrigger == 1){
			if(BasketEntra[Team, 0].Into && !BasketEntra[Team, 1].Into) {
				BasketEntra[Team, 1].Into = true;
				if(GameController.Visible && GameController.Get.BasketSituation == EBasketSituation.Swish){
					PlayShoot(Team, 0);
					SetBasketState(EPlayerState.BasketActionSwishEnd, Team);
				}
			}
		}
	}

    public void CloneReallBall()
    {
        if (RealBallObj == null)
        {
            RealBallObj = GameObject.Instantiate(Resources.Load("Prefab/Stadium/RealBall")) as GameObject;
            RealBallCompoment = RealBallObj.GetComponent<RealBall>();
            mRealBallSFX = RealBallObj.transform.FindChild("BallFX").gameObject;
            RealBallObj.name = "RealBall";
            if (RealBallCurve == null)
                RealBallCurve = RealBallObj.GetComponent<BallCurve>();
			
//			if(RealBallCurve == null || RealBallCurve.gameObject == null) {
//				GameObject obj = GameObject.Instantiate (Resources.Load ("Prefab/Stadium/BallCurve")) as GameObject;
//				RealBallCurve = obj.GetComponent<BallCurve> ();
//			}
        }

        if (RealBallObj)
        {
            RealBallObj.transform.localPosition = new Vector3(0, 7, 0);
            RealBallObj.GetComponent<Rigidbody>().isKinematic = true;
            RealBallObj.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    public void CheckCollider()
    {
        if (crtCollider == null)
        {
            crtCollider = Instantiate(Resources.Load("Prefab/Stadium/StadiumCollider")) as GameObject;
            crtCollider.transform.parent = RefGameObject.transform;
            BallShadow = GetGameObjtInCollider(string.Format("{0}/BallShadow", crtCollider.name)).GetComponent<AutoFollowGameObject>();

            if (spotlight == null && BallShadow)
                spotlight = BallShadow.transform.FindChild("SpotLight").gameObject; 
        }
		
        EndPlayerPosition[0] = GetGameObjtInCollider(string.Format("{0}/GameFinishPos/Win/1", crtCollider.name)).transform;
        EndPlayerPosition[1] = GetGameObjtInCollider(string.Format("{0}/GameFinishPos/Win/2", crtCollider.name)).transform;
        EndPlayerPosition[2] = GetGameObjtInCollider(string.Format("{0}/GameFinishPos/Win/3", crtCollider.name)).transform;
        EndPlayerPosition[3] = GetGameObjtInCollider(string.Format("{0}/GameFinishPos/Lose/4", crtCollider.name)).transform;
        EndPlayerPosition[4] = GetGameObjtInCollider(string.Format("{0}/GameFinishPos/Lose/5", crtCollider.name)).transform;
        EndPlayerPosition[5] = GetGameObjtInCollider(string.Format("{0}/GameFinishPos/Lose/6", crtCollider.name)).transform;

        if (GameStart.Get.CourtMode == ECourtMode.Full)
        {
            Walls[0] = GetGameObjtInCollider(string.Format("{0}/Wall/Wall/WallA", crtCollider.name));
            Walls[1] = GetGameObjtInCollider(string.Format("{0}/Wall/Wall/WallB", crtCollider.name)); 
            Hood[0] = GetGameObjtInCollider(string.Format("{0}/HoodA", crtCollider.name));
            Hood[1] = GetGameObjtInCollider(string.Format("{0}/HoodB", crtCollider.name)); 
            ShootPoint[0] = GetGameObjtInCollider(string.Format("{0}/HoodA/ShootPoint", crtCollider.name));
            ShootPoint[1] = GetGameObjtInCollider(string.Format("{0}/HoodB/ShootPoint", crtCollider.name));
            BasketRangeCenter[0] = GetGameObjtInCollider(string.Format("{0}/RangeOfActionCenterL", crtCollider.name));
            BasketRangeCenter[1] = GetGameObjtInCollider(string.Format("{0}/RangeOfActionCenterR", crtCollider.name));
            MissPoint[0] = GetGameObjtInCollider(string.Format("{0}/MissPos/A", crtCollider.name));
            MissPoint[1] = GetGameObjtInCollider(string.Format("{0}/MissPos/B", crtCollider.name));
            DunkPoint[0] = GetGameObjtInCollider(string.Format("{0}/DunkL/Point", crtCollider.name));
            DunkPoint[1] = GetGameObjtInCollider(string.Format("{0}/DunkR/Point", crtCollider.name));
            DunkJumpPoint[0] = GetGameObjtInCollider(string.Format("{0}/DunkL/JumpPoint", crtCollider.name));
            DunkJumpPoint[1] = GetGameObjtInCollider(string.Format("{0}/DunkR/JumpPoint", crtCollider.name));
            CameraHood[0] = GetGameObjtInCollider(string.Format("{0}/CameraHood/A", crtCollider.name));
            CameraHood[1] = GetGameObjtInCollider(string.Format("{0}/CameraHood/B", crtCollider.name));
            BasketEntra[0, 0] = GetGameObjtInCollider(string.Format("{0}/HoodA/Entra", crtCollider.name)).GetComponent<ScoreTrigger>();
			BasketEntra[0, 0].ScoreDel += ScoreMgr;
			BasketEntra[0, 1] = GetGameObjtInCollider(string.Format("{0}/HoodA/Sale", crtCollider.name)).GetComponent<ScoreTrigger>();
			BasketEntra[0, 1].ScoreDel += ScoreMgr;
            BasketEntra[0, 1].IntTrigger = 1;
			BasketEntra[1, 0] = GetGameObjtInCollider(string.Format("{0}/HoodB/Entra", crtCollider.name)).GetComponent<ScoreTrigger>();
			BasketEntra[1, 0].ScoreDel += ScoreMgr;
			BasketEntra[1, 1] = GetGameObjtInCollider(string.Format("{0}/HoodB/Sale", crtCollider.name)).GetComponent<ScoreTrigger>();
			BasketEntra[1, 1].ScoreDel += ScoreMgr;
            BasketEntra[1, 1].IntTrigger = 1;
            BasketAirBall[0] = GetGameObjtInCollider(string.Format("{0}/HoodA/AirBall", crtCollider.name)).GetComponent<AirBallTrigger>();
			BasketAirBall[0].AirBallDel += AirBallMgr;
			BasketAirBall[1] = GetGameObjtInCollider(string.Format("{0}/HoodB/AirBall", crtCollider.name)).GetComponent<AirBallTrigger>();
			BasketAirBall[1].AirBallDel += AirBallMgr;

            for (int i = 0; i < Distance3Pos.GetLength(0); i++)
                for (int j = 0; j < Distance3Pos.GetLength(1); j++)
                    Distance3Pos[i, j] = GetGameObjtInCollider(string.Format("{0}/Distance3/{1}/Distance3_{2}", crtCollider.name, i, j));
        }
        else
        {
            Walls[0] = GetGameObjtInCollider(string.Format("{0}/Wall/Wall/WallA", crtCollider.name));
            Walls[1] = Walls[0];
            Hood[0] = GetGameObjtInCollider(string.Format("{0}/HoodA", crtCollider.name));
            Hood[1] = Hood[0];
            ShootPoint[0] = GetGameObjtInCollider(string.Format("{0}/HoodA/ShootPoint", crtCollider.name));
            ShootPoint[1] = ShootPoint[0];
            MissPoint[0] = GetGameObjtInCollider(string.Format("{0}/MissPos/A", crtCollider.name));
            MissPoint[1] = MissPoint[0];
            DunkPoint[0] = GetGameObjtInCollider(string.Format("{0}/DunkL/Point", crtCollider.name));
            DunkPoint[1] = DunkPoint[0];
            DunkJumpPoint[0] = GetGameObjtInCollider(string.Format("{0}/DunkL/JumpPoint", crtCollider.name));
            DunkJumpPoint[1] = DunkJumpPoint[0];
            CameraHood[0] = GetGameObjtInCollider(string.Format("{0}/CameraHood/A", crtCollider.name));
            CameraHood[1] = CameraHood[0];
			BasketEntra[0, 0] = GetGameObjtInCollider(string.Format("{0}/HoodA/Entra", crtCollider.name)).GetComponent<ScoreTrigger>();
			BasketEntra[0, 0].ScoreDel += ScoreMgr;
			BasketEntra[0, 1] = GetGameObjtInCollider(string.Format("{0}/HoodA/Sale", crtCollider.name)).GetComponent<ScoreTrigger>();
			BasketEntra[0, 1].ScoreDel += ScoreMgr;
            BasketEntra[0, 1].IntTrigger = 1;
            BasketEntra[1, 0] = BasketEntra[0, 0];
            BasketEntra[1, 1] = BasketEntra[0, 1];
			BasketAirBall[0] = GetGameObjtInCollider(string.Format("{0}/HoodA/AirBall", crtCollider.name)).GetComponent<AirBallTrigger>();
			BasketAirBall[0].AirBallDel += AirBallMgr;
            BasketAirBall[1] = BasketAirBall[0];

            for (int i = 0; i < Distance3Pos.GetLength(0); i++)
                for (int j = 0; j < Distance3Pos.GetLength(1); j++)
                    Distance3Pos[i, j] = GetGameObjtInCollider(string.Format("{0}/Distance3/{1}/Distance3_{2}", crtCollider.name, 0, j));
        }
    }

    public void InitBallShadow()
    {
        BallShadow.RefGameObject.SetActive(true);
        BallShadow.SetTarget(RealBallObj);
        spotlight.SetActive(false);
    }

    private void switchGameobj(ref GameObject obj1, ref GameObject obj2)
    {
        GameObject obj = obj1;
        obj1 = obj2;
        obj2 = obj;
    }

    public void SwitchDirection(int direction)
    {
        if (attackDirection != direction)
        {
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

    public void ChangeBasket(int basketIndex)
    {
        isPve = true;
        for (int i = 0; i < pveBasketAy.Length; i++)
        {
            if (pveBasketAy[i])
                pveBasketAy[i].SetActive(true);
			
            if (BuildBasket[i])
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
		for (int i = 0; i < pveBasketAy.Length; i++)
		{
            if (!pveBasketAy[i].GetComponent<Timeline>())
            {
                Timeline timer = pveBasketAy[i].AddComponent<Timeline>();
                timer.mode = TimelineMode.Global;
                timer.globalClockKey = ETimerKind.Default.ToString();
                //timer.recordTransform = false;
            }
        }
		
        animPos[0] = pveBasketAy[0].transform.localPosition;
        animPos[1] = pveBasketAy[1].transform.localPosition;
        animRotate[0] = pveBasketAy[0].transform.localEulerAngles;
        animRotate[1] = pveBasketAy[1].transform.localEulerAngles;
		
        crtBasket.transform.parent = RefGameObject.transform;
        crtBasketIndex = basketIndex;
		
        BasketHoop[0] = crtBasket.transform.FindChild("Left/BasketballAction");
        BasketHoop[1] = crtBasket.transform.FindChild("Right/BasketballAction");
        BasketHoopDummy[0] = BasketHoop[0].FindChild("DummyHoop");
		BasketHoopDummy[1] = BasketHoop[1].FindChild("DummyHoop");
		InitBasketDelegate ();

        Transform obj = crtBasket.transform.FindChild("Left/Basket/DummyBasketRoot/Bone01/Bone02/Bone03/Bone04/EffectPoint");
        if (obj)
            EffectPoint[0] = obj.gameObject;

        obj = crtBasket.transform.FindChild("Right/Basket/DummyBasketRoot/Bone01/Bone02/Bone03/Bone04/EffectPoint");
        if (obj)
            EffectPoint[1] = obj.gameObject;
		
        BasketHoopAnimator[0] = BasketHoop[0].gameObject.GetComponent<Animator>();
        BasketHoopAnimator[1] = BasketHoop[1].gameObject.GetComponent<Animator>();

        InitBasket(BasketHoopAnimator[0].runtimeAnimatorController);
    }

    public void RealBallPath(int team, string animationName, int index)
    {
        if (!GameController.Get.IsReset)
        {
            //ToDo:目前bug BasketNetPlay 跟 ActionNoScoreEnd 會同時呼叫
			
            switch (animationName)
            {
                case "ActionEnd":
                    SetBasketState(EPlayerState.BasketActionEnd, team);
                    SetBallOwnerNull();
                    break;
                case "ActionNoScoreShot":
//				PlayShootNoScore(team);
                    break;
                case "ActionNoScoreEnd":
                    SetBasketState(EPlayerState.BasketActionNoScoreEnd, team);
                    SetBallOwnerNull();
                    break;
                case "BasketNetPlay":
                    PlayShoot(team, index);
                    if (index < 100)
                        RealBallCompoment.MoveVelocity = Vector3.zero;
                    break;
            }
        }
    }

    public void IfSwishNoScore()
    {
        if (isSwishIn)
        {
            isSwishIn = false;
            if (GameStart.Get.IsDebugAnimation)
            {
                GameController.Get.shootScoreSwishTimes++;
                Debug.LogWarning("RealBall Swish Out:" + Time.time);
            }
            if (scoreTeam != -1)
            {
                GameController.Get.PlusScore(scoreTeam, false, true);
                GameController.Get.ShowShootSate(true, scoreTeam);
                LayerMgr.Get.IgnoreLayerCollision(ELayer.BasketCollider, ELayer.RealBall, false);
                RealBallCompoment.AddForce(Vector3.down * 2, ForceMode.VelocityChange);
                scoreTeam = -1;
            }
        }
    }

    public void SetBasketState(EPlayerState state, int team = 0)
    {
        if (!GameController.Get.IsReset)
        {
            //Debug.LogError("SetBasketState : " + state.ToString());
            switch (state)
            {
                case EPlayerState.BasketActionSwish:
                    scoreTeam = team;
                    LayerMgr.Get.IgnoreLayerCollision(ELayer.BasketCollider, ELayer.RealBall, true);
                    RealBallObj.transform.DOMove(new Vector3(BasketEntra[team, 1].transform.position.x + Mathf.Clamp(-(RealBallObj.transform.position.x - BasketEntra[team, 0].transform.position.x), -BasketEntra[team, 1].transform.localPosition.x, BasketEntra[team, 1].transform.localPosition.x),
                            BasketEntra[team, 1].transform.position.y,
                            BasketEntra[team, 1].transform.position.z + Mathf.Clamp(-(RealBallObj.transform.position.z - BasketEntra[team, 0].transform.position.z), -BasketEntra[team, 1].transform.localPosition.z, BasketEntra[team, 1].transform.localPosition.z)), 0.15f).OnComplete(IfSwishNoScore);

                    break;
                case EPlayerState.BasketActionSwishEnd:
                    RealBallDoMoveFinish();
                    IfSwishNoScore();
                    isBallOffensive = false;
                    break;
                case EPlayerState.BasketAnimationStart:
                    RealBallCompoment.Gravity = false;
                    RealBallCompoment.TriggerEnable = false;
					RealBallObj.transform.parent = BasketHoopDummy[team];

                    GameController.Get.IsReboundTime = true;
                    GameController.Get.BallState = EBallState.None;
                    break;
                case EPlayerState.BasketActionEnd:
                    if (GameStart.Get.IsDebugAnimation)
                    {
                        GameController.Get.shootScoreTimes++;
                        Debug.LogWarning("RealBall Score Out:" + BasketAnimationName);
                    }
                    isBallOffensive = false;
                    GameController.Get.PlusScore(team, false, true);
                    GameController.Get.ShowShootSate(true, team);
                    SetBallOwnerNull();

					RealBallDoMoveFinish();
                    RealBallCompoment.AddForce(Vector3.right * 5, ForceMode.Impulse);
                    GameController.Get.IsPassing = false;
                    GameController.Get.BallState = EBallState.None;
                    break;
                case EPlayerState.BasketActionNoScoreEnd:
                    if (GameStart.Get.IsDebugAnimation)
                        Debug.LogWarning("RealBall NoScore Out:" + BasketAnimationName);
                    isBallOffensive = false;
                    GameController.Get.ShowShootSate(false, team);
                    SetBallOwnerNull();
                    RealBallCompoment.AddForce(new Vector3(1, 0, 0) * (70 + GameController.Get.ShootDistance * 2), ForceMode.Impulse);
					RealBallDoMoveFinish();
                    GameController.Get.IsPassing = false;
                    GameController.Get.BallState = EBallState.CanRebound;
                    break;
            }
        }
    }

    public void SetBallOwnerNull()
    {
        RealBallCompoment.ColliderEnable = true;
        RealBallCompoment.TriggerEnable = true;
        RealBallCompoment.Parent = null;
        RealBallCompoment.Gravity = true;
    }

    public void SetBallStateByLobby(EPlayerState state, Transform dummyTransfrom)
    {
        LayerMgr.Get.SetLayerAllChildren(RealBallObj, "Player");
        switch (state)
        {
            case EPlayerState.Dribble0:
                RealBallCompoment.ColliderEnable = false;
                RealBallCompoment.Gravity = false;

                if (dummyTransfrom)
                    RealBallCompoment.Parent = dummyTransfrom;
                RealBallCompoment.TriggerEnable = false;

                HideBallSFX();
                spotlight.SetActive(false);
                break;

            case EPlayerState.Shoot0: 
            case EPlayerState.Shoot1: 
            case EPlayerState.Shoot2: 
            case EPlayerState.Shoot3: 
            case EPlayerState.Shoot4: 
            case EPlayerState.Shoot5: 
            case EPlayerState.Shoot6: 
            case EPlayerState.Shoot7: 
            case EPlayerState.Shoot20: 
            case EPlayerState.Layup0: 
            case EPlayerState.Layup1: 
            case EPlayerState.Layup2: 
            case EPlayerState.Layup3: 
            case EPlayerState.TipIn: 
            case EPlayerState.Shooting: 
                spotlight.SetActive(false);
                RealBallCompoment.Parent = null;
                RealBallCompoment.Gravity = true;
                break;
        }
    }

    public void SetBallState(EPlayerState state, PlayerBehaviour player = null)
    {
        if (!GameController.Get.IsStart && state != EPlayerState.Start &&
        state != EPlayerState.Reset && GameStart.Get.TestMode == EGameTest.None)
            return;
//        Debug.LogError("SetBallState : " + state.ToString());
        SetBallOwnerNull();

        RealBallState = state;
			
        switch (state)
        {
            case EPlayerState.Dribble0:
                RealBallCompoment.ColliderEnable = false;
                RealBallCompoment.Gravity = false;

                if (player)
                    RealBallCompoment.Parent = player.DummyBall.transform;
                RealBallCompoment.TriggerEnable = false;
                HideBallSFX();
                spotlight.SetActive(false);
                break;

            case EPlayerState.Shoot0: 
            case EPlayerState.Shoot1: 
            case EPlayerState.Shoot2: 
            case EPlayerState.Shoot3: 
            case EPlayerState.Shoot4: 
            case EPlayerState.Shoot5: 
            case EPlayerState.Shoot6: 
            case EPlayerState.Shoot7: 
            case EPlayerState.Shoot20: 
            case EPlayerState.Layup0: 
            case EPlayerState.Layup1: 
            case EPlayerState.Layup2: 
            case EPlayerState.Layup3: 
            case EPlayerState.TipIn: 
            case EPlayerState.Shooting: 
                spotlight.SetActive(false);
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
                RealBallCompoment.Gravity = false;
                spotlight.SetActive(false);
                break;

            case EPlayerState.Steal0:
            case EPlayerState.Steal1:
            case EPlayerState.Steal2:
                GameController.Get.IsPassing = false;
				
//				Vector3 v = RealBall.transform.forward * -1;
//				if(player != null)					
//					v = player.transform.forward * 10;

                Vector3 newDir = Vector3.zero;
                newDir.Set(Random.Range(-1, 1), 0, Random.Range(-1, 1));

                // 10 是速度. 如果給太低, 球會在持球者附近, 變成持球者還是可以繼續撿球.
                RealBallCompoment.MoveVelocity = newDir.normalized * 10;
                mRealBallSFX.SetActive(true);
                spotlight.SetActive(true);
                break;
            case EPlayerState.JumpBall:
                if (!GameController.Get.IsJumpBall)
                {
                    GameController.Get.IsJumpBall = true;
                    GameController.Get.IsPassing = false;

                    Vector3 v1;
                    if (player != null)
                        v1 = player.transform.position; // 球要拍到某位球員的位置.
					else
                        v1 = RealBallObj.transform.forward * -1;

                    RealBallCompoment.MoveVelocity = GameFunction.GetVelocity(RealBallObj.transform.position, v1, 60);
                    mRealBallSFX.SetActive(true);
                    spotlight.SetActive(false);
                    AudioMgr.Get.PlaySound(SoundType.SD_Rebound);
                }
                break;
			
            case EPlayerState.Block0:
            case EPlayerState.Block1:
            case EPlayerState.Block2:
            case EPlayerState.Block20:
            case EPlayerState.KnockDown0: 
            case EPlayerState.KnockDown1: 
                GameController.Get.Shooter = null;
                GameController.Get.IsPassing = false;
				
                Vector3 v = RealBallObj.transform.forward * -1;
                if (player != null)
                    v = player.transform.forward * 10;

                RealBallCompoment.MoveVelocity = v;
                mRealBallSFX.SetActive(true);
                spotlight.SetActive(true);
                break;

            case EPlayerState.Dunk0:
            case EPlayerState.Dunk1:
            case EPlayerState.Dunk3:
            case EPlayerState.Dunk5:
            case EPlayerState.Dunk7:
                GameController.Get.BallState = EBallState.CanDunkBlock;
                RealBallCompoment.ColliderEnable = true;
                RealBallCompoment.Gravity = false;
				
                if (player)
                    RealBallCompoment.Parent = player.DummyBall.transform;
                RealBallCompoment.TriggerEnable = false;
                HideBallSFX();
                spotlight.SetActive(false);
                break;

            case EPlayerState.DunkBasket:
                if (player)
                    RealBallObj.transform.position = player.DummyBall.transform.position;

                RealBallCompoment.AddForce(Vector3.down * 1, ForceMode.VelocityChange);
                break;

            case EPlayerState.Reset:
                RealBallCompoment.Gravity = false;
                RealBallObj.transform.position = new Vector3(0, 7, 0);
                mRealBallSFX.SetActive(true);
                spotlight.SetActive(false);
                break;

            case EPlayerState.Start:
                RealBallObj.transform.localPosition = new Vector3(0, 6, 0);
                RealBallCompoment.Gravity = true;
                spotlight.SetActive(false);
                break;

            case EPlayerState.HoldBall:
            case EPlayerState.Pick0:
            case EPlayerState.Pick1:
            case EPlayerState.Pick2:
                RealBallCompoment.ColliderEnable = false;
                if (player)
                {
                    RealBallCompoment.Parent = player.DummyBall.transform;
                    RealBallObj.transform.DOKill();
                }

                RealBallCompoment.Gravity = false;
                RealBallCompoment.TriggerEnable = false;
                HideBallSFX();
                spotlight.SetActive(false);
                break;
        }
    }

	public void JudgeBasketAnimationName (int basketDistanceAngleType) {
		int random = 0;
		if(GameController.Get.BasketSituation == EBasketSituation.Score){
			if(DBasketAnimationName.Count > 0 && basketDistanceAngleType < DBasketAnimationName.Count){
				random = Random.Range(0, DBasketAnimationName[basketDistanceAngleType].Count);
				if(DBasketAnimationName.Count > 0 && random < DBasketAnimationName.Count)
					BasketAnimationName = DBasketAnimationName[basketDistanceAngleType][random];
			}
		} else 
			if(GameController.Get.BasketSituation == EBasketSituation.NoScore){
				if(DBasketAnimationNoneState.Count > 0 && basketDistanceAngleType < DBasketAnimationNoneState.Count) {
					random = Random.Range(0, DBasketAnimationNoneState[basketDistanceAngleType].Count);
					if(DBasketAnimationNoneState.Count > 0 && random < DBasketAnimationNoneState.Count)
						BasketAnimationName = DBasketAnimationNoneState[basketDistanceAngleType][random];
				}
			}

		if(GameController.Get.BasketSituation == EBasketSituation.Score || GameController.Get.BasketSituation == EBasketSituation.NoScore) 
		if(string.IsNullOrEmpty(BasketAnimationName))
			JudgeBasketAnimationName(basketDistanceAngleType);

	}

	public void RealBallShoot (PlayerBehaviour player, int shootAngle, float ShootDistance) {
		if(GameController.Get.BasketSituation == EBasketSituation.AirBall) {
            LayerMgr.Get.IgnoreLayerCollision(ELayer.IgnoreRaycast, ELayer.RealBall, true);
			RealBallCompoment.MoveVelocity = GameFunction.GetVelocity(RealBallObj.transform.position, BasketAirBall[player.Team.GetHashCode()].transform.position, shootAngle);
		} else
			if(player.crtState == EPlayerState.TipIn) {
				if(GameController.Get.BasketSituation == EBasketSituation.Swish) {
					if(RealBallObj.transform.position.y > (ShootPoint [player.Team.GetHashCode()].transform.position.y + 0.2f)) {
						RealBallObj.transform.DOMove(new Vector3(ShootPoint [player.Team.GetHashCode()].transform.position.x,
														      ShootPoint [player.Team.GetHashCode()].transform.position.y + 0.5f,
														      ShootPoint [player.Team.GetHashCode()].transform.position.z), 1 / TimerMgr.Get.CrtTime * 0.5f);
					} else {
						RealBallObj.transform.DOMove(ShootPoint [player.Team.GetHashCode()].transform.position, 1/ TimerMgr.Get.CrtTime * 0.2f); //0.2f	
					}
				} else {
					if(RealBallObj.transform.position.y > (DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].y + 0.2f)) {

						RealBallObj.transform.DOMove(new Vector3(DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].x,
							DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].y + 0.5f,
							DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].z), 1 / TimerMgr.Get.CrtTime * 0.5f);
					} else
						RealBallObj.transform.DOMove(DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName], 1/ TimerMgr.Get.CrtTime * 0.2f); //0.2f	
				}
			}else 
				if(GameController.Get.BasketSituation == EBasketSituation.Swish) {
                    LayerMgr.Get.IgnoreLayerCollision(ELayer.BasketCollider, ELayer.RealBall, true);
					if(player.GetSkillKind == ESkillKind.LayupSpecial) {
						RealBallObj.transform.DOMove(ShootPoint [player.Team.GetHashCode()].transform.position, 1/ TimerMgr.Get.CrtTime * 0.4f); //0.2
					} else if(player.Attribute.BodyType == 0 && ShootDistance < 5) {
                        RealBallCompoment.MoveVelocity = GameFunction.GetVelocity(RealBallObj.transform.position, 
							ShootPoint [player.Team.GetHashCode()].transform.position , shootAngle, 1f);
					} else 
                        RealBallCompoment.MoveVelocity = GameFunction.GetVelocity(RealBallObj.transform.position, 
							ShootPoint [player.Team.GetHashCode()].transform.position , shootAngle);	
				} else {
					if(DBasketShootWorldPosition.ContainsKey (player.Team.GetHashCode().ToString() + "_" + BasketAnimationName)) {
						if(player.GetSkillKind == ESkillKind.LayupSpecial) {
							RealBallObj.transform.DOMove(ShootPoint [player.Team.GetHashCode()].transform.position, 1/ TimerMgr.Get.CrtTime * 0.4f); //0.2
						} else if(player.Attribute.BodyType == 0 && ShootDistance < 5) {
                            RealBallCompoment.MoveVelocity = GameFunction.GetVelocity(RealBallObj.transform.position, 
								DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName] , shootAngle, 1f);
						}  else {
							float dis = GameController.Get.GetDis(new Vector2(RealBallObj.transform.position.x, RealBallObj.transform.position.z),
								new Vector2(DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].x, DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].z));
							if(dis>10)
								dis = 10;
                            RealBallCompoment.MoveVelocity = GameFunction.GetVelocity(RealBallObj.transform.position,
								DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName],
								shootAngle,
								dis * 0.05f);
						}

					} else 
						Debug.LogError("No key:"+player.Team.GetHashCode().ToString() + "_" + BasketAnimationName);
				}
	}

    public void RealBallDoMoveFinish()
    {
        RealBallObj.transform.DOKill(false);
        RealBallCompoment.TriggerEnable = true;
    }

    public void SetScoreboards(int team, int score)
    {
//		if (Scoreboards [team] == null) {
//			InitScoreboard(true);		
//		}
//
//		if(Scoreboards [team])
//			Scoreboards [team].text = score.ToString();
    }

    public void SetRealBallPosition(Vector3 pos)
    {
        RealBallObj.transform.position = pos;
    }

    public void SetRealBallOffset(Vector3 pos)
    {
        RealBallObj.transform.Translate(pos);
    }

    public void ResetBasketEntra()
    {
        LayerMgr.Get.IgnoreLayerCollision(ELayer.IgnoreRaycast, ELayer.RealBall, false);
        GameController.Get.IsReboundTime = false;

        for (int i = 0; i < 2; i++)
        {
            BasketAirBall[i].Into = false;
            for (int j = 0; j < 2; j++)
                BasketEntra[i, j].Into = false;
        }
    }

    public void PlayDunk(int team, int stageNo)
    {
        Animator animator;
        string animationName = string.Format("Dunk_{0}", stageNo);
        if (team == 0)
        {
            animator = pveBasketAy[0].GetComponent<Animator>();

            if (Hood[0])
                Hood[0].gameObject.SetActive(true);
        }
        else
        {
            if (isPve)
            {
                animator = pveBasketAy[1].GetComponent<Animator>();
            }
            else
            {
                animator = BuildBasket[1].GetComponent<Animator>();
            }
           
            Hood[1].gameObject.SetActive(true);
        }
        animator.SetTrigger(animationName);
    }

    public void PlayBasketEffect(int team, string effectName, int parent, float duration)
    {
        if (!string.IsNullOrEmpty(effectName))
        {
            if (parent == 0)
            { 
                EffectManager.Get.PlayEffect(effectName, ShootPoint[team].transform.position, null, null, duration);
            }
            else
            {
                EffectManager.Get.PlayEffect(effectName, Vector3.zero, EffectPoint[team], null, duration);
            }
        }
    }

    public void PlayShoot(int team, int index)
    {
        Animator animator;
        string animationName = "Shot_" + index.ToString();
        if (team == 0)
        {
            animator = pveBasketAy[0].GetComponent<Animator>();
            Hood[0].gameObject.SetActive(true);
        }
        else
        {
            if (isPve)
            {
                animator = pveBasketAy[1].GetComponent<Animator>();
            }
            else
            {
                animator = BuildBasket[1].GetComponent<Animator>();
            }
			
            Hood[1].gameObject.SetActive(true);
        }
        animator.SetTrigger(animationName);
    }

    IEnumerator Reset()
    {  
        yield return new WaitForSeconds(3f);

        if (isPve)
        {
            for (int i = 0; i < 2; i++)
            {
                if (pveBasketAy[i])
                {
                    pveBasketAy[i].transform.localEulerAngles = animRotate[i];
                    pveBasketAy[i].transform.localPosition = animPos[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                if (BuildDummyAy[i])
                {
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

    public Vector3 GetHoodPosition(ETeamKind teamKind)
    {
        return Hood[(int)teamKind].transform.position;
    }

    public Vector3 GetShootPointPosition(ETeamKind teamKind)
    {
        return ShootPoint[(int)teamKind].transform.position;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sfxTime"> 特效顯示的時間, 單位:秒. -1: 表示特效永遠顯示, 必須要手動關閉. </param>
    public void ShowBallSFX(float sfxTime = -1)
    {
        mRealBallSFX.SetActive(true);
        if (sfxTime > 0)
            mRealBallSFXTimer.Start(sfxTime);
    }

    public bool IsBallSFXEnabled()
    {
        return mRealBallSFX.activeInHierarchy;
    }

    public void HideBallSFX()
    {
        mRealBallSFX.SetActive(false);
        mRealBallSFXTimer.Stop();
    }

    public bool IsRealBallActive
    {
        get { return isRealBallInAcitve; }
        set { isRealBallInAcitve = value; }
    }

    public bool IsBallOffensive
    {
        get { return isBallOffensive; }
        set { isBallOffensive = value; }
    }
}

