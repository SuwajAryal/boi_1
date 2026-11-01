# Credits and reuse record

## Blue cloud tools interface — 2026-11-01

The owner selected cloud option1, retained as assets/selected-cloud-ui.png. Built-in Image Gen created assets/cloud-panel.png from that reference: a blank, transparent cloud silhouette with ivory center, pale-blue outline and bottom cloud puffs. Timer, tasks, settings, buttons and labels are live native controls over that artwork, not a flattened screenshot.

Settings, close, clock, list and play/pause icons use Font Awesome Free6.7.2 by Fonticons, Inc. Bundled source: https://github.com/FortAwesome/Font-Awesome/tree/6.7.2 . Font file assets/fa-solid-900.ttf is used under SIL OFL1.1; complete original distribution license retained in assets/licenses/font-awesome.txt and copied into builds. Ordinary button outlines are native control styling. No additional external character artwork was adopted.

## Enhanced original cat — current selection, 2026-11-01

The owner rejected the bunny build and returned to the original cream tabby in assets/selected-cat-reference.png. That image was the reference for built-in Image Gen to create cat-idle-loop-v2.png (8 frames), cat-walk-loop-v2.png (12 frames), cat-reaction-loops.png (4 rows of4: curious, enjoying, goofing, celebrating), and cat-extra-expressions-v2.png (8 still poses: surprised, confused, proud, yawning, stretching, shy, grumpy, affectionate). Prompts preserve the selected soft 2D anime identity, cream-orange stripes, brown eyes, pink paws and blush, requesting generous transparent gutters and full tails. The padded extra-expression revision shrinks the initial sheet to remove clipping. The earlier cat-expressions-v2.png supplies original focus, nap and picked-up poses. No external character assets were copied. Runtime alpha cleanup, consistent scale/bottom anchors and frame caching support playback; some generated shape drift remains.

## Selected bunny assets — 2026-11-01

Owner chose bunny option 1 after reviewing bunny/fox/hamster concepts. Reference: assets/selected-bunny-reference.png. Built-in Image Gen produced original sprite atlases from that reference: bunny-idle.png (8 frames,4x2), bunny-walk.png (12 frames,4x3), bunny-reactions-v2.png (16 frames,4x4; curious/enjoying/goofing/celebrating rows). Build copies padded reactions as bunny-reactions.png. Prompt briefs requested consistent proportions, soft 2D anime linework, cream fur, floppy pink-lined ears, brown sleepy eyes, pastel shading, sequential poses and transparent margins. No external character assets were imported. Runtime removes negligible alpha noise and normalizes common scale/bottom anchors; preview GIFs encode native renderer outputs. Older cat artwork below is historical and not included in the bunny package.

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
