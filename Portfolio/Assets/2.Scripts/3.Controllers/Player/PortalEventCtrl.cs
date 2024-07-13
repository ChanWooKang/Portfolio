using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEventCtrl : MonoBehaviour
{
    public ParticleSystem portal;

    public bool isActivePortal;
    public void PlayPortal()
    {
        isActivePortal = true;
        portal.gameObject.SetActive(true);
        portal.Play(true);
    }

    public void StopPortal()
    {
        isActivePortal = false;
        portal.gameObject.SetActive(false);
        portal.Stop(true);
    }
}
