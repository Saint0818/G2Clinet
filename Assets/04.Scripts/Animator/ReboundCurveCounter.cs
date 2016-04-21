using GameEnum;
using UnityEngine;

/// <summary>
/// JumpBall 跟 Rebound共用
/// </summary>
public class ReboundCurveCounter
{
    private Transform mController;
    private string curveName;
    private float mCurrentTime = 0;
    private bool mIsPlaying = false;
    private float timeScale = 1f;
    private TReboundCurve mCurve;

    public float Timer
    {
        set{ timeScale = value; }
    }

    private EAnimatorState state = EAnimatorState.Rebound;
    private Vector3 skillMoveTarget;
    private float BodyHeight;
    private Vector3 mHorizontalMove; 

    public void Apply(Transform player, int index, Vector3 skillmovetarget, float bodyHeight, 
                      Vector3 reboundMove)
    {
        mController = player;
        skillMoveTarget = skillmovetarget;
        BodyHeight = bodyHeight;
        mHorizontalMove = reboundMove;
        curveName = string.Format("{0}{1}", state, index);

        if (mCurve == null || (mCurve != null && mCurve.Name != curveName))
        {
            mCurve = null;
            for (int i = 0; i < ModelManager.Get.AnimatorCurve.Rebound.Length; i++)
                if (ModelManager.Get.AnimatorCurve.Rebound[i].Name == curveName)
                    mCurve = ModelManager.Get.AnimatorCurve.Rebound[i];
        }
        mCurrentTime = 0;
        mIsPlaying = mCurve != null;
        if (curveName != string.Empty && mCurve == null && LobbyStart.Get.IsDebugAnimation)
            LogMgr.Get.LogError("Can not Find aniCurve: " + curveName);
    }

    public void FixedUpdate()
    {
        if(timeScale <= GameConst.Min_TimePause || !mIsPlaying)
            return;

        mCurrentTime += Time.deltaTime * timeScale;
        if (mCurve.isSkill)
        {
            //轉向
            mController.LookAt(new Vector3(skillMoveTarget.x, mController.position.y, skillMoveTarget.z));
            if (skillMoveTarget.y > BodyHeight)
            {
                mController.position = new Vector3(Mathf.Lerp(mController.position.x, skillMoveTarget.x, mCurrentTime), 
                    Mathf.Max(0, mCurve.aniCurve.Evaluate(mCurrentTime) * ((skillMoveTarget.y - BodyHeight) / 3)), 
                    Mathf.Lerp(mController.position.z, skillMoveTarget.z, mCurrentTime));
            }
            else
            {
                mController.position = new Vector3(Mathf.Lerp(mController.position.x, skillMoveTarget.x, mCurrentTime), 
                    mController.position.y, 
                    Mathf.Lerp(mController.position.z, skillMoveTarget.z, mCurrentTime));
            }
        }
        else
        {
            if(mController.position.y > 0.2f)
            {
                float newX;
                float newZ;
                if(mCurrentTime < 0.7f && mHorizontalMove != Vector3.zero)
                {
                    newX = mController.position.x + mHorizontalMove.x * Time.deltaTime * timeScale;
                    newZ = mController.position.z + mHorizontalMove.z * Time.deltaTime * timeScale;
//                    mController.position =
//                        new Vector3(mController.position.x + mHorizontalMove.x * Time.deltaTime * 2 * timeScale,
//                                    Mathf.Max(0, mCurve.aniCurve.Evaluate(mCurrentTime)),
//                                    mController.position.z + mHorizontalMove.z * Time.deltaTime * 2 * timeScale);
                }
                else
                {
                    newX = mController.position.x + mController.forward.x * 0.05f;
                    newZ = mController.position.z + mController.forward.z * 0.05f;
//                    mController.position =
//                        new Vector3(mController.position.x + mController.forward.x * 0.05f,
//                            Mathf.Max(0, mCurve.aniCurve.Evaluate(mCurrentTime)),
//                            mController.position.z + mController.forward.z * 0.05f);
                }

                mController.position = new Vector3(newX, 
                        Mathf.Max(0, mCurve.aniCurve.Evaluate(mCurrentTime)), newZ);
            }
            else
            {
                mController.position = new Vector3(mController.position.x,
                                                   Mathf.Max(0, mCurve.aniCurve.Evaluate(mCurrentTime)),
                                                   mController.position.z);
            }
        }

        if(mCurrentTime >= mCurve.LifeTime)
            mIsPlaying = false;
    }
}
