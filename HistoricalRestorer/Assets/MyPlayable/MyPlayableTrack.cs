using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.3915094f, 0.6957547f, 1f)]
[TrackClipType(typeof(MyPlayableClip))]
[TrackBindingType(typeof(ActorManager))]
public class MyPlayableTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<MyPlayableMixerBehaviour>.Create (graph, inputCount);
    }
}
