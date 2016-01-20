
using System.Collections.Generic;
using GameStruct;
using UnityEngine;

public static class UIInstanceBuilder
{
    public static UIInstanceChapter.Data Build(ChapterData data, List<TStageData> oneChapterStages, 
                                               int bossPlayerID)
    {
        return new UIInstanceChapter.Data
        {
            Title = data.Name,
            Desc = data.Explain,
            Model = buildModel(bossPlayerID)
        };
    }

    private static GameObject buildModel(int playerID)
    {
        TPlayer player = new TPlayer(0) { ID = playerID };
        player.SetAvatar();

        GameObject model = new GameObject { name = "BossModel" };
        ModelManager.Get.SetAvatar(ref model, player.Avatar, GameData.DPlayers[playerID].BodyType,
                                   EAnimatorType.AvatarControl, false);

        changeLayersRecursively(model.transform, "UI");

        return model;
    }

    private static void changeLayersRecursively(Transform current, string name)
    {
        current.gameObject.layer = LayerMask.NameToLayer(name);
        foreach(Transform child in current)
        {
            child.gameObject.layer = LayerMask.NameToLayer(name);
            changeLayersRecursively(child, name);
        }
    }
}
