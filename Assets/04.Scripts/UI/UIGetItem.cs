using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGetItem : UIBase {
    private static UIGetItem instance = null;
    private const string UIName = "UIGetItem";

    private GameObject itemAward;
    private GameObject itemExp;
    private GameObject itemAnchor;
    private UILabel labelTitle;
    private List<GameObject> itemList = new List<GameObject>();

    public static bool Visible {
        get {
            if(instance)
                return instance.gameObject.activeInHierarchy;
            else
                return false;
        }

        set {
            if (instance) {
                if (!value)
                    RemoveUI(UIName);
                else
                    instance.Show(value);
            } else
            if (value)
                Get.Show(value);
        }
    }

    public static UIGetItem Get
    {
        get {
            if (!instance) 
                instance = LoadUI(UIName) as UIGetItem;

            return instance;
        }
    }

    protected override void InitCom() {
		UIEventListener.Get(GameObject.Find(UIName + "/Center/CoverBackground")).onClick = OnClose;

        itemAward = Resources.Load("Prefab/UI/Items/ItemAwardGroup") as GameObject;
        itemExp = Resources.Load("Prefab/UI/Items/ItemExp") as GameObject;
        itemAnchor = GameObject.Find(UIName + "/Center/AwardsGroup/ScrollView") as GameObject; 
        labelTitle = GameObject.Find(UIName + "/Center/TitleLabel").GetComponent<UILabel>();
    }

    protected override void OnShow(bool isShow) {
        base.OnShow(isShow);
    }

	public void OnClose(GameObject go) {
        Visible = false;
    }

    //0.diamond 1.money 2.pvp coin 3.social coin 4.exp
    public void AddExp(int kind, int value) {
        Visible = true;
        GameObject obj = Instantiate(itemExp, Vector3.zero, Quaternion.identity) as GameObject;
        string name = itemList.Count.ToString();
        obj.name = name;
        GameObject.Find(name + "/Icon").GetComponent<UISprite>().spriteName = GameFunction.SpendKindTexture(kind);
        UILabel lab = GameObject.Find(name + "/ValueLabel").GetComponent<UILabel>();
        if (lab) 
            lab.text = value.ToString();

        obj.transform.parent = itemAnchor.transform;
        obj.transform.localPosition = new Vector3(-230 + itemList.Count * 150, 0, 0);
        obj.transform.localScale = Vector3.one;
        itemList.Add(obj);
    }

    public void AddItem(int id) {
        if (GameData.DItemData.ContainsKey(id)) {
            Visible = true;
            GameObject obj = Instantiate(itemAward, Vector3.zero, Quaternion.identity) as GameObject;
            string name = itemList.Count.ToString();
            obj.name = name;
            ItemAwardGroup award = obj.GetComponent<ItemAwardGroup>();
            if (award)
                award.Show(GameData.DItemData[id]);

            LayerMgr.Get.SetLayer(obj, ELayer.TopUI);
            obj.transform.parent = itemAnchor.transform;
            obj.transform.localPosition = new Vector3(-230 + itemList.Count * 150, 0, 0);
            obj.transform.localScale = Vector3.one;
            itemList.Add(obj);
        }
    }

    public void SetTitle(string text) {
        labelTitle.text = text;
    }
}
