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

	public void OnShaffle () {
//		if(GameData.Team.Diamond >= 30) {
//			UIMainLobby.Get.HideAll();
//			UI3DMainLobby.Get.Hide();
//			UIBuyStore.Get.ShowView(true);
//			UI3DBuyStore.Get.Show();
//		} else
//			UIHint.Get.ShowHint(TextConst.S(233), Color.white);

		UIMainLobby.Get.Hide();
		UIMall.Get.Show();
	}

	public void OnShaffleTen () {
		if(GameData.Team.Diamond >= 250) {
			UIMainLobby.Get.HideAll();
			UI3DMainLobby.Get.Hide();
			UIBuyStore.Get.ShowView(false);
			UI3DBuyStore.Get.Show();
		} else
			UIHint.Get.ShowHint(TextConst.S(233), Color.white);
		}
}
