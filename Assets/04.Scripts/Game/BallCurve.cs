using UnityEngine;
using System.Collections;

[System.Serializable]
public class TBallCurve {
	public string Name = "Curve_Name";
	public float LifeTime = 1f;
	public AnimationCurve aniCurve = new AnimationCurve();
}

public class BallCurve : MonoBehaviour {
	public TBallCurve Ball;
	public TBallCurve ShortBall;
}
