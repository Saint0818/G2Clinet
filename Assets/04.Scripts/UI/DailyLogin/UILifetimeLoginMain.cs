using UnityEngine;

public class UILifetimeLoginMain : MonoBehaviour
{
    public UILabel LifetimeLoginNumLabel;

    public int LifetimeLoginNum
    {
        set { LifetimeLoginNumLabel.text = string.Format(TextConst.S(3806), value); }
    }
}