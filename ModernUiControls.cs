using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Web_Page_Screensaver
{
    /// <summary>
    /// 모던 테마 색상 팔레트 인터페이스 및 데이터 모델
    /// 다크 모드와 라이트(화이트) 모드의 모든 UI 색상 토큰을 일관성 있게 관리합니다.
    /// </summary>
    public class ThemePalette
    {
        public Color Background { get; set; }           // 폼 전체 메인 배경
        public Color CardBackground { get; set; }       // 카드/패널 배경
        public Color CardBorder { get; set; }           // 카드/패널 테두리
        public Color InputBackground { get; set; }      // 텍스트박스/리스트 배경
        public Color InputBorder { get; set; }          // 텍스트박스/리스트 테두리

        public Color TextPrimary { get; set; }          // 메인 텍스트
        public Color TextSecondary { get; set; }        // 보조 텍스트
        public Color TextMuted { get; set; }            // 비활성 텍스트

        public Color Accent { get; set; }               // 브랜드 액센트 (Blue-600)
        public Color AccentHover { get; set; }          // 액센트 호버
        public Color AccentPressed { get; set; }        // 액센트 클릭

        public Color SecondaryBtn { get; set; }         // 보조 버튼 기본
        public Color SecondaryBtnHover { get; set; }    // 보조 버튼 호버
        public Color SecondaryBtnPressed { get; set; }  // 보조 버튼 클릭
        public Color SecondaryBtnBorder { get; set; }   // 보조 버튼 테두리

        public Color Danger { get; set; }               // 경고/삭제 기본
        public Color DangerHover { get; set; }          // 경고/삭제 호버
        public Color DangerPressed { get; set; }        // 경고/삭제 클릭

        public Color TabIndicator { get; set; }         // 활성 탭 하단 액센트 바
    }

    /// <summary>
    /// 윈도우 시스템 테마 감지 및 다크/라이트 모드 자동 전환 관리자
    /// </summary>
    public static class ThemeManager
    {
        // 1. 다크 테마 색상 정의 (Zinc/Slate Deep Dark)
        public static readonly ThemePalette Dark = new ThemePalette
        {
            Background = Color.FromArgb(20, 20, 23),
            CardBackground = Color.FromArgb(28, 28, 32),
            CardBorder = Color.FromArgb(46, 46, 54),
            InputBackground = Color.FromArgb(15, 15, 17),
            InputBorder = Color.FromArgb(52, 52, 60),

            TextPrimary = Color.FromArgb(244, 244, 245),
            TextSecondary = Color.FromArgb(161, 161, 170),
            TextMuted = Color.FromArgb(113, 113, 122),

            Accent = Color.FromArgb(41, 103, 195),             // 차분하고 세련된 슬레이트 사파이어 (과도한 쨍함 제거)
            AccentHover = Color.FromArgb(56, 125, 225),        // 부드러운 액센트 호버
            AccentPressed = Color.FromArgb(30, 80, 160),

            SecondaryBtn = Color.FromArgb(36, 36, 42),
            SecondaryBtnHover = Color.FromArgb(50, 50, 58),
            SecondaryBtnPressed = Color.FromArgb(28, 28, 33),
            SecondaryBtnBorder = Color.FromArgb(46, 46, 54),

            Danger = Color.FromArgb(185, 45, 45),              // 절제된 크림슨
            DangerHover = Color.FromArgb(215, 55, 55),         // 호버 시 경고 레드
            DangerPressed = Color.FromArgb(160, 30, 30),

            TabIndicator = Color.FromArgb(56, 125, 225)
        };

        // 2. 라이트 테마 색상 정의 (Soft Clean White & Modern Slate)
        public static readonly ThemePalette Light = new ThemePalette
        {
            Background = Color.FromArgb(245, 246, 250),        // 부드러운 오프화이트 배경
            CardBackground = Color.FromArgb(255, 255, 255),    // 퓨어 화이트 카드
            CardBorder = Color.FromArgb(226, 232, 240),        // 은은한 라이트 테두리
            InputBackground = Color.FromArgb(248, 250, 252),   // 정갈한 입력 필드 배경
            InputBorder = Color.FromArgb(203, 213, 225),       // 입력창 연한 테두리

            TextPrimary = Color.FromArgb(15, 23, 42),          // 딥 슬레이트 블랙 (최고의 가독성)
            TextSecondary = Color.FromArgb(71, 85, 105),       // 부드러운 슬레이트 보조 텍스트
            TextMuted = Color.FromArgb(148, 163, 184),         // 비활성 텍스트

            Accent = Color.FromArgb(30, 41, 59),               // 모던 슬레이트 딥 네이비 (원색 블루 대신 최고급 프리미엄 톤)
            AccentHover = Color.FromArgb(51, 65, 85),          // 부드러운 슬레이트 700
            AccentPressed = Color.FromArgb(15, 23, 42),

            SecondaryBtn = Color.FromArgb(241, 245, 249),
            SecondaryBtnHover = Color.FromArgb(226, 232, 240),
            SecondaryBtnPressed = Color.FromArgb(203, 213, 225),
            SecondaryBtnBorder = Color.FromArgb(226, 232, 240),

            Danger = Color.FromArgb(210, 45, 45),              // 절제된 소프트 레드
            DangerHover = Color.FromArgb(225, 60, 60),
            DangerPressed = Color.FromArgb(180, 35, 35),

            TabIndicator = Color.FromArgb(30, 41, 59)
        };

        private static bool isLightTheme = false;

        public static bool IsLightTheme
        {
            get => isLightTheme;
            set
            {
                if (isLightTheme != value)
                {
                    isLightTheme = value;
                    ThemeChanged?.Invoke();
                }
            }
        }

        public static ThemePalette Colors => isLightTheme ? Light : Dark;

        public static event Action ThemeChanged;

        /// <summary>
        /// Windows 10/11 레지스트리를 조회하여 시스템 앱 테마가 라이트 모드인지 확인합니다.
        /// </summary>
        public static bool CheckWindowsLightTheme()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("AppsUseLightTheme");
                        if (value is int intVal)
                        {
                            return intVal == 1;
                        }
                    }
                }
            }
            catch { }
            return false; // 기본값 다크
        }

        // DWM API를 통한 Windows 타이틀바 다크/라이트 테마 제어
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        /// <summary>
        /// 윈도우 창의 타이틀바(제목 표시줄 및 닫기 버튼)를 현재 테마에 맞춰 변경합니다.
        /// </summary>
        public static void SetFormTitleBarTheme(IntPtr hWnd, bool isLight)
        {
            if (hWnd == IntPtr.Zero) return;

            try
            {
                int useDarkMode = isLight ? 0 : 1;
                // Windows 10 20H1 이상 및 Windows 11 (Attr 20)
                int result = DwmSetWindowAttribute(hWnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int));
                if (result != 0)
                {
                    // 구버전 Windows 10 (Attr 19)
                    DwmSetWindowAttribute(hWnd, DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, ref useDarkMode, sizeof(int));
                }
            }
            catch { }
        }
    }

    /// <summary>
    /// 기존 코드와의 완벽한 하위 호환성을 위한 DarkColors 프록시
    /// ThemeManager.Colors로 동적 매핑되어 실시간 테마 변경을 투명하게 반영합니다.
    /// </summary>
    public static class DarkColors
    {
        public static Color Background => ThemeManager.Colors.Background;
        public static Color CardBackground => ThemeManager.Colors.CardBackground;
        public static Color CardBorder => ThemeManager.Colors.CardBorder;
        public static Color InputBackground => ThemeManager.Colors.InputBackground;
        public static Color InputBorder => ThemeManager.Colors.InputBorder;

        public static Color TextPrimary => ThemeManager.Colors.TextPrimary;
        public static Color TextSecondary => ThemeManager.Colors.TextSecondary;
        public static Color TextMuted => ThemeManager.Colors.TextMuted;

        public static Color Accent => ThemeManager.Colors.Accent;
        public static Color AccentHover => ThemeManager.Colors.AccentHover;
        public static Color AccentPressed => ThemeManager.Colors.AccentPressed;

        public static Color SecondaryBtn => ThemeManager.Colors.SecondaryBtn;
        public static Color SecondaryBtnHover => ThemeManager.Colors.SecondaryBtnHover;
        public static Color SecondaryBtnPressed => ThemeManager.Colors.SecondaryBtnPressed;

        public static Color Danger => ThemeManager.Colors.Danger;
        public static Color DangerHover => ThemeManager.Colors.DangerHover;
        public static Color DangerPressed => ThemeManager.Colors.DangerPressed;
    }

    /// <summary>
    /// 모던 스타일 플랫 둥근 버튼 컨트롤
    /// </summary>
    public enum ModernButtonStyle
    {
        Primary,
        Secondary,
        Danger,
        Ghost,
        Segment
    }

    public class ModernButton : Button
    {
        private ModernButtonStyle style = ModernButtonStyle.Secondary;
        private int borderRadius = 6;
        private bool isHovered = false;
        private bool isPressed = false;
        private bool isSelected = false;

        public ModernButtonStyle Style
        {
            get => style;
            set { style = value; Invalidate(); }
        }

        public int BorderRadius
        {
            get => borderRadius;
            set { borderRadius = value; Invalidate(); }
        }

        public bool IsSelected
        {
            get => isSelected;
            set { isSelected = value; Invalidate(); }
        }

        public ModernButton()
        {
            SetStyle(ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.ResizeRedraw | 
                     ControlStyles.SupportsTransparentBackColor, true);

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            FlatAppearance.CheckedBackColor = Color.Transparent;
            UseVisualStyleBackColor = false;
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            Size = new Size(80, 30);

            ThemeManager.ThemeChanged += OnThemeChanged;
        }

        private void OnThemeChanged()
        {
            if (IsDisposed) return;
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ThemeManager.ThemeChanged -= OnThemeChanged;
            }
            base.Dispose(disposing);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            isPressed = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            if (mevent.Button == MouseButtons.Left)
            {
                isPressed = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            isPressed = false;
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // 기본 사각 배경 차단
        }

        private Color GetSolidParentBackColor()
        {
            Control c = Parent;
            while (c != null && (c.BackColor == Color.Transparent || c.BackColor.A < 255))
            {
                c = c.Parent;
            }
            return c != null ? c.BackColor : ThemeManager.Colors.Background;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 1. 부모 컨트롤 배경색으로 채우기
            using (var parentBrush = new SolidBrush(GetSolidParentBackColor()))
            {
                g.FillRectangle(parentBrush, ClientRectangle);
            }

            var colors = ThemeManager.Colors;
            Color bgColor;
            Color textColor = colors.TextPrimary;
            Color borderColor = Color.Transparent;

            switch (style)
            {
                case ModernButtonStyle.Primary:
                    bgColor = isPressed ? colors.AccentPressed : (isHovered ? colors.AccentHover : colors.Accent);
                    textColor = Color.White;
                    break;

                case ModernButtonStyle.Danger:
                    if (isPressed)
                    {
                        bgColor = colors.DangerPressed;
                        textColor = Color.White;
                    }
                    else if (isHovered)
                    {
                        bgColor = colors.DangerHover;
                        textColor = Color.White;
                    }
                    else
                    {
                        // 평상시에는 튀지 않고 주변과 조화로운 세컨더리 서피스 + 소프트 레드 텍스트
                        bgColor = colors.SecondaryBtn;
                        textColor = ThemeManager.IsLightTheme ? Color.FromArgb(195, 40, 40) : Color.FromArgb(248, 113, 113);
                        borderColor = colors.SecondaryBtnBorder;
                    }
                    break;

                case ModernButtonStyle.Segment:
                    if (isSelected)
                    {
                        bgColor = colors.Accent;
                        textColor = Color.White;
                    }
                    else
                    {
                        bgColor = isHovered ? colors.SecondaryBtnHover : colors.SecondaryBtn;
                        textColor = isHovered ? colors.TextPrimary : colors.TextSecondary;
                        borderColor = colors.CardBorder;
                    }
                    break;

                case ModernButtonStyle.Ghost:
                    bgColor = isPressed ? colors.SecondaryBtnPressed : (isHovered ? colors.SecondaryBtnHover : Color.Transparent);
                    textColor = isHovered ? colors.TextPrimary : colors.TextSecondary;
                    borderColor = isHovered ? colors.CardBorder : Color.Transparent;
                    break;

                case ModernButtonStyle.Secondary:
                default:
                    bgColor = isPressed ? colors.SecondaryBtnPressed : (isHovered ? colors.SecondaryBtnHover : colors.SecondaryBtn);
                    borderColor = colors.CardBorder;
                    textColor = colors.TextPrimary;
                    break;
            }

            if (!Enabled)
            {
                bgColor = ThemeManager.IsLightTheme ? Color.FromArgb(241, 245, 249) : Color.FromArgb(28, 28, 33);
                textColor = colors.TextMuted;
                borderColor = Color.Transparent;
            }

            var rect = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
            using (var path = GetRoundedRectangleF(rect, borderRadius))
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                if (borderColor != Color.Transparent)
                {
                    using (var pen = new Pen(borderColor, 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // 텍스트 출력
            var textRect = new Rectangle(0, 0, Width, Height);
            TextRenderer.DrawText(g, Text, Font, textRect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
        }

        public static GraphicsPath GetRoundedRectangleF(RectangleF rect, float radius)
        {
            var path = new GraphicsPath();
            float diameter = radius * 2f;

            if (radius <= 0f || diameter >= rect.Width || diameter >= rect.Height)
            {
                path.AddRectangle(rect);
                return path;
            }

            var arc = new RectangleF(rect.Location, new SizeF(diameter, diameter));

            // Top-left
            path.AddArc(arc, 180, 90);

            // Top-right
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom-right
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom-left
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }

    /// <summary>
    /// 모던 카드형 컨테이너 패널
    /// </summary>
    public class ModernCard : Panel
    {
        private int borderRadius = 8;
        public Color BorderColor { get; set; }

        public int BorderRadius
        {
            get => borderRadius;
            set { borderRadius = value; Invalidate(); }
        }

        public ModernCard()
        {
            BorderColor = ThemeManager.Colors.CardBorder;
            SetStyle(ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.ResizeRedraw | 
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = ThemeManager.Colors.CardBackground;
            Padding = new Padding(12);

            ThemeManager.ThemeChanged += OnThemeChanged;
        }

        private void OnThemeChanged()
        {
            if (IsDisposed) return;
            BorderColor = ThemeManager.Colors.CardBorder;
            BackColor = ThemeManager.Colors.CardBackground;
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ThemeManager.ThemeChanged -= OnThemeChanged;
            }
            base.Dispose(disposing);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // 기본 사각 배경 차단
        }

        private Color GetSolidParentBackColor()
        {
            Control c = Parent;
            while (c != null && (c.BackColor == Color.Transparent || c.BackColor.A < 255))
            {
                c = c.Parent;
            }
            return c != null ? c.BackColor : ThemeManager.Colors.Background;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using (var parentBrush = new SolidBrush(GetSolidParentBackColor()))
            {
                g.FillRectangle(parentBrush, ClientRectangle);
            }

            var rect = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
            using (var path = ModernButton.GetRoundedRectangleF(rect, borderRadius))
            {
                using (var brush = new SolidBrush(BackColor))
                {
                    g.FillPath(brush, path);
                }

                if (BorderColor != Color.Transparent)
                {
                    using (var pen = new Pen(BorderColor, 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 모던 플랫 탭 컨트롤
    /// </summary>
    public class ModernTabControl : TabControl
    {
        public ModernTabControl()
        {
            SetStyle(ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.ResizeRedraw, true);
            DrawMode = TabDrawMode.OwnerDrawFixed;
            SizeMode = TabSizeMode.Normal;
            ItemSize = new Size(140, 36);
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

            ThemeManager.ThemeChanged += OnThemeChanged;
        }

        private void OnThemeChanged()
        {
            if (IsDisposed) return;
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ThemeManager.ThemeChanged -= OnThemeChanged;
            }
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var colors = ThemeManager.Colors;

            // 전체 배경 지우기
            using (var bgBrush = new SolidBrush(colors.Background))
            {
                g.FillRectangle(bgBrush, ClientRectangle);
            }

            // 탭 헤더 하단 라인
            using (var linePen = new Pen(colors.CardBorder, 1f))
            {
                g.DrawLine(linePen, 0, ItemSize.Height + 2, Width, ItemSize.Height + 2);
            }

            for (int i = 0; i < TabCount; i++)
            {
                var tabRect = GetTabRect(i);
                bool isSelected = (SelectedIndex == i);

                // 선택된 탭 배경 및 활성 인디케이터
                if (isSelected)
                {
                    using (var tabBg = new SolidBrush(colors.CardBackground))
                    {
                        g.FillRectangle(tabBg, tabRect.X, tabRect.Y, tabRect.Width, tabRect.Height + 2);
                    }

                    // 상단/좌우 얇은 테두리
                    using (var borderPen = new Pen(colors.CardBorder, 1f))
                    {
                        g.DrawLine(borderPen, tabRect.X, tabRect.Y, tabRect.X, tabRect.Bottom + 1);
                        g.DrawLine(borderPen, tabRect.Right, tabRect.Y, tabRect.Right, tabRect.Bottom + 1);
                    }

                    // 하단 액센트 바
                    using (var accentBrush = new SolidBrush(colors.TabIndicator))
                    {
                        g.FillRectangle(accentBrush, tabRect.X, tabRect.Bottom, tabRect.Width, 3);
                    }
                }

                Color textColor = isSelected ? colors.TextPrimary : colors.TextSecondary;
                var font = isSelected ? new Font(Font, FontStyle.Bold) : Font;

                TextRenderer.DrawText(g, TabPages[i].Text, font, tabRect, textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
            }
        }
    }

    /// <summary>
    /// 모던 스타일 라디오 버튼 (테마 연동)
    /// </summary>
    public class ModernRadioButton : RadioButton
    {
        private bool isHovered = false;

        public ModernRadioButton()
        {
            SetStyle(ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.ResizeRedraw | 
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 9f);
            ForeColor = ThemeManager.Colors.TextPrimary;

            ThemeManager.ThemeChanged += OnThemeChanged;
        }

        private void OnThemeChanged()
        {
            if (IsDisposed) return;
            ForeColor = ThemeManager.Colors.TextPrimary;
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ThemeManager.ThemeChanged -= OnThemeChanged;
            }
            base.Dispose(disposing);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // 기본 사각 배경 차단
        }

        private Color GetParentBackColor()
        {
            Control c = Parent;
            while (c != null && (c.BackColor == Color.Transparent || c.BackColor.A < 255))
            {
                c = c.Parent;
            }
            return c != null ? c.BackColor : ThemeManager.Colors.Background;
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size textSize = TextRenderer.MeasureText(Text, Font);
            return new Size(textSize.Width + 28, Math.Max(textSize.Height + 4, 24));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var colors = ThemeManager.Colors;

            using (var parentBrush = new SolidBrush(GetParentBackColor()))
            {
                g.FillRectangle(parentBrush, ClientRectangle);
            }

            float circleSize = 16f;
            float cy = (Height - circleSize) / 2f;
            var circleRect = new RectangleF(2f, cy, circleSize, circleSize);

            // 원형 배경
            using (var bgBrush = new SolidBrush(colors.InputBackground))
            {
                g.FillEllipse(bgBrush, circleRect);
            }

            // 원형 테두리
            Color circleBorder = Checked ? colors.Accent : (isHovered ? colors.AccentHover : colors.CardBorder);
            using (var pen = new Pen(circleBorder, 1.2f))
            {
                g.DrawEllipse(pen, circleRect);
            }

            // 체크 시 내부 도트
            if (Checked)
            {
                float dotSize = 8f;
                float dxy = (circleSize - dotSize) / 2f;
                var dotRect = new RectangleF(circleRect.X + dxy, circleRect.Y + dxy, dotSize, dotSize);

                using (var dotBrush = new SolidBrush(colors.Accent))
                {
                    g.FillEllipse(dotBrush, dotRect);
                }
            }

            // 텍스트 출력
            Color textColor = Enabled ? (isHovered ? colors.AccentHover : colors.TextPrimary) : colors.TextMuted;
            var textRect = new Rectangle((int)(circleSize + 8), 0, Width - (int)(circleSize + 8), Height);
            TextRenderer.DrawText(g, Text, Font, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
        }
    }

    /// <summary>
    /// 모던 스타일 체크박스 (테마 연동)
    /// </summary>
    public class ModernCheckBox : CheckBox
    {
        private bool isHovered = false;

        public ModernCheckBox()
        {
            SetStyle(ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.ResizeRedraw | 
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 9f);
            ForeColor = ThemeManager.Colors.TextPrimary;

            ThemeManager.ThemeChanged += OnThemeChanged;
        }

        private void OnThemeChanged()
        {
            if (IsDisposed) return;
            ForeColor = ThemeManager.Colors.TextPrimary;
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ThemeManager.ThemeChanged -= OnThemeChanged;
            }
            base.Dispose(disposing);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // 기본 사각 배경 차단
        }

        private Color GetParentBackColor()
        {
            Control c = Parent;
            while (c != null && (c.BackColor == Color.Transparent || c.BackColor.A < 255))
            {
                c = c.Parent;
            }
            return c != null ? c.BackColor : ThemeManager.Colors.Background;
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size textSize = TextRenderer.MeasureText(Text, Font);
            return new Size(textSize.Width + 28, Math.Max(textSize.Height + 4, 24));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var colors = ThemeManager.Colors;

            using (var parentBrush = new SolidBrush(GetParentBackColor()))
            {
                g.FillRectangle(parentBrush, ClientRectangle);
            }

            float boxSize = 16f;
            float by = (Height - boxSize) / 2f;
            var boxRect = new RectangleF(2f, by, boxSize, boxSize);
            float r = 4f;

            using (var path = ModernButton.GetRoundedRectangleF(boxRect, r))
            {
                if (Checked)
                {
                    // 체크 상태: 액센트 배경 + 흰색 체크 마크
                    using (var fillBrush = new SolidBrush(colors.Accent))
                    {
                        g.FillPath(fillBrush, path);
                    }

                    using (var checkPen = new Pen(Color.White, 2.0f))
                    {
                        checkPen.StartCap = LineCap.Round;
                        checkPen.EndCap = LineCap.Round;
                        checkPen.LineJoin = LineJoin.Round;

                        var pt1 = new PointF(boxRect.X + 3.8f, boxRect.Y + 8.2f);
                        var pt2 = new PointF(boxRect.X + 6.8f, boxRect.Y + 11.5f);
                        var pt3 = new PointF(boxRect.X + 12.2f, boxRect.Y + 4.8f);

                        g.DrawLines(checkPen, new PointF[] { pt1, pt2, pt3 });
                    }
                }
                else
                {
                    // 미체크 상태: 입력 필드 배경 + 카드 테두리
                    using (var bgBrush = new SolidBrush(colors.InputBackground))
                    {
                        g.FillPath(bgBrush, path);
                    }

                    Color boxBorder = isHovered ? colors.AccentHover : colors.CardBorder;
                    using (var pen = new Pen(boxBorder, 1.2f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // 텍스트 출력
            Color textColor = Enabled ? (isHovered ? colors.AccentHover : colors.TextPrimary) : colors.TextMuted;
            var textRect = new Rectangle((int)(boxSize + 8), 0, Width - (int)(boxSize + 8), Height);
            TextRenderer.DrawText(g, Text, Font, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
        }
    }
}
