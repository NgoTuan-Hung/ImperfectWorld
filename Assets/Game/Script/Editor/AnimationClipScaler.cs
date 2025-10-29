#if UNITY_EDITOR
using OneLine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimationClipScaler : EditorWindow
{
    [MenuItem("Window/AnimationClipScaler")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor.
        EditorWindow wnd = GetWindow<AnimationClipScaler>();
        wnd.titleContent = new GUIContent("AnimationClipScaler");

        // Limit size of the window.
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
    }

    void CreateGUI()
    {
        ScrollView root = new ScrollView();

        var inspectorListView = new InspectorElement();
        ListAnimationClipSO listAnimationClipSO = CreateInstance<ListAnimationClipSO>();
        inspectorListView.Bind(new SerializedObject(listAnimationClipSO));

        FloatField scaleFactorFF = new();
        scaleFactorFF.label = "Scale Factor";
        scaleFactorFF.value = 1f;

        Button button = new();
        button.text = "Scale";
        button.clicked += () =>
        {
            listAnimationClipSO.clips.ForEach(aC =>
            {
                ScaleClipTimes(aC, scaleFactorFF.value);
                EditorUtility.SetDirty(aC);
            });
            AssetDatabase.SaveAssets();
        };

        root.Add(inspectorListView);
        root.Add(scaleFactorFF);
        root.Add(button);
        rootVisualElement.Add(root);
    }

    void ScaleClipTimes(AnimationClip clip, float factor)
    {
        var events = clip.events;
        for (int i = 0; i < events.Length; i++)
        {
            events[i].time *= factor;
        }
        AnimationUtility.SetAnimationEvents(clip, events);

        var objBindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
        foreach (var binding in objBindings)
        {
            var keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);
            for (int i = 0; i < keyframes.Length; i++)
            {
                keyframes[i].time *= factor;
            }
            AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);
        }
    }
}
#endif
