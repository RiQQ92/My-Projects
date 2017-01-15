using UnityEngine;
using System.Collections;

public class EnemyTurnAI : MonoBehaviour {

	private Health health;
	private EnemyAttackAI attackAI;
	private EnemyMoveToTarget movementAI;
    public bool moveFinished;

	void Start()
	{
		health = gameObject.GetComponent<Health>();
		attackAI = gameObject.GetComponent<EnemyAttackAI>();
		movementAI = gameObject.GetComponent<EnemyMoveToTarget>();
        moveFinished = false;
    }

    void Update()
    {
        if(moveFinished)
        {
            moveFinished = false;
            gameObject.GetComponent<Turn>().EndMyTurn();
        }
    }

	public IEnumerator AIaction()
	{
		if(!gameObject.GetComponent<EnemyMoveToTarget>().MoveStarted)
		{
			if (!health.isDead)
			{
				if(movementAI.HasTarget)
				{
					if (attackAI.AttemptAttack())
					{
						moveFinished = true;
						yield return new WaitForSeconds(0.5f);
					}
					else // target not in range, move instead
					{
	                    movementAI.move();
					}
				}
				else // non-combat behavior goes here
	            {
	                movementAI.move();
	            }
			}
			else
			{
				Debug.Log("DÖD");
			}
		}
		yield return null;
	}
}