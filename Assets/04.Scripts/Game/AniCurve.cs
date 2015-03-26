using UnityEngine;
using System.Collections;

[System.Serializable]
public class TDunkCurve {
	public string Name = "Curve_Name";
	public float LifeTime = 3.166f;
	public float ToBasketTime = 0.166f;
	public float StartMoveTime = 1f; 
	public AnimationCurve aniCurve = new AnimationCurve();
}

public class AniCurve : MonoBehaviour {
	public TDunkCurve[] Dunk;
}
