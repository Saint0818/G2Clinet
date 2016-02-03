using System;
using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

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
	private const int BuildCount = 10;
	public GameObject[] BuildPos = new GameObject[BuildCount];
	public GameObject[] Builds = new GameObject[BuildCount];
	private Animator animator;
	private UIButton[] Btns = new UIButton[BuildCount];
	private GameObject advertisementPic;
    private GameObject mAvatarPlayer;
	private AnimatorBehavior aniBehavior;
    private EPlayerState crtState;
    private int selectIndex = -1;
    private float delay = 0;

	private List<EPlayerState> showstate = new List<EPlayerState>();

    [UsedImplicitly]
    private void Awake()
    {
		if (PrefabSettingIsLegal ()) {
			int[] temp = new int[BuildCount];
			for(int i = 0;i < temp.Length;i++){
				if(i == 0)
					temp[i] = -1;
				else
					temp[i] = 101;
			}
			//TODO:Read Server Data
			InitBuilds(temp);
			InitButtons();

			if(advertisementPic == null){
				advertisementPic = Instantiate(Resources.Load("Prefab/Stadium/StadiumItem/AdvertisementPic")) as GameObject;
				advertisementPic.transform.parent = BuildPos[2].transform;
				advertisementPic.transform.localPosition = Vector3.zero;
				advertisementPic.transform.localScale = Vector3.one;
				advertisementPic.transform.localEulerAngles = Vector3.zero;
			}

		}
		else
			Debug.LogError ("Setting Prefab Error");
    }

	private void InitButtons()
	{
		for (int i = 0; i< BuildPos.Length; i++) {
			Btns[i] = BuildPos[i].transform.parent.GetComponent<UIButton>();
			BuildPos[i].transform.parent.name = i.ToString();
			if(Btns[i])
				Btns[i].onClick.Add(new EventDelegate(OnSelect));
		}
	}


    [UsedImplicitly]
	private void Update()
	{
		if(delay > 0)
			delay -= Time.deltaTime;
	}

	private void OnSelect()
		{
//		//TODO: it's not open yet.
//		return;
		if (delay > 0)
			return;

		int index;

		if (!UITutorial.Visible && int.TryParse (UIButton.current.name, out index)) {
			if(selectIndex == index){
				//back 
				selectIndex = -1;
				SetAnimator(index, false);
				UpdateButtonCollider(index, true);
                UIMainLobby.Get.Main.PlayEnterAnimation();
				delay = 1;
			}else{
				//go
				if(selectIndex == -1){
                    if (index == 0)
                    {
                        DoAni();
                    }
                    else
                    {
                        selectIndex = index;
                        SetAnimator(index, true);
                        UpdateButtonCollider(index, false);
                        UIMainLobby.Get.Main.PlayExitAnimation();
                        delay = 1;
                    }
				}
			}
            AudioMgr.Get.PlaySound(SoundType.SD_LobbyCamara);
		}
	}

    private void DoAni()
    {
        if (GameData.Team.Player.SkillCards.Length > 0)
        {
            crtState = GetRandomState();
            aniBehavior.AddTrigger(crtState, 0); 

        }
    }

	private void UpdateButtonCollider(int index, bool isopen)
	{
		for (int i = 0; i < BuildPos.Length; i++) {
			if(isopen)
				BuildPos[i].transform.parent.transform.GetComponent<BoxCollider>().enabled = true;
			else{
				if(i != index)
					BuildPos[i].transform.parent.transform.GetComponent<BoxCollider>().enabled = false;
			}
		}
	}

	private string GetEBildsTypeString(int index)
	{
		return ((EBildsType)index).ToString ();
	}

	private int GetEBildsTypeCount()
	{
		return Enum.GetNames (typeof(EBildsType)).Length;
	}

	private void SetAnimator(int index, bool isgo)
	{
		if (index >= 0 && index < GetEBildsTypeCount())
		{
			string state = (isgo == true ? "Go" : "Back");
			string eventName = string.Format("Event{0}_{1}", GetEBildsTypeString(index), state);
			animator.SetTrigger(eventName);
		}
		else
			Debug.LogError("Animator Index Error");
	}

	private void InitBuilds(int[] buids)
	{
		if (buids.Length != BuildCount)
			return;
		else{
			for(int i = 0;i < Builds.Length;i++){
				CloneObj(ref Builds[i], i, buids[i]);
			}
		}
	}

	public void CloneObj(ref GameObject clone, int index, int id)
	{
		if(id > 0){
			string name = GetEBildsTypeString(index) + id.ToString();
			string path = string.Format("Prefab/Stadium/StadiumItem/{0}",name);
			GameObject obj;

			if(clone && name == clone.name)
				return;

			if (clone) {
				Destroy(clone);
				clone = null;
			}

			obj = Resources.Load(path) as GameObject;
			if(obj)
				clone = Instantiate(obj) as GameObject;
			else
				Debug.LogError("Can't found GameObject in Resource : " + path);
			
			if (clone && index < BuildPos.Length){
				clone.transform.parent = BuildPos [index].transform;
				clone.transform.localPosition = Vector3.zero;
				clone.transform.localScale = Vector3.one;
				clone.transform.localEulerAngles = Vector3.zero;
				clone.name = name;
			}
		}else{
			if(clone)
			{
				Destroy(clone);
				clone = null;
			}
		}
	}

	public bool PrefabSettingIsLegal()
	{
		if(GetEBildsTypeCount() == BuildPos.Length){
			for (int i = 0; i < BuildPos.Length; i++) {
				string name = string.Format("{0}Pos", ((EBildsType)i).ToString());
				if(BuildPos[i].name != name)
					return false;
			}
		}
		else{
			return false;
		}

		return true;
	}

    public void Show()
    {
        UpdateAvatar();
        InitSkillstate();
    }

    public void Hide(){}

    public void UpdateAvatar()
    {
        if(mAvatarPlayer)
            Destroy(mAvatarPlayer);

        mAvatarPlayer = new GameObject { name = "LobbyAvatarPlayer" };
        ModelManager.Get.SetAvatar(ref mAvatarPlayer, GameData.Team.Player.Avatar, 
                                   GameData.Team.Player.BodyType,
                                   EAnimatorType.AnimationControl, false);

        mAvatarPlayer.transform.parent = BuildPos[0].transform;
        mAvatarPlayer.transform.localPosition = Vector3.zero;
        mAvatarPlayer.transform.localScale = Vector3.one;
        mAvatarPlayer.transform.localRotation = Quaternion.identity;

        animator = mAvatarPlayer.gameObject.GetComponent<Animator>();

		aniBehavior = mAvatarPlayer.GetComponent<AnimatorBehavior> ();
        if (aniBehavior == null)
        {
            aniBehavior = mAvatarPlayer.AddComponent<AnimatorBehavior>();
           
        }
        CourtMgr.Get.DunkPoint[0] = BuildPos[1].gameObject;
        aniBehavior.Init(animator);
    }
	
	void InitSkillstate()
    {
        showstate.Clear();
        for (int i = 0; i < GameData.Team.Player.SkillCards.Length; i++)
            if (GameData.DSkillData.ContainsKey(GameData.Team.Player.SkillCards[i].ID))
            {
                EPlayerState State = (EPlayerState)System.Enum.Parse(typeof(EPlayerState), GameData.DSkillData[GameData.Team.Player.SkillCards[i].ID].Animation);
                showstate.Add(State);
            }
    }

    private EPlayerState GetRandomState()
    {
        int random = UnityEngine.Random.Range(0, showstate.Count - 1);
        return showstate[random];
    }

//    private OnShooting()
//    {
//        CourtMgr.Get.RealBallVelocity = GameFunction.GetVelocity(CourtMgr.Get.RealBall.transform.position, 
//            CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position , shootAngle, 1f);
//
//
//        animator.SetTrigger(animationName);
//    }
}