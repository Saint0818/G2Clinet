// <copyright file="CubeController.cs" company="Armoire Cannonball">
// Copyright Armoire Cannonball, LLC (c) 2014. All Rights Reserved.
// </copyright>
// <author>Adam Ellis</author>
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CubeController : MonoBehaviour 
{
	public string horizontalAxis;
	public string verticalAxis;
	public string jumpButton;

	public float moveSpeed = 20;
	public float jumpSpeed = 15;

	private Vector3 moveDirection;

	private CharacterController characterController;

	void Awake ()
	{
		characterController = GetComponent<CharacterController> ();
	}

	void FixedUpdate () 
	{
		if (characterController.isGrounded) 
		{
			moveDirection.x = Input.GetAxisRaw(horizontalAxis);
			moveDirection.y = 0.0f;
			moveDirection.z = Input.GetAxisRaw(verticalAxis);
			moveDirection = moveDirection.normalized * moveSpeed;
		
			if (Input.GetButton(jumpButton))
			{
				moveDirection.y = jumpSpeed;
			}
		}

		moveDirection.y += Physics.gravity.y * Time.deltaTime;

		characterController.Move (moveDirection * Time.deltaTime);
	}
}
