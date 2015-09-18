using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UICreateRoleStyleViewGroup : MonoBehaviour
{
    public delegate void Action(UICreateRoleStyleView.EEquip equip, TItemData item);
    public delegate void Action2(UICreateRoleStyleView.EEquip equip);
    public event Action OnEquipClickListener;
    public event Action2 OnTitleClickListener;

    public UISprite Slider;
    public UILabel TitleLabel;
    public GameObject ButtonGroup;
    public UICreateRoleStyleViewGroupButton[] Buttons;

    private const float ShowTime = 0.15f; // 單位: 秒.
    private float mElapsedTime;
    private bool mIsPlaying;

    private TItemData[] mItems;
    private UICreateRoleStyleView.EEquip mEquip;

    [UsedImplicitly]
    private void Awake()
    {
        Hide();
    }

    [UsedImplicitly]
    private void Update()
    {
        if(!mIsPlaying)
            return;

        if(mElapsedTime >= ShowTime)
        {
            mIsPlaying = false;
            Slider.fillAmount = 1;
            return;
        }

        mElapsedTime += Time.deltaTime;
        Slider.fillAmount = mElapsedTime / ShowTime;

//        Debug.LogFormat("ElapsedTime:{0}, FillAmount:{1}", mElapsedTime, Slider.fillAmount);
    }

    public void Init(UICreateRoleStyleView.EEquip equip, TItemData[] items)
    {
        if(items == null || items.Length == 0)
        {
            Debug.LogWarning("Items is empty");
            return;
        }

        mEquip = equip;
        mItems = items;

        TitleLabel.text = mItems[0].Name;

        for(int i = 0; i < Buttons.Length; i++)
        {
            if(i >= items.Length)
            {
                Buttons[i].Hide();
                continue;
            }

            Buttons[i].Show(mItems[i].Name);
        }

        resetButtonSelected();
    }

    private void resetButtonSelected()
    {
        Buttons[0].SetSelected();

        for(int i = 1; i < Buttons.Length; i++)
        {
            Buttons[i].SetUnSelected();
        }
    }

    public void Play()
    {
        Slider.gameObject.SetActive(true);
        Slider.fillAmount = 0;

        ButtonGroup.SetActive(true);

        mElapsedTime = 0;
        mIsPlaying = true;
    }

    public void Hide()
    {
        Slider.gameObject.SetActive(false);
        ButtonGroup.SetActive(false);

        mIsPlaying = false;
    }

    public void SetSelected()
    {
        GetComponent<UIToggle>().Set(true);
    }

    private void onEquipClick(int index)
    {
//        Debug.LogFormat("onEquipClick, item:{0}", mItems[index]);

        TitleLabel.text = mItems[index].Name;

        if(OnEquipClickListener != null)
            OnEquipClickListener(mEquip, mItems[index]);
    }

    public void OnEquip1Click()
    {
        if(UIToggle.current.value)
            onEquipClick(0);
    }

    public void OnEquip2Click()
    {
        if (UIToggle.current.value)
            onEquipClick(1);
    }

    public void OnEquip3Click()
    {
        if (UIToggle.current.value)
            onEquipClick(2);
    }

    public void OnEquip4Click()
    {
        if (UIToggle.current.value)
            onEquipClick(3);
    }

    public void OnEquip5Click()
    {
        if (UIToggle.current.value)
            onEquipClick(4);
    }

    public void OnEquip6Click()
    {
        if (UIToggle.current.value)
            onEquipClick(5);
    }

    public void OnTitleClick()
    {
        if(UIToggle.current.value)
        {
//            Debug.LogFormat("OnTitleClick, Equip:{0}", mEquip);

            if (OnTitleClickListener != null)
                OnTitleClickListener(mEquip);
        }
    }
}