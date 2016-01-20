using UnityEngine;

public class UIInstanceChapter : MonoBehaviour
{
    public UILabel Title;
    public UILabel Desc;
    public GameObject[] FocusObjects;
    public Transform ModelParent;

    public class Data
    {
        public string Title;
        public string Desc;

        public GameObject Model;
    }

    public bool FocusVisible
    {
        set
        {
            foreach(GameObject obj in FocusObjects)
            {
                obj.SetActive(value);
            }
        }
    }

    public void SetData(Data data)
    {
        Title.text = data.Title;
        Desc.text = data.Desc;

        data.Model.transform.parent = ModelParent;
        data.Model.transform.localPosition = Vector3.zero;
        data.Model.transform.localRotation = Quaternion.identity;
        data.Model.transform.localScale = Vector3.one;
    }
}