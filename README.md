# Unique Boss Fish

SMAPI mod for Stardew Valley 1.6.

## What it does

- Finds every standard fishing spawn in `Data/Locations` with `IsBossFish = true`.
- Forces that spawn to `CatchLimit = 1`, using Stardew Valley's native per-player catch limit.
- If the active fishing minigame is a boss fish, caps Challenge Bait to one fish so it can't award multiple copies in the same catch.
- Uses no fish-name list and doesn't depend on the informational `fish_legendary` context tag.
- Includes the SMAPI console command `ubf_scan` to list detected boss-fish spawns and their catch limits.

## Compatibility target

Designed for Stardew Valley 1.6.x + SMAPI 4.x and mods which add fish through the standard `Data/Locations` system.

Code-driven custom fishing systems which bypass `Data/Locations` or the vanilla `BobberBar` need separate compatibility testing.

## Build

The repository builds automatically with GitHub Actions using Pathoschild's SMAPI Mod Build Workflow.
