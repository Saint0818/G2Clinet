using UnityEngine;
using UnityEditor;

namespace Chronos
{
	[CustomEditor(typeof(AnimatorRecorder)), CanEditMultipleObjects]
	public class AnimatorRecorderEditor : RecorderEditor<AnimatorRecorder>
	{

	}
}