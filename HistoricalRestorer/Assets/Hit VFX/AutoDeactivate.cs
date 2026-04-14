using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//当对象被启用时，使用协程来挂起一小段时间
//(记得用对象池来管理特效)
public class AutoDeactivate : MonoBehaviour
{
    [SerializeField] bool destroyGameObject;
    [SerializeField] float lifetime = 3f;

    WaitForSeconds waitLifetime;

    private void Awake()
    {
        waitLifetime = new WaitForSeconds(lifetime);
    }

    private void OnEnable()
    {
        StartCoroutine(DeactivateCoroutine());
    }

    IEnumerator DeactivateCoroutine()
    {
        yield return waitLifetime;

        if (destroyGameObject)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
