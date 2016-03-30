﻿using UnityEngine;
using System.Collections;
using DG.Tweening;
using GameEnum;

/// <summary>
/// JumpBall 跟 Rebound共用
/// </summary>

public class ReboundCurveCounter
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

    private EAnimatorState state = EAnimatorState.Rebound;
    private Vector3 skillMoveTarget;
    private float BodyHeight;
    private Vector3 reboundMove;
//    private bool isJumpBall = false;

    public void Init(GameObject player, int index, Vector3 skillmovetarget, float bodyHeight, Vector3 reboundmove)
    {
        self = player;
        skillMoveTarget = skillmovetarget;
        BodyHeight = bodyHeight;
        reboundMove = reboundmove;
        curveName = string.Format("{0}{1}", state.ToString(), index);

        if (Curve == null || (Curve != null && Curve.Name != curveName))
        {
            Curve = null;
            for (int i = 0; i < ModelManager.Get.AnimatorCurveManager.Rebound.Length; i++)
                if (ModelManager.Get.AnimatorCurveManager.Rebound[i].Name == curveName)
                    Curve = ModelManager.Get.AnimatorCurveManager.Rebound[i];
        }
        isFindCurve = Curve != null ? true : false;
        curveTime = 0;
        isplaying = true;
        if (curveName != string.Empty && !isFindCurve && LobbyStart.Get.IsDebugAnimation)
            LogMgr.Get.LogError("Can not Find aniCurve: " + curveName);
    }

    private void Calculation()
    {	
		if (timeScale <= GameConst.Min_TimePause)
            return;

        if (isplaying && Curve != null)
        {
            curveTime += Time.deltaTime * timeScale;
            if (Curve.isSkill)
            {
				//轉向
                self.transform.LookAt(new Vector3(skillMoveTarget.x, self.transform.position.y, skillMoveTarget.z));
                if (skillMoveTarget.y > BodyHeight)
                {
                    self.transform.position = new Vector3(Mathf.Lerp(self.transform.position.x, skillMoveTarget.x, curveTime), 
                        Mathf.Max(0, Curve.aniCurve.Evaluate(curveTime) * ((skillMoveTarget.y - BodyHeight) / 3)), 
                        Mathf.Lerp(self.transform.position.z, skillMoveTarget.z, curveTime));
                }
                else
                {
                    self.transform.position = new Vector3(Mathf.Lerp(self.transform.position.x, skillMoveTarget.x, curveTime), 
                        self.transform.position.y, 
                        Mathf.Lerp(self.transform.position.z, skillMoveTarget.z, curveTime));
                }
            }
            else
            {
				if(self.transform.position.y > 0.2f) {
					if (curveTime < 0.7f && reboundMove != Vector3.zero)
					{
						self.transform.position = new Vector3(self.transform.position.x + reboundMove.x * Time.deltaTime * 2 * timeScale, 
							Mathf.Max(0, Curve.aniCurve.Evaluate(curveTime)), 
							self.transform.position.z + reboundMove.z * Time.deltaTime * 2 * timeScale);
					}
					else
						self.transform.position = new Vector3(self.transform.position.x + self.transform.forward.x * 0.05f, 
							Mathf.Max(0, Curve.aniCurve.Evaluate(curveTime)), 
							self.transform.position.z + self.transform.forward.z * 0.05f);
				} else {
					self.transform.position = new Vector3(self.transform.position.x, 
						Mathf.Max(0, Curve.aniCurve.Evaluate(curveTime)), 
						self.transform.position.z);
				}
            }

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
