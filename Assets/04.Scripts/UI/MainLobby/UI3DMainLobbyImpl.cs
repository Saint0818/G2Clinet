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
        if(mAvatar == null)
        {
            var player = GameData.Team.Player;
            mAvatar = new AvatarPlayer(PlayerPos, null, "LobbyAvatarPlayer", player.ID, player.GetBodyItemID(),
                player.GetHairItemID(), player.GetClothItemID(), player.GetPantsItemID(), player.GetShoesItemID());
        }
        else
        {
            var player = GameData.Team.Player;
            mAvatar.UpdateParts(player.GetBodyItemID(), player.GetHairItemID(), player.GetClothItemID(), 
                                player.GetPantsItemID(), player.GetShoesItemID());
        }
        
    }
}