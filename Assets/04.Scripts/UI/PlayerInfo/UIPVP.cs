using UnityEngine;
using GameStruct;
using GameItem;

public class PVPPage1TopView
{
    private GameObject self;
    private UISprite PvPRankIcon;
    private UILabel RangeNameLabel;
    private UILabel NowRangeLabel;
    private UIButton MyRankBtn;
    private UILabel Award0;
    private UILabel Award1;

    public void Init(GameObject go)
    {
        if (go)
        {
            self = go;
            PvPRankIcon = self.transform.FindChild("NowRankGroup/PvPRankIcon").gameObject.GetComponent<UISprite>();
            RangeNameLabel = self.transform.FindChild("NowRankGroup/RangeNameLabel").gameObject.GetComponent<UILabel>();
            NowRangeLabel = self.transform.FindChild("NowRankGroup/NowRangeLabel").gameObject.GetComponent<UILabel>();
            MyRankBtn = self.transform.FindChild("MyRankBtn").gameObject.GetComponent<UIButton>();
            Award0 = self.transform.FindChild("AwardGroup/Award0/ValueLabel").gameObject.GetComponent<UILabel>();
            Award1 = self.transform.FindChild("AwardGroup/Award1/ValueLabel").gameObject.GetComponent<UILabel>();
        }
    }

    public bool Enable
    {
        set{ self.SetActive(value);}
        get{ return self.activeSelf;} 
    }

    public void UpdateView()
    {
        
    }
}

public class PVPPage1ListView
{
    private GameObject self;
    private TItemRankGroup myRankInfo;

    public void Init(GameObject go)
    {
        if (go){
            self = go;
            myRankInfo = new TItemRankGroup();
            GameObject myrank = self.transform.FindChild("ItemRankGroup").gameObject;
            myRankInfo.Init(myrank);
            myRankInfo.Enable = false;
        }
    }

    public bool Enable
    {
        set{ self.SetActive(value);}
        get{ return self.activeSelf;} 
    }

    public void UpdateView()
    {

    } 
}

public class PVPPage1
{
    private GameObject self;
    private PVPPage1TopView topView;
    private PVPPage1ListView listView;

    public void Init(GameObject go)
    {
        if (go)
        {
            self = go;

            topView = new PVPPage1TopView();
            listView = new PVPPage1ListView();
            topView.Init(self.transform.Find("TopView").gameObject);
            listView.Init(self.transform.Find("ListView").gameObject);
        }
    }
}

public class UIPVP : UIBase
{
	private static UIPVP instance = null;
	private const string UIName = "UIPVP";

    private int pageCount = 2;
    private UIButton[] tabs; 
    private GameObject[] pages;
    private PVPPage1 page1;
  
    public static bool Visible
    {
        get
        {
            if (instance)
                return instance.gameObject.activeInHierarchy;
            else
                return false;
        }
    }

	public static UIPVP Get
    {
        get
        {
            if (!instance)
				instance = LoadUI(UIName) as UIPVP;
			
            return instance;
        }
    }

    public static void UIShow(bool isShow)
    {
        if (instance)
        {
            if (!isShow)
            {
                RemoveUI(UIName);
            }
            else
            {
                instance.Show(isShow);
            }
        }
        else if (isShow)
        {
            Get.Show(isShow);
        }
    }

    protected override void InitCom()
    {
        tabs = new UIButton[pageCount];
        pages = new GameObject[pageCount];
        page1 = new PVPPage1();

        for (int i = 0; i < pageCount; i++)
        {
            tabs[i] = GameObject.Find(string.Format(UIName + "/Center/Window/Tabs/{0}", i)).GetComponent<UIButton>(); 
            tabs[i].onClick.Add(new EventDelegate(OnPage));
            pages[i] = GameObject.Find(string.Format(UIName + "/Center/Window/Pages/{0}", i)); 

            switch (i)
            {
                case 0:
                    
                    break;
                case 1:
                    page1.Init(pages[i]);
                    break;
            }
        }
            
        SetBtnFun(UIName + "/BottomLeft/BackBtn", OnReturn);
    }

    public void OnPage()
    {
        int index;

        if (int.TryParse(UIButton.current.name, out index))
            DoPage(index);
    }

    private void DoPage(int index)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (index == i)
                pages[i].SetActive(true);
            else
                pages[i].SetActive(false);
        }
    }

    public void OnReturn()
    {
        UIShow(false);
        UIMainLobby.Get.Show();
    }

    protected override void OnShow(bool isShow)
    {
        base.OnShow(isShow);
        if (isShow)
        {
            DoPage(0);
        }
    }
}
