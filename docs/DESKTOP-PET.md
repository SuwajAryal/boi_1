# Desktop-pet research and revised behavior

2026-10-31. Planning and source review only; no upstream application was installed or run.

## Product decision

boi_1 is a small cloud pet living above the taskbar. The ordinary screen has only the pet, with no permanent window frame, dashboard, card, or timer panel. It drifts along the bottom, pauses, looks around, and naps. Click to open simple tools; dismissing them leaves the pet roaming.

The earlier panel-heavy concepts do not represent the requested product. New concepts should show a small pet on a desktop, not a panel with a decorative pet beside it.

## Findings and reuse decision

| Reference | Evidence reviewed | Useful lesson | Decision |
| --- | --- | --- | --- |
| [VPet](https://github.com/LorisYounger/VPet) | README, Core layout, [MainLogic.cs](https://github.com/LorisYounger/VPet/blob/main/VPet-Simulator.Core/Display/MainLogic.cs), Apache-2.0 license | WPF is used for an actual desktop-pet project; animation and behavior have distinct responsibilities | Keep C#/WPF proposal; evaluate small reusable parts during implementation rather than adopting the full simulator |
| [VS Code Pets](https://github.com/tonybaloney/vscode-pets) | [states.ts](https://github.com/tonybaloney/vscode-pets/blob/main/src/panel/states.ts), [basepettype.ts](https://github.com/tonybaloney/vscode-pets/blob/main/src/panel/basepettype.ts), MIT license | Explicit idle/walk states, boundaries, direction changes, and temporary reactions | Use these established concepts; consider attributed adaptation of a small behavior module if it saves work |
| [Rainier Bandit](https://github.com/Manaiakalani/rainier-bandit) | Repository README and displayed license label | Taskbar roaming and no-focus/click-through behavior closely match the desired experience | Behavior inspiration; Electron implementation not selected, license text still needs checking before copying |

Recommendation: use mature framework/Windows facilities and the proven behavior patterns above. Do not pull an entire simulator into two small utilities without first comparing its integration cost. No upstream code is adopted yet; this is not a claim that every component must be written from scratch. See [credits](../CREDITS.md).

## Movement and interaction specification

- Cloud size: approximately 80 display-independent pixels wide, adjustable later after visual selection.
- One lane on the primary monitor, baseline just above the taskbar's usable work-area edge. For auto-hidden taskbars, leave a small inset from the bottom. Recompute when work area or scaling changes.
- Proposed drift speed: 20–35 display-independent pixels per second, bounded to the visible lane. Short journeys alternate with longer rests; movement should not be continuous pacing.
- The cloud glides with a gentle bob rather than pretending to have feet. Turn by shifting eyes and a small body lean; no sudden teleports.
- Hover immediately stops translation. Clicking opens tools; keep the pet stationary while tools or a menu are open. Resume only after pointer leaves and tools close.
- Left-button motion beyond a small drag threshold becomes a drag, suppressing the click action. On release, ease back to the bottom lane at the chosen horizontal position. No throwing or physics in v0.1.
- Right-click opens Timer, Tasks, Pause/resume roaming, Hide, and Quit. Tray offers equivalent access. Keyboard shortcut opens tools, so a moving pet is never the only entry point.
- Nap follows a quiet interval since the last pet interaction; no keyboard logging or screen reading is needed. Opening tools wakes the pet.
- Reduced motion stops travel and bobbing. The static character remains usable.
- Fullscreen use hides the pet on that monitor; timer operation continues. First version supports one primary-monitor lane, not climbing windows or crossing displays.

All numbers above are starting defaults to tune in a real Windows prototype, not measured usability findings.

## States and priority

Base states: idle, drift-left, drift-right, nap. Temporary states: dragged and celebrate. User pause, hover, popup visibility, hidden/fullscreen, and reduced motion are control flags rather than separate copies of every animation.

Priority: hidden/fullscreen > drag > open tools/hover/pause > completion reaction > normal idle/drift/nap. If completion arrives while hidden, record it and show it on return; if dragged, wait until release. Never interrupt a drag or open a popup automatically. Timer progress is independent of pet motion.

## Art and animation requirements

Choose one consistent cloud identity before implementation. Minimum artwork: idle/blink loop, side-facing drift loop, nap loop, lifted/drag pose, and happy completion. Use a shared canvas, stable anchor, consistent scale, and real transparent alpha. Sprite-frame timing and footprint must be defined; generated concept pictures alone are insufficient production animation.

Use original or clearly licensed cloud art. Investigate reusable animation/rendering code as needed, with path-level provenance. No copying a recognizable proprietary pet character.

## First engineering proof

Before timer or task features, verify on the actual Windows desktop: transparent borderless rendering, visible-pixel click behavior, no keyboard focus theft, bounded movement above the taskbar, pause-on-hover, drag detection, hide/tray recovery, scaling, and fullscreen behavior. Do not call this successful based on a web mockup.

WPF transparent rendering and native hit testing need a small proof. An invisible rectangle that blocks work is a release failure. Avoid a fullscreen overlay where a small pet-sized window will suffice. Cap animation refresh, stop it while hidden, and measure resource use before making lightweight claims.
