using GameEnum;
using UnityEngine;

public class UIMainLobbyEvents : MonoBehaviour
{
    public void ShowSettings()
    {
        UISetting.UIShow(true);
    }

    public void ShowAvatarFitted()
    {
        if(isLevelArrive(EOpenUI.Avatar))
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), GameData.DOpenUILv[EOpenUI.Avatar]), Color.white);
        else
        {
            UIAvatarFitted.UIShow(true);
            UIMainLobby.Get.Hide();
        }
    }

    public void ShowStage()
    {
        UIGameLobby.Get.Show();
        UIMainLobby.Get.Hide();
    }

    public void ShowSkillFormation()
    {
        if(isLevelArrive(EOpenUI.Ability))
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), GameData.DOpenUILv[EOpenUI.Ability]), Color.white);
        else
        {
            UISkillFormation.UIShow(true);
            UIMainLobby.Get.Hide();
        }
    }

    public void ShowEquipment()
    {
        if(isLevelArrive(EOpenUI.Equipment))
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), GameData.DOpenUILv[EOpenUI.Equipment]), Color.white);
        else
        {
            UIEquipment.Get.Show();
            UIMainLobby.Get.Hide();
        }
    }

    public void ShowPlayerInfo()
    {
        UIMainLobby.Get.Hide();
        UIPlayerInfo.UIShow(true, ref GameData.Team);
    }

    public void ShowPalyerPotential()
    {
        UIMainLobby.Get.Hide();
        UIPlayerInfo.UIShow(true, ref GameData.Team);
        UIPlayerPotential.UIShow(true);
    }

    public void OnMission()
    {
        if(isLevelArrive(EOpenUI.Mission))
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), GameData.DOpenUILv[EOpenUI.Mission]), Color.white);
        else
        {
            UIMainLobby.Get.Hide();
            UIMission.Visible = true;
        }
    }

    public void OnSocial()
    {
        if (string.IsNullOrEmpty(GameData.Team.Player.Name)) 
            UIHint.Get.ShowHint(TextConst.S(5026), Color.white);
        else
        if(isLevelArrive(EOpenUI.Social))
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), GameData.DOpenUILv[EOpenUI.Social]), Color.white);
        else
        {
            UIMainLobby.Get.Hide();
            UISocial.Visible = true;
        }
    }

    public void OnShop()
    {
        if(isLevelArrive(EOpenUI.Shop))
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), GameData.DOpenUILv[EOpenUI.Shop]), Color.white);
        else
        {
            UIMainLobby.Get.Hide(2);
            UIShop.Visible = true;
        }
    }

    public void OnMall()
    {
        if(isLevelArrive(EOpenUI.Mall))
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), GameData.DOpenUILv[EOpenUI.Mall]), Color.white);
        else
        {
            UIMainLobby.Get.Hide();
            UIMall.Get.Show();
        }
    }

    private static bool isLevelArrive(EOpenUI openUI)
    {
        return GameData.DOpenUILv.ContainsKey(openUI) &&
               GameData.Team.Player.Lv < GameData.DOpenUILv[openUI];
    }

    public void OnBuyCoin()
    {
        UIRecharge.Get.Show(ERechargeType.Coin.GetHashCode());
    }

    public void OnBuyDiamond()
    {
        UIRecharge.Get.Show(ERechargeType.Diamond.GetHashCode());
    }

    public void OnBuyPower()
    {
        UIRecharge.Get.Show(ERechargeType.Power.GetHashCode());
    }
}