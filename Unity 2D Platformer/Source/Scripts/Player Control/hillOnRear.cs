using UnityEngine;
using System.Collections;

// for commentation, see hillOnFront.cs
public class hillOnRear : MonoBehaviour
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
		if(checkCollision && GetComponentInParent<plrControl>().isGrounded)
		{
			if(transform.parent.FindChild("downhillCheckFront").gameObject.GetComponent<collisionChecker>().isColliding == false)
			{GetComponentInParent<plrControl>().onHillRear = true;}
			else
			{GetComponentInParent<plrControl>().onHillRear = false;}
		}
		else
			GetComponentInParent<plrControl>().onHillRear = false;
	}
}
