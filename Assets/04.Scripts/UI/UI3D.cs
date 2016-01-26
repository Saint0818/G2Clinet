using UnityEngine;

public class UI3D : MonoBehaviour {
	private static UI3D instance = null;
    private const string UIName = "UI3D";

    public static UI3D Get
    {
        get {
            if (!instance) {
                GameObject obj = GameObject.Find(UIName);
                if (!obj) {
                    GameObject obj2 = Resources.Load<GameObject>("Prefab/UI/" + UIName);
                    if (obj2) {
                        GameObject obj3 = Instantiate(obj2) as GameObject;
                        obj3.name = UIName;
                        instance = obj3.GetComponent<UI3D>();
                        if(!instance) 
                            instance = obj3.AddComponent<UI3D>();
                    } else {
                        obj2 = new GameObject();
                        obj2.name = UIName;
                        instance = obj2.AddComponent<UI3D>();
                    }
                } else
                    instance = obj.GetComponent<UI3D>();
            }

            return instance;
        }
    }

    private void Show(bool isShow) {
        gameObject.SetActive(isShow);
    }

	public static bool Visible
	{
		get {
			if (instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}

        set {
            if (instance) 
                instance.Show(value);
            else
            if (value)
                Get.Show(value);
        }
	}
}
