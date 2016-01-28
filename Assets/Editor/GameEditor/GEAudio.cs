using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GEAudio : GEBase
{
    private void ShowHint(string str)
    {
        this.ShowNotification(new GUIContent(str));
    }

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
}
