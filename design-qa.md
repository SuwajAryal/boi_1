# Design QA — first native build, 2026-11-01

## Latest enhanced-cat build — supersedes bunny selection

The user returned to the original cat. Current source truth is assets/selected-cat-reference.png. Current native renderer evidence is dist/cat/preview/{Idle,Walking,Surprised,...}/000.png and corresponding GIFs, with native-form evidence under dist/evidence/. Reference, rendered idle, rendered walk and surprised pose were opened together in a single comparison input. The 144x144 physical pet window retains the cream-tabby identity at150% Windows scale. Initial clipped idle/walk/extra atlases were rejected; padded revisions have contained sprites and full tails. Rendering uses common scale and bottom anchoring and caches unchanged frames. The soft cream/orange/pink palette and anime face remain; generated tiny motion marks and modest shape drift are residual polish, not a claim of perfect in-between animation.

There are17 states: six dedicated frame sequences, three original focus/nap/drag poses, and eight new still expressions. All36 core timing/storage/lane/clip-mapping checks pass, and isolated native-form smoke checks pass. More expressions are available through the scrollable Cat & settings panel. Desktop roaming remains in the rightmost15%, including recovery after dragging. The prior app was not forcibly stopped. Live integration checks below remain outstanding; final result stays blocked for production acceptance. The enhanced cat is delivered as a test build under dist/cat/.

## Latest bunny build update

Latest source: assets/selected-bunny-reference.png (1487x1058). Renderer evidence: dist/bunny/preview/{Idle,Walking,Curious,Enjoying,Goofing,Celebrating}/000.png plus loop frames and GIFs. Native tool evidence: dist/evidence/{timer,tasks,settings}.png. Source, idle and walking render were opened together in one comparison input. The 144x144 pet render preserves cream floppy ears, brown anime eyes and pink paws against alpha. Tools inherit prior typography/layout checks; settings now says Bunny. No reference wallpaper is shipped.

All 19 core checks and isolated native-form smoke checks pass. Walking uses12 frames, idle8, reactions4 per state. Proportional cell bounds handle atlas rounding. Shared scale/ground anchors replace per-frame resizing; no static rotation or synthetic bobbing. Some generated body/ear shape drift remains across frames and loops; no perfect hand-animation claim. Focus/rest/drag currently use fallback idle/curious poses.

The previous cat instance remains running and was not forcibly stopped. Preview/smoke modes use separate mutexes and synthetic data. Live bunny desktop capture is not yet obtained; the overall result remains blocked pending the desktop checks below. Close older boi_1 via its tray before opening dist/bunny/boi_1.exe.

final result: blocked

## Evidence and comparison

Source visual truth: assets/selected-cat-reference.png, selected cream-tabby concept, 1487x1058 presentation image with portrait and desktop context. Implementation: evidence/pet-idle.png and pet-walk.png, 144x144 physical-pixel pet window at 150% Windows scale (96 logical pixels). Native form renders: evidence/timer.png, tasks.png, settings.png, approximately 607x900 physical pixels.

Source and pet/timer/task renders were opened together in a single comparison input. Portrait and small pet use intentionally different presentation scales; compare identity, palette, silhouette and alpha edges, not the wallpaper. Form images come from Windows Forms DrawToBitmap inside the isolated smoke harness, not the interactive desktop compositor.

## Iterations

1. P1: controls clipped at 150% DPI. Added explicit DPI sizing and scaled dynamic task rows. Later captures show readable full action labels.
2. P2: Reset clipped after adding persistent footer. Increased popup client height; latest timer capture shows Pause and Reset completely.
3. P2: original atlas tail crossed a cell boundary. Generated padded cat-expressions-v2.png, rebuilt, and opened the reference and native pet render together. All nine visible-alpha bounds are inside their cells; full tail retained in the final idle render. Asset clipping resolved.

## Fidelity surfaces

- Typography: native Segoe UI and explicit action labels; large timer digits. No utility-popup layout was selected in the source image, so this is a functional secondary design rather than a pixel-match claim.
- Spacing/layout: pet-only default implemented. Popup checked at 150% scale; long settings content scrolls. Desktop placement still requires live capture.
- Colors: warm cream, peach and brown echo the selected cat; controls retain native Windows styling.
- Assets: real generated raster sprites retain orange stripes, brown anime eyes, pink paws and relaxed expression. True alpha retained, near-zero noise discarded at runtime. No code-drawn cat substitute.
- Copy: clear timer/task actions, local storage notice and timer footer; no AI/cloud features advertised. Initial clipped action labels fixed.

## Checks completed

Build succeeds. All 13 core timer/storage checks pass, including pause/resume/reset, restart expiry, single completion, atomic save, backup recovery, recovered-file persistence and visible save failure.

Native-form smoke harness passes timer start/pause/resume, task add/complete/edit/delete/undo, expression selection and settings-state changes. It renders the real forms using isolated synthetic data.

## Blocking live desktop verification

Initially the desktop tool did not expose boi_1. Giving only the tools form a normal taskbar presence exposed its native accessibility tree, including timer controls, tabs and footer. The pet itself still has no taskbar entry. However, the live capture was covered by a fullscreen app and raising boi_1 failed. No other app or browser was operated to work around this gap; unrelated fullscreen content was not saved or published.

Still required on the user's interactive desktop: visible default cat, roaming path, hover pause, clicks through transparent pixels into other apps, drag/release, focus retention, tray/hotkey reopen, fullscreen transitions, real sleep/wake, monitor changes and normal quit. No production-readiness or performance claim is made.

Next: open dist/boi_1.exe, or Open-boi-tools.exe for tools, on the user's desktop and capture the native experience. Resolve this gate before calling the release complete.
