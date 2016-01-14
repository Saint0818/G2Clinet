
using UnityEngine;

public class UIPrefabPath
{
    public static readonly string AttriuteHexagon = "Prefab/UI/UIAttributeHexagon";

    public static readonly string EquipPartSlot = "Prefab/UI/Items/EquipPartSlot";
    public static readonly string UIEquipItem = "Prefab/UI/Items/UIEquipItem";
    public static readonly string EquipListButton = "Prefab/UI/Items/ItemListEquipment";
    public static readonly string EquipMaterialItem = "Prefab/UI/Items/MaterialItem";
    public static readonly string UIFXAddValueItemInlay = "Effect/UIFXAddValueItemInlay";

    public static readonly string ItemSkillHint = "Prefab/UI/Items/ItemSkillHint";
	public static readonly string UIStageHint = "Prefab/UI/UIStageHint";
	public static readonly string ItemSkillCard = "Prefab/UI/Items/ItemSkillCard";
	public static readonly string ItemCardEquipped = "Prefab/UI/Items/ItemCardEquipped";

	public static readonly string ItemSourceElement = "Prefab/UI/Items/ItemSourceElement";

    public static readonly string StageChapter = "Prefab/UI/UIStageChapter";

	public static readonly string ItemAwardGroup = "Prefab/UI/Items/ItemAwardGroup";

	public static readonly string ItemRechargeGems = "Prefab/UI/Items/ItemRechargeGems";
	public static readonly string ItemRechargeMoney = "Prefab/UI/Items/ItemRechargeMoney";
	public static readonly string ItemRechargeStamina = "Prefab/UI/Items/ItemRechargeStamina";

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
