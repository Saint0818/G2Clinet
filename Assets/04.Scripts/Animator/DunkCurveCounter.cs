using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DunkCurveCounter : MonoBehaviour
{
    private EAnimatorState state = EAnimatorState.Dunk;
    private string curveName;
    private TDunkCurve Curve;
    private Vector3 dunkPoint;
    private float curveTime = 0;
    private bool isplaying = false;
    private bool isFindCurve = false;
    private float timeScale = 1f;
    private GameObject self;
    private bool isAnimatorMove = false;
	private Vector3 recordPlayerPosition;
	private float rotateAngle;

    private bool IsAnimatorMove
    {
        get{ return isAnimatorMove; }
        set{ isAnimatorMove = value; }
    }

    public float Timer
    {
        set{ timeScale = value; }
    }

    public bool IsPlaying
    {
        set{ isplaying = value;}
        get{ return isplaying; }
    }

	public void Init(int index, GameObject player, Vector3 dunkpos, float rotateangle)
    {
        curveName = string.Format("{0}{1}", state.ToString(), index);
        self = player;
		rotateAngle = rotateangle;

        //CourtMgr.Get.DunkPoint[Team.GetHashCode()].transform.position
        dunkPoint = dunkpos;

        if (Curve == null || (Curve != null && Curve.Name != curveName))
        {
            Curve = null;
            for (int i = 0; i < ModelManager.Get.AnimatorCurveManager.Dunk.Length; i++)
                if (ModelManager.Get.AnimatorCurveManager.Dunk[i].Name == curveName)
                    Curve = ModelManager.Get.AnimatorCurveManager.Dunk[i];
        }
        recordPlayerPosition = player.transform.position;
        isFindCurve = Curve != null ? true : false;
        IsAnimatorMove = false;
        isplaying = true;
        curveTime = 0;

        if (curveName != string.Empty && !isFindCurve && GameStart.Get.IsDebugAnimation)
            LogMgr.Get.LogError("Can not Find aniCurve: " + curveName);
    }

    void FixedUpdate()
    {
        CalculationDunkMove();
    }

    private bool iscanblock = false;

    public bool IsCanBlock{ get{ return iscanblock;} }

    private void CalculationDunkMove()
    {
        if (!isplaying)
            return;

        if (Curve != null)
        {
            curveTime += Time.deltaTime * timeScale;

            Vector3 position = self.transform.position;
            if (timeScale != 0)
            { 
                position.y = Curve.aniCurve.Evaluate(curveTime);

                if (position.y < 0)
                    position.y = 0; 

                if (curveTime >= Curve.StartMoveTime)
                {
					position.x = Mathf.Lerp(recordPlayerPosition.x, dunkPoint.x, (curveTime - Curve.StartMoveTime) / (Curve.ToBasketTime - Curve.StartMoveTime));
					position.z = Mathf.Lerp(recordPlayerPosition.z, dunkPoint.z, (curveTime - Curve.StartMoveTime) / (Curve.ToBasketTime - Curve.StartMoveTime));
                }

                if (IsAnimatorMove == false && curveTime >= Curve.StartMoveTime && curveTime <= Curve.ToBasketTime)
                { 
                    IsAnimatorMove = true; 
					//將玩家轉向籃筐灌籃，避免背對籃筐
					self.transform.DORotate(new Vector3(0, rotateAngle, 0), Curve.ToBasketTime, 0); 
                } 
            }
            self.transform.position = position;

            if (curveTime > Curve.BlockMomentStartTime && curveTime <= Curve.BlockMomentEndTime)
                iscanblock = true;
            else
                iscanblock = false;

            if (curveTime >= Curve.LifeTime)
            {
                self.transform.DOKill();
                isplaying = false;
                iscanblock = false;
                IsAnimatorMove = false;
            }
        }
        else
        {
            isplaying = false;
            IsAnimatorMove = false;
            LogMgr.Get.LogError("playCurve is null");
        }
    }

    public void CloneMesh()
    {
        EffectManager.Get.CloneMesh(self, Curve.CloneMaterial, Curve.CloneDeltaTime, Curve.CloneCount);
    }
}
