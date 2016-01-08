using System.Collections.Generic;
using UnityEngine;

public class UITableExtension : UITable {

	public override void Reposition () {
		if (Application.isPlaying && !mInitDone && NGUITools.GetActive(this)) Init();

		mReposition = false;
		Transform myTrans = transform;
		List<Transform> ch = GetChildList();
		if (ch.Count > 0) RepositionVariableSizeNew(ch);

		if (keepWithinPanel && mPanel != null)
		{
			mPanel.ConstrainTargetToBounds(myTrans, true);
			UIScrollView sv = mPanel.GetComponent<UIScrollView>();
			if (sv != null) sv.UpdateScrollbars(true);
		}

		if (onReposition != null)
			onReposition();
	}

	protected void RepositionVariableSizeNew (List<Transform> children)
	{
		float xOffset = 0;
		float yOffset = 0;

		int cols = columns > 0 ? children.Count / columns + 1 : 1; // 4
		int rows = columns > 0 ? columns : children.Count; // 1

		Bounds[,] bounds = new Bounds[rows, cols]; // [1, 4]
		Bounds[] boundsRows = new Bounds[cols];// 4
		Bounds[] boundsCols = new Bounds[rows];// 1

		int x = 0;
		int y = 0;

		for (int i = 0, imax = children.Count; i < imax; ++i)
		{
			Transform t = children[i];
			Bounds b = NGUIMath.CalculateRelativeWidgetBounds(t, !hideInactive);

			Vector3 scale = t.localScale;
			b.min = Vector3.Scale(b.min, scale);
			b.max = Vector3.Scale(b.max, scale);
			bounds[y, x] = b;

			boundsRows[x].Encapsulate(b);
			boundsCols[y].Encapsulate(b);

			if (++y >= columns && columns > 0)
			{
				y = 0;
				++x;
			}
		}

		x = 0;
		y = 0;

		Vector2 po = NGUIMath.GetPivotOffset(cellAlignment);

		for (int i = 0, imax = children.Count; i < imax; ++i)
		{
			Transform t = children[i];
			Bounds b = bounds[y, x];
			Bounds br = boundsRows[x];
			Bounds bc = boundsCols[y];

			Vector3 pos = t.localPosition;
			pos.x = xOffset + b.extents.x - b.center.x;
			pos.x -= Mathf.Lerp(0f, b.max.x - b.min.x - br.max.x + br.min.x, po.x) - padding.x;

			xOffset += br.size.x + padding.x * 2f;

			t.localPosition = pos;

			if (++y >= columns && columns > 0)
			{
				y = 0;
				++x;

				yOffset = 0f;
			}
		}
	}
}
