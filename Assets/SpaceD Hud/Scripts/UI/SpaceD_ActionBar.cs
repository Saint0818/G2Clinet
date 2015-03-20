using UnityEngine;
using System.Collections;

public class SpaceD_ActionBar : MonoBehaviour {

	public SpaceD_Spells spellDatabase;
	public SpaceD_SpellSlot[] slots;
	public SpaceD_CastBar castBar;

	void Start()
	{
		// Assign example spells
		this.AssignSpellSlot(0, spellDatabase.Get(0));
		this.AssignSpellSlot(1, spellDatabase.Get(1));
		this.AssignSpellSlot(2, spellDatabase.Get(2));
		this.AssignSpellSlot(3, spellDatabase.Get(3));
		this.AssignSpellSlot(4, spellDatabase.Get(4));
		this.AssignSpellSlot(5, spellDatabase.Get(5));
		
		// Special slot
		this.AssignSpellSlot(10, spellDatabase.Get(6));
	}

	private void AssignSpellSlot(int slotIndex, SpellInfo spellInfo)
	{
		if (slots[slotIndex] != null)
			slots[slotIndex].Assign(spellInfo);
	}
}
