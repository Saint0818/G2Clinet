using GameEnum;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// 
/// </summary>
/// How to use:
/// <list type="number">
/// <item> Attach UIRandomName component to GameObject. </item>
/// <item> Call RandomName to generate a new name. </item>
/// </list>
[RequireComponent(typeof(UILabel))]
[DisallowMultipleComponent]
public class UIRandomName : MonoBehaviour
{
    private TTeamName[] mTeamNames;
    private UILabel mLabel;

    [UsedImplicitly]
	private void Awake()
    {
        mLabel = GetComponent<UILabel>();
        TextAsset text = Resources.Load("GameData/teamname") as TextAsset;
        if(text)
			mTeamNames = (TTeamName[])JsonConvert.DeserializeObject<TTeamName[]>(text.text, SendHttp.Get.JsonSetting));
        else
            Debug.LogError("Load Resource fail:GameData/teamname.");
    }

    public void RandomName()
    {
        if(mTeamNames == null)
        {
            Debug.LogError("Load Resource fail:GameData/teamname.");
            return;
        }

        if(mTeamNames.Length <= 0)
        {
            Debug.LogError("Resource is empty:GameData/teamname.");
            return;
        }

        // if parameters is integer,
        // returns a random integer number between min [inclusive] and max [exclusive] (Read Only).
        int index1 = Random.Range(0, mTeamNames.Length);
        int index2 = Random.Range(0, mTeamNames.Length);
        int index3 = Random.Range(0, mTeamNames.Length);
     	mLabel.text = mTeamNames[index1].TeamName1 + mTeamNames[index2].TeamName2 + mTeamNames[index3].TeamName3;
    }
}

public struct TTeamName
{
    public string TeamName1TW;
    public string TeamName2TW;
    public string TeamName3TW;
    public string TeamName1EN;
    public string TeamName2EN;
    public string TeamName3EN;

    public string TeamName1
    {
        get
        {
            switch (GameData.Setting.Language)
            {
                case ELanguage.TW:
                    return TeamName1TW;
                case ELanguage.EN:
                    return TeamName1EN;
                default:
                    return TeamName1EN;
            }
        }
    }

    public string TeamName2
    {
        get
        {
            switch (GameData.Setting.Language)
            {
                case ELanguage.TW:
                    return TeamName2TW;
                case ELanguage.EN:
                    return TeamName2EN;
                default:
                    return TeamName2EN;
            }
        }
    }

    public string TeamName3
    {
        get
        {
            switch (GameData.Setting.Language)
            {
                case ELanguage.TW:
                    return TeamName3TW;
                case ELanguage.EN:
                    return TeamName3EN;
                default:
                    return TeamName3EN;
            }
        }
    }
}
