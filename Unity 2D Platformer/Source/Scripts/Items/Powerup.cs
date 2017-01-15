using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour
{
	public bool direction = true;

	protected float speed = 150;
	protected bool hasSpawned = false;
	protected Vector3 spawnPoint = new Vector3(0,0,0);

	public void turn()
	{
		if(hasSpawned)
		{
			direction = !direction;
			transform.localScale = new Vector3(transform.localScale.x *-1, transform.localScale.y, transform.localScale.z);
		}
	}

	protected void spawn()
	{
		if(!hasSpawned)
		{
			transform.Translate(new Vector3(0,Time.deltaTime,0));

			if(transform.localPosition.y > spawnPoint.y+1)
			{
				if(!publicStorage.localGame && Network.isServer && publicNetworkData.cooperative)
					networkView.RPC("updatePosNDir",RPCMode.Others, (Vector3)rigidbody2D.position, direction);

				gameObject.rigidbody2D.isKinematic = false;
				gameObject.collider2D.enabled = true;
				hasSpawned = true;
			}
		}
	}

	protected void move(Vector2 movement)
	{
		gameObject.rigidbody2D.velocity = movement;
		if(gameObject.rigidbody2D.velocity.y < -12f)
			gameObject.rigidbody2D.velocity = new Vector2(movement.x, -12f);
	}

	[RPC]
	protected void updatePosNDir(Vector3 pos, bool dir)
	{
		rigidbody2D.position = pos;
		direction = dir;
	}

	[RPC]
	protected void netDestroy()
	{
		Destroy(gameObject);
	}
}
