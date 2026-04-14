using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    BehaviourTree tree;
    public GameObject player;
    public enum ActionState
    {
        IDLE,WALKING,ATTACKING
    };
    ActionState state = ActionState.IDLE;
    Node.Status treeStatus = Node.Status.RUNNING;

    // Start is called before the first frame update
    void Start()
    {
        tree = new BehaviourTree();
        Sequence peace = new Sequence("Peace State");
        Leef peaceIdle = new Leef("Peace Idle", PeaceIdle);
        Leef peaceWalk = new Leef("Peace Walk", PeaceWalk);

        peace.AddChild(peaceIdle);
        peace.AddChild(peaceWalk);
        tree.AddChild(peace);

        peace.Process();
    }

    public Node.Status PeaceIdle()
    {
        Node.Status s = FindPlayer(player.transform.position);
        if (s==Node.Status.RUNNING)
        {

        }
        return s;
    }
    public Node.Status PeaceWalk()
    {
        Node.Status s = FindPlayer(player.transform.position);
        if (s == Node.Status.RUNNING)
        {

        }
        return s;
    }
   
    Node.Status FindPlayer(Vector3 player)
    {
        float distance = Vector3.Distance(player, this.transform.position);
        if (state==ActionState.IDLE)
        {
            state = ActionState.WALKING;
        }
        if (distance<=5)
        {
            state = ActionState.IDLE;
            return Node.Status.FAILURE;
        }
        else if (distance>5)
        {
            state = ActionState.IDLE;
            return Node.Status.RUNNING;
        }
        return Node.Status.RUNNING;
    }
    // Update is called once per frame
    void Update()
    {
        if (treeStatus==Node.Status.RUNNING)
        {
            treeStatus = tree.Process();
        }
    }
}
