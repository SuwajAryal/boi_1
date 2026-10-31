# Roadmap and release checklist

## Milestone 0 — planning (this pass)

- [x] Define boi_1 and keep timer/tasks as the core.
- [x] Write product scope, user flows, architecture, and acceptance checks.
- [ ] Review the plan with the owner.
- [ ] Generate three visual concepts and select one.

## Milestone 1 — desktop shell

After visual selection, implement the approved character and panel; tray access; show/hide; dragging; screen-bound recovery; single instance; and explicit Quit.

Done when the app is reachable after hiding, reopening, and changing display configuration. Verify keyboard focus and Windows scaling before adding polish.

## Milestone 2 — timer

Implement one timer, presets/custom minutes, pause/resume/reset, local state, completion, and sound preference.

Done when transitions are correct and hiding, sleep, restart, and delayed refresh cannot create a wrong countdown or duplicate completion.

## Milestone 3 — tasks and saving

Implement add/edit/complete/reopen/delete/undo, Done section, and durable local saves.

Done when tasks survive restart, invalid input is handled, and recovery from a corrupt or unwritable file is understandable.

## Milestone 4 — polish and first packaged build

Finish accessibility, reduced motion, settings, and chosen character states. Package a Windows x64 build. Record measured performance and known limitations. No automatic updates or cloud services.

Done when the acceptance checklist passes on a normal Windows user account and the owner has used the app for a short everyday session.

## Acceptance checklist

| Scenario | Expected result |
| --- | --- |
| Start custom 1-minute timer | Accurate elapsed countdown and one completion |
| Pause, wait, resume | Paused duration is preserved |
| Reset a running timer | Stops and returns to selected duration |
| Switch to Tasks while running | Timer continues with visible status |
| Hide panel/character | Timer continues; tray reopens app |
| Sleep beyond deadline | Finished state shown once on return |
| Quit/relaunch before deadline | Remaining time reconciles; no alert while closed |
| Relaunch after deadline | One “while you were away” completion |
| Blank, zero, negative, text, huge duration | No invalid timer starts; clear feedback |
| Complete/reopen/edit/delete task | Correct item changes; delete can be undone |
| Restart with saved tasks/settings | Data is recovered exactly |
| Corrupt data file | Original preserved; backup/recovery explained |
| Unwritable storage | Warning; no false claim that data was saved |
| Shortcut already occupied | Explain conflict; tray access still works |
| Second launch | Bring existing instance forward |
| Disconnect monitor/change scale | Character/panel remain reachable and readable |
| Keyboard and screen reader | Every core action has a name and focus path |
| Sound off/reduced motion | Visible completion remains; decorative movement stops |
| Quit | Process exits, tray icon disappears, hotkey released |
| Offline use | All v0.1 actions work without an account or network |

Automate timer state/deadline and storage recovery checks. Manually verify desktop integration, accessibility, visual fidelity, packaging, and real sleep/wake behavior. Record results when implemented; these checks have not been run yet.
