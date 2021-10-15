using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class RuntimeEnumEditor
{
    private const string Tab = "    ";

    /// <summary>
    /// checks if file exists and contains enum of type: particleType
    /// </summary>
    /// <param name="path"></param>
    /// <param name="enumType"></param>
    /// <returns></returns>
    public static bool ValidateFilePath(string path, Type enumType)
    {
        if (!File.Exists(path))
            return false;

        var file = File.ReadAllText(path);

        if (!file.Contains($"public enum {enumType.Name}"))
            return false;

        return true;
    }

    /// <summary>
    /// Checks if a new enum type newType can be added to the enum
    /// </summary>
    /// <param name="enumType"></param>
    /// <param name="newType"></param>
    /// <returns></returns>
    public static bool ValidateEnumAddition(Type enumType, string newType)
    {
        foreach (var name in Enum.GetNames(enumType))
        {
            if (name == newType)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Adds enum value of newEnum to file at enumFilePath
    /// </summary>
    /// <param name="enumFilePath"></param>
    /// <param name="newEnum"></param>
    public static void AddEnum(string enumFilePath, string newEnum, bool refreshAssets = false)
    {
        var file = File.ReadAllText(enumFilePath);

        var indexOfLastComma = file.LastIndexOf(",");
        var indexOfLastEquals = file.LastIndexOf("=");

        var enumValueSubstring = file.Substring(indexOfLastEquals + 1, indexOfLastComma - indexOfLastEquals - 1).Trim();

        if (!int.TryParse(enumValueSubstring, out int lastEnumValue))
        {
            Debug.LogError("Could not read last value of enum");
            return;
        }

        file = file.Insert(indexOfLastComma + 1, $"\n{Tab}{newEnum} = {lastEnumValue + 1},");

        File.WriteAllText(enumFilePath, file);

        if(refreshAssets)
            AssetDatabase.Refresh();
    }
}
