using UnityEngine;
using System.Collections;

public class RealBall : MonoBehaviour
{
    public BallTrigger Trigger;
    public GameObject Effect;
    public SphereCollider realBallCollider;
    public Rigidbody RigidbodyCom;

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
        set{ 
            if (Trigger != null)
            {
                Trigger.SetBoxColliderEnable(value);
            }
        }
    }

    public Transform Parent
    {
        set{
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
        set{
            if (RigidbodyCom){
                RigidbodyCom.useGravity = value;
                RigidbodyCom.isKinematic = !RigidbodyCom.useGravity;
            }
        }
    }

    public void AddForce(Vector3 to, ForceMode mode)
    {
        //DebugLog
//        Debug.LogError("AddForce : " + to);
        RigidbodyCom.AddRelativeForce(to, mode);
    }

    public Vector3 MoveVelocity
    {
        set{ 
            //DebugLog
//            Debug.LogError("MoveVelocity : " + value);
            if(RigidbodyCom)
                RigidbodyCom.velocity = value;
        }

        get { 
            return RigidbodyCom == null ? Vector3.zero : RigidbodyCom.velocity;
        }
    }
}
