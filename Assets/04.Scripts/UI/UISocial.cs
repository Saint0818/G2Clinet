using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GameStruct;
using Newtonsoft.Json;

public class TSocialEventItem{
    public int Index;
    public TFriend Friend;
    public TSocialEvent Event;
    public GameObject Item;
    public GameObject ModelAnchor;
    public GameObject PlayerModel;
    public GameObject UILv;
    public GameObject UIPower;
    public GameObject UIStage;
    public GameObject UIAward;
    public GameObject UISkill;
    public GameObject UIAchievement;
    public GameObject UIAwardName;
    public GameObject UIMessage;
    public ItemAwardGroup AwardGroup;
    public UILabel LabelName;
    public UILabel LabelLv;
    public UILabel LabelPower;
    public UILabel LabelRelation;
    public UILabel LabelAward;
    public UILabel LabelMessage;
    public UIButton ButtonGood;
}

public class UISocial : UIBase {
    private static UISocial instance = null;
    private const string UIName = "UISocial";

    private int nowPage = 0;
    private int nowIndex = 0;
    private const int pageNum = 4;

    private UILabel totalLabel;
    private UILabel labelStats;
    private UILabel labelSearch;
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
        SetBtnFun(UIName + "/Window/Center/Pages/2/SearchBtn", OnSearch);
        SetBtnFun(UIName + "/Window/Center/Pages/2/ResetListGroup/ResetBtn", OnFresh);
        SetBtnFun(UIName + "/Window/BottomLeft/BackBtn", OnClose);

        itemSocialEvent = Resources.Load("Prefab/UI/Items/ItemSocialEvent") as GameObject;
        uiOpation = GameObject.Find(UIName + "/Window/Center/ButtonListGroup");
        uiOpation.SetActive(false);
        totalLabel = GameObject.Find(UIName + "/Window/Center/Total").GetComponent<UILabel>();
        labelSearch = GameObject.Find(UIName + "/Window/Center/Pages/2/SearchArea/TypeLabel").GetComponent<UILabel>();
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
            count = GameData.Team.Friends.Count;
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
            case 0: //event
                for (int i = 0; i < GameData.SocialEvents.Count; i++)
                    if (GameData.SocialEvents[i].Kind == 1 || GameData.Team.Friends.ContainsKey(GameData.SocialEvents[i].TargetID)) {
                        addEvent(page, count, GameData.SocialEvents[i]);
                        count++;
                    }
                
                break;
            case 1: //follow
                if (GameData.Team.Friends != null) {
                    foreach (TFriend item in GameData.Team.Friends.Values) {
                        if (item.Kind == 2 || item.Kind == 3) {
                            addFriend(page, count, item);
                            count++;
                        }
                    }
                }

                break;
            case 2: //advice
                if (GameData.Team.Friends != null) {
                    foreach (TFriend item in GameData.Team.Friends.Values) {
                        if (item.Kind <= 1) {
                            addFriend(page, count, item);
                            count++;
                        }
                    }
                }

                break;
            case 3:
                if (GameData.Team.Friends != null) {
                    foreach (TFriend item in GameData.Team.Friends.Values) {
                        if (item.Kind == 4) {
                            addFriend(page, count, item);
                            count++;
                        }
                    }
                }

                break;
        }
    }
        
    protected override void OnShow(bool isShow) {
        if (isShow) {
            if (DateTime.UtcNow > GameData.Team.FreshFriendTime)
                SendHttp.Get.FreshFriends(waitFreshFriends, false);

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

    private void addItem(int page, int index) {
        if (index >= friendList[page].Count) {
            TSocialEventItem team = new TSocialEventItem();
            team.Item = Instantiate(itemSocialEvent, Vector3.zero, Quaternion.identity) as GameObject;
            string name = page.ToString() + "-" + index.ToString();
            team.Item.name = name;
            team.ModelAnchor = GameObject.Find(name + "/Slot/Anchor");
            team.UILv = GameObject.Find(name + "/Window/Lv");
            team.UIPower = GameObject.Find(name + "/Window/Power");
            team.UIStage = GameObject.Find(name + "/Window/Stage");
            team.UIAward = GameObject.Find(name + "/Window/Item");
            team.UISkill = GameObject.Find(name + "/Window/Skill");
            team.UIAchievement = GameObject.Find(name + "/Window/Achievement");
            team.UIAwardName = GameObject.Find(name + "/Window/AwardName");
            team.UIMessage = GameObject.Find(name + "/Window/Message");
            team.LabelName = GameObject.Find(name + "/Window/Name").GetComponent<UILabel>();
            team.LabelPower = GameObject.Find(name + "/Window/Power").GetComponent<UILabel>();
            team.LabelLv = GameObject.Find(name + "/Window/Lv").GetComponent<UILabel>();
            team.LabelRelation = GameObject.Find(name + "/Window/Good/Label").GetComponent<UILabel>();
            SetLabel(name + "/Window/Power/Label", TextConst.S(3019));
            SetLabel(name + "/Window/Lv/Label", TextConst.S(3761));
            SetBtnFun(name + "/Window/Info", OnInfo);

            GameObject obj = GameObject.Find(name + "/Window/Good");
            if (obj) {
                team.ButtonGood = obj.GetComponent<UIButton>();
                SetBtnFun(ref team.ButtonGood, OnGood);
            }

            team.UILv.SetActive(false);
            team.UIPower.SetActive(false);
            team.UIStage.SetActive(false);
            team.UIAward.SetActive(false);
            team.UISkill.SetActive(false);
            team.UIAchievement.SetActive(false);
            team.UIAwardName.SetActive(false);
            team.UIMessage.SetActive(false);
            team.Item.transform.parent = pageScrollViews[page].gameObject.transform;
            team.Item.transform.localPosition = new Vector3(-370 + index * 330, 77, 0);
            team.Item.transform.localScale = Vector3.one;
            friendList[page].Add(team);
            index = friendList[page].Count-1;
        }
    }

    private void addFriend(int page, int index, TFriend friend) {
        addItem(page, index);
        friendList[page][index].Index = index;

        if (friend.Player.Avatar.HaveAvatar && friendList[page][index].Friend.Identifier != friend.Identifier) {
            friendList[page][index].Friend = friend;
            if (friendList[page][index].PlayerModel)
                Destroy(friendList[page][index].PlayerModel);

            friendList[page][index].PlayerModel = new GameObject("PlayerModel");
            friendList[page][index].PlayerModel.transform.parent = friendList[page][index].ModelAnchor.transform;
            friendList[page][index].PlayerModel.transform.localPosition = Vector3.zero;
            friendList[page][index].PlayerModel.transform.localScale = Vector3.one;
            friendList[page][index].PlayerModel.transform.localRotation = Quaternion.identity;
            modelLoader.Enqueue(friendList[page][index]);
        }

        friendList[page][index].Friend = friend;
        friendList[page][index].Item.SetActive(true);
        friendList[page][index].UILv.SetActive(true);
        friendList[page][index].UIPower.SetActive(true);
        friendList[page][index].LabelName.text = friend.Player.Name;
        friendList[page][index].LabelPower.text = string.Format("{0:F0}",friend.Player.Power());
        friendList[page][index].LabelLv.text = friend.Player.Lv.ToString();
        setGoodSprite(page, friendList[page][index]);
    }

    private void addEvent(int page, int index, TSocialEvent e) {
        addItem(page, index);
        friendList[page][index].Index = index;

        if (e.Player.Avatar.HaveAvatar && friendList[page][index].Event.TargetID != e.TargetID) {
            if (friendList[page][index].PlayerModel)
                Destroy(friendList[page][index].PlayerModel);

            friendList[page][index].PlayerModel = new GameObject("PlayerModel");
            friendList[page][index].PlayerModel.transform.parent = friendList[page][index].ModelAnchor.transform;
            friendList[page][index].PlayerModel.transform.localPosition = Vector3.zero;
            friendList[page][index].PlayerModel.transform.localScale = Vector3.one;
            friendList[page][index].PlayerModel.transform.localRotation = Quaternion.identity;
            modelLoader.Enqueue(friendList[page][index]);
        }

        friendList[page][index].Event = e;
        friendList[page][index].Item.SetActive(true);
        friendList[page][index].UILv.SetActive(true);
        friendList[page][index].UIPower.SetActive(true);
        setGoodSprite(page, friendList[page][index]);
        setEventText(page, index);
    }

    private void setEventText(int page, int index) {
        friendList[page][index].LabelName.text = "";
        TSocialEvent e = friendList[page][index].Event;
        if (page == 0) {
            friendList[page][index].LabelName.text = e.Name;
            switch (e.Kind) {
                case 1: //friend
                    switch (e.Value) {
                        case 2:
                            friendList[page][index].LabelName.text += "\n" + TextConst.S(5029);
                            break;
                        case 3:
                            friendList[page][index].LabelName.text += "\n" + TextConst.S(5024);
                            break;
                        case 4:
                            friendList[page][index].LabelName.text += "\n" + TextConst.S(5030);
                            break;
                    }
                    break;
            }
        }
    }

    private IEnumerator loadModel(TSocialEventItem item) {
        yield return new WaitForSeconds(0.2f);
        ModelManager.Get.SetAvatar(ref item.PlayerModel, item.Friend.Player.Avatar, item.Friend.Player.BodyType, EAnimatorType.TalkControl);
        LayerMgr.Get.SetLayerAllChildren(item.PlayerModel, ELayer.UI.ToString());
    }

    private void waitFreshFriends() {
        initFriendList(nowPage);
    }

    public void OnLink() {
        
    }

    private void waitSearch(bool ok, WWW www) {
        if (ok) {
            if (www.text.Length > 6) {
                if (nowPage == 2) {
                    TFriend friend = JsonConvert.DeserializeObject <TFriend>(www.text, SendHttp.Get.JsonSetting);
                    friend.Player.Init();

                    if (!GameData.Team.Friends.ContainsKey(friend.Identifier)) {
                        GameData.Team.Friends.Add(friend.Identifier, friend);
                        initFriendList(nowPage);
                    }
                }
            } else {
                int index = -1;
                if (int.TryParse(www.text, out index))
                    UIHint.Get.ShowHint(TextConst.S(index), Color.white);
            }
        }
    }

    public void OnSearch() {
        WWWForm form = new WWWForm();
        form.AddField("Name", labelSearch.text);
        SendHttp.Get.Command(URLConst.SearchFriend, waitSearch, form, true);
    }

    public void OnFresh() {
        SendHttp.Get.FreshFriends(waitFreshFriends, true);
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

    private void setGoodSprite(int page, TSocialEventItem item) {
        if (page == 0) {
            if (item.Event.Kind == 1) {
                item.LabelRelation.text = "";
                item.ButtonGood.gameObject.SetActive(false);
            }
        } else {
            if (item.Friend.Kind == 0) {
                item.LabelName.color = Color.yellow;
                item.LabelName.text += "\n" + TextConst.S(5031);
            } else
                item.LabelName.color = Color.white;
            
            switch (item.Friend.Kind) {
                case 0:
                case 1:
                case 3:
                    item.LabelRelation.text = TextConst.S(5023);
                    break;
                case 2:
                    item.LabelRelation.text = TextConst.S(5024);
                    break;
                case 4:
                    item.LabelRelation.text = TextConst.S(5025);
                    break;
            }

            item.ButtonGood.gameObject.SetActive(true);
            if (item.Friend.Kind == 2 || item.Friend.Kind == 4) {
                item.ButtonGood.defaultColor = Color.white;
                item.ButtonGood.hover = Color.white;
                item.ButtonGood.pressed = Color.white;
            } else {
                item.ButtonGood.defaultColor = new Color32(150, 150, 150, 255);
                item.ButtonGood.hover = new Color32(150, 150, 150, 255);
                item.ButtonGood.pressed = new Color32(150, 150, 150, 255);
            }
        }
    }

    private void waitMakeFriend() {
        string id = friendList[nowPage][nowIndex].Friend.Identifier;
        if (GameData.Team.Friends.ContainsKey(id)) {
            friendList[nowPage][nowIndex].Friend = GameData.Team.Friends[id];
            setGoodSprite(nowPage, friendList[nowPage][nowIndex]);
            if (GameData.Team.Friends[id].Kind == 2) {
                UIHint.Get.ShowHint(string.Format(TextConst.S(5027), GameData.Team.Friends[id].Player.Name), Color.white);
                UIHint.Get.ShowHint(TextConst.S(5028), Color.white);
            }
        }
    }

    public void OnGood() {
        Transform obj = UIButton.current.gameObject.transform.parent;
        if (obj && obj.parent) {
            char[] c = {'-'};
            string[] s = obj.parent.name.Split(c, 2);
            if (s.Length == 2) {
                nowPage = -1;
                nowIndex = -1;
                if (int.TryParse(s[0], out nowPage) && int.TryParse(s[1], out nowIndex)) {
                    if (nowPage == 1 || nowPage == 2) {
                        SendHttp.Get.MakeFriend(waitMakeFriend, friendList[nowPage][nowIndex].Friend.Identifier);
                    }
                }
            }
        }
    }

    public void OnInfo() {

    }

    public void FreshSocialEvent() {
        
    }
}
