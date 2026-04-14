using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyBehaviourTree;

public class GuardBT : EnemyBehaviourTree.Tree
{
    public Transform[] waypoints;
    public static float speed = 2f;

    protected override EnemyBehaviourTree.Node SetupTree()
    {
        EnemyBehaviourTree.Node root = new TaskPatrol(transform, waypoints);
        return root;
    }
}
