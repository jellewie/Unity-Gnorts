using System;
using System.Collections;
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

/*
    SpecialBuild
    1 Wall (Can move up and down)
    2 Stair (Will move up and down by its surroundings)
    3 FirePit (Can only be build on top of: Stone_Wall || Stone_Gate || Stone_Tower)

    SpecialClick (Please check BuildingPopUp.cs in the code 'if (PopUp)' to what options we have
    1 Gate  (gate status) 
    2 Keep  (tax rate) Default:3-no tax
    3 Ox_Transport (only move this resource) 254 = not set yet
    4 Lumberjack_Hut || Stone_Quarry || Iron_Mine  (How to move the products)
    5 Granary (Rations)[also Needs a new window] See whats in the Granary - ?ban certain food to be consumed?
    6 Armory                [Needs a new window] See whats in the Armory
    7 Barracks              [Needs a new window] Build troops (with auto buy tools option)
    8 Stockpile             [Needs a new window] See whats in the stockpile
    9 Trading_House         [Needs a new window] 
    10 Church               [Needs a new window] Bribe/donate
*/