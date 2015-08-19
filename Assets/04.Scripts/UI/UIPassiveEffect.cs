﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class UIPassiveEffect : UIBase {
	private static UIPassiveEffect instance = null;
	private const string UIName = "UIPassiveEffect";

	private GameObject[] uiCardMotion = new GameObject[3];
	private Animator[] animatorCardGroup = new Animator[3];
	private UISprite[] spriteCardFrame = new UISprite[3];
	private UITexture[] textureCardInfo = new UITexture[3];
	private UILabel[] labelCardLabel = new UILabel[3];

	private float[] timers = new float[3];
	private float[] delayCloseTimers = new float[3];
	private int[] cardPicNos = new int[3];
	private int[] cardLVs = new int[3];
	private string[] cardNames = new string[3];

	private int[] recordIndex = new int[3];

	public static bool Visible{
		get{
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static UIPassiveEffect Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIPassiveEffect;
			
			return instance;
		}
	}

	public static void UIShow(bool isShow){
		if (instance) {
			instance.Show(isShow);
		} else
			if (isShow)
				Get.Show(isShow);
	}

	protected override void InitCom() {
		for(int i=0; i<uiCardMotion.Length; i++) {
			uiCardMotion[i] = GameObject.Find (UIName + "/Center/CardMotion_" + (i+1).ToString());
			uiCardMotion[i].transform.localPosition = new Vector3(500, 200, 0);
			uiCardMotion[i].SetActive(false);
			animatorCardGroup[i] = GameObject.Find (UIName + "/Center/CardMotion_" + (i+1).ToString() + "/CardGroup").GetComponent<Animator>();
			spriteCardFrame[i] = GameObject.Find (UIName + "/Center/CardMotion_" + (i+1).ToString() + "/CardGroup/CardFrame").GetComponent<UISprite>();
			textureCardInfo[i] = GameObject.Find (UIName + "/Center/CardMotion_" + (i+1).ToString() + "/CardGroup/CardInfo").GetComponent<UITexture>();
			labelCardLabel[i] = GameObject.Find (UIName + "/Center/CardMotion_" + (i+1).ToString() + "/CardGroup/CardLabel").GetComponent<UILabel>();
		}

		for (int i=0; i<recordIndex.Length; i++) {
			recordIndex[i] = -1;
		}
	}

	private int getRecordCount () {
		int result = 0;
		for (int i=0; i<recordIndex.Length; i++) {
			if(recordIndex[i] != -1)
				result ++;
		}
		return result;
	}

	private int indexOf (int index){
		int result = -1;
		for (int i=0; i<recordIndex.Length; i++) {
			if(recordIndex[i] == index)
				result = i;
		}
		return result;
	}

	private bool contains(int index) {
		for (int i=0; i<recordIndex.Length; i++) {
			if(recordIndex[i] == index)
				return true;
		}
		return false;
	}

	private void addValue (int index) {
		recordIndex[2] = recordIndex[1];
		recordIndex[1] = recordIndex[0];
		recordIndex[0] = index;
	}

	private void initCard (int index, int picNo = 0, int lv = 0, string name = "") {
		timers[index] = 2;
		delayCloseTimers[index] = 999;
		cardPicNos[index] = picNo;
		cardLVs[index] = lv;
		cardNames[index] = name;
		showCard();
	}

	private void showCard () {
		uiCardMotion[recordIndex[0]].SetActive(false);
		uiCardMotion[recordIndex[0]].SetActive(true);
		uiCardMotion[recordIndex[0]].transform.DOKill(true);
		uiCardMotion[recordIndex[0]].transform.localPosition = new Vector3(500, 200, 0);
		if(getRecordCount() < 3) {
			for(int i=1; i<recordIndex.Length; i++) {
				if(recordIndex[i] != -1)
					uiCardMotion[recordIndex[i]].transform.DOLocalMoveX(500 - (200 * (getRecordCount () - i)), 0.2f);
			}
		} else {
			uiCardMotion[recordIndex[1]].transform.DOLocalMoveX(300, 0.2f);
			uiCardMotion[recordIndex[2]].transform.DOLocalMoveX(100, 0.2f);
			hideCard(recordIndex[2]);
		}
		spriteCardFrame[recordIndex[0]].spriteName = "SkillCard" + cardLVs[recordIndex[0]].ToString();
		textureCardInfo[recordIndex[0]].mainTexture = GameData.CardTextures[cardPicNos[recordIndex[0]]];
		labelCardLabel[recordIndex[0]].text = cardNames[recordIndex[0]];
		timers[recordIndex[0]] = 2;
	}
	
	private void hideCard (int index) {
		animatorCardGroup[index].SetTrigger("Close");
		delayCloseTimers[index] = 0.2f;
	}

	public void ShowCard (PlayerBehaviour player = null, int picNo = 0, int lv = 0, string name = ""){
		if(!Visible)
			Show(true);

		EffectManager.Get.PlayEffect("PassiveFX", Vector3.zero, player.gameObject, null, 0.5f);

		for (int i=0; i<recordIndex.Length; i++) {
			if(!contains(i)) {
				addValue(i);
				initCard(i, picNo, lv, name);
				break;
			} else {
				if(recordIndex[recordIndex.Length - 1] == i) {
					addValue(i);
					initCard(i, picNo, lv, name);
					break;
				}
			}
		}
	}
	
	void FixedUpdate () {

		for (int i=0; i< timers.Length; i++) {
			if (timers[i] > 0) {
				timers[i] -= Time.deltaTime;
				if (timers[i] <= 0) {
					timers[i] = 0;
					hideCard(i);
				}
			}
		}

		for (int i=0; i< delayCloseTimers.Length; i++) {
			if (delayCloseTimers[i] > 0) {
				delayCloseTimers[i] -= Time.deltaTime;
				if (delayCloseTimers[i] <= 0 ) {
					delayCloseTimers[i] = 0;
					uiCardMotion[i].SetActive(false);
					if(indexOf(i) != -1)
						recordIndex[indexOf(i)] = -1;
				}
			}
		}
	}
}
