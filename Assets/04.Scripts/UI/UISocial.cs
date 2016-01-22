using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameStruct;
using GameEnum;
using Newtonsoft.Json;

public class EFriendKind {
    public const int Search = 0;
    public const int Advice = 1;
    public const int Follow = 2;
    public const int Waiting = 3;
    public const int Friend = 4;
    public const int Ask = 5;
}

public class TSocialEventItem {
    public int Index;
    public TFriend Friend;
    public TSocialEvent Event;
    public GameObject Item;
    public GameObject ModelAnchor;
    public GameObject PlayerModel;
    public GameObject UICancel;
    public GameObject UILv;
    public GameObject UIPower;
    public GameObject UIStage;
    public GameObject UIAward;
    public GameObject UISkill;
    public GameObject UIMission;
    public ItemAwardGroup AwardGroup;
    public TActiveSkillCard SkillCard;
    public UILabel LabelName;
    public UILabel LabelLv;
    public UILabel LabelPower;
    public UILabel LabelRelation;
    public UILabel LabelTime;
    public UILabel LabelGoodCount;
    public UIButton ButtonGood;
}

public class UISocial : UIBase {
    private static UISocial instance = null;
    private const string UIName = "UISocial";

    private int nowPage = 0;
    private int nowIndex = -1;
    private const int pageNum = 4;
    private TSkill skillData = new TSkill();

    private UILabel totalLabel;
    private UILabel labelStats;
    private UILabel labelSearch;
    private UILabel labelFreshTime;
    private UILabel labelFreshDiamond;
    private GameObject itemSocialEvent;
    private GameObject uiOpation;
    private GameObject[] redPoints = new GameObject[pageNum];
    private GameObject[] pageObjects = new GameObject[pageNum];
    private UIScrollView[] pageScrollViews = new UIScrollView[pageNum];
    private List<TSocialEventItem>[] friendList = new List<TSocialEventItem>[pageNum];
    private Queue<TSocialEventItem> modelLoader = new Queue<TSocialEventItem>();
    private Queue<TSocialEventItem> avatarDownloader = new Queue<TSocialEventItem>();
    private TSocialEventItem waitDownloadItem;

    public static bool Visible {
        get {
            if(instance)
                return instance.gameObject.activeInHierarchy;
            else
                return false;
        }

        set {
            if (instance) {
                //if (!value)
                //    RemoveUI(UIName);
                //else
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
        labelFreshTime = GameObject.Find(UIName + "/Window/Center/Pages/2/ResetListGroup/TextLabel").GetComponent<UILabel>();
        labelFreshDiamond = GameObject.Find(UIName + "/Window/Center/Pages/2/ResetListGroup/ResetBtn/Label").GetComponent<UILabel>();
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
        if (GameData.Team.Friends != null)
            GameData.Team.InitFriends();
    }

    private void initList(int page) {
        if (friendList[page] == null)
            friendList[page] = new List<TSocialEventItem>();

        for (int i = 0; i < friendList[page].Count; i++)
            friendList[page][i].Item.SetActive(false);

        int count = 0;
        switch (page) {
            case 0: //event
                GameData.SocialEvents = GameData.SocialEvents.OrderBy(x => -x.Time.Ticks).ToList();
                int num = Mathf.Min(20, GameData.SocialEvents.Count);
                for (int i = 0; i < num; i++) {
                        addEvent(page, count, GameData.SocialEvents[i]);
                        count++;
                    }
                
                break;
            case 1: //follow
                if (GameData.Team.Friends != null) {
                    GameData.Team.Friends.OrderBy(x => -x.Value.Kind);
                    foreach (TFriend item in GameData.Team.Friends.Values) {
                        if (item.Kind == EFriendKind.Ask || item.Kind == EFriendKind.Follow) {
                            addFriend(page, count, item);
                            count++;
                        }
                    }

                    if (avatarDownloader.Count > 0)
                        StartCoroutine(downloadModel(avatarDownloader.Dequeue()));
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
                        if (item.Kind == EFriendKind.Friend) {
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
            if (DateTime.UtcNow > GameData.Team.FreshFriendTime.ToUniversalTime())
                SendHttp.Get.FreshFriends(waitFreshFriends, false);

            redPoints[0].SetActive(GameData.Setting.ShowEvent);
            redPoints[1].SetActive(GameData.Setting.ShowWatchFriend);
            labelFreshDiamond.text = (50 * (GameData.Team.DailyCount.FreshFriend +1)).ToString();
                
            openPage(nowPage);
        }

        base.OnShow(isShow);
    }
        
    void FixedUpdate() {
        if (modelLoader.Count > 0)
            StartCoroutine(loadModel(modelLoader.Dequeue()));

        if (nowPage == 2) {
            if (GameData.Team.FreshFriendTime.ToUniversalTime() > DateTime.UtcNow)
                labelFreshTime.text = string.Format(TextConst.S(4507), TextConst.DeadlineString(GameData.Team.FreshFriendTime.ToUniversalTime()));
            else {
                labelFreshTime.text = TextConst.S(4510);
                labelFreshDiamond.text = "0";
            }
        }
    }

    private void addItem(int page, int index) {
        if (index >= friendList[page].Count) {
            TSocialEventItem team = new TSocialEventItem();
            team.Item = Instantiate(itemSocialEvent, Vector3.zero, Quaternion.identity) as GameObject;
            string name = page.ToString() + "-" + index.ToString();
            team.Item.name = name;
            team.ModelAnchor = GameObject.Find(name + "/Slot/Anchor");
            team.UICancel = GameObject.Find(name + "/Window/Cancel");
            team.UILv = GameObject.Find(name + "/Window/Lv");
            team.UIPower = GameObject.Find(name + "/Window/Power");
            team.UIStage = GameObject.Find(name + "/Window/Stage");
            team.UISkill = GameObject.Find(name + "/Window/Skill");
            team.UIMission = GameObject.Find(name + "/Window/Mission");
            team.UIAward = GameObject.Find(name + "/Window/Item");
            team.AwardGroup = team.UIAward.GetComponent<ItemAwardGroup>();
            team.SkillCard = new TActiveSkillCard();
            team.SkillCard.Init(team.UISkill, new EventDelegate(OnSkillInfo));
            team.LabelName = GameObject.Find(name + "/Window/Name").GetComponent<UILabel>();
            team.LabelPower = GameObject.Find(name + "/Window/Power").GetComponent<UILabel>();
            team.LabelLv = GameObject.Find(name + "/Window/Lv").GetComponent<UILabel>();
            team.LabelRelation = GameObject.Find(name + "/Window/Good/Label").GetComponent<UILabel>();
            team.LabelGoodCount = GameObject.Find(name + "/Window/Good/Count").GetComponent<UILabel>();
            team.LabelTime = GameObject.Find(name + "/Window/TimeLabel").GetComponent<UILabel>();

            SetLabel(name + "/Window/Power/Label", TextConst.S(3019));
            SetLabel(name + "/Window/Lv/Label", TextConst.S(3761));
            SetLabel(name + "/Window/Cancel/Label", TextConst.S(5033));
            SetBtnFun(name + "/Window/Cancel", OnCancelWatch);
            SetBtnFun(name + "/Window/Info", OnInfo);

            GameObject obj = GameObject.Find(name + "/Window/Good");
            if (obj) {
                team.ButtonGood = obj.GetComponent<UIButton>();
                SetBtnFun(ref team.ButtonGood, OnGood);
            }

            team.UICancel.SetActive(false);
            team.UILv.SetActive(false);
            team.UIPower.SetActive(false);
            team.UIStage.SetActive(false);
            team.UIAward.SetActive(false);
            team.UISkill.SetActive(false);
            team.UIMission.SetActive(false);
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
        } else {
            if (friend.Kind == EFriendKind.Ask) {
                friendList[page][index].Friend = friend;
                if (friendList[page][index].PlayerModel)
                    Destroy(friendList[page][index].PlayerModel);

                friendList[page][index].PlayerModel = new GameObject("PlayerModel");
                friendList[page][index].PlayerModel.transform.parent = friendList[page][index].ModelAnchor.transform;
                friendList[page][index].PlayerModel.transform.localPosition = Vector3.zero;
                friendList[page][index].PlayerModel.transform.localScale = Vector3.one;
                friendList[page][index].PlayerModel.transform.localRotation = Quaternion.identity;
                avatarDownloader.Enqueue(friendList[page][index]);
            }
        }

        friendList[page][index].Friend = friend;
        friendList[page][index].Item.SetActive(true);
        friendList[page][index].UILv.SetActive(true);
        friendList[page][index].UIPower.SetActive(true);
        friendList[page][index].LabelPower.text = string.Format("{0:F0}",friend.Player.CombatPower());
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
        setGoodSprite(page, friendList[page][index]);
        setEventContent(page, index);
    }

    private void setGoodSprite(int page, TSocialEventItem item) {
        item.UICancel.SetActive(false);
        item.LabelTime.text = "";
        item.LabelRelation.text = "";
        item.LabelGoodCount.text = "";
        item.LabelName.color = Color.white;
        if (page == 0) {
            item.LabelTime.text = TextConst.AfterTimeString(item.Event.Time.ToUniversalTime());
            if (item.Event.Good != null)
                item.LabelRelation.text = item.Event.GoodCount.ToString();

            if (item.Event.Good != null && item.Event.Good.ContainsKey(GameData.Team.Identifier) && 
                item.Event.Good[GameData.Team.Identifier] != "") {
                item.ButtonGood.defaultColor = Color.white;
                item.ButtonGood.hover = Color.white;
                item.ButtonGood.pressed = Color.white;
            } else {
                item.ButtonGood.defaultColor = new Color32(150, 150, 150, 255);
                item.ButtonGood.hover = new Color32(150, 150, 150, 255);
                item.ButtonGood.pressed = new Color32(150, 150, 150, 255);
            }
        } else {
            item.LabelName.text = item.Friend.Player.Name;

            switch (item.Friend.Kind) {
                case EFriendKind.Search:
                    item.LabelName.color = Color.yellow;
                    item.LabelName.text += "\n" + TextConst.S(5031);
                    break;
                case EFriendKind.Advice:
                    item.LabelRelation.text = TextConst.S(5023);
                    break;
                case EFriendKind.Follow:
                    item.LabelRelation.text = TextConst.S(5024);
                    break;
                case EFriendKind.Ask:
                    item.LabelTime.text = TextConst.AfterTimeString(item.Friend.Time.ToUniversalTime());
                    item.LabelRelation.text = TextConst.S(5023);
                    item.LabelName.text += "\n" + TextConst.S(5032);
                    item.UICancel.SetActive(true);
                    break;
                case EFriendKind.Friend:
                    item.LabelRelation.text = TextConst.S(5025);
                    break;
            }

            item.ButtonGood.gameObject.SetActive(true);
            if (item.Friend.Kind == EFriendKind.Follow || item.Friend.Kind == EFriendKind.Friend) {
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

    private void setEventContent(int page, int index) {
        friendList[page][index].UIStage.SetActive(false);
        friendList[page][index].UIAward.SetActive(false);
        friendList[page][index].UISkill.SetActive(false);
        friendList[page][index].UIMission.SetActive(false);
        friendList[page][index].LabelName.text = "";
        friendList[page][index].LabelRelation.text = "";

        TSocialEvent e = friendList[page][index].Event;
        if (page == 0) {
            friendList[page][index].LabelName.text = e.Player.Name + TextConst.GetSocialText(e);
            switch (e.Kind) {
                case 3: //mission
                    if (GameData.DMissionData.ContainsKey(e.Value)) {
                        int itemID = 0;
                        if (GameData.DMissionData[e.Value].AwardID != null && GameData.DMissionData[e.Value].AwardID.Length > 0) {
                            itemID = GameData.DMissionData[e.Value].AwardID[GameData.DMissionData[e.Value].AwardID.Length-1];
                            setItemPic(itemID, friendList[page][index]);
                        }
                    }

                    break;
                case 4: //item
                    setItemPic(e.Value, friendList[page][index]);

                    break;
            }
        }
    }

    private void setItemPic(int id, TSocialEventItem itemObj) {
        if (GameData.DItemData.ContainsKey(id)) {
            if (GameData.DItemData[id].Kind == 21 && GameData.DSkillData.ContainsKey(GameData.DItemData[id].Avatar)) {
                skillData.ID = GameData.DItemData[id].Avatar;
                skillData.Lv = GameData.DItemData[id].LV;
                itemObj.UISkill.SetActive(true);
                itemObj.SkillCard.UpdateView(itemObj.Index, skillData);
            } else {
                itemObj.UIAward.SetActive(true);
                itemObj.AwardGroup.Show(GameData.DItemData[id]);
            }
        }
    }

    private IEnumerator loadModel(TSocialEventItem item) {
        yield return new WaitForSeconds(0.2f);
        ModelManager.Get.SetAvatar(ref item.PlayerModel, item.Friend.Player.Avatar, item.Friend.Player.BodyType, EAnimatorType.TalkControl);
        LayerMgr.Get.SetLayerAllChildren(item.PlayerModel, ELayer.UI.ToString());
    }
        
    private IEnumerator downloadModel(TSocialEventItem item) {
        yield return new WaitForSeconds(0.2f);

        if (!string.IsNullOrEmpty(item.Friend.Identifier)) {
            waitDownloadItem = item;
            WWWForm form = new WWWForm();
            form.AddField("Identifier", item.Friend.Identifier);
            SendHttp.Get.Command(URLConst.LookAvatar, waitLookAvatar, form, false);
        } else
            item = null;
    }
        
    private void waitFreshFriends() {
        labelFreshDiamond.text = (50 * (GameData.Team.DailyCount.FreshFriend +1)).ToString();
        initList(nowPage);
    }

    private void waitLookAvatar(bool ok, WWW www) {
        if (ok) {
            TFriend friend = JsonConvert.DeserializeObject <TFriend>(www.text, SendHttp.Get.JsonSetting);
            friend.Player.Init();
            if (GameData.Team.Friends.ContainsKey(friend.Identifier)) {
                friend.Kind = GameData.Team.Friends[friend.Identifier].Kind;
                GameData.Team.Friends[friend.Identifier] = friend;
            }

            if (waitDownloadItem != null && waitDownloadItem.Friend.Identifier == friend.Identifier) {
                friend.Kind = waitDownloadItem.Friend.Kind;
                waitDownloadItem.Friend = friend;
                ModelManager.Get.SetAvatar(ref waitDownloadItem.PlayerModel, friend.Player.Avatar, friend.Player.BodyType, EAnimatorType.TalkControl);
                LayerMgr.Get.SetLayerAllChildren(waitDownloadItem.PlayerModel, ELayer.UI.ToString());
            }
        }

        if (avatarDownloader.Count > 0)
            StartCoroutine(downloadModel(avatarDownloader.Dequeue()));
    }

    private void waitSearch(bool ok, WWW www) {
        if (ok) {
            if (SendHttp.Get.CheckServerMessage(www.text)) {
                if (nowPage == 2) {
                    TFriend friend = JsonConvert.DeserializeObject <TFriend>(www.text, SendHttp.Get.JsonSetting);
                    friend.Player.Init();

                    if (!GameData.Team.Friends.ContainsKey(friend.Identifier)) {
                        GameData.Team.Friends.Add(friend.Identifier, friend);
                        initList(nowPage);
                    }
                }
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

    private void waitConfirm(bool ok, WWW www) {
        if (ok) {
            if (SendHttp.Get.CheckServerMessage(www.text)) {
                TFriend friend = JsonConvert.DeserializeObject <TFriend>(www.text, SendHttp.Get.JsonSetting);

                if (friend.Kind == EFriendKind.Friend) {
                    GameData.Team.LifetimeRecord.FriendCount++;
                    friend.Player.Init();
                    if (GameData.Team.Friends.ContainsKey(friend.Identifier))
                        GameData.Team.Friends[friend.Identifier] = friend;
                    else
                        GameData.Team.Friends.Add(friend.Identifier, friend);

                    UIHint.Get.ShowHint(string.Format(TextConst.S(5035), friend.Player.Name), Color.white);
                } else 
                if (GameData.Team.Friends.ContainsKey(friend.Identifier))
                    GameData.Team.Friends.Remove(friend.Identifier);

                if (friendList[nowPage][nowIndex].Friend.Identifier == friend.Identifier) 
                    initList(nowPage);
            } else {
                if (GameData.Team.Friends.ContainsKey(friendList[nowPage][nowIndex].Friend.Identifier))
                    GameData.Team.Friends.Remove(friendList[nowPage][nowIndex].Friend.Identifier);

                if (nowPage == 1) 
                    initList(nowPage);
            }
        }
    }

    private void waitRemoveFriend(bool ok, WWW www) {
        if (ok) {
            if (www.text == "1") {
                GameData.Team.LifetimeRecord.FriendCount--;
                string id = friendList[nowPage][nowIndex].Friend.Identifier;
                if (GameData.Team.Friends.ContainsKey(id))
                    GameData.Team.Friends.Remove(id);
                
                initList(nowPage);
            } else
                SendHttp.Get.CheckServerMessage(www.text);
        }
    }

    private void waitGood(bool ok, WWW www) {
        if (ok) {
            TSocialEvent result = JsonConvert.DeserializeObject<TSocialEvent>(www.text, SendHttp.Get.JsonSetting);
            GameData.Team.LifetimeRecord.GoodCount = result.GoodCount;
            if (result.Good != null) {
                friendList[nowPage][nowIndex].Event.Good = result.Good;
                for (int i = 0; i < GameData.SocialEvents.Count; i++)
                    if (GameData.SocialEvents[i]._id == friendList[nowPage][nowIndex].Event._id)
                        GameData.SocialEvents[i] = friendList[nowPage][nowIndex].Event;

                setGoodSprite(nowPage, friendList[nowPage][nowIndex]);
            }
        }
    }

    public void OnClose() {
        Visible = false;
        UIMainLobby.Get.Show();
    }

    public void OnLink() {

    }

    public void OnSearch() {
        WWWForm form = new WWWForm();
        form.AddField("Name", labelSearch.text);
        SendHttp.Get.Command(URLConst.SearchFriend, waitSearch, form, true);
    }

    private void doFreshFriend() {
        SendHttp.Get.FreshFriends(waitFreshFriends, true);
    }

    public void OnFresh() {
        int diamond = 0;
        if (GameData.Team.FreshFriendTime.ToUniversalTime() > DateTime.UtcNow)
            diamond = 50 * (GameData.Team.DailyCount.FreshFriend +1);

        CheckDiamond(diamond, true, TextConst.S(4511) + diamond.ToString(), doFreshFriend);
    }

    private void openPage(int page) {
        for (int i = 0; i < pageObjects.Length; i++)
            pageObjects[i].SetActive(false);

        totalLabel.text = GameData.Team.LifetimeRecord.FriendCount.ToString() + " / 300"; 
        pageObjects[page].SetActive(true);
        redPoints[page].SetActive(false);
        nowPage = page;
        initList(page);

        if (page == 0 || page == 1) {
            int flag = 0;
            if (page == 0) {
                GameData.Setting.SocialEventTime = DateTime.UtcNow;
                PlayerPrefs.SetString(ESave.SocialEventTime.ToString(), DateTime.UtcNow.ToString());
                if (GameData.Setting.ShowEvent) {
                    GameData.Setting.ShowEvent = false;
                    flag = GameData.Setting.ShowEvent == true ? 1 : 0;
                    PlayerPrefs.SetInt(ESave.ShowEvent.ToString(), flag);
                }

                PlayerPrefs.Save();
            } else {
                GameData.Setting.WatchFriendTime = DateTime.UtcNow;
                PlayerPrefs.SetString(ESave.WatchFriendTime.ToString(), DateTime.UtcNow.ToString());
                if (GameData.Setting.ShowWatchFriend) {
                    GameData.Setting.ShowWatchFriend = false;
                    flag = GameData.Setting.ShowWatchFriend == true ? 1 : 0;
                    PlayerPrefs.SetInt(ESave.ShowWatchFriend.ToString(), flag);
                }

                PlayerPrefs.Save();
            }
        }
    }

    public void OnPage() {
        int index = -1;
        if (int.TryParse(UIButton.current.name, out index))
            openPage(index);
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
                    WWWForm form = new WWWForm();
                    switch (nowPage) {
                        case 0:
                            form.AddField("Identifier", SystemInfo.deviceUniqueIdentifier);
                            form.AddField("Name", GameData.Team.Player.Name);
                            form.AddField("_id", friendList[nowPage][nowIndex].Event._id);
                            SendHttp.Get.Command(URLConst.Good, waitGood, form);

                            break;
                        case 1:
                        case 2:
                            if (friendList[nowPage][nowIndex].Friend.Kind == EFriendKind.Ask) {
                                form.AddField("Identifier", SystemInfo.deviceUniqueIdentifier);
                                form.AddField("FriendID", friendList[nowPage][nowIndex].Friend.Identifier);
                                form.AddField("Name", GameData.Team.Player.Name);
                                form.AddField("Ask", "1");
                                SendHttp.Get.Command(URLConst.ConfirmMakeFriend, waitConfirm, form);
                            } else
                                SendHttp.Get.MakeFriend(waitMakeFriend, friendList[nowPage][nowIndex].Friend.Identifier);
                            
                            break;
                        case 3:
                            form.AddField("Identifier", SystemInfo.deviceUniqueIdentifier);
                            form.AddField("FriendID", friendList[nowPage][nowIndex].Friend.Identifier);
                            SendHttp.Get.Command(URLConst.RemoveFriend, waitRemoveFriend, form);
                            break;
                    }
                }
            }
        }
    }

    public void OnCancelWatch() {
        Transform obj = UIButton.current.gameObject.transform.parent;
        if (obj && obj.parent) {
            char[] c = {'-'};
            string[] s = obj.parent.name.Split(c, 2);
            if (s.Length == 2) {
                nowPage = -1;
                nowIndex = -1;
                if (int.TryParse(s[0], out nowPage) && int.TryParse(s[1], out nowIndex)) {
                    if (nowPage == 1 && friendList[nowPage][nowIndex].Friend.Kind == 5) {
                        WWWForm form = new WWWForm();
                        form.AddField("Identifier", SystemInfo.deviceUniqueIdentifier);
                        form.AddField("FriendID", friendList[nowPage][nowIndex].Friend.Identifier);
                        form.AddField("Name", GameData.Team.Player.Name);
                        form.AddField("Ask", "0");
                        SendHttp.Get.Command(URLConst.ConfirmMakeFriend, waitConfirm, form);
                    }
                }
            }
        }
    }

    public void OnSkillInfo() {
        int index = -1;
        if (int.TryParse(UIButton.current.name, out index) && index >= -1 && index < friendList[0].Count)
            UIItemHint.Get.OnShowSkill(friendList[0][index].SkillCard.Skill);
    }

    public void OnInfo() {

    }

    public void FreshSocialEvent(int page) {
        if (nowPage == page)
            initList(page);
    }

    public void FreshFriend(int page) {
        if (nowPage == page)
            initList(page);
    }
}
