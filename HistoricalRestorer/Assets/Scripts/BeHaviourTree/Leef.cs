using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leef : Node
{
    public delegate Status Tick();
    public Tick ProcessMethod; //节点执行什么方法（Status返回类型
    public Leef() { }
    public Leef(string n,Tick pm)
    {
        name = n;
        ProcessMethod = pm;
    }
    public override Status Process()
    {
        if (ProcessMethod!=null)
        {
            return ProcessMethod();
        }
        return Status.FAILURE;
    }
}
