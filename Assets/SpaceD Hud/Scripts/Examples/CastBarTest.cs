using UnityEngine;
using System.Collections;

public class CastBarTest : MonoBehaviour {

	public SpaceD_CastBar castBar;
	public SpaceD_Spells spellDatabase;
	private SpellInfo spell1;
	private SpellInfo spell2;

	void Start()
	{
		if (this.castBar != null && this.spellDatabase != null)
		{
			this.spell1 = spellDatabase.Get(2);
			this.spell2 = spellDatabase.Get(3);

			this.StartCoroutine("StartTestRoutine");
		}
	}

	IEnumerator StartTestRoutine()
	{
		yield return new WaitForSeconds(1f);

		this.castBar.StartCasting(this.spell1, this.spell1.CastTime, (Time.time + this.spell1.CastTime));

		yield return new WaitForSeconds(1f + this.spell1.CastTime);

		this.castBar.StartCasting(this.spell2, this.spell2.CastTime, (Time.time + this.spell2.CastTime));

		yield return new WaitForSeconds(this.spell2.CastTime * 0.75f);

		this.castBar.Interrupt();

		this.StartCoroutine("StartTestRoutine");
	}
}
