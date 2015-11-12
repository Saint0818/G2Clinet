using GameStruct;
using UnityEngine;
using JetBrains.Annotations;

public class UIEquipPlayer : MonoBehaviour
{
    public Transform HexagonParent;

    private UIAttributes mAttributes;

    private UIEquipmentImpl mImpl;

    [UsedImplicitly]
    private void Awake()
    {
        mImpl = GetComponent<UIEquipmentImpl>();

        GameObject obj = Instantiate(Resources.Load<GameObject>(UIPrefabPath.AttriuteHexagon));
        obj.transform.parent = HexagonParent;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        mAttributes = obj.GetComponent<UIAttributes>();
    }

    public void UpdateUI()
    {
        mAttributes.SetVisible(true);

        var value = mImpl.BasicAttr[EAttributeKind.Strength] + mImpl.BasicAttr[EAttributeKind.Block] +
                    getSumValue(EAttributeKind.Strength) + getSumValue(EAttributeKind.Block);
        mAttributes.SetValue(UIAttributes.EGroup.StrBlk, value / GameConst.AttributeMax);

        value = mImpl.BasicAttr[EAttributeKind.Defence] + mImpl.BasicAttr[EAttributeKind.Steal] +
                getSumValue(EAttributeKind.Defence) + getSumValue(EAttributeKind.Steal);
        mAttributes.SetValue(UIAttributes.EGroup.DefStl, value / GameConst.AttributeMax);

        value = mImpl.BasicAttr[EAttributeKind.Dribble] + mImpl.BasicAttr[EAttributeKind.Pass] +
                getSumValue(EAttributeKind.Dribble) + getSumValue(EAttributeKind.Pass);
        mAttributes.SetValue(UIAttributes.EGroup.DrbPass, value / GameConst.AttributeMax);

        value = mImpl.BasicAttr[EAttributeKind.Speed] + mImpl.BasicAttr[EAttributeKind.Stamina] +
                getSumValue(EAttributeKind.Speed) + getSumValue(EAttributeKind.Stamina);
        mAttributes.SetValue(UIAttributes.EGroup.SpdSta, value / GameConst.AttributeMax);

        value = mImpl.BasicAttr[EAttributeKind.Point2] + mImpl.BasicAttr[EAttributeKind.Point3] +
                getSumValue(EAttributeKind.Point2) + getSumValue(EAttributeKind.Point3);
        mAttributes.SetValue(UIAttributes.EGroup.Pt2Pt3, value / GameConst.AttributeMax);

        value = mImpl.BasicAttr[EAttributeKind.Rebound] + mImpl.BasicAttr[EAttributeKind.Dunk] +
                getSumValue(EAttributeKind.Rebound) + getSumValue(EAttributeKind.Dunk);
        mAttributes.SetValue(UIAttributes.EGroup.RebDnk, value / GameConst.AttributeMax);
    }

    private int getSumValue(EAttributeKind kind)
    {
        int sum = 0;
        for (int i = 0; i < mImpl.Items.Length; i++)
        {
            sum += mImpl.Items[i].GetValue(kind);
        }

//            Debug.LogFormat("{0}:{1}", kind, sum);

        return sum;
    }
} // end of the class.


