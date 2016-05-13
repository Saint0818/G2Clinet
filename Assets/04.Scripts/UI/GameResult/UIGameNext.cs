using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using GameStruct;
using GameEnum;

public class UIGameNext : UIBase {
    private static UIGameNext instance = null;
    private const string UIName = "UIGameNext";

    private TStageData stageData;
    private GameObject uiBottomRight;
    private UILabel labelNext1;
    private UILabel labelNext2;

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
                    RemoveUI(instance.gameObject);
                else
                    instance.Show(value);
            } else
            if (value)
                Get.Show(value);
        }
    }

    public static UIGameNext Get
    {
        get {
            if (!instance) 
                instance = LoadUI(UIName) as UIGameNext;

            return instance;
        }
    }

    protected override void InitCom() {
        SetBtnFun(UIName + "/BottomRight/Next/Button", OnNext);
        SetBtnFun(UIName + "/BottomLeft/Exit/Button", OnExit);

        uiBottomRight = GameObject.Find(UIName + "/BottomRight");
        labelNext1 = GameObject.Find(UIName + "/BottomRight/Next/Label1").GetComponent<UILabel>();
        labelNext2 = GameObject.Find(UIName + "/BottomRight/Next/Label2").GetComponent<UILabel>();
    }

    protected override void OnShow(bool isShow) {
        base.OnShow(isShow);
    }

    private void initLabel(string text) {
        labelNext1.text = text;
        labelNext2.text = text;
    }

    public void Init(int stageID, bool isWin) {
        Visible = true;
        uiBottomRight.SetActive(true);
        stageData = StageTable.Ins.GetByID(stageID);
        if (stageData.IDKind == TStageData.EKind.PVP || !isWin)
            initLabel(TextConst.S(10117));
        else {
            uiBottomRight.SetActive(true);
            initLabel(TextConst.S(10116));
        }
    }

    private void initEnemy() {
        if (stageData.IDKind == TStageData.EKind.PVP) {
            int num = Mathf.Min(GameData.EnemyMembers.Length, GameData.PVPEnemyMembers.Length);
            for (int i = 0; i < num; i ++) {
                GameData.EnemyMembers[i] = GameData.PVPEnemyMembers[i];
            }
        } else
            if (stageData.FriendKind == 1) {
                int count = 0;
                foreach (KeyValuePair<string, TFriend> item in GameData.Team.Friends) {
                    if (item.Value.Kind == EFriendKind.Advice && item.Value.Identifier != GameData.TeamMembers[1].Identifier && item.Value.Identifier != GameData.TeamMembers[2].Identifier) {
                        GameData.EnemyMembers[count].Player = item.Value.Player;
                        count++;
                        if (count >= GameData.EnemyMembers.Length)
                            break;
                    }
                }

                if (count < 3) {
                    for (int i = 0; i < stageData.PlayerID.Length; i ++) {
                        if (GameData.DPlayers.ContainsKey(stageData.PlayerID[i])) {
                            GameData.EnemyMembers[count].Player.SetID(stageData.PlayerID[i]);
                            GameData.EnemyMembers[count].Player.Name = GameData.DPlayers[stageData.PlayerID[i]].Name;
                            count++;
                            if (count >= GameData.EnemyMembers.Length)
                                break;
                        }
                    }
                }
            } else {
                int num = Mathf.Min(GameData.EnemyMembers.Length, stageData.PlayerID.Length);
                for (int i = 0; i < num; i ++) {
                    if (GameData.DPlayers.ContainsKey(stageData.PlayerID[i])) {
                        GameData.EnemyMembers[i].Player.SetID(stageData.PlayerID[i]);
                        GameData.EnemyMembers[i].Player.Name = GameData.DPlayers[stageData.PlayerID[i]].Name;
                    }
                }
            }
    }

    public void OnNext() {
        if (stageData.IDKind == TStageData.EKind.PVP) {
        } else {
            var protocol = new MainStageStartProtocol();
            protocol.Send(stageData.ID+1, waitMainStageStart);
        }
    }

    public void OnExit() {
        Time.timeScale = 1;
        Visible = false;
        if(GameData.IsMainStage)
        {
            SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
            UILoading.OpenUI = UILoading.OpenStageUI;
        }
        else if(GameData.IsInstance)
        {
            SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
            UILoading.OpenUI = UILoading.OpenInstanceUI;
        }
        else if (GameData.IsPVP)
        {
            SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
            UILoading.OpenUI = UILoading.OpenPVPUI;
        }
        else
        {
            Visible = false;
            SceneMgr.Get.ChangeLevel(ESceneName.Lobby);
            UILoading.OpenUI = UILoading.OpenStageUI;
        }
    }

    private void SendPVPStart()
    {
        WWWForm form = new WWWForm();
        form.AddField("StageID", GameData.StageID);
        SendHttp.Get.Command(URLConst.PVPStart, waitPVPStart, form, true);  
    }

    private void waitPVPStart(bool ok, WWW www)
    {
        if (ok)
        {
            TPVPStart data = JsonConvertWrapper.DeserializeObject <TPVPStart>(www.text);
            GameData.Team.PVPTicket = data.PVPTicket;
            GameData.Team.PVPCD = data.PVPCD;

            GameData.Team.Player.ValueItems = data.Player.ValueItems;
            GameData.Team.Player.ConsumeValueItems = data.Player.ConsumeValueItems;

            Statistic.Ins.LogEvent(17, GameData.Team.Player.Lv.ToString());

            enterGame();
        }
    }

    private void waitMainStageStart(bool ok, MainStageStartProtocol.Data data)
    {
        if(ok) {
            GameData.StageID = stageData.ID+1;
            stageData = StageTable.Ins.GetByID(GameData.StageID);
            enterGame();
        } else
            UIHint.Get.ShowHint(TextConst.S(9514), Color.red);
    }

    private void enterGame() {
        initEnemy();
        UILoading.UIShow(true, ELoading.Game);
        Visible = false;
    }
}
