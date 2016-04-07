using UnityEngine;
using System.Collections;

public class UI3DPVP : UIBase {
    private static UI3DPVP instance = null;
    private const string UIName = "UI3DPVP";
    private const int playerNum = 6;

    public GameObject [] PlayerAnchor = new GameObject[playerNum];

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
                    RemoveUI(instance.gameObject);
                else
                    instance.Show(value);
            } else
            if (value) 
                Get.Show(value);
        }
    }

    public static UI3DPVP Get {
        get {
            if (!instance)
                instance = Load3DUI(UIName) as UI3DPVP;

            return instance;
        }
    }

    protected override void InitCom() {
        for (int i = 0; i < playerNum; i++)
            PlayerAnchor[i] = GameObject.Find(UIName + "/PlayerPos/" + i.ToString());
    }
}
