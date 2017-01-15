using UnityEngine;
using System.Collections;

public class enemyAI_Jumping : Enemy
{
	private int countFrames;

	void Start ()
	{
		destroyed = false;
		isGrounded = false;
		rigidbody2D.isKinematic = true;
		speed = 50;
		countFrames = 0;
		spawnPoint = transform.position;
	}

	void FixedUpdate ()
	{
		if(isActive && !destroyed)
		{
			if(direction)
				hdir = 1;
			else
				hdir = -1;

			if(transform.localScale.x < 0 && direction)
				transform.localScale = new Vector3(transform.localScale.x *-1, transform.localScale.y, transform.localScale.z);
			else if(transform.localScale.x > 0 && !direction)
				transform.localScale = new Vector3(transform.localScale.x *-1, transform.localScale.y, transform.localScale.z);
			
			if(isGrounded && !destroyed)
				move(new Vector2(hdir * speed * Time.deltaTime, 12.0f));
			else if(!destroyed)
				move(new Vector2(hdir * speed * Time.deltaTime, rigidbody2D.velocity.y));

			if(!publicStorage.localGame && Network.isServer && publicNetworkData.cooperative)
			{
				if(countFrames > 60)
				{
					networkView.RPC("updatePosNDirNVelo",RPCMode.Others, (Vector3)rigidbody2D.position, direction, (Vector3)rigidbody2D.velocity);
					countFrames = 0;
				}
				countFrames++;
			}
			
			if(timer > 15)
			{
				checkMyArea();
				timer = 0;
			}
			timer++;
			
			checkTeleport();
			//anim.SetBool("isDead", destroyed);
		}
		else if(!destroyed)
		{
			if(isAtSpawn)
			{
				if(timer > 15)
				{
					checkMyArea();
					timer = 0;
				}
				timer++;
			}
			else
			{
				if(timer > 15)
				{
					checkSpawnArea();
					timer = 0;
				}
				timer++;
			}
		}
	}
}
