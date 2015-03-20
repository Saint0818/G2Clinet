using UnityEngine;
using System.Collections;

public class SpaceD_Spells : ScriptableObject {

	public SpellInfo[] spells;

	public SpellInfo Get(int index)
	{
		return (spells[index]);
	}
}