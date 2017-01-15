using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemyMoveToTarget : MonoBehaviour
{
    public Grid grid;
    public PathFinder pathFinder;

    public float moveSpeed = 1.5f;

    private Vector2 FALSE_POSITION = new Vector2(-1, -1); // fake constant

    private bool hasTarget;
    private bool moveStarted;
    private bool moveFinished;
    private bool isMoving;
    private bool targetMissing;
    private bool isPathGenereated;
	private bool isPlayerSeen;
	private GameObject target;

	private Vector2 lastSeen;
    private Vector2 enemyPos;
    private Vector2 spawnPoint;

    private List<Vector2> pathToTarget;
	
	public bool HasTarget
	{
		get{return hasTarget;}
	}
	public bool MoveStarted
	{
		get{return moveStarted;}
	}

	// Use this for initialization
	void Start ()
	{
		if(grid == null)
			grid = GameObject.FindGameObjectWithTag("Level").GetComponent<Grid>() as Grid;

		if(pathFinder == null)
			pathFinder = GameObject.FindGameObjectWithTag("Level").GetComponent<PathFinder>() as PathFinder;

        moveStarted = false;
        moveFinished = false;
        isMoving = false;
        isPathGenereated = false;
        hasTarget = false;
		isPlayerSeen = false;
		targetMissing = true;

		target = GameObject.FindGameObjectWithTag("Player");
		lastSeen = FALSE_POSITION;
		pathToTarget = null;

        enemyPos = new Vector2((Mathf.Abs(grid.gridOffsetZ) + (int)transform.position.z) / grid.GRID_SIZE, (Mathf.Abs(grid.gridOffsetX) + (int)transform.position.x) / grid.GRID_SIZE);
        spawnPoint = new Vector2(enemyPos.x, enemyPos.y);
    }

    // Update is called once per frame
    void Update ()
	{
		if (isPathGenereated)
			if(pathToTarget.Count <= 0)
				isPathGenereated = false;

		if (!moveFinished && moveStarted && !isMoving)
        {
            if (isPathGenereated)
            {
                if (hasTarget)
				{
					StartCoroutine("MoveTo", pathToTarget[0]);
					pathToTarget.RemoveAt(0);
                }
                else
                {
                    if (pathToTarget.Count > 0 && pathToTarget != null)
                    {
                        // Debug.Log("target position: " + pathToTarget[0].ToString() + " ");
                        StartCoroutine("MoveTo", pathToTarget[0]);
                        pathToTarget.RemoveAt(0);
                    }
                    else
                    {
                        moveFinished = true;
                    }
                }
            }
        }
        else if(moveFinished && moveStarted)
        {
            moveFinished = false;
            moveStarted = false;
            gameObject.GetComponent<EnemyTurnAI>().moveFinished = true;
        }
	}

    public void idleMove()
    {
        //Debug.Log("Idle behavior run");

		if (!isPathGenereated || pathToTarget == null || pathToTarget.Count == 0)
        {
            if (enemyPos != spawnPoint)
            {
                //Debug.Log("target position: ");
				StartCoroutine( "generatePath", spawnPoint);
            }
            else
            {
                moveFinished = true;
            }
        }
    }

    public void move()
    {
        moveStarted = true;
        //Debug.Log("Enemy move");

        Vector3 headingToTarget =  target.transform.position - gameObject.transform.position;
        float distanceToTarget = headingToTarget.magnitude;

        Ray ray = new Ray(gameObject.transform.position, headingToTarget);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanceToTarget))
        {
			isPlayerSeen = false;
            // check if target is still on vision, 
            if (hit.collider.CompareTag("Player"))
            {
				isPlayerSeen = true;
                //Debug.Log("Player on sight!");
				Vector2 targetPos = new Vector2((Mathf.Abs(grid.gridOffsetZ) + (int)target.transform.localPosition.z) / grid.GRID_SIZE, (Mathf.Abs(grid.gridOffsetX) + (int)target.transform.localPosition.x) / grid.GRID_SIZE);

                // if we dont have target, get path to it and set target
                if (!hasTarget || pathToTarget == null || pathToTarget.Count == 0)
				{
					//Debug.Log(enemyPos);
					//Debug.Log(targetPos);
					StartCoroutine("generatePath", targetPos);
					hasTarget = true;
                }
                else // else check whether target has moved from its last location
                {
                    if (pathToTarget[pathToTarget.Count - 1] != targetPos)
                        pathToTarget.Add(targetPos);

					lastSeen = pathToTarget[pathToTarget.Count-1];
                }
            }
            else
            {
                //Debug.Log("Player not visible");

                // else check if we have target and we have seen it
                if (hasTarget && lastSeen != FALSE_POSITION)
				{
					Debug.Log("last seen: "+lastSeen.ToString()+" enemy pos: "+enemyPos.ToString());
                    // then check if we are already on targets last seen location
                    if (enemyPos == lastSeen)
                    {
                        // if yes, note that target lost and start returnal to original position
                        Debug.Log("target lost!");
                        lastSeen = FALSE_POSITION;
						hasTarget = false;
						idleMove();
                    }
                    // if not, move towards last seen position
                    /*else
                    {
						StartCoroutine("MoveTo", pathToTarget[0]);
						pathToTarget.RemoveAt(0);
                    }*/
                }
                else // move towards spawn point
                {
                    idleMove();
                }
            }
        }
    }


    IEnumerator MoveTo(Vector2 trgtLocation)
    {
        isMoving = true;
        Vector3 targetLocation = new Vector3((trgtLocation.y*grid.GRID_SIZE) -Mathf.Abs(grid.gridOffsetX), transform.position.y, (trgtLocation.x*grid.GRID_SIZE) -Mathf.Abs(grid.gridOffsetZ));
        Vector3 origin = transform.localPosition;
        float t = 0;
		if (isPlayerSeen)
			while (Mathf.RoundToInt(t*100) != Mathf.RoundToInt(moveSpeed*100))
			{
				t += moveSpeed / 60;
				transform.localPosition = Vector3.Lerp (origin, targetLocation, t);
				yield return new WaitForFixedUpdate ();
			}
		else
			transform.localPosition = targetLocation;
        
        transform.localPosition = new Vector3(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y), Mathf.RoundToInt(transform.localPosition.z));
        enemyPos = new Vector2((Mathf.Abs(grid.gridOffsetZ) + (int)transform.localPosition.z) / grid.GRID_SIZE, (Mathf.Abs(grid.gridOffsetX) + (int)transform.localPosition.x) / grid.GRID_SIZE);

        moveFinished = true;
        isMoving = false;
        //Debug.Log("enemy pos updated! new pos: "+enemyPos.ToString());
	}

    IEnumerator generatePath(Vector2 to)
    {
		Vector2 from = enemyPos;
        pathToTarget = pathFinder.findPath(from, to);
		
		if(pathToTarget.Count > 2)
			pathToTarget.RemoveAt(0);
		if(pathToTarget.Count > 0)
			lastSeen = pathToTarget[pathToTarget.Count-1];

        isPathGenereated = true;

        yield return new WaitForEndOfFrame();
    }
}