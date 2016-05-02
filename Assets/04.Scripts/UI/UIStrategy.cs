using UnityEngine;
using System.Collections;

public class UIStrategy : UIBase {
    private static UIStrategy instance = null;
    private const string UIName = "UIStrategy";
    private const int strantegyNum = 3;
    private int mStrategy;

    public UILabel LabelStrategy;
	public UILabel PauseLabelStrategy;
    private UIToggle[] toggleStrantegy = new UIToggle[strantegyNum];

    public static bool Visible {
        get {
            if(instance)
                return instance.gameObject.activeInHierarchy;
            else
                return false;
        }

        set {
            if (instance) {
                if (!value) {
                    if (UIGame.Visible && GameData.Team.Player.Strategy != instance.mStrategy)
                        UIHint.Get.ShowHint(TextConst.S(10211) + TextConst.S(15102 + GameData.Team.Player.Strategy), Color.black);
                    
                    RemoveUI(instance.gameObject);
                } else
                    instance.Show(value);
            } else
            if (value)
                Get.Show(value);

            if (value)
                instance.mStrategy = GameData.Team.Player.Strategy;
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

    protected override void OnShow(bool isShow) {
        base.OnShow(isShow);

        if (isShow) {
            for (int i = 0; i < toggleStrantegy.Length; i++)
                if (i == GameData.Team.Player.Strategy)
                    toggleStrantegy[i].value = true;
                else
                    toggleStrantegy[i].value = false;
        }
    }

    protected override void InitCom() {
        SetBtnFun(UIName + "/Window/Center/NoBtn", onClose);

        for (int i = 0; i < 3; i++)
            SetBtnFun(UIName + "/Window/Center/MainView/" + i.ToString(), OnSelect);

        for (int i = 0; i < toggleStrantegy.Length; i++)
            toggleStrantegy[i] = GameObject.Find(UIName + "/Window/Center/MainView/" + i.ToString()).GetComponent<UIToggle>();
    }

    private void onClose()
    {
        if(GameData.IsPVP)
            Statistic.Ins.LogEvent(14, GameData.Team.AttackTactical.ToString());
        else if(GameData.IsMainStage)
            Statistic.Ins.LogEvent(53, GameData.Team.AttackTactical.ToString());
        else if(GameData.IsInstance)
            Statistic.Ins.LogEvent(102, GameData.Team.AttackTactical.ToString());

        Visible = false;
    }

    public void OnSelect()
    {
        int index;
        if(int.TryParse(UIButton.current.name, out index) && GameData.Team.Player.Strategy != index)
        {
            try
            {
                GameData.Team.Player.Strategy = index;

                if(index == 0)
                    GameData.Team.AttackTactical = ETacticalAuto.AttackNormal;
                else if(index == 1)
                    GameData.Team.AttackTactical = ETacticalAuto.AttackShoot2;
                else
                    GameData.Team.AttackTactical = ETacticalAuto.AttackShoot3;

                if (AIController.Visible)
                {
                    try
                    {
                        AIController.Get.PlayerAttackTactical = GameData.Team.AttackTactical;
                    }
                    catch(System.Exception e)
                    {
                    }
                }

                if(LabelStrategy != null)
                    LabelStrategy.text = GameData.Team.Player.StrategyText;

                if(PauseLabelStrategy != null)
                    PauseLabelStrategy.text = GameData.Team.Player.StrategyText;

                WWWForm form = new WWWForm();
                form.AddField("Strategy", index.ToString());
                SendHttp.Get.Command(URLConst.ChangeStrategy, waitStrantegy, form, false);
            }
            catch(System.Exception e)
            {
            }
        }
    }

    private void waitStrantegy(bool ok, WWW www) {
    }
}
