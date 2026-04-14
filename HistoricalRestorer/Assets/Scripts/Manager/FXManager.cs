using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : IActorManagerInterface
{
    public static FXManager instance;
    public GameObject openBloom;

    public void OnOpenBloom()
    {
        StartCoroutine("DisplayBloom");
    }
    
    IEnumerator DisplayBloom()
    {
        yield return new WaitForSeconds(3.0f);
        FXManager.instance.openBloom.SetActive(true);
    }
}
