using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineDeserialize : MonoBehaviour
{
    public TimelineSerializeSO timelineSerializeSO;
    PlayableDirector playableDirector;
    public List<GameObject> trackReferences;
    SignalReceiver signalReceiver;

    private void Awake()
    {
        playableDirector = GetComponent<PlayableDirector>();
        signalReceiver = GetComponent<SignalReceiver>();
        var tracks = ((TimelineAsset)timelineSerializeSO.timelinePlayable)
            .GetOutputTracks()
            .ToArray();

        for (int i = 0; i < tracks.Count(); i++)
        {
            playableDirector.SetGenericBinding(tracks[i], trackReferences[i]);
        }

        playableDirector.playableAsset = timelineSerializeSO.timelinePlayable;
    }

    private void Start()
    {
        playableDirector.Play();
    }
}
