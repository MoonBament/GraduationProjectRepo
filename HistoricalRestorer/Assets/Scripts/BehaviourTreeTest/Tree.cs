using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyBehaviourTree
{
    public abstract class Tree : MonoBehaviour
    {
        private Node root = null;
        protected void Start()
        {
            root = SetupTree();
        }
        protected void Update()
        {
            if (root!=null)
            {
                root.Evalute();
            }
        }
        protected abstract Node SetupTree();
    }
}

