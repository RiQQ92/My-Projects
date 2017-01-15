using UnityEngine;
using System.Collections;

public class pwupMushroom : Powerup
{
	private Vector3 movement;
	private int dir;
	private int countFrames;
	private int upsSent;
	// Use this for initialization
	void Start ()
	{
		spawnPoint = transform.localPosition;
		movement = new Vector3(0,0,0);
		dir = 1;
		countFrames = 0;
		upsSent = 0;
		gameObject.collider2D.enabled = false;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if(!hasSpawned)
		{
			spawn();

			if(direction && transform.localScale.x < 0)
				transform.localScale = new Vector3(transform.localScale.x *-1, transform.localScale.y, transform.localScale.z);
			else if(!direction && transform.localScale.x > 0)
				transform.localScale = new Vector3(transform.localScale.x *-1, transform.localScale.y, transform.localScale.z);
		}
		else
		{
			if(direction && transform.localScale.x < 0)
				transform.localScale = new Vector3(transform.localScale.x *-1, transform.localScale.y, transform.localScale.z);
			else if(!direction && transform.localScale.x > 0)
				transform.localScale = new Vector3(transform.localScale.x *-1, transform.localScale.y, transform.localScale.z);

			if(direction)
				dir = 1;
			else
				dir = -1;

			movement = new Vector2(speed * dir * Time.deltaTime, gameObject.rigidbody2D.velocity.y);
			move(movement);

			if(!publicStorage.localGame && Network.isServer && publicNetworkData.cooperative)
			{
				if(upsSent < 18 && countFrames > 10)
				{
					networkView.RPC("updatePosNDir",RPCMode.Others, (Vector3)rigidbody2D.position, direction);
					countFrames = 0;
					upsSent++;
				}
				else if(countFrames > 60)
				{
					networkView.RPC("updatePosNDir",RPCMode.Others, (Vector3)rigidbody2D.position, direction);
					countFrames = 0;
				}
				countFrames++;
			}
		}
	}
}
