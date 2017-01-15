using UnityEngine;
using System.Collections;

public class wallCheck : MonoBehaviour
{
	public Transform target;
	public bool ignoreOneWayPlatforms = true;
	private bool waitTurn = false;
	
	private IEnumerator wait()
	{
		yield return new WaitForSeconds(0.05f);
		//turns if finds collision
		target.gameObject.SendMessage("turn");
		waitTurn = false;
	}

	
	void OnTriggerEnter2D(Collider2D col)
	{
		// checks collision with base ground
		if(col.gameObject.tag == "baseGround" && !waitTurn || col.gameObject.tag == "powerup" && !waitTurn || col.gameObject.tag == "enemy" && !waitTurn)
		{
			if(!ignoreOneWayPlatforms || ignoreOneWayPlatforms && col.gameObject.layer != LayerMask.NameToLayer("OneWayPlatform"))
			{
				StartCoroutine(wait());
				waitTurn = true;
			}
		}
	}
	void OnTriggerStay2D(Collider2D col)
	{
		// checks collision with base ground
		if(col.gameObject.tag == "baseGround" && !waitTurn || col.gameObject.tag == "powerup" && !waitTurn || col.gameObject.tag == "enemy" && !waitTurn)
		{
			if(!ignoreOneWayPlatforms || ignoreOneWayPlatforms && col.gameObject.layer != LayerMask.NameToLayer("OneWayPlatform"))
			{
				StartCoroutine(wait());
				waitTurn = true;
			}
		}
	}
}
