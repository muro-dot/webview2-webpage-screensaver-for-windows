"""Validated settings and read-only migration from the original C# application."""
from __future__ import annotations

import json
import os
import tempfile
from dataclasses import asdict, dataclass, field
from pathlib import Path
from urllib.parse import urlsplit


CONFIG_PATH = Path(os.environ.get('LOCALAPPDATA', str(Path.home()))) / 'WebScreensaverPython' / 'settings.json'


def parse_url(raw: str) -> tuple[str, int | None]:
    raw = raw.strip()
    address, separator, seconds = raw.rpartition('|')
    duration = None
    if separator:
        if not seconds.isdigit() or not 1 <= int(seconds) <= 86400:
            raise ValueError('URL duration must be between 1 and 86400 seconds.')
        raw, duration = address.strip(), int(seconds)
    parsed = urlsplit(raw)
    if parsed.scheme not in ('http', 'https', 'file') or (parsed.scheme != 'file' and not parsed.netloc):
        raise ValueError('Use a complete http://, https:// or file:/// URL.')
    if any(character.isspace() for character in raw):
        raise ValueError('URL spaces must be encoded as %20.')
    return raw, duration


@dataclass
class Display:
    urls: list[str] = field(default_factory=lambda: ['https://example.com'])
    interval: int = 30
    randomize: bool = False
    zoom: int = 100


@dataclass
class Settings:
    language: str = 'ko'
    theme: str = 'system'
    mode: str = 'Separate'
    close_on_activity: bool = True
    mute: bool = True
    private: bool = False
    clock: bool = False
    clock_position: str = 'BottomRight'
    displays: list[Display] = field(default_factory=lambda: [Display()])

    def validate(self) -> None:
        if self.language not in ('ko', 'en') or self.theme not in ('system', 'dark', 'light'):
            raise ValueError('Invalid language or theme.')
        if self.mode not in ('Separate', 'Mirror', 'Span'):
            raise ValueError('Invalid monitor mode.')
        if self.clock_position not in ('BottomRight', 'BottomLeft', 'TopRight', 'TopLeft'):
            raise ValueError('Invalid clock position.')
        if not all(type(getattr(self, key)) is bool for key in ('close_on_activity', 'mute', 'private', 'clock')):
            raise ValueError('Invalid boolean setting.')
        if not 1 <= len(self.displays) <= 32:
            raise ValueError('Provide between 1 and 32 display profiles.')
        for display in self.displays:
            if type(display.interval) is not int or not 1 <= display.interval <= 86400:
                raise ValueError('Rotation interval must be between 1 and 86400 seconds.')
            if type(display.zoom) is not int or not 75 <= display.zoom <= 200:
                raise ValueError('Zoom must be between 75 and 200%.')
            if type(display.randomize) is not bool or not isinstance(display.urls, list):
                raise ValueError('Invalid display profile.')
            for raw in display.urls:
                if not isinstance(raw, str):
                    raise ValueError('URLs must be text.')
                parse_url(raw)

    @classmethod
    def from_dict(cls, data: dict) -> Settings:
        if not isinstance(data, dict):
            raise ValueError('Settings must be a JSON object.')
        if 'UrlsByScreen' in data:
            # Preserve compatibility with backups exported by the original application.
            mapping = {'Language': 'language', 'MultiScreenMode': 'mode', 'CloseOnActivity': 'close_on_activity',
                       'MuteAudio': 'mute', 'InPrivate': 'private', 'ShowClockOverlay': 'clock', 'ClockPosition': 'clock_position'}
            converted = {target: data[key] for key, target in mapping.items() if key in data}
            converted['displays'] = []
            for index, urls in enumerate(data['UrlsByScreen']):
                def item(key, default):
                    values = data.get(key, [])
                    return values[index] if index < len(values) else default
                converted['displays'].append(dict(urls=urls, interval=item('RotationIntervalsByScreen', 30),
                                                  randomize=item('RandomizeFlagByScreen', False), zoom=item('ZoomFactorsByScreen', 100)))
            data = converted
        known = set(cls.__dataclass_fields__) - {'displays'}
        result = cls(**{key: value for key, value in data.items() if key in known})
        if 'displays' in data:
            result.displays = [Display(**profile) for profile in data['displays']]
        result.validate()
        return result

    def save(self, path: Path = CONFIG_PATH) -> None:
        self.validate()
        path = Path(path)
        path.parent.mkdir(parents=True, exist_ok=True)
        # An atomic replacement keeps the previous settings intact if writing is interrupted.
        descriptor, temporary = tempfile.mkstemp(dir=path.parent, suffix='.tmp')
        try:
            with os.fdopen(descriptor, 'w', encoding='utf-8') as stream:
                json.dump(asdict(self), stream, ensure_ascii=False, indent=2)
                stream.flush()
                os.fsync(stream.fileno())
            os.replace(temporary, path)
        finally:
            Path(temporary).unlink(missing_ok=True)


def migrate_registry() -> Settings:
    import winreg
    result = Settings()
    try:
        with winreg.OpenKey(winreg.HKEY_CURRENT_USER, r'Software\Web-Page-Screensaver') as key:
            def read(name, default):
                try:
                    return winreg.QueryValueEx(key, name)[0]
                except OSError:
                    return default
            def boolean(name, default):
                return str(read(name, str(default))).lower() == 'true'
            result.language = read('Language', 'ko')
            result.mode = read('MultiScreenMode', 'Separate')
            result.close_on_activity = boolean('CloseOnActivity', True)
            result.mute = boolean('MuteAudio', True)
            result.private = boolean('InPrivate', False)
            result.clock = boolean('ShowClockOverlay', False)
            result.clock_position = read('ClockPosition', 'BottomRight')
            result.displays = []
            for index in range(32):
                raw = read(f'UrlScreen{index}', read('Url', 'https://example.com') if index == 0 else None)
                if raw is None:
                    break
                result.displays.append(Display(str(raw).split(), int(read(f'RotationIntervalScreen{index}', 30)),
                                               boolean(f'RandomOrderScreen{index}', False), int(read(f'ZoomFactorScreen{index}', 100))))
            result.validate()
    except (OSError, TypeError, ValueError):
        return Settings()
    return result


def load(path: Path = CONFIG_PATH) -> Settings:
    if not Path(path).exists():
        return migrate_registry() if os.name == 'nt' else Settings()
    # Invalid files are reported to the caller, never silently overwritten with defaults.
    return Settings.from_dict(json.loads(Path(path).read_text(encoding='utf-8-sig')))
