using UnityEngine;
using System.Collections;

public class hillOnFront : MonoBehaviour
{
	public bool checkCollision = false;

	public bool isColliding
	{
		get
		{
			return(checkCollision);
		}
	}

	void FixedUpdate()
	{
		checkCollision = false;
	}
	
	void OnTriggerEnter2D(Collider2D c)
	{
		if(c.gameObject.tag == "baseGround")
			checkCollision = true;
	}

	void OnTriggerStay2D(Collider2D c)
	{
		if(c.gameObject.tag == "baseGround")
			checkCollision = true;
	}
	
	void OnTriggerExit2D(Collider2D c)
	{
		if(c.gameObject.tag == "baseGround")
			checkCollision = true;
	}
	
	void LateUpdate()
	{
		// if this collides, if player is grounded
		if(checkCollision && GetComponentInParent<plrControl>().isGrounded)
		{
			// if parents child opposite side downhill check is not colliding -> we are obviously in hill
			if(transform.parent.FindChild("downhillCheckRear").gameObject.GetComponent<collisionChecker>().isColliding == false)
			{GetComponentInParent<plrControl>().onHillFront = true;}
			else
			{GetComponentInParent<plrControl>().onHillFront = false;}
		}
		else
			GetComponentInParent<plrControl>().onHillFront = false;
	}
}
