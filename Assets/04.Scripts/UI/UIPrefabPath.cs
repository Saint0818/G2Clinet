﻿
using UnityEngine;

public class UIPrefabPath
{
    public static readonly string AttriuteHexagon = "Prefab/UI/UIAttributeHexagon";

    public static string UIMainStageElement = "Prefab/UI/UIMainStageElement";
    // Kind = 9 的特殊關卡.
    public static string UIMainStageElement9 = "Prefab/UI/UIMainStageElement9";

    public static readonly string EquipPartSlot = "Prefab/UI/Items/EquipPartSlot";
    public static readonly string UIEquipItem = "Prefab/UI/Items/UIEquipItem";
    public static readonly string UIEquipListButton = "Prefab/UI/Items/UIEquipListButton";
    public static readonly string EquipMaterialItem = "Prefab/UI/Items/MaterialItem";
	public static readonly string UIFXAddValueItemInlay = "Effect/UIFXAddValueItemInlay";
	public static readonly string UIFXAwardGetItem = "Effect/UIFXAwardGetItem";

    public static readonly string UIInstanceChapter = "Prefab/UI/UIInstanceChapter";
    public static readonly string UIInstanceStage0 = "Prefab/UI/Items/UIInstanceStage0";
    public static readonly string UIInstanceStage9 = "Prefab/UI/Items/UIInstanceStage9";

    public static readonly string UIDailyReward = "Prefab/UI/Items/UIDailyLoginReward";
    public static readonly string UIDailyReward7 = "Prefab/UI/Items/UIDailyLoginReward7";
    public static readonly string UILifetimeReward = "Prefab/UI/Items/UILifetimeReward";

    public static readonly string ItemSkillHint = "Prefab/UI/Items/ItemSkillHint";
	public static readonly string UIStageHint = "Prefab/UI/UIStageHint";
	public static readonly string ItemSkillCard = "Prefab/UI/Items/ItemSkillCard";
	public static readonly string ItemCardEquipped = "Prefab/UI/Items/ItemCardEquipped";
	public static readonly string ItemSuitCardGroup = "Prefab/UI/Items/ItemSuitCardGroup";
	public static readonly string ItemSuitCardLaunch = "Prefab/UI/Items/ItemSuitCardLaunch";
	public static readonly string ItemSuitAvatarGroup = "Prefab/UI/Items/ItemSuitAvatarGroup";

	public static readonly string UICreateRoleStyleViewPartsWindowButton = "Prefab/UI/Items/UICreateRoleStyleViewPartsWindowButton";

	public static readonly string ItemSourceElement = "Prefab/UI/Items/ItemSourceElement";

    public static readonly string StageChapter = "Prefab/UI/UIStageChapter";

	public static readonly string ItemAwardGroup = "Prefab/UI/Items/ItemAwardGroup";

	public static readonly string ItemRechargeGems = "Prefab/UI/Items/ItemRechargeGems";
	public static readonly string ItemRechargeMoney = "Prefab/UI/Items/ItemRechargeMoney";
	public static readonly string ItemRechargeStamina = "Prefab/UI/Items/ItemRechargeStamina";

	public static readonly string ItemGymEngage = "Prefab/UI/Items/ItemGymEngage";
	public static readonly string UIView = "Prefab/UI/UIView";

    public static GameObject LoadUI(string path)
    {
        return LoadUI(path, null, Vector3.zero);
    }

    public static GameObject LoadUI(string path, Transform parent)
    {
        return LoadUI(path, parent, Vector3.zero);
    }

    public static GameObject LoadUI(string path, Transform parent, Vector3 localPos)
    {
        GameObject obj = Object.Instantiate(Resources.Load<GameObject>(path));
        obj.transform.parent = parent;
        obj.transform.localPosition = localPos;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        return obj;
    }
}
