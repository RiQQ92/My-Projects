using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
	public PlayerShoot shootScript;
	public PlayerMovement movementScript;

	// Update is called once per frame
	void Update ()
	{
		if(Input.GetKey(KeyCode.Space))
		{
			if (shootScript != null)
			{
				shootScript.shoot();
			}
		}
		if (Input.GetMouseButtonDown (0))
		{
			movementScript.MouseClicked = true;
		}
		if (Input.GetMouseButtonUp (0))
		{
			movementScript.MouseClicked = false;
		}
	}
}
