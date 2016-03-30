using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PVPLvRangeItem : MonoBehaviour
{
    public UISprite Line;
    public GameObject Graduation;
    private int interval = 0;
    private int total = 880;
    private GameObject[] graduations;
    private float centerIndex = 0;

    public GameObject ThumbOffset;
    public UILabel ThumbOffsetLabel;
    public UISprite Trim;

    public GameObject NowRankOffset;
    public UIButton NowRankBtn;

    public void InitData(int count, EventDelegate nowRankFunc)
    {
        if (Line && Graduation && NowRankOffset && NowRankBtn && Trim)
        {
            NowRankBtn.onClick.Add(nowRankFunc);
            graduations = new GameObject[count];
            Line.width = total;
            interval = total / (count - 1);

            UISprite ThumbOffsetSp = ThumbOffset.GetComponent<UISprite>();

            if (ThumbOffsetSp)
                ThumbOffsetSp.width = interval;

            Trim.width = interval;
            Vector3 pos = Trim.transform.localPosition;
            Trim.transform.localPosition = new Vector3(Trim.width / 2, pos.y, pos.z);

            pos = ThumbOffsetLabel.transform.localPosition;
            ThumbOffsetLabel.transform.localPosition = new Vector3(Trim.width / 2, pos.y, pos.z);
            
            for (int i = 0; i < graduations.Length; i++)
            {
                
                if (i != 0 && i != graduations.Length - 1)
                {
                    if (i == 1)
                    {
                        graduations[i] = Graduation; 
                    }
                    else
                    {
                        graduations[i] = Instantiate(Graduation) as GameObject;  
                    } 
                }
                else
                {
                    //開頭跟結尾不用產生Graduation,塞空object取座標時會用到
                    graduations[i] = new GameObject();
                }

                if (graduations[i])
                {
                    graduations[i].transform.parent = gameObject.transform;
                    graduations[i].name = i.ToString();
                }
            }
            UpdatePosition();
        }
    }

    void UpdatePosition()
    {
        if (graduations != null)
        {
            //奇數偶數迷思，只要把Center變成float就可以用一公式解決算法問題
            float x = 0;
            centerIndex = (float)(graduations.Length - 1) / 2f;
           
            for (int i = 0; i < graduations.Length; i++)
            {
                x = interval * (i - centerIndex);
                graduations[i].transform.localPosition = new Vector3(x, 0, 0);
            }
        }
    }


    private float tweenSpeed = 0.5f;

    public void SetOffset(int lv)
    {
        ThumbOffset.transform.DOLocalMoveX((lv - 1) * interval, tweenSpeed);
        if (GameData.DPVPData.ContainsKey(lv))
        {
            ThumbOffsetLabel.text = string.Format(TextConst.S(9741), GameData.DPVPData[lv].LowScore, GameData.DPVPData[lv].HighScore);
        }
    }

    public void SetNowRankOffset(int lv)
    {
        NowRankOffset.transform.localPosition = new Vector3((lv - 1) * interval + (interval / 2), 0, 0);
    }
}
