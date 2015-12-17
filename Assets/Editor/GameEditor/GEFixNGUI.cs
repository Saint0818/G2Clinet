/* Introduction:Add Atuo Subscribe
 * Author:Shui
 * Update Date:
 * */
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

public class GEFixNGUI
{
    public enum SubscribeType
    {
        button,
        label,
        sprite,
        page}

    ;

    private static SubscribeType mType = SubscribeType.button;

    [MenuItem("GameEditor/NGUITune/ResetPanelDepth")]
    static void ResetPanelDepth()
    {
        if (Selection.gameObjects.Length > 0)
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                UIPanel[] panels = Selection.gameObjects[i].GetComponentsInChildren<UIPanel>();
                if (panels != null && panels.Length > 0)
                {
                    for (int j = 0; j < panels.Length; j++)
                    {
                        panels[j].showInPanelTool = true;
                    }
                }
            }
        }
    }

    [MenuItem("GameEditor/NGUITune/ResetLabelDepth")]
    static void ResetLabelDepth()
    {
        if (Selection.gameObjects.Length > 0)
        {
            Material mat = Resources.Load<Material>("Font/NotoSansCJKtc-Black");
            UIFont font = Resources.Load<UIFont>("Font/NotoSans");
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                UILabel[] labs = Selection.gameObjects[i].GetComponentsInChildren<UILabel>();
                if (labs != null && labs.Length > 0)
                {
                    for (int j = 0; j < labs.Length; j++)
                    {
                        labs[j].bitmapFont = font;
                        labs[j].trueTypeFont = null;

                        labs[j].material = mat;
                        //labs[j].depth = 100;
                        labs[j].spacingX = 0;
                        labs[j].spacingY = 0;
                        labs[j].effectDistance = new Vector2(1, 1);
                        if (true){//labs[j].effectStyle != UILabel.Effect.Outline8) {
                            labs[j].effectStyle = UILabel.Effect.Outline8;
                            labs[j].effectColor = new Color32(100, 100, 100, 255);
                        }

                        if (labs[j].fontSize > 80)
                            labs[j].fontSize = 96;
                        else if (labs[j].fontSize > 64)
                            labs[j].fontSize = 80;
                        else if (labs[j].fontSize > 48)
                            labs[j].fontSize = 64;
                        else if (labs[j].fontSize > 32)
                            labs[j].fontSize = 48;
                        else if (labs[j].fontSize > 28)
                            labs[j].fontSize = 32;
                        else if (labs[j].fontSize > 24)
                            labs[j].fontSize = 28;
                        else if (labs[j].fontSize > 20)
                            labs[j].fontSize = 24;
                        else if (labs[j].fontSize > 16)
                            labs[j].fontSize = 20;
                        else
                            labs[j].fontSize = 16;
                    }
                }
            }
        }
    }

    [MenuItem("GameEditor/NGUITune/ResetSpriteDepth")]
    static void ResetSpriteDepth()
    {
        if (Selection.gameObjects.Length > 0)
        {
            GameObject obj = (GameObject)Resources.Load("Textures/Ui_info/Ui_info_4", typeof(GameObject));
            UIAtlas at = obj.GetComponent<UIAtlas>();
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                UISprite[] sps = Selection.gameObjects[i].GetComponentsInChildren<UISprite>();
                if (sps != null && sps.Length > 0)
                {
                    for (int j = 0; j < sps.Length; j++)
                    {
                        sps[j].atlas = at;
                    }
                }
            }
        }
    }

    [MenuItem("GameEditor/NGUITune/AddSubscribe_Button")]
    static void ButtonType()
    {
        mType = SubscribeType.button;
        Add();
    }

    [MenuItem("GameEditor/NGUITune/AddSubscribe_Label")]
    static void LabelType()
    {
        mType = SubscribeType.label;
        Add();
    }

    [MenuItem("GameEditor/NGUITune/AddSubscribe_Sprite")]
    static void SpriteType()
    {
        mType = SubscribeType.sprite;
        Add();
    }

    [MenuItem("GameEditor/NGUITune/AddSubscribe_Page")]
    static void PageType()
    {
        mType = SubscribeType.page;
        Add();
    }

    static void Add()
    {
        Font f = (Font)Resources.Load("TAI");
        if (Selection.gameObjects.Length > 0)
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                UILabel[] labs = Selection.gameObjects[i].GetComponentsInChildren<UILabel>();
                if (labs != null && labs.Length > 0)
                {
                    for (int j = 0; j < labs.Length; j++)
                    {
                        labs[j].trueTypeFont = f;
                        labs[j].material = null;

                        Debug.Log(labs[j].name + " set font.");
                    }
                }
            }
        }
    }

    private static void CheckDirectories(string path)
    {
        string[] dirs = Directory.GetFiles(path, "UI???.prefab");
        foreach (string d in dirs)
        {
            GameObject go = AssetDatabase.LoadAssetAtPath(d, typeof(Object)) as GameObject;
            if (go == null)
                continue;

            CheckSelected(go);
        }

        string[] subDirs = Directory.GetDirectories(path);
        if (subDirs.Length == 0)
            return;

        foreach (string dir in subDirs)
        {
            CheckDirectories(dir + "/");
        }
    }

    private static void CheckSelected(GameObject[] go)
    {
        foreach (GameObject g in go)
        {
            CheckAndAdd(g);
            CheckSelected(g);
        }
    }

    private static void CheckSelected(GameObject go)
    {
        CheckAndAdd(go);

        for (int i = 0; i < go.transform.childCount; i++)
        {
            GameObject gochild = go.transform.GetChild(i).gameObject;
            CheckAndAdd(gochild);

            if (gochild.transform.childCount > 0)
                CheckSelected(gochild);
        }
    }

    private static void CheckAndAdd(GameObject go)
    {
        if (CheckNameFormat(go))
        if (CheckSubscribeCom(go) == false)
            AddKindComponent(go);
    }

    private static bool CheckNameFormat(GameObject go)
    {
        int number;
        if (mType == SubscribeType.button)
        {
            if (string.Equals(go.name.Substring(0, 1), "B", System.StringComparison.OrdinalIgnoreCase))
            if (go.name.Substring(1).Length == 6)
            if (int.TryParse(go.name.Substring(1), out number))
                return true;
        }
        else if (mType == SubscribeType.label)
        {
            if (string.Equals(go.name.Substring(0, 1), "L", System.StringComparison.OrdinalIgnoreCase))
            if (go.name.Substring(1).Length == 6)
            if (int.TryParse(go.name.Substring(1), out number))
                return true;
        }
        else if (mType == SubscribeType.sprite)
        {
            if (string.Equals(go.name.Substring(0, 1), "S", System.StringComparison.OrdinalIgnoreCase))
            if (go.name.Substring(1).Length == 6)
            if (int.TryParse(go.name.Substring(1), out number))
                return true;
        }
        else if (mType == SubscribeType.page)
        {
            if (string.Equals(go.name.Substring(0, 1), "P", System.StringComparison.OrdinalIgnoreCase))
            if (go.name.Substring(1).Length == 6)
            if (int.TryParse(go.name.Substring(1), out number))
                return true;
        }
        return false;
    }

    private static bool CheckSubscribeCom(GameObject go)
    {
        return false;
    }

    private static void AddKindComponent(GameObject go)
    {
		
    }

    private static void ShowErrorMessage()
    {
        EditorUtility.DisplayDialog("DS", "No UI Selected.", "OK");
    }
}
