using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(ParticleData))]
public class ParticleDataEditor : Editor
{
    private string GetEditorPrefsKey(string key) => $"{name}.{key}";

    private string NewType;

    private TextAsset ParticleTypeFile;
    private string ParticleTypeAssetPath => ParticleTypeFile != null ? AssetDatabase.GetAssetPath(ParticleTypeFile) : "";

    public void OnEnable()
    {
        RawDataStart = EditorPrefs.GetInt(GetEditorPrefsKey("RawDataStart"), 0);
        var path = EditorPrefs.GetString(GetEditorPrefsKey("ParticleTypeFile"), default);
        if (path != "") 
            ParticleTypeFile = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

        NewType = default;
    }
    public void OnDisable()
    {
        EditorPrefs.SetInt(GetEditorPrefsKey("RawDataStart"), RawDataStart);
        EditorPrefs.SetString(GetEditorPrefsKey("ParticleTypeFile"), ParticleTypeAssetPath);
        NewType = default;
    }

    public override void OnInspectorGUI()
    {
        ParticleTypeFile = (TextAsset)EditorGUILayout.ObjectField("Path to enum asset:", ParticleTypeFile, typeof(TextAsset), false);
        GUILayout.Label(ParticleTypeAssetPath);
        GUILayout.Space(10);
        ShowSyncButton();
        GUILayout.Space(10);
        ShowAddButton();

        GUILayout.Space(20);
        ShowRawData();
    }

    private int RawDataStart = 0;
    private int RawDataShowCount = 10;
    private void ShowRawData()
    {
        var ParticleData = ((ParticleData)target);

        EditorGUILayout.BeginHorizontal();

        GUILayout.Space(20f);

        if (GUILayout.Button("<-", EditorStyles.miniButton))
        {
            RawDataStart = Mathf.Clamp(RawDataStart - RawDataShowCount, 0, ParticleData.ParticleDataRaw.Count - 1);
        }

        var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        int prevEnd = Mathf.Clamp(RawDataStart + RawDataShowCount, 0, ParticleData.ParticleDataRaw.Count);
        EditorGUILayout.LabelField($"Showing: {RawDataStart}-{prevEnd}", style);

        if (GUILayout.Button("->", EditorStyles.miniButton))
        {
            RawDataStart = Mathf.Clamp(RawDataStart + RawDataShowCount, 0, ParticleData.ParticleDataRaw.Count - 2);
        }
        GUILayout.Space(20f);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10f);


        int end = Mathf.Clamp(RawDataStart + RawDataShowCount, 0, ParticleData.ParticleDataRaw.Count);
        for (int i = RawDataStart; i < end; i++)
        {
            var kvp = ParticleData.ParticleDataRaw[i];

            var r = EditorGUILayout.GetControlRect(false, 1);
            r.height = 1;
            EditorGUI.DrawRect(r, Color.black);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("-", EditorStyles.miniButton))
            {
                ParticleData.ParticleDataRaw.RemoveAt(i);
                RuntimeEnumEditor.RemoveEnum(ParticleTypeAssetPath, kvp.Key);
                EditorUtility.SetDirty(target);
                end--;
            }
            EditorGUILayout.BeginVertical();
            EditorGUI.LabelField(EditorGUILayout.GetControlRect(), $"ID:\t{kvp.Key}");
            var v = (ParticleSystem)EditorGUILayout.ObjectField("Value:", kvp.Value, typeof(ParticleSystem), false);
            if (v != kvp.Value)
            {
                kvp.Value = v;
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }
    }

    private void ShowSyncButton()
    {
        var ParticleData = ((ParticleData)target);
        if (GUILayout.Button("Sync from enum"))
        {
            foreach (var e in System.Enum.GetValues(typeof(ParticleType)))
            {
                string key = e.ToString();
                if (!ParticleData.ParticleDataRaw.Any((kvp) => kvp.Key == key))
                {
                    var newElem = new ParticleData.ParticleKVP { Key = key, Value = null };
                    ParticleData.ParticleDataRaw.Add(newElem);
                }
            }
            EditorUtility.SetDirty(target);
        }
    }

    private void ShowAddButton()
    {
        var ParticleData = ((ParticleData)target);

        GUILayout.BeginHorizontal();
        GUILayout.Label("New ParticleType", GUILayout.MaxWidth(150));
        NewType = GUILayout.TextField(NewType);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Add new ParticleType"))
        {
            if (ParticleTypeAssetPath == default)
            {
                Debug.LogError("Path to particle type .cs file is not set");
                return;
            }
            if (!RuntimeEnumEditor.ValidateFilePath(ParticleTypeAssetPath, typeof(ParticleType)))
            {
                Debug.LogError("Path to particle type .cs file is not valid");
                return;
            }

            if (NewType == default)
            {
                Debug.LogError("New ParticleType is invalid");
                return;
            }

            if (!RuntimeEnumEditor.ValidateEnumAddition(typeof(ParticleType), NewType))
            {
                Debug.LogError($"ParticleType already contains {NewType}");
                return;
            }

            RuntimeEnumEditor.AddEnum(ParticleTypeAssetPath, NewType);

            AssetDatabase.Refresh();

            var newElem = new ParticleData.ParticleKVP { Key = NewType, Value = null };
            ParticleData.ParticleDataRaw.Add(newElem);

            NewType = default;
            EditorUtility.SetDirty(target);
        }
    }
}