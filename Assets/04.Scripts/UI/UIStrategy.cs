using UnityEngine;
using System.Collections;

public class UIStrategy : UIBase {
    private static UIStrategy instance = null;
    private const string UIName = "UIStrategy";

    public UILabel LabelStrategy;

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

    public static UIStrategy Get
    {
        get {
            if (!instance) 
                instance = LoadUI(UIName) as UIStrategy;

            return instance;
        }
    }

    protected override void InitCom() {
        SetBtnFun(UIName + "/Window/Center/NoBtn", OnClose);

        for (int i = 0; i < 3; i++)
            SetBtnFun(UIName + "/Window/Center/MainView/" + i.ToString(), OnSelect);
    }

    protected override void InitData() {

    }

    public void OnClose() {
        Visible = false;
    }

    public void OnSelect() {
        int index = -1;
        if (int.TryParse(UIButton.current.name, out index)) {
            GameData.Team.Player.Strategy = index;

            if (index == 0)
                GameData.Team.AttackTactical = ETacticalAuto.AttackNormal;
            else
            if (index == 1)
                GameData.Team.AttackTactical = ETacticalAuto.AttackShoot2;
            else
                GameData.Team.AttackTactical = ETacticalAuto.AttackShoot3;
            
            if (LabelStrategy != null)
                LabelStrategy.text = TextConst.S(15002 + GameData.Team.Player.Strategy);
        }
    }

}
