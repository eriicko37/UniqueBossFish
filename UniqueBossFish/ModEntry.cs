using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Locations;
using StardewValley.Menus;

namespace EricSV.UniqueBossFish;

public sealed class ModEntry : Mod
{
    private bool challengeBaitWarningShown;

    public override void Entry(IModHelper helper)
    {
        helper.Events.Content.AssetRequested += this.OnAssetRequested;
        helper.Events.Display.MenuChanged += this.OnMenuChanged;

        helper.ConsoleCommands.Add(
            "ubf_scan",
            "List all currently loaded Data/Locations fish spawns with IsBossFish=true and their CatchLimit.",
            this.OnScanCommand
        );

        this.Monitor.Log(
            "Unique Boss Fish loaded. Standard Data/Locations entries with IsBossFish=true will be forced to CatchLimit=1.",
            LogLevel.Info
        );
    }

    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo("Data/Locations"))
            return;

        e.Edit(
            asset =>
            {
                IDictionary<string, LocationData> locations =
                    asset.AsDictionary<string, LocationData>().Data;

                int bossEntries = 0;
                int changedEntries = 0;

                foreach (KeyValuePair<string, LocationData> locationPair in locations)
                {
                    LocationData location = locationPair.Value;
                    if (location.Fish is null)
                        continue;

                    foreach (SpawnFishData spawn in location.Fish)
                    {
                        if (!spawn.IsBossFish)
                            continue;

                        bossEntries++;

                        if (spawn.CatchLimit != 1)
                        {
                            spawn.CatchLimit = 1;
                            changedEntries++;
                        }
                    }
                }

                this.Monitor.Log(
                    $"Checked {bossEntries} boss-fish spawn entries in Data/Locations; "
                    + $"{changedEntries} were changed to CatchLimit=1.",
                    LogLevel.Trace
                );
            },
            AssetEditPriority.Late
        );
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        if (e.NewMenu is not BobberBar bobberBar || !bobberBar.bossFish)
            return;

        try
        {
            if (bobberBar.challengeBaitFishes > 1)
            {
                bobberBar.challengeBaitFishes = 1;

                this.Monitor.Log(
                    "Boss fish detected in BobberBar: Challenge Bait fish count capped to 1.",
                    LogLevel.Trace
                );
            }
        }
        catch (Exception ex)
        {
            if (this.challengeBaitWarningShown)
                return;

            this.challengeBaitWarningShown = true;
            this.Monitor.Log(
                "Couldn't cap Challenge Bait for a boss fish. CatchLimit=1 protection is still active, "
                + "but Challenge Bait protection is pending verification.\n"
                + ex,
                LogLevel.Warn
            );
        }
    }

    private void OnScanCommand(string command, string[] args)
    {
        try
        {
            IDictionary<string, LocationData> locations =
                this.Helper.GameContent.Load<Dictionary<string, LocationData>>("Data/Locations");

            int count = 0;

            foreach (KeyValuePair<string, LocationData> locationPair in locations)
            {
                LocationData location = locationPair.Value;
                if (location.Fish is null)
                    continue;

                foreach (SpawnFishData spawn in location.Fish)
                {
                    if (!spawn.IsBossFish)
                        continue;

                    count++;

                    this.Monitor.Log(
                        $"BOSS | Location={locationPair.Key} | Id={spawn.Id ?? "(no Id)"} | "
                        + $"ItemId={spawn.ItemId ?? "(no ItemId)"} | CatchLimit={spawn.CatchLimit}",
                        LogLevel.Info
                    );
                }
            }

            this.Monitor.Log(
                $"ubf_scan complete: {count} IsBossFish=true spawn entries found.",
                LogLevel.Info
            );
        }
        catch (Exception ex)
        {
            this.Monitor.Log($"ubf_scan failed:\n{ex}", LogLevel.Error);
        }
    }
}
