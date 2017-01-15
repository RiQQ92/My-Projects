using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
	public int maxHealth = 100;
	private int currentHealth = 100;

	private SpriteRenderer sr;
	private Sprite healthBar;
	private GameObject healthContainer;
	private bool healthChanged = true;

	public void getDamage(int dmg)
	{
		currentHealth -= dmg;

		if (currentHealth <= 0)
		{
			die ();
		}
		
		healthChanged = true;
	}

	private void die()
	{
		if (gameObject.transform.parent)
		{
			if(gameObject.transform.parent.name == "Enemy")
				Assets.enemies.Remove(transform);

			Destroy (transform.parent.gameObject);
		}
		else
		{
			if(gameObject.transform.name == "EnemyShip")
				Assets.enemies.Remove(transform);

            if (gameObject.name == "Player")
            {
                GameObject.FindGameObjectWithTag("UI").GetComponent<Canvas>().enabled = true;
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
		}
	}

	// Use this for initialization
	void Start ()
	{
		currentHealth = maxHealth;
		sr = gameObject.GetComponent<SpriteRenderer> ();
		DrawQuad (new Rect(0, 0, -sr.sprite.rect.width, -10), Color.green);
		healthContainer = new GameObject ();
		healthContainer.transform.parent = this.transform;
		SpriteRenderer renderer = healthContainer.AddComponent<SpriteRenderer>();
		renderer.sprite = healthBar;
		healthContainer.transform.localPosition = Vector3.zero;
		healthContainer.transform.position = this.transform.position + new Vector3(0.75f, sr.sprite.rect.height/2/100 +0.1f, 0);
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (healthChanged)
		{
			float healthSize = (float)currentHealth / (float)maxHealth;

			healthContainer.transform.localScale = new Vector3 (healthSize, 1, 1);
			healthChanged = false;
		}
	}

	void DrawQuad(Rect position, Color color)
	{
		int width;
		int height;
		
		if(((int)position.width) %2 == 0)
			width = (int) Mathf.Abs(position.width);
		else
			width = (int) Mathf.Abs(position.width)+1;
		
		if(((int)position.height) %2 == 0)
			height = (int) Mathf.Abs(position.width);
		else
			height = (int) Mathf.Abs(position.width)+1;

		Texture2D texture = new Texture2D(width, height);
		Color[] cols = new Color[width * height];

		for(int i = 0; i < cols.Length; i++)
			cols [i] = color;

		texture.SetPixels(0,0,width-1, height-1, cols);
		texture.Apply();
		healthBar = Sprite.Create (texture, position, Vector2.zero);
	}
}
