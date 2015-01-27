using UnityEngine;
using System.Collections;

public class Shake : MonoBehaviour
{
	private float crtTime = 0;
	private float shakeTime = 0.2f;
	private float prelShakeTime = 0.02f;
	private int mShakeCount = 0;
	private int mCrtCount = 0;

  private float mShake = 1f;
	private float mCrtShake = 0;
  private float setShake;
  private bool shakeSwitch = false;
  private Vector3 mStartPos;
  public bool mIsOpenTestUI = false;

  
    void Start()
    {
        setShake = mShake;
        mShakeCount = (int)(shakeTime / prelShakeTime); 
        mCrtCount = 0;
        mCrtShake = mShake;
    }

    public void Play()
    {
		mStartPos = gameObject.transform.localPosition;
        mShake = setShake;
        shakeSwitch = true;
        crtTime = Time.time;
    }

    void Stop()
    {
        shakeSwitch = false;
        mCrtCount = 0;
        gameObject.transform.localPosition = mStartPos; 
        mCrtShake = mShake;
    }

	void FixedUpdate()
    { 
        if (shakeSwitch)
        {
            if (Time.time - crtTime > prelShakeTime)
            {
                Vector3 pos = new Vector3(mStartPos.x + (Random.Range(0, mCrtShake * 1) - mCrtShake), mStartPos.y + (Random.Range(0, mCrtShake * 1) - mCrtShake), mStartPos.z);
                transform.localPosition = pos;
                mCrtShake = mCrtShake / 1.05f;
                mCrtCount++;
                if (mCrtCount >= mShakeCount)
                    Stop();
            }
        }
    }

#if UNITY_EDITOR
  void OnGUI()
  {
    if(mIsOpenTestUI == false)
      return;

    if (GUILayout.Button ("Shake")) 
    {
      mShake=setShake;
      shakeSwitch=true;
    }
  }
#endif
}
