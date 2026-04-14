using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyBehaviourTree
{
    public class Sequence : Node
    {
        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }

        public override NodeState Evalute()
        {
            bool anyChildIsRunning = false;

            foreach (Node node in children)
            {
                switch (node.Evalute())
                {
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }
            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;        
        }
    }
}

