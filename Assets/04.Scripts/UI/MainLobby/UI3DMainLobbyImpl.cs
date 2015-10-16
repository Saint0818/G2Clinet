using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UI3DMainLobbyImpl : MonoBehaviour
{
    public Transform PlayerPos;

    private AvatarPlayer mAvatar;

    [UsedImplicitly]
    private void Awake()
    {
    }

    public void Show()
    {
        UpdateAvatar();
    }

    public void Hide()
    {
    }

    public void UpdateAvatar()
    {
        var player = GameData.Team.Player;
        Dictionary<UICreateRole.EEquip, int> itemIDs = new Dictionary<UICreateRole.EEquip, int>
            {
                {UICreateRole.EEquip.Body, player.GetBodyItemID()},
                {UICreateRole.EEquip.Hair, player.GetHairItemID()},
                {UICreateRole.EEquip.Cloth, player.GetClothItemID()},
                {UICreateRole.EEquip.Pants, player.GetPantsItemID()},
                {UICreateRole.EEquip.Shoes, player.GetShoesItemID()},
                {UICreateRole.EEquip.Head, player.GetHeadItemID()},
                {UICreateRole.EEquip.Hand, player.GetHandItemID()},
                {UICreateRole.EEquip.Back, player.GetBackItemID()}
            };

        if (mAvatar == null)
            mAvatar = new AvatarPlayer(PlayerPos, null, "LobbyAvatarPlayer", player.ID, itemIDs);
        else
            mAvatar.ChangeParts(itemIDs);
        
    }
}