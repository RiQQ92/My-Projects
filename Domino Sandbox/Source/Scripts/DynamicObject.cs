using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DynamicObject : MonoBehaviour
{
    public AudioClip[] hittingSounds;

    private bool isSet;

    private float MaxImpactForce = 10f;
    private float MinImpactForce = 0.5f;

    private Vector3 startPos = Vector3.zero;
    private Quaternion startRot = Quaternion.identity;
    private AudioSource AS;
    
    void Start ()
    {
        AS = GetComponent<AudioSource>();
        MyEvents.AddEventListener(Reset, MyEventTypes.RESET_SCENE_EVENT);
        MyEvents.AddEventListener(Clear, MyEventTypes.CLEAR_DYNAMIC_EVENT);
        MyEvents.AddEventListener(Clear, MyEventTypes.CLEAR_SCENE_EVENT);
    }

    void OnCollisionEnter(Collision col)
    {
        float impactForce = Mathf.Abs(col.impulse.x) + Mathf.Abs(col.impulse.y) + Mathf.Abs(col.impulse.z);
        impactForce *= 1000;

        if (impactForce > MinImpactForce)
        {
            impactForce = Mathf.Clamp(impactForce, MinImpactForce, MaxImpactForce);
            float impactSoundVolume = impactForce / MaxImpactForce;

            
            AS.volume = impactSoundVolume;
            AS.pitch = Random.Range(0.7f, 1.2f);

            playRandomSound();
        }
    }

    public void Set()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        isSet = true;
    }

    private void RemoveListeners()
    {
        MyEvents.RemoveEventListener(Reset, MyEventTypes.RESET_SCENE_EVENT);
        MyEvents.RemoveEventListener(Clear, MyEventTypes.CLEAR_DYNAMIC_EVENT);
        MyEvents.RemoveEventListener(Clear, MyEventTypes.CLEAR_SCENE_EVENT);
    }

    private void Reset()
    {
        if (isSet)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            transform.position = startPos;
            transform.rotation = startRot;
        }
    }

    private void Clear()
    {
        if (isSet)
        {
            RemoveListeners();
            Destroy(gameObject);
        }
    }

    private void playRandomSound()
    {
        int sound2play = Random.Range(0, hittingSounds.Length-1);

        if(sound2play >= 0)
        {
            AS.PlayOneShot(hittingSounds[sound2play]);
        }
    }
}
