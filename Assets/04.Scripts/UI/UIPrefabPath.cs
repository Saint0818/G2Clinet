
using UnityEngine;

public class UIPrefabPath
{
    public static readonly string AttriuteHexagon = "Prefab/UI/UIAttributeHexagon";

    public static readonly string EquipPartSlot = "Prefab/UI/Items/EquipPartSlot";
    public static readonly string ItemEquipmentBtn = "Prefab/UI/Items/ItemEquipmentBtn";
    public static readonly string EquipListButton = "Prefab/UI/Items/ItemListEquipment";

	public static readonly string ItemSkillHint = "Prefab/UI/Items/ItemSkillHint";
	public static readonly string UIStageHint = "Prefab/UI/UIStageHint";
	public static readonly string ItemSkillCard = "Prefab/UI/Items/ItemSkillCard";
	public static readonly string ItemCardEquipped = "Prefab/UI/Items/ItemCardEquipped";

    public static readonly string StageChapter = "Prefab/UI/UIStageChapter";

	public static readonly string ItemAwardGroup = "Prefab/UI/Items/ItemAwardGroup";
//	public static readonly string ItemResultAwardGroup = "Prefab/UI/Items/ItemResultAwardGroup";

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
