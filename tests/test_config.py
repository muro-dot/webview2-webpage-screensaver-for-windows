import json
from pathlib import Path
import tempfile
import unittest

from screensaver.config import Display, Settings, load, parse_url


class SettingsTests(unittest.TestCase):
    def test_original_backup_import(self):
        result = Settings.from_dict({'Language': 'en', 'MultiScreenMode': 'Mirror', 'MuteAudio': False,
            'UrlsByScreen': [['https://example.com|60'], []], 'RotationIntervalsByScreen': [45], 'ZoomFactorsByScreen': [150]})
        self.assertEqual(result.mode, 'Mirror')
        self.assertFalse(result.mute)
        self.assertEqual(result.displays[0].zoom, 150)
        self.assertEqual(result.displays[1].interval, 30)

    def test_round_trip_and_atomic_replacement(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / 'settings.json'
            original = Settings(displays=[Display(['https://example.com|45'], 50, True, 125), Display([])])
            original.save(path)
            self.assertEqual(load(path), original)
            original.language = 'en'
            original.save(path)
            self.assertEqual(load(path).language, 'en')
            self.assertEqual(len(list(Path(directory).iterdir())), 1)

    def test_invalid_settings_never_replace_valid_file(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / 'settings.json'
            Settings().save(path)
            before = path.read_bytes()
            invalid = Settings(displays=[Display(['javascript:alert(1)'])])
            with self.assertRaises(ValueError):
                invalid.save(path)
            self.assertEqual(path.read_bytes(), before)

    def test_url_duration_and_protocol_validation(self):
        self.assertEqual(parse_url('https://example.com/a?b=c|90'), ('https://example.com/a?b=c', 90))
        self.assertEqual(parse_url('file:///C:/test.html'), ('file:///C:/test.html', None))
        for url in ('https://example.com|0', 'https://example.com|abc', 'javascript:alert(1)', 'https://', 'https://a b'):
            with self.subTest(url=url), self.assertRaises(ValueError):
                parse_url(url)

    def test_bad_json_types_rejected(self):
        for data in ({'mute': 'false'}, {'displays': []}, {'displays': [{'interval': -1}]}, {'displays': [{'zoom': 500}]}):
            with self.subTest(data=data), self.assertRaises(ValueError):
                Settings.from_dict(data)


if __name__ == '__main__':
    unittest.main()
