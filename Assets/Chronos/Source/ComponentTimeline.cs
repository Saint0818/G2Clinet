using UnityEngine;
using System.Collections;

namespace Chronos
{
	public interface IComponentTimeline
	{
		Component component { get; }
		void Start();
		void Update();
		void FixedUpdate();
		void AdjustProperties(float timeScale);
	}

	public abstract class ComponentTimeline<T> : IComponentTimeline where T : Component
	{
		protected Timeline timeline { get; private set; }
		public T component { get; protected set; }

		Component IComponentTimeline.component
		{
			get { return component; }
		}

		public ComponentTimeline(Timeline timeline)
		{
			this.timeline = timeline;
		}

		public virtual void Start() { }
		public virtual void Update() { }
		public virtual void FixedUpdate() { }
		public virtual void CopyProperties(T source) { }
		public virtual void AdjustProperties(float timeScale) { }

		public bool Cache(T source)
		{
			if (component == null && source != null)
			{
				CopyProperties(source);
			}

			component = source;

			return source != null;
		}
	}
}
