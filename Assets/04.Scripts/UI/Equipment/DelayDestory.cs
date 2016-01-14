using JetBrains.Annotations;
using UnityEngine;

public class DelayDestory : MonoBehaviour
{
    public float AliveTimeInSeconds;

    [UsedImplicitly]
	private void Update()
    {
        AliveTimeInSeconds -= Time.deltaTime;
        if(AliveTimeInSeconds <= 0)
            Destroy(gameObject);
    }
}
