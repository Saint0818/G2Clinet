using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameEnum;
using GameStruct;

public class TMember{
	public string TeamName;
	public int Index;
	public TPlayer Player;
	public GameObject Item;
    public GameObject UISelected;
    public UILabel LabelSelected;
	public UILabel LabelTeamName;
	public UILabel LabelPower;
	public UISprite SpriteFace;
}

public class UISelectPartner : UIBase {
	private static UISelectPartner instance = null;
	private const string UIName = "UISelectPartner";

	private List<TMember> memberList = new List<TMember>();
	private List<TPassiveSkillCard> skillList = new List<TPassiveSkillCard>();
	private GameObject itemMember;
	private GameObject itemSkill;
	private UIScrollView partnerScrollView;
	private UIScrollView skillScrollView;
	private int selectIndex;

	public static bool Visible
	{
		get
		{
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static void UIShow(bool isShow){
		if(instance) {
			if (!isShow) 
                Get.Show(isShow);
			else
				instance.Show(isShow);
		} else
		if(isShow)
			Get.Show(isShow);
	}
	
	public static UISelectPartner Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISelectPartner;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
        SetBtnFun (UIName + "/Right/Window/Close", OnClose);
		itemMember = Resources.Load("Prefab/UI/Items/ItemSelectPartner") as GameObject;
		itemSkill = Resources.Load("Prefab/UI/Items/ItemCardEquipped") as GameObject;
		partnerScrollView = GameObject.Find(UIName + "/Right/Window/Partner/ScrollView").GetComponent<UIScrollView>();
        skillScrollView = GameObject.Find(UIName + "/Right/Window/Skill/ScrollView").GetComponent<UIScrollView>();
	}

	public void OnClose(){
        UISelectRole.Get.InitPlayer();
		UIShow(false);
	}

	private void initSkillList(int index) {
		for (int i = 0; i < skillList.Count; i++)
			skillList[i].Enable = false;

		for (int i = 0; i < memberList[index].Player.SkillCards.Length; i++)
			addSkillItem(i, memberList[index].Player.SkillCards[i]); 
	}

	private void addSkillItem(int index, TSkill skill) {
		if (index >= skillList.Count) {
			TPassiveSkillCard sc = new TPassiveSkillCard();
			GameObject obj = Instantiate(itemSkill) as GameObject;
			sc.Init(skillScrollView.gameObject, obj, new EventDelegate(OnSkillHint));

			obj.name = skill.ID.ToString();
			LayerMgr.Get.SetLayer(obj, ELayer.TopUI);
			obj.GetComponent<UIDragScrollView>().scrollView = skillScrollView;
			skillList.Add(sc);
		}

		skillList[index].Enable = true;
		skillList[index].UpdateView(index, skill, new Vector3(0, 145-index * 70, 0));
	}

	public void OnSkillHint() {
		int index = -1;
		if (int.TryParse(UIButton.current.name, out index) && index >= -1 && index < skillList.Count)
			UIItemHint.Get.OnShowSkill(skillList[index].Skill);
	}

	public void InitMemberList(ref List<TPlayer> playerList, ref TPlayer[] selectAy, int index) {
		selectIndex = index;
		for (int i = 0; i < memberList.Count; i ++) {
            memberList[i].LabelSelected.text = "";
            memberList[i].UISelected.SetActive(false);
			memberList[i].Item.SetActive(false);
		}
		
		for (int i = 0; i < playerList.Count; i++)
			addMember(i, playerList[i]);
		
        for (int i = 0; i < memberList.Count; i ++) {
            memberList[i].LabelSelected.text = "";
            memberList[i].UISelected.SetActive(false);
        }

		for (int j = 0; j < selectAy.Length; j++) 
            if (selectAy[j].RoleIndex >= 0 && selectAy[j].RoleIndex < memberList.Count) {
                memberList[selectAy[j].RoleIndex].UISelected.SetActive(true);
                memberList[selectAy[j].RoleIndex].LabelSelected.text = TextConst.S(9511+j);
            }
        
		if (index >= 0 && index < selectAy.Length)
			initSkillList(selectAy[index].RoleIndex);
	}

	private void addMember(int index, TPlayer player) {
		if (index >= memberList.Count) {
			TMember team = new TMember();
			team.Item = Instantiate(itemMember, Vector3.zero, Quaternion.identity) as GameObject;
			team.Item.name = index.ToString();
			UIButton btn = team.Item.GetComponent<UIButton>();
			SetBtnFun(ref btn, OnSelectPartner);
			team.Item.GetComponent<UIDragScrollView>().scrollView = partnerScrollView;
			team.LabelTeamName = GameObject.Find(team.Item.name + "/PlayerName/NameLabel").GetComponent<UILabel>();
			team.LabelPower = GameObject.Find(team.Item.name + "/CombatGroup/CombatValueLabel").GetComponent<UILabel>();
			team.SpriteFace = GameObject.Find(team.Item.name + "/PlayerInGameBtn").GetComponent<UISprite>();
            team.UISelected = GameObject.Find(team.Item.name + "/UISelected");
            team.LabelSelected = GameObject.Find(team.Item.name + "/UISelected/Label").GetComponent<UILabel>();
			GameObject obj = GameObject.Find(team.Item.name + "/UISelected/Label");
			if (obj) {
				UILabel lab = obj.GetComponent<UILabel>();
				if (lab)
					lab.text = TextConst.S(9510);
			}

			team.Item.transform.parent = partnerScrollView.gameObject.transform;
			team.Item.transform.localPosition = new Vector3(0, 120 - index * 130, 0);
			team.Item.transform.localScale = Vector3.one;
			memberList.Add(team);
			index = memberList.Count-1;
		}
		
		memberList[index].Index = index;
		memberList[index].Player = player;
		
		memberList[index].Item.SetActive(true);
        memberList[index].LabelSelected.text = "";
        memberList[index].UISelected.SetActive(false);
		memberList[index].LabelTeamName.text = player.Name;
		memberList[index].LabelPower.text = string.Format(TextConst.S(9509), + player.Power());
		memberList[index].SpriteFace.spriteName = player.FacePicture; 
	}

	public void OnSelectPartner() {
		if (UISelectRole.Visible) {
			int index = -1;
			if (int.TryParse(UIButton.current.name, out index)) {
				int i = UISelectRole.Get.GetSelectedListIndex(selectIndex);
				int j = -1;
				if (selectIndex == 1)
				 	j = UISelectRole.Get.GetSelectedListIndex(2);
				else
					j = UISelectRole.Get.GetSelectedListIndex(1);

				if (i != index && j != index) {
                    if (i >= 0 && i < memberList.Count) {
                        memberList[i].LabelSelected.text = "";
                        memberList[i].UISelected.SetActive(false);
                    }

                    if (index >= 0 && index < memberList.Count) {
                        memberList[index].UISelected.SetActive(true);
                        memberList[index].LabelSelected.text = TextConst.S(9511+selectIndex);
                    }

					UISelectRole.Get.SetPlayerAvatar(selectIndex, index);
				}
				
				initSkillList(index);
			}
		}
	}
}

