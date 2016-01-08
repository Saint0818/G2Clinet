using UnityEngine;
using GameEnum;

public class UIMainLobbyEvents : MonoBehaviour
{
    public void ShowSettings()
    {
        UISetting.UIShow(true);
    }

    public void ShowAvatarFitted()
    {
        int index = EOpenUI.Avatar.GetHashCode();
        if (GameData.DOpenUILv.ContainsKey(index) && 
            GameData.Team.Player.Lv < GameData.DOpenUILv[index])
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), GameData.DOpenUILv[index]), Color.white);
        else {
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
        int index = EOpenUI.Ability.GetHashCode();
        if (GameData.DOpenUILv.ContainsKey(index) && 
            GameData.Team.Player.Lv < GameData.DOpenUILv[index])
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), GameData.DOpenUILv[index]), Color.white);
        else {
            UISkillFormation.UIShow(true);
            UIMainLobby.Get.Hide();
        }
    }

    public void ShowEquipment()
    {
        int index = EOpenUI.Equipment.GetHashCode();
        if (GameData.DOpenUILv.ContainsKey(index) && 
            GameData.Team.Player.Lv < GameData.DOpenUILv[index])
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), GameData.DOpenUILv[index]), Color.white);
        else {
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
		UIPlayerPotential.UIShow (true);	
	}

    public void OnMission() {
        int index = EOpenUI.Mission.GetHashCode();
        if (GameData.DOpenUILv.ContainsKey(index) && 
            GameData.Team.Player.Lv < GameData.DOpenUILv[index])
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), GameData.DOpenUILv[index]), Color.white);
        else {
            UIMainLobby.Get.Hide();
    		UIMission.Visible = true;
        }
    }

    public void OnSocial() {
        int index = EOpenUI.Social.GetHashCode();
        if (GameData.DOpenUILv.ContainsKey(index) && 
            GameData.Team.Player.Lv < GameData.DOpenUILv[index])
            UIHint.Get.ShowHint(string.Format(TextConst.S(512), GameData.DOpenUILv[index]), Color.white);
        else {
            UIMainLobby.Get.Hide();
            UISocial.Visible = true;
        }
    }

	public void OnMall () {
		UIMainLobby.Get.Hide();
		UIMall.Get.Show();
	}

	public void OnBuyCoin() {
		UIRecharge.Get.Show(ERechargeType.Coin.GetHashCode());
	}

	public void OnBuyDiamond() {
		UIRecharge.Get.Show(ERechargeType.Diamond.GetHashCode());
	}

	public void OnBuyPower() {
		UIRecharge.Get.Show(ERechargeType.Power.GetHashCode());
	}
}
