using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct;

public class TSocialEventItem{
    public int Index;
    public TFriend Data;
    public GameObject Item;
    public GameObject ModelAnchor;
    public GameObject PlayerModel;
    public ItemAwardGroup AwardGroup;
    public UILabel LabelName;
    public UILabel LabelLv;
    public UILabel LabelPower;
}

public class UISocial : UIBase {
    private static UISocial instance = null;
    private const string UIName = "UISocial";

    private int nowPage = 0; 
    private const int pageNum = 4;

    private UILabel totalLabel;
    private UILabel labelStats;
    private GameObject itemSocialEvent;
    private GameObject uiOpation;
    private GameObject[] redPoints = new GameObject[pageNum];
    private GameObject[] pageObjects = new GameObject[pageNum];
    private UIScrollView[] pageScrollViews = new UIScrollView[pageNum];
    private List<TSocialEventItem>[] friendList = new List<TSocialEventItem>[pageNum];
    private Queue<TSocialEventItem> modelLoader = new Queue<TSocialEventItem>();

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

    public static UISocial Get
    {
        get {
            if (!instance) 
                instance = LoadUI(UIName) as UISocial;

            return instance;
        }
    }

    protected override void InitCom() {
        SetBtnFun(UIName + "/Window/Center/Tabs/SocialNetworkBtn", OnLink);
        SetBtnFun(UIName + "/Window/Center/Pages/2/ResetListGroup/ResetBtn", OnFresh);
        SetBtnFun(UIName + "/Window/BottomLeft/BackBtn", OnClose);

        itemSocialEvent = Resources.Load("Prefab/UI/Items/ItemSocialEvent") as GameObject;
        uiOpation = GameObject.Find(UIName + "/Window/Center/ButtonListGroup");
        uiOpation.SetActive(false);
        totalLabel = GameObject.Find(UIName + "/Window/Center/Total").GetComponent<UILabel>();
        for (int i = 0; i < pageNum; i++) {
            redPoints[i] = GameObject.Find(UIName + "/Window/Center/Tabs/" + i.ToString() + "/RedPoint");
            pageObjects[i] = GameObject.Find(UIName + "/Window/Center/Pages/" + i.ToString());
            pageScrollViews[i] = GameObject.Find(UIName + "/Window/Center/Pages/" + i.ToString() + "/ScrollView").GetComponent<UIScrollView>();
            SetBtnFun(UIName + "/Window/Center/Tabs/" + i.ToString(), OnPage);

            redPoints[i].SetActive(false);
            pageObjects[i].SetActive(false);
        }
    }

    protected override void InitData() {
        int count = 0;
        if (GameData.Team.Friends != null) {
            GameData.Team.InitFriends();
            count = GameData.Team.Friends.Length;
        }

        totalLabel.text = count.ToString() + " / 300"; 
    }

    private void initFriendList(int page) {
        if (friendList[page] == null)
            friendList[page] = new List<TSocialEventItem>();

        for (int i = 0; i < friendList[page].Count; i++)
            friendList[page][i].Item.SetActive(false);

        int count = 0;
        switch (page) {
            case 3:
                if (GameData.Team.Friends != null) {
                    for (int i = 0; i < GameData.Team.Friends.Length; i++) {
                        addFriend(page, count, GameData.Team.Friends[i]);
                        count++;
                    }
                }

                break;
        }
    }
        
    protected override void OnShow(bool isShow) {
        if (isShow) {
            for (int i = 0; i < pageObjects.Length; i++)
                pageObjects[i].SetActive(false);
        }

        base.OnShow(isShow);
    }

    public void OnClose() {
        Visible = false;
        UIMainLobby.Get.Show();
    }

    void FixedUpdate() {
        if (modelLoader.Count > 0)
            StartCoroutine(loadModel(modelLoader.Dequeue()));
    }

    private void addFriend(int page, int index, TFriend data) {
        if (index >= friendList[page].Count) {
            TSocialEventItem team = new TSocialEventItem();
            team.Item = Instantiate(itemSocialEvent, Vector3.zero, Quaternion.identity) as GameObject;
            string name = index.ToString();
            team.Item.name = name;
            team.ModelAnchor = GameObject.Find(name + "/Slot/Anchor");
            team.LabelName = GameObject.Find(name + "/Window/Name").GetComponent<UILabel>();
            team.LabelPower = GameObject.Find(name + "/Window/Power").GetComponent<UILabel>();
            team.LabelLv = GameObject.Find(name + "/Window/Lv").GetComponent<UILabel>();
            SetLabel(name + "/Window/Power/Label", TextConst.S(3019));
            SetLabel(name + "/Window/Lv/Label", TextConst.S(3761));

            team.Item.transform.parent = pageScrollViews[page].gameObject.transform;
            team.Item.transform.localPosition = new Vector3(-370 + index * 330, 88, 0);
            team.Item.transform.localScale = Vector3.one;
            friendList[page].Add(team);
            index = friendList[page].Count-1;
        }

        friendList[page][index].Index = index;

        if (data.Player.Avatar.HaveAvatar && friendList[page][index].Data.Identifier != data.Identifier) {
            friendList[page][index].Data = data;
            if (friendList[page][index].PlayerModel)
                Destroy(friendList[page][index].PlayerModel);

            friendList[page][index].PlayerModel = new GameObject("PlayerModel");
            friendList[page][index].PlayerModel.transform.parent = friendList[page][index].ModelAnchor.transform;
            friendList[page][index].PlayerModel.transform.localPosition = Vector3.zero;
            friendList[page][index].PlayerModel.transform.localScale = Vector3.one;
            friendList[page][index].PlayerModel.transform.localRotation = Quaternion.identity;
            modelLoader.Enqueue(friendList[page][index]);
        }

        friendList[page][index].Data = data;
        friendList[page][index].Item.SetActive(true);
        friendList[page][index].LabelName.text = data.Player.Name;
        friendList[page][index].LabelPower.text = string.Format("{0:F1}",data.Player.Power());
        friendList[page][index].LabelLv.text = data.Player.Lv.ToString();
    }

    private IEnumerator loadModel(TSocialEventItem item) {
        yield return new WaitForSeconds(0.2f);
        ModelManager.Get.SetAvatar(ref item.PlayerModel, item.Data.Player.Avatar, item.Data.Player.BodyType, EAnimatorType.TalkControl);
        LayerMgr.Get.SetLayerAllChildren(item.PlayerModel, ELayer.UI.ToString());
    }

    private void waitLookFriend() {
        GameData.Team.InitFriends();
        initFriendList(nowPage);
    }

    public void OnLink() {
        
    }

    public void OnFresh() {
        SendHttp.Get.LookFriends(waitLookFriend, true);
    }

    public void OnPage() {
        for (int i = 0; i < pageObjects.Length; i++)
            pageObjects[i].SetActive(false);

        int index = -1;
        if (int.TryParse(UIButton.current.name, out index)) {
            pageObjects[index].SetActive(true);
            nowPage = index;
            initFriendList(index);
        }
    }
}
