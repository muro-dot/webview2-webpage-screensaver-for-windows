"""Opt-in real WebView2 tests: python tests/integration.py (interactive desktop)."""
import json
from pathlib import Path
import subprocess
import sys
import tempfile

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT))
from screensaver.config import Display, Settings


def main():
    results = []
    executable = [str(ROOT / 'dist' / 'WebScreensaver.exe')] if '--frozen' in sys.argv else [sys.executable, str(ROOT / 'main.py')]
    for scenario in ('rotation', 'fallback', 'private'):
        with tempfile.TemporaryDirectory() as directory:
            directory = Path(directory)
            urls = [(ROOT / 'tests' / 'page-one.html').as_uri() + '|2', (ROOT / 'tests' / 'page-two.html').as_uri() + '|2']
            if scenario == 'fallback':
                urls = [(ROOT / 'tests' / 'does-not-exist.html').as_uri()]
            settings = Settings(language='en', private=scenario == 'private', clock=True, displays=[Display(urls=urls, zoom=125)])
            settings.save(directory / 'config.json')
            process = subprocess.run([*executable, '--preview', '--config', str(directory / 'config.json'), '--seconds', '8', '--report', str(directory / 'report.json')], cwd=ROOT, timeout=40)
            assert process.returncode == 0, f'{scenario}: exit {process.returncode}'
            report = json.loads((directory / 'report.json').read_text())
            assert report['ready'] == 1, report
            profile = report['profiles'][0]
            if scenario == 'fallback':
                assert profile['fallback'], report
            else:
                assert profile['loaded'] >= 2 and not profile['fallback'], report
                assert len(set(profile['history'])) == 2, report
                assert not report['errors'], report
                assert profile['actual'] == {'mute': True, 'zoom': 1.25, 'context_menus': False, 'devtools': False}, report
            if scenario == 'private':
                assert report['private_profile_removed'], report
            results.append({'scenario': scenario, 'passed': True, 'report': report})
    destination = ROOT / 'assets' / ('integration-frozen.json' if '--frozen' in sys.argv else 'integration-source.json')
    destination.write_text(json.dumps(results, indent=2).replace(ROOT.as_uri(), '$PROJECT'), encoding='utf-8')
    print(f'PASS: {len(results)} actual WebView2 scenarios. {destination}')


if __name__ == '__main__':
    main()
