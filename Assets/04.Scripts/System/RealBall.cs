using UnityEngine;
using System.Collections;
using AI;
using JetBrains.Annotations;
using GameEnum;
using DG.Tweening;

/// <summary>
/// realBallCollider 用來與地板作實際的碰撞
/// Trigger 用來判斷
/// </summary>

public class RealBall : MonoBehaviour
{
    public BallTrigger Trigger;
    public SphereCollider realBallCollider;
    public Rigidbody RigidbodyCom;
    private GameObject spotlight;

    public GameObject mRealBallSFX;
    // 特效顯示的時間. 單位: 秒.
    private readonly CountDownTimer mRealBallSFXTimer = new CountDownTimer(1);
    private AutoFollowGameObject BallShadow;

    void Awake()
    {
        mRealBallSFXTimer.TimeUpListener += HideBallSFX;
        GameObject obj = Instantiate(Resources.Load("Prefab/Stadium/BallShadow")) as GameObject;
        if (obj)
        {
            BallShadow = obj.GetComponent<AutoFollowGameObject>();
            spotlight = obj.transform.FindChild("SpotLight").gameObject; 
            BallShadow.Enable = true;
            BallShadow.SetTarget(gameObject);
            spotlight.SetActive(false);
        }
    }

    [UsedImplicitly]
    private void FixedUpdate()
    {
        mRealBallSFXTimer.Update(Time.deltaTime);
    }

    public void HideBallSFX()
    {
        mRealBallSFX.SetActive(false);
        mRealBallSFXTimer.Stop();
    }

    public bool IsBallSFXEnabled()
    {
        return mRealBallSFX.activeInHierarchy;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sfxTime"> 特效顯示的時間, 單位:秒. -1: 表示特效永遠顯示, 必須要手動關閉. </param>
    public void ShowBallSFX(float sfxTime = -1)
    {
        mRealBallSFX.SetActive(true);
        if (sfxTime > 0)
            mRealBallSFXTimer.Start(sfxTime);
    }

    public bool ColliderEnable
    {
        set
        { 
            if (realBallCollider)
                realBallCollider.enabled = value;
        }	
    }

    public bool TriggerEnable
    {
        set
        { 
            if (Trigger != null)
            {
                Trigger.SetBoxColliderEnable(value);
            }
        }
    }

    public Transform Parent
    {
        set
        {
            gameObject.transform.parent = value;
            if (gameObject.transform.parent)
            {
                gameObject.transform.localPosition = Vector3.zero;  
                gameObject.transform.localEulerAngles = Vector3.zero;
                gameObject.transform.localScale = Vector3.one;
            }
        }
    }

    public bool Gravity
    {
        set
        {
            if (RigidbodyCom)
            {
                RigidbodyCom.useGravity = value;
                RigidbodyCom.isKinematic = !RigidbodyCom.useGravity;
            }
        }

        get{ return RigidbodyCom.useGravity; }
    }

    public void AddForce(Vector3 to, ForceMode mode)
    {
        //Debug.LogError("AddForce : " + to);
        RigidbodyCom.AddRelativeForce(to, mode);
    }

    public Vector3 MoveVelocity
    {
        set
        { 
            //DebugLog
//            Debug.LogError("MoveVelocity : " + value);
            if (RigidbodyCom)
                RigidbodyCom.velocity = value;
        }

        get
        { 
            return RigidbodyCom == null ? Vector3.zero : RigidbodyCom.velocity;
        }
    }

    public EPlayerState RealBallState;

    public void SetBallState(EPlayerState state, PlayerBehaviour player = null)
    {
        if (!GameController.Get.IsStart && state != EPlayerState.Start &&
        state != EPlayerState.Reset && GameStart.Get.TestMode == EGameTest.None)
            return;
        
        //        Debug.LogError("SetBallState : " + state.ToString());
        SetBallOwnerNull();

        RealBallState = state;

        switch (state)
        {
            case EPlayerState.Dribble0:
                ColliderEnable = false;
                Gravity = false;

                if (player)
                    Parent = player.DummyBall.transform;
                TriggerEnable = false;
                HideBallSFX();

                if(spotlight)
                    spotlight.SetActive(false);
                break;

            case EPlayerState.Shoot0: 
            case EPlayerState.Shoot1: 
            case EPlayerState.Shoot2: 
            case EPlayerState.Shoot3: 
            case EPlayerState.Shoot4: 
            case EPlayerState.Shoot5: 
            case EPlayerState.Shoot6: 
            case EPlayerState.Shoot7: 
            case EPlayerState.Shoot20: 
            case EPlayerState.Layup0: 
            case EPlayerState.Layup1: 
            case EPlayerState.Layup2: 
            case EPlayerState.Layup3: 
            case EPlayerState.TipIn: 
                if (player)
                    Parent = player.DummyBall.transform;

                Gravity = false;
                break;

            case EPlayerState.Shooting: 
                if (spotlight)
                    spotlight.SetActive(false);

                Parent = null;

                Gravity = true;
                break;

            case EPlayerState.Pass0: 
            case EPlayerState.Pass2: 
            case EPlayerState.Pass1: 
            case EPlayerState.Pass3: 
            case EPlayerState.Pass4: 
            case EPlayerState.Pass5: 
            case EPlayerState.Pass6: 
            case EPlayerState.Pass7: 
            case EPlayerState.Pass8: 
            case EPlayerState.Pass9: 
                Gravity = false;
                if(spotlight)
                    spotlight.SetActive(false);
                break;

            case EPlayerState.Steal0:
            case EPlayerState.Steal1:
            case EPlayerState.Steal2:
                GameController.Get.IsPassing = false;

                Vector3 newDir = Vector3.zero;
                newDir.Set(Random.Range(-1, 1), 0, Random.Range(-1, 1));

				// 10 是速度. 如果給太低, 球會在持球者附近, 變成持球者還是可以繼續撿球.
                MoveVelocity = newDir.normalized * 10;
                ShowBallSFX();
                if(spotlight)
                    spotlight.SetActive(true);
                break;
            case EPlayerState.JumpBall:
                if (!GameController.Get.IsJumpBall)
                {
                    GameController.Get.IsJumpBall = true;
                    GameController.Get.IsPassing = false;

                    Vector3 v1;
                    if (player != null)
                        v1 = player.transform.position; // 球要拍到某位球員的位置.
							else
                        v1 = gameObject.transform.forward * -1;

                    MoveVelocity = GameFunction.GetVelocity(gameObject.transform.position, v1, 60);
                    ShowBallSFX();
                    if(spotlight)
                        spotlight.SetActive(false);
                    AudioMgr.Get.PlaySound(SoundType.SD_Rebound);
                }
                break;

            case EPlayerState.Block0:
            case EPlayerState.Block1:
            case EPlayerState.Block2:
            case EPlayerState.Block20:
            case EPlayerState.KnockDown0: 
            case EPlayerState.KnockDown1: 
                GameController.Get.Shooter = null;
                GameController.Get.IsPassing = false;

                Vector3 v = gameObject.transform.forward * -1;
                if (player != null)
                    v = player.transform.forward * 10;

                MoveVelocity = v;
                ShowBallSFX();
                if(spotlight)
                    spotlight.SetActive(true);
                break;

            case EPlayerState.Dunk0:
            case EPlayerState.Dunk1:
            case EPlayerState.Dunk3:
            case EPlayerState.Dunk5:
            case EPlayerState.Dunk7:
                ColliderEnable = true;
                Gravity = false;

                if (player)
                    Parent = player.DummyBall.transform;
                TriggerEnable = false;
                HideBallSFX();
                if(spotlight)
                    spotlight.SetActive(false);
                break;

            case EPlayerState.DunkBasket:
                if (player)
                    gameObject.transform.position = player.DummyBall.transform.position;

                AddForce(Vector3.down * 1, ForceMode.VelocityChange);
                break;

            case EPlayerState.Reset:
                Gravity = false;
                gameObject.transform.position = new Vector3(0, 7, 0);
                ShowBallSFX();
                if(spotlight)
                    spotlight.SetActive(false);
                break;

            case EPlayerState.Start:
                gameObject.transform.localPosition = new Vector3(0, 6, 0);
                Gravity = true;
                if(spotlight)
                    spotlight.SetActive(false);
                break;

            case EPlayerState.HoldBall:
            case EPlayerState.Pick0:
            case EPlayerState.Pick1:
            case EPlayerState.Pick2:
                ColliderEnable = false;
                if (player)
                {
                    Parent = player.DummyBall.transform;
                    gameObject.transform.DOKill();
                }

                Gravity = false;
                TriggerEnable = false;
                HideBallSFX();
                if(spotlight)
                    spotlight.SetActive(false);
                break;
        }
    }

    public void SetBallOwnerNull()
    {
        ColliderEnable = true;
        TriggerEnable = true;
        Parent = null;
        Gravity = true;
    }

    public void SetBallStateByLobby(EPlayerState state, Transform dummyTransfrom)
    {
        spotlight.SetActive(false);
        LayerMgr.Get.SetLayerAllChildren(gameObject, "Player");
        switch (state)
        {
            case EPlayerState.Dribble0:
                ColliderEnable = false;
                Gravity = false;

                if (dummyTransfrom)
                    Parent = dummyTransfrom;
                TriggerEnable = false;
                HideBallSFX();
                break;

            case EPlayerState.Shoot0: 
            case EPlayerState.Shoot1: 
            case EPlayerState.Shoot2: 
            case EPlayerState.Shoot3: 
            case EPlayerState.Shoot4: 
            case EPlayerState.Shoot5: 
            case EPlayerState.Shoot6: 
            case EPlayerState.Shoot7: 
            case EPlayerState.Shoot20: 
            case EPlayerState.Layup0: 
            case EPlayerState.Layup1: 
            case EPlayerState.Layup2: 
            case EPlayerState.Layup3: 
            case EPlayerState.TipIn: 
            case EPlayerState.Shooting: 
                Parent = null;
                Gravity = true;
                break;
        }
    }
}
