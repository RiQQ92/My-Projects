using UnityEngine;
using System.Collections;

public class restrictMovement : MonoBehaviour
{

	// new save
	public float Top;
	public float Bottom;
	public float Left;
	public float Right;

	public bool manualBounds = false;

	private bool xRestricted = false;
	private bool yRestricted = false;
	private Vector2 halfSize;

	public bool xEdge
	{
		get
		{
			return xRestricted;
		}
	}
	public bool yEdge
	{
		get
		{
			return yRestricted;
		}
	}

	public void updateBorders()
	{
		if(GetComponent<BoxCollider2D>() != null)
		{
			BoxCollider2D box = GetComponent<BoxCollider2D>();
			halfSize = new Vector2(box.bounds.size.x/2, box.bounds.size.y/2);
		}
		else if(GetComponent<SpriteRenderer>() != null)
		{
			SpriteRenderer sprite = GetComponent<SpriteRenderer>();
			halfSize = new Vector2(sprite.bounds.size.x/2, sprite.bounds.size.y/2);
		}
		else
			halfSize = new Vector2(0,0);
	}

	void Start()
	{
		if(GetComponent<BoxCollider2D>() != null)
		{
			BoxCollider2D box = GetComponent<BoxCollider2D>();
			halfSize = new Vector2(box.bounds.size.x/2, box.bounds.size.y/2);
		}
		else if(GetComponent<SpriteRenderer>() != null)
		{
			SpriteRenderer sprite = GetComponent<SpriteRenderer>();
			halfSize = new Vector2(sprite.bounds.size.x/2, sprite.bounds.size.y/2);
		}
		else
			halfSize = new Vector2(0,0);

		if(!manualBounds)
		{
			Top = transform.Find("/Level").GetComponent<levelData>().Top;
			Bottom = transform.Find("/Level").GetComponent<levelData>().Bottom;
			Left = transform.Find("/Level").GetComponent<levelData>().Left;
			Right = transform.Find("/Level").GetComponent<levelData>().Right;
		}

		if(gameObject.tag == "Player")
		{
			Top = 100;
			Bottom = -100;
		}
	}

	// Update is called once per frame
	void LateUpdate ()
	{
		if(transform.position.x <= Left + halfSize.x)
		{
			if(rigidbody2D)
			{
				if(rigidbody2D.velocity.x != 0)
				{
					rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
					transform.position = new Vector3(Left + halfSize.x, transform.position.y, transform.position.z);
				}
				else
					if(transform.position.x < Left + halfSize.x)
						transform.position = new Vector3(Left + halfSize.x, transform.position.y, transform.position.z);
			}
			else
				transform.position = new Vector3(Left + halfSize.x, transform.position.y, transform.position.z);

			xRestricted = true;
		}
		else if(transform.position.x >= Right - halfSize.x)
		{
			if(rigidbody2D)
			{
				if(rigidbody2D.velocity.x != 0)
				{
					rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
					transform.position = new Vector3(Right - halfSize.x, transform.position.y, transform.position.z);
				}
				else
					if(transform.position.x < Right - halfSize.x)
						transform.position = new Vector3(Right - halfSize.x, transform.position.y, transform.position.z);
			}
			else
				transform.position = new Vector3(Right - halfSize.x, transform.position.y, transform.position.z);

			xRestricted = true;
		}
		else
			xRestricted = false;

		if(transform.position.y < Bottom + halfSize.y)
		{
			transform.position = new Vector3(transform.position.x, Bottom + halfSize.y, transform.position.z);
			yRestricted = true;
		}
		else if(transform.position.y > Top - halfSize.y)
		{
			transform.position = new Vector3(transform.position.x, Top - halfSize.y, transform.position.z);
			yRestricted = true;
		}
		else
			yRestricted = false;
	}
}
