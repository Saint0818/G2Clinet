using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ShootCurveCounter
{
    private GameObject self;
    private string curveName;
    private float curveTime = 0;
    private bool isFindCurve = false;
    private bool isplaying = false;
    private float timeScale = 1f;
    private TShootCurve Curve;
    private bool isAnimatorMove = false;
	private Vector3 rotateTo;

    public float Timer
    {
        set{ timeScale = value; }
    }

    public bool IsPlaying
    {
        get{ return isplaying; }
    }

    private bool IsAnimatorMove
    {
        get{ return isAnimatorMove; }
        set{ isAnimatorMove = value; }
    }

    private EAnimatorState state = EAnimatorState.Shoot;

	public void Init(int index, GameObject player, Vector3 rotateto)
    {
		rotateTo = rotateto;
        self = player;
        curveName = string.Format("{0}{1}", state.ToString(), index);

        if (Curve == null || (Curve != null && Curve.Name != curveName))
        {
            for (int i = 0; i < ModelManager.Get.AnimatorCurveManager.Shoot.Length; i++)
                if (ModelManager.Get.AnimatorCurveManager.Shoot[i].Name == curveName)
                    Curve = ModelManager.Get.AnimatorCurveManager.Shoot[i];
        }
        isFindCurve = Curve != null ? true : false;
        curveTime = 0;
        isplaying = true;

        if (curveName != string.Empty && !isFindCurve && GameStart.Get.IsDebugAnimation)
            LogMgr.Get.LogError("Can not Find aniCurve: " + curveName);
    }

    private void Calculation()
    {	
        if (isplaying && Curve != null)
        {
            curveTime += Time.deltaTime * timeScale;

			if (IsAnimatorMove == false)
            { 
                IsAnimatorMove = true; 
				self.transform.DOLookAt(rotateTo, 0.5f, AxisConstraint.Y);
            } 

            switch (Curve.Dir)
            {
                case AniCurveDirection.Forward:
                    if (curveTime >= Curve.OffsetStartTime && curveTime < Curve.OffsetEndTime)
                        self.transform.position = new Vector3(self.transform.position.x + (self.transform.forward.x * Curve.DirVaule * timeScale), 
                            Mathf.Max(0, Curve.aniCurve.Evaluate(curveTime)), 
                            self.transform.position.z + (self.transform.forward.z * Curve.DirVaule * timeScale));
                    else
                        self.transform.position = new Vector3(self.transform.position.x, Curve.aniCurve.Evaluate(curveTime), self.transform.position.z);
                    break;
                case AniCurveDirection.Back:
                    if (curveTime >= Curve.OffsetStartTime && curveTime < Curve.OffsetEndTime)
                        self.transform.position = new Vector3(self.transform.position.x + (self.transform.forward.x * -Curve.DirVaule * timeScale), 
                            Mathf.Max(0, Curve.aniCurve.Evaluate(curveTime)), 
                            self.transform.position.z + (self.transform.forward.z * -Curve.DirVaule * timeScale));
                    else
                        self.transform.position = new Vector3(self.transform.position.x, Mathf.Max(0, Curve.aniCurve.Evaluate(curveTime)), self.transform.position.z);
                    break;

                default : 
                    self.transform.position = new Vector3(self.transform.position.x, Mathf.Max(0, Curve.aniCurve.Evaluate(curveTime)), self.transform.position.z);
                    break;
            }

            if (curveTime >= Curve.LifeTime)
            {
                isplaying = false;
                IsAnimatorMove = false;
                curveTime = 0;
            }
        }
        else
        {
            IsAnimatorMove = false;
        }
    }

    public void FixedUpdate()
    {
        Calculation();
    }
}
