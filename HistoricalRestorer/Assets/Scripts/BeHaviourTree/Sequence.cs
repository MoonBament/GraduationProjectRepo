using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public Sequence(string n)
    {
        name = n;
    }
    public override Status Process()
    {
        Status childrenStatus = children[currentChild].Process();
        if (childrenStatus==Status.RUNNING)
        {
            return Status.RUNNING;
        }
        if (childrenStatus==Status.FAILURE)
        {
            return childrenStatus;
        }
        currentChild++;
        if (currentChild>=children.Count)
        {
            currentChild = 0;
            //走完所有流程，成功
            return Status.SUCCESS;
        }
        return Status.RUNNING;
    }
}
