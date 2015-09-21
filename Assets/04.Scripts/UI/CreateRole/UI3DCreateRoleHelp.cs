using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;

public class UI3DCreateRoleHelp : MonoBehaviour
{
    public Transform CenterParent;
    public Transform ForwardParent;
    public Transform GuardParent;
    public Transform SelectSFX;
    public Animator SelectSFXAnimator;

    private readonly Dictionary<EPlayerPostion, Transform> mPositions = new Dictionary<EPlayerPostion, Transform>();

    [UsedImplicitly]
	private void Awake()
    {
	    mPositions.Add(EPlayerPostion.C, CenterParent);
	    mPositions.Add(EPlayerPostion.F, ForwardParent);
	    mPositions.Add(EPlayerPostion.G, GuardParent);
	}

    public Transform GetTransformByPos(EPlayerPostion pos)
    {
        return mPositions[pos];
    }
}
