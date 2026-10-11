# 고도화 계획 (PLAN)

- 작성일: 2026-10-11
- 기준 버전: v1.0.6 (.NET Framework 4.8 / WinForms / WebView2 1.0.3912.50 / Costura.Fody 단일 exe → `.scr`)
- 상태: **계획 수립 완료, 사용자 확인 대기 (구현 미착수)**

## 현재 단계 / 다음 할 일 / 미해결 이슈

- 현재 단계: 분석 완료, Phase 0 착수 전
- 다음 할 일: 사용자가 Phase 범위 확정 → Phase 0부터 구현
- 미해결 이슈: 아래 "확인 필요 사항" 참고

## 1. 분석 요약

구조: `Program.cs`(진입점·입력 감지) → `ScreensaverForm`(모니터별 WebView2 + URL 로테이션) / `PreferencesForm`(설정 UI) / `PreferencesManager`(레지스트리 저장) / `FallbackHtmlProvider`(오프라인 시계·HUD) / `UpdateChecker` / `ConfigExportImport` / `AssetScreenshotGenerator`.
강점: WebView2 전환, 다중 모니터 3모드, 폴백 시계, 백업/복원, 다국어·테마 대응.

### 발견한 문제 (우선순위순)

| # | 구분 | 내용 | 위치 |
|---|---|---|---|
| 1 | 안정성 | 키보드 훅이 `nCode`/`wParam`을 보지 않고 키 **뗌(KEYUP) 포함 모든 이벤트**에 종료. 우클릭 "테스트"로 실행 직후 Enter 키 뗌에 즉시 종료될 위험 | Program.cs:156-165 |
| 2 | 안정성 | 레지스트리 값 손상 시 `Enum.Parse`/`bool.Parse`/`(string)` 캐스팅 예외 → 화면보호기 실행 자체가 크래시 | PreferencesManager.cs:349-354 |
| 3 | 안정성 | 크래시·로드 실패 시 복구 없음. 폴백 시계 후 **URL이 1개면 영원히 재시도 안 함**, `ProcessFailed`(렌더러 사망) 미처리 | ScreensaverForm.cs:110-130 |
| 4 | 보안/UX | 새 창·팝업, 카메라/마이크/위치 권한 요청, 다운로드, 컨텍스트 메뉴, DevTools 차단 설정 없음. 무인 상태에서 권한 팝업이 뜰 수 있음 | ScreensaverForm.cs |
| 5 | 정확성 | InPrivate를 비공식 인자 `--inprivate`로 적용 → 실제 적용 불확실. 공식 `CoreWebView2ControllerOptions.IsInPrivateModeEnabled` 사용 필요 | ScreensaverForm.cs:93-99 |
| 6 | 정확성 | URL 스킴 검증·정규화 없음(`example.com` 입력 시 조용히 실패, `file:`/`javascript:` 허용). URL을 공백으로 구분 저장해 공백 포함 URL 불가 | ScreensaverForm.cs:207, PreferencesManager.cs:377 |
| 7 | 유지보수 | 버전이 `UpdateChecker.CURRENT_VERSION`(문자열)과 `AssemblyInfo`에 **이중 관리** → 불일치 시 잘못된 업데이트 알림 | UpdateChecker.cs:27 |
| 8 | 유지보수 | URL 저장 구조의 기존 TODO(화면 순서 뒤섞임, "전면 재작성이 맞다"고 작성됨) | PreferencesManager.cs:184-190 |
| 9 | 빌드 | PostBuild가 **빌드마다 exe를 실행**해 스크린샷 생성(CI/헤드리스에서 실패 가능, assets 매번 변경). `nuget.exe` 바이너리 커밋, CI·테스트·설치 스크립트 없음 | csproj:241 |
| 10 | 화질 | `SetProcessDPIAware()`(시스템 DPI 인식)로 고배율 모니터에서 흐림 가능 → Zoom으로 우회 중. PerMonitorV2 매니페스트가 근본 해결 | Program.cs:23 |
| 11 | 구조 | `ModernUiControls.cs` 1096줄, `PreferencesForm.cs` 638줄로 모듈화 규칙 초과 | 해당 파일 |
| 12 | 중복 | 마우스 감지가 2중(타이머 3px / 메시지필터 10px), 종료 경로가 3개(`Application.Exit`/`Close`/폼 이벤트) | Program.cs, ScreensaverForm.cs |
| 13 | 효율 | 모니터별로 WebView2 1개씩 생성 → 다중 모니터에서 메모리 증가. 배터리/디스플레이 꺼짐 시 일시정지 없음 | ScreensaverForm.cs |

## 2. 단계별 계획

### Phase 0 — 안정성 핫픽스 (v1.0.7 후보, 위험 낮음)
- [ ] 키보드 훅: `nCode >= 0` + `WM_KEYDOWN/WM_SYSKEYDOWN`만 처리, 시작 후 유예 시간(1.5초) 적용
- [ ] 레지스트리 로드를 `TryParse` + 기본값 폴백으로 전환 (손상돼도 실행)
- [ ] `ProcessFailed` 처리 + 로드 실패 시 N초 후 자동 재시도 (URL 1개 케이스 포함)
- [ ] 버전 단일 출처화: `UpdateChecker`가 `Assembly` 버전을 읽도록 변경
- [ ] 종료 로직을 한 곳(`ExitScreensaver()`)으로 통합, 타이머·WebView2 `Dispose` 보장
- 변경 파일: Program.cs, PreferencesManager.cs, ScreensaverForm.cs, UpdateChecker.cs

### Phase 1 — WebView2 보안·정확성
- [ ] InPrivate를 공식 API(`ControllerOptions.IsInPrivateModeEnabled`)로 교체 후 실제 데이터 미잔류 검증
- [ ] 새 창(`NewWindowRequested`)을 같은 창 이동 또는 차단, 권한 요청 자동 거부, 다운로드 취소
- [ ] 컨텍스트 메뉴·DevTools·상태바·자동완성·비밀번호 저장 비활성화 (`CoreWebView2Settings`)
- [ ] URL 정규화: 스킴 없으면 `https://` 자동 보정, 허용 스킴 `http/https`(+선택적 `file`), 설정창 저장 시 검증 메시지
- 변경 파일: ScreensaverForm.cs, PreviewDialog.cs, PreferencesForm.cs, PreferencesManager.cs(ScreensaverUrlItem)

### Phase 2 — 설정 저장 구조 개선
- [ ] URL을 "공백 구분 문자열"에서 **화면별 JSON 목록**으로 재설계 (기존 TODO 해소)
- [ ] 구버전 레지스트리 값 자동 마이그레이션 + 실패 시 원본 보존
- [ ] `ConfigExportImport`와 스키마 버전 필드 통일
- 리스크: 기존 사용자 설정 유실 → 마이그레이션 단위 테스트와 백업 키 유지 필수

### Phase 3 — 사용자 가치 기능 (선택, 우선순위 협의)
- [ ] 배터리/절전 시 일시정지 또는 재생 정지 옵션
- [ ] 로컬 HTML/이미지 폴더 슬라이드쇼 (`file:` 허용 시)
- [ ] 시간대·요일별 URL 스케줄, URL별 새로고침 주기
- [ ] 다중 모니터에서 WebView2 환경 공유로 메모리 절감
- [ ] DPI: PerMonitorV2 매니페스트 적용 후 Zoom과의 상호작용 검증

### Phase 4 — 품질·배포 인프라
- [ ] PostBuild 스크린샷 생성을 별도 대상(`Release`에서만 / 수동 스크립트)으로 분리
- [ ] 순수 로직(URL 파싱, 버전 비교, 설정 파싱) 단위 테스트 프로젝트 추가
- [ ] GitHub Actions: 빌드 + 테스트 + 태그 시 릴리스 ZIP 자동 생성
- [ ] `nuget.exe` 저장소 제거, `PackageReference` 전환 검토
- [ ] `ModernUiControls.cs`, `PreferencesForm.cs` 기능별 파일 분리

## 3. 검증 기준 (각 Phase 공통)
- Release 빌드 성공(Exit 0), 경고/타입 오류 없음, 신규 테스트 통과
- `.scr` 수동 확인: `/s` 실행, `/c` 설정, 테스트 직후 즉시 종료되지 않음, 오프라인 폴백·복구
- UI 변경 시: KOR/ENG × 다크/라이트 스크린샷을 직접 확인(AGENTS.md 원칙)
- 완료 후 `docs/CHANGELOG.md`에 날짜·변경 파일·검증 출력 기록

## 4. 확인 필요 사항
1. 이번에 진행할 범위: Phase 0~1을 먼저 v1.0.7로 묶을지, Phase 3 기능을 우선할지
2. `file:` 로컬 페이지 허용 여부 (보안과 편의의 트레이드오프)
3. 프레임워크는 .NET Framework 4.8 유지 권장 (Win11 기본 내장 → 별도 런타임 설치 불필요, 단일 exe 배포에 유리). Tauri/.NET 8 이전은 이득 대비 비용이 커서 비권장
4. 참고: `PreferencesForm`/`ModernUiControls` 내부는 이번 분석에서 정독하지 못함 (다국어 문자열 분리 방식 등은 Phase 4 착수 시 추가 점검)
