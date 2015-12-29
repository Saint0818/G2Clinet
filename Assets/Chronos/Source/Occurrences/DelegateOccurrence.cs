using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace Chronos
{
	/// <summary>
	/// An occurrence action that is executed when time goes forward. Returns any object that will be transfered to the backward action if time is rewinded.
	/// </summary>
	public delegate object ForwardAction();

	/// <summary>
	/// An occurrence action that is executed when time goes backward. Uses the object returned by the forward action to remember state information.
	/// </summary>
	public delegate void BackwardAction(object transfer);

	internal sealed class DelegateOccurrence : Occurrence
	{
		private ForwardAction forward { get; set; }
		private BackwardAction backward { get; set; }
		private object transfer { get; set; }

		public DelegateOccurrence(ForwardAction forward, BackwardAction backward)
		{
			this.forward = forward;
			this.backward = backward;
		}

		public override void Forward ()
		{
			transfer = forward();
		}

		public override void Backward ()
		{
			backward(transfer);
		}
	}
}
