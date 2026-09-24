using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Web_Page_Screensaver
{
    /// <summary>
    /// 화면보호기 전체 설정을 담는 직렬화용 데이터 모델 DTO
    /// </summary>
    public class ScreensaverConfigDto
    {
        public string Version { get; set; } = "1.0.6";
        public DateTime ExportedAt { get; set; } = DateTime.Now;
        public string Language { get; set; }
        public string MultiScreenMode { get; set; }
        public bool CloseOnActivity { get; set; }
        public bool MuteAudio { get; set; }
        public bool InPrivate { get; set; }
        public bool ShowClockOverlay { get; set; }
        public string ClockPosition { get; set; } = "BottomRight";
        public List<List<string>> UrlsByScreen { get; set; }
        public List<int> RotationIntervalsByScreen { get; set; }
        public List<bool> RandomizeFlagByScreen { get; set; }
        public List<int> ZoomFactorsByScreen { get; set; }
    }

    /// <summary>
    /// 설정 내보내기(Export) 및 가져오기(Import) 로직을 전담하는 클래스
    /// </summary>
    public static class ConfigExportImport
    {
        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();

        /// <summary>
        /// 현재 설정을 JSON 파일로 대화상자를 통해 저장합니다.
        /// </summary>
        public static bool ExportToFile(PreferencesManager prefs, IWin32Window owner, string language)
        {
            bool isKo = (language == "ko");
            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = isKo ? "설정 파일 내보내기 (JSON)" : "Export Settings to JSON";
                sfd.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
                sfd.FileName = $"WebScreensaver_Backup_{DateTime.Now:yyyyMMdd}.json";

                if (sfd.ShowDialog(owner) == DialogResult.OK)
                {
                    try
                    {
                        var dto = new ScreensaverConfigDto
                        {
                            Language = prefs.Language,
                            MultiScreenMode = prefs.MultiScreenMode.ToString(),
                            CloseOnActivity = prefs.CloseOnActivity,
                            MuteAudio = prefs.MuteAudio,
                            InPrivate = prefs.InPrivate,
                            ShowClockOverlay = prefs.ShowClockOverlay,
                            ClockPosition = prefs.ClockPositionPref.ToString(),
                            UrlsByScreen = prefs.GetAllUrlsByScreenDirect(),
                            RotationIntervalsByScreen = prefs.GetAllIntervalsDirect(),
                            RandomizeFlagByScreen = prefs.GetAllRandomizeDirect(),
                            ZoomFactorsByScreen = prefs.GetAllZoomFactorsDirect()
                        };

                        string json = Serializer.Serialize(dto);
                        File.WriteAllText(sfd.FileName, json, System.Text.Encoding.UTF8);

                        MessageBox.Show(
                            isKo ? "설정 파일이 성공적으로 저장되었습니다." : "Settings exported successfully.",
                            isKo ? "성공" : "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            (isKo ? "설정 저장 중 오류가 발생했습니다: " : "Error exporting settings: ") + ex.Message,
                            isKo ? "오류" : "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// JSON 파일로부터 설정을 읽어들여 PreferencesManager에 적용합니다.
        /// </summary>
        public static bool ImportFromFile(PreferencesManager prefs, IWin32Window owner, string language)
        {
            bool isKo = (language == "ko");
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = isKo ? "설정 파일 가져오기 (JSON)" : "Import Settings from JSON";
                ofd.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";

                if (ofd.ShowDialog(owner) == DialogResult.OK)
                {
                    try
                    {
                        string json = File.ReadAllText(ofd.FileName, System.Text.Encoding.UTF8);
                        var dto = Serializer.Deserialize<ScreensaverConfigDto>(json);
                        if (dto == null)
                        {
                            throw new Exception("Invalid configuration file format.");
                        }

                        if (!string.IsNullOrEmpty(dto.Language)) prefs.Language = dto.Language;
                        if (!string.IsNullOrEmpty(dto.MultiScreenMode))
                        {
                            if (Enum.TryParse(dto.MultiScreenMode, out PreferencesManager.MultiScreenModeItem mode))
                            {
                                prefs.MultiScreenMode = mode;
                            }
                        }

                        prefs.CloseOnActivity = dto.CloseOnActivity;
                        prefs.MuteAudio = dto.MuteAudio;
                        prefs.InPrivate = dto.InPrivate;
                        prefs.ShowClockOverlay = dto.ShowClockOverlay;

                        if (!string.IsNullOrEmpty(dto.ClockPosition))
                        {
                            if (Enum.TryParse(dto.ClockPosition, out PreferencesManager.ClockPosition cp))
                            {
                                prefs.ClockPositionPref = cp;
                            }
                        }

                        if (dto.UrlsByScreen != null) prefs.SetAllUrlsByScreenDirect(dto.UrlsByScreen);
                        if (dto.RotationIntervalsByScreen != null) prefs.SetAllIntervalsDirect(dto.RotationIntervalsByScreen);
                        if (dto.RandomizeFlagByScreen != null) prefs.SetAllRandomizeDirect(dto.RandomizeFlagByScreen);
                        if (dto.ZoomFactorsByScreen != null) prefs.SetAllZoomFactorsDirect(dto.ZoomFactorsByScreen);

                        prefs.SavePreferences();

                        MessageBox.Show(
                            isKo ? "설정을 성공적으로 불러와 저장했습니다. 설정창에 반영됩니다." : "Settings imported and applied successfully.",
                            isKo ? "성공" : "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            (isKo ? "설정 불러오기 중 오류가 발생했습니다: " : "Error importing settings: ") + ex.Message,
                            isKo ? "오류" : "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
            return false;
        }
    }
}
