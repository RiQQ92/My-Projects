using UnityEngine;
using System.Collections;

public class Turn : MonoBehaviour {

	[HideInInspector]
	public bool myTurn;
	private TurnManager turnManager;

	public bool isInCombat;

	void Start()
	{
		GameObject manager = GameObject.FindGameObjectWithTag("Turn Manager");
		turnManager = manager.GetComponent<TurnManager>();
	}

	public void StartMyTurn()
	{
		if (gameObject.CompareTag("Enemy"))
		{
			StartCoroutine(GetComponent<EnemyTurnAI>().AIaction());
		}
		else
		{
			myTurn = true;
		}
	}

	public void EndMyTurn()
	{
		myTurn = false;
		turnManager.NextTurn();
	}
}
