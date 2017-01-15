using UnityEngine;
using System.Collections;

public class teleporterBehavior : MonoBehaviour
{
	public int teleportID = 1;
	public bool isReceiver = true;
	public bool isSender = true;
	public struct enemyInfo
	{
		public bool cooledDown;
		public bool rdyToTeleport;
	}
	public enemyInfo enemy;

	private float scaleMultiplier;
	private float teleporterCooldown;
	private bool coolingDown;

	private Vector3 fullScale;
	private Transform portal;

	[RPC]
	private void netStartCooldown()
	{
		startCooldown();
	}

	public bool getCooldown
	{
		get
		{
			return(coolingDown);
		}
	}

	public void startCooldown()
	{
		coolingDown = true;
		StartCoroutine(wait());
	}

	private IEnumerator wait()
	{
		yield return new WaitForSeconds(teleporterCooldown);
		coolingDown = false;
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(Network.isServer && isSender  && publicNetworkData.cooperative || publicStorage.localGame && isSender || !publicNetworkData.cooperative && isSender)
		{
			if(col.gameObject.tag == "Player" && !coolingDown)
			{
				if(col.gameObject.name != "HeadCollider")
				{
					if(!publicNetworkData.cooperative && col.networkView.isMine || Network.isServer && publicNetworkData.cooperative || publicStorage.localGame)
					{
						if(col.GetComponent<plrControl>().cooledDown && col.GetComponent<plrControl>().rdyToTeleport)
						{
							GameObject[] GOs = GameObject.FindGameObjectsWithTag("teleport");

							if(GOs != null) foreach(GameObject go in GOs)
							{
								if(go.GetInstanceID() != gameObject.GetInstanceID() && go.GetComponent<teleporterBehavior>().teleportID == teleportID 
								   && go.GetComponent<teleporterBehavior>().isReceiver && !go.GetComponent<teleporterBehavior>().getCooldown)
								{
									col.GetComponent<plrControl>().rdyToTeleport = false;
									col.transform.position = go.transform.position;
									col.GetComponent<plrControl>().startCooldown();

									if(publicStorage.localGame || !publicNetworkData.cooperative)
									{
										startCooldown();
										go.GetComponent<teleporterBehavior>().startCooldown();
									}
									else
									{
										networkView.RPC("netStartCooldown", RPCMode.All);
										go.GetComponent<teleporterBehavior>().networkView.RPC("netStartCooldown", RPCMode.All);
									}

									if(Network.isServer && publicNetworkData.cooperative)
										col.networkView.RPC("setPositionByServer", RPCMode.Others, go.transform.position);
									break;
								}
							}
						}
					}
				}
			}
			else if(col.gameObject.tag == "enemy" && !coolingDown)
			{
				col.gameObject.SendMessage("canTeleport", this);
				if(enemy.rdyToTeleport && enemy.cooledDown)
				{
					GameObject[] GOs = GameObject.FindGameObjectsWithTag("teleport");
					
					if(GOs != null) foreach(GameObject go in GOs)
					{
						if(go.GetInstanceID() != gameObject.GetInstanceID() && go.GetComponent<teleporterBehavior>().teleportID == teleportID 
						   && go.GetComponent<teleporterBehavior>().isReceiver && !go.GetComponent<teleporterBehavior>().getCooldown)
						{
							col.gameObject.SendMessage("teleportingCooldown");
							col.transform.position = go.transform.position;
							
							if(publicStorage.localGame || !publicNetworkData.cooperative)
							{
								startCooldown();
								go.GetComponent<teleporterBehavior>().startCooldown();
							}
							else
							{
								networkView.RPC("netStartCooldown", RPCMode.All);
								go.GetComponent<teleporterBehavior>().networkView.RPC("netStartCooldown", RPCMode.All);
							}
							
							if(Network.isServer && publicNetworkData.cooperative)
								col.gameObject.SendMessage("posAfterTeleporting");
							break;
						}
					}
				}
				enemy.rdyToTeleport = false;
				enemy.cooledDown = false;
			}
		}
	}

	void OnTriggerStay2D(Collider2D col)
	{
		if(Network.isServer && isSender  && publicNetworkData.cooperative || publicStorage.localGame && isSender || !publicNetworkData.cooperative && isSender)
		{
			if(col.gameObject.tag == "Player" && !coolingDown)
			{
				if(col.gameObject.name != "HeadCollider")
				{
					if(!publicNetworkData.cooperative && col.networkView.isMine || Network.isServer && publicNetworkData.cooperative || publicStorage.localGame)
					{
						if(col.GetComponent<plrControl>().cooledDown && col.GetComponent<plrControl>().rdyToTeleport)
						{
							GameObject[] GOs = GameObject.FindGameObjectsWithTag("teleport");
							
							if(GOs != null) foreach(GameObject go in GOs)
							{
								if(go.GetInstanceID() != gameObject.GetInstanceID() && go.GetComponent<teleporterBehavior>().teleportID == teleportID 
								   && go.GetComponent<teleporterBehavior>().isReceiver && !go.GetComponent<teleporterBehavior>().getCooldown)
								{
									col.GetComponent<plrControl>().rdyToTeleport = false;
									col.transform.position = go.transform.position;
									col.GetComponent<plrControl>().startCooldown();
									
									if(publicStorage.localGame || !publicNetworkData.cooperative)
									{
										startCooldown();
										go.GetComponent<teleporterBehavior>().startCooldown();
									}
									else
									{
										networkView.RPC("netStartCooldown", RPCMode.All);
										go.GetComponent<teleporterBehavior>().networkView.RPC("netStartCooldown", RPCMode.All);
									}
									
									if(Network.isServer && publicNetworkData.cooperative)
										col.networkView.RPC("setPositionByServer", RPCMode.Others, go.transform.position);
									break;
								}
							}
						}
					}
				}
			}
			else if(col.gameObject.tag == "enemy" && !coolingDown)
			{
				col.gameObject.SendMessage("canTeleport", this);
				if(enemy.rdyToTeleport && enemy.cooledDown)
				{
					GameObject[] GOs = GameObject.FindGameObjectsWithTag("teleport");
					
					if(GOs != null) foreach(GameObject go in GOs)
					{
						if(go.GetInstanceID() != gameObject.GetInstanceID() && go.GetComponent<teleporterBehavior>().teleportID == teleportID 
						   && go.GetComponent<teleporterBehavior>().isReceiver && !go.GetComponent<teleporterBehavior>().getCooldown)
						{
							col.gameObject.SendMessage("teleportingCooldownn");
							col.transform.position = go.transform.position;
							
							if(publicStorage.localGame || !publicNetworkData.cooperative)
							{
								startCooldown();
								go.GetComponent<teleporterBehavior>().startCooldown();
							}
							else
							{
								networkView.RPC("netStartCooldown", RPCMode.All);
								go.GetComponent<teleporterBehavior>().networkView.RPC("netStartCooldown", RPCMode.All);
							}
							
							if(Network.isServer && publicNetworkData.cooperative)
								col.gameObject.SendMessage("posAfterTeleporting");
							break;
						}
					}
				}
				enemy.rdyToTeleport = false;
				enemy.cooledDown = false;
			}
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if(Network.isServer && publicNetworkData.cooperative || publicStorage.localGame || !publicNetworkData.cooperative)
		{
			if(col.gameObject.tag == "Player" && !coolingDown)
			{
				if(col.gameObject.name != "HeadCollider")
				{
					col.GetComponent<plrControl>().rdyToTeleport = true;
				}
			}
			else if(col.gameObject.tag == "Enemy" && !coolingDown)
			{
				col.gameObject.SendMessage("setRdyToTeleport");
			}
		}
	}
	
	void Start ()
	{
		portal = transform.parent.FindChild("teleportPortal");
		fullScale = new Vector3(1,1,1);
		scaleMultiplier = 1f;
		teleporterCooldown = 1.5f;
		coolingDown = false;
	}

	void Update()
	{
		if(coolingDown && scaleMultiplier > 0)
		{
			scaleMultiplier -= 0.05f;
			portal.transform.localScale = fullScale*scaleMultiplier;
		}
		else if(!coolingDown && scaleMultiplier < 1)
		{
			scaleMultiplier += 0.05f;
			portal.transform.localScale = fullScale*scaleMultiplier;
		}
	}
}
