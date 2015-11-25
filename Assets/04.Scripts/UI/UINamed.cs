using UnityEngine;
using Newtonsoft.Json;
using GameEnum;
using GameStruct;

public struct TRenameResult {
	public string Name;
	public int Diamond;
}

public class NameView
{
	private GameObject self;
	private UIInput nameInput;
	private UILabel nameLabel;
	private UIButton randomBtn;
	private UILabel tip;
	private UILabel price;
	private bool isInit = false;

	public void Init(GameObject go, EventDelegate randomFunction = null)
	{
		if (go) {
			self =  go;
			nameInput = self.transform.FindChild("PlayerName").gameObject.GetComponent<UIInput>();
			nameLabel = nameInput.transform.FindChild("NameLabel").gameObject.GetComponent<UILabel>();
			randomBtn = self.transform.FindChild("RandomBtn").gameObject.GetComponent<UIButton>();
			tip = self.transform.FindChild("WarningLabel").gameObject.GetComponent<UILabel>();
			price = self.transform.FindChild("SpendGemIcon/PriceLabel").gameObject.GetComponent<UILabel>();

			isInit = self && nameInput && nameLabel && randomBtn && tip && price;

			if(isInit)
			{
				tip.text = TextConst.S(9000002);
				price.text = GameConst.RenamePrice.ToString();
				if(randomFunction != null)
					randomBtn.onClick.Add(randomFunction);
			}
		}
	}

	public void UpdateView()
	{
		if (GameData.Team.Player.Name != string.Empty)
			nameLabel.text = GameData.Team.Player.Name;
		else
			UINamed.Get.OnRandomName ();
	}

	public string Name
	{
		set{nameInput.value = value;}
		get{return nameLabel.text;}
	}

	public bool IsChange
	{
		get{return GameData.Team.Player.Name != Name;}
	}

	public bool IsLegal
	{
		get{ 
			if(Name.Length >= 2 && Name.Length <= 20)
			{
				return true;
			}
			else
			{
				UIHint.Get.ShowHint(TextConst.S(9000005), Color.red);
				return false;
			}
		}
	}
}

public class UINamed : UIBase {
	private static UINamed instance = null;
	private const string UIName = "UINamed";
	private NameView nameView = new NameView();
	private UIButton yesBtn;
	private UIButton noBtn;

	public static int OpenKind = 1;

	public static void UIShow(bool isShow){
		if(instance) {
            if (!isShow)
                RemoveUI(UIName);
            else
			    instance.Show(isShow);
        }
		else
		if(isShow)
			Get.Show(isShow);
	}

	public static UINamed Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UINamed;
			
			return instance;
		}
	}

	protected override void InitCom() {
		GameObject obj = GameObject.Find (UIName + "/Center/NamedView");
		nameView.Init (obj, new EventDelegate(OnRandomName));
		SetBtnFun(UIName + "/Center/CheckBtn", OnCheckBtn);
		SetBtnFun(UIName + "/Center/NoBtn", OnCancelChange);
	}

	protected override void InitText(){
		SetLabel (UIName + "/Center/Title/LabelTitle", TextConst.S(9000000));
	}

	protected override void OnShow(bool isShow) {
		if(isShow){
			nameView.UpdateView ();
		}
	}

	public void OnRandomName()
	{
		if (TextConst.TeamNameAy != null && TextConst.TeamNameAy.Length > 0) {
			int index1 = UnityEngine.Random.Range (0, TextConst.TeamNameAy.Length - 1);
			int index2 = UnityEngine.Random.Range (0, TextConst.TeamNameAy.Length - 1);
			int index3 = UnityEngine.Random.Range (0, TextConst.TeamNameAy.Length - 1);
			nameView.Name = TextConst.TeamNameAy [index1].TeamName1 + TextConst.TeamNameAy [index2].TeamName2 + TextConst.TeamNameAy [index3].TeamName3;
		}
	}

	private void OnCheckBtn()
	{
		if (nameView.IsChange && nameView.IsLegal)
			changePlayerName ();
		else
		{
			UIShow(false);
		}
	}

	private void OnCancelChange()
	{
		if (nameView.IsChange) {
			nameView.UpdateView();
		}

		UIShow(false);
	}

	private void changePlayerName()
	{
		WWWForm form = new WWWForm();
		form.AddField("NewPlayerName", nameView.Name);
		SendHttp.Get.Command(URLConst.ChangePlayerName, waitChangePlayerName, form, true);
	}

	private void waitChangePlayerName(bool ok, WWW www)
	{
		if (ok)
		{
			TRenameResult result = JsonConvert.DeserializeObject<TRenameResult>(www.text);
			GameData.Team.Player.Name = result.Name;
			GameData.Team.Diamond = result.Diamond;
			UIMainLobby.Get.UpdateUI();
//			UIHint.Get.ShowHint("Change Name Success!", Color.black);
		}
		else
			UIHint.Get.ShowHint("Change Player Name fail!", Color.red);

		if (UIPlayerInfo.Visible)
			UIPlayerInfo.Get.UpdatePage (0);

		UIShow (false);
	}
}
