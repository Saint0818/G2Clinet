using UnityEngine;

public class UIMainLobbyEvents : MonoBehaviour
{
    public void ShowAvatarFitted()
    {
        UIAvatarFitted.UIShow(true);
        UIMainLobby.Get.Hide();
        UIResource.Get.Show();
    }

    public void ShowGameLobby()
    {
        UIGameLobby.Get.Show();

        UIMainLobby.Get.Hide();
        UIResource.Get.Show();

        Statistic.Ins.LogEvent(1);
    }

    public void ShowSkillFormation()
    {
        UISkillFormation.Get.ShowView();
        UIMainLobby.Get.Hide();
        UIResource.Get.Show();
    }

    public void ShowEquipment()
    {
        UIEquipment.Get.Show();
        UIMainLobby.Get.Hide();
        UIResource.Get.Show();
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
        UIMainLobby.Get.Hide();
        UIResource.Get.Show();
        UIMission.Visible = true;
    }

    public void OnSocial()
    {
        if(string.IsNullOrEmpty(GameData.Team.Player.Name))
            UITutorial.Get.ShowTutorial(34, 1);
        else
        {
            UIMainLobby.Get.Hide();
            UIResource.Get.Show();
            UISocial.Visible = true;
        }
    }

    public void OnShop()
    {
        UIMainLobby.Get.Hide();
        UIResource.Get.Show(UIResource.EMode.PvpSocial);
        UIShop.Visible = true;
    }

    public void OnMall()
    {
        UIMainLobby.Get.Hide();
        UIResource.Get.Show();
        UIMall.Get.ShowView();
    }

    public void OnAlbum()
    {
        UIResource.Get.Show();
        UISuitAvatar.Get.ShowView();
    }
}