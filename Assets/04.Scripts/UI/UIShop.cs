using UnityEngine;
using System.Collections;

public class UIShop : UIBase {
    private static UIShop instance = null;
    private const string UIName = "UIShop";

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

    public static UIShop Get
    {
        get {
            if (!instance) 
                instance = LoadUI(UIName) as UIShop;

            return instance;
        }
    }

    protected override void InitCom() {
        SetBtnFun(UIName + "/BottomLeft/BackBtn", OnClose);
    }

    protected override void InitData() {

    }

    protected override void OnShow(bool isShow) {
        base.OnShow(isShow);
    }

    public void OnClose() {
        Visible = false;
        UIMainLobby.Get.Show();
    }
}
