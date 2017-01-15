using UnityEngine;
using System.Collections;

public class checkGap : MonoBehaviour
{
	//keeps info if colliding with ground
	bool checkCollision = false;

	void FixedUpdate()
	{
		// sets false every frame
		checkCollision = false;
	}

	// but if it has any collision action with tagged object 'baseGround' sets true
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
		// and if true and also the parentobjects script 'enemyAiSimple' variable isGrounded is true - Turns enemy with function turnEnemy();	
		if(checkCollision == false && GetComponentInParent<enemyAI_Simple>().isGrounded)
			GetComponentInParent<enemyAI_Simple>().turn();
	}
}
