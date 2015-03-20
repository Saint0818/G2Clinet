using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("SpaceD UI/Spell Slot")]
public class SpaceD_SpellSlot : SpaceD_IconSlot
{
	public static SpaceD_SpellSlot current;
	
	private SpaceD_CastBar castBar;
	public SpaceD_Cooldown cooldownHandle;
	private SpellInfo spellInfo;
	
	/// <summary>
	/// Assign event listener.
	/// </summary>
	public List<EventDelegate> onAssign = new List<EventDelegate>();
	
	/// <summary>
	/// Unassign event listener.
	/// </summary>
	public List<EventDelegate> onUnassign = new List<EventDelegate>();
	
	protected override void Awake()
	{
		base.Awake();
		
		// Try finding the cooldown handler
		if (this.cooldownHandle == null) this.cooldownHandle = this.GetComponent<SpaceD_Cooldown>();
		if (this.cooldownHandle == null) this.cooldownHandle = this.GetComponentInChildren<SpaceD_Cooldown>();
		
		// Get the action bar component
		SpaceD_ActionBar actionBar = this.transform.root.GetComponentInChildren<SpaceD_ActionBar>();
		
		// Copy the cast bar referrence
		this.castBar = actionBar.castBar;
	}
	
	/// <summary>
	/// Gets the spell info of the spell assigned to this slot.
	/// </summary>
	/// <returns>The spell info.</returns>
	public SpellInfo GetSpellInfo()
	{
		return spellInfo;
	}
	
	/// <summary>
	/// Determines whether this slot is assigned.
	/// </summary>
	/// <returns><c>true</c> if this instance is assigned; otherwise, <c>false</c>.</returns>
	public override bool IsAssigned()
	{
		return (this.spellInfo != null);
	}
	
	/// <summary>
	/// Assign the slot by spell info.
	/// </summary>
	/// <param name="spellInfo">Spell info.</param>
	public bool Assign(SpellInfo spellInfo)
	{
		if (spellInfo == null)
			return false;
		
		// Use the base class assign
		if (this.Assign(spellInfo.Icon))
		{
			// Set the spell info
			this.spellInfo = spellInfo;
			
			// Check if we have a cooldown handler
			if (this.cooldownHandle != null)
				this.cooldownHandle.OnAssignSpell(spellInfo);
			
			// Execute the assign event listener
			this.ExecuteOnAssign();
			
			// Success
			return true;
		}
		
		return false;
	}
	
	/// <summary>
	/// Executes the on assign event listener.
	/// </summary>
	protected virtual void ExecuteOnAssign()
	{
		current = this;
		EventDelegate.Execute(this.onAssign);
		current = null;
	}
	
	/// <summary>
	/// Assign the slot by the passed source slot.
	/// </summary>
	/// <param name="source">Source.</param>
	public override bool Assign(Object source)
	{
		if (source is SpaceD_SpellSlot)
		{
			SpaceD_SpellSlot sourceSlot = source as SpaceD_SpellSlot;
			
			if (sourceSlot != null)
				return this.Assign(sourceSlot.GetSpellInfo());
		}
		
		// Default
		return false;
	}
	
	/// <summary>
	/// Unassign this slot.
	/// </summary>
	public override void Unassign()
	{
		// Remove the icon
		base.Unassign();
		
		// Clear the spell info
		this.spellInfo = null;
		
		// Check if we have a cooldown handler
		if (this.cooldownHandle != null)
			this.cooldownHandle.OnUnassign();
		
		// Execute the unassign event listener
		this.ExecuteOnUnassign();
	}
	
	/// <summary>
	/// Executes the on unassign event listener.
	/// </summary>
	protected virtual void ExecuteOnUnassign()
	{
		current = this;
		EventDelegate.Execute(this.onUnassign);
		current = null;
	}
	
	/// <summary>
	/// Determines whether this slot can swap with the specified target slot.
	/// </summary>
	/// <returns><c>true</c> if this instance can swap with the specified target; otherwise, <c>false</c>.</returns>
	/// <param name="target">Target.</param>
	public override bool CanSwapWith(Object target)
	{
		return (target is SpaceD_SpellSlot);
	}
	
	/// <summary>
	/// Performs a slot swap.
	/// </summary>
	/// <param name="targetObject">Target slot.</param>
	public override bool PerformSlotSwap(Object targetObject)
	{
		// Get the source slot
		SpaceD_SpellSlot targetSlot = (targetObject as SpaceD_SpellSlot);
		
		// Get the target slot icon
		SpellInfo targetSpellInfo = targetSlot.GetSpellInfo();
		
		// Assign the target slot with this one
		bool assign1 = targetSlot.Assign(this);
		
		// Assign this slot by the target slot spell info
		bool assign2 = this.Assign(targetSpellInfo);
		
		// Return the status
		return (assign1 && assign2);
	}
	
	/// <summary>
	/// Raises the click event.
	/// </summary>
	public override void OnClick()
	{
		if (!this.IsAssigned())
			return;
		
		// Test casting
		if (this.spellInfo.CastTime > 0f)
		{
			if (this.castBar.IsCasting)
				return;
			
			if (this.castBar != null)
				this.castBar.StartCasting(this.spellInfo, this.spellInfo.CastTime, (Time.time + this.spellInfo.CastTime));
		}
		
		// Check if the slot is on cooldown
		if (this.cooldownHandle != null)
		{
			if (this.cooldownHandle.IsOnCooldown)
				return;
			
			this.cooldownHandle.StartCooldown(this.spellInfo.Cooldown);
		}
	}
	
	/// <summary>
	/// Raises the tooltip event.
	/// </summary>
	/// <param name="show">If set to <c>true</c> show.</param>
	public override void OnTooltip(bool show)
	{
		if (show && this.IsAssigned())
		{
			// Set the title and description
			SpaceD_Tooltip.SetTitle(this.spellInfo.Name);
			SpaceD_Tooltip.SetDescription(this.spellInfo.Description);
			
			if (this.spellInfo.Flags.Has(SpellInfo_Flags.Passive))
			{
				SpaceD_Tooltip.AddAttribute("Passive", "");
			}
			else
			{
				// Power consumption
				if (this.spellInfo.PowerCost > 0f)
				{
					if (this.spellInfo.Flags.Has(SpellInfo_Flags.PowerCostInPct))
						SpaceD_Tooltip.AddAttribute(this.spellInfo.PowerCost.ToString("0") + "%", " Energy");
					else
						SpaceD_Tooltip.AddAttribute(this.spellInfo.PowerCost.ToString("0"), " Energy");
				}
				
				// Range
				if (this.spellInfo.Range > 0f)
				{
					if (this.spellInfo.Range == 1f)
						SpaceD_Tooltip.AddAttribute("Melee range", "");
					else
						SpaceD_Tooltip.AddAttribute(this.spellInfo.Range.ToString("0"), " yd range");
				}
				
				// Cast time
				if (this.spellInfo.CastTime == 0f)
					SpaceD_Tooltip.AddAttribute("Instant", "");
				else
					SpaceD_Tooltip.AddAttribute(this.spellInfo.CastTime.ToString("0.0"), " sec cast");
				
				// Cooldown
				if (this.spellInfo.Cooldown > 0f)
					SpaceD_Tooltip.AddAttribute(this.spellInfo.Cooldown.ToString("0.0"), " sec cooldown");
			}
			
			// Set the tooltip position
			SpaceD_Tooltip.SetPosition(this.iconSprite as UIWidget);
			
			// Show the tooltip
			SpaceD_Tooltip.Show();
			
			// Prevent hide
			return;
		}
		
		// Default hide
		SpaceD_Tooltip.Hide();
	}
}