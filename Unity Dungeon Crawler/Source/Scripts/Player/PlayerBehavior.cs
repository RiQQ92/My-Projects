using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerBehavior : MonoBehaviour
{
	public enum Dir
	{
		North = 0,
		East = 1,
		South = 2,
		West = 3
	}

	private int maxMana;
	public int MaxMana{
		get{
			return maxMana;
		}
		set{
			maxMana = value;
			UpdateMaxMana();
		}
	}
	public int healCost;

	public Stats stats;
    public float moveSpeed;
    public float turnSpeed;
	public Turn playerTurn;
    public Grid grid;
	public Vector2 plrPos;
	public Image screenFlash;
	
	private GameObject canvas;
	private RectTransform manaBar;
	private Text manaText;
	private RectTransform healthBar;
	private Text healthText;
	private bool isTravelling;
	private int currentMana;
	private Vector2 destination;
	private PHealth pHeal;
	private int plrDir;

	// Use this for initialization
	void Start ()
	{
		canvas = GameObject.FindGameObjectWithTag("Player UI");
		Button[] canvasButtons = canvas.GetComponentsInChildren<Button> ();
		
		foreach (Button btn in canvasButtons)
		{
			if (btn.name == "Attack")
			{
				Debug.Log("attack found");
				btn.onClick.AddListener(() => {this.AttemptAttack();});
			}
			else if (btn.name == "Heal")
			{
				Debug.Log("heal found");
				btn.onClick.AddListener(() => {this.AttemptHeal();});
			}
			else if (btn.name == "Mana")
			{
				Debug.Log("mana found");
				btn.onClick.AddListener(() => {this.HealMana(10);});
			}
		}

		canvas = GameObject.FindGameObjectWithTag("Player UI");
		Image[] canvasImages = canvas.GetComponentsInChildren<Image> ();

		foreach (Image img in canvasImages)
		{
			if (img.name == "ScreenFlash")
			{
				Debug.Log("image found");
				screenFlash = img;
			}
		}

		GameObject[] uiTxts = GameObject.FindGameObjectsWithTag ("Stats txt");
		foreach(GameObject uiTxt in uiTxts)
		{
			if(uiTxt.name == "ManaText")
				manaText = uiTxt.GetComponent<Text>();
		}
		
		GameObject[] uiBars = GameObject.FindGameObjectsWithTag ("Stats bar");
		foreach(GameObject uiBar in uiBars)
		{
			if(uiBar.name == "ManaBar")
				manaBar = uiBar.GetComponent<RectTransform>();
		}

		maxMana = stats.CalcManaPool();
		currentMana = maxMana;
		//manaText = manaBar.gameObject.GetComponentInChildren<Text>();
		if(manaText != null)
			manaText.text = currentMana.ToString();
		pHeal = gameObject.GetComponent<PHealth>();

		grid = GameObject.FindGameObjectWithTag("Level").GetComponent<Grid>();

		isTravelling = false;
		destination = new Vector2(0, 0);
		plrDir = (int)Dir.North;
		plrPos = new Vector2((Mathf.Abs(grid.gridOffsetZ) + (int) transform.position.z)/ grid.GRID_SIZE, (Mathf.Abs(grid.gridOffsetX) + (int) transform.position.x)/ grid.GRID_SIZE);
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	public void move(bool dir) // false == backwards
	{
		if(!isTravelling && playerTurn.myTurn)
		{
			if(dir) // move forward
			{
				switch(plrDir)
				{
					case (int)Dir.North:
						if((int)(plrPos.x +1) < grid.gridHeight)
							if(grid.grid[(int)plrPos.x+1, (int)plrPos.y])
								travel(Dir.North);
						break;

					case (int)Dir.East:
						if((plrPos.y +1) < grid.gridWidth)
							if(grid.grid[(int)plrPos.x, (int)plrPos.y+1])
								travel(Dir.East);
						break;

					case (int)Dir.South:
						if((plrPos.x -1) >= 0)
							if(grid.grid[(int)plrPos.x-1, (int)plrPos.y])
								travel(Dir.South);
						break;

					case (int)Dir.West:
						if((plrPos.y -1) >= 0)
							if(grid.grid[(int)plrPos.x, (int)plrPos.y-1])
								travel(Dir.West);
						break;

					default:
						Debug.Log ("ERROR, PLR DIR OUT OF BOUNDS");
						break;
				}
			}
			else // move backward
			{
				switch(plrDir)
				{
				case (int)Dir.South:
					if((int)(plrPos.x +1) < grid.gridHeight)
						if(grid.grid[(int)plrPos.x+1, (int)plrPos.y])
							travel(Dir.North);
					break;
					
				case (int)Dir.West:
					if((plrPos.y +1) < grid.gridWidth)
						if(grid.grid[(int)plrPos.x, (int)plrPos.y+1])
							travel(Dir.East);
					break;
					
				case (int)Dir.North:
					if((plrPos.x -1) >= 0)
						if(grid.grid[(int)plrPos.x-1, (int)plrPos.y])
							travel(Dir.South);
					break;
					
				case (int)Dir.East:
					if((plrPos.y -1) >= 0)
						if(grid.grid[(int)plrPos.x, (int)plrPos.y-1])
							travel(Dir.West);
					break;
					
				default:
					Debug.Log ("ERROR, PLR DIR OUT OF BOUNDS");
					break;
				}
			}
		}
	}

	public void turn(bool dir) // false == left
    {
		if (!isTravelling && playerTurn.myTurn)
        {
            if (dir)
            {
                plrDir++;
                if (plrDir > 3)
                    plrDir = 0;

				isTravelling = true;
                StartCoroutine("Turn", transform.rotation.eulerAngles + new Vector3(0, 90, 0));
            }
            else
            {
                plrDir--;
                if (plrDir < 0)
                    plrDir = 3;
				
				isTravelling = true;
                StartCoroutine("Turn", transform.rotation.eulerAngles + new Vector3(0, -90, 0));
            }
        }
	}

	public void AttemptAttack()
	{
		if (playerTurn.myTurn == false)
		{
			return;
		}
		Debug.DrawRay (transform.position, transform.forward*5, Color.green, 5);
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 4f))
		{
			if (hit.collider.CompareTag("Enemy"))
			{
				// Attack
				Attack(hit.collider.gameObject);
			}
		}
	}

	void Attack(GameObject target)
	{
		Debug.Log("Player Attempts to attack");
		if (stats.CalcAccuracy() >= Random.Range(0f, 100f))
		{
			target.GetComponent<Health>().TakeDamage(stats.CalcAttackDamage());
		}
		else
		{
			Debug.Log("Attack misses");
		}
		Invoke("EndTurn", 0.5f);
	}
	
	void EndTurn()
	{
		playerTurn.EndMyTurn();
	}

	private void travel(Dir dir) // use Direction enums
	{
		int travelAmount = 1 * grid.GRID_SIZE;
		switch(dir)
		{
			case Dir.South:
				isTravelling = true;
			    destination = new Vector2((plrPos.y*grid.GRID_SIZE)-Mathf.Abs(grid.gridOffsetX), (plrPos.x*grid.GRID_SIZE)-Mathf.Abs(grid.gridOffsetZ) -travelAmount);
				break;
					
			case Dir.West:
				isTravelling = true;
				destination = new Vector2((plrPos.y*grid.GRID_SIZE)-Mathf.Abs(grid.gridOffsetX) -travelAmount, (plrPos.x*grid.GRID_SIZE)-Mathf.Abs(grid.gridOffsetZ));
				break;
					
			case Dir.North:
			    destination = new Vector2((plrPos.y*grid.GRID_SIZE)-Mathf.Abs(grid.gridOffsetX), (plrPos.x*grid.GRID_SIZE)-Mathf.Abs(grid.gridOffsetZ) +travelAmount);
				isTravelling = true;
				break;
				
			case Dir.East:
				isTravelling = true;
				destination = new Vector2((plrPos.y*grid.GRID_SIZE)-Mathf.Abs(grid.gridOffsetX) +travelAmount, (plrPos.x*grid.GRID_SIZE)-Mathf.Abs(grid.gridOffsetZ));
				break;
		}

        if(isTravelling)
        {
            StartCoroutine("Move", destination);
        }
	}

	
	public void AttemptHeal()
	{
		if (playerTurn.myTurn == false)
		{
			return;
		}
		if (pHeal.currentHealth >= pHeal.maxHealth)
		{
			Debug.Log("Already at full health");
			return;
		}
		if (currentMana >= healCost)
		{
			playerTurn.myTurn = false;
			pHeal.HealDamage(stats.CalcHealStrength(), true);
			TakeMana(healCost);
			StartCoroutine (ScreenFlash (Color.green));
		}
		else
			Debug.Log("Out of Mana");
	}

	void UpdateMaxMana()
	{
		float current = currentMana;
		float max = maxMana;
		float barWidth = current / max * 180;
		manaBar.sizeDelta = new Vector2(barWidth, 15f);
	}
	
	public void TakeMana(int amount)
	{
		int originalMana = currentMana;
		currentMana -= amount;
		manaText.text = currentMana.ToString();
		StartCoroutine(ManaAnim(originalMana, currentMana, 2, false));
	}
	
	public void HealMana(int amount)
	{
		if (playerTurn.myTurn == false)
		{
			return;
		}
		if (currentMana >= maxMana)
		{
			Debug.Log("Already at full mana");
			return;
		}
		else
		{
			int originalMana = currentMana;
			currentMana += amount;
			if (currentMana >= maxMana)
			{
				currentMana = maxMana;
			}
			manaText.text = currentMana.ToString ();
			StartCoroutine (ScreenFlash ());
			StartCoroutine (ManaAnim (originalMana, currentMana, 2, true));
		}
	}
	
	IEnumerator ManaAnim(float start, float end, float time, bool endTurn)
	{
		start = start / maxMana * 180;
		end = end / maxMana * 180;
		
		float t = 0;
		while (t < 1)
		{
			manaBar.sizeDelta = new Vector2(Mathf.Lerp(start, end, t), 15);
			
			t += time / 60;
			yield return new WaitForFixedUpdate();
		}
		if (endTurn)
		{
			playerTurn.EndMyTurn();
		}
	}

	public IEnumerator ScreenFlash(Color? c = null)
	{
		Color col = c ?? Color.blue;

		float t = 0.8f;
		while (t > 0)
		{
			t -= 0.05f;
			screenFlash.color = new Color(col.r, col.g, col.b, t);
			yield return new WaitForFixedUpdate();
		}
	}

    IEnumerator Turn(Vector3 targetDirection)
	{
		// playerTurn.myTurn = false;
        Vector3 origin = transform.eulerAngles;
        float t = 0;

        while (t < 1)
        {
            t += turnSpeed / 60;
            transform.eulerAngles = Vector3.Lerp(origin, targetDirection, t);
            yield return new WaitForFixedUpdate();
		}

        transform.eulerAngles = new Vector3(Mathf.RoundToInt(transform.eulerAngles.x), Mathf.RoundToInt(transform.eulerAngles.y), Mathf.RoundToInt(transform.eulerAngles.z));
        isTravelling = false;
		
		// playerTurn.EndMyTurn();
    }

    IEnumerator Move(Vector2 trgtLocation)
    {
		playerTurn.myTurn = false;
        Vector3 targetLocation = new Vector3(trgtLocation.x, transform.position.y, trgtLocation.y);
        Vector3 origin = transform.localPosition;
        float t = 0;

        while (transform.localPosition != targetLocation)
        {
            t += moveSpeed / 60;
            transform.localPosition = Vector3.Lerp(origin, targetLocation, t);
            yield return new WaitForFixedUpdate();
		}

        isTravelling = false;
        transform.localPosition = new Vector3(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y), Mathf.RoundToInt(transform.localPosition.z));
        plrPos = new Vector2((Mathf.Abs(grid.gridOffsetZ) + (int)transform.localPosition.z) / grid.GRID_SIZE, (Mathf.Abs(grid.gridOffsetX) + (int)transform.localPosition.x) / grid.GRID_SIZE);
		
		playerTurn.EndMyTurn();
	}
}
