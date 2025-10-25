#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System;
using UnityEditor.Animations;

public class CharacterBuilder : EditorWindow
{
    public GameObject basePrefab;
    public VisualTreeAsset vTA;
    GameObject mainComp;
    Animator animator;
    SpriteRenderer spriteRenderer;
    ObjectField spriteObjectField;
    CustomMono customMono;
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
        animatorNameTextField,
        animatorPathTextField;
    LayerMaskField collisionExcludeLayerMaskLayerMaskField;
    ListGameEffectBehavior listGameEffectBehavior;
    CollideAndDamageSO collideAndDamageSO;
    VisualElement scrollViewContent,
        seperator1;
    DropdownField addColliderDropdownField;
    GameObject build;
    int combatColliderLayer;

    private void OnEnable()
    {
        combatColliderLayer = LayerMask.NameToLayer("CombatCollider");
    }

    [MenuItem("Tools/CharacterBuilder")]
    private static void ShowWindow()
    {
        var window = GetWindow<CharacterBuilder>();
        window.titleContent = new GUIContent("CharacterBuilder");
        window.Show();
    }

    void CreateGUI()
    {
        GetVisualElements();
        AddHandler();
    }

    private void AddHandler()
    {
        buildButton.clicked += Build;
    }

    void GetVisualElements()
    {
        vTA.CloneTree(rootVisualElement);
        nameTextField = rootVisualElement.Q<TextField>("name-tf");
        spriteObjectField = rootVisualElement.Q<ObjectField>("sprite-of");
        animatorNameTextField = rootVisualElement.Q<TextField>("animator-name-tf");
        animatorPathTextField = rootVisualElement.Q<TextField>("animator-path-tf");
        buildButton = rootVisualElement.Q<Button>("build-b");
    }

    void Build()
    {
        build = Instantiate(basePrefab);
        build.name = nameTextField.text;
        mainComp = build.transform.Find("DirectionModifier/MainComponent").gameObject;
        customMono = build.GetComponent<CustomMono>();

        CharAttackInfo charAttackInfo = CreateInstance<CharAttackInfo>();
        charAttackInfo.name = nameTextField.text + "CAI";
        customMono.charAttackInfo = charAttackInfo;

        animator = mainComp.GetComponent<Animator>();
        var templateAC = animator.runtimeAnimatorController;
        string srcPath = AssetDatabase.GetAssetPath(templateAC);
        Debug.Log(srcPath);
        AssetDatabase.CopyAsset(
            srcPath,
            "Assets/Game/Graphics/Aseprite/Char/"
                + animatorPathTextField.text
                + "/"
                + animatorNameTextField.text
                + ".controller"
        );

        var t_animatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
            "Assets/Game/Graphics/Aseprite/Char/"
                + animatorPathTextField.text
                + "/"
                + animatorNameTextField.text
                + ".controller"
        );

        animator.runtimeAnimatorController = t_animatorController;

        spriteRenderer = mainComp.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = spriteObjectField.value as Sprite;

        AssetDatabase.CreateAsset(
            charAttackInfo,
            "Assets/Game/Resources/AttackInfo/" + charAttackInfo.name + ".asset"
        );
        PrefabUtility.SaveAsPrefabAssetAndConnect(
            build,
            "Assets/Game/Resources/Champion/" + build.name + ".prefab",
            InteractionMode.UserAction
        );
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
