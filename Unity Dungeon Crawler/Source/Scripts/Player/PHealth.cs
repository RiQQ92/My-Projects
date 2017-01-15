using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PHealth : MonoBehaviour {

	public int maxHealth;
	public int MaxHealth{
		get{
			return maxHealth;
		}
		set{
			maxHealth = value;
			UpdateMaxHealth();
		}
	}

	public int currentHealth;

    public RectTransform healthBar;
	public Text hpText;
	public Image screenFlash;
	private Turn turn;
	public Stats stats;

	void Start ()
	{
		GameObject[] uiTxts = GameObject.FindGameObjectsWithTag ("Stats txt");
		foreach(GameObject uiTxt in uiTxts)
		{
			if(uiTxt.name == "HealthText")
				hpText = uiTxt.GetComponent<Text>();
		}
		GameObject[] uiBars = GameObject.FindGameObjectsWithTag ("Stats bar");
		foreach(GameObject uiBar in uiBars)
		{
			if(uiBar.name == "HealthBar")
				healthBar = uiBar.GetComponent<RectTransform>();
		}

		maxHealth = stats.CalcHealthPool();
		currentHealth = maxHealth;
		turn = gameObject.GetComponent<Turn>();
		hpText.text = currentHealth.ToString();
	}

	public void TakeDamage(int damage)
	{
		int originalHealth = currentHealth;
		currentHealth -= damage;
		StartCoroutine (gameObject.GetComponent<PlayerBehavior>().ScreenFlash(Color.red));
		hpText.text = currentHealth.ToString();
		Debug.Log("Player took " + damage + " damage. Player has " + currentHealth + " health left.");
		StartCoroutine(HealthAnim(originalHealth, currentHealth, 2, false));
		HPcolor();
		if (currentHealth <= 0)
		{
			Die();
		}
    }

	public void HealDamage(int amount, bool endTurn)
	{
		float originalHealth = currentHealth;
		currentHealth += amount;
		if (currentHealth > maxHealth)
		{
			currentHealth = maxHealth;
		}
		hpText.text = currentHealth.ToString();
		Debug.Log("Player healed " + amount + " damage. Player has " + currentHealth + " health left.");
		StartCoroutine(HealthAnim(originalHealth, currentHealth, 2, endTurn));
		HPcolor();
	}

	void HPcolor()
	{
		float conCH = currentHealth;
		float conMH = maxHealth;
		if (conCH / conMH <= 0.2f)
		{
			healthBar.gameObject.GetComponent<Image>().color = Color.red;
		}
		else
		{
			healthBar.gameObject.GetComponent<Image>().color = Color.green;
		}
	}

	void UpdateMaxHealth()
	{
		float current = currentHealth;
		float max = MaxHealth;
		float barWidth = current / max * 180;
		healthBar.sizeDelta = new Vector2(barWidth, 15f);
	}

	IEnumerator HealthAnim(float start, float end, float time, bool endTurn)
	{
		start = start / maxHealth * 180;
		end = end / maxHealth * 180;

		float t = 0;
		while (t < 1)
		{
			healthBar.sizeDelta = new Vector2(Mathf.Lerp(start, end, t), 15);

			t += time / 60;
			yield return new WaitForFixedUpdate();
		}
		if (endTurn)
		{
			turn.EndMyTurn();
		}
	}

	private void Die()
	{
        Application.LoadLevel(Application.loadedLevel);
		Debug.Log("Player is dead");
	}
}
