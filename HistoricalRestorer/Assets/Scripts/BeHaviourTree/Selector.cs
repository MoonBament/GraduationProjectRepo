using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    public Selector(string n)
    {
        name = n;
    }
    public override Status Process()
    {
        Status childrenStatus = children[currentChild].Process();
        if (childrenStatus == Status.RUNNING)
        {
            return Status.RUNNING;
        }
        if (childrenStatus == Status.SUCCESS)
        {
            currentChild = 0;
            return Status.SUCCESS;
        }
        //前面都失败，就走下一个孩子节点
        currentChild++;
        if (currentChild >= children.Count)
        {
            currentChild = 0;
            //走完所有节点，无成功一个，则失败
            return Status.FAILURE;
        }
        //没走完所有孩子节点，继续走
        return Status.RUNNING;
    }
}
