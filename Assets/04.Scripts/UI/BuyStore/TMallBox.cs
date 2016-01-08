using GameStruct;
using UnityEngine;

public class TMallBox 
{
	public TPickCost mPickCost;

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

	public GameObject BtnOne;
	public GameObject BtnFive;
	public GameObject BtnTen;

	private bool isHaveFree = false;

	public void Init(GameObject obj, EventDelegate oneBtn, EventDelegate fiveBtn, EventDelegate tenBtn) {
		mMallBox = obj;
		FreeLabelTitle = obj.transform.FindChild("FreeLabel").GetComponent<UILabel>();
		PriceIcon = obj.transform.FindChild("OpenBtn1/Icon").GetComponent<UISprite>();
		PriceLabel = obj.transform.FindChild("OpenBtn1/PriceLabel").GetComponent<UILabel>();
		FreeLabel1 = obj.transform.FindChild("OpenBtn1/FreeLabel1").GetComponent<UILabel>();

		TitleLabel = obj.transform.FindChild("MainBtn/TitleLabel").GetComponent<UILabel>();
		ExplainLabel = obj.transform.FindChild("MainBtn/ExplainLabel").GetComponent<UILabel>();

		Tween = obj.transform.FindChild("Tween");
		SubheadLabelDisk = obj.transform.FindChild("Tween/SubheadLabelDisk").GetComponent<UILabel>();
		SubheadLabelItem = obj.transform.FindChild("Tween/SubheadLabelItem").GetComponent<UILabel>();
		Open10Label = obj.transform.FindChild("Tween/Open10Label").GetComponent<UILabel>();
		Open5Label = obj.transform.FindChild("Tween/Open5Label").GetComponent<UILabel>();
		DiskScrollView = obj.transform.FindChild("Tween/VerticalDisk/ScrollView1/UIWrapContent").gameObject;
		ItemScrollView = obj.transform.FindChild("Tween/VerticalItem/ScrollView2/UIWrapContent").gameObject;
		Open5Icon = obj.transform.FindChild("Tween/OpenBtn5/Icon").GetComponent<UISprite>();
		Open5Price = obj.transform.FindChild("Tween/OpenBtn5/PriceLabel").GetComponent<UILabel>();
		Open10Icon = obj.transform.FindChild("Tween/OpenBtn10/Icon").GetComponent<UISprite>();
		Open10Price = obj.transform.FindChild("Tween/OpenBtn10/PriceLabel").GetComponent<UILabel>();
		SaleLabel = obj.transform.FindChild("Tween/OpenBtn10/SaleLabel").GetComponent<UILabel>();

		BtnOne = obj.transform.FindChild("OpenBtn1").gameObject;
		BtnFive = obj.transform.FindChild("Tween/OpenBtn5").gameObject;
		BtnTen = obj.transform.FindChild("Tween/OpenBtn10").gameObject;

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
		Open10Label.text = TextConst.S(4103);
		Open5Label.text = TextConst.S(4104);
		SaleLabel.text = TextConst.S(4105);
		FreeLabelTitle.text = TextConst.S(4106);
		FreeLabel1.text = TextConst.S(4107);

		Tween.gameObject.SetActive(false);
		Tween.transform.localScale = new Vector3(0.01f, 1, 1);
	}

	public void UpdateView(int index, TPickCost pickcost) {
		mPickCost = pickcost;
		BtnOne.name = index.ToString();
		BtnFive.name = index.ToString();
		BtnTen.name = index.ToString();
		mMallBox.transform.localPosition = new Vector3(420 * index, 0, 0);
		setHaveFree((pickcost.FreeTime != 0));
		changeSpendKind(pickcost.SpendKind);
		TitleLabel.text = pickcost.Name;
		ExplainLabel.text = pickcost.Explain;
		PriceLabel.text = pickcost.OnePick.ToString();
		Open5Price.text = pickcost.FivePick.ToString();
		Open10Price.text = pickcost.TenPick.ToString();
	}

	public void UpdateFreeTimeCD () {
		
	}

	public void UpdataCards (int index, GameObject obj) {
		obj.transform.parent = DiskScrollView.transform;
		obj.transform.localPosition = new Vector3(200 * index, 0, 0);
	}

	public void UpdataItems (int index, GameObject obj) {
		obj.transform.parent = ItemScrollView.transform;
		obj.transform.localPosition = new Vector3(200 * index, 0, 0);
	}

	private void setHaveFree (bool isHave) {
		isHaveFree = isHave;
		FreeLabelTitle.gameObject.SetActive(isHave);
		FreeLabel1.gameObject.SetActive(isHave);
		PriceIcon.gameObject.SetActive(!isHave);
		PriceLabel.gameObject.SetActive(!isHave);
	}

	private void changeSpendKind (int kind) {
		if(kind == 1) {
			PriceIcon.spriteName = "MallGem1";
			Open5Icon.spriteName = "MallGem1";
			Open10Icon.spriteName = "MallGem1";
		} else if(kind == 2) {
			PriceIcon.spriteName = "MallCoin1";
			Open5Icon.spriteName = "MallCoin1";
			Open10Icon.spriteName = "MallCoin1";
		} else if(kind == 3) {
			
		}
	}

	public bool IsHaveFree {
		get{return isHaveFree;}
	}
}
