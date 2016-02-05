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

[AddComponentMenu("NGUI/Interaction/Better Grid")]
public class UIBetterGrid : MonoBehaviour
{
	public delegate void OnInitializeItem (GameObject go, int wrapIndex, int realIndex);

	public int itemSize = 200;
	public int itemSizeY = 265;

	private bool cullContent = true;

//	private bool hideInactive = false;

	public Transform mTrans;
	protected UIPanel mPanel;
	protected UIScrollView mScroll;
	protected bool mHorizontal = false;
//	protected List<Transform> mChildren = new List<Transform>();
	public List<GameObject> mChildren = new List<GameObject>();

	private bool isBuy;

	public void init () {
		if (!CacheScrollView()) return;
		if (mScroll != null) mScroll.GetComponent<UIPanel>().onClipMove = OnMove;
	}

	protected virtual void OnMove (UIPanel panel) { WrapContent(); }
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

	private int getSN (string name){
		string[] sn = name.Split("_"[0]);
		return int.Parse(sn[2]);
	}

	private int getID (string name){
		string[] sn = name.Split("_"[0]);
		return int.Parse(sn[1]);
	}

	private int getSort{
		get {return PlayerPrefs.GetInt(ESave.SkillCardFilter.ToString(), EFilter.All.GetHashCode());}
	}

	private bool isNeedShow (string name) {
		int id = getID(name);
		if(getSort == (int)EFilter.All) 
			return true;
		else if(getSort == (int)EFilter.Active) 
			return GameFunction.IsActiveSkill(id);
		else if(getSort == (int)EFilter.Passive) 
			return !GameFunction.IsActiveSkill(id);
		
		return true;
	}

	public virtual void WrapContent ()
	{
		Vector3[] corners = mPanel.worldCorners;

		for (int i = 0; i < 4; ++i)
		{
			Vector3 v = corners[i];
			v = mTrans.InverseTransformPoint(v);
			corners[i] = v;
		}

		Vector3 center = Vector3.Lerp(corners[0], corners[2], 0.5f);
		if(isBuy) {
			if (mHorizontal)
			{
				float min = corners[0].x - itemSize;
				float max = corners[2].x + itemSize;
				if(mChildren.Count > 0) {
					for (int i = 0, imax = mChildren.Count; i < imax; ++i)
					{
						Transform t = mChildren[i].transform;
						if(t != null) {
							if(isSkillCardCanSell(getSN(mChildren[i].name)) && isNeedShow(mChildren[i].name)) {
								float distance = t.localPosition.x - center.x;
								
								if (cullContent)
								{
									distance += mPanel.clipOffset.x - mTrans.localPosition.x;
									if (!UICamera.IsPressed(t.gameObject))
										NGUITools.SetActive(t.gameObject, (distance > min && distance < max), false);
								}
							}else {
								NGUITools.SetActive(t.gameObject, false, false);
							}
						} 
					}
				}
			}
		} else {
			if (mHorizontal)
			{
				float min = corners[0].x - itemSize;
				float max = corners[2].x + itemSize;
				if(mChildren.Count > 0) {
					for (int i = 0, imax = mChildren.Count; i < imax; ++i)
					{
						Transform t = mChildren[i].transform;
						if(t != null) {
							if(isNeedShow(mChildren[i].name)) {
								float distance = t.localPosition.x - center.x;
								
								if (cullContent)
								{
									distance += mPanel.clipOffset.x - mTrans.localPosition.x;
									if (!UICamera.IsPressed(t.gameObject))
										NGUITools.SetActive(t.gameObject, (distance > min && distance < max), false);
								}
							}
						}
					}
				}
			}
		}

		mScroll.restrictWithinPanel = true;
	}

	private bool isSkillCardCanSell(int sn) {
		if(GameData.Team.PlayerBank != null && GameData.Team.PlayerBank.Length > 0) {
			for (int i=0; i<GameData.Team.PlayerBank.Length; i++) {
				if(GameData.Team.PlayerBank[i].ID != GameData.Team.Player.ID) {
					if(GameData.Team.PlayerBank[i].SkillCardPages != null && GameData.Team.PlayerBank[i].SkillCardPages.Length > 0) {
						for (int j=0; j<GameData.Team.PlayerBank[i].SkillCardPages.Length; j++) {
							int[] SNs = GameData.Team.PlayerBank[i].SkillCardPages[j].SNs;
							if (SNs.Length > 0) {
								for (int k=0; k<SNs.Length; k++)
									if (SNs[k] == sn)
										return false;
							}
						}
					}
				}
			}
		}

		for(int i=0; i<GameData.Team.Player.SkillCards.Length; i++) 
			if(GameData.Team.Player.SkillCards[i].SN == sn)
				return false;

		return true;
	}

	public bool IsBuy {
		set {isBuy = value;}
	}
}
