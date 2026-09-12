# Street Football Pakistan — Unity 6.3 LTS

A clean Unity 6.3 LTS 3D mobile football foundation. This project is intentionally independent of the old Godot prototype.

## Target
- Android mobile
- 3D street-football presentation
- Touch joystick + action buttons
- Player movement, sprint, pass, shoot, tackle
- Ball physics and possession
- Basic teammate/opponent AI
- Broadcast-style camera and HUD
- Procedural Pakistan street environment so the repository has no external art dependency

Unity version: **6000.3 LTS**. Unity 6.3 LTS is the current LTS release and is supported through December 2027. See Unity's release page for installation details.

## First run
Open the project in Unity Hub, let packages import, then open `Assets/Scenes/StreetMatch.unity` or use the editor menu `Street Football/Build Demo Scene`.

## Android
The repository includes an editor build entry point. A licensed Unity 6.3 LTS Android-capable build runner is required to produce the APK; the project itself contains the Android player settings and build automation.

Build pipeline: Android serial activation + APK integrity verification.
