using DG.Tweening;
using GameEnum;
using UnityEngine;

public class ShootCurveCounter
{
    private Transform mController;
    private float mSamplingTime = 0;
    private bool mIsPlaying = false;
    private float timeScale = 1f;
    private TShootCurve mCurve;
//    private Vector3 mLookAt;

    public float Timer
    {
        set{ timeScale = value; }
    }

//    private bool mIsLookAt;

    public void Apply(int index, Transform controlPlayer, Vector3 lookAt)
    {
//		mLookAt = lookAt;
        mController = controlPlayer;
	    string curveName = string.Format("{0}{1}", EAnimatorState.Shoot, index);

//	    if(mCurve == null || (mCurve != null && mCurve.Name != curveName))
//        {
//            for (int i = 0; i < ModelManager.Get.AnimatorCurve.Shoot.Length; i++)
//                if (ModelManager.Get.AnimatorCurve.Shoot[i].Name == curveName)
//                    mCurve = ModelManager.Get.AnimatorCurve.Shoot[i];
//        }

        mCurve = ModelManager.Get.AnimatorCurve.FindShootCurve(curveName);
        if(mCurve != null)
            mController.DOLookAt(lookAt, 0.5f, AxisConstraint.Y);

        mSamplingTime = 0;
        mIsPlaying = mCurve != null;
//        mIsLookAt = false;

//        if(!string.IsNullOrEmpty(curveName) && mCurve == null && LobbyStart.Get.IsDebugAnimation)
        if(mCurve == null && LobbyStart.Get.IsDebugAnimation)
            Debug.LogErrorFormat("Can't Found Curve: {0}", curveName);
    }

    private void calculation()
    {	
//        if(mCurve != null)
//        {
        mSamplingTime += Time.deltaTime * timeScale;

//			if(mIsLookAt == false)
//            { 
//                mIsLookAt = true; 
//				mController.DOLookAt(mLookAt, 0.5f, AxisConstraint.Y);
//            } 

        switch(mCurve.Dir)
        {
            case AniCurveDirection.Forward:
                if(mSamplingTime >= mCurve.OffsetStartTime && mSamplingTime < mCurve.OffsetEndTime)
                    mController.position = new Vector3(
                        mController.position.x + mController.forward.x * mCurve.DirVaule * timeScale, 
                        Mathf.Max(0, mCurve.aniCurve.Evaluate(mSamplingTime)), 
                        mController.transform.position.z + mController.forward.z * mCurve.DirVaule * timeScale);
                else
                    mController.position = new Vector3(
                        mController.position.x, 
                        mCurve.aniCurve.Evaluate(mSamplingTime), 
                        mController.position.z);
                break;
            case AniCurveDirection.Back:
                if (mSamplingTime >= mCurve.OffsetStartTime && mSamplingTime < mCurve.OffsetEndTime)
                    mController.position = new Vector3(
                        mController.position.x + mController.forward.x * -mCurve.DirVaule * timeScale,
                        Mathf.Max(0, mCurve.aniCurve.Evaluate(mSamplingTime)), 
                        mController.position.z + mController.forward.z * -mCurve.DirVaule * timeScale);
                else
                    mController.position = new Vector3(
                        mController.position.x, 
                        Mathf.Max(0, mCurve.aniCurve.Evaluate(mSamplingTime)), 
                        mController.position.z);
                break;

            default : 
                mController.position = new Vector3(
                    mController.position.x, 
                    Mathf.Max(0, mCurve.aniCurve.Evaluate(mSamplingTime)), 
                    mController.position.z);
                break;
        }

        if (mSamplingTime >= mCurve.LifeTime)
        {
            mIsPlaying = false;
//                mIsLookAt = false;
            mSamplingTime = 0;
        }
//        }
//        else
//        {
//            mIsLookAt = false;
//        }
    }

    public void FixedUpdate()
    {
        if(!mIsPlaying || timeScale <= GameConst.Min_TimePause)
            return;

        calculation();
    }
}
