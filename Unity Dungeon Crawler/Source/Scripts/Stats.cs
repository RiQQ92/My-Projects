using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Stats : MonoBehaviour {

	public int strength;
	public int Strength {
		get {
			return strength;
		}
		set {
			strength = value;
			UpdateStats();
		}
	}

	public int constitution;
	public int Constitution {
		get {
			return constitution;
		}
		set {
			constitution = value;
			UpdateStats();
		}
	}

	public int wisdom;
	public int Wisdom {
		get {
			return wisdom;
		} set
		{
			wisdom = value;
			UpdateStats();
		}
	}

	public int magic;
	public int Magic {
		get {
			return magic;
		}
		set {
			magic = value;
			UpdateStats();
		}
	}

	public int intelligence;
	public int Intelligence {
		get {
			return intelligence;
		}
		set {
			intelligence = value;
			UpdateStats();
		}
	}

	public int accuracy;
	public int Accuracy
	{
		get
		{
			return accuracy;
		}
		set
		{
			accuracy = value;
			UpdateStats();
		}
	}

	public int level;
	[HideInInspector]
	public int experience;
	public int Experience {
		get {
			return experience;
		}
		set {
			experience = value;
			while (experience >= 100) {
				experience -= 100;
				LevelUp();
			}
		}
	}

	private int statPoint;
	private int baseHealth = 100;
	private int baseDamage = 10;
	private int baseHeal = 9;
	private int baseManaHeal = 3;
	private int baseAccuracy = 50;

	private Text strText;
	private Text conText;
	private Text wisText;
	private Text magText;
	private Text intText;
	private Text accText;
	private Button strUp;
	private Button conUp;
	private Button wisUp;
	private Button magUp;
	private Button intUp;
	private Button accUp;
	private Text statPointText;

	void Start()
	{
		GameObject[] uiTxts = GameObject.FindGameObjectsWithTag ("Stats txt");
		GameObject[] uiBtns = GameObject.FindGameObjectsWithTag ("Stats btn");

		foreach(GameObject uiTxt in uiTxts)
		{
            if (uiTxt.name == "StrText")
            {
                strText = uiTxt.GetComponent<Text>();
            }
            else if (uiTxt.name == "ConText")
            {
                conText = uiTxt.GetComponent<Text>();
            }
            else if (uiTxt.name == "WisText")
            {
                wisText = uiTxt.GetComponent<Text>();
            }
            else if (uiTxt.name == "MagText")
            {
                magText = uiTxt.GetComponent<Text>();
            }
            else if (uiTxt.name == "IntText")
            {
                intText = uiTxt.GetComponent<Text>();
            }
            else if (uiTxt.name == "accText")
            {
                accText = uiTxt.GetComponent<Text>();
            }
            else if (uiTxt.name == "Point Text")
            {
                statPointText = uiTxt.GetComponent<Text>();
            }
		}
		foreach(GameObject uiBtn in uiBtns)
		{
            if (uiBtn.name == "Str Up")
            {
                strUp = uiBtn.GetComponent<Button>();
                strUp.onClick.AddListener(() => { this.RaiseStat(0); });
            }
            else if (uiBtn.name == "Con Up")
            {
                conUp = uiBtn.GetComponent<Button>();
                conUp.onClick.AddListener(() => { this.RaiseStat(1); });
            }
            else if (uiBtn.name == "Wis Up")
            {
                wisUp = uiBtn.GetComponent<Button>();
                wisUp.onClick.AddListener(() => { this.RaiseStat(2); });
            }
            else if (uiBtn.name == "Mag Up")
            {
                magUp = uiBtn.GetComponent<Button>();
                magUp.onClick.AddListener(() => { this.RaiseStat(3); });
            }
            else if (uiBtn.name == "Int Up")
            {
                intUp = uiBtn.GetComponent<Button>();
                intUp.onClick.AddListener(() => { this.RaiseStat(4); });
            }
            else if (uiBtn.name == "Acc Up")
            {
                accUp = uiBtn.GetComponent<Button>();
                accUp.onClick.AddListener(() => { this.RaiseStat(5); });
            }
		}

		UpdateStats();
	}

	void LevelUp()
	{
		level += 1;
		statPoint += 1;
		CheckUpgrade();
	}

	public void RaiseStat(int statIndex)
	{
		// inrease stat and reduce spendable statPoints
		statPoint -= 1;

		if (statIndex == 0)
		{
			Strength += 1;
		}
		if (statIndex == 1)
		{
			Constitution += 1;
			if (GetComponent<PHealth>() != null)
			{
				GetComponent<PHealth>().MaxHealth = CalcHealthPool();
			}
		}
		if (statIndex == 2)
		{
			Wisdom += 1;
			if (GetComponent<PlayerBehavior>() != null)
			{
				GetComponent<PlayerBehavior>().MaxMana = CalcManaPool();
			}
		}
		if (statIndex == 3)
		{
			Magic += 1;
		}
		if (statIndex == 4)
		{
			Intelligence += 1;
		}
		if (statIndex == 5)
		{
			Accuracy += 1;
		}

		CheckUpgrade();
	}

	void CheckUpgrade()
	{
		// Enable / Disable raise_stat button
		if (statPoint <= 0)
		{
			statPoint = 0;
			strUp.gameObject.SetActive(false);
			conUp.gameObject.SetActive(false);
			wisUp.gameObject.SetActive(false);
			magUp.gameObject.SetActive(false);
			intUp.gameObject.SetActive(false);
			accUp.gameObject.SetActive(false);
		}
		else
		{
			strUp.gameObject.SetActive(true);
			conUp.gameObject.SetActive(true);
			wisUp.gameObject.SetActive(true);
			magUp.gameObject.SetActive(true);
			intUp.gameObject.SetActive(true);
			if (Accuracy < 20)
			{
				accUp.gameObject.SetActive(true);
			}
			else
			{
				accUp.gameObject.SetActive(false);
			}
		}
		statPointText.text = statPoint.ToString();
	}

	void UpdateStats()
	{
		// Updates stats in the UI
		if (strText != null)
		{
			strText.text = strength.ToString();
		}
		if (conText != null)
		{
			conText.text = constitution.ToString();
		}
		if (wisText != null)
		{
			wisText.text = wisdom.ToString();
		}
		if (magText != null)
		{
			magText.text = magic.ToString();
		}
		if (intText != null)
		{
			intText.text = intelligence.ToString();
		}
		if (accText != null)
		{
			accText.text = accuracy.ToString();
		}
		CheckUpgrade();
	}

	public int CalcAttackDamage()
	{
		if (strength >= 10)
		{
			int amount = baseDamage + (strength - 10);
			amount = RandomFactor(amount);
			return amount;
		}
		else
		{
			int amount = baseDamage - Mathf.CeilToInt((strength - 10) * 0.5f);
			amount = RandomFactor(amount);
			return amount;
		}
	}

	public int CalcHealthPool()
	{
		if (constitution >= 10)
		{
			return baseHealth + ((constitution - 10) * 10);
		}
		else
		{
			return baseHealth - ((constitution - 10) * 5);
		}
	}

	public int CalcManaPool()
	{
		return wisdom;
	}

	public int CalcHealStrength()
	{
		if (magic >= 10)
		{
			int amount = baseHeal + (magic - 10);
			amount = RandomFactor(amount);
			return amount;
		}
		else
		{
			int amount = baseHeal + Mathf.CeilToInt((magic - 10) * 0.5f);
			amount = RandomFactor(amount);
			return amount;
		}
	}

	public int CalcManaStrength()
	{
		int amount = baseManaHeal + Mathf.CeilToInt((intelligence - 10) * 0.4f);
		amount = RandomFactor(amount);
		return amount;
	}

	public int CalcAccuracy()
	{
		if (accuracy >= 10)
		{
			return baseAccuracy + ((accuracy - 10) * 5);
		}
		else
		{
			return baseAccuracy - ((accuracy - 10) * 3);
		}
	}

	int RandomFactor(int max)
	{
		float rValue = Random.Range(0f, max);
		int outCome = Mathf.RoundToInt(rValue);
		if (outCome == 0)
		{
			return 1;
		}
		else
		{

			return outCome;
		}
	}
}
