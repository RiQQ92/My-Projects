using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinder : MonoBehaviour
{
	public Grid grid;

	private bool[] level;
	private bool[,] coveredArea;
	private int goalX;
	private int goalY;
	private List<SearchHead> searchers;

    public List<Vector2> findPath(Vector2 from, Vector2 to)
    {
        return findPath((int)from.x, (int)from.y, (int)to.x, (int)to.y);
    }

    public List<Vector2> findPath(int fromX, int fromY, int toX, int toY)
    {
        return findPath(fromX, fromY, toX, toY, grid.gridWidth, grid.gridHeight, grid.grid);
    }

    public List<Vector2> findPath(int fromX, int fromY, int toX, int toY, int pathWidth, int pathHeight, bool[,] level)
	{
		List<Vector2> path = new List<Vector2>();
		path.Add(new Vector2(fromX, fromY));
		searchers = new List<SearchHead>();
		coveredArea = new bool[pathHeight, pathWidth];
		addSearcher(fromX, fromY, pathWidth, pathHeight, 0, level, path);

		bool goalFound = false;
		int pathLength = 0;
		
		goalX = toX;
		goalY = toY;

		// timer to break infinite or too long pathfinding loop
		float time = Time.realtimeSinceStartup + 3f;

		while(!goalFound && time > Time.realtimeSinceStartup)
		{
			for(int i = 0; i < searchers.Count; i++)
			{

				if(searchers[i].posX == goalX && searchers[i].posY == goalY)
				{
					goalFound = true;
					pathLength = searchers[i].tilesTraversed;
					path = new List<Vector2>(searchers[i].moves);
					
					//string log = "";
					//for (int y = 0; y < searchers[i].moves.Count; y++)
					//	log += searchers[i].moves[y].ToString() + ", ";
					
					//Debug.Log (log);
					Debug.Log("Pathlength: "+pathLength.ToString());
					break;
				}
				else if(!searchers[i].move())
				{
					searchers.RemoveAt(i);
				}
			}
		}
		
		return path;
	}
	
	public void addSearcher(int x, int y, int xw, int yw, int count, bool[,] level, List<Vector2> moves, string lastMove = "none")
	{
		searchers.Add(new SearchHead(x, y, xw, yw, count, level, moves, this, lastMove));
	}
	
	public void uncoverArea(int x, int y)
	{
		coveredArea [x, y] = true;
	}

	public bool isCovered(int x, int y)
	{
		return coveredArea [x, y];
	}

	// Use this for initialization
	void Start ()
	{
        /*
		Vector2 startPos = new Vector2((Mathf.Abs(grid.gridOffsetZ) + (int) transform.position.z)/ grid.GRID_SIZE, (Mathf.Abs(grid.gridOffsetX) + (int) transform.position.x)/ grid.GRID_SIZE);
		GameObject goal;
		goal = GameObject.FindGameObjectWithTag("Finish");
		
		Vector2 goalPos = new Vector2((Mathf.Abs(grid.gridOffsetZ) + (int) goal.transform.position.z)/ grid.GRID_SIZE, (Mathf.Abs(grid.gridOffsetX) + (int) goal.transform.position.x)/ grid.GRID_SIZE);
		
		Debug.Log ("Start Pos: "+startPos);
		Debug.Log ("Goal Pos: "+goalPos);

		findPath ((int)startPos.x, (int)startPos.y, (int)goalPos.x, (int)goalPos.y, grid.gridWidth, grid.gridHeight, grid.grid);
        */
	}
}


class SearchHead
{
	private PathFinder parentClass;
	private string lastMove;
	
	public int posX;
	public int posY;
	private int pathWidth;
	private int pathHeight;
	
	private int _tilesTraversed;
	
	public int tilesTraversed
	{
		get {return _tilesTraversed;}
	}
	
	public List<Vector2> moves;
	private bool[,] level;
	
	public SearchHead(int _posX, int _posY, int _pathWidth, int _pathHeight, int tilesTraversed, bool[,] _level, List<Vector2> _moves, PathFinder _parentClass, string _lastMove = "none")
	{
		parentClass = _parentClass;
		
		pathHeight = _pathWidth;
		pathWidth = _pathHeight;
		
		posX = _posX;
		posY = _posY;

		parentClass.uncoverArea (posX, posY);
		
		_tilesTraversed = tilesTraversed;
		
		level = _level;
		moves = new List<Vector2>(_moves);

		if (moves[moves.Count-1].ToString() != new Vector2 (posX, posY).ToString())
			moves.Add(new Vector2(posX, posY));

		lastMove = _lastMove;
	}
	
	public bool move()
	{
		bool straightPossible = false;
		
		bool up = false;
		bool down = false;
		bool left = false;
		bool right = false;
		
		bool freePath = false;
		int possiblePaths = 0;

		if (lastMove == "none")
		{
			if(checkUp())
			{
				createSearcher(posX, posY - 1, "up");
			}
			if(checkDown())
			{
				createSearcher(posX, posY + 1, "down");
			}
			if(checkLeft())
			{
				createSearcher(posX - 1, posY, "left");
			}
			if(checkRight())
			{
				createSearcher(posX + 1, posY, "right");
			}
		}
		else
		{
			if(checkUp() && lastMove != "down")
			{
				possiblePaths++;
				up = true;
				if(lastMove == "up")
					straightPossible = true;
			}
			if(checkDown() && lastMove != "up")
			{
				possiblePaths++;
				down = true;
				if(lastMove == "down")
					straightPossible = true;
			}
			if(checkLeft() && lastMove != "right")
			{
				possiblePaths++;
				left = true;
				if(lastMove == "left")
					straightPossible = true;
			}
			if(checkRight() && lastMove != "left")
			{
				possiblePaths++;
				right = true;
				if(lastMove == "right")
					straightPossible = true;
			}
			/*
			string log = "";
			for(int z = 0; z < pathWidth; z++)
			{
				for(int x = 0; x < pathHeight; x++)
				{
					if(parentClass.isCovered(z, x))
						log += "X";
					else
						log += "_";
				}
				log += "\n";
			}
			Debug.Log (log);
			*/
	        if (possiblePaths > 3)
	        {
	            freePath = true;
	            createSearcher(posX + 1, posY, "right");
	            createSearcher(posX - 1, posY, "left");
	            createSearcher(posX, posY + 1, "down");
	            moveTo("up");
	        }
	        else if (possiblePaths > 2)
	        {
	            freePath = true;
	            switch (lastMove)
	            {
	                case "up":
	                    createSearcher(posX + 1, posY, "right");
	                    createSearcher(posX - 1, posY, "left");
	                    moveTo("up");
	                    break;

	                case "down":
	                    createSearcher(posX + 1, posY, "right");
	                    createSearcher(posX - 1, posY, "left");
	                    moveTo("down");
	                    break;

	                case "left":
	                    createSearcher(posX, posY - 1, "up");
	                    createSearcher(posX, posY + 1, "down");
	                    moveTo("left");
	                    break;

	                case "right":
	                    createSearcher(posX, posY - 1, "up");
	                    createSearcher(posX, posY + 1, "down");
	                    moveTo("right");
	                    break;

	                default:
	                    break;
	            }
	        }
	        else if (possiblePaths > 1)
	        {
	            freePath = true;
	            if (straightPossible)
	            {
	                switch (lastMove)
	                {
	                    case "up":
	                        if (right)
	                            createSearcher(posX + 1, posY, "right");
	                        else
	                            createSearcher(posX - 1, posY, "left");
	                        moveTo("up");
	                        break;

	                    case "down":
	                        if (right)
	                            createSearcher(posX + 1, posY, "right");
	                        else
	                            createSearcher(posX - 1, posY, "left");
	                        moveTo("down");
	                        break;

	                    case "left":
	                        if (up)
	                            createSearcher(posX, posY - 1, "up");
	                        else
	                            createSearcher(posX, posY + 1, "down");
	                        moveTo("left");
	                        break;

	                    case "right":
	                        if (up)
	                            createSearcher(posX, posY - 1, "up");
	                        else
	                            createSearcher(posX, posY + 1, "down");
	                        moveTo("right");
	                        break;
	                    default:
	                        break;
	                }
	            }
	            else
	            {
	                switch (lastMove)
	                {
	                    case "up":
	                        createSearcher(posX - 1, posY, "left");
	                        moveTo("right");
	                        break;

	                    case "down":
	                        createSearcher(posX - 1, posY, "left");
	                        moveTo("right");
	                        break;

	                    case "left":
	                        createSearcher(posX, posY + 1, "down");
	                        moveTo("up");
	                        break;

	                    case "right":
	                        createSearcher(posX, posY + 1, "down");
	                        moveTo("up");
	                        break;

	                    default:
	                        break;
	                }
	            }
	        }
	        else if (possiblePaths > 0)
	        {
	            freePath = true;
	            if (up)
	            {
	                moveTo("up");
	            }
	            else if (down)
	            {
	                moveTo("down");
	            }
	            else if (left)
	            {
	                moveTo("left");
	            }
	            else if (right)
	            {
	                moveTo("right");
	            }
        }
		}
		//Debug.Log ("Possible paths: "+possiblePaths+" is there free path? - "+freePath.ToString());

		return freePath;
	}
	
	private void createSearcher(int _x, int _y, string dir)
	{
		parentClass.addSearcher(_x, _y, pathHeight, pathWidth, _tilesTraversed+1, level, moves, dir);
	}
	
	private void moveTo(string to)
	{
		lastMove = to;
		
		_tilesTraversed++;
		
		switch(to)
		{
			case "up":
				posY--;
				break;
			case "down":
				posY++;
				break;
			case "left":
				posX--;
				break;
			case "right":
				posX++;
				break;
		}
		
		moves.Add(new Vector2(posX, posY));
		parentClass.uncoverArea (posX, posY);
	}
	
	private bool checkUp()
	{
		if(posY -1 < 0)
			return false;
		else
			if(level[posX, posY-1] && !parentClass.isCovered(posX, posY-1))
				return true;
		
		return false;
	}
	
	private bool checkDown()
	{
		if(posY +1 > pathHeight-1)
			return false;
		else
			if(level[posX, posY+1] && !parentClass.isCovered(posX, posY+1))
				return true;
		
		return false;
	}
	
	private bool checkLeft()
	{
		if(posX -1 < 0)
			return false;
		else
			if(level[posX-1, posY] && !parentClass.isCovered(posX-1, posY))
				return true;
		
		return false;
	}
	
	private bool checkRight()
	{
		if(posX +1 > pathWidth-1)
			return false;
		else
			if(level[posX+1, posY] && !parentClass.isCovered(posX+1, posY))
				return true;
		
		return false;
	}
}
