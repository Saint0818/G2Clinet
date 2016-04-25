using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureManager : KnightSingleton<TextureManager> {
    private Dictionary<int, UIAtlas> mDItemAtlas = new Dictionary<int, UIAtlas>();
    private Dictionary<int, Texture2D> cardTextureCache = new Dictionary<int, Texture2D>();
    private Dictionary<string, Texture2D> cardItemTextureCache = new Dictionary<string, Texture2D>();

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy() {
        Release();
    }

    public void Release() {
        //foreach (KeyValuePair<int, UIAtlas> item in mDItemAtlas)
        //    item.Value.spriteMaterial = null;

        mDItemAtlas.Clear();
        cardTextureCache.Clear();
        cardItemTextureCache.Clear();
    }

    public UIAtlas ItemAtlas(int atlasID) {
        if (mDItemAtlas.ContainsKey(atlasID))
            return mDItemAtlas[atlasID];
        else {
            UIAtlas atlas = null;
            GameObject obj = Resources.Load("UI/AtlasItem/AtlasItem_" + atlasID.ToString()) as GameObject;
            if (obj)
                atlas = obj.GetComponent<UIAtlas>();
            
            mDItemAtlas.Add(atlasID, atlas);
            return atlas;
        }
    } 

    public Texture2D CardTexture(int id)
    {
        if (GameData.DSkillData.ContainsKey(id))
        {
            if (GameData.DSkillData[id].PictureNo > 0)
                id = GameData.DSkillData[id].PictureNo;

            if (cardTextureCache.ContainsKey(id))
            {
                return cardTextureCache[id];
            }
            else
            {
				string path = "Textures/SkillCards/Big/" + id.ToString();
                Texture2D obj = Resources.Load(path) as Texture2D;
                if (obj)
                {
                    cardTextureCache.Add(id, obj);
                    return obj;
                }
                else
                {
                    //download form server
                    return null;
                }
            }
        }
        else
            return null;
    }

    public Texture2D CardItemTexture(int id)
    {
        if (GameData.DSkillData.ContainsKey(id))
        {
            if (!string.IsNullOrEmpty(GameData.DSkillData[id].RectanglePicture))
            {
                if (cardItemTextureCache.ContainsKey(GameData.DSkillData[id].RectanglePicture))
                {
                    return cardItemTextureCache[GameData.DSkillData[id].RectanglePicture];
                }
                else
                {
                    string path = "Textures/SkillCards/Rectangle/" + GameData.DSkillData[id].RectanglePicture;
                    Texture2D obj = Resources.Load(path) as Texture2D;
                    if (obj)
                    {
                        cardItemTextureCache.Add(GameData.DSkillData[id].RectanglePicture, obj);
                        return obj;
                    }
                    else
                    {
                        //download form server
                        return null;
                    }
                }
            }
        }
        return null;
    }
}
