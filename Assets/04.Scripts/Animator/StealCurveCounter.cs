using UnityEngine;
using System.Collections;
using DG.Tweening;

public class StealCurveCounter
{
    private GameObject self;
    private string curveName;
    private float curveTime = 0;
    private bool isFindCurve = false;
    private bool isplaying = false;
    private float timeScale = 1f;
    private TStealCurve Curve;

    public float Timer
    {
        set{ timeScale = value; }
    }

    public bool IsPlaying
    {
        get{ return isplaying; }
    }

    private EAnimatorState state = EAnimatorState.Steal;

    public void Init(int index, GameObject player)
    {
        self = player;
        curveName = string.Format("{0}{1}", state.ToString(), index);
        if (Curve == null || (Curve != null && Curve.Name != curveName))
        {
            Curve = null;
            for (int i = 0; i < ModelManager.Get.AnimatorCurveManager.Steal.Length; i++)
                if (ModelManager.Get.AnimatorCurveManager.Steal[i].Name == curveName)
                    Curve = ModelManager.Get.AnimatorCurveManager.Steal[i];
        }
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

            if (curveTime >= Curve.StartTime)
            {
                if (GameController.Get.BallOwner != null)
                {
                    self.transform.DOMove((GameController.Get.BallOwner.transform.position + Vector3.forward * (-2)), Curve.LifeTime);
                    self.transform.LookAt(new Vector3(GameController.Get.BallOwner.transform.position.x, self.transform.position.y, GameController.Get.BallOwner.transform.position.z));
                    GameController.Get.BallOwner.AniState(EPlayerState.GotSteal);
                }
                else
                {
                    if (GameController.Get.Catcher != null)
                    {
                        self.transform.DOMove((GameController.Get.Catcher.transform.position + Vector3.forward * (-2)), Curve.LifeTime);
                        self.transform.LookAt(new Vector3(GameController.Get.Catcher.transform.position.x, self.transform.position.y, GameController.Get.Catcher.transform.position.z));
                        GameController.Get.Catcher.AniState(EPlayerState.GotSteal);
                    }
                    else if (GameController.Get.Shooter != null)
                    {
                        self.transform.DOMove((GameController.Get.Shooter.transform.position + Vector3.forward * (-2)), Curve.LifeTime);
                        self.transform.LookAt(new Vector3(GameController.Get.Shooter.transform.position.x, self.transform.position.y, GameController.Get.Shooter.transform.position.z));
                        GameController.Get.Shooter.AniState(EPlayerState.GotSteal);
                    }
                }
                isplaying = false;
            }
        }
    }

    public void FixedUpdate()
    {
        Calculation();
    }
}
