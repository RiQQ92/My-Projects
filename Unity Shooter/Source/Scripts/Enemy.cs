using UnityEngine;
using System.Collections;

// vihollisen pääluokan runko, ei mitään kiveen hakattua
public class Enemy : MonoBehaviour
{
    public Transform ammoType; // mitä ammuksia vihollinen ampuu
	public int weaponCooldown = 90; // ampumisen aikaväli freimeinä
	public int bulletAmount = 1; // panosten määrä per laukaus
	public float bulletSpread = 5.0f; // ammusten väli
	public float weaponRotationAmount = 0.0f;
	public bool rotateAlways = false;
	public bool rotateDir = true; // true = clock wise

    /* ie. forward, down, player, behind(miinoja varten) */
    public string shootDirection;

	protected Vector3 shootDir = Vector3.zero;
	protected Vector3 weaponRotation = Vector3.zero;

	protected int cooldown;

	private int rotDir = 1;
	private Animator anim;
	public string PathAnimation
	{
		get
		{
			return "Current animation playing";
		}
		set
		{
			if (anim == null) anim = GetComponent<Animator> ();
			anim.Play(value);
		}
	}

	void Awake()
	{
		Assets.enemies.Add (transform);
	}

	// Use this for initialization
	void Start ()
    {
		cooldown = weaponCooldown;
		inverseShootDir ();
		if (anim == null) anim = GetComponent<Animator> ();

		anim = GetComponent<Animator> ();
    }

    protected void inverseShootDir()
    {
        shootDir += new Vector3(0, 0, 180);
        if(shootDir.z > 360)
            shootDir -= new Vector3(0, 0, 360);
    }

	void Update()
	{
		if(!rotateDir)
			rotDir = -1;
		else
			rotDir = 1;

		weaponCooldown = (int)(1f / Time.fixedDeltaTime);
		if (rotateAlways)
			weaponRotation += (new Vector3 (0, 0, weaponRotationAmount) * rotDir);
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
		cooldown++;
		if(weaponCooldown <= cooldown)
        {
            shoot();
			cooldown = 0;
        }
	}

    protected void shoot() // <- virtual sana override komentoa varten lapsi luokissa
	{
		if(bulletAmount > 1)
		{
			for(float temp = (float)((float)-bulletAmount/2); temp < (float)((float)bulletAmount/2); temp++)
			{
				Transform bul = Instantiate(ammoType, transform.position, Quaternion.Euler(transform.eulerAngles + weaponRotation + shootDir + new Vector3(0, 0, (temp*bulletSpread) + (bulletSpread/2)))) as Transform;
				bul.tag = "EnemyBullet";
			}
		}
		else
		{
			Transform bul = Instantiate(ammoType, transform.position, Quaternion.Euler(transform.eulerAngles +  weaponRotation + shootDir)) as Transform;
	        bul.tag = "EnemyBullet";
		}
		
		if (!rotateAlways)
			weaponRotation += new Vector3 (0, 0, weaponRotationAmount);
    }
}
