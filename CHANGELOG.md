# Changelog / 변경 이력

All notable changes to this project will be documented in this file.  
이 프로젝트의 주요 변경 이력은 이 문서에 기록됩니다.

---

## [v1.0.5] - 2026-09-22

### 🌐 English
- **Audio Mute (Silent Screensaver)**: Prevents sudden noise by automatically muting audio on all web pages.
- **InPrivate Browsing**: Protects privacy with isolated, non-persistent browsing sessions.
- **Graceful Fallback Clock**: Automatically displays a sleek neon digital clock when offline or when a webpage fails to load.
- **Live Web Preview**: Instant preview dialog to test URLs directly from the settings window with active zoom and mute.
- **Zoom Factor Adjustment**: Custom display scale (75% to 200%) optimized for 4K and QHD monitors.
- **Clock HUD Overlay & Corner Selection**: Optional glassmorphism floating digital clock/date widget on top of any web screensaver, with selectable position across all 4 screen corners (Bottom-Right, Bottom-Left, Top-Right, Top-Left).
- **Custom Per-URL Intervals**: Support for individual rotation times (`URL|seconds`).
- **Config Backup & Restore**: Export and import full configuration in JSON format.
- **GitHub Update Checker**: Non-intrusive background check for newer releases.
- **Dynamic Responsive Layout**: Resolved text overlapping and improved dialog spacing (800px width).

### 🇰🇷 한국어
- **오디오 자동 음소거 (소리 끄기)**: 웹페이지 로드 시 불시의 소음을 방지하기 위해 기본 음소거 적용.
- **시크릿 모드 (InPrivate)**: 공용/업무 PC 환경에서 방문 기록 및 쿠키를 남기지 않는 안전 모드.
- **오프라인/에러 시 우아한 모던 시계 폴백**: 인터넷 단절 시 에러창 대신 세련된 네온 디지털 시계 자동 출력.
- **실시간 미니 웹 미리보기**: 설정창에서 선택한 사이트의 렌더링 상태를 즉시 모달 창으로 확인.
- **화면 배율 조절 (Zoom Factor)**: 4K 및 QHD 고해상도 환경에 최적화된 화면 배율(75% ~ 200%) 지원.
- **디지털 시계 HUD 오버레이 & 4개 모서리 위치 선택**: 웹 화면보호기 위에 반투명 글래스모피즘 시계/날짜 위젯을 4대 모서리(우측 하단, 좌측 하단, 우측 상단, 좌측 상단) 중 원하는 위치로 지정.
- **URL별 개별 표시 시간(초) 지정**: 사이트마다 `URL|초` 형식으로 가변 회전 주기 지원.
- **설정 원클릭 백업/복원 (JSON)**: 전체 설정을 JSON 파일로 손쉽게 내보내고 타 PC에 복원.
- **GitHub 최신 버전 자동 감지**: 새 릴리즈 출시 시 설정창 상단에 알림 뱃지 자동 표시.
- **동적 반응형 레이아웃 & 텍스트 겹침 방지**: 언어 변경 시 라벨과 컨트롤 간격 자동 조정 및 800px 폼 확장.

---

## [v1.0.4] - 2026-09-20

### 🌐 English
- **Real-Time System Theme Sync**: Instantly switches between Windows Light and Dark modes in real-time without restart, with native DWM title bar synchronization.
- **Refined Modern Color Tone**: Eye-friendly slate blue/sapphire color palette with interactive delete button hover effects.
- **Inline In-Place Editing**: Edit URLs or add new entries directly in the list via double-click or F2 key, with no extra popup windows needed.
- **Integrated Vertical Toolbar**: Compact right-side toolbar (▲/▼/＋/✎/✕) seamlessly attached to the list view, maximizing visible URL area.
- **Refreshed Fluent App Icon**: Title bar and executable icons upgraded with a sleek new Fluent design.

### 🇰🇷 한국어
- **시스템 테마 실시간 연동**: Windows 라이트/다크 모드 변경 시 재시작 없이 즉시 자동 전환 및 타이틀바 동기화.
- **모던 컬러 톤 정제**: 눈이 편안한 슬레이트 블루/사파이어 톤 적용 및 삭제 버튼 호버 인터랙션.
- **인라인 직접 편집(In-Place Edit)**: 별도 입력창 없이 목록에서 바로 더블클릭/F2로 수정 및 새 행 추가.
- **우측 일체형 툴바**: 조작 버튼(▲/▼/＋/✎/✕)을 목록 우측에 밀착 배치하고 목록 뷰 대폭 확장.
- **신규 모던 로고 적용**: 타이틀바 및 실행 파일 아이콘을 세련된 신규 로고로 교체.

---

## [v1.0.3] - 2026-08-26

### 🌐 English
- **Modern Dark UI**: Sleek dark theme with rounded corners and anti-aliased rendering.
- **Multi-Language Support (KOR/ENG)**: Instant real-time switching between English and Korean directly within the settings dialog.
- **Layout Optimization**: Fine-tuned spacing for radio buttons and controls.

### 🇰🇷 한국어
- **모던 다크 UI 적용**: 둥근 모서리와 안티앨리어싱이 적용된 세련된 다크 테마.
- **다국어 지원(KOR/ENG)**: 한국어와 영어를 설정창에서 실시간 전환 가능.
- 라디오 버튼 등 레이아웃 간격 최적화.

---

## [v1.0.2] - 2026-04-24

### 🌐 English
- Initial WebView2 migration and multi-monitor screensaver enhancements.

### 🇰🇷 한국어
- CefSharp에서 WebView2 런타임으로의 전환 및 다중 모니터 화면 보호기 안정화.
