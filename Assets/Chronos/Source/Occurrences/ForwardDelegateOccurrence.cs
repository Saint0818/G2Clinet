using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace Chronos
{
	/// <summary>
	/// An occurrence action that can only be executed when time goes forward. Does not return any transferable state object that could be used to revert its effect.
	/// </summary>
	public delegate void ForwardOnlyAction();
	
	internal sealed class ForwardDelegateOccurrence : Occurrence
	{
		private ForwardOnlyAction forward { get; set; }
		
		public ForwardDelegateOccurrence(ForwardOnlyAction forward)
		{
			this.forward = forward;
		}
		
		public override void Forward ()
		{
			forward();
		}

		public override void Backward ()
		{
			Debug.LogWarning("Trying to revert a forward-only occurrence.");
		}
	}
}
