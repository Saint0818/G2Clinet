using UnityEngine;
using System.Collections;
using DG.Tweening;

public class LayupCurveCounter
{
    private GameObject self;
    private string curveName;
    private float curveTime = 0;
    private bool isFindCurve = false;
    private bool isplaying = false;
    private float timeScale = 1f;
    private TLayupCurve Curve;
    private Vector3 recordPlayerPosition;

    public float Timer
    {
        set{ timeScale = value; }
    }

    public bool IsPlaying
    {
        get{ return isplaying; }
    }

    private EAnimatorState state = EAnimatorState.Layup;
//    private bool isShootJumpActive = false;
    private Vector3 layupPoint;

    public void Init(int index, GameObject player, Vector3 layuppoint)
    {
        self = player;
        layupPoint = layuppoint;
        curveName = string.Format("{0}{1}", state.ToString(), index);

        if (Curve == null || (Curve != null && Curve.Name != curveName))
        {
            Curve = null;
            for (int i = 0; i < ModelManager.Get.AnimatorCurveManager.Layup.Length; i++)
                if (ModelManager.Get.AnimatorCurveManager.Layup[i].Name == curveName)
                    Curve = ModelManager.Get.AnimatorCurveManager.Layup[i];
        }

        //
        recordPlayerPosition = self.transform.position;
        isFindCurve = Curve != null ? true : false;
        curveTime = 0;
        isplaying = true;

        if (curveName != string.Empty && !isFindCurve && GameStart.Get.IsDebugAnimation)
            LogMgr.Get.LogError("Can not Find aniCurve: " + curveName);
    }

    private void Calculation()
    {	
		if (!isplaying || timeScale <= GameConst.Min_TimePause)
				return;

        if (Curve != null)
        {
            curveTime += Time.deltaTime * timeScale;

            Vector3 position = self.transform.position;
            position.y = Mathf.Max(0, Curve.aniCurve.Evaluate(curveTime));

            if (position.y < 0)
                position.y = 0;

            if (curveTime >= Curve.StartMoveTime)
            {
                position.x = Mathf.Lerp(recordPlayerPosition.x, layupPoint.x, (curveTime - Curve.StartMoveTime) / (Curve.ToBasketTime - Curve.StartMoveTime));
                position.z = Mathf.Lerp(recordPlayerPosition.z, layupPoint.z, (curveTime - Curve.StartMoveTime) / (Curve.ToBasketTime - Curve.StartMoveTime));
            }

            self.transform.position = position;

            if (curveTime >= Curve.LifeTime)
                isplaying = false;
        }
        else
        {
            isplaying = false;
            LogMgr.Get.LogError("playCurve is null");
        }
    }

    public void FixedUpdate()
    {
        Calculation();
    }
}
