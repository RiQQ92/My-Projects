using System;
using System.Collections;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [NonSerialized]
    public int GRID_SIZE;

    [NonSerialized]
    public int gridWidth, gridHeight, gridOffsetX, gridOffsetZ, gridEndX, gridEndZ;

    [NonSerialized]
    public bool[,] grid;

    // all road pieces
    public Transform
                roadCornerIn,
                roadCornerOut,
                roadCornerPassThrough,
                roadDeadEnd,
                roadOpen,
                roadOpening,
                roadOpeningWithTurn,
                roadStraight,
                roadTIntersection,
                roadTurn,
                roadWall,
                roadWallWithOuterCorner,
                roadXIntersection;

    // all road pieces

    private GameObject curRoad;
	private GameObject[] roads;

	// Use this for initialization
	void Awake ()
	{
        GRID_SIZE = 2;
        gridOffsetX = 1337;
		gridOffsetZ = 1337;
		gridEndX = 1337;
		gridEndZ = 1337;

		initGrid ();
        autotileTheGrid();
	}

    private void initGrid()
	{
		// find all road pieces
		if (roads == null)
			roads = GameObject.FindGameObjectsWithTag("Road");

		// loop through all road pieces and check their position
		foreach (GameObject road in roads)
		{
			// Set grid offset to most farthest object towards negative pos
			if(gridOffsetX == 1337)
				gridOffsetX = (int) road.transform.localPosition.x;
			else if(gridOffsetX > road.transform.localPosition.x)
				gridOffsetX = (int) road.transform.localPosition.x;
			
			if(gridOffsetZ == 1337)
				gridOffsetZ = (int) road.transform.localPosition.z;
			else if(gridOffsetZ > road.transform.localPosition.z)
				gridOffsetZ = (int) road.transform.localPosition.z;
			
			// Set grid offset to most farthest object towards positive pos
			if(gridEndX == 1337)
				gridEndX = (int) road.transform.localPosition.x;
			else if(gridEndX < road.transform.localPosition.x)
				gridEndX = (int) road.transform.localPosition.x;
			
			if(gridEndZ == 1337)
				gridEndZ = (int) road.transform.localPosition.z;
			else if(gridEndZ < road.transform.localPosition.z)
				gridEndZ = (int) road.transform.localPosition.z;
		}

		// calculate max difference between x and z axis on grid and set grid width and height
		gridWidth = (Mathf.Abs(gridOffsetX) + Mathf.Abs(gridEndX))/ GRID_SIZE +1;
		gridHeight = (Mathf.Abs(gridOffsetZ) + Mathf.Abs(gridEndZ))/ GRID_SIZE +1;

		// set grid size and false info
		grid = new bool[gridHeight, gridWidth];

		for(int z = 0; z < gridHeight; z++)
		{
			for(int x = 0; x < gridWidth; x++)
			{
				grid[z, x] = false;
			}
		}
		
		// input all road pieces to grid
		foreach (GameObject road in roads)
		{
			grid[(Mathf.Abs(gridOffsetZ) + (int) road.transform.localPosition.z)/ GRID_SIZE, (Mathf.Abs(gridOffsetX) + (int) road.transform.localPosition.x)/ GRID_SIZE] = true;
		}
		
		Debug.Log ("GridOffX : "+gridOffsetX+"; GridOffZ : "+gridOffsetZ+"; GridEndX : "+gridEndX+"; GridEndZ : "+gridEndZ+"; gridWidth : "+gridWidth+"; gridHeight : "+gridHeight);

		// print level on console
		string log = "";
		for(int z = 0; z < gridHeight; z++)
		{
			for(int x = 0; x < gridWidth; x++)
			{
				if(grid[z, x])
					log += "X";
				else
					log += "_";
			}
			log += "\n";
		}
		Debug.Log (log);
    }

    private void autotileTheGrid ()
    {
        // find all road pieces
        if (roads == null)
            roads = GameObject.FindGameObjectsWithTag("Road");

        foreach (GameObject road in roads)
        {
            curRoad = road;

            int roadX = 0;
            int roadY = 0;

            roadX = (Mathf.Abs(gridOffsetX) + (int)road.transform.localPosition.x) / GRID_SIZE;
            roadY = (Mathf.Abs(gridOffsetZ) + (int)road.transform.localPosition.z) / GRID_SIZE;

            // check boundaries for grid
            // this is in the corner of grid
            if (roadX == 0 && roadY == 0 || roadX == (gridWidth-1) && roadY == 0 || roadX == (gridWidth - 1) && roadY == (gridHeight -1) || roadX == 0 && roadY == (gridHeight - 1))
            {
                drawCornerTile(roadX, roadY);
            }
            // this is in the horizontal edge of grid
            else if (roadX == 0 || roadX == (gridWidth -1))
            {
                drawHorizontalEdgeTile(roadX, roadY);
            }
            // this is in the vertical edge of grid
            else if (roadY == 0 || roadY == (gridHeight - 1))
            {
                drawVerticalEdgeTile(roadX, roadY);
            }
            // this is somewhere in the middle of grid
            else
            {
                drawMiddleTile(roadX, roadY);
            }
        }
    }

    private void drawCornerTile (int posX, int posY)
    {
        string tileToDraw = "";

        // vasen yläkulma
        if (posX == 0 && posY == 0)
        {
            tileToDraw = checkSurroundings(posX, posY, "NW");
        }
        // oikea yläkulma
        else if (posX == (gridWidth - 1) && posY == 0)
        {
            tileToDraw = checkSurroundings(posX, posY, "NE");
        }
        // oikea alakulma
        else if (posX == (gridWidth - 1) && posY == (gridHeight - 1))
        {
            tileToDraw = checkSurroundings(posX, posY, "SE");
        }
        // vasen alakulma
        else
        {
            tileToDraw = checkSurroundings(posX, posY, "SW");
        }

        drawNewTile(tileToDraw);
    }

    private void drawHorizontalEdgeTile (int posX, int posY)
    {
        string tileToDraw = "";

        // vasen reuna
        if (posX == 0)
        {
            tileToDraw = checkSurroundings(posX, posY, "W");
        }
        // oikea reuna
        else
        {
            tileToDraw = checkSurroundings(posX, posY, "E");
        }

        drawNewTile(tileToDraw);
    }

    private void drawVerticalEdgeTile (int posX, int posY)
    {
        string tileToDraw = "";

        // yläreuna
        if (posY == 0)
        {
            tileToDraw = checkSurroundings(posX, posY, "N");
        }
        // alareuna
        else
        {
            tileToDraw = checkSurroundings(posX, posY, "S");
        }

        drawNewTile(tileToDraw);
    }

    private void drawMiddleTile (int posX, int posY)
    {
        string tileToDraw = "";
        tileToDraw = checkSurroundings(posX, posY);
        drawNewTile(tileToDraw);
    }

    //limits: NW, NE, SW, SE for corners; N, E, S, W for edges; or none, then checks all around
    private string checkSurroundings (int posY, int posX, string limitations = "none")
    {
        char[] surroundings;
        surroundings = new char[8];

        // set zero value to all
        for (int i = 0; i < 8; i++)
        {
            surroundings[i] = 'F';
        }

        switch (limitations)
        {
            case "none":
                // NW
                if (grid[posX - 1, posY - 1])
                    surroundings[0] = 'T';

                // N
                if (grid[posX - 1, posY])
                    surroundings[1] = 'T';

                // NE
                if (grid[posX - 1, posY + 1])
                    surroundings[2] = 'T';

                // E
                if (grid[posX, posY + 1])
                    surroundings[3] = 'T';

                // SE
                if (grid[posX + 1, posY + 1])
                    surroundings[4] = 'T';

                // S
                if (grid[posX + 1, posY])
                    surroundings[5] = 'T';

                // SW
                if (grid[posX + 1, posY - 1])
                    surroundings[6] = 'T';

                // W
                if (grid[posX, posY - 1])
                    surroundings[7] = 'T';

                break;

            case "NW":
                // E
                if (grid[posX, posY + 1])
                    surroundings[3] = 'T';

                // SE
                if (grid[posX + 1, posY + 1])
                    surroundings[4] = 'T';

                // S
                if (grid[posX + 1, posY])
                    surroundings[5] = 'T';

                break;

            case "NE":
                // S
                if (grid[posX + 1, posY])
                    surroundings[5] = 'T';

                // SW
                if (grid[posX + 1, posY - 1])
                    surroundings[6] = 'T';

                // W
                if (grid[posX, posY - 1])
                    surroundings[7] = 'T';

                break;

            case "SW":
                // N
                if (grid[posX - 1, posY])
                    surroundings[1] = 'T';

                // NE
                if (grid[posX - 1, posY + 1])
                    surroundings[2] = 'T';

                // E
                if (grid[posX, posY + 1])
                    surroundings[3] = 'T';

                break;

            case "SE":
                // NW
                if (grid[posX - 1, posY - 1])
                    surroundings[0] = 'T';

                // N
                if (grid[posX - 1, posY])
                    surroundings[1] = 'T';
                
                // W
                if (grid[posX, posY - 1])
                    surroundings[7] = 'T';

                break;

            case "N":
                // E
                if (grid[posX, posY + 1])
                    surroundings[3] = 'T';

                // SE
                if (grid[posX + 1, posY + 1])
                    surroundings[4] = 'T';

                // S
                if (grid[posX + 1, posY])
                    surroundings[5] = 'T';

                // SW
                if (grid[posX + 1, posY - 1])
                    surroundings[6] = 'T';

                // W
                if (grid[posX, posY - 1])
                    surroundings[7] = 'T';

                break;

            case "E":
                // NW
                if (grid[posX - 1, posY - 1])
                    surroundings[0] = 'T';

                // N
                if (grid[posX - 1, posY])
                    surroundings[1] = 'T';
                
                // S
                if (grid[posX + 1, posY])
                    surroundings[5] = 'T';

                // SW
                if (grid[posX + 1, posY - 1])
                    surroundings[6] = 'T';

                // W
                if (grid[posX, posY - 1])
                    surroundings[7] = 'T';

                break;

            case "S":
                // NW
                if (grid[posX - 1, posY - 1])
                    surroundings[0] = 'T';

                // N
                if (grid[posX - 1, posY])
                    surroundings[1] = 'T';

                // NE
                if (grid[posX - 1, posY + 1])
                    surroundings[2] = 'T';

                // E
                if (grid[posX, posY + 1])
                    surroundings[3] = 'T';
                
                // W
                if (grid[posX, posY - 1])
                    surroundings[7] = 'T';
                break;

            case "W":
                // N
                if (grid[posX - 1, posY])
                    surroundings[1] = 'T';

                // NE
                if (grid[posX - 1, posY + 1])
                    surroundings[2] = 'T';

                // E
                if (grid[posX, posY + 1])
                    surroundings[3] = 'T';

                // SE
                if (grid[posX + 1, posY + 1])
                    surroundings[4] = 'T';

                // S
                if (grid[posX + 1, posY])
                    surroundings[5] = 'T';
                
                break;

            default:
                Debug.Log("Got incorrect argument for checkSurroundings function. value got = "+limitations);
                break;
        }

        return selectCorrectTile(surroundings);
    }

    // Autotiles 46 different tile placements with 13 different tiles
    private string selectCorrectTile (char[] surroundings)
    {
        /*
        string text = "surrounding values! - ";
         for (int i = 0; i < 8; i++)
             text += surroundings[i].ToString()+", "; 
         Debug.Log(text);
        */

        /*  Checking shape and order //

         NW    N    NE
           [0][1][2]
         W [7][x][3] E
           [6][5][4]
         SW    S    SE

       //  Checking shape and order */

        /*  Char Values //

        - F(False)          = There is no road
        - T(True)           = There is road
        - I(Insignificant)  = Doesnt matter if there is or isn't wall

        //  Char Values */

        string tileName = "";
        curRoad.transform.localScale = new Vector3(1, 1, 1);

        // X Intersection
        if (compareCharArrs(surroundings, new char[] { 'F', 'T', 'F', 'T', 'F', 'T', 'F', 'T' }))
        {
            tileName = "Road X Intersection";
        }

        // Open area
        else if (compareCharArrs(surroundings, new char[] { 'T', 'T', 'T', 'T', 'T', 'T', 'T', 'T' }))
        {
            tileName = "Road Open";
        }

        // Opening
        else if (compareCharArrs(surroundings, new char[] { 'F', 'T', 'F', 'T', 'T', 'T', 'T', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'T', 'T', 'F', 'T', 'F', 'T', 'T', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'T', 'T', 'T', 'T', 'F', 'T', 'F', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'F', 'T', 'T', 'T', 'T', 'T', 'F', 'T' }))
        {
            tileName = "Road Opening";

            if (compareCharArrs(surroundings, new char[] { 'T', 'T', 'T', 'T', 'F', 'T', 'F', 'T' }))
                curRoad.transform.eulerAngles = new Vector3(0, 90, 0) + curRoad.transform.eulerAngles;
            else if (compareCharArrs(surroundings, new char[] { 'T', 'T', 'F', 'T', 'F', 'T', 'T', 'T' }))
                curRoad.transform.eulerAngles = new Vector3(0, 180, 0) + curRoad.transform.eulerAngles;
            else if (compareCharArrs(surroundings, new char[] { 'F', 'T', 'F', 'T', 'T', 'T', 'T', 'T' }))
                curRoad.transform.eulerAngles = new Vector3(0, 270, 0) + curRoad.transform.eulerAngles;
        }

        // Opening with turn
        else if (compareCharArrs(surroundings, new char[] { 'F', 'T', 'F', 'T', 'F', 'T', 'T', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'T', 'T', 'F', 'T', 'F', 'T', 'F', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'F', 'T', 'T', 'T', 'F', 'T', 'F', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'F', 'T', 'F', 'T', 'T', 'T', 'F', 'T' }))
        {
            tileName = "Road Opening With Turn";
            
            if (compareCharArrs(surroundings, new char[] { 'F', 'T', 'F', 'T', 'F', 'T', 'T', 'T' }))
            { curRoad.transform.eulerAngles = new Vector3(0, 270, 0) + curRoad.transform.eulerAngles; }
            else if (compareCharArrs(surroundings, new char[] { 'T', 'T', 'F', 'T', 'F', 'T', 'F', 'T' }))
            { curRoad.transform.eulerAngles = new Vector3(0, 180, 0) + curRoad.transform.eulerAngles; }
            else if (compareCharArrs(surroundings, new char[] { 'F', 'T', 'T', 'T', 'F', 'T', 'F', 'T' }))
            { curRoad.transform.eulerAngles = new Vector3(0, 90, 0) + curRoad.transform.eulerAngles; }
        }

        // Corner Pass Through
        else if (compareCharArrs(surroundings, new char[] { 'F', 'T', 'T', 'T', 'F', 'T', 'T', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'T', 'T', 'F', 'T', 'T', 'T', 'F', 'T' }))
        {
            tileName = "Road Corner Pass Through";

            if (compareCharArrs(surroundings, new char[] { 'F', 'T', 'T', 'T', 'F', 'T', 'T', 'T' }))
                curRoad.transform.eulerAngles = new Vector3(0, 90, 0) + curRoad.transform.eulerAngles;
        }

        // Corner out
        else if (compareCharArrs(surroundings, new char[] { 'F', 'T', 'T', 'T', 'T', 'T', 'T', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'T', 'T', 'F', 'T', 'T', 'T', 'T', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'T', 'T', 'T', 'T', 'F', 'T', 'T', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'T', 'T', 'T', 'T', 'T', 'T', 'F', 'T' }))
        {
            tileName = "Road Corner Out";
           
            if (compareCharArrs(surroundings, new char[] { 'T', 'T', 'F', 'T', 'T', 'T', 'T', 'T' }))
                curRoad.transform.eulerAngles = new Vector3(0, 90, 0) + curRoad.transform.eulerAngles;
            else if (compareCharArrs(surroundings, new char[] { 'F', 'T', 'T', 'T', 'T', 'T', 'T', 'T' }))
                curRoad.transform.eulerAngles = new Vector3(0, 180, 0) + curRoad.transform.eulerAngles;
            else if (compareCharArrs(surroundings, new char[] { 'T', 'T', 'T', 'T', 'T', 'T', 'F', 'T' }))
                curRoad.transform.eulerAngles = new Vector3(0, 270, 0) + curRoad.transform.eulerAngles;
        }

        // Corner in
        else if (compareCharArrs(surroundings, new char[] { 'T', 'T', 'I', 'F', 'F', 'F', 'I', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'I', 'T', 'T', 'T', 'I', 'F', 'F', 'F' }) ||
                 compareCharArrs(surroundings, new char[] { 'F', 'F', 'I', 'T', 'T', 'T', 'I', 'F' }) ||
                 compareCharArrs(surroundings, new char[] { 'I', 'F', 'F', 'F', 'I', 'T', 'T', 'T' }))
        {
            tileName = "Road Corner In";

            if (compareCharArrs(surroundings, new char[] { 'I', 'F', 'F', 'F', 'I', 'T', 'T', 'T' }))
                curRoad.transform.eulerAngles = new Vector3(0, 90, 0) + curRoad.transform.eulerAngles;
            else if (compareCharArrs(surroundings, new char[] { 'F', 'F', 'I', 'T', 'T', 'T', 'I', 'F' }))
                curRoad.transform.eulerAngles = new Vector3(0, 180, 0) + curRoad.transform.eulerAngles;
            else if (compareCharArrs(surroundings, new char[] { 'I', 'T', 'T', 'T', 'I', 'F', 'F', 'F' }))
                curRoad.transform.eulerAngles = new Vector3(0, 270, 0) + curRoad.transform.eulerAngles;
        }

        // Straight
        else if (compareCharArrs(surroundings, new char[] { 'I', 'T', 'I', 'F', 'I', 'T', 'I', 'F' }) ||
                 compareCharArrs(surroundings, new char[] { 'I', 'F', 'I', 'T', 'I', 'F', 'I', 'T' }))
        {
            tileName = "Road Straight";

            if(compareCharArrs(surroundings, new char[] { 'I', 'T', 'I', 'F', 'I', 'T', 'I', 'F' }))
                curRoad.transform.eulerAngles = new Vector3(0, 90, 0) + curRoad.transform.eulerAngles;
                
        }

        // Turn
        else if (compareCharArrs(surroundings, new char[] { 'F', 'T', 'I', 'F', 'F', 'F', 'I', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'I', 'T', 'F', 'T', 'I', 'F', 'F', 'F' }) ||
                 compareCharArrs(surroundings, new char[] { 'F', 'F', 'I', 'T', 'F', 'T', 'I', 'F' }) ||
                 compareCharArrs(surroundings, new char[] { 'I', 'F', 'F', 'F', 'I', 'T', 'F', 'T' }))
        {
            tileName = "Road Turn";

            if (compareCharArrs(surroundings, new char[] { 'F', 'F', 'I', 'T', 'F', 'T', 'I', 'F' }))
                curRoad.transform.eulerAngles = new Vector3(0, 90, 0) + curRoad.transform.eulerAngles;
            else if (compareCharArrs(surroundings, new char[] { 'I', 'T', 'F', 'T', 'I', 'F', 'F', 'F' }))
                curRoad.transform.eulerAngles = new Vector3(0, 180, 0) + curRoad.transform.eulerAngles;
            else if (compareCharArrs(surroundings, new char[] { 'F', 'T', 'I', 'F', 'F', 'F', 'I', 'T' }))
                curRoad.transform.eulerAngles = new Vector3(0, 270, 0) + curRoad.transform.eulerAngles;
        }

        // Wall
        else if (compareCharArrs(surroundings, new char[] { 'I', 'F', 'I', 'T', 'T', 'T', 'T', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'T', 'T', 'I', 'F', 'I', 'T', 'T', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'T', 'T', 'T', 'T', 'I', 'F', 'I', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'I', 'T', 'T', 'T', 'T', 'T', 'I', 'F' }))
        {
            tileName = "Road Wall";

            if (compareCharArrs(surroundings, new char[] { 'I', 'F', 'I', 'T', 'T', 'T', 'T', 'T' }))
                curRoad.transform.eulerAngles = new Vector3(0, 90, 0) + curRoad.transform.eulerAngles;
            else if (compareCharArrs(surroundings, new char[] { 'I', 'T', 'T', 'T', 'T', 'T', 'I', 'F' }))
                curRoad.transform.eulerAngles = new Vector3(0, 180, 0) + curRoad.transform.eulerAngles;
            else if (compareCharArrs(surroundings, new char[] { 'T', 'T', 'T', 'T', 'I', 'F', 'I', 'T' }))
                curRoad.transform.eulerAngles = new Vector3(0, 270, 0) + curRoad.transform.eulerAngles;
        }

        // Wall with outer corner
        else if (compareCharArrs(surroundings, new char[] { 'I', 'F', 'I', 'T', 'F', 'T', 'T', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'I', 'F', 'I', 'T', 'T', 'T', 'F', 'T' }) ||

                 compareCharArrs(surroundings, new char[] { 'T', 'T', 'I', 'F', 'I', 'T', 'F', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'F', 'T', 'I', 'F', 'I', 'T', 'T', 'T' }) ||

                 compareCharArrs(surroundings, new char[] { 'F', 'T', 'T', 'T', 'I', 'F', 'I', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'T', 'T', 'F', 'T', 'I', 'F', 'I', 'T' }) ||

                 compareCharArrs(surroundings, new char[] { 'I', 'T', 'F', 'T', 'T', 'T', 'I', 'F' }) ||
                 compareCharArrs(surroundings, new char[] { 'I', 'T', 'T', 'T', 'F', 'T', 'I', 'F' }))
        {
            tileName = "Road Wall With Outer Corner";

            if(compareCharArrs(surroundings, new char[] { 'I', 'F', 'I', 'T', 'T', 'T', 'F', 'T' }) ||
               compareCharArrs(surroundings, new char[] { 'F', 'T', 'I', 'F', 'I', 'T', 'T', 'T' }) ||
               compareCharArrs(surroundings, new char[] { 'T', 'T', 'F', 'T', 'I', 'F', 'I', 'T' }) ||
               compareCharArrs(surroundings, new char[] { 'I', 'T', 'T', 'T', 'F', 'T', 'I', 'F' }))
            {
                // MIRRORED
                curRoad.transform.localScale = new Vector3(1, -1, 1);

                if (compareCharArrs(surroundings, new char[] { 'I', 'F', 'I', 'T', 'T', 'T', 'F', 'T' }))
                    curRoad.transform.eulerAngles = new Vector3(0, 90, 0) + curRoad.transform.eulerAngles;
                else if (compareCharArrs(surroundings, new char[] { 'I', 'T', 'T', 'T', 'F', 'T', 'I', 'F' }))
                    curRoad.transform.eulerAngles = new Vector3(0, 180, 0) + curRoad.transform.eulerAngles;
                else if (compareCharArrs(surroundings, new char[] { 'T', 'T', 'F', 'T', 'I', 'F', 'I', 'T' }))
                    curRoad.transform.eulerAngles = new Vector3(0, 270, 0) + curRoad.transform.eulerAngles;
            }
            else
            {
                // NORMAL
                if (compareCharArrs(surroundings, new char[] { 'I', 'F', 'I', 'T', 'F', 'T', 'T', 'T' }))
                    curRoad.transform.eulerAngles = new Vector3(0, 90, 0) + curRoad.transform.eulerAngles;
                else if (compareCharArrs(surroundings, new char[] { 'I', 'T', 'F', 'T', 'T', 'T', 'I', 'F' }))
                    curRoad.transform.eulerAngles = new Vector3(0, 180, 0) + curRoad.transform.eulerAngles;
                else if (compareCharArrs(surroundings, new char[] { 'F', 'T', 'T', 'T', 'I', 'F', 'I', 'T' }))
                    curRoad.transform.eulerAngles = new Vector3(0, 270, 0) + curRoad.transform.eulerAngles;
            }
        }

        // Dead end
        else if (compareCharArrs(surroundings, new char[] { 'I', 'F', 'I', 'F', 'I', 'T', 'I', 'F' }) ||
                 compareCharArrs(surroundings, new char[] { 'I', 'T', 'I', 'F', 'I', 'F', 'I', 'F' }) ||
                 compareCharArrs(surroundings, new char[] { 'I', 'F', 'I', 'F', 'I', 'F', 'I', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'I', 'F', 'I', 'T', 'I', 'F', 'I', 'F' }))
        {
            tileName = "Road Dead End";

            if (compareCharArrs(surroundings, new char[] { 'I', 'F', 'I', 'F', 'I', 'T', 'I', 'F' }))
                curRoad.transform.eulerAngles = new Vector3(0, 90, 0) + curRoad.transform.eulerAngles;
            else if (compareCharArrs(surroundings, new char[] { 'I', 'F', 'I', 'T', 'I', 'F', 'I', 'F' }))
                curRoad.transform.eulerAngles = new Vector3(0, 180, 0) + curRoad.transform.eulerAngles;
            else if (compareCharArrs(surroundings, new char[] { 'I', 'T', 'I', 'F', 'I', 'F', 'I', 'F' }))
                curRoad.transform.eulerAngles = new Vector3(0, 270, 0) + curRoad.transform.eulerAngles;
        }

        // T Intersection
        else if (compareCharArrs(surroundings, new char[] { 'I', 'F', 'I', 'T', 'I', 'T', 'I', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'I', 'T', 'I', 'F', 'I', 'T', 'I', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'I', 'T', 'I', 'T', 'I', 'F', 'I', 'T' }) ||
                 compareCharArrs(surroundings, new char[] { 'I', 'T', 'I', 'T', 'I', 'T', 'I', 'F' }))
        {
            tileName = "Road T Intersection";

            if (compareCharArrs(surroundings, new char[] { 'I', 'F', 'I', 'T', 'I', 'T', 'I', 'T' }))
                curRoad.transform.eulerAngles = new Vector3(0, 90, 0) + curRoad.transform.eulerAngles;
            else if (compareCharArrs(surroundings, new char[] { 'I', 'T', 'I', 'T', 'I', 'T', 'I', 'F' }))
                curRoad.transform.eulerAngles = new Vector3(0, 180, 0) + curRoad.transform.eulerAngles;
            else if (compareCharArrs(surroundings, new char[] { 'I', 'T', 'I', 'T', 'I', 'F', 'I', 'T' }))
                curRoad.transform.eulerAngles = new Vector3(0, 270, 0) + curRoad.transform.eulerAngles;
        }

        return tileName;
    }

    private void drawNewTile (string tileName)
    {
        Transform prefab;

        switch (tileName)
        {
            case "Road Corner In":
                prefab = roadCornerIn;
                break;

            case "Road Corner Out":
                prefab = roadCornerOut;
                break;

            case "Road Corner Pass Through":
                prefab = roadCornerPassThrough;
                break;

            case "Road Dead End":
                prefab = roadDeadEnd;
                break;

            case "Road Open":
                prefab = roadOpen;
                break;

            case "Road Opening":
                prefab = roadOpening;
                break;

            case "Road Opening With Turn":
                prefab = roadOpeningWithTurn;
                break;

            case "Road Straight":
                prefab = roadStraight;
                break;

            case "Road T Intersection":
                prefab = roadTIntersection;
                break;

            case "Road Turn":
                prefab = roadTurn;
                break;

            case "Road Wall":
                prefab = roadWall;
                break;

            case "Road Wall With Outer Corner":
                prefab = roadWallWithOuterCorner;
                break;

            case "Road X Intersection":
                prefab = roadXIntersection;
                break;

            default:
                prefab = roadStraight;
                break;
        }
        
        Transform go = Instantiate(prefab, curRoad.transform.position, prefab.rotation) as Transform;
        go.eulerAngles += new Vector3(0, curRoad.transform.eulerAngles.y, 0);
        go.position = new Vector3(go.position.x, 1, go.position.z);
        go.localScale = new Vector3(1, curRoad.transform.localScale.y, 1);
        Destroy(curRoad);
    }

    private bool compareCharArrs (char[] arr1, char[] arr2)
    {
        /*  Char Values //
        
        - F(False)          = There is no road
        - T(True)           = There is road
        - I(Insignificant)  = Doesnt matter if there is or isn't road

        //  Char Values */
        bool isEqual = true;

        for (int i = 0; i < arr1.Length; i++)
        {
            if (arr1[i] != arr2[i] && arr2[i] != 'I')
            {
                isEqual = false;
                break;
            }
        }

        return isEqual;
    }
}
