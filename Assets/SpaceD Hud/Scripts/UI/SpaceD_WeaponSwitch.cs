using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceD_WeaponSwitch : UIWidgetContainer {

	public Transform container;
	public UIScrollView scrollView;

	public float scaleLerp = 8f;
	public float positionLerp = 8f;
	public float alphaLerp = 8f;

	public float activeScale = 1f;
	public float inactiveScale = 0.8f;

	public float activePositionY = 0f;
	public float inactivePositionY = 14f;

	public float activeAlpha = 1f;
	public float inactiveAlpha = 0.4f;

	public bool isVisible { get; private set; }
	public bool autoHideEnable = true;
	public float autoHideAfter = 3f;
	public float hideDuration = 1f;

	public float cellWidth = 170f;

	private int ActiveWeapon = 0;
	private float lastActivity = 0f;
	private List<Transform> currentList = new List<Transform>();

	private static int SortByName(Transform A, Transform B)
	{
		return A.name.CompareTo(B.name);
	}

	void Start()
	{
		if (this.scrollView == null)
		{
			this.scrollView = this.GetComponent<UIScrollView>();
			
			if (this.scrollView == null)
			{
				Debug.LogWarning(GetType() + " requires " + typeof(UIScrollView) + " in order to work.", this);
				this.enabled = false;
				return;
			}
		}

		if (this.container == null)
		{
			Debug.LogWarning(GetType() + " requires that you define the container of the items.", this);
			this.enabled = false;
			return;
		}

		// prepare the is visible variable
		UIPanel panel = this.GetComponent<UIPanel>();
		if (panel != null)
			this.isVisible = (panel.alpha > 0f) ? true : false;
	}

	void Update()
	{
		if (this.currentList.Count == 0 || !this.isVisible)
			return;

		// Check if it's time to hide the panel
		if (this.autoHideEnable && Time.time > (this.lastActivity + this.autoHideAfter))
			this.Hide();

		// Handle animations
		for (int i = 0; i < this.currentList.Count; i++)
		{
			Transform item = this.GetWeapon(i);
			UISprite sprite = item.GetComponent<UISprite>();

			if (i == this.ActiveWeapon)
			{
				item.localScale = Vector3.Lerp(item.localScale, new Vector3(this.activeScale, this.activeScale, 1f), Time.deltaTime * this.scaleLerp);
				item.localPosition = new Vector3(item.localPosition.x, Mathf.Lerp(item.localPosition.y, this.activePositionY, Time.deltaTime * this.positionLerp), item.localPosition.z);

				if (sprite != null)
					sprite.alpha = Mathf.Lerp(sprite.alpha, this.activeAlpha, Time.deltaTime * this.alphaLerp);
			}
			else
			{
				item.localScale = Vector3.Lerp(item.localScale, new Vector3(this.inactiveScale, this.inactiveScale, 1f), Time.deltaTime * this.scaleLerp);
				item.localPosition = new Vector3(item.localPosition.x, Mathf.Lerp(item.localPosition.y, this.inactivePositionY, Time.deltaTime * this.positionLerp), item.localPosition.z);

				if (sprite != null)
					sprite.alpha = Mathf.Lerp(sprite.alpha, this.inactiveAlpha, Time.deltaTime * this.alphaLerp);
			}
		}
	}

	public void Show()
	{
		// Stop the hide coroutine if running
		this.StopCoroutine("_Hide");

		// Set visible
		this.isVisible = true;

		// Set he panels opacity to one
		UIPanel panel = this.GetComponent<UIPanel>();
		
		if (panel != null)
			panel.alpha = 1f;
	}
	
	public void Hide()
	{
		// Star the hide coroutine
		this.StartCoroutine("_Hide");
	}
	
	IEnumerator _Hide()
	{
		UIPanel panel = this.GetComponent<UIPanel>();
		
		if (panel == null)
			yield break;
		
		float startTime = Time.time;
		
		while (Time.time <= (startTime + this.hideDuration))
		{
			float RemainingTime = (startTime + this.hideDuration) - Time.time;
			
			// Update the alpha by the percentage of the time elapsed
			panel.alpha = RemainingTime / this.hideDuration;
			
			yield return 0;
		}
		
		// Make sure it's zero
		panel.alpha = 0f;
		this.isVisible = false;
	}

	/// <summary>
	/// Gets the weapon on the given index from the current list.
	/// </summary>
	/// <returns>The weapon transform.</returns>
	/// <param name="index">Index.</param>
	public Transform GetWeapon(int index)
	{
		return this.currentList[index];
	}

	/// <summary>
	/// Gets the index of the currently active weapon.
	/// </summary>
	/// <value>The index of the current.</value>
	public int currentIndex {
		get {
			return this.ActiveWeapon;
		}
	}

	/// <summary>
	/// Gets the last weapon index in the list.
	/// </summary>
	/// <value>The last index.</value>
	public int lastIndex {
		get {
			return (this.currentList.Count - 1);
		}
	}
	
	/// <summary>
	/// The previous weapon index based on the given one.
	/// </summary>
	/// <returns>The index.</returns>
	/// <param name="index">Index.</param>
	public int prevIndex(int index)
	{
		return (index > 0) ? (index - 1) : this.lastIndex;
	}
	
	/// <summary>
	/// The next weapon index based on the given one.
	/// </summary>
	/// <returns>The index.</returns>
	/// <param name="index">Index.</param>
	public int nextIndex(int index)
	{
		return (index < this.lastIndex) ? (index + 1) : 0;
	}

	/// <summary>
	/// Adds a weapon to the view.
	/// </summary>
	/// <param name="prefab">Prefab.</param>
	public void AddWeapon(GameObject prefab)
	{
		if (this.container == null)
			return;
		
		// Instantiate the object
		GameObject obj = (GameObject)Instantiate(prefab);
		
		// Rename
		obj.name = "Weapon " + (this.lastIndex + 1).ToString();
		
		// Add to the container
		obj.transform.parent = this.container;
		obj.transform.localScale = new Vector3(this.inactiveScale, this.inactiveScale, 1f);
		obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, this.inactivePositionY, obj.transform.localPosition.z);

		// Prepare the weapon sprite
		UISprite sprite = obj.GetComponent<UISprite>();
		if (sprite != null)
			sprite.alpha = this.inactiveAlpha;

		// Add the weapon to the list
		this.currentList.Add(obj.transform);
		
		// Refresh the view
		this.RefreshView();
	}


	/// <summary>
	/// Focuse the weapon on the given index.
	/// </summary>
	/// <param name="weaponIndex">Weapon index.</param>
	public void FocusWeapon(int weaponIndex)
	{
		this.FocusWeapon(weaponIndex, true);
	}

	/// <summary>
	/// Focuse the weapon on the given index.
	/// </summary>
	/// <param name="weaponIndex">Weapon index.</param>
	/// <param name="animate">If set to <c>true</c> animate the position change.</param>
	public void FocusWeapon(int weaponIndex, bool animate)
	{
		if (this.currentList.Count == 0) return;

		if (weaponIndex < 0 || weaponIndex > this.lastIndex)
		{
			Debug.LogWarning("Trying to center on invalid item index.", this);
			return;
		}

		// Show the panel
		this.Show();
		
		// Register this as last activity
		this.lastActivity = Time.time;

		// Get the item at the given index
		Transform item = this.GetWeapon(weaponIndex);

		// Move the surrounding weapons
		this.MoveSurroundingOf(weaponIndex);

		// Set the currently active index
		this.ActiveWeapon = weaponIndex;

		// Center on the item
		this.CenterOn(item, animate);
	}

	private void MoveSurroundingOf(int weaponIndex)
	{
		// Get the item at the given index
		Transform item = this.GetWeapon(weaponIndex);

		// Move the surrounding items around this objects position
		Transform prevItem = this.GetWeapon(this.prevIndex(weaponIndex));
		Transform nextItem = this.GetWeapon(this.nextIndex(weaponIndex));
		
		// Move the prev and next items
		prevItem.localPosition = new Vector3((item.localPosition.x - this.cellWidth), prevItem.localPosition.y, prevItem.localPosition.z);
		nextItem.localPosition = new Vector3((item.localPosition.x + this.cellWidth), nextItem.localPosition.y, nextItem.localPosition.z);
	}

	/// <summary>
	/// Centers the on the target transform.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="spring">If set to <c>true</c> spring will be used to move to the target position.</param>
	void CenterOn(Transform target, bool spring)
	{
		// Get the scrollview panel center
		Vector3[] corners = this.scrollView.panel.worldCorners;
		Vector3 panelCenter = (corners[2] + corners[0]) * 0.5f;

		Transform panelTrans = this.scrollView.panel.cachedTransform;
		
		// Figure out the difference between the chosen child and the panel's center in local coordinates
		Vector3 cp = panelTrans.InverseTransformPoint(target.position);
		Vector3 cc = panelTrans.InverseTransformPoint(panelCenter);
		Vector3 localOffset = cp - cc;

		// Restrict Y and Z
		localOffset.y = 0f;
		localOffset.z = 0f;

		// Spring the panel to this calculated position
		if (spring)
			SpringPanel.Begin(this.scrollView.panel.cachedGameObject, (panelTrans.localPosition - localOffset), this.positionLerp).onFinished = OnCenterFinish;
		else
			this.scrollView.MoveRelative(new Vector3((localOffset.x * -1), 0f, 0f));
	}

	void OnCenterFinish()
	{
		// Force the scrollview offset to zero once in a while
		// otherwise the scrollview offset could grow up to hundreds of thousands
		// resulting in unnecessary memory usage

		// Start by getting the active item
		Transform item = this.GetWeapon(this.ActiveWeapon);

		// Check if we're too far with the positioning
		if (Mathf.Abs(item.localPosition.x) > 2000f)
		{
			// Reposition all the weapons
			this.RepositionWeapons();

			// Reposition the item to the correct center position without animating it
			this.FocusWeapon(this.ActiveWeapon, false);
		}
	}

	/// <summary>
	/// Repositions the weapons in a horizontal grid.
	/// </summary>
	public void RepositionWeapons()
	{
		if (this.currentList.Count == 0) return;

		// Arrange the items
		for (int i = 0; i < this.currentList.Count; i++)
		{
			Transform t = this.currentList[i];
			t.localPosition = new Vector3(cellWidth * i, t.localPosition.y, t.localPosition.z);
		}
	}

	/// <summary>
	/// Refreshs the view, that repositions the weapons and centers on the active without animations.
	/// </summary>
	[ContextMenu("Refresh view")]
	public void RefreshView()
	{
		// reposition the weapons
		this.RepositionWeapons();
		
		// Center on the active item
		this.CenterOn(this.GetWeapon(this.ActiveWeapon), false);
		
		// Move the surrounding of the active
		this.MoveSurroundingOf(this.ActiveWeapon);
	}
}
