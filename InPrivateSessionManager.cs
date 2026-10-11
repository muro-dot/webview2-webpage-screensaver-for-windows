using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;

namespace Web_Page_Screensaver
{
    /// <summary>
    /// InPrivate 격리 세션 및 사용자 데이터 폴더(User Data Folder)를 중앙 관리하는 매니저입니다.
    /// 1. 다중 모니터 환경에서 모니터마다 무분별하게 별도 폴더와 프로세스가 중복 생성되는 문제를 방지하기 위해 단일 세션 폴더를 공유합니다.
    /// 2. 비정상 종료 등으로 남아있는 오래된 임시 폴더를 프로그램 시작 시 안전하게 일괄 정리합니다.
    /// 3. 화면보호기 종료 시 사용된 임시 데이터를 깨끗이 파기하여 보안과 프라이버시를 유지합니다.
    /// </summary>
    public static class InPrivateSessionManager
    {
        private static readonly object _lockObj = new object();
        private static string _currentInPrivateFolder = null;
        private const string IN_PRIVATE_PREFIX = "Screensaver_InPrivate_";

        /// <summary>
        /// InPrivate 모드 여부에 따른 적절한 UserDataFolder 경로를 반환합니다.
        /// InPrivate가 켜져 있으면 전체 모니터가 단 1개의 고유 임시 디렉토리를 공유합니다.
        /// </summary>
        public static string GetUserDataFolder(bool inPrivate)
        {
            if (!inPrivate)
            {
                // 일반 모드: 로컬 앱 데이터 폴더의 영구 캐시/쿠키 저장소 사용
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LibraryScreensaver_Data");
            }

            lock (_lockObj)
            {
                if (string.IsNullOrEmpty(_currentInPrivateFolder))
                {
                    // 단일 실행 세션 동안 모든 모니터 폼이 공유할 고유 임시 디렉토리 1개만 생성
                    string folderName = IN_PRIVATE_PREFIX + Guid.NewGuid().ToString("N");
                    _currentInPrivateFolder = Path.Combine(Path.GetTempPath(), folderName);
                }
                return _currentInPrivateFolder;
            }
        }

        /// <summary>
        /// InPrivate 모드일 때 CoreWebView2EnvironmentOptions를 반환합니다.
        /// </summary>
        public static CoreWebView2EnvironmentOptions GetEnvironmentOptions(bool inPrivate)
        {
            if (inPrivate)
            {
                return new CoreWebView2EnvironmentOptions
                {
                    AdditionalBrowserArguments = "--inprivate"
                };
            }
            return null;
        }

        /// <summary>
        /// 프로그램 시작 시 이전 실행에서 크래시나 강제 종료로 남아있는 고아(Orphaned) 임시 디렉토리를 정리합니다.
        /// </summary>
        public static void CleanupOrphanedSessions()
        {
            Task.Run(() =>
            {
                try
                {
                    string tempPath = Path.GetTempPath();
                    var directories = Directory.GetDirectories(tempPath, IN_PRIVATE_PREFIX + "*");
                    foreach (var dir in directories)
                    {
                        // 현재 세션에서 사용 중인 폴더는 건너뜀
                        if (!string.IsNullOrEmpty(_currentInPrivateFolder) &&
                            string.Equals(dir, _currentInPrivateFolder, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        try
                        {
                            Directory.Delete(dir, true);
                        }
                        catch
                        {
                            // 다른 프로세스가 사용 중이거나 잠겨있으면 안전하게 무시
                        }
                    }
                }
                catch
                {
                    // 디렉토리 열람 실패 등 예외 무시
                }
            });
        }

        /// <summary>
        /// 프로그램 종료 시 현재 세션에서 사용한 InPrivate 임시 디렉토리를 디스크에서 삭제합니다.
        /// </summary>
        public static void CleanupCurrentSession()
        {
            string targetDir;
            lock (_lockObj)
            {
                targetDir = _currentInPrivateFolder;
                _currentInPrivateFolder = null;
            }

            if (string.IsNullOrEmpty(targetDir) || !Directory.Exists(targetDir))
            {
                return;
            }

            try
            {
                Directory.Delete(targetDir, true);
            }
            catch
            {
                // Edge 백그라운드 프로세스가 아직 종료 중이라 파일 핸들이 잠겨있을 수 있으므로,
                // 백그라운드 태스크에서 잠시 후 지연 재시도합니다.
                Task.Run(async () =>
                {
                    for (int i = 0; i < 5; i++)
                    {
                        await Task.Delay(1000);
                        try
                        {
                            if (Directory.Exists(targetDir))
                            {
                                Directory.Delete(targetDir, true);
                                break;
                            }
                        }
                        catch
                        {
                            // 마지막 시도까지 실패하면 다음 실행의 CleanupOrphanedSessions에서 청소됨
                        }
                    }
                });
            }
        }
    }
}
