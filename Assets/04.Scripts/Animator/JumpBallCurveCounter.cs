using UnityEngine;
using System.Collections;
using DG.Tweening;

public class JumpBallCurveCounter
{
    private GameObject self;
    private string curveName;
    private float curveTime = 0;
    private bool isFindCurve = false;
    private bool isplaying = false;
    private float timeScale = 1f;
    private TReboundCurve Curve;

    public float Timer
    {
        set{ timeScale = value; }
    }

    public bool IsPlaying
    {
        get{ return isplaying; }
    }

    private EAnimatorState state = EAnimatorState.JumpBall;
    private Vector3 reboundMove;
    private bool isJumpBall = false;

    public void Init(GameObject player, int index, Vector3 reboundmove)
    {
        self = player;
        reboundMove = reboundmove;
        curveName = string.Format("{0}{1}", state.ToString(), index);

        if (Curve == null || (Curve != null && Curve.Name != curveName))
        {
            Curve = null;
			for (int i = 0; i < ModelManager.Get.AnimatorCurveManager.JumpBall.Length; i++)
				if (ModelManager.Get.AnimatorCurveManager.JumpBall[i].Name == curveName)
					Curve = ModelManager.Get.AnimatorCurveManager.JumpBall[i];
        }
        isFindCurve = Curve != null ? true : false;
        curveTime = 0;
        isplaying = true;
        if (curveName != string.Empty && !isFindCurve && GameStart.Get.IsDebugAnimation)
            LogMgr.Get.LogError("Can not Find aniCurve: " + curveName);
    }

    private void Calculation()
    {	
        if (timeScale == 0)
            return;

        if (isplaying && Curve != null)
        {
            curveTime += Time.deltaTime * timeScale;
//                if (curveTime < 0.7f && !IsBallOwner && reboundMove != Vector3.zero)
	        if (curveTime < 0.7f && reboundMove != Vector3.zero)
	        {
	            self.transform.position = new Vector3(self.transform.position.x, 
	                Mathf.Max(0, Curve.aniCurve.Evaluate(curveTime)), 
	                self.transform.position.z);
	        }
	        else
	            self.transform.position = new Vector3(self.transform.position.x, 
	                Mathf.Max(0, Curve.aniCurve.Evaluate(curveTime)), 
	                self.transform.position.z);

            if (curveTime >= Curve.LifeTime)
            {
                isplaying = false;
            }
        }
        else
            isplaying = false;
    }

    public void FixedUpdate()
    {
        Calculation();
    }
}
