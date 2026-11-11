# GitHub Copilot Instructions for Prisoner Arena (Continued)

## Mod Overview and Purpose

The Prisoner Arena (Continued) mod is an update to Gl0b's original Prisoner Arena mod for RimWorld. The mod allows for fights between prisoners, slaves, animals, mechs, and mutants. Spectators can watch the fights and build fences around the fighting area. The mod also includes a scoreboard to track the names of all winners and the option to release winning prisoners as rewards.

## Key Features and Systems

- **Fight Duration**: Fights can now continue until death.
- **Scoreboard**: Keeps track of all the winners.
- **Reward System**: Winning prisoners can be released as rewards in addition to gaining glory.
- **Spectators**: Spectators can watch the fights from outside the gray area, allowing for the construction of fences around the fighters.
- **Fighter Selection**: Animals, slaves, mechs, and mutants can be selected as fighters.
- **Language Support**: Russian translation added.

## Coding Patterns and Conventions

- **C# Classes and Methods**: Follow standard C# naming conventions and structure.
- **XML Definitions**: Use XML files to define new duties, interactions, jobs, mental states, and thing definitions.
- **Harmony Patching**: Use Harmony to patch existing RimWorld code to integrate new features.

## XML Integration

The mod uses XML files to define various aspects of the game, such as new duties, interactions, jobs, mental states, and thing definitions. These XML files are located in the `Defs` directory of the mod.

Example XML file (`Defs/DutyDefs/Duties_Arena_Gatherings.xml`):
xml
<DutyDef>
    <defName>ArenaGathering</defName>
    <label>Arena Gathering</label>
    <description>Prisoners are gathered for a fight.</description>
    <pawnSkill>Combat</pawnSkill>
</DutyDef>


## Harmony Patching

The mod uses Harmony to patch existing RimWorld code to integrate new features. Harmony is a library that allows for the modification of game code without directly editing the original binaries.

Example Harmony patch (`Building_Bell.cs`):
csharp
[HarmonyPatch(typeof(Pawn), "GetGatherSpot", MethodType.Getter)]
public static class Pawn_GetGatherSpot_Patch
{
    [HarmonyPostfix]
    public static void Postfix(Pawn __instance, ref IntVec3 __result)
    {
        if (__instance.IsPrisoner())
        {
            __result = ArenaManager.GetGatherSpot(__instance);
        }
    }
}


## Suggestions for Copilot

- **Provide XML Examples**: When generating XML definitions, provide examples of existing definitions in the mod.
- **Use Harmony Patches**: When generating code to modify existing RimWorld functionality, suggest using Harmony patches.
- **Follow Naming Conventions**: Ensure that generated code follows standard C# naming conventions and structures.
- **Integrate with XML**: When generating C# code that interacts with XML definitions, ensure that the code correctly references and uses the XML data.

By following these guidelines, Copilot will be able to generate code that is consistent with the mod's style and functionality.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

