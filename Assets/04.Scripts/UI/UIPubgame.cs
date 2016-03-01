using UnityEngine;
using System.Collections;
using Pubgame;

public class UIPubgame : UIBase {
    private static UIPubgame instance = null;
    private const string UIName = "UIPubgame";
    public delegate void LoginCallback (int resultCode, string playerId, string token);
    public LoginCallback LoginHandle = null;
    private bool eventSwitch = true;

    public static bool Visible {
        get {
            if(instance)
                return instance.gameObject.activeInHierarchy;
            else
                return false;
        }

        set {
            if (instance)
                instance.Show(value);
            else
            if (value)
                Get.Show(value);
        }
    }

    public static UIPubgame Get
    {
        get {
            if (!instance) 
                instance = LoadUI(UIName) as UIPubgame;

            return instance;
        }
    }

    protected override void InitCom() {
        PubgameSdk.Get.InitSDK();

        SetBtnFun(UIName + "/Login", OnLogin);
    }

    protected override void OnShow(bool isShow) {
        base.OnShow(isShow);

        SetLabel(UIName + "/BottomLeft/Label", TextConst.StringFormat (12006, BundleVersion.Version));
    }

    public void OnLogin() {
        Pubgame.PubgameSdk.Get.Login(pubgameLogin);
    }

    public bool OnSwitchWidge() {
        eventSwitch = !eventSwitch;
        Pubgame.PubgameSdk.Get.SetPgToolsActive(eventSwitch);
        return eventSwitch;
    }

    private void pubgameLogin(bool ok, PubgameLoginResponse res) {
        if (ok) {
            if (LoginHandle != null)
                LoginHandle(res.ResultCode, res.PlayerId, res.Token);

            Visible = false;
        }
    }
}
