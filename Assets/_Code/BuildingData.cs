using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PublicCode;

/// <summary>
/// A repository of information about building types.
/// </summary>
public class BuildingData
{
    private const string FilePath = @"Assets\_Data\buildings.json";
    private static Dictionary<string, BuildingInfo> _data;

    /// <summary>
    /// Load building data from file.
    /// </summary>
    private static void LoadData()
    {
        _data = new Dictionary<string, BuildingInfo>();

        if (!File.Exists(FilePath))
        {
            throw new Exception("Required file not found: " + FilePath);
        }

        string json = File.ReadAllText(FilePath);
        BuildingInfo[] buildings = JsonHelper.GetJsonArray<BuildingInfo>(json);

        foreach (var building in buildings)
        {
            _data.Add(building.Name, building);
        }

        Debug.Log("Imported " + buildings.Length + " buildings");
    }

    /// <summary>
    /// Get information about a type of building.  
    /// </summary>
    /// <param name="name">The name of the building type</param>
    /// <returns></returns>
    public static BuildingInfo GetInfo(string name)
    {
        if (_data == null)
            LoadData();
        BuildingInfo info;
        if (_data != null && _data.TryGetValue(name, out info))
        {
            return info;
        }

        Debug.LogWarning("BuildingData::GetInfo -- No building named: '" + name + "'"); //Log error, object not found
        return new BuildingInfo();
    }

    /// <summary>
    /// A helper class to deal with the fact that top-level arrays are not understood by Unity's JsonUtility.
    /// </summary>
    public static class JsonHelper
    {
        public static T[] GetJsonArray<T>(string json)
        {
            return JsonUtility.FromJson<Wrapper<T>>("{ \"Array\": " + json + "}").Array;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Array;
        }
    }
}

/// <summary>
/// Specification of how much of each resource is needed.
/// </summary>
[Serializable]
public struct Cost
{
    public int Wood;
    public int Stone;
    public int Iron;
    public int Gold;
}

/// <summary>
/// Data type to store buildings build price and settings.
/// </summary>
[Serializable]
public struct BuildingInfo
{
    /// <summary>
    /// The name of the building.
    /// </summary>
    public string Name;

    /// <summary>
    /// How much of each resource it costs to build.
    /// </summary>
    public Cost Cost;

    /// <summary>
    /// If the first one is free is you can't pay for it (Stockpile, woodcutter)
    /// </summary>
    public bool FirstFree;

    /// <summary>
    /// If this building has special build code (like walls; higher lower)
    /// </summary>
    public BuildType BuildType;

    /// <summary>
    /// If this building has special click code (like gates; open/close)
    /// </summary>
    public byte ClickSpecial;

    /// <summary>
    /// If this building has special destroy code
    /// </summary>
    public byte DestroySpecial;
}