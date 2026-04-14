using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class MyPlayableClip : PlayableAsset, ITimelineClipAsset
{
    public MyPlayableBehaviour template = new MyPlayableBehaviour ();
    public ExposedReference<ActorManager> am;
    //ExposedReference  是获取场景物件的
    //BehaviourReference 是获取Assets资源里的

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }//这里的None地方就是能调变两个Clip，None则代表两个Clip不会有任何重合的效果
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<MyPlayableBehaviour>.Create (graph, template);
        MyPlayableBehaviour clone = playable.GetBehaviour ();
        //解决不同轨道的clip里myCamara被塞相同的值，也就是初始化exposedName
        //（其实不加这句之前也是正常的，只是因为Unity在一开始没有帮你初始化这个东西，加了防止会出现相同情况的错误
        //am.exposedName = GetInstanceID().ToString(); 写的位置不对啦应该写在DiretorManager里

        clone.am = am.Resolve (graph.GetResolver ());
        return playable;
    }
}
