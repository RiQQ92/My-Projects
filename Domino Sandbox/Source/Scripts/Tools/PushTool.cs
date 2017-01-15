using UnityEngine;
using System.Collections;

public class PushTool
{
    private Vector3 mouseWorldPos = Vector3.zero, lastMouseWorldPos = Vector3.zero;
    private Transform pusher;
    private Transform pusherOffset;

	private Animator anim;

    private bool cursorLocked;
    private bool knockActivated;

    Rigidbody rb;

    private GameObject toolSettingsPanel;

    public void Initialize(Transform pushMarker)
    {
        toolSettingsPanel = GameObject.Find("/HUD/LeftHandPanel");

        pusherOffset = new GameObject("Pusher Offset").transform;
        pusher = GameObject.Instantiate(pushMarker);
        pusher.parent = pusherOffset;

        pusherOffset.position = Vector3.zero;
        pusher.position = Vector3.zero;

        rb = pusher.gameObject.GetComponent<Rigidbody>();
        pusherOffset.gameObject.SetActive(false);

		anim = pusher.GetComponent<Animator> ();

    }

	private void doKnock()
    {
        if (pusherOffset.gameObject.activeSelf)
        {
            if (anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex("Base Layer")).IsName("KnockPull"))
            {
                anim.SetTrigger("Knock");
                knockActivated = true;
                EnableColliders();
            }
        }
	}

    public void EnableColliders()
    {
        foreach (CapsuleCollider sc in pusher.GetComponentsInChildren<CapsuleCollider>())
        {
            sc.isTrigger = false;
        }
    }

    public void DisableColliders()
    {
        foreach (CapsuleCollider sc in pusher.GetComponentsInChildren<CapsuleCollider>())
        {
            sc.isTrigger = true;
        }
    }


    /// <summary>
    /// Update function for non-monobehavior functions. Should be called from Update() in script which inherits from MonoBehavior
    /// </summary>
    public void ManualUpdate()
    {
        if (cursorLocked)
        {
            cursorLocked = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if(knockActivated)
        {
            if (!anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex("Base Layer")).IsName("KnockRelease") && !anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex("Base Layer")).IsName("KnockPull"))
            {
                knockActivated = false;
                DisableColliders();
            }
        }

        lastMouseWorldPos = mouseWorldPos;
        float rotFactor = 0.2f;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8))
        {
            if (!GetInput.leftMousePressed)
            {
                if (anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex("Base Layer")).IsName("Idle"))
                    pusherOffset.position = Vector3.Lerp(pusherOffset.position, hit.point + new Vector3(0, 0.8f, 0), 0.2f);
            }
            else
                rotFactor = 0.05f;

            if (!anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex("Base Layer")).IsName("KnockRelease"))
                mouseWorldPos = hit.point;
        }

        if(lastMouseWorldPos != mouseWorldPos)
        {
            Vector3 vecDir = mouseWorldPos - lastMouseWorldPos;
            float dir = Mathf.Atan2(vecDir.x, vecDir.z) * Mathf.Rad2Deg - 90;

            pusherOffset.rotation = Quaternion.Lerp(pusherOffset.rotation, Quaternion.Euler(pusherOffset.rotation.eulerAngles.x, dir, pusherOffset.rotation.eulerAngles.z), rotFactor);
        }

        if(GetInput.leftMousePressed)
        {
			anim.SetBool ("IsReadyToKnock", true);
        }
        else
		{
			anim.SetBool ("IsReadyToKnock", false);
        }
    }

    public void Enable()
    {
        toolSettingsPanel.SetActive(false);
		pusherOffset.gameObject.SetActive(true);
		MyEvents.AddEventListener (doKnock, MyEventTypes.LEFT_MB_UP);
    }

    public void Disable()
    {
        toolSettingsPanel.SetActive(true);
		pusherOffset.gameObject.SetActive(false);
		MyEvents.RemoveEventListener (doKnock, MyEventTypes.LEFT_MB_UP);
    }
}

