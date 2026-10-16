# Iwacu Gallery

**An interactive first-person virtual gallery for exploring Rwandan cultural heritage.**

- **GCGO:** Art and Culture
- **WebGL:** [Play Iwacu Gallery on Unity Play](https://play.unity.com/en/games/d020d678-ee81-4167-a835-0bd4fcf3652e/iwacugallery)
- **Android:** [Download the Android release](https://github.com/Emmanuella00/IwacuGallery/releases/tag/IwacuGallery)
- **Demo video:** [Watch the project demonstration](https://www.youtube.com/watch?v=K61g5I1CGn0)

## GCGO and purpose

I am passionate about creating impactful, sustainable solutions through art and technology. Iwacu Gallery explores that interest through a small digital museum about Rwandan culture.

Living heritage is learned and shared through practice. Young Rwandans, members of the diaspora, and international learners may not always be able to encounter cultural traditions in person. This prototype offers a browser- and phone-accessible way to explore five exhibits without travelling to a physical gallery. It requires an internet-connected device and is intended to complement—not replace—community knowledge, cultural practice, or physical museums. Its real-world preservation impact has not been measured.


## Simulation overview

The visitor walks through a winding gallery and discovers five exhibits: **Intore, Imigongo, Agaseke, Inyambo, and traditional drums**. When close enough, the visitor aims at an artifact and opens its story panel. Closing the panel records the discovery for the current play session and updates the counter. The Heritage Trail guides the visitor toward an undiscovered exhibit.

**Intended users:** young Rwandans, the Rwandan diaspora, and international learners interested in Rwandan culture.

### Key interactions

- Explore the gallery using desktop keyboard/mouse controls or Android touch controls.
- Approach an exhibit and use the prompt to see when it can be inspected.
- Aim at an artifact and inspect it to open its story panel, image, and source credit.
- Close the story panel to mark the exhibit discovered and update the progress counter.
- Follow the Heritage Trail toward undiscovered exhibits.
- Discover all five exhibits to reach the completion screen.

## Unity mechanics implemented

### User Interface

The Canvas contains the start and completion screens, crosshair, target prompt, progress counter, story panel, and mobile controls. Story content is displayed from `ExhibitData` ScriptableObjects.

### Scripting

Scripts separate player control, exhibit interaction, exhibit state, story UI, progress tracking, audio, mobile controls, and trail guidance. `ProgressManager` counts each exhibit ID once during a play session and raises progress and completion events. Progress is **not saved between sessions**.

### Collision and triggers

Solid colliders on the gallery and exhibit pieces prevent the player from walking through them. Each exhibit also has a trigger sphere used only to detect whether the player is close enough; it does not block movement. The player uses a `CharacterController`, with gravity applied by the movement script.

### Raycasting

A ray from the camera checks which artifact the player is aiming at. Trigger colliders are ignored by the ray, and inspection requires both a valid raycast target and being inside that exhibit’s range. This prevents inspecting an exhibit from across the room and lets the visitor choose between nearby exhibits by aiming.

### Line Renderer

The Heritage Trail uses a Line Renderer as a subtle connector along a NavMesh route. Arrow stepping-stones make the direction easier to read. The route is selected from the undiscovered exhibits and avoids the gallery dividers when the NavMesh is available.

## Additional features

- **Gallery audio:** ambience starts after the first inspect action; a discovery chime plays on first discovery; drum audio fades in near the Drums exhibit.
- **Mobile optimization:** Android uses landscape orientation and on-screen movement/look/Inspect controls. In WebGL, touch controls are revealed after a real touch; desktop mouse use does not show them.
- **Start and completion flow:** a start screen begins the visit, and a completion screen appears after all five exhibits have been discovered.

The Heritage Trail supports the required Line Renderer mechanic; its arrow markers are a visual enhancement. This project does not include saved progress or the discovery particle burst.

## Controls

| Platform | Controls |
| --- | --- |
| Desktop browser | WASD or arrow keys to move; mouse to look; E or left mouse button to inspect a target. Click the game view to begin mouse look. |
| Android | Use the left on-screen stick to move, the right stick to look, tap Inspect when it appears for a valid target, and use Close on the story panel. |
| WebGL on a touch device | Touch controls appear after the first touch. That initial touch reveals the controls. |

## Build and run

### Play the published builds

- **WebGL:** open the [Unity Play build](https://play.unity.com/en/games/d020d678-ee81-4167-a835-0bd4fcf3652e/iwacugallery) in a supported browser.
- **Android:** open the [GitHub release](https://github.com/Emmanuella00/IwacuGallery/releases/tag/IwacuGallery), download the APK asset, and install it on an Android device. Android may ask permission to install apps from that source.

### Open the Unity project

1. Clone or download this public repository.
2. Open it with **Unity 6000.5.10f1** (Unity 6.5) and allow the Package Manager to resolve packages.
3. Open `Assets/_Project/Scenes/IwacuGallery.unity`.
4. To make a new build, select the target in **File > Build Profiles** and use the platform’s build settings.

The project uses URP and the **New Unity Input System only**. The EventSystem uses `InputSystemUIInputModule`.

## Project organization

- `Assets/_Project/Scenes` — gallery scene
- `Assets/_Project/Scripts/Player` — player movement and look
- `Assets/_Project/Scripts/Exhibits` — exhibit behavior and data
- `Assets/_Project/Scripts/UI` — story, progress, game-flow, and mobile UI
- `Assets/_Project/Scripts/Systems` — audio and trail systems
- `Assets/_Project/Prefabs` — reusable exhibit and interaction prefabs
- `Assets/_Project/ScriptableObjects/Exhibits` — exhibit story data
- `Assets/_Project/Input` — project Input Actions asset

## Testing, limitations, and reflection

The final WebGL build was tested in a desktop browser, and the Android build was tested on an Itel A669W phone. Platform-specific work included forcing landscape orientation, adjusting Canvas scaling and touch controls, and including ARMv7 support for the test phone. The project also addressed a too-small exhibit trigger and a camera near-clip issue at walls.

A key learning was that working gameplay mechanics are not enough on their own: interface layout, input behavior, device compatibility, asset rights, and testing on the actual target hardware all affect the visitor experience. With more time, I would improve the trail’s visual clarity, refine the exhibit models and lighting, expand the content with community input, and test accessibility with more users. This is a prototype and does not claim a measured cultural-preservation outcome.

## Credits and image rights

Third-party asset sources and licences are recorded in [CREDITS.md](CREDITS.md).
