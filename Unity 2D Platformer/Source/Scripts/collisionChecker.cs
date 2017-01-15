using UnityEngine;
using System.Collections;

// will be edited at one point to be reusable
public class collisionChecker : MonoBehaviour
{
	// checkCollision per frame
	private bool checkCollision = false;
	// public variable to tell the caller if this is colliding or not
	public bool isColliding = false;
	public bool ignoreOneWayPlatforms = false;
	
	void FixedUpdate()
	{
		// set false on every frame
		checkCollision = false;
	}

	// but if it has any collision action with tagged object 'baseGround' sets true
	void OnTriggerEnter2D(Collider2D c)
	{
		if(c.gameObject.tag == "baseGround")
			if(!ignoreOneWayPlatforms || ignoreOneWayPlatforms && c.gameObject.layer != LayerMask.NameToLayer("OneWayPlatform"))
				checkCollision = true;
	}
	
	void OnTriggerStay2D(Collider2D c)
	{
		if(c.gameObject.tag == "baseGround")
			if(!ignoreOneWayPlatforms || ignoreOneWayPlatforms && c.gameObject.layer != LayerMask.NameToLayer("OneWayPlatform"))
				checkCollision = true;
	}
	
	void OnTriggerExit2D(Collider2D c)
	{
		if(c.gameObject.tag == "baseGround")
			if(!ignoreOneWayPlatforms || ignoreOneWayPlatforms && c.gameObject.layer != LayerMask.NameToLayer("OneWayPlatform"))
				checkCollision = true;
	}
	
	void LateUpdate()
	{
		// and if true and also the parentobjects script 'plrControl' variable isGrounded is true - set isColliding true
		if(checkCollision == true && GetComponentInParent<plrControl>().isGrounded)
			isColliding = checkCollision;
		else
			isColliding = false;
	}
}
