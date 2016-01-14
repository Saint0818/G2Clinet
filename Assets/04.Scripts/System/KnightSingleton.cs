using UnityEngine;

public class KnightSingleton<T> : MonoBehaviour where T : KnightSingleton<T>
{
	private static bool IsQuit = false;
    private static T mInst = null;
	
	protected virtual void Init()
	{
	}
	
	protected virtual void OnDestroyInst()
	{

	}
	
	public virtual string ResName
	{
		get
		{
			return string.Empty;
		}
	}
	
	public static T Get
	{
		get
		{
			if(IsQuit)
				return null;
			
			if (mInst != null)
				return mInst;
			
			mInst = FindObjectOfType(typeof(T)) as T;// check obj is aready instance
			if (mInst != null)
				return mInst;
			
			//if can't found, create GameObject
			GameObject obj = new GameObject("TempName");
			mInst = obj.AddComponent<T>();
			obj.name = typeof(T).ToString();
            //Debug.Log("Singletion " + obj.name);
			mInst.Init();
			return mInst;
		}
	}

	public static bool Visible
	{
		get
		{
			return (mInst != null && mInst.gameObject.activeInHierarchy);
		}
        set {
            if (mInst)
                mInst.gameObject.SetActive(value);
        }
	}

	public static bool IsExist
	{
		get
		{
			return (mInst != null);
		}
	}
	
	public static void DestroyInst()
	{
		if(mInst == null)
			return;

	    mInst.OnDestroyInst();

        Destroy(mInst.gameObject);

	    mInst = null;
	}
	/*
	void OnApplicationQuit()
	{
		IsQuit = true;
		
		if(this.gameObject)
			Destroy(this.gameObject);
	}*/
}