using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineProblemTest : MonoBehaviour
{
    public List<GameObject> trackRefs;
    public TimelineAsset timelineAsset1,
        timelineAsset2,
        timelineAsset3;
    PlayableDirector playableDirector;
    SpriteRenderer spriteRenderer;
    public bool _1,
        _2,
        _3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_1)
        {
            spriteRenderer.material.SetFloat("OK", 0);
            _1 = false;
            playableDirector.time = 0;
            playableDirector.Evaluate();
            playableDirector.playableAsset = timelineAsset1;

            /* Handle timeline rebind */
            var tracks = timelineAsset1.GetOutputTracks().ToArray();

            for (int i = 0; i < tracks.Count(); i++)
            {
                playableDirector.ClearGenericBinding(trackRefs[i]);
                playableDirector.SetGenericBinding(tracks[i], trackRefs[i]);
            }

            playableDirector.Play();
        }

        if (_2)
        {
            _2 = false;

            playableDirector.time = 0;
            playableDirector.Evaluate();
            playableDirector.playableAsset = timelineAsset2;

            /* Handle timeline rebind */
            var tracks = timelineAsset2.GetOutputTracks().ToArray();

            for (int i = 0; i < tracks.Count(); i++)
            {
                playableDirector.ClearGenericBinding(trackRefs[i]);
                playableDirector.SetGenericBinding(tracks[i], trackRefs[i]);
            }

            playableDirector.Play();
        }

        if (_3)
        {
            _3 = false;

            playableDirector.time = 0;
            playableDirector.Evaluate();
            playableDirector.playableAsset = timelineAsset3;

            /* Handle timeline rebind */
            var tracks = timelineAsset3.GetOutputTracks().ToArray();

            for (int i = 0; i < tracks.Count(); i++)
            {
                playableDirector.ClearGenericBinding(trackRefs[i]);
                playableDirector.SetGenericBinding(tracks[i], trackRefs[i]);
            }

            playableDirector.Play();
        }
    }
}
