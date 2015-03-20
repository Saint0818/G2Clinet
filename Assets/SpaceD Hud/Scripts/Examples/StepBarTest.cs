using UnityEngine;
using System.Collections;

public class StepBarTest : MonoBehaviour {

	private SpaceD_StepBar bar;
	public float stepDelay = 0.5f;

	void Start()
	{
		this.bar = this.GetComponent<SpaceD_StepBar>();

		if (this.bar == null)
		{
			this.enabled = false;
			return;
		}

		this.StartCoroutine("FillProgress");
	}

	private IEnumerator FillProgress()
	{
		this.bar.SetStep(5);
		yield return new WaitForSeconds(this.stepDelay);
		this.bar.SetStep(4);
		yield return new WaitForSeconds(this.stepDelay);
		this.bar.SetStep(3);
		yield return new WaitForSeconds(this.stepDelay);
		this.bar.SetStep(2);
		yield return new WaitForSeconds(this.stepDelay);
		this.bar.SetStep(1);
		yield return new WaitForSeconds(this.stepDelay);
		this.bar.SetStep(0);
		yield return new WaitForSeconds(this.stepDelay);
		this.bar.SetStep(1);
		yield return new WaitForSeconds(this.stepDelay);
		this.bar.SetStep(2);
		yield return new WaitForSeconds(this.stepDelay);
		this.bar.SetStep(3);
		yield return new WaitForSeconds(this.stepDelay);
		this.bar.SetStep(4);
		yield return new WaitForSeconds(this.stepDelay);

		this.StartCoroutine("FillProgress");
	}
}
