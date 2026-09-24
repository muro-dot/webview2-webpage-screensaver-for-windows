using System;

namespace Web_Page_Screensaver
{
    /// <summary>
    /// 네트워크 단절/오류 시 표시할 우아한 Fallback 디지털 시계 HTML 및
    /// 웹페이지 상단/하단에 띄울 글래스모피즘 HUD 시계 스크립트를 생성하는 헬퍼 클래스
    /// </summary>
    public static class FallbackHtmlProvider
    {
        /// <summary>
        /// 인터넷이 끊기거나 URL 접근이 불가할 때 못생긴 브라우저 에러창 대신
        /// 화면보호기로 작동할 수 있도록 미려한 네온 다크 디지털 시계를 생성합니다.
        /// </summary>
        public static string GetFallbackClockHtml(string failedUrl, string language)
        {
            bool isKo = (language == "ko");
            string statusTitle = isKo ? "네트워크 연결 대기 중" : "Waiting for Network Connection";
            string statusSub = isKo 
                ? "페이지 로드 실패: 재시도 대기 중..." 
                : "Failed to load page. Retrying automatically...";

            return @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Screensaver Fallback Clock</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body {
            background: radial-gradient(circle at center, #181824 0%, #0c0c12 100%);
            color: #f4f4f5;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
            height: 100vh;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            overflow: hidden;
            user-select: none;
        }
        .clock-container {
            text-align: center;
            background: rgba(255, 255, 255, 0.03);
            border: 1px solid rgba(255, 255, 255, 0.08);
            backdrop-filter: blur(20px);
            padding: 48px 64px;
            border-radius: 24px;
            box-shadow: 0 30px 60px rgba(0, 0, 0, 0.6);
            animation: fadeIn 1s ease-out;
        }
        .time {
            font-size: 6.5rem;
            font-weight: 700;
            letter-spacing: -2px;
            background: linear-gradient(135deg, #ffffff 30%, #a1a1aa 100%);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            font-variant-numeric: tabular-nums;
            line-height: 1.1;
            margin-bottom: 12px;
        }
        .date {
            font-size: 1.5rem;
            color: #71717a;
            letter-spacing: 1px;
            text-transform: uppercase;
            margin-bottom: 28px;
        }
        .status-badge {
            display: inline-flex;
            align-items: center;
            gap: 10px;
            padding: 8px 18px;
            background: rgba(239, 68, 68, 0.12);
            border: 1px solid rgba(239, 68, 68, 0.3);
            border-radius: 999px;
            color: #f87171;
            font-size: 0.95rem;
            font-weight: 500;
        }
        .pulse-dot {
            width: 8px;
            height: 8px;
            background-color: #ef4444;
            border-radius: 50%;
            animation: pulse 1.8s infinite;
        }
        .url-info {
            margin-top: 14px;
            color: #52525b;
            font-size: 0.85rem;
            max-width: 480px;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
        }
        @keyframes pulse {
            0% { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(239, 68, 68, 0.7); }
            70% { transform: scale(1.1); box-shadow: 0 0 0 10px rgba(239, 68, 68, 0); }
            100% { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(239, 68, 68, 0); }
        }
        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(12px); }
            to { opacity: 1; transform: translateY(0); }
        }
    </style>
</head>
<body>
    <div class=""clock-container"">
        <div class=""time"" id=""time"">--:--:--</div>
        <div class=""date"" id=""date"">----.--.--</div>
        <div class=""status-badge"">
            <span class=""pulse-dot""></span>
            <span>" + statusTitle + @"</span>
        </div>
        <div class=""url-info"">" + statusSub + @" (" + System.Net.WebUtility.HtmlEncode(failedUrl) + @")</div>
    </div>

    <script>
        function updateClock() {
            const now = new Date();
            const hours = String(now.getHours()).padStart(2, '0');
            const minutes = String(now.getMinutes()).padStart(2, '0');
            const seconds = String(now.getSeconds()).padStart(2, '0');
            document.getElementById('time').textContent = `${hours}:${minutes}:${seconds}`;

            const options = { weekday: 'short', year: 'numeric', month: 'short', day: 'numeric' };
            document.getElementById('date').textContent = now.toLocaleDateString(undefined, options);
        }
        updateClock();
        setInterval(updateClock, 1000);
    </script>
</body>
</html>";
        }

        /// <summary>
        /// 웹페이지 모서리에 반투명 글래스모피즘 디지털 시계/날짜 위젯을 띄우는 주입 스크립트를 반환합니다.
        /// 사용자가 선택한 모서리 위치(상하좌우 4모서리)에 맞춰 배치되며,
        /// pointer-events: none 으로 마우스 조작을 일체 방해하지 않습니다.
        /// </summary>
        /// <param name="position">시계가 표시될 모서리 위치 (기본: BottomRight)</param>
        public static string GetClockOverlayScript(PreferencesManager.ClockPosition position = PreferencesManager.ClockPosition.BottomRight)
        {
            string posCss;
            string textAlign;

            switch (position)
            {
                case PreferencesManager.ClockPosition.TopLeft:
                    posCss = "host.style.top = '24px'; host.style.left = '24px';";
                    textAlign = "left";
                    break;
                case PreferencesManager.ClockPosition.TopRight:
                    posCss = "host.style.top = '24px'; host.style.right = '24px';";
                    textAlign = "right";
                    break;
                case PreferencesManager.ClockPosition.BottomLeft:
                    posCss = "host.style.bottom = '24px'; host.style.left = '24px';";
                    textAlign = "left";
                    break;
                case PreferencesManager.ClockPosition.BottomRight:
                default:
                    posCss = "host.style.bottom = '24px'; host.style.right = '24px';";
                    textAlign = "right";
                    break;
            }

            string scriptTemplate = @"
(function() {
    if (document.getElementById('webview2-screensaver-hud-clock')) return;

    const host = document.createElement('div');
    host.id = 'webview2-screensaver-hud-clock';
    host.style.position = 'fixed';
    __POS_CSS__
    host.style.zIndex = '2147483647';
    host.style.pointerEvents = 'none';
    host.style.userSelect = 'none';
    host.style.fontFamily = '-apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif';

    const box = document.createElement('div');
    box.style.background = 'rgba(15, 15, 20, 0.72)';
    box.style.border = '1px solid rgba(255, 255, 255, 0.12)';
    box.style.backdropFilter = 'blur(12px)';
    box.style.webkitBackdropFilter = 'blur(12px)';
    box.style.borderRadius = '14px';
    box.style.padding = '10px 18px';
    box.style.color = '#f4f4f5';
    box.style.boxShadow = '0 10px 30px rgba(0,0,0,0.45)';
    box.style.textAlign = '__TEXT_ALIGN__';

    const timeEl = document.createElement('div');
    timeEl.style.fontSize = '26px';
    timeEl.style.fontWeight = '700';
    timeEl.style.letterSpacing = '-0.5px';
    timeEl.style.fontVariantNumeric = 'tabular-nums';
    timeEl.style.color = '#ffffff';
    timeEl.style.lineHeight = '1.1';

    const dateEl = document.createElement('div');
    dateEl.style.fontSize = '12px';
    dateEl.style.color = '#a1a1aa';
    dateEl.style.marginTop = '4px';

    box.appendChild(timeEl);
    box.appendChild(dateEl);
    host.appendChild(box);
    document.body.appendChild(host);

    function update() {
        const now = new Date();
        const h = String(now.getHours()).padStart(2, '0');
        const m = String(now.getMinutes()).padStart(2, '0');
        const s = String(now.getSeconds()).padStart(2, '0');
        timeEl.textContent = `${h}:${m}:${s}`;
        dateEl.textContent = now.toLocaleDateString(undefined, { weekday: 'short', month: 'short', day: 'numeric' });
    }
    update();
    setInterval(update, 1000);
})();
";
            return scriptTemplate
                .Replace("__POS_CSS__", posCss)
                .Replace("__TEXT_ALIGN__", textAlign);
        }
    }
}
