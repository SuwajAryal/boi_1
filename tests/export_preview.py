"""Encode the native renderer's output frames as a small review animation."""
from pathlib import Path
import sys
sys.path.insert(0, str(Path(__file__).resolve().parents[1] / '.tools' / 'pillow'))
from PIL import Image

root = Path(__file__).resolve().parents[1]
preview = root / 'dist' / 'cat' / 'preview'
for folder in preview.iterdir():
    if not folder.is_dir():
        continue
    frames = []
    for path in sorted(folder.glob('*.png')):
        with Image.open(path).convert('RGBA') as sprite:
            canvas = Image.new('RGBA', sprite.size, '#fff9f0')
            canvas.alpha_composite(sprite)
            frames.append(canvas.convert('RGB'))
    if frames:
        frames[0].save(preview / (folder.name.lower() + '.gif'), save_all=True,
                       append_images=frames[1:], duration=56, loop=0, disposal=2)
