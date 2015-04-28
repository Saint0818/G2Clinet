// <copyright file="MagicSplitscreen.cs" company="Armoire Cannonball">
// Copyright Armoire Cannonball, LLC (c) 2014. All Rights Reserved.
// </copyright>
// <author>Adam Ellis</author>
using System;
using UnityEngine;

/// <summary>
/// Controller class for two player Magic Splitscreen camera(s)
/// </summary>
public class SplitScreen : MonoBehaviour
{
	#region Unity inspector values
	/// <summary>
	/// Distance away from the player(s) that the camera(s) should be
	/// </summary>
	#if UNITY_4_5
	[Tooltip("Distance away from the player(s) that the camera(s) should be")]
	#endif
	public float cameraDistance = 30.0f;
	
	/// <summary>
	/// Desired camera rotation (in degrees)
	/// </summary>
	#if UNITY_4_5
	[Tooltip("Desired camera rotation (in degrees)")]
	#endif
	public Vector3 cameraRotation;
	
	/// <summary>
	/// How far apart players must be before splitscreen kicks in
	/// </summary>
	#if UNITY_4_5
	[Tooltip("How far apart players must be before splitscreen kicks in")]
	#endif
	public float triggerDistance = 10.0f;
	
	/// <summary>
	/// The scene's primary camera
	/// </summary>
	#if UNITY_4_5
	[Tooltip("The scene's primary camera")]
	#endif
	public Camera primaryCamera;
	
	/// <summary>
	/// A Transform that moves with player 1
	/// </summary>
	#if UNITY_4_5
	[Tooltip("A Transform that moves with player 1")]
	#endif
	public Transform player1;
	
	/// <summary>
	/// A Transform that moves with player 2
	/// </summary>
	#if UNITY_4_5
	[Tooltip("A Transform that moves with player 2")]
	#endif
	public Transform player2;
	
	/// <summary>
	/// Whether to show a separation stripe when the screen splits
	/// </summary>
	#if UNITY_4_5
	[Tooltip("Whether to show a separation stripe when the screen splits")]
	#endif
	public bool showSeparator;
	#endregion
	
	#region Private variables
	/// <summary>
	/// The scene's Audio Listener
	/// </summary>
	/// <remarks>This is automatically set (and, optionally, created) during initialization</remarks>
	private AudioListener audioListener;
	
	/// <summary>
	/// 2d camera displacement from player to camera target (the same as <c>cameraDisplacement3d</c> with <c>y</c> set to 0)
	/// </summary>
	private Vector3 cameraDisplacement2d;
	
	/// <summary>
	/// The rotation both cameras use
	/// </summary>
	/// <remarks>This is set up based on the values set for cameraRotation variable</remarks>
	private Quaternion cameraQuaternion;
	
	/// <summary>
	/// The point in space that camera 1 looks at
	/// </summary>
	private Vector3 cameraTarget1;
	
	/// <summary>
	/// The point in space that camera 2 looks at
	/// </summary>
	private Vector3 cameraTarget2;
	
	/// <summary>
	/// The position between both players
	/// </summary>
	private Vector3 centralPosition;
	
	/// <summary>
	/// The distance between both players
	/// </summary>
	private Vector3 distanceBetweenPlayers;
	
	/// <summary>
	/// Track whether the separator stripe has necessary components
	/// </summary>
	private bool isSeparatorUsable;
	
	/// <summary>
	/// The layer for the splitscreen mask
	/// </summary>
	private int maskLayer;
	
	/// <summary>
	/// Distance in front of Camera 2 that the splitscreen mask should be
	/// </summary>
	/// <remarks>This is automatically set to be just past the near plane inside that camera's viewing frustum</remarks>
	private float maskOffset;
	
	/// <summary>
	/// The calculated scale for the splitscreen mask
	/// </summary>
	private Vector3 maskScale;
	
	/// <summary>
	/// The Transform on the splitscreen mask
	/// </summary>
	private Transform maskTransform;
	
	/// <summary>
	/// 2d distance (in screen space) between the player and its camera's target
	/// </summary>
	private Vector2 screenDisplacement2d;
	
	/// <summary>
	/// Secondary camera
	/// </summary>
	/// <remarks>This is automatically created as a slightly altered clone of the primary camera</remarks>
	private Camera secondaryCamera;
	
	/// <summary>
	/// Material on the separator stripe
	/// </summary>
	private Material separatorMaterial;
	
	/// <summary>
	/// MeshRenderer for the separator stripe
	/// </summary>
	private MeshRenderer separatorRenderer;
	
	/// <summary>
	/// MeshRenderer for the splitscreen mask
	/// </summary>
	private MeshRenderer splitscreenMask;

	private bool isSplit = false;
	#endregion
	
	#region Public accessors
	/// <summary>
	/// Gets a value indicating whether the MagicSplitscreen has been properly initialized
	/// </summary>
	public bool IsInitialized { get; private set; }
	
	/// <summary>
	/// Gets a value indicating whether the screen is currently split, showing half of the screen for each player
	/// </summary>
	public bool IsSplitscreenOn { get; private set; }
	
	/// <summary>
	/// Gets a value representing an active player: p1 if it's assigned; p2 if it's assigned and p1 isn't; null otherwise.
	/// </summary>
	public Transform MainPlayer
	{
		get
		{
			if (this.player1)
			{
				return this.player1;
			}
			else if (this.player2)
			{
				return this.player2;
			}
			else
			{
				return null;
			}
		}
	}
	
	/// <summary>
	/// Gets the number of players being tracked
	/// </summary>
	public int NumPlayers
	{
		get
		{
			return (this.player1 ? 1 : 0) + (this.player2 ? 1 : 0);
		}
	}
	#endregion
	
	#region Unity methods
	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		this.Initialize();
		
		if (!this.IsInitialized)
		{
			Debug.LogError("MagicSplitscreen: There were problems initializing. This class will not run.");
		}
	}
	
	/// <summary>
	/// This is called every frame after all Update methods have been called
	/// </summary>
	void LateUpdate()
	{
		// If there were issues during initial validation, trying to continue will just spam the console with unnecessary errors, so don't bother
		if (!this.IsInitialized)
		{
			return;
		}
		
		// There must be at least one player assigned for the camera(s) to have something to track
		if (this.NumPlayers == 0)
		{
			Debug.LogWarning("MagicSplitscreen: No players are assigned. There is nothing to do.");
			return;
		}
		
		// Find the average location of all tracked players
		this.SetCentralPosition();
		
		// Place the AudioListener in the central position
		//        this.audioListener.transform.position = this.centralPosition;
		
		// Determine if players are far enough apart to use splitscreen
		this.distanceBetweenPlayers = this.centralPosition - this.MainPlayer.position;
		this.PerformSplitscreenCheck();
		
		// Position camera(s)
		if (this.IsSplitscreenOn)
		{
			// Adjust displacement to be in the direction of the central point but not at it
			this.cameraDisplacement2d = (this.distanceBetweenPlayers.normalized * this.triggerDistance);

			// Aim cameras at players
//			this.cameraTarget1 = this.player1.position + this.cameraDisplacement2d;
//			this.cameraTarget2 = this.player2.position - this.cameraDisplacement2d;
//			this.MoveCamera(this.primaryCamera, this.cameraTarget1);
			//			this.MoveCamera(this.secondaryCamera, this.cameraTarget2);
			isSplit = true;
//			CameraMgr.Get.SplitLookAt(true, this.cameraDisplacement2d);
			this.secondaryCamera.gameObject.transform.position = CameraMgr.Get.CourtCamera.gameObject.transform.position;
			this.secondaryCamera.gameObject.transform.LookAt(GameController.Get.Joysticker.gameObject.transform.position - this.cameraDisplacement2d);
			
			// Position the splitscreen mask in front of the second camera
			this.PositionSplitscreenMask(this.secondaryCamera, this.player2.position, this.player2.position + this.cameraDisplacement2d);
			this.separatorRenderer.enabled = this.isSeparatorUsable && this.showSeparator;
		}
		else
		{
			if(isSplit) {
				isSplit = false;
//				CameraMgr.Get.SplitLookAt(isSplit, Vector3.zero);
			}
			//            this.MoveCamera(this.primaryCamera, this.MainPlayer.position + this.distanceBetweenPlayers);
		}
	}
	#endregion
	
	#region Helper methods
	/// <summary>
	/// Set centralPosition to the 3D central point in the world equidistant from both players
	/// </summary>
	/// <remarks>If there is only one player, this will return that player's position</remarks>
	private void SetCentralPosition()
	{
		this.centralPosition = Vector3.zero;
		
		if (this.player1)
		{
			this.centralPosition += this.player1.position;
		}
		
		if (this.player2)
		{
			this.centralPosition += this.player2.position;
		}
		
		this.centralPosition /= this.NumPlayers;
	}
	
	/// <summary>
	/// Validates that required member variables are set and assigns values to the class's private members
	/// Sets IsInitialized to True if all expected data are present; False otherwise
	/// </summary>
	private void Initialize()
	{
		this.IsInitialized = true;
		
		//        this.cameraQuaternion = Quaternion.Euler(this.cameraRotation);
		//		this.cameraQuaternion = CameraMgr.Get.CourtCamera.gameObject.transform.rotation;
		this.maskScale = Vector3.one;
		
		// Validate Inspector fields
		if (!this.primaryCamera)
		{
			Debug.LogError("MagicSplitscreen: Primary Camera is not assigned.");
			this.IsInitialized = false;
		}
		else
		{
			// Put the mask just inside the view frustum
			this.maskOffset = this.primaryCamera.nearClipPlane + 0.1f;
		}
		
		//        AudioListener cameraListener = this.primaryCamera.GetComponent<AudioListener>() as AudioListener;
		//        if (cameraListener)
		//        {
		//            Debug.Log("MagicSplitscreen: Primary Camera has an AudioListener. It will be removed.");
		//            MonoBehaviour.Destroy(cameraListener);
		//        }
		//
		//        AudioListener[] listeners = GameObject.FindObjectsOfType(typeof(AudioListener)) as AudioListener[];
		//        if (listeners.Length == 0 ||
		//            (listeners.Length == 1 && listeners[0] == cameraListener))
		//        {
		//            if (listeners.Length == 0)
		//            {
		//                Debug.Log("MagicSplitscreen: Could not find an AudioListener. One will be created.");
		//            }
		//            else
		//            {
		//                Debug.Log("MagicSplitscreen: Creating a replacement AudioListener.");
		//            }
		//
		//            GameObject go = new GameObject();
		//            go.transform.parent = this.transform;
		//            go.name = "Audio Listener (MagicSplitscreen)";
		//            this.audioListener = go.AddComponent<AudioListener>();
		//        }
		//        else
		//        {
		//            Debug.LogError("MagicSplitscreen: There are unexpected AudioListeners in the scene. Please remove all but one.");
		//            this.IsInitialized = false;
		//        }
		
		// Set up the splitscreen mask and separator
		this.isSeparatorUsable = false;
		this.splitscreenMask = this.GetComponentInChildren<MeshRenderer>();
		if (!this.splitscreenMask)
		{
			Debug.LogError("MagicSplitscreen: No MeshRenderer mask found in Magic Splitscreen's children.");
			this.IsInitialized = false;
		}
		else
		{
			this.maskLayer = this.splitscreenMask.gameObject.layer;
			this.maskTransform = this.splitscreenMask.transform;
			
			MeshRenderer[] renderers = this.splitscreenMask.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer mr in renderers)
			{
				if (mr != this.splitscreenMask)
				{
					this.separatorRenderer = mr;
					break;
				}
			}
			
			if (!this.separatorRenderer)
			{
				Debug.LogWarning("Magic Splitscreen: The separator stripe is missing from the splitscreen mask.");
			}
			else
			{
				this.separatorMaterial = this.separatorRenderer.material;
				if (!this.separatorMaterial)
				{
					Debug.LogWarning("Magic Splitscreen: The separator stripe does not have a material.");
				}
				else
				{
					this.isSeparatorUsable = true;
				}
			}
		}
		
		this.InitializeCameras();
	}
	
	/// <summary>
	/// Initializes the primary camera and creates the secondary camera
	/// </summary>
	private void InitializeCameras()
	{
		// Clone the primary camera
		this.secondaryCamera = GameObject.Instantiate(this.primaryCamera) as Camera;
		this.secondaryCamera.transform.parent = this.transform;
		this.secondaryCamera.clearFlags = CameraClearFlags.Depth;
		this.secondaryCamera.cullingMask = -1;
		
		// Position primary camera to look at the main player
		// Note, it is not necessary to position the secondary camera here
		//        this.primaryCamera.transform.localRotation = this.cameraQuaternion;
		//        this.MoveCamera(this.primaryCamera, this.MainPlayer.position);
		
		// Specifically initialize splitscreen settings by turning it off for now
		this.StopSplitscreenCamera();
	}
	
	/// <summary>
	/// Move a camera to look at a specified position
	/// </summary>
	/// <param name="camera">The camera to move</param>
	/// <param name="targetPos">The position for that camera to look at</param>
	/// <remarks>This is the place to add specialized camera movement behavior if you want it.</remarks>
	private void MoveCamera(Camera camera, Vector3 targetPos)
	{
		camera.transform.localRotation = this.cameraQuaternion;
		camera.transform.position = targetPos - (camera.transform.forward * this.cameraDistance);
	}
	
	/// <summary>
	/// Positions the splitscreen mask to cover a player's half of the screen
	/// </summary>
	/// <param name="camera">The camera to position the mask in front of</param>
	/// <param name="playerPos">The position of the player to place the mask over</param>
	/// <param name="targetPos">A 3D position that should be considered along the line from the player toward of the center of the screen</param>
	private void PositionSplitscreenMask(Camera camera, Vector3 playerPos, Vector3 targetPos)
	{
		// Resize the mask to cover the proper amount of the screen. It just needs to be long enough to go past the
		// ends of a diagonal across the screen. Making it square makes for fewer calculations to get it big enough  
		if (camera.orthographic)
		{
			if (camera.aspect >= 1.0f)
			{
				this.maskScale.x = this.maskScale.y = camera.orthographicSize * 2.83f /* 2√2 */ * camera.aspect;
			}
			else
			{
				this.maskScale.x = this.maskScale.y = camera.orthographicSize * 2.83f /* 2√2 */;
			}
		}
		else
		{
			if (camera.aspect >= 1.0f)
			{
				this.maskScale.x = this.maskScale.y = 2.83f /* 2√2 */ * this.maskOffset * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * camera.aspect;
			}
			else
			{
				this.maskScale.x = this.maskScale.y = 2.83f /* 2√2 */ * this.maskOffset * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) / camera.aspect;
			}
		}
		
		this.maskTransform.localScale = this.maskScale;
		
		// Project the two points onto the camera's 2D view
		this.screenDisplacement2d = camera.WorldToScreenPoint(playerPos) - camera.WorldToScreenPoint(targetPos);
		
		// Align the splitscreen mask with the camera and rotate it based on the split angle
		this.maskTransform.rotation = camera.transform.rotation;
		this.maskTransform.Rotate(this.maskTransform.forward, Mathf.Atan2(this.screenDisplacement2d.y, this.screenDisplacement2d.x) * Mathf.Rad2Deg, Space.World);
		
		// Place the mask in front of the camera, far enough to the side to only conceal half of the screen
		this.maskTransform.position = camera.transform.position + (camera.transform.forward * this.maskOffset) + (this.maskTransform.right * this.splitscreenMask.transform.lossyScale.x * 0.5f);

	}
	
	/// <summary>
	/// Checks whether the players' distance from one another warrants using splitscreen and turns it on or off accordingly
	/// </summary>
	private void PerformSplitscreenCheck()
	{
//		if (!this.IsSplitscreenOn && this.distanceBetweenPlayers.sqrMagnitude > this.triggerDistance * this.triggerDistance)
//		{
//			this.StartSplitscreenCamera();
//		}
//		
//		if (this.IsSplitscreenOn && this.distanceBetweenPlayers.sqrMagnitude < this.triggerDistance * this.triggerDistance)
//		{
//			this.StopSplitscreenCamera();
//		}
	}
	
	/// <summary>
	/// Activate splitscreen
	/// </summary>
	public void StartSplitscreenCamera()
	{
		// Activate the splitscreen components
		this.secondaryCamera.gameObject.SetActive(true);
		this.maskTransform.gameObject.SetActive(true);
		
		// Position the new camera
		this.secondaryCamera.transform.position = this.primaryCamera.transform.position;
		this.secondaryCamera.transform.rotation = this.cameraQuaternion;
		
		// Turn off culling of the splitscreen mask layer for the main camera
//		this.primaryCamera.cullingMask &= ~(1 << this.maskLayer);
		
		this.IsSplitscreenOn = true;
	}
	
	/// <summary>
	/// Disable splitscreen
	/// </summary>
	public void StopSplitscreenCamera()
	{
		// Just turn everything off
		this.IsSplitscreenOn = false;
		this.secondaryCamera.gameObject.SetActive(false);
		this.maskTransform.gameObject.SetActive(false);
		this.separatorRenderer.enabled = false;
	}
	#endregion
}
