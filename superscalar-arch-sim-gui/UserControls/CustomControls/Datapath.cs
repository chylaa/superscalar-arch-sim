using System;
using System.ComponentModel; 
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.UserControls.CustomControls
{
    public partial class Datapath : UserControl
    {
        private readonly static Brush TextBrush = new SolidBrush(Color.White);
        private readonly static StringFormat TextStringFormat = new StringFormat()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            FormatFlags = StringFormatFlags.NoWrap,
            Trimming = StringTrimming.None
        };
        #region Custom Properties
        /// <summary>Defines apperance of <see cref="Datapath"/> line, <see cref="Single"/> or <see cref="Double"/>.</summary>
        public enum LineApperance { Single, Double };

        /// <summary>Always <see cref="BorderStyle.None"/>.</summary>
        new public BorderStyle BorderStyle => BorderStyle.None;

        /// <summary>Apperance of <see cref="Datapath"/> line.</summary>
        public LineApperance Appearance { get; set; } = LineApperance.Double;

        /// <summary>
        /// Color of outline of datapath line when <see cref="DataValue"/> <b>is not</b>
        /// <see langword="null"/> and <see cref="LineApperance.Double"/> selected. See also <see cref="DoubleInactiveInnerColor"/>
        /// </summary>
        public Color DoubleActiveOutlineColor { get; set; } = Color.Black;
        /// <summary>
        /// Color of inner belt in line if  <see cref="DataValue"/> <b>is</b> <see langword="null"/> 
        /// and <see cref="LineApperance.Double"/> selected. See also <see cref="DoubleActiveOutlineColor"/>.
        /// </summary>
        public Color DoubleInactiveInnerColor { get; set; } = SystemColors.Control;
        /// <summary>Color of datapath line when <see cref="DataValue"/> <b>is not</b> <see langword="null"/>.</summary>
        public Color ColorActive { get; set; } = Color.Red;
        /// <summary>Indicates color of line on when <see cref="DataValue"/> has no value (is <see langword="null"/>).</summary>
        new public Color ForeColor { get; set; } = SystemColors.ControlDarkDark;
        /// <summary>Control's backgroud always transparent.</summary>
        new public Color BackColor => Color.Transparent;

        /// <summary>Width of drawn line.</summary>
        public int WidthOfLine { get; set; } = 4;
        /// <summary>Size of the Ending Cap Arrow.</summary>
        public float EndCapSize { get; set; } = 2.0f;
        /// <summary>Rotates lines by given valid multiple of 90 degrees.</summary>
        public int Rotation { get => _rotation; set { _rotation = FixRotation(value); Invalidate(); } }
        /// <summary>Defines vertical offset between control area and area of line drawing.</summary>
        public int VerticalOffset => (Padding.Vertical + WidthOfLine);

        [Description("Specifies the attachment point of the center point of the displayed text from the \"Text\" property (if \"DataValue\" has value).\r\nValue -1;-1 will display text at bottom center of control, and value -1;1 at top center.")]
        /// <summary>
        /// Specifies the attachment point of the center point of the displayed text from the <see cref="Text"/> property (if <see cref="DataValue"/> has value).
        /// Value <see cref="Point"/> -1;-1 will display text at bottom center of control, and value -1;1 at top center.
        /// </summary>
        public Point TextCPoint { get; set; }
        /// <summary>Starting point's coordinates in 2x2 point grid with middle point being <see cref="Point"/>(0,0).</summary>
        public Point StartPoint { get => _start; set => _start = (Point)ValidateFixPoint(value); } 
        /// <summary>First Middle point's coordinates in 2x2 point grid with middle point being <see cref="Point"/>(0,0).</summary>
        public Point? MiddlePoint_1 { get => _middle1; set => _middle1 = ValidateFixPoint(value); } 
        /// <summary>Second Middle point's coordinates in 2x2 point grid with middle point being <see cref="Point"/>(0,0).</summary>
        public Point? MiddlePoint_2 { get => _middle2; set => _middle2 = ValidateFixPoint(value); } 
        /// <summary>Ending point's coordinates in 2x2 point grid with middle point being <see cref="Point"/>(0,0).</summary>
        public Point EndPoint { get => _end; set => _end = (Point)ValidateFixPoint(value); }
        
        /// <summary>Value integer formatting for <see cref="ValueToolTip"/>.</summary>
        public string DataFormat { get; set; } = "X8";
        /// <summary>Value assinged to control, can be <see langword="null"/>.</summary>
        public int? DataValue { get => _dataValue; set => SetDataValue(value); }


        protected override CreateParams CreateParams {
            // Ugly transparency workaround
            get { CreateParams cp = base.CreateParams; cp.ExStyle |= 0x20; return cp; } 
        }
        #endregion

        private int? _dataValue = null;
        private int _rotation;
        private Point _start;
        private Point? _middle1;
        private Point? _middle2;
        private Point _end;

        public Datapath()
        {
            InitializeComponent();
            Font = new Font("Microsoft YaHei UI", 8.25f, FontStyle.Bold, GraphicsUnit.Point);
            TextCPoint = new Point(Width / 2, Height / 2);
            DataValue = null;
            Text = null;
            Padding = new Padding(all: WidthOfLine);
            // Init straight line
            Rotation = 0;
            StartPoint = new Point(-1,0);
            MiddlePoint_1 = MiddlePoint_2 = null;
            EndPoint = new Point(1, 0);
        }


        protected void SetDataValue(int? value)
        {
            if ((_dataValue = value) == null) Text = null;
            else Text = _dataValue.Value.ToString(format: DataFormat);
            Invalidate();
        }
        private int NormalizeValue(int value)
        {
            if (value >= 1) return 1;
            else if (value <= -1) return -1;
            else return 0;
        }
        private Point? ValidateFixPoint(Point? p) 
        {
            return p.HasValue? new Point(NormalizeValue(p.Value.X), NormalizeValue(p.Value.Y)) : p;
        }
        private int FixRotation(int value)
        {
            int remainder = value % 90;
            int roundedValue = value;
            if (remainder < 45)
                roundedValue -= remainder;
            else
                roundedValue += (90 - remainder);
            return roundedValue;
        }
        protected override void OnPaintBackground(PaintEventArgs _)
        {
            base.OnPaintBackground(_);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawLine(e.Graphics);
            DrawText(e.Graphics);
        }
        private Point TranslateToPoint(Point p)
        {
            double angleRadians = Rotation * Math.PI / 180.0;
            double cosAngle = Math.Cos(angleRadians);
            double sinAngle = Math.Sin(angleRadians);

            double xRotated = p.X * cosAngle - p.Y * sinAngle;
            double yRotated = p.X * sinAngle + p.Y * cosAngle;

            int x = ((int)(Width / 2 * (1 + xRotated)));
            int y = ((int)(Height / 2 * (1 + (-yRotated))));

            x = Math.Max(Padding.Horizontal, Math.Min(Width - Padding.Horizontal, x));
            y = Math.Max(Padding.Vertical, Math.Min(Height - Padding.Vertical, y));

            return new Point(x, y);
        }

        private void DrawLine(Graphics g)
        {
            using (GraphicsPath path = new GraphicsPath() { FillMode = FillMode.Winding})
            {                
                path.StartFigure();
                path.AddLine(TranslateToPoint(StartPoint), TranslateToPoint(MiddlePoint_1 ?? StartPoint));
                if (MiddlePoint_2.HasValue)
                    path.AddLine(TranslateToPoint(MiddlePoint_1.Value), TranslateToPoint(MiddlePoint_2.Value));
                path.AddLine(TranslateToPoint(MiddlePoint_2 ?? EndPoint), TranslateToPoint(EndPoint));

                Color outer = (DataValue is null) ?
                            ForeColor
                            : (Appearance == LineApperance.Double) ?
                            DoubleActiveOutlineColor
                            : ColorActive;

                float size = (DataValue is null) ? 0 : EndCapSize;
                AdjustableArrowCap endcap = new AdjustableArrowCap(size / 1.5f, size, isFilled: false) { MiddleInset = 0 };
                LineCap startcap = LineCap.RoundAnchor;

                using (Pen pen = new Pen(outer, WidthOfLine) { EndCap = LineCap.Custom, CustomEndCap = endcap, StartCap = startcap })
                    g.DrawPath(pen, path); // draw always

                if (Appearance == LineApperance.Double) 
                {
                    Color inner = DataValue.HasValue ? ColorActive : DoubleInactiveInnerColor;
                    using (Pen pen = new Pen(inner, (WidthOfLine / 2f)) { Alignment = PenAlignment.Center, EndCap = LineCap.Custom, CustomEndCap = endcap, StartCap = startcap})
                        g.DrawPath(pen, path);
                }
            }
        }
        private void DrawTextOutline(Graphics g, Color outline, PointF location)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddString(Text, Font.FontFamily, (int)Font.Style,
                    (g.DpiY * Font.Size / 72), location, TextStringFormat);

                using (Pen pen = new Pen(outline, 2.5f) )
                    g.DrawPath(pen, path);
            }
        }

        private void DrawText(Graphics g)
        {
            if (false == string.IsNullOrEmpty(Text))
            {   
                SizeF textSize = g.MeasureString(Text, Font);
                PointF textStartPoint;

                if (TextCPoint.X == -1 && TextCPoint.Y == -1)
                {
                    textStartPoint = new PointF((Width/2), Height - (textSize.Height / 3));
                } 
                else if (TextCPoint.X == -1 && TextCPoint.Y == 1)
                {
                    textStartPoint = new PointF((Width / 2), 1 + (textSize.Height / 3) );
                } 
                else
                {
                    textStartPoint = new PointF(TextCPoint.X, TextCPoint.Y - (textSize.Height / 2));
                }

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                DrawTextOutline(g, Color.Black, textStartPoint);
                g.DrawString(Text, Font, TextBrush, textStartPoint, TextStringFormat);
            }
        }

    }
}
