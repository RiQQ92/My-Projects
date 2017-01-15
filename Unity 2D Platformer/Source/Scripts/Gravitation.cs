using UnityEngine;
using System.Collections;

// gravity class
public class Gravitation : Teleportable // all objects which apply gravity are teleportable
{
	protected bool gravityEnabled = true;
	protected float gravityFactor = 1;
	protected Vector2 gravityDirection = -Vector2.up;

	private float gravityStrength = 9.81f;

	// pulls game object towards gravity direction
	protected void gravityPull()
	{
		if(gravityEnabled)
		{
			Vector2 gravitate = gravityDirection * gravityStrength * gravityFactor * Time.deltaTime;
			gameObject.rigidbody2D.velocity += gravitate;
		}
	}

	protected Vector2 gravityPull(Vector2 target)
	{
		if(gravityEnabled)
		{
			Vector2 gravitate = gravityDirection * gravityStrength * gravityFactor * Time.deltaTime;
			target += gravitate;
		}
		return(target);
	}
}
