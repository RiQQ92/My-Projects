using UnityEngine;
using System.Collections;

public class Enemy : Teleportable
{
	public bool isGrounded;
	public bool direction;

	protected bool destroyed;
	protected bool isActive = false;
	protected bool isAtSpawn = true;

	protected int timer = 0;

	protected float vdir;
	protected float hdir;
	protected float speed;

	protected Vector2 spawnPoint;

	private int xLength = 15;
	private int yLength = 8;

	public void setRdyToTeleport()
	{
		rdyToTeleport = true;
	}

	public void posAfterTeleporting()
	{
		networkView.RPC("updatePosNDirNVelo",RPCMode.Others, (Vector3)rigidbody2D.position, direction, (Vector3)rigidbody2D.velocity);
	}

	public void canTeleport(teleporterBehavior script)
	{
		script.enemy.cooledDown = cooledDown;
		script.enemy.rdyToTeleport = rdyToTeleport;
	}

	public void teleportingCooldown()
	{
		rdyToTeleport = false;
		startCooldown();
	}

	public void setIsGrounded()
	{
		isGrounded = true;
	}

	public void setNotGrounded()
	{
		isGrounded = false;
	}

	//destroys this object
	public void destroy()
	{
		destroyed = true;
		// destroy totally after 2 seconds
		Destroy (gameObject, 2);
		
		//instantly remove colliders
		Destroy(gameObject.rigidbody2D);
		Destroy(gameObject.collider2D);
		Destroy(transform.FindChild("stompZone").collider2D);
	}
	
	public void turn()
	{
		direction = !direction;
		transform.localScale = new Vector3(transform.localScale.x *-1, transform.localScale.y, transform.localScale.z);
	}

	protected void checkTeleport()
	{
		if(publicStorage.localGame || Network.isServer && publicNetworkData.cooperative || !publicNetworkData.cooperative)
		{
			if(!rdyToTeleport)
			{
				bool findTeleport = false;
				foreach(Collider2D col in Physics2D.OverlapCircleAll(rigidbody2D.position, 1f))
				{
					if(col.gameObject.tag == "teleport")
					{
						findTeleport = true;
					}
				}
				if(!findTeleport)
					rdyToTeleport = true;
			}
		}
	}

	protected void move(Vector2 movement)
	{
		rigidbody2D.velocity = movement;
	}

	protected void checkSpawnArea()
	{
		if(publicStorage.Splitscreen)
		{
			for(int i = 0; i < publicStorage.plrsInThisWorld; i++)
			{
				if(publicStorage.refToPlayers[i].position.x < spawnPoint.x + xLength*2 && publicStorage.refToPlayers[i].position.x > spawnPoint.x - xLength*2
				&& publicStorage.refToPlayers[i].position.y < spawnPoint.y + yLength && publicStorage.refToPlayers[i].position.y > spawnPoint.y - yLength)
				{
					// someone at spawn, cant move there!
				}
				else
				{
					transform.position = spawnPoint;
					isAtSpawn = true;
				}
			}
		}
		else
		{
			for(int i = 0; i < publicStorage.plrsInThisWorld; i++)
			{
				if(!publicStorage.localGame && !publicNetworkData.cooperative)
				{
					if(publicStorage.refToPlayers[i].networkView)
					{
						if(publicStorage.refToPlayers[i].networkView.isMine)
						{
							if(publicStorage.refToPlayers[i].position.x < spawnPoint.x + xLength && publicStorage.refToPlayers[i].position.x > spawnPoint.x - xLength
							&& publicStorage.refToPlayers[i].position.y < spawnPoint.y + yLength && publicStorage.refToPlayers[i].position.y > spawnPoint.y - yLength)
							{
								// someone at spawn, cant move there!
							}
							else
							{
								transform.position = spawnPoint;
								isAtSpawn = true;
							}
						}
					}
				}
				else
				{
					if(publicStorage.refToPlayers[i].position.x < spawnPoint.x + xLength && publicStorage.refToPlayers[i].position.x > spawnPoint.x - xLength
					&& publicStorage.refToPlayers[i].position.y < spawnPoint.y + yLength && publicStorage.refToPlayers[i].position.y > spawnPoint.y - yLength)
					{
						// someone at spawn, cant move there!
					}
					else
					{
						transform.position = spawnPoint;
						isAtSpawn = true;
					}
				}
			}
		}
	}

	protected void checkMyArea()
	{
		if(publicStorage.Splitscreen)
		{
			bool playerAtRange = false;
			for(int i = 0; i < publicStorage.plrsInThisWorld; i++)
			{
				if(!isActive)
				{
					if(publicStorage.refToPlayers[i].position.x < transform.position.x + xLength*2 && publicStorage.refToPlayers[i].position.x > transform.position.x - xLength*2
					&& publicStorage.refToPlayers[i].position.y < transform.position.y + yLength && publicStorage.refToPlayers[i].position.y > transform.position.y - yLength)
					{
						playerAtRange = true;
						activate();
						break;
					}
				}
				else
				{
					if(publicStorage.refToPlayers[i].position.x < transform.position.x + 5 + xLength*2 && publicStorage.refToPlayers[i].position.x > transform.position.x -5 - xLength*2
					&& publicStorage.refToPlayers[i].position.y < transform.position.y + 5 + yLength && publicStorage.refToPlayers[i].position.y > transform.position.y -5 - yLength)
					{
						playerAtRange = true;
						break;
					}
				}
			}

			if(!playerAtRange && isActive)
			{
				disable();
			}
		}
		else
		{
			bool playerAtRange = false;
			for(int i = 0; i < publicStorage.plrsInThisWorld; i++)
			{
				if(!publicStorage.localGame && !publicNetworkData.cooperative)
				{
					if(publicStorage.refToPlayers[i].networkView)
					{
						if(publicStorage.refToPlayers[i].networkView.isMine)
						{
							if(!isActive)
							{
								if(publicStorage.refToPlayers[i].position.x < transform.position.x + xLength && publicStorage.refToPlayers[i].position.x > transform.position.x - xLength
								&& publicStorage.refToPlayers[i].position.y < transform.position.y + yLength && publicStorage.refToPlayers[i].position.y > transform.position.y - yLength)
								{
									playerAtRange = true;
									activate();
									break;
								}
							}
							else
							{
								if(publicStorage.refToPlayers[i].position.x < transform.position.x + 5 + xLength && publicStorage.refToPlayers[i].position.x > transform.position.x -5 - xLength
								&& publicStorage.refToPlayers[i].position.y < transform.position.y + 5 + yLength && publicStorage.refToPlayers[i].position.y > transform.position.y -5 - yLength)
								{
									playerAtRange = true;
									break;
								}
							}
						}
					}
				}
				else
				{
					if(!isActive)
					{
						if(publicStorage.refToPlayers[i].position.x < transform.position.x + xLength && publicStorage.refToPlayers[i].position.x > transform.position.x - xLength
						&& publicStorage.refToPlayers[i].position.y < transform.position.y + yLength && publicStorage.refToPlayers[i].position.y > transform.position.y - yLength)
						{
							playerAtRange = true;
							activate();
							break;
						}
					}
					else
					{
						if(publicStorage.refToPlayers[i].position.x < transform.position.x + 5 + xLength && publicStorage.refToPlayers[i].position.x > transform.position.x -5 - xLength
						&& publicStorage.refToPlayers[i].position.y < transform.position.y + 5 + yLength && publicStorage.refToPlayers[i].position.y > transform.position.y -5 - yLength)
						{
							playerAtRange = true;
							break;
						}
					}
				}
			}
			
			if(!playerAtRange && isActive)
			{
				disable();
			}
		}
	}

	protected void activate()
	{
		isActive = true;
		isAtSpawn = false;
		rigidbody2D.isKinematic = false;
	}

	protected void disable()
	{
		isActive = false;
		transform.position = new Vector3(-10, -10, transform.position.z);
		rigidbody2D.isKinematic = true;
	}

	[RPC]
	protected void updatePosNDir(Vector3 pos, bool dir)
	{
		rigidbody2D.position = pos;
		direction = dir;
	}

	[RPC]
	protected void updatePosNDirNVelo(Vector3 pos, bool dir, Vector3 velo)
	{
		rigidbody2D.velocity = velo;
		rigidbody2D.position = pos;
		direction = dir;
	}

	[RPC]
	protected void netDestroy()
	{
		destroy ();
	}
}
