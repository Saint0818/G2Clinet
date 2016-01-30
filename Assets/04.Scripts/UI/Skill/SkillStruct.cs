using UnityEngine;
using GameStruct;
//Evolution
public struct TEvolution {
	public int Money;
	public TSkill[] SkillCards;
	public TSkill[] PlayerSkillCards;
	public TMaterialItem[] MaterialItems;
	public TTeamRecord LifetimeRecord;
}

public struct TSkillCardValue {
	public GameObject[] AttrView;
	public UILabel[] GroupLabel;
	public UILabel[] ValueLabel0;

	public void Init (Transform t) {
		AttrView = new GameObject[6];
		GroupLabel = new UILabel[6];
		ValueLabel0 = new UILabel[6];

		for(int i=0; i<AttrView.Length; i++) {
			AttrView[i] = t.FindChild("AttrView" + i.ToString()).gameObject;
			GroupLabel[i] = AttrView[i].transform.FindChild("GroupLabel").GetComponent<UILabel>();
			ValueLabel0[i] = AttrView[i].transform.FindChild("ValueLabel0").GetComponent<UILabel>();
		}
	}

	public void HideAll () {
		for(int i=0; i<AttrView.Length; i++) 
			AttrView[i].SetActive(false);	
	}

	public void UpdateView(TSkill skill) {
		HideAll ();
		int index = 0;
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(GameData.DSkillData[skill.ID].space > 0) {
				AttrView[index].SetActive(true);	
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].Space(skill.Lv).ToString();
				index ++;
			}
			if(GameData.DSkillData[skill.ID].distance > 0) {
				AttrView[index].SetActive(true);	
				if(GameFunction.IsActiveSkill(skill.ID)) {
					GroupLabel[index].text = TextConst.S(7207);
					ValueLabel0[index].text = GameData.DSkillData[skill.ID].MaxAnger.ToString();
				} else {
					GroupLabel[index].text = TextConst.S(7206);
					ValueLabel0[index].text = GameData.DSkillData[skill.ID].Rate(skill.Lv).ToString();
				}
				index ++;
			}
			if(GameData.DSkillData[skill.ID].aniRate > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7404);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].AniRate(skill.Lv).ToString();
				index ++;
			}
			if(GameData.DSkillData[skill.ID].distance > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7405);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].Distance(skill.Lv).ToString();
				index ++;
			}
			if(GameData.DSkillData[skill.ID].valueBase > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(10500 + GameData.DSkillData[skill.ID].AttrKind);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].Value(skill.Lv).ToString();
				index ++;
			}
			if(GameData.DSkillData[skill.ID].lifeTime > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7406);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].LifeTime(skill.Lv).ToString();
				index ++;
			}
		}
	}
}

public struct TSkillCardMaterial {
	public GameObject[] mMaterial;
	public UIButton[] MaterialItem;
	public UISprite[] ElementPic;
	public UILabel[] NameLabel;
	public UILabel[] AmountLabel; // 99/99
	public GameObject[] RedPoint;

	public TSkill mSkill;
	public int material1index;
	public int material2index;
	public int material3index;
	public int material1count;
	public int material2count;
	public int material3count;

	public void Init (GameObject obj) {
		mMaterial = new GameObject[3];
		MaterialItem = new UIButton[3];
		ElementPic = new UISprite[3];
		NameLabel = new UILabel[3];
		AmountLabel = new UILabel[3];
		RedPoint = new GameObject[3];

		for (int i=0; i<3; i++) {
			mMaterial[i] = obj.transform.FindChild("ElementSlot" + i.ToString()).gameObject;
			MaterialItem[i] = obj.transform.FindChild("ElementSlot" + i.ToString() + "/View/MaterialItem").GetComponent<UIButton>();
			ElementPic[i] = obj.transform.FindChild("ElementSlot" + i.ToString() + "/View/MaterialItem/ElementPic").GetComponent<UISprite>();
			NameLabel[i] = obj.transform.FindChild("ElementSlot" + i.ToString() + "/View/MaterialItem/NameLabel").GetComponent<UILabel>();
			AmountLabel[i] = obj.transform.FindChild("ElementSlot" + i.ToString() + "/View/MaterialItem/AmountLabel").GetComponent<UILabel>();
			RedPoint[i] =  obj.transform.FindChild("ElementSlot" + i.ToString() + "/View/MaterialItem/RedPoint").gameObject;
		}
	}

	public void UpdateView (TSkill skill) {
		HideAllMaterial ();
		material1index = -1;
		material2index = -1;
		material3index = -1;
		mSkill = skill;
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(GameData.DSkillData[skill.ID].Material1 != 0 && GameData.DSkillData[skill.ID].MaterialNum1 != 0) {
				mMaterial[0].SetActive(true);
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[skill.ID].Material1)) {
					if(GameData.DItemAtlas.ContainsKey(GameData.AtlasName(GameData.DItemData[GameData.DSkillData[skill.ID].Material1].Atlas))) {
						ElementPic[0].atlas = GameData.DItemAtlas[GameData.AtlasName(GameData.DItemData[GameData.DSkillData[skill.ID].Material1].Atlas)];
					}
					ElementPic[0].spriteName = "Item_" + GameData.DItemData[GameData.DSkillData[skill.ID].Material1].Icon;
					NameLabel[0].text = GameData.DItemData[GameData.DSkillData[skill.ID].Material1].Name;

					TMaterialItem materialSkillCard = new TMaterialItem();
					material1index = GameData.Team.FindMaterialItem(GameData.DSkillData[skill.ID].Material1, ref materialSkillCard);

					if(material1index != -1)
						AmountLabel[0].text = materialSkillCard.Num + "/" + GameData.DSkillData[skill.ID].MaterialNum1.ToString();
					else 
						AmountLabel[0].text = "0/" + GameData.DSkillData[skill.ID].MaterialNum1.ToString();

					RedPoint[0].SetActive((materialSkillCard.Num >= GameData.DSkillData[skill.ID].MaterialNum1));

					material1count = materialSkillCard.Num;
				}
			}

			if(GameData.DSkillData[skill.ID].Material2 != 0 && GameData.DSkillData[skill.ID].MaterialNum2 != 0) {
				mMaterial[1].SetActive(true);
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[skill.ID].Material2)) {
					if(GameData.DItemAtlas.ContainsKey(GameData.AtlasName(GameData.DItemData[GameData.DSkillData[skill.ID].Material2].Atlas))) {
						ElementPic[1].atlas = GameData.DItemAtlas[GameData.AtlasName(GameData.DItemData[GameData.DSkillData[skill.ID].Material2].Atlas)];
					}
					ElementPic[1].spriteName = "Item_" + GameData.DItemData[GameData.DSkillData[skill.ID].Material2].Icon;
					NameLabel[1].text = GameData.DItemData[GameData.DSkillData[skill.ID].Material2].Name;

					TMaterialItem materialSkillCard = new TMaterialItem();
					material2index = GameData.Team.FindMaterialItem(GameData.DSkillData[skill.ID].Material2, ref materialSkillCard);

					if(material2index != -1)
						AmountLabel[1].text = materialSkillCard.Num + "/" + GameData.DSkillData[skill.ID].MaterialNum2.ToString();
					else 
						AmountLabel[1].text = "0/" + GameData.DSkillData[skill.ID].MaterialNum2.ToString();
					
					RedPoint[1].SetActive((materialSkillCard.Num >= GameData.DSkillData[skill.ID].MaterialNum2));

					material2count = materialSkillCard.Num;
				}
			}

			if(GameData.DSkillData[skill.ID].Material3 != 0 && GameData.DSkillData[skill.ID].MaterialNum3 != 0) {
				mMaterial[2].SetActive(true);
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[skill.ID].Material3)) {
					if(GameData.DItemAtlas.ContainsKey(GameData.AtlasName(GameData.DItemData[GameData.DSkillData[skill.ID].Material3].Atlas))) {
						ElementPic[2].atlas = GameData.DItemAtlas[GameData.AtlasName(GameData.DItemData[GameData.DSkillData[skill.ID].Material3].Atlas)];
					}
					ElementPic[2].spriteName = "Item_" + GameData.DItemData[GameData.DSkillData[skill.ID].Material3].Icon;
					NameLabel[2].text = GameData.DItemData[GameData.DSkillData[skill.ID].Material3].Name;

					TMaterialItem materialSkillCard = new TMaterialItem();
					material3index = GameData.Team.FindMaterialItem(GameData.DSkillData[skill.ID].Material3, ref materialSkillCard);

					if(material3index != -1)
						AmountLabel[2].text = materialSkillCard.Num + "/" + GameData.DSkillData[skill.ID].MaterialNum3.ToString();
					else 
						AmountLabel[2].text = "0/" + GameData.DSkillData[skill.ID].MaterialNum3.ToString();

					RedPoint[2].SetActive((materialSkillCard.Num >= GameData.DSkillData[skill.ID].MaterialNum3));

					material3count = materialSkillCard.Num;
				}
			}
		}
	}

	public bool IsEnoughMaterial {
		get {
			bool flag1 = true;
			bool flag2 = true;
			bool flag3 = true;
			if(GameData.DSkillData.ContainsKey(mSkill.ID) || GameData.DSkillData[mSkill.ID].EvolutionSkill != 0) {
				if(!GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material1) && 
					!GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material2) &&
					!GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material3))
					return false;

				if(GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material1)) {
					if(material1count >= GameData.DSkillData[mSkill.ID].MaterialNum1)
						flag1 = true;
					else 
						flag1 = false;
				}

				if(GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material2)) {
					if(material2count >= GameData.DSkillData[mSkill.ID].MaterialNum2)
						flag2 = true;
					else
						flag2 = false;
				} 

				if(GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material3)) {
					if(material3count >= GameData.DSkillData[mSkill.ID].MaterialNum3)
						flag3 = true;
					else
						flag3 = false;
				} 

				if(flag1 && flag2 && flag3)
					return true;
				else
					return false;
			} else 
				return false;

		}
	}

	public void HideAllMaterial () {
		for (int i=0; i<mMaterial.Length; i++) 
			mMaterial[i].SetActive(false);
	}
}

//=========================================================================================================
//Reinforce
public struct TReinforceCallBack {
	public TSkill[] SkillCards;
	public TSkill[] PlayerSkillCards;
	public int Money;
	public TTeamRecord LifetimeRecord;
}

//LeftView
public struct TExpView {
	public GameObject ExpView;
	public UISlider ProgressBar;
	public UISlider ProgressBar2;
	public UILabel NextLevelLabel;
	public UILabel GetLevelLabel;
	public GameObject BarFullFX;

	private int currentExp;
	private int maxExp;

	public void Init (Transform t) {
		ExpView = t.FindChild("Window/Center/LeftView/EXPView").gameObject;
		ProgressBar = ExpView.transform.FindChild("ProgressBar").GetComponent<UISlider>();
		ProgressBar2 = ExpView.transform.FindChild("ProgressBar2").GetComponent<UISlider>();
		NextLevelLabel = ExpView.transform.FindChild("NextLevelLabel").GetComponent<UILabel>();
		GetLevelLabel = ExpView.transform.FindChild("GetLevelLabel").GetComponent<UILabel>();
		BarFullFX = ExpView.transform.FindChild("BarFullFX").gameObject;

		if(ExpView == null || ProgressBar == null || ProgressBar2 == null || BarFullFX == null)
			Debug.LogError("TExpStruct not init");
		if(BarFullFX != null)
			BarFullFX.SetActive(false);
	}

	public void UpdateView (TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			maxExp = GameData.DSkillData[skill.ID].GetUpgradeExp(skill.Lv);
			if(skill.Lv >= GameData.DSkillData[skill.ID].MaxStar) {
				currentExp = GameData.DSkillData[skill.ID].GetUpgradeExp(skill.Lv);
				SetTopProgressView ();
			} else {
				currentExp = skill.Exp;
				ProgressBar.value = (float)currentExp / (float)maxExp;
				ProgressBar2.value = (float)currentExp/ (float)maxExp;
				NextLevelLabel.text = string.Format(TextConst.S(7407), GameData.DSkillData[skill.ID].GetUpgradeExp(skill.Lv));
			}
			GetLevelLabel.text = string.Format(TextConst.S(7408), 0);
		}
	}

	public void SetUpgradeView (int id, int lv, int originalExp, int upgradeExp) {
		if(GameData.DSkillData.ContainsKey(id)) {
			if(lv < GameData.DSkillData[id].MaxStar) {
				ProgressBar2.value = (float)(originalExp + upgradeExp)/ (float)maxExp;
				GetLevelLabel.text = string.Format(TextConst.S(7408), upgradeExp);
			}
		}
	}

	public void SetProgressView (int id, int lv, int yellowExpValue, int greenExpValue, int getLevelExpValue) {
		if(GameData.DSkillData.ContainsKey(id)) {
			maxExp = GameData.DSkillData[id].GetUpgradeExp(lv);
			NextLevelLabel.text = string.Format(TextConst.S(7407), maxExp);
			ProgressBar.value = (float)yellowExpValue / (float)maxExp;
			ProgressBar2.value = (float)greenExpValue / (float)maxExp;
			GetLevelLabel.text = string.Format(TextConst.S(7408), getLevelExpValue);
		}
	}

	public void SetTopProgressView () {
		ProgressBar.value = 0;
		ProgressBar2.value = 1;
		NextLevelLabel.text = TextConst.S(7409);
		GetLevelLabel.text = string.Format(TextConst.S(7408), 0);
	}

	public void ShowFull () {
		BarFullFX.SetActive(false);
		BarFullFX.SetActive(true);
	}
}

public struct TCostView {
	public GameObject CostView;
	public UILabel FirstLabel;
	public UILabel SecondLabel;

	public void Init (Transform t) {
		CostView = t.FindChild("Window/Center/LeftView/CostView").gameObject;
		FirstLabel = CostView.transform.FindChild("ValueLabel0").GetComponent<UILabel>();
		SecondLabel = CostView.transform.FindChild("ValueLabel1").GetComponent<UILabel>();
		SecondLabel.color = Color.white;

		if(CostView == null || FirstLabel == null || SecondLabel == null)
			Debug.LogError("CostView not Init");
	}

	public void UpdateView (TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			FirstLabel.text = GameData.DSkillData[skill.ID].Space(skill.Lv).ToString();
			SecondLabel.text = GameData.DSkillData[skill.ID].Space(skill.Lv).ToString();
			SecondLabel.color = Color.white;
		}
	}

	public void UpgradeView (TSkill skill , int newLv) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(skill.Lv < GameData.DSkillData[skill.ID].MaxStar && newLv <= GameData.DSkillData[skill.ID].MaxStar) {
				SecondLabel.text = GameData.DSkillData[skill.ID].Space(newLv).ToString();
				if(GameData.DSkillData[skill.ID].Space(newLv) > GameData.DSkillData[skill.ID].Space(skill.Lv))
					SecondLabel.color = Color.green;
				else 
					SecondLabel.color = Color.white;
			}
		}
	}
}

public struct TEnergyView {
	public GameObject EnergyView;
	public UILabel TitleLabel;
	public UILabel FirstLabel;
	public UILabel SecondLabel;

	public void Init (Transform t) {
		EnergyView = t.FindChild("Window/Center/LeftView/EnergyView").gameObject;
		TitleLabel = EnergyView.transform.FindChild("GroupLabel").GetComponent<UILabel>();
		FirstLabel = EnergyView.transform.FindChild("ValueLabel0").GetComponent<UILabel>();
		SecondLabel = EnergyView.transform.FindChild("ValueLabel1").GetComponent<UILabel>();
		SecondLabel.color = Color.white;

		if(EnergyView == null || FirstLabel == null || SecondLabel == null)
			Debug.LogError("TEnergyView not Init");
	}

	public void UpdateView (TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(GameFunction.IsActiveSkill(skill.ID)) {
				TitleLabel.text = TextConst.S(7207);
				FirstLabel.text = GameData.DSkillData[skill.ID].MaxAnger.ToString();
				SecondLabel.text = GameData.DSkillData[skill.ID].MaxAnger.ToString();
			} else {
				TitleLabel.text = TextConst.S(7206);
				FirstLabel.text = GameData.DSkillData[skill.ID].Rate(skill.Lv).ToString();
				SecondLabel.text = GameData.DSkillData[skill.ID].Rate(skill.Lv).ToString();
			}
			SecondLabel.color = Color.white;
		}
	}

	public void UpgradeView (TSkill skill , int newLv) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(skill.Lv < GameData.DSkillData[skill.ID].MaxStar && newLv <= GameData.DSkillData[skill.ID].MaxStar) {
				if(GameFunction.IsActiveSkill(skill.ID)) {
					SecondLabel.text = GameData.DSkillData[skill.ID].MaxAnger.ToString();
				} else {
					SecondLabel.text = GameData.DSkillData[skill.ID].Rate(newLv).ToString();
					if(GameData.DSkillData[skill.ID].Rate(newLv) > GameData.DSkillData[skill.ID].Rate(skill.Lv))
						SecondLabel.color = Color.green;
					else 
						SecondLabel.color = Color.white;
				}
			}
		}
	}
}

//CenterView
public struct TReinforceInfo {
	public GameObject[] AttrView;
	public UILabel[] GroupLabel;
	public UILabel[] ValueLabel0;
	public UILabel[] ValueLabel1;

	public void Init (Transform t) {
		AttrView = new GameObject[4];
		GroupLabel = new UILabel[4];
		ValueLabel0 = new UILabel[4];
		ValueLabel1 = new UILabel[4];

		for(int i=0; i<AttrView.Length; i++) {
			AttrView[i] = t.FindChild("AttrView" + i.ToString()).gameObject;
			GroupLabel[i] = AttrView[i].transform.FindChild("GroupLabel").GetComponent<UILabel>();
			ValueLabel0[i] = AttrView[i].transform.FindChild("ValueLabel0").GetComponent<UILabel>();
			ValueLabel1[i] = AttrView[i].transform.FindChild("ValueLabel1").GetComponent<UILabel>();
			ValueLabel1[i].color = Color.white;
		}
	}

	private void hideAll () {
		for(int i=0; i<AttrView.Length; i++) 
			AttrView[i].SetActive(false);	
	}
	//AttrKindRate  AniRate
	//Distance 
	//AttrKind Value
	//LifeTime 
	public void UpdateView(TSkill skill) {
		hideAll ();
		int index = 0;
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(GameData.DSkillData[skill.ID].aniRate > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7404);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].AniRate(skill.Lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[skill.ID].AniRate(skill.Lv).ToString();
				ValueLabel1[index].color = Color.white;
				index ++;
			}
			if(GameData.DSkillData[skill.ID].distance > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7405);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].Distance(skill.Lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[skill.ID].Distance(skill.Lv).ToString();
				ValueLabel1[index].color = Color.white;
				index ++;
			}
			if(GameData.DSkillData[skill.ID].valueBase > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(10500 + GameData.DSkillData[skill.ID].AttrKind);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].Value(skill.Lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[skill.ID].Value(skill.Lv).ToString();
				ValueLabel1[index].color = Color.white;
				index ++;
			}
			if(GameData.DSkillData[skill.ID].lifeTime > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7406);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].LifeTime(skill.Lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[skill.ID].LifeTime(skill.Lv).ToString();
				ValueLabel1[index].color = Color.white;
				index ++;
			}
		}
	}

	public void UpgradeView (TSkill skill, int newLv) {
		int index = 0;
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(skill.Lv < GameData.DSkillData[skill.ID].MaxStar && newLv <= GameData.DSkillData[skill.ID].MaxStar) {
				if(GameData.DSkillData[skill.ID].aniRate > 0) {
					ValueLabel1[index].text = GameData.DSkillData[skill.ID].AniRate(newLv).ToString();
					if(GameData.DSkillData[skill.ID].AniRate(newLv) > GameData.DSkillData[skill.ID].AniRate(skill.Lv)) {
						ValueLabel1[index].color = Color.green;
					} else {
						ValueLabel1[index].color = Color.white;
					}
					index ++;
				}
				if(GameData.DSkillData[skill.ID].distance > 0) {
					ValueLabel1[index].text = GameData.DSkillData[skill.ID].Distance(newLv).ToString();
					if(GameData.DSkillData[skill.ID].Distance(newLv) > GameData.DSkillData[skill.ID].Distance(skill.Lv)) {
						ValueLabel1[index].color = Color.green;
					} else {
						ValueLabel1[index].color = Color.white;
					}
					index ++;
				}
				if(GameData.DSkillData[skill.ID].valueBase > 0) {
					ValueLabel1[index].text = GameData.DSkillData[skill.ID].Value(newLv).ToString();
					if(GameData.DSkillData[skill.ID].Value(newLv) > GameData.DSkillData[skill.ID].Value(skill.Lv)) {
						ValueLabel1[index].color = Color.green;
					} else {
						ValueLabel1[index].color = Color.white;
					}
					index ++;
				}
				if(GameData.DSkillData[skill.ID].lifeTime > 0) {
					ValueLabel1[index].text = GameData.DSkillData[skill.ID].LifeTime(newLv).ToString();
					if(GameData.DSkillData[skill.ID].LifeTime(newLv) > GameData.DSkillData[skill.ID].LifeTime(skill.Lv)) {
						ValueLabel1[index].color = Color.green;
					} else {
						ValueLabel1[index].color = Color.white;
					}
					index ++;
				}
			}
		}
	}

	public void UpgradeViewForLevelUp (TSkill skill, TSkill newSkill) {
		hideAll ();
		int index = 0;
		if(GameData.DSkillData.ContainsKey(skill.ID) && GameData.DSkillData.ContainsKey(newSkill.ID)) {
			if(GameData.DSkillData[skill.ID].aniRate > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7404);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].AniRate(skill.Lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[newSkill.ID].AniRate(newSkill.Lv).ToString();
				if(GameData.DSkillData[newSkill.ID].AniRate(newSkill.Lv) > GameData.DSkillData[skill.ID].AniRate(skill.Lv)) {
					ValueLabel1[index].gameObject.SetActive(true);
					ValueLabel1[index].color = Color.green;
				} else {
					ValueLabel1[index].gameObject.SetActive(false);
				}
				index ++;
			}

			if(GameData.DSkillData[skill.ID].distance > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7405);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].Distance(skill.Lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[newSkill.ID].Distance(newSkill.Lv).ToString();
				if(GameData.DSkillData[newSkill.ID].Distance(newSkill.Lv) > GameData.DSkillData[skill.ID].Distance(skill.Lv)) {
					ValueLabel1[index].gameObject.SetActive(true);
					ValueLabel1[index].color = Color.green;
				} else {
					ValueLabel1[index].gameObject.SetActive(false);
				}
				index ++;
			}

			if(GameData.DSkillData[skill.ID].valueBase > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(10500 + GameData.DSkillData[skill.ID].AttrKind);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].Value(skill.Lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[newSkill.ID].Value(newSkill.Lv).ToString();
				if(GameData.DSkillData[newSkill.ID].Value(newSkill.Lv) > GameData.DSkillData[skill.ID].Value(skill.Lv)) {
					ValueLabel1[index].gameObject.SetActive(true);
					ValueLabel1[index].color = Color.green;
				} else {
					ValueLabel1[index].gameObject.SetActive(false);
				}
				index ++;
			}

			if(GameData.DSkillData[skill.ID].lifeTime > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7406);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].LifeTime(skill.Lv).ToString();
				ValueLabel1[index].text = GameData.DSkillData[newSkill.ID].LifeTime(newSkill.Lv).ToString();
				if(GameData.DSkillData[newSkill.ID].LifeTime(newSkill.Lv) > GameData.DSkillData[skill.ID].LifeTime(skill.Lv)) {
					ValueLabel1[index].gameObject.SetActive(true);
					ValueLabel1[index].color = Color.green;
				} else {
					ValueLabel1[index].gameObject.SetActive(false);
				}
			}
		}
	}
}