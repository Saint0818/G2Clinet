//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using GameEnum;

/// <summary>
/// This script makes it possible for a scroll view to wrap its content, creating endless scroll views.
/// Usage: simply attach this script underneath your scroll view where you would normally place a UIGrid:
/// 
/// + Scroll View
/// |- UIWrappedContent
/// |-- Item 1
/// |-- Item 2
/// |-- Item 3
/// </summary>

[AddComponentMenu("NGUI/Interaction/ReinForce Grid")]
public class UIReinForceGrid : MonoBehaviour
{
	//	public delegate void OnInitializeItem (GameObject go, int wrapIndex, int realIndex);

	public int itemSize = 80;

	private bool cullContent = true;

	//	private bool hideInactive = false;

	public Transform mTrans;
	protected UIPanel mPanel;
	protected UIScrollView mScroll;
	protected bool mHorizontal = false;
	public List<GameObject> mChildren = new List<GameObject>();

	public void init () {
		if (!CacheScrollView()) return;
		if (mScroll != null) mScroll.GetComponent<UIPanel>().onClipMove = OnMove;
	}

	protected virtual void OnMove (UIPanel panel) { WrapContent(true); }
	protected bool CacheScrollView ()
	{
		mTrans = transform;
		mPanel = NGUITools.FindInParents<UIPanel>(gameObject);
		mScroll = mPanel.GetComponent<UIScrollView>();
		if (mScroll == null) return false;
		if (mScroll.movement == UIScrollView.Movement.Horizontal) mHorizontal = true;
		else if (mScroll.movement == UIScrollView.Movement.Vertical) mHorizontal = false;
		else return false;
		return true;
	}

	public virtual void WrapContent (bool isMoving)
	{
		Vector3[] corners = mPanel.worldCorners;

		for (int i = 0; i < 4; ++i)
		{
			Vector3 v = corners[i];
			v = mTrans.InverseTransformPoint(v);
			corners[i] = v;
		}

		Vector3 center = Vector3.Lerp(corners[0], corners[2], 0.5f);
		if (!mHorizontal)
		{
			float min = corners[0].y - itemSize;
			float max = corners[2].y + itemSize;
			if(mChildren.Count > 0) {
				for (int i = 0, imax = mChildren.Count; i < imax; ++i)
				{
					Transform t = mChildren[i].transform;
					if(t != null) {
						float distance = t.localPosition.y - center.y;

						if (cullContent)
						{
							distance += mPanel.clipOffset.y - mTrans.localPosition.y;
							if (!UICamera.IsPressed(t.gameObject)) {
								if(isMoving ){
									if((distance > min && distance < max))
										NGUITools.SetActive(t.gameObject, true, false);
								} else 
									NGUITools.SetActive(t.gameObject, (distance > min && distance < max), false);
							}
						}
					}
				}
			}
		}

		mScroll.restrictWithinPanel = true;
	}
}
