using System;
using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;
using DG.Tweening;

public enum EBildsType
{
    Player = 0,
    Basket = 1,
    Advertisement = 2,
    Store = 3,
    Gym = 4,
    Door = 5,
    Logo = 6,
    Chair = 7,
    Calendar = 8,
    Mail = 9
}

[DisallowMultipleComponent]
public class UI3DMainLobbyImpl : MonoBehaviour
{
    public Transform PlayerPos;
    public Animator cameraControl;
    private GameObject basket;
    private const int BuildCount = 10;
    public GameObject[] BuildPos = new GameObject[BuildCount];
    public GameObject[] Builds = new GameObject[BuildCount];
    private UIButton[] Btns = new UIButton[BuildCount];
    private GameObject advertisementPic;
    private GameObject mAvatarPlayer;
//    private Transform DummyBall;
    private AnimatorBehavior playerControl;
    private EPlayerState crtState;
    private int stateNo = 0;
    private int selectIndex = -1;
    private float delay = 0;

    private List<EPlayerState> showstate = new List<EPlayerState>();

    [UsedImplicitly]
    private void Awake()
    {
        if (PrefabSettingIsLegal())
        {
            int[] temp = new int[BuildCount];
            for (int i = 0; i < temp.Length; i++)
            {
                if (i == 0)
                    temp[i] = -1;
                else
                    temp[i] = 101;
            }
            //TODO:Read Server Data
            InitBuilds(temp);
            InitButtons();

            if (advertisementPic == null)
            {
                advertisementPic = Instantiate(Resources.Load("Prefab/Stadium/StadiumItem/AdvertisementPic")) as GameObject;
                advertisementPic.transform.parent = BuildPos[2].transform;
                advertisementPic.transform.localPosition = Vector3.zero;
                advertisementPic.transform.localScale = Vector3.one;
                advertisementPic.transform.localEulerAngles = Vector3.zero;
            }
        }
        else
            Debug.LogError("Setting Prefab Error");
    }

    private void InitButtons()
    {
        for (int i = 0; i < BuildPos.Length; i++)
        {
            Btns[i] = BuildPos[i].transform.parent.GetComponent<UIButton>();
            BuildPos[i].transform.parent.name = i.ToString();
            if (Btns[i])
                Btns[i].onClick.Add(new EventDelegate(OnSelect));
        }
    }


    [UsedImplicitly]
    private void Update()
    {
        if (delay > 0)
            delay -= Time.deltaTime;

//        Move();
    }

    private void OnSelect()
    {
//		//TODO: it's not open yet.
		return;
        if (delay > 0)
            return;

        int index;

        if (!UITutorial.Visible && int.TryParse(UIButton.current.name, out index))
        {
            if (selectIndex == index)
            {
                //back 
                selectIndex = -1;
                SetCameraAnimator(index, false);
                UpdateButtonCollider(index, true);
                UIMainLobby.Get.Main.PlayEnterAnimation();
                delay = 1;
            }
            else
            {
                //go
                if (selectIndex == -1)
                {
                    if (index == 0)
                    {
                        Play();
                    }
                    else
                    {
                        selectIndex = index;
                        SetCameraAnimator(index, true);
                        UpdateButtonCollider(index, false);
                        UIMainLobby.Get.Main.PlayExitAnimation();
                        delay = 1;
                    }
                }
            }
            AudioMgr.Get.PlaySound(SoundType.SD_LobbyCamara);
        }
    }

    private void Play()
    {
        Vector3 pos = CourtMgr.Get.ShootPoint[0].transform.position;
        mAvatarPlayer.transform.DOLookAt(new Vector3(pos.x, 0, pos.z), 0.4f).OnComplete(DoAni);
    }

    private void DoAni()
    {
        if (GameData.Team.Player.SkillCards.Length > 0)
        {
            crtState = GetRandomState();
            TAnimatorItem next = AnimatorMgr.Get.GetAnimatorStateType(crtState);

            if (next.Type == EAnimatorState.Pass)
            {
                next.Type = EAnimatorState.Shoot;
                stateNo = 0; 
                crtState = EPlayerState.Shoot0;
            }
            else
            {
                stateNo = next.StateNo;
            }
            Debug.LogError("LogError : " + crtState.ToString());
            playerControl.Play(crtState, 0); 
        }
    }

    private void UpdateButtonCollider(int index, bool isopen)
    {
        for (int i = 0; i < BuildPos.Length; i++)
        {
            if (isopen)
                BuildPos[i].transform.parent.transform.GetComponent<BoxCollider>().enabled = true;
            else
            {
                if (i != index)
                    BuildPos[i].transform.parent.transform.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    private string GetEBildsTypeString(int index)
    {
        return ((EBildsType)index).ToString();
    }

    private int GetEBildsTypeCount()
    {
        return Enum.GetNames(typeof(EBildsType)).Length;
    }

    private void SetCameraAnimator(int index, bool isgo)
    {
        if (index >= 0 && index < GetEBildsTypeCount() && cameraControl)
        {
            string state = (isgo == true ? "Go" : "Back");
            string eventName = string.Format("Event{0}_{1}", GetEBildsTypeString(index), state);
            cameraControl.SetTrigger(eventName);
        }
        else
            Debug.LogError("Animator Index Error");
    }

    private void InitBuilds(int[] buids)
    {
        if (buids.Length != BuildCount)
            return;
        else
        {
            for (int i = 0; i < Builds.Length; i++)
            {
                CloneObj(ref Builds[i], i, buids[i]);
            }
        }
    }

    public void CloneObj(ref GameObject clone, int index, int id)
    {
        if (id > 0)
        {
            string name = GetEBildsTypeString(index) + id.ToString();
            string path = string.Format("Prefab/Stadium/StadiumItem/{0}", name);
            GameObject obj;

            if (clone && name == clone.name)
                return;

            if (clone)
            {
                Destroy(clone);
                clone = null;
            }

            obj = Resources.Load(path) as GameObject;
            if (obj)
                clone = Instantiate(obj) as GameObject;
            else
                Debug.LogError("Can't found GameObject in Resource : " + path);
			
            if (clone && index < BuildPos.Length)
            {
                clone.transform.parent = BuildPos[index].transform;
                clone.transform.localPosition = Vector3.zero;
                clone.transform.localScale = Vector3.one;
                clone.transform.localEulerAngles = Vector3.zero;
                clone.name = name;

                if (index == 1)
                {
                    basket = clone;
                }
										
            }
        }
        else
        {
            if (clone)
            {
                Destroy(clone);
                clone = null;
            }
        }
    }

    public bool PrefabSettingIsLegal()
    {
        if (GetEBildsTypeCount() == BuildPos.Length)
        {
            for (int i = 0; i < BuildPos.Length; i++)
            {
                string name = string.Format("{0}Pos", ((EBildsType)i).ToString());
                if (BuildPos[i].name != name)
                    return false;
            }
        }
        else
        {
            return false;
        }

        return true;
    }

    public void Show()
    {
        UpdateAvatar();
//        InitSkillstate();
//        InitAnmator();
    }

    public void Hide()
    {
    }

    private Vector3 playerCenterPos;

    public void UpdateAvatar()
    {
        if (mAvatarPlayer)
            Destroy(mAvatarPlayer);

        mAvatarPlayer = new GameObject { name = "LobbyAvatarPlayer" };
        ModelManager.Get.SetAvatar(ref mAvatarPlayer, GameData.Team.Player.Avatar, 
            GameData.Team.Player.BodyType,
            EAnimatorType.AnimationControl, false);

        mAvatarPlayer.transform.parent = BuildPos[0].transform;
        mAvatarPlayer.transform.localPosition = Vector3.zero;
        mAvatarPlayer.transform.localScale = Vector3.one;
        mAvatarPlayer.transform.localRotation = Quaternion.identity;

        playerCenterPos = mAvatarPlayer.transform.position;
        CourtMgr.Get.DunkPoint[0] = BuildPos[1].gameObject;
        //TODO: 因新帳號無人物資料造成find null，所以先關掉
//        DummyBall = mAvatarPlayer.transform.Find("DummyBall").gameObject.transform;
    }

    void InitSkillstate()
    {
        showstate.Clear();
//        for (int i = 0; i < GameData.Team.Player.SkillCards.Length; i++)
//            if (GameData.DSkillData.ContainsKey(GameData.Team.Player.SkillCards[i].ID))
//            {
//                EPlayerState State = (EPlayerState)System.Enum.Parse(typeof(EPlayerState), GameData.DSkillData[GameData.Team.Player.SkillCards[i].ID].Animation);
//                showstate.Add(State);
//            }

        //Test
        foreach (KeyValuePair<int, TSkillData> item in GameData.DSkillData)
        {
            if (item.Value.Kind < 70 || item.Value.Kind == 180)
//            if (item.Value.Kind == 40)
            {
                foreach (EPlayerState val in Enum.GetValues(typeof(EPlayerState)))
                {
                    if (val.ToString() == item.Value.Animation)
                    {
                        showstate.Add(val);
                    }
                }
            }
        }
    }

    private EPlayerState GetRandomState()
    {
        int random = UnityEngine.Random.Range(0, showstate.Count - 1);
        return showstate[random];
    }

//    private void InitAnmator()
//    {
//        playerControl = mAvatarPlayer.GetComponent<AnimatorBehavior>();
//        if (playerControl == null)
//            playerControl = mAvatarPlayer.AddComponent<AnimatorBehavior>();
//        playerControl.Init(mAvatarPlayer.gameObject.GetComponent<Animator>());
//        playerControl.Play(EPlayerState.Dribble0, 0);
//        CourtMgr.Get.CloneReallBall();
//        CourtMgr.Get.SetBallStateByLobby(EPlayerState.Dribble0, DummyBall);
//
//        if (basket)
//        {
//            if (basket.GetComponent<BasketAnimation>() == null)
//                basket.AddComponent<BasketAnimation>();
//            CourtMgr.Get.ChangeBasketByLobby(basket);
//            CourtMgr.Get.ShootPoint[0] = basket.transform.Find("DummyBasketRoot/DummyBasketIK/DummyBackBoard/DummyHoop").gameObject;
//        }
//
//        playerControl.DunkBasketStartDel = DunkBasketStart;
//        playerControl.AnimationEndDel = AnimatorEnd;
//        playerControl.ShootingDel = OnShooting;
//        playerControl.SkillDel = SkillEventCallBack;
//    }

//    private void DunkBasketStart()
//    {
//        CourtMgr.Get.PlayDunk(0, stateNo);
//    }

//    private void AnimatorEnd()
//    {
//        isReturn = true;
//    }

//    private bool isReturn = false;
//    private bool isRotate = false;
//    private bool isfloor = true;

//    private void Move()
//    {
//        if (isReturn)
//        {
//            mAvatarPlayer.transform.position = Vector3.MoveTowards(mAvatarPlayer.transform.position, 
//                playerCenterPos, 
//                Time.deltaTime * GameConst.DefSpeedup * GameData.Team.Player.Speed * 0.08f);
//
//            if (Vector3.Distance(mAvatarPlayer.transform.position, playerCenterPos) < 3f && isRotate == false)
//            {
//                isRotate = true;
//                mAvatarPlayer.transform.DOLocalRotate(Vector3.zero, 0.4f);
//            }
//
//            if (!isfloor)
//            {
//                if (CourtMgr.Get.RealBall.transform.position.y <= 0)
//                {
//                    if (Vector3.Distance(mAvatarPlayer.transform.position, playerCenterPos) < 1f)
//                    {
//                        Debug.Log("1");
//                        PlayDribble(EPlayerState.Dribble0);
//                        Finish();
//                    }
//                    else
//                    {
//                        Debug.Log("2");
//                        PlayDribble(EPlayerState.Dribble2);
//                    }
//                }
//                else
//                {
//                    if (Vector3.Distance(mAvatarPlayer.transform.position, playerCenterPos) < 0.5f)
//                    {
//                        Debug.Log("3");
//                        PlayDribble(EPlayerState.Dribble0);
//                    }
//                    else
//                    {
//                        Debug.Log("4");
//                        PlayDribble(EPlayerState.Dribble2);
//                    }
//                }  
//            }
//            else
//            {
//                if (Vector3.Distance(mAvatarPlayer.transform.position, playerCenterPos) < 1f)
//                {
//                    Debug.Log("5");
//                    PlayDribble(EPlayerState.Dribble0);
//                    Finish();
//                }
//                else
//                {
//                    Debug.Log("6");
//                    PlayDribble(EPlayerState.Dribble2);
//                }
//            }
//        }
//        else
//        {
//            mAvatarPlayer.transform.LookAt(new Vector3(playerCenterPos.x, mAvatarPlayer.transform.position.y, playerCenterPos.z));
//        }
//    }

//    private void Finish()
//    {
//        isRotate = false;
//        isReturn = false;
//        isfloor = true;
//    }

//    private void PlayDribble(EPlayerState state)
//    {
//        if (crtState != state)
//        {
//            crtState = state;
//            playerControl.Play(crtState, 0);
//            if (state == EPlayerState.Dribble0 || state == EPlayerState.Dribble1 || state == EPlayerState.Dribble2 || state == EPlayerState.Dribble3)
//            {
//                CourtMgr.Get.SetBallStateByLobby(EPlayerState.Dribble0, DummyBall);
//            }
//        }	
//    }

//    private void OnShooting()
//    {
//        isfloor = false;
//        CourtMgr.Get.SetBallStateByLobby(EPlayerState.Shooting, DummyBall);
//
//        CourtMgr.Get.RealBallVelocity = GameFunction.GetVelocity(CourtMgr.Get.RealBall.transform.position, 
//            CourtMgr.Get.ShootPoint[0].transform.position, 60);  
//    }

//    public void SkillEventCallBack(AnimationEvent aniEvent)
//    {
//        switch (aniEvent.stringParameter)
//        {
//            case "Shooting":
//                OnShooting();
//                break;
//        }
//    }
}