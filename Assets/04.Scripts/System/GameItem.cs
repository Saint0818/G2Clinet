using UnityEngine;
using System.Collections;
using GameStruct;

namespace GameItem
{
	public class TPlayerInGameBtn
	{
			private GameObject self;
			private UISprite headTexture;
			private UISprite position;
			private UILabel lv;

			public void Init(GameObject go)
			{
					if(go)
					{
							self = go;
							headTexture = self.transform.FindChild("PlayerPic").GetComponent<UISprite>();
							position = headTexture.transform.FindChild("PositionIcon").GetComponent<UISprite>();
							lv = self.transform.FindChild("LevelGroup").GetComponent<UILabel>();
					}
			}

			public void UpdateView(TPlayer player)
			{
					headTexture.spriteName = player.FacePicture;
					position.spriteName = player.PositionPicture;
					lv.text = player.Lv.ToString();
			}
	}	
}