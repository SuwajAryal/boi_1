# Animation options — 2026-11-01

Owner feedback: current animation is not fluid enough. Keep a cute anime feeling; species is flexible. Present choices before installing a replacement. Pet should remain in the rightmost 10–20% of the primary screen; implementation uses 15% including the pet's full width. Release after dragging clamps back into that lane. Full-width roaming is removed.

## Reusable preview shortlist

[Free Animated Character Sprites by Sprited](https://kndlt.itch.io/spritedx-character-sprites), by kndlt / SpriteDX, is the source of these candidates. The creator describes 77 AI-generated pixel-inspired chibi characters with transparent idle, greet and run clips and PNG atlases with JSON frame coordinates/timing. Assets are described as CC0 1.0; offline preview code as MIT. The page acknowledges flicker and imperfect loops, so the selected candidate must be viewed at pet size and checked before integration. [Source gallery](https://spritedx.com/explore).

Three publicly linked sample characters, in displayed sample order:

1. Blonde chibi in blue dress: https://spritedx.com/p/09859d52-8285-4d22-9501-48758c568047/transparent_idle.webp
2. Cat-ear chibi in black outfit: https://spritedx.com/p/544acd30-5445-473f-98be-352ad296ec0c/transparent_idle.webp
3. Brown-haired chibi in dark dress: https://spritedx.com/p/42e98547-591d-4d9c-a125-d47c10ed43b0/transparent_idle.webp

These are external previews. No SpriteDX assets or code have yet been copied into the application or repository. On selection, retain source ID, full license and author attribution, inspect the downloadable pack's provenance and manifest, decode the actual frame timing, and preserve consistent scale and ground anchor across frames. Do not replace a real frame cycle with rotation of one static pose. Rendering refresh and animation frame timing should be independent; reduced motion and hidden-state suspension remain supported.

## Other sources reviewed

- [VS Code Pets media](https://github.com/tonybaloney/vscode-pets/tree/main/media): author specifically requested cat assets not be freely distributed on GitHub. Not imported.
- [FrolicForge high-resolution cat](https://frolicforge.itch.io/cat-animation-high-res): 12 walking frames; page disallows asset redistribution. Not imported.
- [Paws & Whiskers](https://netherzapdos.itch.io/paws-whiskers-isometric-cats-pack): page lists redistribution restrictions and free-pack noncommercial limits. Not imported.

A generated 16-frame cream-tabby atlas was also explored locally, but is unselected and has continuity/clipping issues. It is not integrated or published as a smooth replacement.
