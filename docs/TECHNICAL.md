# Implementation approach

This is an architecture proposal, not an implemented or benchmarked system.

## Proposed stack

C# with WPF on a supported .NET LTS runtime, selected and pinned at implementation time. WPF is a Windows-only desktop framework with controls, styling, graphics, and animation support: [Microsoft WPF overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview).

Use a normal Windows process with a small character window and a separate tool panel. Integrate a notification-area icon using Windows Forms NotifyIcon: [Microsoft reference](https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.notifyicon).

This avoids needing a browser engine for two local utilities. It also intentionally targets Windows rather than promising cross-platform support.

## Components

- Views and view models: character, panel, timer, tasks, and settings.
- Timer service: state transitions, deadline reconciliation, and a clock abstraction.
- Task store: list operations and persistence.
- Settings store: display preferences, shortcut, and positions.
- Desktop integration: tray, hotkey, display bounds, and single-instance handling.
- Asset folder: chosen character images and bundled sound with clear provenance.

Keep core timer and task behavior separate from window code so important behavior is testable without opening the interface. Avoid a plugin architecture, database server, dependency-heavy framework, or installer service.

## Timer rules

States: idle, running, paused, finished.
Running state stores deadlineUtc; paused stores remaining duration; idle stores selected duration. Persist state on transitions rather than on every displayed tick.

During a session, use an elapsed-time source rather than subtracting one second per UI refresh. Reconcile the saved UTC deadline after sleep, restart, or closing the app. Define wall-clock changes explicitly: remaining time follows elapsed time while running; recovery across process restarts follows the saved UTC deadline and may reflect a manual clock change. Test both paths.

Completion must be idempotent: repeated refresh/resume events cannot replay the alert. Store acknowledgment and completion status. Invalid or negative input cannot start a timer.

## Local storage

Planned location: %LOCALAPPDATA%/boi_1/.
A versioned JSON file is sufficient for the initial list and settings. Use stable task IDs and completion timestamps. Store timer state separately or as an independently versioned section.

Write a temporary file then atomically replace the previous file; retain one last-known-good backup. Validate on load. If data is corrupt, preserve the original, try the backup, and explain recovery rather than silently overwriting the user's tasks. If saving fails, keep edits in memory and show a persistent warning.

Runtime data, logs containing user task text, and local backups must be excluded from Git. The public repo must contain only synthetic examples. Never require admin access for ordinary use.

## Desktop behavior

Register a configurable global hotkey and handle conflicts gracefully. Tray access remains the fallback. Unregister the shortcut and dispose of tray resources on Quit.

Clamp saved positions to current monitor work areas; handle DPI scaling and disconnected displays. Ensure transparent-window hit testing does not block clicks outside the visible character. Test focus, dragging, and screen-reader labels. Suspend decorative animation when hidden and honor reduced motion.

## Packaging and measurements

Start with a local development build after visual selection. Then create a self-contained Windows x64 release for a clean user-account test. Confirm antivirus behavior and unsigned-app prompts before describing installation as smooth. Signing and automatic updating are separate later decisions.

Measure cold launch, idle CPU, memory, and hidden-state behavior on the target computer. Aim for no continuous decorative polling when hidden and near-idle CPU. Do not publish performance claims before measurement.

## Proposed source layout

- src/Boi.App — WPF windows and Windows integration
- src/Boi.Core — timer rules and task models
- tests/Boi.Core.Tests — meaningful timer and persistence tests
- assets — approved character and sound
- docs — plan, decisions, and manual release checklist

Do not create application scaffolding until the visual direction is selected. No Google/browser component is part of this architecture.
