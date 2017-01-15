using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public float smooth;

	public float turnFactor = 0.01f;
	public float maxAngle = 45;

	public float maxSpeed = 0.02f;
	private Animator anim;

    private Vector3 moveDir;
	public bool leftMouseClicked = false;
	public bool MouseClicked
	{
		get
		{
			return leftMouseClicked;
		}
		set
		{
			if (anim == null)
				anim = GetComponent<Animator> ();
			anim.SetBool ("focusFire", value);
			leftMouseClicked = value;
		}
	}

	private Vector3 originalRotation;
	private Rigidbody2D rb;

	void Start ()
    {
		if (anim == null)
			anim = GetComponent<Animator> ();
    moveDir = Vector3.zero;
    originalRotation = transform.rotation.eulerAngles;
		rb = GetComponent<Rigidbody2D>();
	}

	void Update ()
	{

	}

	void FixedUpdate()
	{
		if (!leftMouseClicked)
		{
			if(transform.rotation.eulerAngles.z != originalRotation.z)
			{
				if(transform.eulerAngles.z > 180)
					transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, 0, 359.9f), turnFactor);
				else
					transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, originalRotation, turnFactor);

				if(transform.eulerAngles.z >= 359.5f ||
				   transform.eulerAngles.z <= 0.5f)
				{
					transform.eulerAngles = originalRotation;
				}
			}

			Vector3 targetPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector3 newPos = Vector3.Lerp (rb.transform.position, targetPos, smooth);

			Vector3 angleToTarget =  newPos - new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, transform.position.z);
			float distanceToTarget = angleToTarget.magnitude;
			/*
			bool outOfBounds = false;
			float percent = 1f;
			if(newPos.x > maxSpeed && newPos.y > maxSpeed ||
			   newPos.x < -maxSpeed && newPos.y < -maxSpeed)
			{
				if(newPos.x > newPos.y)
				{
					percent = maxSpeed / newPos.x;
				}
				else
				{
					percent = maxSpeed / newPos.y;
				}

				outOfBounds = true;
			}
			else if(newPos.x > maxSpeed || newPos.x < -maxSpeed)
			{
				percent = maxSpeed / newPos.x;
				
				outOfBounds = true;
			}
			else if(newPos.y > maxSpeed || newPos.y < -maxSpeed)
			{
				percent = maxSpeed / newPos.y;
				
				outOfBounds = true;
			}

			if(outOfBounds)
			{
				newPos += transform.position;
				newPos *= percent;
			}
			*/
			moveDir = Vector3.Lerp(rb.transform.position, targetPos, smooth*7); // smooth breaking
			rb.MovePosition (newPos);
		}
		else
		{
            Vector3 newPos = Vector3.Lerp(rb.transform.position, moveDir, smooth*2); // smooth breaking
            rb.MovePosition(newPos);

			Vector3 targetPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector3 angleToTarget =  targetPos - new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, transform.position.z);
			angleToTarget.Normalize();
			float rot_z = Mathf.Atan2 (angleToTarget.y, angleToTarget.x) * Mathf.Rad2Deg;
            rot_z -= 90;
            
			if(transform.eulerAngles.z >= 0 &&
               transform.eulerAngles.z < maxAngle+10
               ||
			   transform.eulerAngles.z <= 360 &&
               transform.eulerAngles.z > 360 -maxAngle -10)
                  transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, rot_z), turnFactor);

            if (transform.eulerAngles.z > maxAngle && !(transform.eulerAngles.z > 360 - maxAngle - 20))
                transform.rotation = Quaternion.Euler(0f, 0f, maxAngle);
            else if (transform.eulerAngles.z < 360 - maxAngle && !(transform.eulerAngles.z < maxAngle + 20))
                transform.rotation = Quaternion.Euler(0f, 0f, -maxAngle);
        }
	}
}
