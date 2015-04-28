using UnityEngine;
using System.Collections;

public enum AniCurveDirection
{
	None,
	Forward,
	Back
}

[System.Serializable]
public class TDunkCurve {
	public string Name = "Curve_Name";
	public float LifeTime = 3.166f;
	public float ToBasketTime = 0.166f;
	public float StartMoveTime = 1f; 
	public AnimationCurve aniCurve = new AnimationCurve();
}

[System.Serializable]
public class TBlockCurve {
	public string Name = "Curve_Name";
	public float LifeTime = 1.8f;
	public AnimationCurve aniCurve = new AnimationCurve();
}

[System.Serializable]
public class TShootCurve {
	public string Name = "Curve_Name";
	public AniCurveDirection Dir = AniCurveDirection.None;
	public float OffsetStartTime = 1f;
	public float OffsetEndTime = 1f;
	public float LifeTime = 1.8f;
	public AnimationCurve aniCurve = new AnimationCurve();
}

[System.Serializable]
public class TReboundCurve {
	public string Name = "Curve_Name";
	public float LifeTime = 1.8f;
	public AnimationCurve aniCurve = new AnimationCurve();
}

[System.Serializable]
public class TLayupCurve {
	public string Name = "Curve_Name";
	public float LifeTime = 1.8f;
	public AnimationCurve aniCurve = new AnimationCurve();
}

public class AniCurve : MonoBehaviour {
	public TDunkCurve[] Dunk;
	public TBlockCurve[] Block;
	public TShootCurve[] Shoot;
	public TReboundCurve[] Rebound; 
	public TLayupCurve[] Layup;
}
