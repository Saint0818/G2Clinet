using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct;

public class SkillHeadBtn
{
    private GameObject item;
	private UISprite SkillIcon;
    private UISprite Selected;
    private UISprite Cover;
    private UIButton btn;
    private bool isInit = false;

    private void Init(GameObject obj, EventDelegate btnFunc)
    {
        if (obj)
        {
            item = obj;
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

    private void UpdateView(int id, bool hadItme, bool isEquip)
    {
        item.name = id.ToString();
        Had = hadItme;
        Equip = isEquip;
    }

    public bool Had
    {
        set{ Cover.enabled = !value;} 
        get{ return !Cover.enabled;}
    }

    public bool Equip
    {
        set{ Selected.enabled = value;}
        get{return Selected.enabled;}
    }
}

public class BaseHeadBtn
{
    private GameObject item;
    private UISprite icon;
    private UISprite Selected;
    private UISprite Cover;
    private UIButton btn;
    private bool isInit = false;

    private void Init(GameObject obj, EventDelegate btnFunc)
    {
        if (obj)
        {
            item = obj;
            icon = item.transform.FindChild("PlayerIcon").gameObject.GetComponent<UISprite>();
            Selected = item.transform.FindChild("Selected").gameObject.GetComponent<UISprite>();
            btn = item.GetComponent<UIButton>();

            isInit = icon && Selected && Cover && btn;

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

    private void UpdateView(int id, bool isEquip)
    {
        item.name = id.ToString();
        Equip = isEquip;
    }
        
    public bool Equip
    {
        set{ Selected.enabled = value;}
        get{return Selected.enabled;}
    } 
}

public class UIHeadPortrait : UIBase
{
	private static UIHeadPortrait instance = null;
	private const string UIName = "UIHeadPortrait";
	private List<TSkillData> skill = new List<TSkillData>();
    private SkillHeadBtn[] skillHeads;

	public static bool Visible
	{
		get
		{
			if(instance)
				return instance.gameObject.activeInHierarchy;

            return false;
		}
	}
	
	public static void UIShow(bool isShow){
		if(instance) {
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		}
		else
		if(isShow)
			Get.Show(isShow);
	}

	public static UIHeadPortrait Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIHeadPortrait;
			
			return instance;
		}
	}

	protected override void InitCom()
    {
				
    }

	protected override void InitData()
	{
		InitSkillData ();
	}

    public void Hide()
    {
        UIShow(false);
    }

    protected override void OnShow(bool isShow)
    {
        base.OnShow(isShow);
    }

	private void InitSkillData()
	{
		foreach (KeyValuePair<int, TSkillData> item in GameData.DSkillData) {
			if (item.Value.Open > 0) {
				skill.Add (item.Value);
			}
		}

//        if(skill)
//          skillHeads
	}

    void OnDestroy()
    {
        skill = null; 
    }
}
