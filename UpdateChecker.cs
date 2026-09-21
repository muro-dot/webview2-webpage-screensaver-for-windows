using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Web_Page_Screensaver
{
    /// <summary>
    /// GitHub Release 정보를 표현하는 모델
    /// </summary>
    public class GitHubReleaseInfo
    {
        public bool HasUpdate { get; set; }
        public string LatestVersion { get; set; }
        public string ReleaseUrl { get; set; }
        public string ReleaseNotes { get; set; }
    }

    /// <summary>
    /// GitHub Releases API를 조회하여 최신 업데이트 여부를 비동기로 검사하는 헬퍼
    /// </summary>
    public static class UpdateChecker
    {
        private const string REPO_OWNER = "muro-dot";
        private const string REPO_NAME = "Webview2_WebPage_Screensaver";
        private const string CURRENT_VERSION = "1.0.5";

        /// <summary>
        /// 백그라운드에서 GitHub 최신 릴리즈 정보를 비동기로 조회합니다.
        /// 실패 시 UI 지연이나 오류 팝업 없이 안전하게 HasUpdate = false를 반환합니다.
        /// </summary>
        public static async Task<GitHubReleaseInfo> CheckForUpdateAsync()
        {
            return await Task.Run(() =>
            {
                var result = new GitHubReleaseInfo { HasUpdate = false };
                try
                {
                    // TLS 1.2+ 강제 설정 (GitHub API 요구사항)
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    string apiUrl = $"https://api.github.com/repos/{REPO_OWNER}/{REPO_NAME}/releases/latest";
                    var request = (HttpWebRequest)WebRequest.Create(apiUrl);
                    request.UserAgent = "Webview2-Screensaver-UpdateChecker";
                    request.Timeout = 4000; // 4초 타임아웃
                    request.Method = "GET";

                    using (var response = (HttpWebResponse)request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        string json = reader.ReadToEnd();
                        var serializer = new JavaScriptSerializer();
                        var dict = serializer.Deserialize<System.Collections.Generic.Dictionary<string, object>>(json);

                        if (dict != null && dict.ContainsKey("tag_name"))
                        {
                            string tagName = dict["tag_name"].ToString().TrimStart('v', 'V');
                            string htmlUrl = dict.ContainsKey("html_url") ? dict["html_url"].ToString() : "";

                            result.LatestVersion = tagName;
                            result.ReleaseUrl = htmlUrl;

                            if (IsNewerVersion(tagName, CURRENT_VERSION))
                            {
                                result.HasUpdate = true;
                            }
                        }
                    }
                }
                catch
                {
                    // 오프라인이거나 API 제한일 때는 무시하고 안전 종료
                    result.HasUpdate = false;
                }
                return result;
            });
        }

        private static bool IsNewerVersion(string latestStr, string currentStr)
        {
            try
            {
                if (Version.TryParse(latestStr, out Version latest) &&
                    Version.TryParse(currentStr, out Version current))
                {
                    return latest > current;
                }
            }
            catch { }
            return false;
        }
    }
}
