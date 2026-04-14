using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextrBones : MonoBehaviour
{
    public SkinnedMeshRenderer skcMeshRenderer;
    public List<SkinnedMeshRenderer> tgtMeshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var tgtMeshRenderer in tgtMeshRenderer)
        {
            tgtMeshRenderer.bones = skcMeshRenderer.bones;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
