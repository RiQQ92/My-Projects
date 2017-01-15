#pragma strict

public var target : Transform;
public var ScriptName : String;
//public var Script : MonoScript;

/*
function Awake()
{
	Script = MonoBehaviour(target.gameObject.GetComponent(ScriptName));
}

function OnTriggerEnter2D(col : Collider2D)
{
	if(col.gameObject.tag == "baseGround" && transform.parent.gameObject.rigidbody2D.velocity.y <= 0.1)
	{
		if(!Script)
			Script.isGrounded = true;
	}
}
function OnTriggerStay2D(col : Collider2D)
{
	if(col.gameObject.tag == "baseGround" && transform.parent.gameObject.rigidbody2D.velocity.y <= 0.1)
	{
		if(!Script)
			Script.isGrounded = true;
	}
}
function OnTriggerExit2D(col : Collider2D)
{
	if(col.gameObject.tag == "baseGround")
	{
		if(!Script)
			Script.isGrounded = false;
	}
}
*/