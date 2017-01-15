using UnityEngine;
using System.Collections;

public class catchPunch : MonoBehaviour
{
	private bool plrCollision = false;

	private IEnumerator wait()
	{
		yield return new WaitForSeconds(0.1f);
		plrCollision = false;
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if(col.gameObject.tag != "baseGround" && !plrCollision)
		{
			if(publicStorage.localGame || !publicNetworkData.cooperative)
				transform.parent.FindChild("lvl_Brick").GetComponent<lvlObjController>().gotPunch(col.collider.transform.position);
			else
				if(col.gameObject.tag != "Player" && !plrCollision)
					transform.parent.FindChild("lvl_Brick").GetComponent<lvlObjController>().gotPunch(col.collider.transform.position);
		}

		if(col.gameObject.tag == "Player" && !plrCollision)
		{
			if(publicStorage.localGame)
			{
				transform.parent.FindChild("lvl_Brick").GetComponent<lvlObjController>().destroy(col.gameObject);
				StartCoroutine(wait());
				plrCollision = true;
			}
			else
			{
				if(!publicNetworkData.cooperative)
				{
					if(col.gameObject.GetComponent<NetworkView>().isMine)
					{
						transform.parent.FindChild("lvl_Brick").GetComponent<lvlObjController>().destroy(col.gameObject);
						StartCoroutine(wait());
						plrCollision = true;
					}
				}
				else
				{
					if(col.gameObject.GetComponent<NetworkView>().isMine)
					{
						transform.parent.FindChild("lvl_Brick").GetComponent<lvlObjController>().sendRPC(col.gameObject.GetComponent<plrControl>().isGrownup, col.collider.transform.position);
						transform.parent.FindChild("lvl_Brick").GetComponent<lvlObjController>().gotPunch(col.collider.transform.position);
						transform.parent.FindChild("lvl_Brick").GetComponent<lvlObjController>().destroy(col.gameObject);
						StartCoroutine(wait());
						plrCollision = true;
					}
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.gameObject.tag == "Player" && !plrCollision)
		{
			if(publicStorage.localGame || !publicNetworkData.cooperative)
			{
				if(col.gameObject.name == "HeadCollider")
				{
					if(!col.gameObject.GetComponent<BoxCollider2D>().isTrigger)
					{
						if(col.transform.parent.gameObject.rigidbody2D.velocity.y > 0.01)
						{
							transform.parent.FindChild("lvl_Brick").GetComponent<lvlObjController>().gotPunch(col.transform.position);
							transform.parent.FindChild("lvl_Brick").GetComponent<lvlObjController>().destroy(col.gameObject);
							StartCoroutine(wait());
							plrCollision = true;
							Vector2 plrSpd = col.transform.parent.gameObject.rigidbody2D.velocity;
							col.transform.parent.gameObject.rigidbody2D.velocity = new Vector2(plrSpd.x, 0);
						}
					}
				}
				else
				{
					if(col.gameObject.rigidbody2D.velocity.y > 0.01)
					{
						transform.parent.FindChild("lvl_Brick").GetComponent<lvlObjController>().gotPunch(col.transform.position);
						transform.parent.FindChild("lvl_Brick").GetComponent<lvlObjController>().destroy(col.gameObject);
						StartCoroutine(wait());
						plrCollision = true;
						Vector2 plrSpd = col.gameObject.rigidbody2D.velocity;
						col.gameObject.rigidbody2D.velocity = new Vector2(plrSpd.x, 0);
					}
				}
			}
			else
			{
				if(col.gameObject.name == "HeadCollider")
				{
					if(!col.gameObject.GetComponent<BoxCollider2D>().isTrigger)
					{
						if(col.transform.parent.gameObject.GetComponent<NetworkView>().isMine)
						{
							if(col.transform.parent.gameObject.rigidbody2D.velocity.y > 0.01)
							{
								transform.parent.FindChild("lvl_Brick").GetComponent<lvlObjController>().sendRPC(col.transform.parent.gameObject.GetComponent<plrControl>().isGrownup, col.transform.position);
								transform.parent.FindChild("lvl_Brick").GetComponent<lvlObjController>().gotPunch(col.transform.position);
								transform.parent.FindChild("lvl_Brick").GetComponent<lvlObjController>().destroy(col.gameObject);
								StartCoroutine(wait());
								plrCollision = true;
								Vector2 plrSpd = col.transform.parent.gameObject.rigidbody2D.velocity;
								col.transform.parent.gameObject.rigidbody2D.velocity = new Vector2(plrSpd.x, 0);
							}
						}
					}
				}
				else
				{
					if(col.gameObject.GetComponent<NetworkView>().isMine)
					{
						if(col.gameObject.rigidbody2D.velocity.y > 0.01)
						{
							transform.parent.FindChild("lvl_Brick").GetComponent<lvlObjController>().sendRPC(col.gameObject.GetComponent<plrControl>().isGrownup, col.transform.position);
							transform.parent.FindChild("lvl_Brick").GetComponent<lvlObjController>().gotPunch(col.transform.position);
							transform.parent.FindChild("lvl_Brick").GetComponent<lvlObjController>().destroy(col.gameObject);
							StartCoroutine(wait());
							plrCollision = true;
							Vector2 plrSpd = col.gameObject.rigidbody2D.velocity;
							col.gameObject.rigidbody2D.velocity = new Vector2(plrSpd.x, 0);
						}
					}
				}
			}
		}
	}
}
