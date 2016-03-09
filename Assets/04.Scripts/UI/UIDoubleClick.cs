using UnityEngine;
using System.Collections;

public delegate void IntDelegate (int lv);
public delegate void PlayerDelegate (int lv, PlayerBehaviour player);

public enum EDoubleClick
{
	Shoot,
	Block,
	Rebound
}

/// <summary>
/// 每個球員身上的bar條結構
/// 使用combo功能：段數：012345 ＋命中機率 2:才顯示美術字5:為max
/// 投籃：玩家可以控制其他隊友的二次點擊
/// 火鍋：玩家只能控制自己的二次點擊
/// </summary>
public struct TDoubleClick
{
	public int Team;
	public int Index;
	private GameObject Group;
	private UISprite runSprite;
//	private UISprite targetSprite;
	public float CrtValue;
//	private float CheckValue;
	private bool isInit;
	
	private IntDelegate finsh;
	private PlayerDelegate finshPalyer;
	private PlayerBehaviour crtPlayer;
	private float speed;
	public float FramSpeed;
	public bool IsStart;
	private EDoubleClick crtType;
	private GameObject DoubleClickEffect;
	private UISprite DoubleClickEffectSp;
	
	public void Init(GameObject obj)
	{
		string mainPath = "Scale/Billboard/QTEGroup/";
		Group = obj.transform.Find(mainPath).gameObject;
		finsh = null;
		finshPalyer = null;
		speed = 1;
		CrtValue = 800;
		FramSpeed =  CrtValue / (speed * 30);
		runSprite = Group.transform.Find ("BarSprite").gameObject.GetComponent<UISprite>();
		IsStart = false;
		Enable = false;
		Group.SetActive (false);

		DoubleClickEffect = obj.transform.Find("Scale/Billboard/Combo").gameObject;
		DoubleClickEffectSp = DoubleClickEffect.transform.Find("HitSprite").gameObject.GetComponent<UISprite>();
		DoubleClickEffect.SetActive(false);
	}

    //Combo最多四級超過一律顯示Max
	public void SetComBoEffect(int index)
	{
		if (index > 1) {
			DoubleClickEffect.SetActive (false);

			if(index > 4)
				DoubleClickEffectSp.spriteName = "maxhits";
			else
				DoubleClickEffectSp.spriteName = string.Format("{0}hits", index);

			DoubleClickEffect.SetActive (true);
		}
		else
			DoubleClickEffect.SetActive(false);	
	}
	
	public bool Clicked
	{
		set{ if(value){
				CheckLv();
			}
		}
	}
	
	public bool Enable
	{
		set{ 
			if (Group)
				Group.SetActive(value);
		}

		get{ 
			if(Group && Group.activeSelf)
				return true;
			else
				return false;}
	}
	
	public void CheckLv()
	{
		if (runSprite.transform.localPosition.y > 0 && runSprite.transform.localPosition.y < 35)
			SetLv(2, Index);
		else
			SetLv(0, Index);
	}
	
	public void SetData(EDoubleClick type, float speedvalue, IntDelegate intFunction = null, PlayerDelegate playerFunction = null ,PlayerBehaviour player = null)
	{
		crtType = type;
		Enable = true;
		finsh = intFunction;
		finshPalyer = playerFunction;
		crtPlayer = player;
		speed = speedvalue;
		FramSpeed = 800 / (speed * 30);
		IsStart = true;
	}
	
	private void SetLv(int index, int playerIndex)
	{
		switch (index) {
		case 0:
			GameController.Get.DoubleClickType = GameEnum.EDoubleType.Weak;
			UIDoubleClick.Get.ShowLvEffect(index, crtType, playerIndex);
            AudioMgr.Get.PlaySound(SoundType.SD_DCWeak);
			break;
		case 1:
			GameController.Get.DoubleClickType = GameEnum.EDoubleType.Good;
			UIDoubleClick.Get.ShowLvEffect(index, crtType, playerIndex);
			break;
		case 2:
			GameController.Get.DoubleClickType = GameEnum.EDoubleType.Perfect;
			UIDoubleClick.Get.ShowLvEffect(index, crtType, playerIndex);
            AudioMgr.Get.PlaySound(SoundType.SD_DCPerfect);
			break;
		}
		if (index != -1) {
			if(finsh != null)
				finsh(index);
			else if(crtPlayer != null)
				finshPalyer(index, crtPlayer);
		}
	}

	public void ClickStop()
	{
		if (Enable) {
			IsStart = false;
			CrtValue = 800;
			CheckLv ();
		}
	}

    //因為之前大小是800速度也都測好，所以才會用800去換算速率，這樣才不需要停整Event裡DoubleClick觸發的時機
	public void ValueCalculation()
	{
		if (IsStart) {
			CrtValue -= FramSpeed;
            //一開始runSprite y的位置會是-50的地方，終點是50,分成100等分再根據之前的速率，所以才得到以下公式;
			float y = 50 - (100 * CrtValue / 800);
			runSprite.transform.localPosition = new Vector3 (0, y, 0);

            //時間到自動關閉
			if (CrtValue <= 0) {
				ClickStop();
                Enable = false;
			}
		}
	}
}

/// <summary>
/// User interface double click.
/// </summary>
public class UIDoubleClick : UIBase {

	public TDoubleClick[] DoubleClicks = new TDoubleClick[6];

	private static UIDoubleClick instance = null;
	private const string UIName = "UIDoubleClick";

	public int Lv = -1;
	private Vector2 size;
	private Vector2 size2;
	private Vector2 SecondStartSize;
	public int Combo = 0;
	private ParticleSystem[] lvEffect = new ParticleSystem[2];
	private GameObject BottomRight; 

	public static UIDoubleClick Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIDoubleClick;
			
            return instance;
        }
    }
    
    public static bool Visible {
		get {
			if(instance)
				return (instance.gameObject.activeInHierarchy && instance.gameObject.activeSelf);
			else
				return false;
		}

		set {
			if (instance) {
				if (!value)
                    RemoveUI(instance.gameObject);
				else
					instance.Show(value);
			} else
				if (value)
					Get.Show(value);
		}
	}
	
	public static void UIShow(bool isShow) {
		if (instance)
			instance.Show (isShow);
		else 
		if (isShow)
			Get.Show (isShow);	
	}

	public void DoBtn(GameObject go, bool state)
	{
		GameController.Get.DoShoot (true);
	}

	void FixedUpdate()
	{
		for (int i = 0; i < DoubleClicks.Length; i++) {
			if(DoubleClicks[i].Enable && DoubleClicks[i].IsStart){
				DoubleClicks[i].ValueCalculation();
			}		
		}
	}
       
	public void SetData(EDoubleClick type, int playerIndex, float speed, IntDelegate intFunction = null, PlayerDelegate playerFunction = null ,PlayerBehaviour player = null)
	{
		if (playerIndex != -1 && playerIndex < DoubleClicks.Length) {
			UIShow(true);
			DoubleClicks[playerIndex].SetData(type, speed, intFunction, playerFunction, player);
			if(BottomRight)
				BottomRight.SetActive(true);

            AudioMgr.Get.PlaySound(SoundType.SD_DoubleClick);
		}
	}

	protected override void InitCom() {
		string name;

		for (int i = 0; i < lvEffect.Length; i++) {
			name = string.Format("UIDoubleClick/Lv/{0}", i);
			lvEffect[i] = GameObject.Find (name).GetComponent<ParticleSystem>();
			if(lvEffect[i] != null)
				lvEffect[i].gameObject.SetActive(false);
		}

		BottomRight = GameObject.Find (UIName + "/BottomRight");
		BottomRight.SetActive (false);
	}

	public void Reset (){
		Combo = 0;
	}

	public void InitDoubleClick(PlayerBehaviour player, int index)
	{
		if (index < DoubleClicks.Length && player != null && player.DoubleClick) {
			DoubleClicks[index].Init(player.DoubleClick);
			DoubleClicks[index].Team = (int)player.Team;
			DoubleClicks[index].Index = index;
		}
	}

	public void ShowLvEffect(int index, EDoubleClick type, int playerIndex)
	{
		Lv = index;
		BottomRight.SetActive(false);

		switch(Lv)
		{
			case 0:
				lvEffect[0].gameObject.SetActive(true);
				lvEffect[0].Play();
				if(type == EDoubleClick.Shoot)
					Combo = 0;
				break;
			case 1:
			case 2:
				if(type == EDoubleClick.Shoot)
					Combo++;
				lvEffect[1].gameObject.SetActive(true);
				lvEffect[1].Play();
				break;
		}

		if (type == EDoubleClick.Shoot && Lv > 0)
			DoubleClicks [playerIndex].SetComBoEffect (Combo);	
		else
			DoubleClicks [playerIndex].SetComBoEffect (0);
	}

	public void ClickStop(int index)
	{
        if (index != -1 && DoubleClicks[index].Team == 0)
        {
            DoubleClicks[index].ClickStop();
            StartCoroutine(DelayToCloseUI(index));
        }
	}

    private IEnumerator DelayToCloseUI(int index)
    {
        yield return new WaitForSeconds (1f);
        DoubleClicks[index].Enable = false;
    }
}