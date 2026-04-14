using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformHelpers //深度优先搜寻
{
    public static Transform DeepFind(this Transform parent,string targetName)
    {
        Transform temptrans = null;
        foreach (Transform child in parent)
        {
            if(child.name==targetName)
            {
                return child;
            }
            else
            {
                temptrans = DeepFind(child, targetName);
                if (temptrans!=null)
                {
                    return temptrans;
                }
            }
        }
        return temptrans;
    }
}
