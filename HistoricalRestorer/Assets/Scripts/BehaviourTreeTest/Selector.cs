using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyBehaviourTree
{
    public class Selector : Node
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evalute()
        {
            foreach (Node node in children)
            {
                switch (node.Evalute())
                {
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        return state;
                    case NodeState.RUNNING:
                        state=NodeState.RUNNING;
                        return state;
                    case NodeState.FAILURE:
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }
            state = NodeState.FAILURE;
            return state;
        }
    }

}
