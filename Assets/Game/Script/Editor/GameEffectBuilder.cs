#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class GameEffectBuilder : EditorWindow
{
    public GameObject basePrefab;
    public VisualTreeAsset vTA;
    Toggle isDeactivateAterTimeToggle,
        isTimelineToggle,
        randomRotationToggle,
        playSoundToggle,
        isColoredOverLifetimeToggle,
        useParticleSystemToggle;
    FloatField deactivateTimeFloatField,
        flyAtSpeedFloatField,
        followSlowlyPositionLerpTimeFloatField;
    Vector3Field followOffsetVector3Field,
        effectLocalPositionVector3Field,
        effectLocalRotationVector3Field;
    GradientField colorOverLifetimeGradientField;
    Button buildButton;
    TextField nameTextField,
        tagTextField;
    LayerMaskField collisionExcludeLayerMaskLayerMaskField;
    ListGameEffectBehavior listGameEffectBehavior;

    [MenuItem("Tools/GameEffectBuilder")]
    private static void ShowWindow()
    {
        var window = GetWindow<GameEffectBuilder>();
        window.titleContent = new GUIContent("GameEffectBuilder");
        window.Show();
    }

    void CreateGUI()
    {
        GetVisualElements();
        buildButton.RegisterCallbackOnce<ClickEvent>(e =>
        {
            GameObject t_build = Instantiate(basePrefab);
            t_build.name = nameTextField.text;

            GameEffectSO t_buildSO = CreateInstance<GameEffectSO>();
            t_buildSO.name = nameTextField.text + "SO";
            AssetDatabase.CreateAsset(
                t_buildSO,
                "Assets/Game/Resources/" + t_buildSO.name + ".asset"
            );
            AssetDatabase.SaveAssets();
        });
    }

    void GetVisualElements()
    {
        vTA.CloneTree(rootVisualElement);

        buildButton = rootVisualElement.Q<Button>("build-b");
        nameTextField = rootVisualElement.Q<TextField>("name-tf");
        isDeactivateAterTimeToggle = rootVisualElement.Q<Toggle>("deactivate-after-time-t");
        deactivateTimeFloatField = rootVisualElement.Q<FloatField>("deactivate-time-ff");
        isTimelineToggle = rootVisualElement.Q<Toggle>("is-timeline-t");
        randomRotationToggle = rootVisualElement.Q<Toggle>("random-rotation-tg");
        playSoundToggle = rootVisualElement.Q<Toggle>("play-sound-t");
        isColoredOverLifetimeToggle = rootVisualElement.Q<Toggle>("is-colored-over-lifetime-t");
        colorOverLifetimeGradientField = rootVisualElement.Q<GradientField>(
            "color-over-lifetime-gf"
        );
        useParticleSystemToggle = rootVisualElement.Q<Toggle>("use-particle-system-t");
        flyAtSpeedFloatField = rootVisualElement.Q<FloatField>("fly-at-speed-ff");
        followSlowlyPositionLerpTimeFloatField = rootVisualElement.Q<FloatField>(
            "follow-slowly-position-lerp-time-ff"
        );
        followOffsetVector3Field = rootVisualElement.Q<Vector3Field>("follow-offset-v3f");
        effectLocalPositionVector3Field = rootVisualElement.Q<Vector3Field>(
            "effect-local-position-v3f"
        );
        effectLocalRotationVector3Field = rootVisualElement.Q<Vector3Field>(
            "effect-local-rotation-v3f"
        );
        tagTextField = rootVisualElement.Q<TextField>("tag-tf");
        collisionExcludeLayerMaskLayerMaskField = rootVisualElement.Q<LayerMaskField>(
            "collision-exclude-layer-mask-lmf"
        );

        var gameEffectBehavioursIE = new InspectorElement();
        listGameEffectBehavior = CreateInstance<ListGameEffectBehavior>();
        gameEffectBehavioursIE.Bind(new SerializedObject(listGameEffectBehavior));
        rootVisualElement.Add(gameEffectBehavioursIE);
        gameEffectBehavioursIE.PlaceBehind(collisionExcludeLayerMaskLayerMaskField);
        rootVisualElement.Add(buildButton);
    }
}
#endif
