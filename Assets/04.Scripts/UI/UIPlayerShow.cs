using UnityEngine;

public class StorePlayerBehaviour : MonoBehaviour
{
	private int UsingSkill = 0;
	public GameObject ObjHandBall;
	
	public bool isFocus = false;
	
	void Awake()
	{   
		ObjHandBall = gameObject.transform.FindChild("DummyBall/HandBall").gameObject as GameObject;
		if (ObjHandBall == null)
			Debug.LogError("ObjHandBall is null");
		else
			ObjHandBall.SetActive(false); 
	}
	
	private void UseHandBall(bool flag)
	{
		if (ObjHandBall)
			ObjHandBall.SetActive(flag);
	} 
	
	void FixedUpdate()
	{  
		if (UsingSkill > 0 && !GetComponent<Animation>().isPlaying) {
			if(UIPlayerShow.Visible && UIPlayerShow.Get.BG && UIPlayerShow.Get.BG.enabled == false)
				UIPlayerShow.Get.BG.enabled = true;
			
			if(ObjHandBall.activeInHierarchy)
				ObjHandBall.SetActive(false);
			
			UsingSkill = 0;	
		}
	}
	
	public bool UseSkill(int skillNo)
	{
		return false;
	}
	
	public void Effect161(){}
	public void Effect162(){}
	public void Effect163(){}
	public void ActionOut(GameObject ball){}
	public void ActionStep1(GameObject ball){}
	public void JumperAnimationStart(){}
	public void BlockAnimationStart(){}
	public void BlockAnimationEnd(){}
	public void BoardAnimationEnd(){}
	public void PassAnimationStart(){}
	public void ReadyShootEnd(){}
}

public class MovePlayerList : MonoBehaviour {
	public int currentIndex = 0;
	private GameObject OffsetGp;
	private GameObject[] childs;
	public StorePlayerBehaviour[] Players;
	private Vector3 screenPoint;
	private Vector3 offset;
	private float dis;
	private float x = 0;
	private float z = 0;
	private int availableCount = 0;
	
	public void InitChild(GameObject gp, int offsetIndex, int storeKind)
	{
		OffsetGp = gp;
		
		for (int i = 1; i < 6; i++) {
			if(GameData.DPlayers.ContainsKey(i))
			{

			}
		}
		
		offset = new Vector3(1.5f * offsetIndex, 0, 0);
		currentIndex = offsetIndex;
	}
	
	void OnDrag (Vector2 delta)
	{
		if (OffsetGp && OffsetGp.transform.childCount > 1)
		{
			delta.x *= 0.07f;
			
			x = OffsetGp.transform.localPosition.x - delta.x;
			
			if(delta.x > 0)
			{
				if(x < 0)
					offset = Vector3.zero;
				else
					offset = new Vector3(x, 0, 0);
			}
			else
			{
				if(x > 1.5f * (availableCount -1))
					offset = new Vector3(1.5f * (availableCount -1), 0, 0);
				else
					offset = new Vector3(x, 0, 0);
			}
		}
	}
	
	void OnDragEnd ()
	{
		float dis = 10f;
		
		if (OffsetGp && availableCount > 0) {
			for(int i = 0; i < availableCount; i++)
			{
				if(i < childs.Length)
				{
					if(Vector3.Distance(new Vector3(OffsetGp.transform.localPosition.x, 0, 0), new Vector3(Mathf.Abs(childs[i].transform.localPosition.x), 0, 0)) < dis)
					{
						dis = Vector3.Distance(new Vector3(OffsetGp.transform.localPosition.x, 0, 0), new Vector3(Mathf.Abs(childs[i].transform.localPosition.x), 0, 0));
						currentIndex = i;
					}
				}
			}
			
			offset = new Vector3 (currentIndex * 1.5f, 0, 0);
		}
	}
	
	void FixedUpdate()
	{
		//move z
		if (OffsetGp) {
			OffsetGp.transform.localPosition = Vector3.Slerp (OffsetGp.transform.localPosition, offset, 0.1f);
			
			if (childs.Length > 0 && availableCount <= childs.Length)
			for (int i = 0; i < availableCount; i++) {
				dis = Vector3.Distance (new Vector3 (OffsetGp.transform.localPosition.x, 0, 0), new Vector3 (Mathf.Abs(childs [i].transform.localPosition.x), 0, 0));
				if (dis < 1.5f) {
					z = dis / 1.5f;
					if (z > 0.1f)
						childs [i].transform.localPosition = Vector3.Slerp (childs [i].transform.localPosition, new Vector3 (i * -1.5f, 0, -z), 0.3f);
					else
						childs [i].transform.localPosition = Vector3.Slerp (childs [i].transform.localPosition, new Vector3 (i * -1.5f, 0, 0), 0.3f);
				} else
					childs [i].transform.localPosition = Vector3.Slerp (childs [i].transform.localPosition, new Vector3 (i * -1.5f, 0, -1), 0.3f);
			}
			
		}
	}
}

public class UIPlayerShow : UIBase {
	private int[] skillNo = new int[3];
	private UILabel labelPlayerName; 
	private UILabel labelPlayerPosition;
	private UILabel labelPlayerPower;
	private UILabel labelShoot;
	private UILabel labelShoot3;
	private UILabel labeldunk;
	private UILabel labelspeed;
	private UILabel labelcontorl;
	private UILabel labelsteal;
	private UILabel labelblock;
	private UILabel labeldodge;
	private UIButton[] skillBtnAy = new UIButton[3];
	private UISprite[] skillSpAy = new UISprite[3];
	private UISprite[] stars = new UISprite[5];
	public UISprite BG;
	private string[] skillExplain = new string[3];
	private string[] skillTitle = new string[3];
	private UILabel labelSkillExplain;
	private UILabel labelSkillTitle;
	private MovePlayerList moveList;
	private UIButton[] DraftBtn = new UIButton[2];
	private UIButton freeBtn;
	private UILabel labelDraftTitle;
	private UILabel labelDraftExplain;

	private UILabel labelbuy10Cost;
	private UILabel labelbuy1Cost;
	private UILabel labelfreeCd;
	
	private static UIPlayerShow instance = null;
	private const string UIName = "UIPlayerShow";
	private GameObject RedPoint;
	private UILabel[] translateLabel = new UILabel[2];

	public static bool Visible
	{
		get
		{
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static void UIShow(bool isShow){
		if(instance) {
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		}
		else
		if(isShow) {
			UI3D.UIShow(true);
			UI3D.Get.Open3DUI(UIKind.PlayerShow);
			Get.Show(isShow);
		}
	}
	
	public static UIPlayerShow Get
	{
		get {
			if (!instance) 
				instance = Load3DUI(UIName) as UIPlayerShow;
			
			return instance;
		}
	}

	protected override void InitText ()
	{
		base.InitText ();
		SetLabel("UIPlayerShow/Window/Top/UITitle/Label", TextConst.S(30401));
		SetLabel("UIPlayerShow/Window/Buy/Buy10Bt/TitleLebel", TextConst.S(30402));
		SetLabel("UIPlayerShow/Window/Buy/Buy10Bt/Explain", TextConst.S(30403));
		SetLabel("UIPlayerShow/Window/Buy/Buy1Bt/TitleLebel", TextConst.S(30404));
		SetLabel("UIPlayerShow/Window/Buy/FreeBt/TitleLabel", TextConst.S(30405));
		SetLabel("UIPlayerShow/Window/Buy/FreeBt/Label", TextConst.S(30406));

		if (translateLabel [0] && translateLabel [1]) {
			translateLabel [0].text = TextConst.S(30407);
			translateLabel[1].text = TextConst.S(30408);
		}

		SetLabel("UIPlayerShow/Window/PlayerData/LeftData/Star_From/Label", TextConst.S(30501));
		SetLabel("UIPlayerShow/Window/PlayerData/LeftData/PlayerCombat/Label", TextConst.S(30502));
		SetLabel("UIPlayerShow/Window/PlayerData/RightData/Skill/0/0/SkillATitle", TextConst.S(168));
		SetLabel("UIPlayerShow/Window/PlayerData/RightData/Skill/1/1/SkillBTitle", TextConst.S(332));
		SetLabel("UIPlayerShow/Window/PlayerData/RightData/Skill/2/2/SkillCTitle", TextConst.S(30503));
	}

	protected override void InitCom() {
		translateLabel[0] = GameObject.Find(UIName + "/Window/Buy/2/VipName").GetComponent<UILabel>();
		translateLabel[1] = GameObject.Find(UIName + "/Window/Buy/1/Back").GetComponent<UILabel>();
		translateLabel [0].text = TextConst.S(30407);
		translateLabel[1].text = TextConst.S(30408);
		RedPoint = GameObject.Find(UIName + "/Window/Buy/FreeBt/Redpoint");
		labelPlayerName = GameObject.Find (UIName + "/Window/PlayerData/LeftData/PlayerName/Name").GetComponent<UILabel> ();
		labelPlayerPosition = GameObject.Find (UIName + "/Window/PlayerData/LeftData/PlayerPostion/Postion").GetComponent<UILabel> ();
		labelPlayerPower = GameObject.Find (UIName + "/Window/PlayerData/LeftData/PlayerCombat/CombatValue").GetComponent<UILabel> ();
		GameObject.Find (UIName + "/Window/PlayerData/LeftData/PVPRecord/2Point").GetComponent<UILabel> ().text = TextConst.S (20021);
		labelShoot = GameObject.Find (UIName + "/Window/PlayerData/LeftData/PVPRecord/2PointValue").GetComponent<UILabel> ();
		GameObject.Find (UIName + "/Window/PlayerData/LeftData/PVPRecord/3Point").GetComponent<UILabel> ().text = TextConst.S (20022);
		labelShoot3 =GameObject.Find (UIName + "/Window/PlayerData/LeftData/PVPRecord/3PointValue").GetComponent<UILabel> ();
		GameObject.Find (UIName + "/Window/PlayerData/LeftData/PVPRecord/Dunk").GetComponent<UILabel> ().text = TextConst.S (20023);
		labeldunk = GameObject.Find (UIName + "/Window/PlayerData/LeftData/PVPRecord/DunkValue").GetComponent<UILabel> ();
		GameObject.Find (UIName + "/Window/PlayerData/LeftData/PVPRecord/Speed").GetComponent<UILabel> ().text = TextConst.S (20024);
		labelspeed = GameObject.Find (UIName + "/Window/PlayerData/LeftData/PVPRecord/SpeedValue").GetComponent<UILabel> ();
		GameObject.Find (UIName + "/Window/PlayerData/LeftData/PVPRecord/Control").GetComponent<UILabel> ().text = TextConst.S (20025);
		labelcontorl = GameObject.Find (UIName + "/Window/PlayerData/LeftData/PVPRecord/ControlValue").GetComponent<UILabel> ();
		GameObject.Find (UIName + "/Window/PlayerData/LeftData/PVPRecord/Steal").GetComponent<UILabel> ().text = TextConst.S (20026);
		labelsteal = GameObject.Find (UIName + "/Window/PlayerData/LeftData/PVPRecord/StealValue").GetComponent<UILabel> ();
		GameObject.Find (UIName + "/Window/PlayerData/LeftData/PVPRecord/Block").GetComponent<UILabel> ().text = TextConst.S (20027);
		labelblock = GameObject.Find (UIName + "/Window/PlayerData/LeftData/PVPRecord/BlockValue").GetComponent<UILabel> ();
		GameObject.Find (UIName + "/Window/PlayerData/LeftData/PVPRecord/Dodge").GetComponent<UILabel> ().text = TextConst.S (20029);
		labeldodge = GameObject.Find (UIName + "/Window/PlayerData/LeftData/PVPRecord/DodgeValue").GetComponent<UILabel> ();

		BG = GameObject.Find (UIName + "/Window/UITiledSprite").GetComponent<UISprite>();

		moveList = GameObject.Find (UIName + "/Window/PlayerList").GetComponent<MovePlayerList>();

		for (int i = 0; i < skillBtnAy.Length; i++) {
			skillBtnAy[i] = GameObject.Find (string.Format(UIName + "/Window/PlayerData/RightData/Skill/{0}/{0}", i)).GetComponent<UIButton>();
			skillSpAy[i] = GameObject.Find (string.Format(UIName + "/Window/PlayerData/RightData/Skill/{0}/{0}/ItemPic", i)).GetComponent<UISprite>();
			SetBtnFun(ref skillBtnAy[i] , ShowSkillExplain);
		}

		SetBtnFun(UIName + "/Window/Close/Close", OnClose);

		for (int i = 0; i < stars.Length; i++) 
			stars[i] = GameObject.Find(string.Format(UIName + "/Window/PlayerData/LeftData/Star_From/Star/{0}", i)).GetComponent<UISprite>();

		labelSkillExplain = GameObject.Find (UIName + "/Window/PlayerData/RightData/Skill/Explain/SkillExplain").GetComponent<UILabel>();
		labelSkillTitle = GameObject.Find (UIName + "/Window/PlayerData/RightData/Skill/Explain/SkillName").GetComponent<UILabel>();

		labelDraftTitle = GameObject.Find(UIName +"/Window/Top/UITitle/ItemName").GetComponent<UILabel>();
		labelDraftExplain = GameObject.Find(UIName +"/Window/Top/UITitle/BulletinExplain").GetComponent<UILabel>();
	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		if (isShow) 
		{
			UI3D.Get.ShowCamera (isShow);

		}
	}

	public void DoDraft()
	{

	}

	public void SetPlayerData(int greatePlayerId)
	{
		if (GameData.DPlayers.ContainsKey (greatePlayerId)){
			labelPlayerName.text = GameData.DPlayers[greatePlayerId].Name; 
			labelShoot.text = GameData.DPlayers[greatePlayerId].Point2.ToString();
			labelShoot3.text = GameData.DPlayers[greatePlayerId].Point3.ToString();
			labeldunk.text = GameData.DPlayers[greatePlayerId].Dunk.ToString();
			labelspeed.text = GameData.DPlayers[greatePlayerId].Speed.ToString();
			labelcontorl.text = GameData.DPlayers[greatePlayerId].Dribble.ToString();
			labelsteal.text = GameData.DPlayers[greatePlayerId].Steal.ToString();
			labelblock.text = GameData.DPlayers[greatePlayerId].Block.ToString();
			labeldodge.text = "";
		}
	}

	public void ResetCom(int id, int index)
	{

	}

	public void UpdateInfo(int kind)
	{

	}

	public void ShowSkillExplain()
	{

	}

	public void ClearSkillExplain()
	{
		labelSkillExplain.text = "";
		labelSkillTitle.text = "";
	}

	public void OnClose(){
		UI3D.Get.ShowCamera (false);
		UIShow (false);
	}
}
