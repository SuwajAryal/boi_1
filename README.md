# boi_1

A little desktop buddy for getting small things done.

**Status: planning — no runnable app yet.**

boi_1 will be a little cloud pet roaming along the bottom of the Windows desktop, above the taskbar. It wanders, blinks, rests, and naps. No panel is visible by default. Click the pet for a timer or simple to-do list; close the popup and the pet stays.

## First version

- One countdown timer with start, pause, resume, reset, and a gentle completion alert.
- One to-do list with add, edit, complete, reopen, delete, and undo.
- A roaming cloud pet with idle, drift, nap, drag, and celebration animations; tray access and a show/hide shortcut.
- Local saving, optional sound, and reduced motion.

Works offline. No account, subscription, AI model, browser automation, or cloud sync is planned for v0.1.

## Planning documents

- [Product and design plan](docs/PLAN.md)
- [Implementation approach](docs/TECHNICAL.md)
- [Milestones and release checklist](docs/ROADMAP.md)
- [Desktop-pet research and behavior specification](docs/DESKTOP-PET.md)
- [Credits and reuse record](CREDITS.md)

Next: choose the roaming pet's appearance, then prove its transparent desktop window, movement, and click behavior before adding utilities.

## Development

Proposed approach: C# and WPF for Windows. The toolchain and supported runtime will be confirmed when implementation begins. No build commands or installer exist yet.

## Privacy and public repository

This repository contains project documentation and, later, source code. Personal tasks, preferences, and timer state belong in local application data and must never be committed.

## License

No license has been selected yet.
