using UnityEngine;
using System;
using System.Collections;
using GameStruct;

public class UIPVPSelectRole : UIBase {
    private static UIPVPSelectRole instance = null;
    private const string UIName = "UIPVPSelectRole";
    private const int playerNum = 6;
    private TStageData stageData;

    public TPlayer [] playerData = new TPlayer[playerNum];
    private UILabel[] labelNames = new UILabel[playerNum];
    private UILabel[] labelPower = new UILabel[playerNum];

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

            UI3DPVP.Visible = value;
            if (value)
                UIMainLobby.Get.HideAll();
        }
    }

    public static UIPVPSelectRole Get
    {
        get {
            if (!instance) 
                instance = LoadUI(UIName) as UIPVPSelectRole;

            return instance;
        }
    }

    protected override void InitCom() {
        SetBtnFun(UIName + "/Left/Back", OnReturn);
        SetBtnFun(UIName + "/Right/GameStart", OnStart);
        for (int i = 0; i < playerNum; i++) {
            labelNames[i] = GameObject.Find(UIName + "/Top/Player" + i.ToString() + "/PlayerName/Label").GetComponent<UILabel>();
            labelPower[i] = GameObject.Find(UIName + "/Top/Player" + i.ToString() + "/CombatPower/Label").GetComponent<UILabel>();
        }
    }

    protected override void InitData() {
        
    }

    protected override void OnShow(bool isShow) {
        base.OnShow(isShow);
    }

    public void OnReturn() {
        Visible = false;
        UIPVP.UIShow(true);
    }

    public void OnStart() {
        Visible = false;
    }
}
