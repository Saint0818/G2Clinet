﻿
using UnityEngine;

public class UIPrefabPath
{
    public static readonly string AttriuteHexagon = "Prefab/UI/UIAttributeHexagon";
    public static readonly string EquipPartSlot = "Prefab/UI/Items/EquipPartSlot";
    public static readonly string ItemEquipmentBtn = "Prefab/UI/Items/ItemEquipmentBtn";

    public static GameObject LoadUI(string path, Transform parent)
    {
        GameObject obj = Object.Instantiate(Resources.Load<GameObject>(path));
        obj.transform.parent = parent;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        return obj;
    }
}
