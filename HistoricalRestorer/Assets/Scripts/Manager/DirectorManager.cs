using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]//在dm的同时将会拥有PlayableDirector组件
//导演管理员DirectorManager
public class DirectorManager : IActorManagerInterface
{
    public PlayableDirector pd;

    [Header("=== Timeline assets ===")]
    public TimelineAsset frontStab;   //前刺动画剧本
    public TimelineAsset openBox;     //打开宝箱剧本
    public TimelineAsset leverUp;     //拉杆动画剧本
    public TimelineAsset talkToGita;  //跟姬塔对话剧本

    [Header("=== Assets Settings ===")]
    public ActorManager attacker;
    public ActorManager victim;


    void Start()
    {
        pd = GetComponent<PlayableDirector>();
        pd.playOnAwake = false;
              
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.H) && gameObject.layer==LayerMask.NameToLayer("Enemy"))
        //{
        //    pd.Play();
        //}
    }

    public bool IsPlaying()
    {
        if (pd.state == PlayState.Playing)
        {
            return true;
        }
        return false;
    }


    public void PlayFrontStab(string timelineName,ActorManager attacker,ActorManager victim)
    {
        if (timelineName=="frontStab")
        {
            pd.playableAsset = Instantiate(frontStab);

            TimelineAsset timeline = (TimelineAsset)pd.playableAsset;         

            foreach (var track in timeline.GetOutputTracks())
            {
                if (track.name== "Attacker Script")
                {
                    pd.SetGenericBinding(track, attacker);
                    foreach (var clip in track.GetClips())
                    {
                        MyPlayableClip myclip = (MyPlayableClip)clip.asset;
                        MyPlayableBehaviour mybehav = myclip.template;
                        myclip.am.exposedName = System.Guid.NewGuid().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, attacker);
                    }
                }
                else if (track.name == "Victim Script")
                {
                    pd.SetGenericBinding(track, victim);
                    foreach (var clip in track.GetClips())
                    {
                        MyPlayableClip myclip = (MyPlayableClip)clip.asset;
                        MyPlayableBehaviour mybehav = myclip.template;
                        mybehav.MyFloat = 666;
                        myclip.am.exposedName = System.Guid.NewGuid().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, victim);
                    }
                }
                else if (track.name == "Attacker Animation")
                {
                    pd.SetGenericBinding(track, attacker.ac.Animator());
                }
                else if (track.name == "Victim Animation")
                {
                    pd.SetGenericBinding(track, victim.ac.Animator());
                }
            }

            pd.Evaluate();//提前呼叫Timeline

            pd.Play();
        }

        else if (timelineName=="openBox")
        {
            pd.playableAsset = Instantiate(openBox);

            TimelineAsset timeline = (TimelineAsset)pd.playableAsset;

            foreach (var track in timeline.GetOutputTracks())
            {
                if (track.name == "Player Script")
                {
                    pd.SetGenericBinding(track, attacker);
                    foreach (var clip in track.GetClips())
                    {
                        MyPlayableClip myclip = (MyPlayableClip)clip.asset;
                        MyPlayableBehaviour mybehav = myclip.template;
                        myclip.am.exposedName = System.Guid.NewGuid().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, attacker);
                    }
                }
                else if (track.name == "Box Script")
                {
                    pd.SetGenericBinding(track, victim);
                    foreach (var clip in track.GetClips())
                    {
                        MyPlayableClip myclip = (MyPlayableClip)clip.asset;
                        MyPlayableBehaviour mybehav = myclip.template;
                        myclip.am.exposedName = System.Guid.NewGuid().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, victim);
                    }
                }
                else if (track.name == "Player Animation")
                {
                    pd.SetGenericBinding(track, attacker.ac.Animator());
                }
                else if (track.name == "Box Animation")
                {
                    pd.SetGenericBinding(track, victim.ac.Animator());
                }
            }

            pd.Evaluate();//提前呼叫Timeline

            pd.Play();
        }

        else if (timelineName=="leverUp")
        {
            pd.playableAsset = Instantiate(leverUp);

            TimelineAsset timeline = (TimelineAsset)pd.playableAsset;

            foreach (var track in timeline.GetOutputTracks())
            {
                if (track.name == "Player Script")
                {
                    pd.SetGenericBinding(track, attacker);
                    foreach (var clip in track.GetClips())
                    {
                        MyPlayableClip myclip = (MyPlayableClip)clip.asset;
                        MyPlayableBehaviour mybehav = myclip.template;
                        myclip.am.exposedName = System.Guid.NewGuid().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, attacker);
                    }
                }
                else if (track.name == "Lever Script")
                {
                    pd.SetGenericBinding(track, victim);
                    foreach (var clip in track.GetClips())
                    {
                        MyPlayableClip myclip = (MyPlayableClip)clip.asset;
                        MyPlayableBehaviour mybehav = myclip.template;
                        myclip.am.exposedName = System.Guid.NewGuid().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, victim);
                    }
                }
                else if (track.name == "Player Animation")
                {
                    pd.SetGenericBinding(track, attacker.ac.Animator());
                }
                else if (track.name == "Lever Animation")
                {
                    pd.SetGenericBinding(track, victim.ac.Animator());
                }
            }

            pd.Evaluate();//提前呼叫Timeline

            pd.Play();
        }

        else if (timelineName == "talkToGita")
        {
            pd.playableAsset = Instantiate(talkToGita);

            TimelineAsset timeline = (TimelineAsset)pd.playableAsset;

            foreach (var track in timeline.GetOutputTracks())
            {
                if (track.name == "Player Script")
                {
                    pd.SetGenericBinding(track, attacker);
                    foreach (var clip in track.GetClips())
                    {
                        MyPlayableClip myclip = (MyPlayableClip)clip.asset;
                        MyPlayableBehaviour mybehav = myclip.template;
                        myclip.am.exposedName = System.Guid.NewGuid().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, attacker);
                    }
                }
                else if (track.name == "Gita Script")
                {
                    pd.SetGenericBinding(track, victim);
                    foreach (var clip in track.GetClips())
                    {
                        MyPlayableClip myclip = (MyPlayableClip)clip.asset;
                        MyPlayableBehaviour mybehav = myclip.template;
                        myclip.am.exposedName = System.Guid.NewGuid().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, victim);
                    }
                }
                else if (track.name == "Player Animation")
                {
                    pd.SetGenericBinding(track, attacker.ac.Animator());
                }
                else if (track.name == "Gita Animation")
                {
                    pd.SetGenericBinding(track, victim.ac.Animator());
                }
            }

            pd.Evaluate();//提前呼叫Timeline

            pd.Play();
        }
    }


}
