using UnityEngine;
using System.Collections;
using Pubgame;

public class UIPubgame : UIBase {
    private static UIPubgame instance = null;
    private const string UIName = "UIPubgame";
    public delegate void LoginCallback (string id);
    public LoginCallback LoginHandle = null;

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
    }

    public void OnLogin() {
        Pubgame.PubgameSdk.Get.Login(pubgameLogin);
    }

    private void pubgameLogin(bool ok, PubgameLoginResponse res) {
        if (ok) {
            if (LoginHandle != null)
                LoginHandle(res.PlayerId);

            Visible = false;
        }
    }
}
