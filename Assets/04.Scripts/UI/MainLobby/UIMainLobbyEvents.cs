using GameEnum;
using UnityEngine;

public class UIMainLobbyEvents : MonoBehaviour
{
//    public void ShowSettings()
//    {
//        UISetting.UIShow(true);
//    }

    public void ShowAvatarFitted()
    {
        if(GameData.IsOpenUIEnable(EOpenID.Avatar))
        {
            UIAvatarFitted.UIShow(true);
            UIMainLobby.Get.Hide();
            UIResource.Get.Show();
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(GameFunction.GetUnlockNumber((int)EOpenID.Avatar)), LimitTable.Ins.GetLv(EOpenID.Avatar)), Color.black);
    }

    public void ShowStage()
    {
        UIGameLobby.Get.Show();
        UIMainLobby.Get.Hide();
        UIResource.Get.Show();
    }

    public void ShowSkillFormation()
    {
		UISkillFormation.Get.ShowView();
        UIMainLobby.Get.Hide();
        UIResource.Get.Show();
    }

    public void ShowEquipment()
    {
        if(GameData.IsOpenUIEnable(EOpenID.Equipment))
        {
            UIEquipment.Get.Show();
            UIMainLobby.Get.Hide();
            UIResource.Get.Show();
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(GameFunction.GetUnlockNumber((int)EOpenID.Equipment)), LimitTable.Ins.GetLv(EOpenID.Equipment)), Color.black);
    }

    public void ShowPlayerInfo()
    {
        UIMainLobby.Get.Hide();
        UIResource.Get.Show();
        UIPlayerInfo.UIShow(true, ref GameData.Team);
    }

    public void ShowPalyerPotential()
    {
        UIMainLobby.Get.Hide();
        UIResource.Get.Show();
        UIPlayerPotential.UIShow(true);
    }

    public void OnMission()
    {
        if(GameData.IsOpenUIEnable(EOpenID.Mission))
        {
            UIMainLobby.Get.Hide();
            UIResource.Get.Show();
            UIMission.Visible = true;
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(GameFunction.GetUnlockNumber((int)EOpenID.Mission)), LimitTable.Ins.GetLv(EOpenID.Mission)), Color.black);
    }

    public void OnSocial()
    {
        if(GameData.IsOpenUIEnable(EOpenID.Social))
        {
            if(string.IsNullOrEmpty(GameData.Team.Player.Name)) 
                UITutorial.Get.ShowTutorial(34, 1);
            else {
                UIMainLobby.Get.Hide();
                UIResource.Get.Show();
                UISocial.Visible = true;
            }
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(GameFunction.GetUnlockNumber((int)EOpenID.Social)), LimitTable.Ins.GetLv(EOpenID.Social)), Color.black);
    }

    public void OnShop()
    {
        if(GameData.IsOpenUIEnable(EOpenID.Shop))
        {
            UIMainLobby.Get.Hide();
            UIResource.Get.Show(2);
            UIShop.Visible = true;
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(GameFunction.GetUnlockNumber((int)EOpenID.Shop)), LimitTable.Ins.GetLv(EOpenID.Shop)), Color.black);
    }

    public void OnMall()
    {
        if(GameData.IsOpenUIEnable(EOpenID.Mall))
        {
            UIMainLobby.Get.Hide();
            UIResource.Get.Show();
            UIMall.Get.ShowView();
        }
        else
            UIHint.Get.ShowHint(string.Format(TextConst.S(GameFunction.GetUnlockNumber((int)EOpenID.Mall)), LimitTable.Ins.GetLv(EOpenID.Mall)), Color.black);
    }

    public void OnDailyLogin()
    {
        UIDailyLogin.Get.Show();
    }

	public void OnAlbum () 
	{
		if(GameData.IsOpenUIEnable(EOpenID.SuitItem))
		{
			UIResource.Get.Show();
			UISuitAvatar.Get.ShowView();
		}
		else
			UIHint.Get.ShowHint(string.Format(TextConst.S(GameFunction.GetUnlockNumber((int)EOpenID.Shop)), LimitTable.Ins.GetLv(EOpenID.Shop)), Color.black);
	}
}