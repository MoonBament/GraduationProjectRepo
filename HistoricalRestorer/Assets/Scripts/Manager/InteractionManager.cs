using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//互动管理员InteractionManager
public class InteractionManager : IActorManagerInterface
{
    private CapsuleCollider interCol;
    //交互作用中的事件列表
    public List<EventCasterManager> overlapEcasterms = new List<EventCasterManager>();

    // Start is called before the first frame update
    void Start()
    {
        interCol = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider col)
    {
        EventCasterManager[] ecasterms = col.GetComponents<EventCasterManager>();
        foreach (var ecasterm in ecasterms)
        {
            if (!overlapEcasterms.Contains(ecasterm))
            {
                overlapEcasterms.Add(ecasterm);
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        EventCasterManager[] ecasterms = col.GetComponents<EventCasterManager>();
        foreach (var ecasterm in ecasterms)
        {
            if (!overlapEcasterms.Contains(ecasterm))
            {
                Debug.Log("清空");
                overlapEcasterms.Remove(ecasterm);
            }
        }
    }
}
