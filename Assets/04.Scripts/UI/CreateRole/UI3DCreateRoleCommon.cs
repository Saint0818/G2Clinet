using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using GameEnum;

/// <summary>
/// 給 UI3DCreateRoleXXX 使用, 主要是放相同的東西.
/// </summary>
[DisallowMultipleComponent]
public class UI3DCreateRoleCommon : MonoBehaviour
{
    public Transform CenterParent;
    public GameObject CenterShadow;

    public Transform ForwardParent;
    public GameObject ForwardShadow;

    public Transform GuardParent;
    public GameObject GuardShadow;

    public GameObject SFXBall;
    public GameObject Ball;

    private readonly Dictionary<EPlayerPostion, GameObject> mShadows = new Dictionary<EPlayerPostion, GameObject>();
    private readonly Dictionary<EPlayerPostion, Transform> mParents = new Dictionary<EPlayerPostion, Transform>();

    [UsedImplicitly]
    private void Awake()
    {
        mShadows.Add(EPlayerPostion.G, GuardShadow);
        mShadows.Add(EPlayerPostion.F, ForwardShadow);
        mShadows.Add(EPlayerPostion.C, CenterShadow);

        mParents.Add(EPlayerPostion.G, GuardParent);
        mParents.Add(EPlayerPostion.F, ForwardParent);
        mParents.Add(EPlayerPostion.C, CenterParent);

        SFXBall.SetActive(false);
        Ball.SetActive(false);
    }

//    public void ShowShadow(EPlayerPostion pos)
//    {
//        foreach(KeyValuePair<EPlayerPostion, GameObject> pair in mShadows)
//        {
//            pair.Value.SetActive(pair.Key == pos);
//        }
//    }
//
//    public void SetAllShadowVisible(bool visible)
//    {
//        foreach (KeyValuePair<EPlayerPostion, GameObject> pair in mShadows)
//        {
//            pair.Value.SetActive(visible);
//        }
//    }

    public Transform GetParent(EPlayerPostion pos)
    {
        return mParents[pos];
    }

    public GameObject GetShadow(EPlayerPostion pos)
    {
        return mShadows[pos];
    }
}