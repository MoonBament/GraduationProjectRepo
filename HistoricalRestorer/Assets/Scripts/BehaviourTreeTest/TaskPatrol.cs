using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyBehaviourTree;
public class TaskPatrol : EnemyBehaviourTree.Node
{
    private Transform transform;
    //private Animator animator;
    private IUserInput dui;
    private Transform[] waypoints;

    private int currentWaypointIndex = 0;

    private float waitTime = 1f;
    private float waitCounter = 0f;
    private bool waiting = false;

    public TaskPatrol(Transform trans,Transform[] theWaypoints)
    {
        transform = trans;
        dui = trans.gameObject.GetComponent<IUserInput>();
        //animator = transform.GetChild(2).gameObject.GetComponent<Animator>();
        waypoints = theWaypoints;
    }

    public override NodeState Evalute()
    {
        if (waiting)
        {
            waitCounter += Time.deltaTime;
            if (waitCounter>=waitTime)
            {
                waiting = false;
                //animator.SetFloat("forward", 1f);
                dui.UpdateDmagDvec(dui.Dup, dui.Dright);
            }
        }
        else
        {
            Transform wp = waypoints[currentWaypointIndex];
            if (Vector3.Distance(transform.position,wp.position)<0.01f)
            {
                transform.position = wp.position;
                waitCounter = 0f;
                waiting = true;

                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                //animator.SetFloat("forward", 0f);
                dui.UpdateDmagDvec(0, 0);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, wp.position, GuardBT.speed * Time.deltaTime);
                transform.LookAt(wp.position);
            }
        }

        state = NodeState.RUNNING;
        return state;
    }
}
