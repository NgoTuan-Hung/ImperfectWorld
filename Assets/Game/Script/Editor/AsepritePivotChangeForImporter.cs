#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class GameObjectInjector : AssetPostprocessor
{
    void OnPreprocessAsset()
    {
        if (assetImporter is AsepriteImporter aseImporter)
            aseImporter.OnPostAsepriteImport += OnPostAsepriteImport;
    }

    static AsepriteImporter.ImportEventArgs myArgs;
    public static BoolSO boolSO;

    static void OnPostAsepriteImport(AsepriteImporter.ImportEventArgs args)
    {
        boolSO = Resources.Load<BoolSO>("Misc/BoolSO");
        if (boolSO.value)
            return;
        myArgs = args;
        EditorApplication.delayCall += Delay;
    }

    static void Delay()
    {
        EditorApplication.delayCall -= Delay;
        var importer = myArgs.importer;
        var assetPath = myArgs.importer.assetPath;
        boolSO.value = true;

        // pick backing serialized field depending on importMode
        string fieldName = importer.importMode switch
        {
            FileImportModes.SpriteSheet => "m_SpriteSheetImportData",
            FileImportModes.TileSet => "m_TileSetImportData",
            _ => "m_AnimatedSpriteImportData",
        };

        var so = new SerializedObject(importer);
        var arrayProp = so.FindProperty(fieldName);
        if (arrayProp == null)
        {
            Debug.LogWarning(
                $"[AsepritePatcher] could not find property '{fieldName}' on importer for {assetPath}"
            );
            return;
        }

        if (arrayProp.arraySize == 0)
        {
            Debug.Log($"[AsepritePatcher] no sprites to patch in {assetPath}");
            return;
        }

        int heightChange = 8;
        for (int i = 0; i < arrayProp.arraySize; i++)
        {
            var elem = arrayProp.GetArrayElementAtIndex(i);

            // SpriteMetaData likely has fields: name, rect, alignment, pivot, border, spriteID, uvTransform
            // try both possible serialized names to be defensive
            var pivotProp =
                elem.FindPropertyRelative("pivot") ?? elem.FindPropertyRelative("m_Pivot");
            var rectProp = elem.FindPropertyRelative("rect") ?? elem.FindPropertyRelative("m_Rect");

            pivotProp.vector2Value = new Vector2(
                pivotProp.vector2Value.x,
                pivotProp.vector2Value.y + (heightChange / rectProp.rectValue.height)
            );
        }

        // Save modified serialized props back to importer
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(importer);

        // Persist settings and trigger one reimport so Unity recreates sprite sub-assets with new names/pivots.
        AssetDatabase.WriteImportSettingsIfDirty(assetPath);
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

        Debug.Log($"[AsepritePatcher] patched and reimported {assetPath}");
    }
}
#endif
