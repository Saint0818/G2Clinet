using System;
using GameStruct;
using UnityEngine;

public class TMallBox 
{
	public TPickCost mPickCost;
	public int mIndex;

	public GameObject mMallBox;
	public UILabel FreeLabelTitle;

	//OpenBtn
	public UISprite PriceIcon;
	public UILabel PriceLabel;
	public UILabel FreeLabel1;

	//MainBtn
	public UILabel TitleLabel;
	public UILabel ExplainLabel;

	//Tween
	public Transform Tween;
	public UILabel SubheadLabelDisk;
	public UILabel SubheadLabelItem;
	public UILabel Open5Label;
	public UILabel Open10Label;
	public GameObject DiskScrollView;
	public GameObject ItemScrollView;
	public UISprite Open5Icon;
	public UILabel Open5Price;
	public UISprite Open10Icon;
	public UILabel Open10Price;
	public UILabel SaleLabel;

	private UIScrollView cardScrollView;
	private UIScrollView itemScrollView;

	public GameObject BtnOne;
	public GameObject BtnFive;
	public GameObject BtnTen;

	public GameObject BtnOneRedPoint;

	private int money1;
	private int money2;
	private int money3;

	public void Init(GameObject obj, EventDelegate oneBtn, EventDelegate fiveBtn, EventDelegate tenBtn) {
		mMallBox = obj;
		FreeLabelTitle = obj.transform.Find("FreeLabel").GetComponent<UILabel>();
		PriceIcon = obj.transform.Find("OpenBtn1/Icon").GetComponent<UISprite>();
		PriceLabel = obj.transform.Find("OpenBtn1/PriceLabel").GetComponent<UILabel>();
		FreeLabel1 = obj.transform.Find("OpenBtn1/FreeLabel1").GetComponent<UILabel>();

		TitleLabel = obj.transform.Find("MainBtn/MallBoxTextureShow/TitleLabel").GetComponent<UILabel>();
		ExplainLabel = obj.transform.Find("MainBtn/ExplainPanel/ExplainLabel").GetComponent<UILabel>();

		Tween = obj.transform.Find("Tween");
		SubheadLabelDisk = obj.transform.Find("Tween/SubheadLabelDisk").GetComponent<UILabel>();
		SubheadLabelItem = obj.transform.Find("Tween/SubheadLabelItem").GetComponent<UILabel>();
		Open10Label = obj.transform.Find("Tween/Open10Label").GetComponent<UILabel>();
		Open5Label = obj.transform.Find("Tween/Open5Label").GetComponent<UILabel>();
		DiskScrollView = obj.transform.Find("Tween/VerticalDisk/ScrollView1/UIWrapContent").gameObject;
		ItemScrollView = obj.transform.Find("Tween/VerticalItem/ScrollView2/UIWrapContent").gameObject;
		Open5Icon = obj.transform.Find("Tween/OpenBtn5/Icon").GetComponent<UISprite>();
		Open5Price = obj.transform.Find("Tween/OpenBtn5/PriceLabel").GetComponent<UILabel>();
		Open10Icon = obj.transform.Find("Tween/OpenBtn10/Icon").GetComponent<UISprite>();
		Open10Price = obj.transform.Find("Tween/OpenBtn10/PriceLabel").GetComponent<UILabel>();
		SaleLabel = obj.transform.Find("Tween/OpenBtn10/SaleLabel").GetComponent<UILabel>();

		BtnOne = obj.transform.Find("OpenBtn1").gameObject;
		BtnOneRedPoint = obj.transform.Find("OpenBtn1/RedPoint").gameObject;
		BtnFive = obj.transform.Find("Tween/OpenBtn5").gameObject;
		BtnTen = obj.transform.Find("Tween/OpenBtn10").gameObject;

		cardScrollView = obj.transform.Find("Tween/VerticalDisk/ScrollView1").GetComponent<UIScrollView>();
		itemScrollView = obj.transform.Find("Tween/VerticalItem/ScrollView2").GetComponent<UIScrollView>();

		if(FreeLabelTitle != null && PriceIcon != null && PriceLabel != null && FreeLabel1 != null && 
			TitleLabel != null && ExplainLabel != null && Tween != null && SubheadLabelDisk != null && SubheadLabelItem != null && 
			Open10Label != null && Open5Label != null && DiskScrollView != null && ItemScrollView != null && Open5Icon != null &&
			Open5Price != null && Open10Icon != null && Open10Price != null && SaleLabel != null && 
			BtnOne != null && BtnFive != null && BtnTen != null){}
		else 
			Debug.LogError("TMallBox Init Fail");

		BtnOne.GetComponent<UIButton>().onClick.Add(oneBtn);
		BtnFive.GetComponent<UIButton>().onClick.Add(fiveBtn);
		BtnTen.GetComponent<UIButton>().onClick.Add(tenBtn);

		SubheadLabelDisk.text = TextConst.S(4101);
		SubheadLabelItem.text = TextConst.S(4102);
//		Open10Label.text = TextConst.S(4103);
		Open5Label.text = TextConst.S(4104);
		SaleLabel.text = TextConst.S(4105);
		FreeLabelTitle.text = TextConst.S(4106);
		FreeLabel1.text = TextConst.S(4107);

		Tween.gameObject.SetActive(false);
		Tween.transform.localScale = new Vector3(0.01f, 1, 1);
	}

	public void Refresh () {
		setHaveFree ();
	}

	public void SetIndex (int index) {
		mIndex = index;
	}

	public void UpdateView(int posIndex, TPickCost pickcost) {
		mPickCost = pickcost;
		BtnOne.name = pickcost.Order.ToString();
		BtnFive.name = pickcost.Order.ToString();
		BtnTen.name = pickcost.Order.ToString();
		mMallBox.transform.localPosition = new Vector3(410 * posIndex, 0, 0);
		setHaveFree();
		changeSpendKind(pickcost.SpendKind);
		TitleLabel.text = pickcost.Name;
		ExplainLabel.text = pickcost.Explain;
		PriceLabel.text = NumFormater.Convert(pickcost.OnePick);
		Open5Price.text = NumFormater.Convert(pickcost.FivePick);
		Open10Price.text = NumFormater.Convert(pickcost.TenPick);
		money1 = pickcost.OnePick;
		money2 = pickcost.FivePick;
		money3 = pickcost.TenPick;

		cardScrollView.MoveRelative(new Vector3(-55, 0, 0));
		itemScrollView.MoveRelative(new Vector3(-55, 0, 0));

		RefreshText();
	}

	public void RefreshText () {
		PriceLabel.color = GameData.CoinEnoughTextColor(GameData.Team.CoinEnough(mPickCost.SpendKind, money1),mPickCost.SpendKind);
		Open5Price.color = GameData.CoinEnoughTextColor(GameData.Team.CoinEnough(mPickCost.SpendKind, money2),mPickCost.SpendKind);
		Open10Price.color = GameData.CoinEnoughTextColor(GameData.Team.CoinEnough(mPickCost.SpendKind, money3),mPickCost.SpendKind);
	}

	public void UpdateFreeTimeCD () {
		if(IsHaveFree) {
			FreeLabelTitle.gameObject.SetActive(!IsPickFree);
			if(!IsPickFree)  {
				FreeLabelTitle.text =  string.Format(TextConst.S(4106), TextConst.SecondString((int)(new System.TimeSpan(GameData.Team.LotteryFreeTime[mIndex].ToUniversalTime().Ticks - DateTime.UtcNow.Ticks).TotalSeconds)));  
			}
			setHaveFree();
		}
	}

	public void UpdataCards (int index, GameObject obj) {
		obj.transform.parent = DiskScrollView.transform;
		obj.transform.localPosition = new Vector3((-175 + 200 * index), 0, 0);
	}

	public void UpdataItems (int index, GameObject obj) {
		obj.transform.parent = ItemScrollView.transform;
		obj.transform.localPosition = new Vector3(90 * index, 0, 0);
	}

	private void setHaveFree () {
		if(IsHaveFree) {
			if(IsPickFree) {
				FreeLabelTitle.gameObject.SetActive(false);
				FreeLabel1.gameObject.SetActive(true);
				PriceIcon.gameObject.SetActive(false);
				PriceLabel.gameObject.SetActive(false);
				BtnOneRedPoint.SetActive(true);
			} else {
				FreeLabelTitle.gameObject.SetActive(true);
				FreeLabel1.gameObject.SetActive(false);
				PriceIcon.gameObject.SetActive(true);
				PriceLabel.gameObject.SetActive(true);
				BtnOneRedPoint.SetActive(false);
			}
		} else {
			FreeLabelTitle.gameObject.SetActive(false);
			FreeLabel1.gameObject.SetActive(false);
			PriceIcon.gameObject.SetActive(true);
			PriceLabel.gameObject.SetActive(true);
			BtnOneRedPoint.SetActive(false);
		}
	}

	private void changeSpendKind (int kind) {
		PriceIcon.spriteName = GameFunction.SpendKindTexture(kind);
		Open5Icon.spriteName = GameFunction.SpendKindTexture(kind);
		Open10Icon.spriteName = GameFunction.SpendKindTexture(kind);
	}

	public bool IsHaveFree {
		get{return (mPickCost.FreeTime > 0);}
	}

	public bool IsPickFree {
		get{
			if(mPickCost.FreeTime == 0)
				return false;

			if(mPickCost.FreeTime > 0 &&  DateTime.UtcNow > GameData.Team.LotteryFreeTime[mIndex].ToUniversalTime())
				return true;
			else
				return false;
		}
	}
}
