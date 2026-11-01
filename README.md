# boi_1

A little desktop buddy for getting small things done.

**Status: first native Windows test build. Core and native-form checks pass; live desktop verification is incomplete.**

boi_1 is a cream-tabby cat with a soft 2D anime look, designed to stroll along the desktop above the taskbar. Click for a timer or task list; dismiss the tools and the cat stays.

Movement update: roaming and drag-release recovery are now bounded to the rightmost 15% of the primary monitor's work area, above the system tray. Timer/storage/lane tests total 16. A smoother replacement character animation is awaiting the owner's choice; current art is retained until then. See [animation shortlist](docs/ANIMATION-OPTIONS.md).

## Run the test build

Run `./build.ps1` in PowerShell, then open `dist/boi_1.exe`. Keep the assets folder beside it. Requires Windows with .NET Framework 4.8. Uses Windows' built-in compiler; no modern SDK or browser engine is needed.

Click the cat for tools, right-click for quick controls, or use Ctrl+Alt+B. `Open-boi-tools.exe` opens the tools directly. Cat & settings includes nine expression previews, reduced motion, sound, and pause roaming. Quit from the tray or tools; no startup entry is installed.

Four walking frames and nine poses support blinking, strolling, goofy wiggling, happy swaying, napping, a serious focusing pose, a drag pose, and a celebration hop. These are sprite animations, not a full skeletal rig or pet-care simulation.

Run `./test.ps1` for 13 core checks. `dist/boi_1.exe --smoke-test` exercises native forms with isolated synthetic data and writes rendered evidence under evidence/. It does not prove real desktop click-through, dragging, focus, fullscreen transitions, hotkey delivery, or sleep/wake. See [QA status](design-qa.md). This is an unsigned test build, not a production release.

State is stored locally under `%LOCALAPPDATA%/boi_1/` as versioned XML, with atomic writes and backup recovery. No telemetry or background network calls are implemented. The credits link opens GitHub only when clicked. Primary monitor only; the initial shortcut is fixed.

## First version

- One countdown timer with start, pause, resume, reset, and a gentle completion alert.
- One to-do list with add, edit, complete, reopen, delete, and undo.
- A roaming cream tabby with idle, walk, nap, drag, and celebration animations; tray access and a show/hide shortcut.
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
