#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class AddSpriteToParticleSystem : EditorWindow
{
    [MenuItem("Tools/AddSpriteToParticleSystem")]
    private static void ShowWindow()
    {
        var window = GetWindow<AddSpriteToParticleSystem>();
        window.titleContent = new GUIContent("AddSpriteToParticleSystem");
        window.Show();
    }

    void CreateGUI()
    {
        ObjectField particleSystemOF = new("ParticleSystem");
        InspectorElement listSpriteIE = new();
        var listSpriteSO = CreateInstance<ListSpriteSO>();
        listSpriteIE.Bind(new SerializedObject(listSpriteSO));
        Button button = new();
        button.text = "ADD";
        button.clicked += () =>
        {
            var textureSheetAnimationModule = ((GameObject)particleSystemOF.value)
                .GetComponent<ParticleSystem>()
                .textureSheetAnimation;
            listSpriteSO.sprites.ForEach(sSO => textureSheetAnimationModule.AddSprite(sSO));
        };

        rootVisualElement.Add(particleSystemOF);
        rootVisualElement.Add(listSpriteIE);
        rootVisualElement.Add(button);
    }
}
#endif
