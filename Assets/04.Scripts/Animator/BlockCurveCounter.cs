using UnityEngine;
using System.Collections;

public class BlockCurveCounter
{
    private GameObject self;
    private EAnimatorState state = EAnimatorState.Block;
    private string curveName;
    private TBlockCurve playerBlockCurve;
    private float blockCurveTime = 0;
    private bool isplaying = false;
    private bool isFindCurve = false;
    private float timeScale = 1f;
    private Vector3 skillMoveTarget;
    private bool isDunkBlock;
    private float BodyHeight;

    public float Timer
    {
        set{ timeScale = value;}
    }

    public bool IsPlaying
    {
        get{return isplaying;}
    }

    public void Init(int index, GameObject player, Vector3 skillmoveTarget, bool isdunkblock)
    {
        self = player;
        isDunkBlock = isdunkblock;
        curveName = string.Format("{0}{1}", state.ToString(), index);
        skillMoveTarget = skillmoveTarget;

        BodyHeight = self.transform.GetComponent<CapsuleCollider>().height + 0.2f;

        if (playerBlockCurve == null || (playerBlockCurve != null && playerBlockCurve.Name != curveName))
        {

            playerBlockCurve = null;
            for (int i = 0; i < ModelManager.Get.AnimatorCurveManager.Block.Length; i++)
                if (ModelManager.Get.AnimatorCurveManager.Block[i].Name == curveName)
                    playerBlockCurve = ModelManager.Get.AnimatorCurveManager.Block[i];
        }
        isFindCurve = playerBlockCurve != null ? true : false;
        blockCurveTime = 0;
        isplaying = true;

        if (curveName != string.Empty && !isFindCurve && GameStart.Get.IsDebugAnimation)
            LogMgr.Get.LogError("Can not Find aniCurve: " + curveName);
    }
        
    private void CalculationBlock()
    {
		if (!isplaying || timeScale <= GameConst.Min_TimePause)
            return;

        if (playerBlockCurve != null)
        {
            blockCurveTime += Time.deltaTime * timeScale;

            if (playerBlockCurve.isSkill)
            {
                self.transform.LookAt(new Vector3(skillMoveTarget.x, self.transform.position.y, skillMoveTarget.z));

                if (blockCurveTime < 1f)
                {
                    if (!isDunkBlock)
                    {
                        self.transform.position = new Vector3(Mathf.Lerp(self.transform.position.x, skillMoveTarget.x, blockCurveTime), 
                            Mathf.Max(0, playerBlockCurve.aniCurve.Evaluate(blockCurveTime) * ((skillMoveTarget.y - BodyHeight) / 3)), 
                            Mathf.Lerp(self.transform.position.z, skillMoveTarget.z, blockCurveTime));
                    }
                    else
                    {
                        self.transform.position = new Vector3(Mathf.Lerp(self.transform.position.x, skillMoveTarget.x, blockCurveTime), 
                            Mathf.Max(0, playerBlockCurve.aniCurve.Evaluate(blockCurveTime) * (skillMoveTarget.y / 3)), 
                            Mathf.Lerp(self.transform.position.z, skillMoveTarget.z, blockCurveTime));
                    }
                }
                else
                {
                    if (!isDunkBlock)
                    {
                        self.transform.position = new Vector3(self.transform.position.x, 
                            Mathf.Max(0, playerBlockCurve.aniCurve.Evaluate(blockCurveTime) * ((skillMoveTarget.y - BodyHeight) / 3)), 
                            self.transform.position.z);
                    }
                    else
                    {
                        self.transform.position = new Vector3(self.transform.position.x, 
                            Mathf.Max(0, playerBlockCurve.aniCurve.Evaluate(blockCurveTime) * (skillMoveTarget.y / 3)), 
                            self.transform.position.z);
                    }
                }   
            }
            else
            {
                if (blockCurveTime < 1f)
                    self.transform.position = new Vector3(self.transform.position.x + (self.transform.forward.x * 0.03f * timeScale), 
                        Mathf.Max(0, playerBlockCurve.aniCurve.Evaluate(blockCurveTime)), 
                        self.transform.position.z + (self.transform.forward.z * 0.03f * timeScale));
                else
                    self.transform.position = new Vector3(self.transform.position.x, 
                        Mathf.Max(0, playerBlockCurve.aniCurve.Evaluate(blockCurveTime)), 
                        self.transform.position.z);
            }

            if (blockCurveTime >= playerBlockCurve.LifeTime)
            {
                isplaying = false;
            }
        }
    }

    public void FixedUpdate()
    {
        CalculationBlock();
    }
}
