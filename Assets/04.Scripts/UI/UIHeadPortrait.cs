using System.Collections.Generic;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public struct TChangeHeadTextureResult
{
    public int HeadTextureNo;
}

public class ItemHeadBtn
{
    private GameObject item;
    private UISprite SkillIcon;
    private UISprite Selected;
    private UISprite Cover;
    private UIButton btn;
    private bool isInit = false;
    public int ID;

    public void Init(GameObject obj, GameObject parent, EventDelegate btnFunc)
    {
        if (obj && parent)
        {
            item = obj;
            item.transform.parent = parent.transform;
            item.transform.localPosition = Vector3.zero;
            SkillIcon = item.transform.FindChild("SkillIcon").gameObject.GetComponent<UISprite>();
            Selected = item.transform.FindChild("Selected").gameObject.GetComponent<UISprite>();
            Cover = item.transform.FindChild("Cover").gameObject.GetComponent<UISprite>();
            btn = item.GetComponent<UIButton>();

            isInit = SkillIcon && Selected && Cover && btn;

            if (isInit)
            {
                btn.onClick.Add(btnFunc);
            }
            else
            {
                Debug.LogError("InitError");
            }
        }
    }

    public void UpdateView(int id, bool hadItem, bool isEquip)
    {
        ID = id;
        item.name = id.ToString();
        Had = hadItem;
        Equip = isEquip;
        SkillIcon.spriteName = string.Format("{0}s", id);
    }

    public void UpdatePos(int sort)
    {
        if (sort < 3)
            item.transform.localPosition = new Vector3(-270 + (135 * sort), 120, 0);
        else
            item.transform.localPosition = new Vector3(-270 + (135 * ((sort - 3) % 5)), -60 - 140 * (((sort - 3) / 5)), 0);  
    }

    public bool Had
    {
        set{ Cover.enabled = !value; } 
        get{ return !Cover.enabled; }
    }

    public bool Equip
    {
        set{ Selected.enabled = value; }
        get{ return Selected.enabled; }
    }
}

public class UIHeadPortrait : UIBase
{
    private static UIHeadPortrait instance = null;
    private const string UIName = "UIHeadPortrait";
    public Dictionary<int, List<int>> DHeadTexture = new Dictionary<int, List<int>>();
    private ItemHeadBtn[] headTexutres;
    private int equipTextureNo = 0;

    public static bool Visible
    {
        get
        {
            if (instance)
            {
                return instance.gameObject.activeInHierarchy;
            }
            else
                return false;
        }

        set
        {
            if (instance)
            {
                if (!value)
                {
                    RemoveUI(UIName);
                }
                else
                    instance.Show(value);
            }
            else if (value)
                Get.Show(value);
        }
    }

    public static void UIShow(bool isShow)
    {
        if (instance)
        {
			if (!isShow)
					RemoveUI (UIName);
			else {
				instance.Show (isShow);
			}
        }
        else if (isShow)
            Get.Show(isShow);	

        if (isShow)
        {
            Vector3 pos = instance.gameObject.transform.localPosition;
            instance.gameObject.transform.localPosition = new Vector3 (pos.x, pos.y, -10);  
        }
    }

    public static UIHeadPortrait Get
    {
        get
        {
            if (!instance)
                instance = LoadUI(UIName) as UIHeadPortrait;
			
            return instance;
        }
    }

    protected override void InitCom()
    {
        InitHeadTextureData();
        InitHeadBtnComponent();
        SetBtnFun(UIName + "/Window/Center/MainView/NoBtn", OnReturn);
    }

    public void Hide()
    {
        UIShow(false);
    }

    protected override void OnShow(bool isShow)
    {
        base.OnShow(isShow);
    }

    private void InitHeadTextureData()
    {
        for (int i = 0; i < 3; i++)
            if (!DHeadTexture.ContainsKey(i))
                DHeadTexture.Add(i, new List<int>());
			
        int picno = 0;
		
        //SkillHead
        foreach (KeyValuePair<int, TItemData> item in GameData.DItemData)
        {
			if (item.Value.Kind == 21 && GameData.DSkillData.ContainsKey(item.Value.Avatar) && GameData.DSkillData[item.Value.Avatar].Open > 0)
            {
                picno = GameData.DSkillData[item.Value.Avatar].PictureNo;

                if (picno > 3)
                {
                    if (!DHeadTexture.ContainsKey(picno))
                        DHeadTexture.Add(picno, new List<int>());
                    
                    DHeadTexture[picno].Add(item.Value.ID);
                }
            }
        }
    }

    private void InitHeadBtnComponent()
    {
        //key : HeadTextureNo, value : item ids
        GameObject go = Resources.Load("Prefab/UI/Items/SkillHeadBtn")as GameObject;
        GameObject parent = GameObject.Find(UIName + "/Window/Center/MainView/ScrollView");
        int sort = 0;
        int index = GameData.Team.Player.HeadTextureNo == -1 ? GameData.Team.Player.BodyType : GameData.Team.Player.HeadTextureNo;
        equipTextureNo = index;

        if (go && parent)
        {
            headTexutres = new ItemHeadBtn[DHeadTexture.Count];
            foreach (KeyValuePair<int, List<int>> item in DHeadTexture)
            {
                headTexutres[sort] = new ItemHeadBtn();
                headTexutres[sort].Init(Instantiate(go), parent, new EventDelegate(OnSelected));
                headTexutres[sort].UpdateView(item.Key, HasItem(item.Key), index == item.Key);
                headTexutres[sort].UpdatePos(sort);
                sort++;
            }   
        }	
    }

    private bool HasItem(int picNo)
    {
        if (picNo > 2)
        {
            foreach (KeyValuePair<int, int> item in GameData.Team.GotItemCount)
            {
                if (GameData.DItemData.ContainsKey(item.Key))
                if (GameData.DItemData[item.Key].Kind == 21)
                if (GameData.DSkillData.ContainsKey(GameData.DItemData[item.Key].Avatar))//企劃把avatar當作skill item的技能編號
				if (GameData.DSkillData[GameData.DItemData[item.Key].Avatar].PictureNo == picNo)
                    return true;
            }
            return false;   
        }
        else
            return true;
    }

    private void OnSelected()
    {
        int index;
        if (int.TryParse(UIButton.current.name, out index))
        {
            if (index != equipTextureNo)
            {
                if (HasItem(index))
                {
                    for (int i = 0; i < headTexutres.Length; i++)
                    {
                        if (headTexutres[i].ID == index)
                        {
                            headTexutres[i].Equip = true; 
                            equipTextureNo = index;
                        }
                        else
                            headTexutres[i].Equip = false; 
                    }
                }
                else
                {
                    if (DHeadTexture.ContainsKey(index))
                    {
                        if (DHeadTexture[index].Count > 0)
                        {
                            //TODO : 目前家明來源只能顯示單個Data的來源，並無法顯示多個Data來源
                            if (GameData.DItemData.ContainsKey(DHeadTexture[index][0]))
                            {
                                TItemData data = GameData.DItemData[DHeadTexture[index][0]];
								UIItemSource.Get.ShowSkill(data, enable =>
                                    {
                                        if (enable)
                                        {
                                            UIShow(false);
                                            UIHeadPortrait.Visible = false;
                                            UIPlayerInfo.UIShow(false, ref GameData.Team);
                                        }
                                    });
                            }
                        }
                    }
                }
            }    
        }     
    }

    public void OnReturn()
    {
        if (equipTextureNo != GameData.Team.Player.HeadTextureNo && DHeadTexture.ContainsKey(equipTextureNo))
        {
            WWWForm form = new WWWForm();
            form.AddField("HeadTextureNo", equipTextureNo);
            form.AddField("AddIndexs", JsonConvert.SerializeObject(DHeadTexture[equipTextureNo]));
            SendHttp.Get.Command(URLConst.ChangeHeadTexture, waitChangeHeadTexture, form);
        }
        else
            UIShow(false);
    }

    public void waitChangeHeadTexture(bool ok, WWW www)
    {
        if (ok)
        {
			TChangeHeadTextureResult result = JsonConvert.DeserializeObject<TChangeHeadTextureResult>(www.text, SendHttp.Get.JsonSetting);
            GameData.Team.Player.HeadTextureNo = result.HeadTextureNo;
            UIMainLobby.Get.UpdateUI();
            UIPlayerInfo.teamdata = GameData.Team;
            UIPlayerInfo.Get.UpdatePage(0);
        }
        UIShow(false);
    }
}
