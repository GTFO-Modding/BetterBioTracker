using AssetShards;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using BetterBioTracker.Comps;
using GTFO.API;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Linq;
using UnityEngine;

namespace BetterBioTracker;
[BepInPlugin("BetterBioTracker", "BetterBioTracker", VersionInfo.Version)]
[BepInDependency("dev.gtfomodding.gtfo-api", BepInDependency.DependencyFlags.HardDependency)]
internal class EntryPoint : BasePlugin
{
    private Harmony _Harmony = null;

    public override void Load()
    {
        CFG.BindAll(Config);

        ClassInjector.RegisterTypeInIl2Cpp<RadarScreenItem>();
        ClassInjector.RegisterTypeInIl2Cpp<BioScreenRef>();
        ClassInjector.RegisterTypeInIl2Cpp<BetterBio>();

        _Harmony = new Harmony($"{VersionInfo.RootNamespace}.Harmony");
        _Harmony.PatchAll();

        AssetAPI.OnStartupAssetsLoaded += AssetAPI_OnStartupAssetsLoaded;
    }

    private void AssetAPI_OnStartupAssetsLoaded()
    {
        var resourceShards = new string[]
        {
            AssetShardManager.GetShardName(AssetBundleName.Gear_Tool_Screen, AssetBundleShard.S2),
            AssetShardManager.GetShardName(AssetBundleName.Gear_Weapon_Sight, AssetBundleShard.S3)
        };
        AssetShardManager.LoadMultipleShardsAsync(resourceShards, (Il2CppSystem.Action)OnGearShardsLoaded);
    }

    private void OnGearShardsLoaded()
    {
        BioScreenRef.LoadResources();
    }

    public override bool Unload()
    {
        _Harmony.UnpatchSelf();
        return base.Unload();
    }
}
