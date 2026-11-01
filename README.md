# boi_1

A cream-tabby anime cat for keeping you company and getting small things done.

**Status: first native Windows test build. Core and native-form checks pass; live desktop verification is incomplete.**

Current correction: walking is temporarily disabled in new builds after the owner reported incorrect leg placement. The gait is not fixed yet. Idle and other expressions remain available. Three cloud-themed popup designs await selection before implementation; see docs/CLOUD-UI-REVISION.md. Older running builds are unchanged.

Selected mascot: the original cream tabby, restored after the bunny was rejected. There are 8 idle frames, 12 walking frames, and 16 reaction frames. Elapsed-time playback and shared scale/ground anchors reduce jitter. Static-pose rotation and artificial bobbing are removed. Generated drawings still have some shape drift; this is a test animation, not a hand-polished production cycle.

Roaming and drag-release recovery stay in the rightmost 15% of the primary monitor's work area, above the system tray. Timer/storage/lane/animation tests total 36. Alternative packs were researched but not imported; generated cat assets follow the selected template.

## Run the test build

Run `./build.ps1` in PowerShell, then open `dist/cat/boi_1.exe`. Keep its assets folder beside it. Quit older boi_1 versions from their tray first. Requires Windows with .NET Framework 4.8. No modern SDK or browser engine needed. `./package.ps1` creates dist/boi_1-cat-test.zip.

Click the cat for tools, right-click for quick controls, or use Ctrl+Alt+B. `Open-boi-tools.exe` opens tools directly. Cat & settings includes state previews, reduced motion, sound, and pause roaming. Quit from the tray or tools; no startup entry is installed.

Six frame sequences: idle/blink, walking, curious, enjoying, goofing and celebrating. Original focus, nap and picked-up poses are retained. Eight new still expressions: surprised, confused, proud, yawning, stretching, shy, grumpy and affectionate. All 17 states can be previewed in Cat & settings. Reduced motion freezes frames and roaming.

Run `./test.ps1` for 36 checks. `dist/cat/boi_1.exe --smoke-test` uses isolated data and writes evidence under dist/evidence/. `--render-preview` exports PNG loops under dist/cat/preview/; tests/export_preview.py optionally encodes GIFs with Pillow. These do not prove desktop click-through, dragging, focus, fullscreen transitions, hotkeys or sleep/wake. See [QA status](design-qa.md). Unsigned test build, not a production release.

State is stored locally under `%LOCALAPPDATA%/boi_1/` as versioned XML, with atomic writes and backup recovery. No telemetry or background network calls are implemented. The credits link opens GitHub only when clicked. Primary monitor only; the initial shortcut is fixed.

## First version

- One countdown timer with start, pause, resume, reset, and a gentle completion alert.
- One to-do list with add, edit, complete, reopen, delete, and undo.
- A roaming cat with frame-by-frame idle, walk and reactions; tray access and a show/hide shortcut.
- Local saving, optional sound, and reduced motion.

Works offline. No account, subscription, AI model, browser automation, or cloud sync is planned for v0.1.

## Planning documents

- [Product and design plan](docs/PLAN.md)
- [Implementation approach](docs/TECHNICAL.md)
- [Milestones and release checklist](docs/ROADMAP.md)
- [Desktop-pet research and behavior specification](docs/DESKTOP-PET.md)
- [Credits and reuse record](CREDITS.md)

Next: verify the native pet's movement and interactions on the user's desktop. The desktop automation surface could not expose a targetable boi_1 window during this build session.

## Development

Implemented first build: C# with Windows Forms and a native alpha-layered pet window on .NET Framework, using the compiler already installed on Windows. This replaces the original WPF/modern .NET proposal for this prototype. No installer or automatic updater is included.

## Privacy and public repository

This repository contains project documentation and, later, source code. Personal tasks, preferences, and timer state belong in local application data and must never be committed.

## License

No license has been selected yet.
