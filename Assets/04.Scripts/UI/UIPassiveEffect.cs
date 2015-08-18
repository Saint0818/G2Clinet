using UnityEngine;
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
	private float[] delaytTimers = new float[3];
	private float[] delayCloseTimers = new float[3];
	private int[] cardPicNos = new int[3];
	private int[] cardLVs = new int[3];
	private string[] cardNames = new string[3];
	
	private GameObject[] uiPassive = new GameObject[6];
	private float[] passiveTimers = new float[6];

//	private List<int> recordIndex = new List<int>();
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

		for (int i=0; i<uiPassive.Length; i++) {
			uiPassive[i] = GameObject.Find (UIName + "/Center/FX_Passive_" + (i+1).ToString());
			uiPassive[i].SetActive(false);
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
		delaytTimers[index] = 0.25f;
		timers[index] = 2;
		delayCloseTimers[index] = 999;
		cardPicNos[index] = picNo;
		cardLVs[index] = lv;
		cardNames[index] = name;
	}

	private void showCard (int index) {
		uiCardMotion[index].SetActive(false);
		uiCardMotion[index].SetActive(true);
		if(getRecordCount() < 3) {
			for(int i=1; i<recordIndex.Length; i++) {
				if(recordIndex[i] != -1)
					uiCardMotion[recordIndex[i]].transform.DOLocalMoveX(500 - (200 * (getRecordCount () - i)), 0.2f);
			}
			uiCardMotion[index].transform.localPosition = new Vector3(500, 200, 0);
		} else {
			uiCardMotion[recordIndex[0]].transform.localPosition = new Vector3(500, 200, 0);
			uiCardMotion[recordIndex[1]].transform.DOLocalMoveX(300, 0.2f);
			uiCardMotion[recordIndex[2]].transform.DOLocalMoveX(100, 0.2f);
			hideCard(recordIndex[2]);
		}
		spriteCardFrame[index].spriteName = "SkillCard" + cardLVs[index].ToString();
		textureCardInfo[index].mainTexture = GameData.CardTextures[cardPicNos[index]];
		labelCardLabel[index].text = cardNames[index];
		timers[index] = 2;


	}
	
	private void hideCard (int index) {
		animatorCardGroup[index].SetTrigger("Close");
		delayCloseTimers[index] = 0.2f;
	}

	public void ShowCard (PlayerBehaviour player = null, int picNo = 0, int lv = 0, string name = ""){
		if(!Visible)
			Show(true);

		for(int i=0; i<uiPassive.Length; i++) {
			if(!uiPassive[i].activeInHierarchy) {
				uiPassive[i].transform.localPosition = new Vector2(CameraMgr.Get.CourtCamera.WorldToScreenPoint(player.gameObject.transform.position).x - (Screen.width * 0.5f), 
				                                                   CameraMgr.Get.CourtCamera.WorldToScreenPoint(player.gameObject.transform.position).y - (Screen.height * 0.5f));
				uiPassive[i].SetActive(true);
				passiveTimers[i] = 0.5f;
				uiPassive[i].transform.DOLocalMove(new Vector3(500, 200, 0), 0.25f).SetEase(Ease.Linear);
				break;
			}
		}

		for (int i=0; i<recordIndex.Length; i++) {
			if(!contains(i)) {
				initCard(i, picNo, lv, name);
				addValue(i);
				break;
			} else {
				if(recordIndex[recordIndex.Length - 1] == i) {
					initCard(i, picNo, lv, name);
					addValue(i);
					break;
				}
			}
		}
	}
	
	void FixedUpdate () {

		for (int i=0; i<passiveTimers.Length; i++) {
			if(passiveTimers[i] > 0) {
				passiveTimers[i] -= Time.deltaTime;
				if(passiveTimers[i] <= 0) {
					passiveTimers[i] = 0;
					uiPassive[i].SetActive(false);
				}
			}
		}

		for (int i=0; i< delaytTimers.Length; i++) {
			if (delaytTimers[i] > 0) {
				delaytTimers[i] -= Time.deltaTime;
				if (delaytTimers[i] <= 0) {
					delaytTimers[i] = 0;
					showCard(i);
				}
			}
		}

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
