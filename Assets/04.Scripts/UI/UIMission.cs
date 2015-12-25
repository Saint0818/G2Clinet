using UnityEngine;
using System.Collections;

public class UIMission : UIBase {
    private static UIMission instance = null;
    private const string UIName = "UIMission";

    private const int pageNum = 4;
    private GameObject itemMission;
    private UIScrollView[] pageScrollViews = new UIScrollView[pageNum];

    private UILabel totalLabel;
    private GameObject[] redPoints = new GameObject[pageNum];
    private GameObject[] pageObjects = new GameObject[pageNum];

    public static bool Visible {
        get {
            if(instance)
                return instance.gameObject.activeInHierarchy;
            else
                return false;
        }

        set {
            if (instance) {
                if (!value)
                    RemoveUI(UIName);
                else
                    instance.Show(value);
            } else
                if (value)
                    Get.Show(value);
        }
    }

    public static UIMission Get
    {
        get {
            if (!instance) 
                instance = LoadUI(UIName) as UIMission;

            return instance;
        }
    }

    protected override void InitCom() {
        itemMission = Resources.Load("Prefab/UI/Items/ItemMission") as GameObject;
        totalLabel = GameObject.Find(UIName + "/Window/Center/Total").GetComponent<UILabel>();
        for (int i = 0; i < pageNum; i++) {
            redPoints[i] = GameObject.Find(UIName + "/Window/Center/Tabs/" + i.ToString() + "/RedPoint");
            pageObjects[i] = GameObject.Find(UIName + "/Window/Center/Pages/" + i.ToString());
            pageScrollViews[i] = GameObject.Find(UIName + "/Window/Center/Pages/" + i.ToString() + "/ScrollView").GetComponent<UIScrollView>();
            SetBtnFun(UIName + "/Window/Center/Tabs/" + i.ToString(), OnPage);
        }

        SetBtnFun(UIName + "/Window/BottomLeft/BackBtn", OnClose);
    }

    protected override void InitData() {

    }

    protected override void OnShow(bool isShow) {
        base.OnShow(isShow);
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

    public void OnPage() {
        for (int i = 0; i < pageObjects.Length; i++)
            pageObjects[i].SetActive(false);

        int index = -1;
        if (int.TryParse(UIButton.current.name, out index))
            pageObjects[index].SetActive(true);
    }

    public void OnClose() {
        UIShow(false);
        UIMainLobby.Get.Show();
    }
}
