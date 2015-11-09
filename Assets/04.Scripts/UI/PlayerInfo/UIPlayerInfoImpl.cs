using UnityEngine;
using GameStruct;
using System.Runtime.InteropServices;

[System.Serializable]
public class TItemAvater
{
	public UISprite BG;
	public UISprite Pic;
	public UILabel Name;
	public UIButton Btn;
	public UISprite[] Stars = new UISprite[4];
}

[DisallowMultipleComponent]
public class UIPlayerInfoImpl : MonoBehaviour
{
	//Page 0

		//part1

		//part2
//	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
	public TItemAvater[] Avatars = new TItemAvater[8];
	
		//part3 

		//part4
	public UISprite[] masteriesPic = new UISprite[12];
	public UILabel[] masteriesLabel= new UILabel[12];
	public UIButton SkillUp;

	//Page 1


	//Page 2
	
    private void Awake()
    {

    }

	public void UpdateAvatarModel(TItem[] items)
	{

	}

	public void UpdateAvatarData(TItem[] items)
	{
		if (Avatars.Length == items.Length) {
			for(int i = 0;i< items.Length;i++)
			{
				if(GameData.DItemData.ContainsKey(items[i].ID))
				{
					Avatars[i].Name.text = GameData.DItemData[items[i].ID].Name;
					Avatars[i].Pic.spriteName = GameData.DItemData[items[i].ID].Icon;
//					Avatars[i].BG = GameData.DItemData[items[i].ID].;
//					Avatars[i].Start =  
				}
			}
		}
	}

	public void UpdateMasteries(int[] indexs)
	{
		if(masteriesLabel.Length == indexs.Length){
			for(int i = 0;i < indexs.Length;i++){
				masteriesLabel[i].text = indexs[i].ToString();
			}
		}
	}

    public void ChangePlayerName()
    {
        if(UIInput.current.value.Length <= 0)
            return;

//        if(ChangePlayerNameListener != null)
//            ChangePlayerNameListener();
    }
}