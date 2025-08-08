# NoBreakdowns

Stops powered machinery that was built with components from ever breaking down.

- Affects buildings that both:
  - Use power (have `CompPowerTrader`), and
  - Have components in their build cost (`ComponentIndustrial` or `ComponentSpacer`).
- Matching buildings will not enter the broken state.
- Already-broken matching buildings are automatically repaired on spawn/load.
 - Prevents random battery short-circuit ("Zzztt") incidents. Rain-caused short circuits still occur.
 - Prevents solar flare incidents.

## How it works
This mod uses Harmony patches:

- Prefix on `RimWorld.CompBreakdownable.DoBreakdown()`
  - Checks the parent building's `ThingDef.costList` for components and whether it uses power.
  - If both are true, returns `false` to skip the original breakdown method.
- Postfix on `RimWorld.CompBreakdownable.PostSpawnSetup(...)`
  - If a matching building is already broken (e.g., when loading a save), it immediately calls `Notify_Repaired()`.
 - Prefix on `RimWorld.IncidentWorker_ShortCircuit.TryExecuteWorker(...)`
   - Returns `false` to cancel random short-circuit incidents. Does not affect `IncidentWorker_ShortCircuitRain`.
 - Prefix on `RimWorld.IncidentWorker_SolarFlare.TryExecuteWorker(...)`
   - Returns `false` to cancel solar flare incidents entirely.

Files:
- `Source/HarmonyPatches/ModInit.cs`: Harmony bootstrapper.
- `Source/HarmonyPatches/NoBreakdownPatch.cs`: Skips breakdowns for qualifying buildings.
- `Source/HarmonyPatches/AutoRepairOnSpawnPatch.cs`: Auto-repairs qualifying buildings on spawn.
- `Source/HarmonyPatches/DisableIncidentsPatches.cs`: Disables random short-circuit and solar flare incidents.
- `Source/NoBreakdowns.csproj`: Project file targeting .NET Framework 4.7.2.

## Compatibility
- Safe to add to existing saves. Matching buildings will repair on next spawn/load tick.
- Safe to remove, but anything that would normally break down will resume vanilla behavior after removal.
- Should be compatible with most mods. Other mods that heavily modify `CompBreakdownable` could conflict if they also patch the same methods.

## Requirements
- RimWorld 1.6
- Harmony 2.x (bundled automatically as `0Harmony.dll` in `Assemblies/`).

## Installation
- Steam Workshop: Not applicable (local mod). Copy this folder `Mods/NoBreakdowns` into your RimWorld `Mods/` directory if needed and enable it in the Mod Manager.
- Manual: Ensure `Assemblies/NoBreakdowns.dll` exists (see Building from source below), then enable the mod.

## Load order
- Anywhere. Place after Harmony (if visible).

## Building from source (Windows)
This project targets .NET Framework 4.7.2 and references RimWorld managed assemblies. Recommended options:

1) Visual Studio (recommended)
- Open `Source/NoBreakdowns.sln` in Visual Studio 2019/2022.
- Configuration: `Release`.
- Build Solution.
- Output: `Mods/NoBreakdowns/Assemblies/NoBreakdowns.dll` (and `0Harmony.dll` copied post-build).

2) MSBuild (Developer Command Prompt)
- Open a "Developer Command Prompt for VS".
- Run:
  ```bat
  MSBuild "c:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\NoBreakdowns\Source\NoBreakdowns.sln" /p:Configuration=Release /v:m
  ```

Notes
- `dotnet build` (SDK-style) may not work if a matching .NET Framework MSBuild is not present. Prefer Visual Studio/MSBuild.
- If references fail, verify the hint paths in `Source/NoBreakdowns.csproj` point to your RimWorld `RimWorldWin64_Data/Managed/` folder.

## Known limitations
- Only prevents breakdowns for powered, component-built buildings. Items without power or without component costs use vanilla behavior.
 - Random battery short-circuits are disabled; rain-exposure short-circuits remain.
 - Solar flares are disabled entirely.

## FAQ
- Q: Does this affect maintenance decay or other failures?
  - A: It only targets `CompBreakdownable` breakdowns.
- Q: Will this refund components?
  - A: No. It prevents the breakdown from occurring.

## Credits
- Code: ShaneeexD
- Libraries: Harmony (pardeike)
