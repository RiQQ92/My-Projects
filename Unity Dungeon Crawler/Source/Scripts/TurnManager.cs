using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour {

	public int currentTurn;
    public Turn playerTurn;

    public List<Turn> lista;
    private GameObject[] enemies;

	void Start ()
	{
		playerTurn = GameObject.FindGameObjectWithTag ("Player").GetComponent<Turn> ();
		currentTurn = 0;
		StartTurn(currentTurn);
	}

	public void StartTurn(int turnNumber)
	{
		GetEnemyList();
		lista[turnNumber].StartMyTurn();
	}

	public void NextTurn()
	{
        do
        {
            currentTurn++;
            if (currentTurn >= lista.Count)
            {
				Debug.Log("------------------ Turn Ended -------------------");
                currentTurn = 0;
            }
        }
        while (lista[currentTurn].isInCombat == false);
		GetEnemyList();
		lista[currentTurn].StartMyTurn();
	}

    private void GetEnemyList()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        lista.Clear();
        lista.Add(playerTurn);
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].GetComponent<Turn>().isInCombat == true)
            {
				lista.Add(enemies[i].GetComponent<Turn>());
            }
        }
    }
}
