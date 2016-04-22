﻿using UnityEngine;
using System.Collections;
using GameEnum;

/// <summary>
/// 只做Ｘ跟Ｚ軸位移
/// </summary>
public class SharedCurveCounter
{
	//共用Curve : push, pick, fall
    private GameObject self;
    private string curveName;
    private float curveTime = 0;
    private bool isFindCurve = false;
    private bool isplaying = false;
    private float timeScale = 1f;
    private TSharedCurve Curve;

    public float Timer
    {
        set{ timeScale = value; }
    }

    public bool IsPlaying
    {
        get{ return isplaying; }
        set{ isplaying = value;}
    }

//    private EAnimatorState state = EAnimatorState.Pick;

	public void Init(EAnimatorState state, int index,GameObject player)
    {
        self = player;
		curveName = string.Format("{0}{1}", state.ToString(), index);
		
        if (Curve == null || (Curve != null && Curve.Name != curveName))
        {
            Curve = null;

            switch (state)
            {
                case EAnimatorState.Push:
                    for (int i = 0; i < CurveManager.Get.AnimatorCurve.Push.Length; i++)
                        if (CurveManager.Get.AnimatorCurve.Push[i].Name == curveName)
                            Curve = CurveManager.Get.AnimatorCurve.Push[i];
                    break;

                case EAnimatorState.Pick:
                    for (int i = 0; i < CurveManager.Get.AnimatorCurve.PickBall.Length; i++)
                        if (CurveManager.Get.AnimatorCurve.PickBall[i].Name == curveName)
                            Curve = CurveManager.Get.AnimatorCurve.PickBall[i];
                    break;

                case EAnimatorState.Fall:
                    for (int i = 0; i < CurveManager.Get.AnimatorCurve.Fall.Length; i++)
                        if (CurveManager.Get.AnimatorCurve.Fall[i].Name == curveName)
                            Curve = CurveManager.Get.AnimatorCurve.Fall[i];
                    break;
            }

        }
        isFindCurve = Curve != null ? true : false;
        curveTime = 0;
        isplaying = true;

		//用來Debug 如果有忘記加curve Perfab的話
        if (curveName != string.Empty && !isFindCurve && LobbyStart.Get.IsDebugAnimation)
            LogMgr.Get.LogError("Can not Find aniCurve: " + curveName);
    }

    private void Calculation()
    {
		if (!isplaying || timeScale <= GameConst.Min_TimePause)
            return;

        if (Curve != null)
        {
            curveTime += Time.deltaTime * timeScale;

            if (curveTime >= Curve.StartTime && curveTime <= Curve.EndTime)
            {
				//向前移動或向後移動一段距離 : StartTime - EndTime
                switch (Curve.Dir)
                {
                    case AniCurveDirection.Forward:
                        self.transform.position = new Vector3(self.transform.position.x + (self.transform.forward.x * Curve.DirVaule * timeScale), 0, 
                        self.transform.position.z + (self.transform.forward.z * Curve.DirVaule) * timeScale);
                        break;
                    case AniCurveDirection.Back:
                        self.transform.position = new Vector3(self.transform.position.x + (self.transform.forward.x * -Curve.DirVaule * timeScale), 0, 
                        self.transform.position.z + (self.transform.forward.z * -Curve.DirVaule) * timeScale);
                        break;
                }
            }

            if (curveTime >= Curve.LifeTime)
                isplaying = false;
        }

    }

    public void FixedUpdate()
    {
        Calculation();
    }
}
