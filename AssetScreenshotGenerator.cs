using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Web_Page_Screensaver
{
    /// <summary>
    /// 빌드 시 또는 개발용으로 최신 버전의 PreferencesForm UI를 
    /// 다크 모드 및 라이트 모드로 각각 캡처하여 assets 폴더에 저장하는 생성기 클래스입니다.
    /// 개인 URL 대신 항상 안전한 공식 예시 URL(https://example.com/screensaver)을 주입합니다.
    /// </summary>
    public static class AssetScreenshotGenerator
    {
        public static void GenerateAssets(string outputDirectory = null)
        {
            if (string.IsNullOrEmpty(outputDirectory))
            {
                outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets");
            }

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // 고해상도 DPI 환경 대응
            Application.EnableVisualStyles();

            using (var form = new PreferencesForm())
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.TopMost = true;
                form.Show();

                // 1. 개인 설정 대신 안전한 예시 URL 주입 및 UI 정리
                form.PrepareForScreenshot("https://example.com/screensaver");
                Application.DoEvents();
                Thread.Sleep(200);

                // 2. 다크 모드 테마 캡처
                ThemeManager.IsLightTheme = false;
                form.ApplyTheme(false);
                form.Refresh();
                Application.DoEvents();
                Thread.Sleep(200);

                CaptureFormWindow(form, Path.Combine(outputDirectory, "screenshot_dark.png"));
                // screenshot.png는 기본 다크 모드 스크린샷과 동일
                CaptureFormWindow(form, Path.Combine(outputDirectory, "screenshot.png"));

                // 3. 라이트 모드 테마 캡처
                ThemeManager.IsLightTheme = true;
                form.ApplyTheme(true);
                form.Refresh();
                Application.DoEvents();
                Thread.Sleep(200);

                CaptureFormWindow(form, Path.Combine(outputDirectory, "screenshot_light.png"));

                // 4. 영문 모드 캡처 (다국어 UI 문구 간섭 및 겹침 자가 검증용)
                form.ApplyLanguage("en");
                ThemeManager.IsLightTheme = false;
                form.ApplyTheme(false);
                form.Refresh();
                Application.DoEvents();
                Thread.Sleep(200);

                CaptureFormWindow(form, Path.Combine(outputDirectory, "screenshot_en.png"));

                // 정리
                form.DialogResult = DialogResult.Cancel;
                form.Close();
            }
        }

        private static void CaptureFormWindow(Form form, string savePath)
        {
            try
            {
                // DrawToBitmap을 사용하여 비대화형 빌드/콘솔 환경에서도 안정적으로 캡처
                Rectangle rect = new Rectangle(0, 0, form.Width, form.Height);
                using (var bitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb))
                {
                    form.DrawToBitmap(bitmap, rect);
                    bitmap.Save(savePath, ImageFormat.Png);
                    Console.WriteLine($"[AssetScreenshotGenerator] 캡처 성공: {Path.GetFileName(savePath)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AssetScreenshotGenerator] 캡처 실패 ({savePath}): {ex.Message}");
            }
        }
    }
}
