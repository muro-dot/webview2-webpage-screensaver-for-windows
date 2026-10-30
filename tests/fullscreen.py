"""Regression: match real monitor and client bounds at the current Windows DPI."""
import json
from pathlib import Path
import subprocess
import sys
import tempfile
import time

from PIL import ImageGrab

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT))
from screensaver.config import Display, Settings
from screensaver.windows import dpi_aware, monitors


def main():
    dpi_aware()
    rectangles = [list(rectangle) for rectangle in monitors()]
    frozen = '--frozen' in sys.argv
    executable = [str(ROOT / 'dist' / 'WebScreensaver.scr')] if frozen else [sys.executable, str(ROOT / 'main.py'), '/s']
    with tempfile.TemporaryDirectory() as directory:
        directory = Path(directory)
        settings = Settings(language='en', close_on_activity=False, displays=[Display(urls=[]) for _ in rectangles])
        settings.save(directory / 'settings.json')
        child = subprocess.Popen([*executable, '--config', str(directory / 'settings.json'), '--seconds', '8', '--report', str(directory / 'report.json')], cwd=ROOT)
        try:
            time.sleep(4)
            ImageGrab.grab(all_screens=True).save(ROOT / 'assets' / ('fullscreen-frozen.png' if frozen else 'fullscreen-source.png'))
            child.wait(timeout=25)
        finally:
            if child.poll() is None:
                child.terminate()
                child.wait(timeout=5)
        assert child.returncode == 0, child.returncode
        report = json.loads((directory / 'report.json').read_text())
        assert report['ready'] == len(rectangles), report
        assert len(report['profiles']) == len(rectangles), report
        for profile, rectangle in zip(report['profiles'], rectangles):
            assert profile['target_bounds'] == rectangle, report
            assert profile['actual_bounds'] == rectangle, report
            assert profile['client_size'] == rectangle[2:], report
        destination = ROOT / 'assets' / ('fullscreen-frozen.json' if frozen else 'fullscreen-source.json')
        destination.write_text(json.dumps(report, indent=2), encoding='utf-8')
    print(f'PASS: all {len(rectangles)} monitor and client bounds exactly match physical pixels.')


if __name__ == '__main__':
    main()
