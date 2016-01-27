using UnityEngine;

public class UIInstanceChapterFocus : MonoBehaviour
{
    private GameObject mIcon;
    private Transform mScrollView;
    private Vector3 mMin;
    private Vector3 mMax;

    private readonly Vector3 mRange = new Vector3(300, 50, 10);

    public void Init(GameObject icon, Transform scrollView, Vector3 center)
    {
        mIcon = icon;
        mScrollView = scrollView;

        mMin = center - mRange;
        mMax = center + mRange;
    }

	private void Update()
	{
	    Vector3 pos = mScrollView.transform.localPosition;

        mIcon.SetActive(mMin.x <= pos.x && mMin.y <= pos.y && mMin.z <= pos.z &&
                        pos.x <= mMax.x && pos.y <= mMax.y && pos.z <= mMax.z);
	}
}
