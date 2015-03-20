using UnityEngine;
using System.Collections;

public class RadarConeTest : MonoBehaviour {

	public float speed = 1f;

	void Update()
	{
		this.transform.Rotate(0f, 0f, this.speed);
	}
}
