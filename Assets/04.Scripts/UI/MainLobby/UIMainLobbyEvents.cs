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
		UIMainLobby.Get.HideAll();
		UI3DMainLobby.Get.Hide();
		UIBuyStore.Get.Show();
		UI3DBuyStore.Get.Show();
	}
}
