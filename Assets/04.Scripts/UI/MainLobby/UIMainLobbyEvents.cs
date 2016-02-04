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
        if(GameData.IsOpenUIEnable(EOpenID.Avatar))
        {
            UIAvatarFitted.UIShow(true);
            UIMainLobby.Get.Hide();
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), LimitTable.Ins.GetLv(EOpenID.Avatar)), Color.white);
    }

    public void ShowStage()
    {
        UIGameLobby.Get.Show();
        UIMainLobby.Get.Hide();
    }

    public void ShowSkillFormation()
    {
        if(GameData.IsOpenUIEnable(EOpenID.Ability))
        {
            UISkillFormation.UIShow(true);
            UIMainLobby.Get.Hide();
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), LimitTable.Ins.GetLv(EOpenID.Ability)), Color.white);
    }

    public void ShowEquipment()
    {
        if(GameData.IsOpenUIEnable(EOpenID.Equipment))
        {
            UIEquipment.Get.Show();
            UIMainLobby.Get.Hide();
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), LimitTable.Ins.GetLv(EOpenID.Equipment)), Color.white);
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
        if(GameData.IsOpenUIEnable(EOpenID.Mission))
        {
            UIMainLobby.Get.Hide();
            UIMission.Visible = true;
            
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), LimitTable.Ins.GetLv(EOpenID.Mission)), Color.white);
    }

    public void OnSocial()
    {
        if(GameData.IsOpenUIEnable(EOpenID.Social))
        {
            if(string.IsNullOrEmpty(GameData.Team.Player.Name)) 
                UITutorial.Get.ShowTutorial(34, 1);
            else {
                UIMainLobby.Get.Hide();
                UISocial.Visible = true;
            }
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), LimitTable.Ins.GetLv(EOpenID.Social)), Color.white);
    }

    public void OnShop()
    {
        if(GameData.IsOpenUIEnable(EOpenID.Shop))
        {
            UIMainLobby.Get.Hide(2);
            UIShop.Visible = true;
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), LimitTable.Ins.GetLv(EOpenID.Shop)), Color.white);
    }

    public void OnMall()
    {
        if(GameData.IsOpenUIEnable(EOpenID.Mall))
        {
            UIMainLobby.Get.Hide();
			UIMall.Get.ShowView();
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), LimitTable.Ins.GetLv(EOpenID.Mall)), Color.white);
    }

    public void OnBuyCoin()
    {
		if(!UIRecharge.Visible)
			UIRecharge.Get.ShowView(ERechargeType.Coin.GetHashCode(), null, false);
    }

    public void OnBuyDiamond()
    {
		if(!UIRecharge.Visible)
			UIRecharge.Get.ShowView(ERechargeType.Diamond.GetHashCode(), null, false);
    }

    public void OnBuyPower()
    {
		if(!UIRecharge.Visible)
			UIRecharge.Get.ShowView(ERechargeType.Power.GetHashCode(), null, false);
    }
}