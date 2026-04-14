using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftArmAnimFix : MonoBehaviour
{
    private Animator anim;
    private ActorController ac;
    public Vector3 a;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        ac = GetComponentInParent<ActorController>();//GetComponentInParent 比较耗性能，不过此次只做一次，所以还能接受
    }
    
    private void OnAnimatorIK(int layerIndex)
    {
        if (ac.leftIsShield)//左手持盾防御
        {
            if (anim.GetBool("defense") == false)//不是防御状态时
            {
                Transform leftLowerArm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                leftLowerArm.localEulerAngles += 0.75f * a;
                anim.SetBoneLocalRotation(HumanBodyBones.LeftLowerArm, Quaternion.Euler(leftLowerArm.localEulerAngles));
            }
        }
    }
}
