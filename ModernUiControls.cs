using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Web_Page_Screensaver
{
    /// <summary>
    /// 모던 다크 테마 색상 팔레트 정의
    /// </summary>
    public static class DarkColors
    {
        public static readonly Color Background = Color.FromArgb(20, 20, 23);         // 메인 윈도우 배경 (Deep Dark)
        public static readonly Color CardBackground = Color.FromArgb(28, 28, 32);     // 카드/패널 배경 (Zinc Dark)
        public static readonly Color CardBorder = Color.FromArgb(46, 46, 54);          // 은은하고 부드러운 카드 테두리 선
        public static readonly Color InputBackground = Color.FromArgb(15, 15, 17);     // 텍스트박스/리스트 배경
        public static readonly Color InputBorder = Color.FromArgb(52, 52, 60);         // 입력창 테두리

        public static readonly Color TextPrimary = Color.FromArgb(244, 244, 245);      // 메인 텍스트 (화이트)
        public static readonly Color TextSecondary = Color.FromArgb(161, 161, 170);    // 보조 텍스트 (밝은 그레이)
        public static readonly Color TextMuted = Color.FromArgb(113, 113, 122);        // 비활성 텍스트

        public static readonly Color Accent = Color.FromArgb(37, 99, 235);             // Blue-600 액센트
        public static readonly Color AccentHover = Color.FromArgb(59, 130, 246);       // Blue-500
        public static readonly Color AccentPressed = Color.FromArgb(29, 78, 216);     // Blue-700

        public static readonly Color SecondaryBtn = Color.FromArgb(36, 36, 42);       // 보조 버튼 기본
        public static readonly Color SecondaryBtnHover = Color.FromArgb(50, 50, 58);  // 보조 버튼 호버
        public static readonly Color SecondaryBtnPressed = Color.FromArgb(28, 28, 33);// 보조 버튼 클릭

        public static readonly Color Danger = Color.FromArgb(190, 30, 30);            // Red-600 삭제/경고
        public static readonly Color DangerHover = Color.FromArgb(220, 45, 45);        // Red-500
        public static readonly Color DangerPressed = Color.FromArgb(160, 20, 20);      // Red-700
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
            get { return style; }
            set { style = value; Invalidate(); }
        }

        public int BorderRadius
        {
            get { return borderRadius; }
            set { borderRadius = value; Invalidate(); }
        }

        public bool IsSelected
        {
            get { return isSelected; }
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
            // WinForms Button 기본 테마 렌더링(사각형 배경) 완전 차단
        }

        private Color GetSolidParentBackColor()
        {
            Control c = Parent;
            while (c != null && (c.BackColor == Color.Transparent || c.BackColor.A < 255))
            {
                c = c.Parent;
            }
            return c != null ? c.BackColor : DarkColors.Background;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 1. 부모 컨트롤 배경색으로 매끄럽게 채우기 (잔상 방지)
            using (var parentBrush = new SolidBrush(GetSolidParentBackColor()))
            {
                g.FillRectangle(parentBrush, ClientRectangle);
            }

            Color bgColor;
            Color textColor = DarkColors.TextPrimary;
            Color borderColor = Color.Transparent;

            switch (style)
            {
                case ModernButtonStyle.Primary:
                    bgColor = isPressed ? DarkColors.AccentPressed : (isHovered ? DarkColors.AccentHover : DarkColors.Accent);
                    textColor = Color.White;
                    break;

                case ModernButtonStyle.Danger:
                    bgColor = isPressed ? DarkColors.DangerPressed : (isHovered ? DarkColors.DangerHover : DarkColors.Danger);
                    textColor = Color.White;
                    break;

                case ModernButtonStyle.Segment:
                    if (isSelected)
                    {
                        bgColor = DarkColors.Accent;
                        textColor = Color.White;
                    }
                    else
                    {
                        bgColor = isHovered ? DarkColors.SecondaryBtnHover : DarkColors.SecondaryBtn;
                        textColor = isHovered ? DarkColors.TextPrimary : DarkColors.TextSecondary;
                        borderColor = DarkColors.CardBorder;
                    }
                    break;

                case ModernButtonStyle.Ghost:
                    bgColor = isPressed ? DarkColors.SecondaryBtnPressed : (isHovered ? DarkColors.SecondaryBtn : Color.FromArgb(32, 32, 37));
                    textColor = isHovered ? Color.White : DarkColors.TextSecondary;
                    borderColor = DarkColors.CardBorder;
                    break;

                case ModernButtonStyle.Secondary:
                default:
                    bgColor = isPressed ? DarkColors.SecondaryBtnPressed : (isHovered ? DarkColors.SecondaryBtnHover : DarkColors.SecondaryBtn);
                    borderColor = DarkColors.CardBorder;
                    break;
            }

            if (!Enabled)
            {
                bgColor = Color.FromArgb(28, 28, 33);
                textColor = DarkColors.TextMuted;
                borderColor = Color.Transparent;
            }

            // 부동소수점 정밀 좌표 (0.5f 오프셋으로 1px 선명도 및 완벽한 곡선 렌더링)
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

            // 텍스트 출력 (NoPrefix로 & 밑줄 방지)
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
            get { return borderRadius; }
            set { borderRadius = value; Invalidate(); }
        }

        public ModernCard()
        {
            BorderColor = DarkColors.CardBorder;
            SetStyle(ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.ResizeRedraw | 
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = DarkColors.CardBackground;
            Padding = new Padding(12);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // 컨테이너 기본 배경 차단
        }

        private Color GetSolidParentBackColor()
        {
            Control c = Parent;
            while (c != null && (c.BackColor == Color.Transparent || c.BackColor.A < 255))
            {
                c = c.Parent;
            }
            return c != null ? c.BackColor : DarkColors.Background;
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
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            DrawMode = TabDrawMode.OwnerDrawFixed;
            SizeMode = TabSizeMode.Normal;
            ItemSize = new Size(140, 36);
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 전체 배경 지우기
            using (var bgBrush = new SolidBrush(DarkColors.Background))
            {
                g.FillRectangle(bgBrush, ClientRectangle);
            }

            // 탭 헤더 하단 라인
            using (var linePen = new Pen(DarkColors.CardBorder, 1f))
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
                    using (var tabBg = new SolidBrush(DarkColors.CardBackground))
                    {
                        g.FillRectangle(tabBg, tabRect.X, tabRect.Y, tabRect.Width, tabRect.Height + 2);
                    }

                    // 상단/좌우 얇은 테두리
                    using (var borderPen = new Pen(DarkColors.CardBorder, 1f))
                    {
                        g.DrawLine(borderPen, tabRect.X, tabRect.Y, tabRect.X, tabRect.Bottom + 1);
                        g.DrawLine(borderPen, tabRect.Right, tabRect.Y, tabRect.Right, tabRect.Bottom + 1);
                    }

                    // 하단 액센트 바
                    using (var accentBrush = new SolidBrush(DarkColors.Accent))
                    {
                        g.FillRectangle(accentBrush, tabRect.X, tabRect.Bottom, tabRect.Width, 3);
                    }
                }

                Color textColor = isSelected ? DarkColors.TextPrimary : DarkColors.TextSecondary;
                var font = isSelected ? new Font(Font, FontStyle.Bold) : Font;

                TextRenderer.DrawText(g, TabPages[i].Text, font, tabRect, textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
            }
        }
    }
}
