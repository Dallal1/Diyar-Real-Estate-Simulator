# Diyar: Property Simulator

## Context

- This is a 3D property simulation game inspired by traditional Syrian architecture, primarily set in Damascus and Aleppo.
- The player returns to Syria with little money, starts with simple jobs, then buys, renovates, and sells properties.
- The user handles programming; their sister handles art and animation.
- Camera perspective, time period, and the balance between renovation and business management are still open decisions.
- The folder scaffold is intended for an existing Unity project. Inspect ProjectVersion.txt and the package manifest before assuming a Unity version or render pipeline.

## Working with the user

- Always explain in Arabic using short, clear, beginner-friendly language.
- Use German for player-facing UI text. Keep code identifiers and tool names in English.
- Read existing files and instructions before making changes.
- Work on one small step at a time. Explain its purpose before implementation.
- Explain important code and the required Unity Editor actions separately from file changes.
- Explain how to test each step and the expected result. Wait for the user's result before advancing to the next development stage.
- Do not generate the entire game or introduce unagreed gameplay systems.
- Distinguish code for files from Terminal commands.
- Explain how to start and stop the project, and when a build is needed. Browser deployment requires its own agreed setup.
- Use appropriate existing checks and report exactly what was tested. Do not claim Unity verification without running it.

## Organization and style

- Keep project-owned assets under Assets/_Diyar.
- Put genuinely shared files under common and gameplay-specific files under game. Add further feature folders only when needed.
- Keep logic in Scripts, scenes in Scenes, reusable objects in Prefabs, and visual resources in the appropriate art or UI folders.
- Keep dependencies and ProjectSettings consistent with the existing Unity project. Add packages only when the current task needs them.
- Preserve existing assets and their .meta files. Let Unity generate metadata for new assets when importing this scaffold.
- Use readable code, consistent indentation, and blank lines between related sections.
- In XML/HTML/UXML, multiline attributes are allowed, but put > or /> at the end of the final attribute line.
- For formatting-only requests, preserve behavior and design. Respect any explicitly requested folder scope.
- Avoid unnecessary architectural layers, assembly definitions, placeholder gameplay scripts, and dependencies.
