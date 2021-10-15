using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(ParticleData))]
public class ParticleDataEditor : Editor
{
    private string newType;

    private const string pathToParticleTypeEnumKey = "particleTypePath";
    private TextAsset ParticleTypeFile;
    private string particleTypeAssetPath => ParticleTypeFile != null ? AssetDatabase.GetAssetPath(ParticleTypeFile) : "";

    System.Action delayedCall;

    public void OnEnable()
    {
        var path = EditorPrefs.GetString(pathToParticleTypeEnumKey, default);
        if (path != "") 
            ParticleTypeFile = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

        newType = default;
    }
    public void OnDisable()
    {
        EditorPrefs.SetString(pathToParticleTypeEnumKey, particleTypeAssetPath);
        newType = default;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(50);

        GUILayout.Label("Path to enum asset:");
        ParticleTypeFile = (TextAsset)EditorGUILayout.ObjectField("ParticleTypeFile", ParticleTypeFile, typeof(TextAsset), false);
        GUILayout.Label(particleTypeAssetPath);

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("New ParticleType", GUILayout.MaxWidth(150));
        newType = GUILayout.TextField(newType);
        GUILayout.EndHorizontal();


        if (GUILayout.Button("Add new ParticleType"))
        {
            if (particleTypeAssetPath == default)
            {
                Debug.LogError("Path to particle type .cs file is not set");
                return;
            }
            if (!RuntimeEnumEditor.ValidateFilePath(particleTypeAssetPath, typeof(ParticleType)))
            {
                Debug.LogError("Path to particle type .cs file is not valid");
                return;
            }

            if (newType == default)
            {
                Debug.LogError("New ParticleType is invalid");
                return;
            }

            if (!RuntimeEnumEditor.ValidateEnumAddition(typeof(ParticleType), newType))
            {
                Debug.LogError($"ParticleType already contains {newType}");
                return;
            }

            var path = AssetDatabase.GetAssetPath(ParticleTypeFile);

            RuntimeEnumEditor.AddEnum(particleTypeAssetPath, newType);

            var nt = newType;
            delayedCall = () => DelayedAddEnum(target, nt);
            EditorApplication.delayCall += DelayCall.ByNumberOfEditorFrames(2, delayedCall);
            AssetDatabase.Refresh();

            newType = default;
        }
    }

    private void DelayedAddEnum(Object target, string newType)
    {
        Debug.LogError($"DelayedAddEnum {newType}");
        if (newType != default)
        {
            Debug.LogError($"Adding {newType}");
            var data = (ParticleData)target;
            data.Particles.Add((ParticleType)System.Enum.Parse(typeof(ParticleType), newType), default);
            AssetDatabase.Refresh();
        }
    }
}

public static class DelayCall
{
    public static EditorApplication.CallbackFunction ByNumberOfEditorFrames(int n, System.Action a)
    {
        EditorApplication.CallbackFunction callback = null;

        callback = new EditorApplication.CallbackFunction(() =>
        {
            if (n-- <= 0)
            {
                a();
            }
            else
            {
                EditorApplication.delayCall += callback;
            }
        });

        return callback;
    }
}
