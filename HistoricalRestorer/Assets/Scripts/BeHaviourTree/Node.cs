using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public enum Status     //状态
    {
        SUCCESS,RUNNING,FAILURE
    };
    public Status status; 
    public List<Node> children = new List<Node>();//节点下的孩子
    public int currentChild = 0;//当前的孩子下标
    public string name;//节点名
    public Node(string n)
    {
        name = n;
    }
    public Node()
    {

    }
    public virtual Status Process()//该节点的总进行状态
    {
        return children[currentChild].Process();
    }
    public void AddChild(Node n)//添加节点下的孩子
    {
        children.Add(n);
    }
}
