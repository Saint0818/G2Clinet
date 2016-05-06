﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using GameEnum;

/// <summary>
/// <para> 管理遊戲球場相關的事情. </para>
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call InitCourtScene() 初始化場景. </item>
/// </list>
public class CourtMgr : KnightSingleton<CourtMgr>
{
    private const string DefaultRealBallPath = "Prefab/Stadium/RealBall";
    private const string PrefabRealBallPath = "Prefab/Stadium/RealBall";

    public int CourtMode = ECourtMode.Full;
    public string BasketAnimationName = "BasketballAction_1";
    public bool IsDebugAnimation;
    private bool isPve = true;
    private int attackDirection = 0;
    private int crtBasketIndex = -1;
    private GameObject crtBasket;

    private bool isBallOffensive = false;

    private GameObject realBall;
    public RealBall RealBall;

    private bool isRealBallInAcitve = false;
    private GameObject crtCollider;
    private GameObject[] pveBasketAy = new GameObject[2];
    private GameObject[] BuildBasket = new GameObject[2];
    private GameObject[] BuildDummyAy = new GameObject[2];
    private Vector3[] animPos = new Vector3[2];
    private Vector3[] animRotate = new Vector3[2];

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
	private BasketAnimation[] basketAnimation = new BasketAnimation[2];
	private BasketAnimation[] basketActionAnimation = new BasketAnimation[2];
		
    public CircularSectorMeshRenderer SkillRangeOfAction;
    public GameObject SkillArrowOfAction;
    private UITexture textureArrow;
    public Transform[] EndPlayerPosition = new Transform[6];
    public GameObject[] CameraHood = new GameObject[2];
    public BallCurve RealBallCurve;

    public Dictionary<string, Vector3> DBasketShootWorldPosition = new Dictionary<string, Vector3>();
    public Dictionary<int, List<string>> DBasketAnimationName = new Dictionary<int, List<string>>();
    public Dictionary<int, List<string>> DBasketAnimationNoneState = new Dictionary<int, List<string>>();

	private int scoreTeam;
	private bool isSwishIn;

    void OnDestroy()
    {
        DBasketShootWorldPosition.Clear();
        DBasketAnimationName.Clear();
        DBasketAnimationNoneState.Clear();

        if (crtBasket)
            Destroy(crtBasket);

        if (SkillRangeOfAction)
            Destroy(SkillRangeOfAction);

        if (SkillArrowOfAction)
            Destroy(SkillArrowOfAction);

        if (realBall)
            Destroy(realBall);

        if (crtCollider)
            Destroy(crtCollider);
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

    public void InitCourtScene(int courtNo)
    {
        loadBall(courtNo);
        ChangeBasket(courtNo);
        checkCollider();
        loadSkillComponents();

        CameraMgr.Get.SetCameraSituation(ECameraSituation.Loading);
        CameraMgr.Get.CourtMode = CourtMode;
    }

    public void InitBasketAnimator(RuntimeAnimatorController controller)
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

                if (CourtMode == ECourtMode.Full)
                    DBasketShootWorldPosition.Add("1_" + GameData.BasketShootPosition[i].AnimationName, BasketHoopDummy[1].position);
                else
                    DBasketShootWorldPosition.Add("1_" + GameData.BasketShootPosition[i].AnimationName, BasketHoopDummy[0].position);
            }

            BasketHoopDummy[0].localPosition = Vector3.zero;
            BasketHoopDummy[1].localPosition = Vector3.zero;
        }

        scoreName.Clear();
        noScoreName.Clear();
        BasketScoreAnimationStateRightWing.Clear();
        BasketScoreAnimationStateRight.Clear();
        BasketScoreAnimationStateCenter.Clear();
        BasketScoreAnimationStateLeft.Clear();
        BasketScoreAnimationStateLeftWing.Clear();
        BasketNoScoreAnimationStateRightWing.Clear();
        BasketNoScoreAnimationStateRight.Clear();
        BasketNoScoreAnimationStateCenter.Clear();
        BasketNoScoreAnimationStateLeft.Clear();
        BasketNoScoreAnimationStateLeftWing.Clear();
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

    private void loadSkillComponents()
    {
        if(!SkillRangeOfAction)
            SkillRangeOfAction = Instantiate(Resources.Load("Effect/RangeOfAction") as GameObject).GetComponent<CircularSectorMeshRenderer>();

        if(!SkillArrowOfAction)
        {
            SkillArrowOfAction = Instantiate(Resources.Load("Effect/SkillArea_Arrow") as GameObject);
            SkillArrowOfAction.SetActive(false);
            Transform t = SkillArrowOfAction.transform.FindChild("Scale/SpriteSkillAreaArrow");
            if(t != null)
                textureArrow = t.GetComponent<UITexture>();
        }
    }

    public void ShowRangeOfAction(bool isShow, Transform parent = null, float degree = 0, float dis = 0, float euler = 0)
    {
        SkillRangeOfAction.transform.parent = parent;
		//有機會會被改到layer,所以要檢查
		if(!LayerMgr.Get.CheckLayer(SkillRangeOfAction.gameObject, ELayer.Default))
			LayerMgr.Get.SetLayer(SkillRangeOfAction.gameObject, ELayer.Default);
        
        if (parent)
            SkillRangeOfAction.transform.localPosition = new Vector3(0, 0.1f, 0);
        
        SkillRangeOfAction.transform.localEulerAngles = new Vector3(0, euler, 0);
        SkillRangeOfAction.ChangeValue(degree, dis);
        SkillRangeOfAction.RefGameObject.SetActive(isShow);
    }

	public void ShowArrowOfAction(bool isShow, PlayerBehaviour parent = null, float dis = 0)
    {
		if(isShow && parent) {
			SkillArrowOfAction.transform.parent = parent.transform;
			SkillArrowOfAction.transform.localPosition = new Vector3(-1, 0.1f, 0);
		}

		//有機會會被改到layer,所以要檢查
		if(!LayerMgr.Get.CheckLayer(SkillArrowOfAction.gameObject, ELayer.Default))
			LayerMgr.Get.SetLayer(SkillArrowOfAction.gameObject, ELayer.Default);

		SkillArrowOfAction.transform.localPosition = Vector3.zero;
        SkillArrowOfAction.transform.localEulerAngles = Vector3.zero;
		if (textureArrow && parent){
			textureArrow.SetRect(0, 0, 200 * parent.GetPushThroughW, dis * 100);  //Scale:1 = X:200
			textureArrow.transform.localPosition = Vector3.zero;
		}
        

        SkillArrowOfAction.SetActive(isShow);
    }

	public Vector3 GetArrowPosition (GameObject player, float dis) {
		SkillArrowOfAction.transform.parent = player.transform;
		SkillArrowOfAction.transform.localPosition = Vector3.zero;
		SkillArrowOfAction.transform.localPosition = new Vector3(SkillArrowOfAction.transform.localPosition.x, SkillArrowOfAction.transform.localPosition.y, SkillArrowOfAction.transform.localPosition.z + dis);
		SkillArrowOfAction.transform.parent = null;
		return SkillArrowOfAction.transform.position;
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
					RealBall.Trigger.IsAutoRotate = false;
					RealBallDoMoveFinish();
					switch (GameController.Get.BasketSituation) {
					case EBasketSituation.Swish:
						if(IsDebugAnimation && GameController.Visible){
							Debug.LogWarning("RealBall Swish IN:"+ Time.time);
							GameController.Get.shootSwishTimes++;
						}

						isSwishIn = true;
						SetBasketState(EPlayerState.BasketActionSwish, Team);
						break;
					case EBasketSituation.Score:
					case EBasketSituation.NoScore:
						if(IsDebugAnimation && GameController.Visible) {
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

    private void loadBall(int courtNum)
    {
        if(realBall)
        {
            Destroy(realBall);
            realBall = null;
        }

        realBall = safeLoadBall(courtNum);
        realBall.name = "RealBall";
        realBall.transform.localPosition = new Vector3(0, 3.5f, 0);

        RealBall = realBall.GetComponent<RealBall>();
        RealBall.RigidbodyCom.isKinematic = true;
        RealBall.RigidbodyCom.useGravity = false;

        if (RealBallCurve == null)
            RealBallCurve = realBall.GetComponent<BallCurve>();
    }

    private GameObject safeLoadBall(int courtNum)
    {
        string realBallPathName = string.Format("{0}_{1}", PrefabRealBallPath, courtNum);
        GameObject realBallObj = Resources.Load<GameObject>(realBallPathName);
        if(realBallObj == null)
        {
            Debug.LogErrorFormat("Can't find RealBall: {0}", realBallPathName);
            realBallObj = Resources.Load<GameObject>(DefaultRealBallPath);
        }

        return Instantiate(realBallObj);
    }

    private void checkCollider()
    {
        if (crtCollider == null) {
            crtCollider = Instantiate(Resources.Load("Prefab/Stadium/StadiumCollider")) as GameObject;
            crtCollider.transform.parent = transform;
		
            EndPlayerPosition[0] = GameObject.Find(string.Format("{0}/GameFinishPos/Win/1", crtCollider.name)).transform;
            EndPlayerPosition[1] = GameObject.Find(string.Format("{0}/GameFinishPos/Win/2", crtCollider.name)).transform;
            EndPlayerPosition[2] = GameObject.Find(string.Format("{0}/GameFinishPos/Win/3", crtCollider.name)).transform;
            EndPlayerPosition[3] = GameObject.Find(string.Format("{0}/GameFinishPos/Lose/4", crtCollider.name)).transform;
            EndPlayerPosition[4] = GameObject.Find(string.Format("{0}/GameFinishPos/Lose/5", crtCollider.name)).transform;
            EndPlayerPosition[5] = GameObject.Find(string.Format("{0}/GameFinishPos/Lose/6", crtCollider.name)).transform;
            
            if (CourtMode == ECourtMode.Full) {
                Walls[0] = GameObject.Find(string.Format("{0}/Wall/Wall/WallA", crtCollider.name));
                Walls[1] = GameObject.Find(string.Format("{0}/Wall/Wall/WallB", crtCollider.name)); 
                Hood[0] = GameObject.Find(string.Format("{0}/HoodA", crtCollider.name));
                Hood[1] = GameObject.Find(string.Format("{0}/HoodB", crtCollider.name)); 
                ShootPoint[0] = GameObject.Find(string.Format("{0}/HoodA/ShootPoint", crtCollider.name));
                ShootPoint[1] = GameObject.Find(string.Format("{0}/HoodB/ShootPoint", crtCollider.name));
                BasketRangeCenter[0] = GameObject.Find(string.Format("{0}/RangeOfActionCenterL", crtCollider.name));
                BasketRangeCenter[1] = GameObject.Find(string.Format("{0}/RangeOfActionCenterR", crtCollider.name));
                MissPoint[0] = GameObject.Find(string.Format("{0}/MissPos/A", crtCollider.name));
                MissPoint[1] = GameObject.Find(string.Format("{0}/MissPos/B", crtCollider.name));
                DunkPoint[0] = GameObject.Find(string.Format("{0}/DunkL/Point", crtCollider.name));
                DunkPoint[1] = GameObject.Find(string.Format("{0}/DunkR/Point", crtCollider.name));
                DunkJumpPoint[0] = GameObject.Find(string.Format("{0}/DunkL/JumpPoint", crtCollider.name));
                DunkJumpPoint[1] = GameObject.Find(string.Format("{0}/DunkR/JumpPoint", crtCollider.name));
                CameraHood[0] = GameObject.Find(string.Format("{0}/CameraHood/A", crtCollider.name));
                CameraHood[1] = GameObject.Find(string.Format("{0}/CameraHood/B", crtCollider.name));
                BasketEntra[0, 0] = GameObject.Find(string.Format("{0}/HoodA/Entra", crtCollider.name)).GetComponent<ScoreTrigger>();
    			BasketEntra[0, 0].ScoreDel += ScoreMgr;
    			BasketEntra[0, 1] = GameObject.Find(string.Format("{0}/HoodA/Sale", crtCollider.name)).GetComponent<ScoreTrigger>();
    			BasketEntra[0, 1].ScoreDel += ScoreMgr;
                BasketEntra[0, 1].IntTrigger = 1;
    			BasketEntra[1, 0] = GameObject.Find(string.Format("{0}/HoodB/Entra", crtCollider.name)).GetComponent<ScoreTrigger>();
    			BasketEntra[1, 0].ScoreDel += ScoreMgr;
    			BasketEntra[1, 1] = GameObject.Find(string.Format("{0}/HoodB/Sale", crtCollider.name)).GetComponent<ScoreTrigger>();
    			BasketEntra[1, 1].ScoreDel += ScoreMgr;
                BasketEntra[1, 1].IntTrigger = 1;
                BasketAirBall[0] = GameObject.Find(string.Format("{0}/HoodA/AirBall", crtCollider.name)).GetComponent<AirBallTrigger>();
    			BasketAirBall[0].AirBallDel += AirBallMgr;
    			BasketAirBall[1] = GameObject.Find(string.Format("{0}/HoodB/AirBall", crtCollider.name)).GetComponent<AirBallTrigger>();
    			BasketAirBall[1].AirBallDel += AirBallMgr;

                for (int i = 0; i < Distance3Pos.GetLength(0); i++)
                    for (int j = 0; j < Distance3Pos.GetLength(1); j++)
                        Distance3Pos[i, j] = GameObject.Find(string.Format("{0}/Distance3/{1}/Distance3_{2}", crtCollider.name, i, j));
            }
            else {
                Walls[0] = GameObject.Find(string.Format("{0}/Wall/Wall/WallA", crtCollider.name));
                Walls[1] = Walls[0];
                Hood[0] = GameObject.Find(string.Format("{0}/HoodA", crtCollider.name));
                Hood[1] = Hood[0];
                ShootPoint[0] = GameObject.Find(string.Format("{0}/HoodA/ShootPoint", crtCollider.name));
                ShootPoint[1] = ShootPoint[0];
                MissPoint[0] = GameObject.Find(string.Format("{0}/MissPos/A", crtCollider.name));
                MissPoint[1] = MissPoint[0];
                DunkPoint[0] = GameObject.Find(string.Format("{0}/DunkL/Point", crtCollider.name));
                DunkPoint[1] = DunkPoint[0];
                DunkJumpPoint[0] = GameObject.Find(string.Format("{0}/DunkL/JumpPoint", crtCollider.name));
                DunkJumpPoint[1] = DunkJumpPoint[0];
                CameraHood[0] = GameObject.Find(string.Format("{0}/CameraHood/A", crtCollider.name));
                CameraHood[1] = CameraHood[0];
    			BasketEntra[0, 0] = GameObject.Find(string.Format("{0}/HoodA/Entra", crtCollider.name)).GetComponent<ScoreTrigger>();
    			BasketEntra[0, 0].ScoreDel += ScoreMgr;
    			BasketEntra[0, 1] = GameObject.Find(string.Format("{0}/HoodA/Sale", crtCollider.name)).GetComponent<ScoreTrigger>();
    			BasketEntra[0, 1].ScoreDel += ScoreMgr;
                BasketEntra[0, 1].IntTrigger = 1;
                BasketEntra[1, 0] = BasketEntra[0, 0];
                BasketEntra[1, 1] = BasketEntra[0, 1];
    			BasketAirBall[0] = GameObject.Find(string.Format("{0}/HoodA/AirBall", crtCollider.name)).GetComponent<AirBallTrigger>();
    			BasketAirBall[0].AirBallDel += AirBallMgr;
                BasketAirBall[1] = BasketAirBall[0];

                for (int i = 0; i < Distance3Pos.GetLength(0); i++)
                    for (int j = 0; j < Distance3Pos.GetLength(1); j++)
                        Distance3Pos[i, j] = GameObject.Find(string.Format("{0}/Distance3/{1}/Distance3_{2}", crtCollider.name, 0, j));
            }
        }
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
		
        animPos[0] = pveBasketAy[0].transform.localPosition;
        animPos[1] = pveBasketAy[1].transform.localPosition;
        animRotate[0] = pveBasketAy[0].transform.localEulerAngles;
        animRotate[1] = pveBasketAy[1].transform.localEulerAngles;
		
        crtBasket.transform.parent = transform;
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

        InitBasketAnimator(BasketHoopAnimator[0].runtimeAnimatorController);
    }

    public void RealBallPath(int team, string animationName, int index)
    {
        if (!GameController.Get.IsReset)
        {
			//Debug.LogError("RealBallPath : " + animationName.ToString());
			
            switch (animationName)
            {
                case "ActionEnd":
                    SetBasketState(EPlayerState.BasketActionEnd, team);
					RealBall.SetBallOwnerNull();
                    break;
                case "ActionNoScoreShot":
//				PlayShootNoScore(team);
                    break;
                case "ActionNoScoreEnd":
                    SetBasketState(EPlayerState.BasketActionNoScoreEnd, team);
					RealBall.SetBallOwnerNull();
                    break;
                case "BasketNetPlay":
                    PlayShoot(team, index);
                    if (index < 100)
                        RealBall.MoveVelocity = Vector3.zero;
                    break;
            }
        }
    }

    public void IfSwishNoScore()
    {
        if (isSwishIn)
        {
            isSwishIn = false;
            if (IsDebugAnimation)
            {
                GameController.Get.shootScoreSwishTimes++;
                Debug.LogWarning("RealBall Swish Out:" + Time.time);
            }
            if (scoreTeam != -1)
            {
                GameController.Get.PlusScore(scoreTeam, false, true);
                GameController.Get.ShowShootSate(true, scoreTeam);
                LayerMgr.Get.IgnoreLayerCollision(ELayer.BasketCollider, ELayer.RealBall, false);
				RealBall.MoveVelocity = Vector3.zero;
				RealBall.AddForce(Vector3.down, ForceMode.VelocityChange);
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
					//第一個偵測點 球：程式控制
                    scoreTeam = team;
                    LayerMgr.Get.IgnoreLayerCollision(ELayer.BasketCollider, ELayer.RealBall, true);
                    RealBall.transform.DOMove(new Vector3(BasketEntra[team, 1].transform.position.x + Mathf.Clamp(-(RealBall.transform.position.x - BasketEntra[team, 0].transform.position.x), -BasketEntra[team, 1].transform.localPosition.x, BasketEntra[team, 1].transform.localPosition.x),
                            BasketEntra[team, 1].transform.position.y,
                            BasketEntra[team, 1].transform.position.z + Mathf.Clamp(-(RealBall.transform.position.z - BasketEntra[team, 0].transform.position.z), -BasketEntra[team, 1].transform.localPosition.z, BasketEntra[team, 1].transform.localPosition.z)), 0.15f).OnComplete(IfSwishNoScore);

                    break;
                case EPlayerState.BasketActionSwishEnd:
					//第二個偵測點 球：物理	
                    RealBallDoMoveFinish();
                    IfSwishNoScore();
                    isBallOffensive = false;
                    break;
                case EPlayerState.BasketAnimationStart:
                    RealBall.Gravity = false;
                    RealBall.TriggerEnable = false;
					RealBall.Parent = BasketHoopDummy[team];					
                    GameController.Get.IsReboundTime = true;
                    GameController.Get.BallState = EBallState.None;
                    break;
                case EPlayerState.BasketActionEnd:
                    if (IsDebugAnimation)
                    {
                        GameController.Get.shootScoreTimes++;
                        Debug.LogWarning("RealBall Score Out:" + BasketAnimationName);
                    }
                    isBallOffensive = false;
                    GameController.Get.PlusScore(team, false, true);
                    GameController.Get.ShowShootSate(true, team);
					RealBall.SetBallOwnerNull();
					RealBallDoMoveFinish();
                    RealBall.AddForce(Vector3.right * 2, ForceMode.VelocityChange);
                    GameController.Get.IsPassing = false;
                    GameController.Get.BallState = EBallState.None;
                    break;
                case EPlayerState.BasketActionNoScoreEnd:
                    if (IsDebugAnimation)
                        Debug.LogWarning("RealBall NoScore Out:" + BasketAnimationName);
                    
                    isBallOffensive = false;
                    GameController.Get.ShowShootSate(false, team);
					RealBall.SetBallOwnerNull();
                    RealBall.AddForce(new Vector3(1, 0, 0) * (70 + GameController.Get.ShootDistance * 2), ForceMode.Impulse);
					RealBallDoMoveFinish();
                    GameController.Get.IsPassing = false;
                    GameController.Get.BallState = EBallState.CanRebound;
                    break;
            }
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

	public void RealBallShoot (PlayerBehaviour player, float shootAngle, float ShootDistance) {
		if(GameController.Get.BasketSituation == EBasketSituation.AirBall) {
            LayerMgr.Get.IgnoreLayerCollision(ELayer.IgnoreRaycast, ELayer.RealBall, true);
			RealBall.MoveVelocity = GameFunction.GetVelocity(RealBall.transform.position, BasketAirBall[player.Team.GetHashCode()].transform.position, shootAngle);
		} else
			if(player.CurrentState == EPlayerState.TipIn) {
				if(GameController.Get.BasketSituation == EBasketSituation.Swish) {
					if(RealBall.transform.position.y > (ShootPoint [player.Team.GetHashCode()].transform.position.y + 0.2f)) {
						RealBall.transform.DOMove(new Vector3(ShootPoint [player.Team.GetHashCode()].transform.position.x,
														      ShootPoint [player.Team.GetHashCode()].transform.position.y + 0.5f,
														      ShootPoint [player.Team.GetHashCode()].transform.position.z), 1 / TimerMgr.Get.CrtTime * 0.5f);
					} else {
						RealBall.transform.DOMove(ShootPoint [player.Team.GetHashCode()].transform.position, 1/ TimerMgr.Get.CrtTime * 0.2f); //0.2f	
					}
				} else {
					if(RealBall.transform.position.y > (DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].y + 0.2f)) {

						RealBall.transform.DOMove(new Vector3(DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].x,
							DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].y + 0.5f,
							DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].z), 1 / TimerMgr.Get.CrtTime * 0.5f);
					} else
						RealBall.transform.DOMove(DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName], 1/ TimerMgr.Get.CrtTime * 0.2f); //0.2f	
				}
			}else 
				if(GameController.Get.BasketSituation == EBasketSituation.Swish) {
                    LayerMgr.Get.IgnoreLayerCollision(ELayer.BasketCollider, ELayer.RealBall, true);
					if(player.GetSkillKind == ESkillKind.LayupSpecial) {
						RealBall.transform.DOMove(ShootPoint [player.Team.GetHashCode()].transform.position, 1/ TimerMgr.Get.CrtTime * 0.4f); //0.2
					} else if(player.Attribute.BodyType == 0 && ShootDistance < 5) {
                        RealBall.MoveVelocity = GameFunction.GetVelocity(RealBall.transform.position, 
							ShootPoint [player.Team.GetHashCode()].transform.position , shootAngle, 1f);
					} else 
                        RealBall.MoveVelocity = GameFunction.GetVelocity(RealBall.transform.position, 
							ShootPoint [player.Team.GetHashCode()].transform.position , shootAngle);	
				} else {
					if(DBasketShootWorldPosition.ContainsKey (player.Team.GetHashCode().ToString() + "_" + BasketAnimationName)) {
						if(player.GetSkillKind == ESkillKind.LayupSpecial) {
							RealBall.transform.DOMove(ShootPoint [player.Team.GetHashCode()].transform.position, 1/ TimerMgr.Get.CrtTime * 0.4f); //0.2
						} else if(player.Attribute.BodyType == 0 && ShootDistance < 5) {
                            RealBall.MoveVelocity = GameFunction.GetVelocity(RealBall.transform.position, 
								DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName] , shootAngle, 1f);
						}  else {
							float dis = GameController.Get.GetDis(new Vector2(RealBall.transform.position.x, RealBall.transform.position.z),
								new Vector2(DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].x, DBasketShootWorldPosition[player.Team.GetHashCode().ToString() + "_" + BasketAnimationName].z));
							if(dis>10)
								dis = 10;
                            RealBall.MoveVelocity = GameFunction.GetVelocity(RealBall.transform.position,
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
        RealBall.transform.DOKill(false);
        RealBall.TriggerEnable = true;
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
        RealBall.transform.position = pos;
    }

    public void SetRealBallOffset(Vector3 pos)
    {
        RealBall.transform.Translate(pos);
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

    public Vector3 GetHoodPosition(ETeamKind teamKind)
    {
        return Hood[(int)teamKind].transform.position;
    }

    public Vector3 GetShootPointPosition(ETeamKind teamKind)
    {
        return ShootPoint[(int)teamKind].transform.position;
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

