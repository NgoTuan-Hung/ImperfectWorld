#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System;
using UnityEditor.Animations;

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
    Button buildButton,
        addColliderButton,
        addAnimationObjectButton;
    TextField nameTextField,
        tagTextField,
        defaultAnimationObjectControllerNameTextField,
        newAnimationObjectControllerNameTextField;
    LayerMaskField collisionExcludeLayerMaskLayerMaskField;
    ListGameEffectBehavior listGameEffectBehavior;
    CollideAndDamageSO collideAndDamageSO;
    VisualElement scrollViewContent,
        seperator1;
    DropdownField addColliderDropdownField;
    GameObject build;
    int combatColliderLayer;
    ObjectField spriteObjectField,
        newAnimationObjectSpriteObjectField;

    private void OnEnable()
    {
        combatColliderLayer = LayerMask.NameToLayer("CombatCollider");
    }

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
        BuildButtonHandler();
        AddButtonHandler();
    }

    private void AddButtonHandler()
    {
        addColliderButton.clicked += OnAddColliderButtonClicked;
        addAnimationObjectButton.clicked += OnAddAnimationObjectButtonClicked;
    }

    private void OnAddAnimationObjectButtonClicked()
    {
        if (build == null)
            return;

        GameObject t_animationObject = new();
        t_animationObject.transform.parent = build.transform.Find("AnimationObjects");
        t_animationObject.transform.localPosition = Vector3.zero;
        t_animationObject.name = "AnimationObject";

        SpriteRenderer t_sR = t_animationObject.AddComponent<SpriteRenderer>();
        t_sR.sprite = newAnimationObjectSpriteObjectField.value as Sprite;
        t_sR.sortingOrder = 3;
        t_sR.sortingLayerName = "Base";

        AnimatorController t_animatorController = AnimatorController.CreateAnimatorControllerAtPath(
            "Assets/Game/Graphics/Animation/"
                + (
                    string.IsNullOrEmpty(newAnimationObjectControllerNameTextField.text)
                        ? nameTextField.text + "AC"
                        : newAnimationObjectControllerNameTextField.text
                )
                + ".controller"
        );

        Animator t_animator = t_animationObject.AddComponent<Animator>();
        t_animator.runtimeAnimatorController = t_animatorController;

        AnimateObject t_aO = t_animationObject.AddComponent<AnimateObject>();
        t_aO.Reset();
    }

    private void OnAddColliderButtonClicked()
    {
        if (build == null)
            return;

        GameObject t_collider = new();
        t_collider.layer = combatColliderLayer;
        t_collider.transform.parent = build.transform.Find("Colliders");
        Collider2D t_collider2D = default;

        switch (addColliderDropdownField.value)
        {
            case "BoxCollider2D":
            {
                t_collider.name = "BoxCollider2D";
                t_collider2D = t_collider.AddComponent<BoxCollider2D>();
                break;
            }
            case "CircleCollider2D":
            {
                t_collider.name = "CircleCollider2D";
                t_collider2D = t_collider.AddComponent<CircleCollider2D>();
                break;
            }
            case "PolygonCollider2D":
            {
                t_collider.name = "PolygonCollider2D";
                t_collider2D = t_collider.AddComponent<PolygonCollider2D>();
                break;
            }
        }

        t_collider2D.isTrigger = true;
    }

    void GetVisualElements()
    {
        vTA.CloneTree(rootVisualElement);

        scrollViewContent = rootVisualElement.Q("scroll-view-content");
        buildButton = scrollViewContent.Q<Button>("build-b");
        nameTextField = scrollViewContent.Q<TextField>("name-tf");
        isDeactivateAterTimeToggle = scrollViewContent.Q<Toggle>("deactivate-after-time-t");
        deactivateTimeFloatField = scrollViewContent.Q<FloatField>("deactivate-time-ff");
        isTimelineToggle = scrollViewContent.Q<Toggle>("is-timeline-t");
        randomRotationToggle = scrollViewContent.Q<Toggle>("random-rotation-t");
        playSoundToggle = scrollViewContent.Q<Toggle>("play-sound-t");
        isColoredOverLifetimeToggle = scrollViewContent.Q<Toggle>("is-colored-over-lifetime-t");
        colorOverLifetimeGradientField = scrollViewContent.Q<GradientField>(
            "color-over-lifetime-gf"
        );
        useParticleSystemToggle = scrollViewContent.Q<Toggle>("use-particle-system-t");
        flyAtSpeedFloatField = scrollViewContent.Q<FloatField>("fly-at-speed-ff");
        followSlowlyPositionLerpTimeFloatField = scrollViewContent.Q<FloatField>(
            "follow-slowly-position-lerp-time-ff"
        );
        followOffsetVector3Field = scrollViewContent.Q<Vector3Field>("follow-offset-v3f");
        effectLocalPositionVector3Field = scrollViewContent.Q<Vector3Field>(
            "effect-local-position-v3f"
        );
        effectLocalRotationVector3Field = scrollViewContent.Q<Vector3Field>(
            "effect-local-rotation-v3f"
        );
        tagTextField = scrollViewContent.Q<TextField>("tag-tf");
        collisionExcludeLayerMaskLayerMaskField = scrollViewContent.Q<LayerMaskField>(
            "collision-exclude-layer-mask-lmf"
        );
        defaultAnimationObjectControllerNameTextField = scrollViewContent.Q<TextField>(
            "default-animation-object-controller-name-tf"
        );
        spriteObjectField = scrollViewContent.Q<ObjectField>("sprite-of");
        seperator1 = scrollViewContent.Q("seperator-1");

        var gameEffectBehavioursIE = new InspectorElement();
        gameEffectBehavioursIE.style.flexGrow = 0;
        listGameEffectBehavior = CreateInstance<ListGameEffectBehavior>();
        gameEffectBehavioursIE.Bind(new SerializedObject(listGameEffectBehavior));
        scrollViewContent.Add(gameEffectBehavioursIE);
        gameEffectBehavioursIE.PlaceInFront(collisionExcludeLayerMaskLayerMaskField);

        collideAndDamageSO = CreateInstance<CollideAndDamageSO>();
        var collideAndDamageIE = new InspectorElement();
        collideAndDamageIE.style.flexGrow = 0;
        collideAndDamageIE.Bind(new SerializedObject(collideAndDamageSO));
        scrollViewContent.Add(collideAndDamageIE);

        addColliderButton = scrollViewContent.Q<Button>("add-collider-b");
        addColliderDropdownField = scrollViewContent.Q<DropdownField>("add-collider-df");

        newAnimationObjectControllerNameTextField = scrollViewContent.Q<TextField>(
            "new-animation-object-controller-name-tf"
        );
        newAnimationObjectSpriteObjectField = scrollViewContent.Q<ObjectField>(
            "new-animation-object-sprite-of"
        );
        addAnimationObjectButton = scrollViewContent.Q<Button>("add-animation-object-b");

        scrollViewContent.Add(buildButton);
        collideAndDamageIE.PlaceInFront(seperator1);
    }

    void BuildButtonHandler()
    {
        buildButton.RegisterCallback<ClickEvent>(e =>
        {
            GameEffectSO t_buildSO = CreateInstance<GameEffectSO>();
            t_buildSO.name = nameTextField.text + "SO";
            t_buildSO.isDeactivateAfterTime = isDeactivateAterTimeToggle.value;
            t_buildSO.deactivateTime = deactivateTimeFloatField.value;
            t_buildSO.isTimeline = isTimelineToggle.value;
            t_buildSO.randomRotation = randomRotationToggle.value;
            t_buildSO.flyAtSpeed = flyAtSpeedFloatField.value;
            t_buildSO.followOffset = followOffsetVector3Field.value;
            t_buildSO.followSlowlyPositionLerpTime = followSlowlyPositionLerpTimeFloatField.value;
            t_buildSO.playSound = playSoundToggle.value;
            t_buildSO.effectLocalPosition = effectLocalPositionVector3Field.value;
            t_buildSO.effectLocalRotation = effectLocalRotationVector3Field.value;
            t_buildSO.isColoredOverLifetime = isColoredOverLifetimeToggle.value;
            t_buildSO.colorOverLifetimeGrad = colorOverLifetimeGradientField.value;
            t_buildSO.tag = tagTextField.text;
            t_buildSO.collisionExcludeLayerMask = collisionExcludeLayerMaskLayerMaskField.value;
            t_buildSO.gameEffectBehaviours = listGameEffectBehavior.effectBehaviours;
            t_buildSO.useParticleSystem = useParticleSystemToggle.value;

            build = Instantiate(basePrefab);
            build.name = nameTextField.text;
            PlayableDirector t_pD;
            Animator t_animator;
            if (t_buildSO.isTimeline)
            {
                t_pD = build.AddComponent<PlayableDirector>();
                t_pD.playOnAwake = false;
                var t_tA = CreateInstance<TimelineAsset>();
                t_tA.name = nameTextField.text + "Timeline";
                AssetDatabase.CreateAsset(t_tA, "Assets/Game/Timeline/" + t_tA.name + ".asset");
                AssetDatabase.SaveAssets();
                t_pD.playableAsset = t_tA;
                t_animator = build.AddComponent<Animator>();
            }
            GameEffect t_gE = build.GetComponent<GameEffect>();
            t_gE.gameEffectSO = t_buildSO;

            #region Add Animator
            var t_animater = build
                .transform.Find("AnimationObjects")
                .Find("AnimationObject")
                .GetComponent<Animator>();
            AnimatorController t_animatorController =
                AnimatorController.CreateAnimatorControllerAtPath(
                    "Assets/Game/Graphics/Animation/"
                        + (
                            string.IsNullOrEmpty(defaultAnimationObjectControllerNameTextField.text)
                                ? nameTextField.text + "AC"
                                : defaultAnimationObjectControllerNameTextField.text
                        )
                        + ".controller"
                );
            t_animater.runtimeAnimatorController = t_animatorController;
            #endregion

            #region Add Sprite
            t_animater.gameObject.GetComponent<SpriteRenderer>().sprite =
                spriteObjectField.value as Sprite;
            #endregion

            t_buildSO.gameEffectBehaviours?.ForEach(behaviour =>
            {
                switch (behaviour)
                {
                    case EGameEffectBehaviour.CollideAndDamage:
                    {
                        build.AddComponent<CollideAndDamage>();
                        collideAndDamageSO.name = nameTextField.text + "CADSO";
                        AssetDatabase.CreateAsset(
                            collideAndDamageSO,
                            "Assets/Game/Resources/" + collideAndDamageSO.name + ".asset"
                        );
                        AssetDatabase.SaveAssets();
                        t_buildSO.collideAndDamageSO = collideAndDamageSO;
                        break;
                    }
                    case EGameEffectBehaviour.BlueHole:
                    {
                        build.AddComponent<BlueHole>();
                        break;
                    }
                    case EGameEffectBehaviour.InfernalTideFanReceiver:
                    {
                        build.AddComponent<InfernalTideFanReceiver>();
                        break;
                    }
                    case EGameEffectBehaviour.SovereignFlowBehaviour:
                    {
                        build.AddComponent<SovereignFlowBehaviour>();
                        break;
                    }
                    case EGameEffectBehaviour.PullingMissile:
                    {
                        build.AddComponent<PullingMissile>();
                        break;
                    }
                    case EGameEffectBehaviour.GlacistreamBehaviour:
                    {
                        build.AddComponent<GlacistreamBehaviour>();
                        break;
                    }
                    default:
                        break;
                }
            });

            AssetDatabase.CreateAsset(
                t_buildSO,
                "Assets/Game/Resources/" + t_buildSO.name + ".asset"
            );
            AssetDatabase.SaveAssets();

            PrefabUtility.SaveAsPrefabAssetAndConnect(
                build,
                "Assets/Game/Resources/" + build.name + ".prefab",
                InteractionMode.UserAction
            );
        });
    }
}
#endif
