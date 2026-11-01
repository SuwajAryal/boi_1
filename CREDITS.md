# Credits and reuse record

2026-11-01 animation research: kndlt / SpriteDX's [Free Animated Character Sprites](https://kndlt.itch.io/spritedx-character-sprites) provides the external chibi previews shortlisted in docs/ANIMATION-OPTIONS.md. Creator states CC0 for character assets and MIT for offline preview code. No assets or code from that pack have been imported yet. Full provenance and license will be retained if selected.

boi_1 learns from existing desktop-pet projects. Thank you to their authors and contributors.

## Research and design inspiration

Reviewed 2026-10-31. No source code, sprites, sounds, or binaries from these projects have been copied into boi_1 in this planning pass.

- [VPet-Simulator](https://github.com/LorisYounger/VPet), by LorisYounger and contributors. Reference for a C#/WPF desktop pet with separate animation, interaction, and behavior logic. Repository license: [Apache-2.0](https://github.com/LorisYounger/VPet/blob/main/LICENSE). Reviewed Core directory and MainLogic.cs. Full simulator integration has not been selected.
- [VS Code Pets](https://github.com/tonybaloney/vscode-pets), by Anthony Shaw and contributors. Reference for idle/walk states, movement bounds, sprite direction, and temporary reactions. Reviewed src/panel/states.ts and basepettype.ts. Repository license: [MIT](https://github.com/tonybaloney/vscode-pets/blob/main/LICENSE), copyright (c) 2022 Anthony Shaw. This is an editor extension, not a standalone Windows overlay.
- [Rainier Bandit / Jimothy](https://github.com/Manaiakalani/rainier-bandit), by Manaiakalani and contributors. README reference for taskbar roaming, no-focus overlay behavior, click-through, and tray controls. Repository advertises MIT; full license text was not retrieved in this review, so no code or assets are approved for reuse from it yet. No runtime verification performed.

## Rules when reusing material

Credit ideas here even when no implementation is copied. Before copying code or assets, record the exact upstream commit, original path, author, license, local destination, and modifications. Keep the applicable full license/copyright notices alongside redistributed material and include them in packaged releases. For Apache-licensed reuse, preserve applicable NOTICE content and mark modified files as required by the upstream license.

Check artwork and sound permissions individually rather than assuming the repository's code license covers every asset. An acknowledgment alone is not a substitute for meeting the source license. No third-party character branding or artwork is selected for boi_1.

## Reused implementation and assets

No upstream implementation code was copied. The first build uses .NET Framework Windows Forms, System.Drawing, System.Xml, and Windows system APIs.

## Generated cat artwork — 2026-11-01

The owner selected the first cream-tabby cat. Its reference is assets/selected-cat-reference.png. Built-in Image Gen generated the animation atlases from that visual reference. No franchise characters or third-party pet sprites were requested or copied.

- assets/cat-expressions-v2.png: nine poses in a 3x3 atlas. Idle, curious, serious, enjoying, sleeping, celebrating, dragged, goofy, blink. A padded revision supersedes the earlier cat-expressions.png to prevent tail clipping.
- assets/cat-walk.png: four left-facing walking frames in a 2x2 atlas.

Prompt briefs: preserve the selected cream-orange tabby, brown anime eyes, soft 2D outlines and pastel shading; full body in equal atlas cells with transparent margins; no text or scenery. Runtime decoding retains alpha and discards negligible alpha noise. Source atlases remain unmodified.
