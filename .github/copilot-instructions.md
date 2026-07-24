# GitHub Copilot Instructions for RimWorld Mod Project

## Mod Overview and Purpose

This RimWorld mod introduces a new gameplay feature centered around organizing and managing arena fights between pawns. The purpose of this mod is to add a dynamic social and combat experience to the game, allowing players to host and participate in brawls. Players can use structures like a bell to start and manage fights, providing a fun and engaging way to use the in-game combat mechanics for entertainment and conflict resolution.

## Key Features and Systems

- **Building_Bell**: Central to the mod, this structure manages the initiation and control of brawls. It tracks the state of fights and manages interactions with fighters and spectators.
- **Fighter System**: Pawns can be designated as Fighters, which allows them to participate in arena events. Various routines, such as cheering and spectating, enhance the realism of these events.
- **Audience and Radial Effects**: The audience's reactions are governed by the radius around the arena, impacting mood and event outcomes.
- **Integration with RimWorld**: Utilizes the game's standard classes and mechanics, enhancing them with custom behavior for a seamless modded experience.

## Coding Patterns and Conventions

- **Class Naming**: Classes are named following their functional roles (e.g., `Building_Bell`, `JobDriver_Cheer`), ensuring clarity on their use and integration.
- **Method Naming**: Methods generally describe their operations (e.g., `StartTheShow`, `isBusy`), making it intuitive to follow the code's logic and intentions.
- **C# Conventions**: Adheres to typical C# coding standards with PascalCase for public methods and camelCase for private methods.

## XML Integration

Given the complexity of integrating with RimWorld's XML-based definitions, the following guidance is provided:

- **Ensure XML Definitions**: Properly define your custom jobs, interactions, thoughts, and mental states in XML to allow integration with the game's systems.
- **Error Handling**: Pay special attention to syntax and references within your XML files, as errors could prevent the mod from loading properly.

## Harmony Patching

- **Patching Essentials**: Use Harmony to patch existing RimWorld methods where modification is needed. This ensures the mod's functionality integrates smoothly without altering the base game files.
- **Patch Scope**: Only patch methods where necessary, keeping the scope limited to enhance compatibility with other mods.
- **Logging**: Implement logging for patches to facilitate debugging and troubleshooting, especially for complex interactions.

## Suggestions for Copilot

1. **Pattern Recognition**: Continuously suggest methods and classes based on repetitive patterns observed in similar game mods or RimWorld projects.
2. **Ensure Compatibility**: Promote code snippets that maintain compatibility with RimWorld's modding API and existing code conventions.
3. **XML Consistency**: Advocate for well-structured XML suggestions, particularly for defining new elements like `ThingDefs` and `JobDefs`.
4. **Harmony Patching**: Encourage safe and structured Harmony patch methodologies for maintaining compatibility and reducing errors.
5. **Documentation and Comments**: Suggest inline documentation and comments for complex logic to enhance code maintainability and clarity for future developers.

This instruction file serves as a foundation for mod developers utilizing GitHub Copilot to streamline the development process, ensuring a cohesive and stable modding experience.
