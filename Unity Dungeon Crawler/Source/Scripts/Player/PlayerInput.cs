using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
	public PlayerBehavior player;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetKey(KeyCode.UpArrow))
		{
			player.move(true);
		}
		if(Input.GetKey(KeyCode.DownArrow))
		{
			player.move(false);
		}
		if(Input.GetKey(KeyCode.RightArrow))
		{
			player.turn(true);
		}
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			player.turn(false);
		}
	}
}
