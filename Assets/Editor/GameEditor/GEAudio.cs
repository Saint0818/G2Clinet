using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GEAudio : GEBase
{
    [MenuItem("GameEditor/Audio/Change NGUIBtn Sound")]
    public static void ChangeNGUISound()
    {
        GameObject[] objs = Selection.gameObjects;
        List<Component[]> UIPlaySounds = new List<Component[]>();
        foreach (GameObject obj in objs)
        {
            Component[] finds = obj.GetComponentsInChildren(typeof(UIPlaySound), true);
            UIPlaySounds.Add(finds);
        }

        foreach (Component[] item in UIPlaySounds)
        {
            for (int i = 0; i < item.Length; i++)
            {
                if (item[i].gameObject.GetComponent<UISound>())
                {
                    continue;
                }
                else
                {
                    item[i].gameObject.AddComponent<UISound>();
                }
            }
        }

        foreach (Component[] item in UIPlaySounds)
        {
            for (int i = 0; i < item.Length; i++)
            {
                DestroyImmediate(item[i]);
            }
        }

        foreach (GameObject obj in objs)
        {
            GameObject instanceRoot = PrefabUtility.FindRootGameObjectWithSameParentPrefab(obj);
            UnityEngine.Object targetPrefab = UnityEditor.PrefabUtility.GetPrefabParent(instanceRoot);

            PrefabUtility.ReplacePrefab(
                instanceRoot,
                targetPrefab,
                ReplacePrefabOptions.ConnectToPrefab
            );
        }
    }

    [MenuItem("GameEditor/Audio/Add UISound")]
    public static void AddUISound()
    {
        GameObject[] objs = Selection.gameObjects;
        if (objs.Length == 1)
        {
            if (objs[0].GetComponent<UISound>())
                Debug.LogError("已有UISound Component");
            else
                objs[0].AddComponent<UISound>();
        }
        else
            Debug.LogError("請選擇GameObject");
    }
}
