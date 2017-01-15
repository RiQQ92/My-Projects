using UnityEngine;
using System.Collections;

public class checkStomp : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D col)
	{
		//collided object tag is 'enemyStomp'
		if(col.gameObject.tag == "enemyStomp")
		{
			if(publicStorage.localGame || !publicNetworkData.cooperative)
			{
				// set bounce height according to if button pressed
				if(GetComponentInParent<plrControl>().jumpBtnIsPressed)
					transform.parent.gameObject.rigidbody2D.velocity = new Vector2(transform.parent.gameObject.rigidbody2D.velocity.x, 25.0f);
				else
					transform.parent.gameObject.rigidbody2D.velocity = new Vector2(transform.parent.gameObject.rigidbody2D.velocity.x, 10.0f);

				// starts destruction of enemy, calls function in collidedobjects component(which is in this case script) destroy(); function
				if(publicStorage.localGame || transform.parent.networkView.isMine)
					col.transform.parent.gameObject.SendMessage("destroy");
			}
			else
			{
				// set bounce height according to if button pressed
				if(GetComponentInParent<plrControl>().jumpBtnIsPressed)
					transform.parent.gameObject.rigidbody2D.velocity = new Vector2(transform.parent.gameObject.rigidbody2D.velocity.x, 25.0f);
				else
					transform.parent.gameObject.rigidbody2D.velocity = new Vector2(transform.parent.gameObject.rigidbody2D.velocity.x, 10.0f);
				
				// starts destruction of enemy, calls function in collidedobjects component(which is in this case script) destroy(); function
				col.transform.parent.networkView.RPC("netDestroy", RPCMode.Others);
				col.transform.parent.gameObject.SendMessage("destroy");
			}
		}
		//collided object tag is 'Player'
		if(col.gameObject.tag == "Player")
		{
			//if the object is headcollider and is not trigger
			if( col.gameObject.name == "HeadCollider" && !col.gameObject.collider2D.isTrigger)
			{
				Debug.Log(col.gameObject.name);
				// set bounce height according to if button pressed
				if(GetComponentInParent<plrControl>().jumpBtnIsPressed)
					transform.parent.gameObject.rigidbody2D.velocity = new Vector2(transform.parent.gameObject.rigidbody2D.velocity.x, 21.0f);
				else
					transform.parent.gameObject.rigidbody2D.velocity = new Vector2(transform.parent.gameObject.rigidbody2D.velocity.x, 10.0f);

				//set the yScale of collided object to half
				col.transform.parent.localScale = new Vector3(col.transform.parent.localScale.x, 0.5f, col.transform.parent.localScale.z);
				//absorb half of the collided objects vertical speed
				transform.parent.gameObject.rigidbody2D.velocity = new Vector2(transform.parent.gameObject.rigidbody2D.velocity.x, transform.parent.gameObject.rigidbody2D.velocity.y +(col.transform.parent.gameObject.rigidbody2D.velocity.y /2));
			}
			// else if its not headcollider but it still is Player tagged
			else if(col.gameObject.name != "HeadCollider" && col.gameObject.tag == "Player")
			{
				// set bounce height according to if button pressed
				if(GetComponentInParent<plrControl>().jumpBtnIsPressed)
					transform.parent.gameObject.rigidbody2D.velocity = new Vector2(transform.parent.gameObject.rigidbody2D.velocity.x, 21.0f);
				else
					transform.parent.gameObject.rigidbody2D.velocity = new Vector2(transform.parent.gameObject.rigidbody2D.velocity.x, 10.0f);

				//set the yScale of collided object to half
				col.transform.localScale = new Vector3(col.transform.localScale.x, 0.75f, col.transform.localScale.z);
				//absorb half of the collided objects vertical speed
				transform.parent.gameObject.rigidbody2D.velocity = new Vector2(transform.parent.gameObject.rigidbody2D.velocity.x, transform.parent.gameObject.rigidbody2D.velocity.y +(col.gameObject.rigidbody2D.velocity.y /2));
			}

			// checks if it is in fact the headcollider children in player or the real collider in the player?
			if(col.gameObject.name != "HeadCollider")
				col.gameObject.rigidbody2D.velocity = new Vector2(col.gameObject.rigidbody2D.velocity.x, (col.gameObject.rigidbody2D.velocity.y /2) -2.5f);
			else
				col.transform.parent.gameObject.rigidbody2D.velocity = new Vector2(col.transform.parent.gameObject.rigidbody2D.velocity.x, (col.transform.parent.gameObject.rigidbody2D.velocity.y /2) -2.5f);


		}

		if(col.gameObject.tag == "movingPlatform")
		{
			float xDist = col.transform.parent.gameObject.GetComponent<moveBackNForth>().moveDistanceX;
			float yDist = col.transform.parent.gameObject.GetComponent<moveBackNForth>().moveDistanceY;
			float speed = col.transform.parent.gameObject.GetComponent<moveBackNForth>().moveSpeed;
			bool reversal = col.transform.parent.gameObject.GetComponent<moveBackNForth>().returning;

			if(reversal)
				transform.parent.GetComponent<plrControl>().groundMovement = new Vector2((-xDist/10) *speed, (-yDist/10) *speed);
			else
				transform.parent.GetComponent<plrControl>().groundMovement = new Vector2((xDist/10) *speed, (yDist/10) *speed);
			
			transform.parent.GetComponent<plrControl>().onMovingPlatform = true;
		}
	}

	void OnTriggerStay2D(Collider2D col)
	{
		if(col.gameObject.tag == "movingPlatform")
		{
			float xDist = col.transform.parent.gameObject.GetComponent<moveBackNForth>().moveDistanceX;
			float yDist = col.transform.parent.gameObject.GetComponent<moveBackNForth>().moveDistanceY;
			float speed = col.transform.parent.gameObject.GetComponent<moveBackNForth>().moveSpeed;
			bool reversal = col.transform.parent.gameObject.GetComponent<moveBackNForth>().returning;
			
			if(reversal)
				transform.parent.GetComponent<plrControl>().groundMovement = new Vector2((-xDist/10) *speed, (-yDist/10) *speed);
			else
				transform.parent.GetComponent<plrControl>().groundMovement = new Vector2((xDist/10) *speed, (yDist/10) *speed);
			
			transform.parent.GetComponent<plrControl>().onMovingPlatform = true;
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if(col.gameObject.tag == "movingPlatform")
		{
			float xDist = col.transform.parent.gameObject.GetComponent<moveBackNForth>().moveDistanceX;
			float yDist = col.transform.parent.gameObject.GetComponent<moveBackNForth>().moveDistanceY;
			float speed = col.transform.parent.gameObject.GetComponent<moveBackNForth>().moveSpeed;
			bool reversal = col.transform.parent.gameObject.GetComponent<moveBackNForth>().returning;
			
			if(reversal)
				transform.parent.GetComponent<plrControl>().groundMovement = new Vector2((-xDist/10) *speed, (-yDist/10) *speed);
			else
				transform.parent.GetComponent<plrControl>().groundMovement = new Vector2((xDist/10) *speed, (yDist/10) *speed);

			transform.parent.GetComponent<plrControl>().onMovingPlatform = true;
		}
	}
}
