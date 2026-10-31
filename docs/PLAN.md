# boi_1 — product and design plan

Planning draft · 2026-10-31

## Purpose

A small, friendly companion that sits on the Windows desktop and helps with two everyday actions: remembering a task and timing a short session. It should feel cute, calm, and easy to dismiss.

The earlier idea was a desktop pet connected to Google Search AI. This version keeps the companion interface and deliberately starts with local utilities. Google integration is a separate future decision, not a dependency or promise for this release.

## User and platform

Initial user: one person using their Windows computer for study and everyday work.
Initial target: Windows 11, x64. Other Windows versions and ARM are not release commitments yet.
Primary success: open the panel, add a task or start a timer, and return to work within a few clicks.

## v0.1 scope

| Area | Included |
| --- | --- |
| Companion | One character, draggable position, click to open panel, hide character |
| Timer | One countdown; 5, 15, 25 minute presets; custom whole minutes from 1 to 180 |
| Timer controls | Start, pause, resume, reset; reset returns to the selected duration |
| Completion | Visible finished state, one gentle optional sound, tray indication |
| Tasks | One list; add, edit, complete, reopen, delete with undo |
| Persistence | Save tasks, settings, window position, and timer state locally |
| Access | Tray menu and configurable show/hide shortcut |
| Settings | Sound, reduced motion, character visibility, always-on-top preference |
| Exit | Explicit Quit; optional startup is deferred |

No chat, voice, web search, recurring tasks, task due dates, subtasks, calendar, accounts, sync, gamification, pet feeding, or multiple timers in v0.1. Stopwatch, recurring Pomodoro cycles, notes, and reminders can wait until the basic app feels good.

## Everyday experience

### Opening boi_1

First launch shows the character near the bottom-right, inside the usable screen area, with a short hint: “Click me for your timer and tasks.” No onboarding wizard or login.

Click opens a small anchored panel. Click again or press Escape to hide it. The timer continues while the panel is hidden. Closing the panel hides it; only Quit stops the process. The tray menu provides Open, Show/hide character, and Quit.

Dragging the character moves it without opening the panel. Save its position after dragging; keep the panel and character reachable when displays change. Do not take keyboard focus just to animate or announce completion.

### Timer

Choose a preset or type minutes; Start begins one countdown. The main action changes to Pause and then Resume. Reset stops the countdown and restores the selected duration. Changing duration while a timer exists requires an explicit reset action.

The countdown uses elapsed real time: a 25-minute timer includes time the computer spends asleep. If it expires during sleep or while the app was closed, show “Timer finished while you were away” on return, once. boi_1 does not wake the computer or promise an alert while it is closed.

At completion, show a calm success expression and finished state. Play one sound only if enabled. Dismiss returns to ready. No forced popup over the current application. A small tray indication remains until acknowledged. OS notification integration is a later enhancement; v0.1 completion feedback must work inside the app.

Quit with a running timer explains that no alert will happen while closed; the deadline is saved and reconciled on the next launch.

### To-do list

A single input says “Add a small task…”; Enter adds it. Trim whitespace, reject blank entries, limit titles to 200 characters, and keep focus in the input for another task.

Checkbox completes a task. Completed tasks appear in a collapsible Done section and can be reopened. New items append to the active list; no categories or reordering controls yet. Edit through an accessible action; Escape cancels. Delete offers Undo until the next deletion or panel close. Do not silently erase completed items.

If the list is empty, say “One small thing at a time.” Show confirmation through a small expression change, not repeated celebration popups.

## Design brief

Character and panel must be designed together. Desired feel: cute, soft, legible, friendly, quiet. One character and a limited palette. The creature should stay recognizable at roughly 64–96 display-independent pixels. Panel starting size: about 340 by 440, adaptable to scaling and a small screen.

Panel information order: app name and hide/settings access; Timer and Tasks tabs; the current tool; compact running-timer status when viewing Tasks. Keep a running timer visible even on the other tab.

No final mascot, palette, or typography is selected. For the next design pass, generate exactly three visual alternatives:
1. Soft little blob — rounded, warm, minimal.
2. Tiny desk robot — friendly, tidy, slightly mechanical.
3. Sleepy forest creature — cozy, gentle, organic.

These are directions for exploration, not completed mockups. Show all three with the same timer and task content so the choice is about visual style. Choose one before app scaffolding. This planning repository does not imply visual approval.

Use clear text labels, visible keyboard focus, adequate contrast, and controls large enough to click easily. All actions must work by keyboard. Reduced motion disables decorative movement. Sound must be optional and never be the only completion cue. Do not communicate state with color alone.

## Default behavior

- Offline, local data, no telemetry or background network calls.
- Startup with Windows: not enabled or added in v0.1.
- Always-on-top: off by default, user can enable it.
- Reduced motion: follow Windows preference where available, plus manual override.
- Sound: gentle and on initially, with a plainly visible mute setting.
- Suggested shortcut: Ctrl+Alt+B, only if Windows allows registration; conflict must leave tray access available.
- One application instance.
- Basic character states only: idle, timing, finished. No pet simulation.

## Definition of a useful first release

A user can launch the packaged app, add and recover tasks after restarting, run a timer while the panel is hidden, understand completion, hide the character, reopen through the tray, and quit cleanly. Data must remain local. Visual design must still be readable at Windows scaling settings.

## Open choices for the next pass

Choose the mascot direction and panel styling. Confirm the intended Windows version if it differs from Windows 11. A code license can be chosen before sharing a usable release. Everything else above is a proposed default that can be adjusted during review.
