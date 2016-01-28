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
        if(GameData.IsOpenUIEnable(EOpenUI.Avatar))
        {
            UIAvatarFitted.UIShow(true);
            UIMainLobby.Get.Hide();
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), GameData.DOpenUILv[EOpenUI.Avatar]), Color.white);
    }

    public void ShowStage()
    {
        UIGameLobby.Get.Show();
        UIMainLobby.Get.Hide();
    }

    public void ShowSkillFormation()
    {
        if(GameData.IsOpenUIEnable(EOpenUI.Ability))
        {
            UISkillFormation.UIShow(true);
            UIMainLobby.Get.Hide();
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), GameData.DOpenUILv[EOpenUI.Ability]), Color.white);
    }

    public void ShowEquipment()
    {
        if(GameData.IsOpenUIEnable(EOpenUI.Equipment))
        {
            UIEquipment.Get.Show();
            UIMainLobby.Get.Hide();
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), GameData.DOpenUILv[EOpenUI.Equipment]), Color.white);
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
        if(GameData.IsOpenUIEnable(EOpenUI.Mission))
        {
            UIMainLobby.Get.Hide();
            UIMission.Visible = true;
            
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), GameData.DOpenUILv[EOpenUI.Mission]), Color.white);
    }

    public void OnSocial()
    {
        if(string.IsNullOrEmpty(GameData.Team.Player.Name)) 
            UIHint.Get.ShowHint(TextConst.S(5026), Color.white);
        else if(GameData.IsOpenUIEnable(EOpenUI.Social))
        {
            UIMainLobby.Get.Hide();
            UISocial.Visible = true;
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), GameData.DOpenUILv[EOpenUI.Social]), Color.white);
    }

    public void OnShop()
    {
        if(GameData.IsOpenUIEnable(EOpenUI.Shop))
        {
            UIMainLobby.Get.Hide(2);
            UIShop.Visible = true;
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), GameData.DOpenUILv[EOpenUI.Shop]), Color.white);
    }

    public void OnMall()
    {
        if(GameData.IsOpenUIEnable(EOpenUI.Mall))
        {
            UIMainLobby.Get.Hide();
            UIMall.Get.Show();
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), GameData.DOpenUILv[EOpenUI.Mall]), Color.white);
    }

    public void OnBuyCoin()
    {
        UIRecharge.Get.Show(ERechargeType.Coin.GetHashCode());
		UIMainLobby.Get.Hide();
    }

    public void OnBuyDiamond()
    {
        UIRecharge.Get.Show(ERechargeType.Diamond.GetHashCode());
		UIMainLobby.Get.Hide();
    }

    public void OnBuyPower()
    {
        UIRecharge.Get.Show(ERechargeType.Power.GetHashCode());
		UIMainLobby.Get.Hide();
    }
}