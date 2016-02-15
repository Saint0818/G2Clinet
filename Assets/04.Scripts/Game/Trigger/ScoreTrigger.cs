using UnityEngine;
using System.Collections;
using GameEnum;

public delegate void ScoreDelegate(int Team, int IntTrigger);
public class ScoreTrigger : MonoBehaviour
{
	public int Team;
	public int IntTrigger;
	public bool Into = false;

	public ScoreDelegate ScoreDel;

    void OnTriggerEnter(Collider c) {
		if(c.CompareTag("RealBall"))
			if(ScoreDel != null)
				ScoreDel(Team, IntTrigger);
    }
}
