using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "TimelineSerializeSO", menuName = "TimelineSerializeSO", order = 0)]
public class TimelineSerializeSO : ScriptableObject
{
    public PlayableAsset timelinePlayable;
}
