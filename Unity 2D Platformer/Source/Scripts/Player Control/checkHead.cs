using UnityEngine;
using System.Collections;

public class checkHead : MonoBehaviour
{
	private bool headInWall;
	private float height;
	private float width;

	void Start()
	{
		BoxCollider2D box = GetComponent<BoxCollider2D>();
		height = box.bounds.extents.y;
		width = box.bounds.extents.x-0.1f;
	}

	void FixedUpdate()
	{
		int triggerOrNot = 0;
		if(collider2D.isTrigger)
			triggerOrNot = -1;
		else
			triggerOrNot = 1;

		Vector2 pos = (Vector2)transform.position;

		foreach(Collider2D col in Physics2D.OverlapAreaAll(new Vector2(-width, triggerOrNot*height-0.1f) + pos, new Vector2(width, triggerOrNot*height+0.1f) + pos))
		{
			if(col.gameObject.tag == "baseGround")
			{
				if(col.gameObject.layer != LayerMask.NameToLayer("OneWayPlatform"))
				{
					GetComponentInParent<plrControl>().jumping = false;
					break;
				}
			}
		}

		headInWall = false;
	}

	// use this if this is set to trigger
	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.gameObject.tag == "baseGround")
		{
			headInWall = true;
		}
	}
	void OnTriggerStay2D(Collider2D col)
	{
		if(col.gameObject.tag == "baseGround")
		{
			headInWall = true;
		}
	}
	/*
	//if not trigger and this collides with physics, use this
	void OnCollisionEnter2D(Collision2D col)
	{
		GetComponentInParent<plrControl>().jumping = false;
	}
	*/

	void LateUpdate()
	{
		GetComponentInParent<plrControl>().headInWall = headInWall;
	}
}
