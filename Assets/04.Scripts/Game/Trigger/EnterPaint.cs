using GameEnum;
using UnityEngine;

public class EnterPaint : MonoBehaviour
{
    private const int TriggerAgainTime = 2;

    public ETeamKind Team;
    private float mElapsedTime;

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            mElapsedTime = 0;
            GameController.Get.PlayerEnterPaint(Team, col.gameObject);
        }
    }

    void OnTriggerStay(Collider col)
    {
        if(mElapsedTime >= TriggerAgainTime && col.CompareTag("Player"))
        {
            mElapsedTime = 0;
            GameController.Get.PlayerEnterPaint(Team, col.gameObject);
        }
    }

    void Update()
    {
        mElapsedTime += Time.deltaTime;
    }
}
