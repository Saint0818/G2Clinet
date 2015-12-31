using UnityEngine;

public class UIMainLobbyEvents : MonoBehaviour
{
    public void ShowSettings()
    {
        UISetting.UIShow(true);
    }

    public void ShowAvatarFitted()
    {
        UIAvatarFitted.UIShow(true);
        UIMainLobby.Get.Hide();
    }

    public void ShowStage()
    {
        UIGameLobby.Get.Show();
        UIMainLobby.Get.Hide();
    }

    public void ShowSkillFormation()
    {
        UISkillFormation.UIShow(true);
        UIMainLobby.Get.Hide();
    }

    public void ShowEquipment()
    {
        UIEquipment.Get.Show();
        UIMainLobby.Get.Hide();
    }

    public void ShowPlayerInfo()
    {
        UIMainLobby.Get.Hide();
        UIPlayerInfo.UIShow(true, ref GameData.Team);
    }

    public void OnMission() {
        UIMainLobby.Get.Hide();
		UIMission.Visible = true;
    }

	public void OnShaffle () {
		if(GameData.Team.Diamond >= 30) {
			UIMainLobby.Get.HideAll();
			UI3DMainLobby.Get.Hide();
			UIBuyStore.Get.ShowView(true);
			UI3DBuyStore.Get.Show();
		} else
			UIHint.Get.ShowHint(TextConst.S(233), Color.white);
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
