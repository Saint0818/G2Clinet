using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : KnightSingleton<SendHttp> {
    private static Dictionary<string, GameObject> UIResources = new Dictionary<string, GameObject>();
    private static Dictionary<string, GameObject> UIDictionary = new Dictionary<string, GameObject>();

    protected override void Init() {
        DontDestroyOnLoad(gameObject);
    }

    public static GameObject LoadPrefab(string uiname) {
        if (UIResources.ContainsKey(uiname))
            return UIResources[uiname];
        else {
            GameObject obj = Resources.Load<GameObject>(uiname);
            UIResources.Add(uiname, obj);
            return obj;
        }
    }

    public static void AddUI(string uiname, ref GameObject ui) {
        if (!UIDictionary.ContainsKey(uiname))
            UIDictionary.Add(uiname, ui);
    }

    public static void RemoveUI(string uiname) {
        if (UIDictionary.ContainsKey(uiname)) {
            NGUIDebug.DestroyImmediate(UIDictionary[uiname], true);
            UIDictionary.Remove(uiname);
            Resources.UnloadUnusedAssets();
        }
    }

}
