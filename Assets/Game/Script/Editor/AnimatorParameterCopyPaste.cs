#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Provides copy and paste functionality for AnimatorController parameters in the Unity Editor.
/// </summary>
/// <remarks>
/// <para>Usage Instructions:</para>
/// <list type="bullet">
/// <item>Right-click an AnimatorController asset in the Project window or Inspector.</item>
/// <item>Select <b>Copy Parameters</b> to copy all parameters to the clipboard.</item>
/// <item>Select <b>Paste Parameters</b> to paste parameters from the clipboard.</item>
/// <item>Use <b>Paste Parameters (Additive)</b> to add parameters without removing existing ones.</item>
/// <item>Use <b>Paste Parameters (Replace)</b> to replace all parameters with those from the clipboard.</item>
/// </list>
/// <para>This script must be placed in an <c>Editor</c> folder.</para>
/// <para><b>Note:</b> Trigger parameters do not have default values. When copying and pasting, triggers are preserved by name and type, but their state is not settable or serialized, which is by design in Unity.</para>
/// </remarks>
public static class AnimatorParameterCopyPaste
{
    #region Constants

    /// <summary>Menu path for copying parameters from the Inspector context menu.</summary>
    private const string CopyMenu = "CONTEXT/AnimatorController/Copy Parameters";
    /// <summary>Menu path for additive paste in the Inspector context menu.</summary>
    private const string PasteMenuAdditive = "CONTEXT/AnimatorController/Paste Parameters (Additive)";
    /// <summary>Menu path for replace paste in the Inspector context menu.</summary>
    private const string PasteMenuReplace = "CONTEXT/AnimatorController/Paste Parameters (Replace)";
    /// <summary>Menu path for additive paste in the Project window context menu.</summary>
    private const string AssetsPasteMenuAdditive = "Assets/Animator Controller/Paste Parameters (Additive)";
    /// <summary>Menu path for replace paste in the Project window context menu.</summary>
    private const string AssetsPasteMenuReplace = "Assets/Animator Controller/Paste Parameters (Replace)";

    #endregion

    #region Inspector Context Menu

    /// <summary>
    /// Copies all parameters from the selected AnimatorController to the system clipboard as JSON.
    /// </summary>
    /// <param name="command">The menu command containing the AnimatorController context.</param>
    [MenuItem(CopyMenu)]
    private static void CopyParameters(MenuCommand command)
    {
        var controller = command.context as AnimatorController;
        if (controller == null) return;

        var parameters = controller.parameters.Select(p => new AnimatorParameterData
        {
            name = p.name,
            type = p.type,
            defaultBool = p.defaultBool,
            defaultFloat = p.defaultFloat,
            defaultInt = p.defaultInt
        }).ToList();

        var json = JsonUtility.ToJson(new AnimatorParameterList { parameters = parameters });
        EditorGUIUtility.systemCopyBuffer = json;
        Debug.Log("Animator parameters copied to clipboard.");
    }

    /// <summary>
    /// Pastes parameters additively or by replacing, depending on the menu selection.
    /// </summary>
    /// <param name="command">The menu command containing the AnimatorController context.</param>
    [MenuItem(PasteMenuAdditive)]
    private static void PasteParametersAdditive(MenuCommand command)
    {
        PasteParametersInternal(command, false);
    }

    /// <summary>
    /// Pastes parameters by replacing all existing parameters.
    /// </summary>
    /// <param name="command">The menu command containing the AnimatorController context.</param>
    [MenuItem(PasteMenuReplace)]
    private static void PasteParametersReplace(MenuCommand command)
    {
        PasteParametersInternal(command, true);
    }

    #endregion

    #region Project Window Context Menu

    /// <summary>
    /// Copies parameters from the selected AnimatorController asset in the Project window.
    /// </summary>
    [MenuItem("Assets/Animator Controller/Copy Parameters", false, 2000)]
    private static void CopyParametersAsset()
    {
        var controller = Selection.activeObject as AnimatorController;
        if (controller == null) return;
        CopyParameters(new MenuCommand(controller));
    }

    /// <summary>
    /// Pastes parameters additively into the selected AnimatorController asset in the Project window.
    /// </summary>
    [MenuItem(AssetsPasteMenuAdditive, false, 2001)]
    private static void PasteParametersAssetAdditive()
    {
        var controller = Selection.activeObject as AnimatorController;
        if (controller == null) return;
        PasteParametersInternal(new MenuCommand(controller), false);
    }

    /// <summary>
    /// Pastes parameters by replacing all existing parameters in the selected AnimatorController asset in the Project window.
    /// </summary>
    [MenuItem(AssetsPasteMenuReplace, false, 2002)]
    private static void PasteParametersAssetReplace()
    {
        var controller = Selection.activeObject as AnimatorController;
        if (controller == null) return;
        PasteParametersInternal(new MenuCommand(controller), true);
    }

    #endregion

    #region Core Logic

    /// <summary>
    /// Validates that the clipboard JSON is a valid AnimatorParameterList structure.
    /// </summary>
    private static bool IsValidParameterJson(string json)
    {
        if (string.IsNullOrEmpty(json)) return false;
        // Quick structure check: must contain "parameters" array
        if (!json.Contains("\"parameters\"")) return false;
        // Optionally, check for at least one parameter object
        if (!json.Contains("\"name\"") || !json.Contains("\"type\"")) return false;
        return true;
    }

    /// <summary>
    /// Attempts to deserialize and validate the parameter list from JSON.
    /// </summary>
    private static bool TryGetParameterList(string json, out AnimatorParameterList paramList)
    {
        paramList = null;
        if (!IsValidParameterJson(json))
            return false;

        try
        {
            var deserialized = JsonUtility.FromJson<AnimatorParameterList>(json);
            // Validate structure
            if (deserialized == null || deserialized.parameters == null)
                return false;
            // Validate each parameter
            foreach (var p in deserialized.parameters)
            {
                if (string.IsNullOrEmpty(p.name)) return false;
                // Type must be a valid enum value
                if (!System.Enum.IsDefined(typeof(AnimatorControllerParameterType), p.type))
                    return false;
            }
            paramList = deserialized;
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Internal method to paste parameters into an AnimatorController, either additively or by replacing all.
    /// </summary>
    /// <param name="command">The menu command containing the AnimatorController context.</param>
    /// <param name="replace">If true, replaces all parameters; otherwise, adds new ones only.</param>
    private static void PasteParametersInternal(MenuCommand command, bool replace)
    {
        var controller = command.context as AnimatorController;
        if (controller == null) return;

        var json = EditorGUIUtility.systemCopyBuffer;
        if (!TryGetParameterList(json, out var paramList))
        {
            Debug.LogWarning("Clipboard does not contain valid animator parameters.");
            return;
        }

        Undo.RecordObject(controller, replace ? "Replace Animator Parameters" : "Paste Animator Parameters");

        if (replace)
        {
            // Remove all existing parameters from the end for safety
            for (int i = controller.parameters.Length - 1; i >= 0; i--)
            {
                controller.RemoveParameter(i);
            }
        }

        foreach (var param in paramList.parameters)
        {
            if (!replace && controller.parameters.Any(p => p.name == param.name)) continue; // Avoid duplicates in additive mode

            controller.AddParameter(param.name, param.type);

            // Set default value
            var addedParam = controller.parameters.FirstOrDefault(p => p.name == param.name);
            if (addedParam != null)
            {
                switch (param.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        addedParam.defaultBool = param.defaultBool;
                        break;
                    case AnimatorControllerParameterType.Float:
                        addedParam.defaultFloat = param.defaultFloat;
                        break;
                    case AnimatorControllerParameterType.Int:
                        addedParam.defaultInt = param.defaultInt;
                        break;
                        // Triggers do not have a default value
                }
            }
        }

        Debug.Log(replace ? "Animator parameters replaced." : "Animator parameters pasted.");
    }

    #endregion

    #region Data Classes

    /// <summary>
    /// Serializable list wrapper for animator parameters.
    /// </summary>
    [System.Serializable]
    private class AnimatorParameterList
    {
        /// <summary>
        /// The list of animator parameters.
        /// </summary>
        public List<AnimatorParameterData> parameters;
    }

    /// <summary>
    /// Serializable data structure for an animator parameter.
    /// </summary>
    [System.Serializable]
    private class AnimatorParameterData
    {
        /// <summary>The name of the parameter.</summary>
        public string name;
        /// <summary>The type of the parameter.</summary>
        public AnimatorControllerParameterType type;
        /// <summary>The default value if the parameter is a bool.</summary>
        public bool defaultBool;
        /// <summary>The default value if the parameter is a float.</summary>
        public float defaultFloat;
        /// <summary>The default value if the parameter is an int.</summary>
        public int defaultInt;
    }

    #endregion
}
#endif