using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using ThoughtWorks.QRCode.Codec;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        // 绘画功能相关变量
        private PaperSize psz;
        private bool isDrawing = false;
        private PointF startPointMm;  // 使用毫米坐标
        private List<RectangleInfo> rectanglesArray = new List<RectangleInfo>();
        private RectangleF tempRectangleMm;  // 使用毫米坐标
        private float currentLineWidth = 2.0f;  // 当前线宽，通过numericUpDown1控制
        private PointF? snapTargetMm = null;  // 使用毫米坐标

        // 配置参数
        private const float START_POINT_SNAP_THRESHOLD_MM = 1f;  // 毫米单位
        private const float EDGE_SNAP_THRESHOLD_MM = 0.5f;       // 毫米单位
        private const float ROW_COL_ALIGN_THRESHOLD_MM = 1f;     // 毫米单位
        private const float SIZE_MATCH_THRESHOLD_MM = 1f;        // 毫米单位

        // 文字大小配置（不随缩放变化）
        private const float DIMENSION_TEXT_SIZE = 10f;

        // 缩放相关变量
        private float zoomFactor = 1.0f;
        private const float ZoomStep = 0.1f;
        private const float MinZoom = 0.5f;
        private const float MaxZoom = 3.0f;

        // 设备PPI（每英寸像素数）
        private float PPI = 96f;

        // 画布尺寸（毫米）
        private float canvasWidthMm = 0;
        private float canvasHeightMm = 0;

        // 文字格式设置
        private readonly StringFormat centerFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisWord
        };

        private readonly StringFormat nearFormat = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisWord
        };

        private readonly StringFormat farFormat = new StringFormat
        {
            Alignment = StringAlignment.Far,
            LineAlignment = StringAlignment.Near,
            Trimming = StringTrimming.EllipsisWord
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 初始化线宽控制器
            InitializeLineWidthControl();

            // 初始化DataGridView数据
            InitializeDataGridView();
            CenterDrawingPanel();

            // 注册鼠标事件
            drawingPanel.MouseWheel += DrawingPanel_MouseWheel;
            drawingPanel.MouseEnter += DrawingPanel_MouseEnter;
            drawingPanel.MouseDown += DrawingPanel_MouseDown;
            drawingPanel.MouseMove += DrawingPanel_MouseMove;
            drawingPanel.MouseUp += DrawingPanel_MouseUp;
            drawingPanel.Paint += DrawingPanel_Paint;

            // 双重缓冲设置
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, drawingPanel, new object[] { true });

            // 设置窗体接收键盘事件
            KeyPreview = true;

            // 初始化打印机列表
            foreach (var printer in PrinterSettings.InstalledPrinters)
            {
                comboBox1.Items.Add(printer);
            }
            comboBox1.SelectedItem = printDocument_test.PrinterSettings.PrinterName;

            this.printDocument_test.PrintPage += new PrintPageEventHandler(this.printDocument_test_PrintPage);
        }

        // 初始化线宽控制器
        private void InitializeLineWidthControl()
        {
            numericUpDown1.Minimum = 0.5m;
            numericUpDown1.Maximum = 10.0m;
            numericUpDown1.Increment = 0.5m;
            numericUpDown1.DecimalPlaces = 1;
            numericUpDown1.Value = (decimal)currentLineWidth;
            numericUpDown1.ValueChanged += NumericUpDown1_ValueChanged;
        }

        // 线宽控制器值变化事件
        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            currentLineWidth = (float)numericUpDown1.Value;
            drawingPanel.Invalidate();
        }

        #region 单位转换方法
        private float PixelsToMillimeters(float pixels) => (pixels * 25.4f) / (PPI * zoomFactor);

        private float MillimetersToPixels(float millimeters) => (millimeters * PPI * zoomFactor) / 25.4f;

        private PointF ScreenToMillimeters(Point point) => new PointF(
            PixelsToMillimeters(point.X),
            PixelsToMillimeters(point.Y)
        );

        private Point MillimetersToScreen(PointF pointMm) => new Point(
            (int)Math.Round(MillimetersToPixels(pointMm.X)),
            (int)Math.Round(MillimetersToPixels(pointMm.Y))
        );

        private RectangleF MillimetersToScreen(RectangleF rectMm) => new RectangleF(
            MillimetersToPixels(rectMm.X),
            MillimetersToPixels(rectMm.Y),
            MillimetersToPixels(rectMm.Width),
            MillimetersToPixels(rectMm.Height)
        );
        #endregion

        #region 键盘快捷键处理
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.Z:
                    WithDrawBtn_Click(null, null);
                    return true;
                case Keys.Control | Keys.Add:
                case Keys.Control | Keys.Oemplus:
                    ZoomIn();
                    return true;
                case Keys.Control | Keys.Subtract:
                case Keys.Control | Keys.OemMinus:
                    ZoomOut();
                    return true;
                case Keys.Control | Keys.D0:
                    ResetZoom();
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region 绘画功能核心方法
        private void DrawingPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && canvasWidthMm > 0 && canvasHeightMm > 0)
            {
                isDrawing = true;
                var mousePosMm = ScreenToMillimeters(e.Location);

                // 限制在画布范围内
                mousePosMm.X = Math.Max(0, Math.Min(canvasWidthMm, mousePosMm.X));
                mousePosMm.Y = Math.Max(0, Math.Min(canvasHeightMm, mousePosMm.Y));

                startPointMm = snapTargetMm ?? mousePosMm;
                tempRectangleMm = new RectangleF(startPointMm, SizeF.Empty);
                snapTargetMm = null;
            }
        }

        private void DrawingPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (canvasWidthMm <= 0 || canvasHeightMm <= 0) return;

            var mousePosMm = ScreenToMillimeters(e.Location);

            if (isDrawing)
            {
                // 限制在画布范围内
                mousePosMm.X = Math.Max(0, Math.Min(canvasWidthMm, mousePosMm.X));
                mousePosMm.Y = Math.Max(0, Math.Min(canvasHeightMm, mousePosMm.Y));

                float x = Math.Min(startPointMm.X, mousePosMm.X);
                float y = Math.Min(startPointMm.Y, mousePosMm.Y);
                float width = Math.Abs(startPointMm.X - mousePosMm.X);
                float height = Math.Abs(startPointMm.Y - mousePosMm.Y);

                tempRectangleMm = new RectangleF(x, y, width, height);
                drawingPanel.Invalidate();
            }
            else
            {
                snapTargetMm = FindPotentialSnapPoint(mousePosMm);
                drawingPanel.Invalidate();
            }
        }

        private void DrawingPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing && e.Button == MouseButtons.Left)
            {
                isDrawing = false;

                if (tempRectangleMm.Width > 1 && tempRectangleMm.Height > 1)
                {
                    float originalWidth = tempRectangleMm.Width;
                    float originalHeight = tempRectangleMm.Height;

                    var newRect = new RectangleInfo(
                        tempRectangleMm,
                        currentLineWidth,
                        rectanglesArray.Count + 1
                    );
                    rectanglesArray.Add(newRect);

                    if (!snapTargetMm.HasValue)
                    {
                        SnapToEdges(newRect);
                    }

                    MatchRowColumnSize(newRect, originalWidth, originalHeight);

                    drawingPanel.Invalidate();
                    UpdateDataGridView();
                }

                tempRectangleMm = RectangleF.Empty;
                snapTargetMm = null;
            }
        }

        private void DrawingPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 绘制所有已完成的矩形
            foreach (var rectInfo in rectanglesArray)
            {
                if (rectInfo.Visiable)
                {
                    using (Pen pen = new Pen(Color.Black, rectInfo.LineWidth * zoomFactor))
                    {
                        RectangleF rectScreen = MillimetersToScreen(rectInfo.RectangleMm);
                        e.Graphics.DrawRectangle(pen, Rectangle.Round(rectScreen));
                    }
                }
                DrawRectangleContent(e.Graphics, rectInfo, e);
            }

            // 绘制临时矩形
            if (tempRectangleMm != RectangleF.Empty)
            {
                using (Pen pen = new Pen(Color.Black, currentLineWidth * zoomFactor))
                {
                    RectangleF rectScreen = MillimetersToScreen(tempRectangleMm);
                    e.Graphics.DrawRectangle(pen, Rectangle.Round(rectScreen));
                }
                DrawTempDimensions(e.Graphics, tempRectangleMm);
            }

            // 绘制吸附范围提示
            if (snapTargetMm.HasValue && !isDrawing)
            {
                Point snapScreen = MillimetersToScreen(snapTargetMm.Value);
                using (Pen pen = new Pen(Color.Blue, 2) { DashStyle = DashStyle.Dash })
                {
                    float snapRadius = MillimetersToPixels(START_POINT_SNAP_THRESHOLD_MM);
                    e.Graphics.DrawEllipse(pen,
                        snapScreen.X - snapRadius,
                        snapScreen.Y - snapRadius,
                        snapRadius * 2,
                        snapRadius * 2);

                    using (SolidBrush brush = new SolidBrush(Color.Red))
                    {
                        e.Graphics.FillEllipse(brush, snapScreen.X - 3, snapScreen.Y - 3, 6, 6);
                    }
                }
            }
        }
        #endregion

        #region 绘画辅助方法
        private PointF? FindPotentialSnapPoint(PointF mouseLocationMm)
        {
            if (rectanglesArray.Count == 0)
                return null;

            var candidatePoints = new List<PointF>();
            foreach (var rect in rectanglesArray)
            {
                foreach (var corner in rect.CornersMm)
                {
                    if (CalculateDistance(mouseLocationMm, corner) <= START_POINT_SNAP_THRESHOLD_MM)
                    {
                        candidatePoints.Add(corner);
                    }
                }
            }

            return candidatePoints.OrderBy(p => CalculateDistance(mouseLocationMm, p)).FirstOrDefault();
        }

        private void SnapToEdges(RectangleInfo newRect)
        {
            foreach (var rect in rectanglesArray.Where(r => r != newRect))
            {
                // 左边缘吸附到其他矩形右边缘
                if (Math.Abs(newRect.RectangleMm.Left - rect.RectangleMm.Right) <= EDGE_SNAP_THRESHOLD_MM)
                {
                    newRect.UpdateRectangle(new RectangleF(
                        rect.RectangleMm.Right,
                        newRect.RectangleMm.Y,
                        newRect.RectangleMm.Width,
                        newRect.RectangleMm.Height));
                    return;
                }

                // 上边缘吸附到其他矩形下边缘
                if (Math.Abs(newRect.RectangleMm.Top - rect.RectangleMm.Bottom) <= EDGE_SNAP_THRESHOLD_MM)
                {
                    newRect.UpdateRectangle(new RectangleF(
                        newRect.RectangleMm.X,
                        rect.RectangleMm.Bottom,
                        newRect.RectangleMm.Width,
                        newRect.RectangleMm.Height));
                    return;
                }
            }
        }

        private void MatchRowColumnSize(RectangleInfo newRect, float originalWidth, float originalHeight)
        {
            var rowGroup = FindRowGroup(newRect);
            var columnGroup = FindColumnGroup(newRect);

            // 匹配行高
            if (rowGroup.Count >= 1)
            {
                float targetHeight = GetMostCommonHeight(rowGroup);
                if (Math.Abs(originalHeight - targetHeight) <= SIZE_MATCH_THRESHOLD_MM)
                {
                    newRect.UpdateRectangle(new RectangleF(
                        newRect.RectangleMm.X,
                        newRect.RectangleMm.Y,
                        newRect.RectangleMm.Width,
                        targetHeight
                    ));
                }
            }

            // 匹配列宽
            if (columnGroup.Count >= 1)
            {
                float targetWidth = GetMostCommonWidth(columnGroup);
                if (Math.Abs(originalWidth - targetWidth) <= SIZE_MATCH_THRESHOLD_MM)
                {
                    newRect.UpdateRectangle(new RectangleF(
                        newRect.RectangleMm.X,
                        newRect.RectangleMm.Y,
                        targetWidth,
                        newRect.RectangleMm.Height
                    ));
                }
            }
        }

        private List<RectangleInfo> FindRowGroup(RectangleInfo newRect) => rectanglesArray
            .Where(r => r != newRect)
            .Where(r => Math.Abs(r.RectangleMm.Top - newRect.RectangleMm.Top) <= ROW_COL_ALIGN_THRESHOLD_MM)
            .ToList();

        private List<RectangleInfo> FindColumnGroup(RectangleInfo newRect) => rectanglesArray
            .Where(r => r != newRect)
            .Where(r => Math.Abs(r.RectangleMm.Left - newRect.RectangleMm.Left) <= ROW_COL_ALIGN_THRESHOLD_MM)
            .ToList();

        private float GetMostCommonHeight(List<RectangleInfo> group)
        {
            var heightCounts = group.GroupBy(r => Math.Round(r.RectangleMm.Height, 1))
                                    .OrderByDescending(g => g.Count())
                                    .FirstOrDefault();
            return (float)(heightCounts?.Key ?? group[0].RectangleMm.Height);
        }

        private float GetMostCommonWidth(List<RectangleInfo> group)
        {
            var widthCounts = group.GroupBy(r => Math.Round(r.RectangleMm.Width, 1))
                                   .OrderByDescending(g => g.Count())
                                   .FirstOrDefault();
            return (float)(widthCounts?.Key ?? group[0].RectangleMm.Width);
        }

        private double CalculateDistance(PointF p1, PointF p2)
        {
            float dx = p1.X - p2.X;
            float dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private void DrawRectangleContent(Graphics graphics, RectangleInfo rectInfo, PaintEventArgs e)
        {
            RectangleF rectScreen = MillimetersToScreen(rectInfo.RectangleMm);

            // 绘制序号（右上角）
            string numberText = $"{rectInfo.Number}";
            FontStyle IsBold = rectInfo.IsBold ? FontStyle.Bold : FontStyle.Regular;
            using (Font numberFont = new Font(rectInfo.FontFamily, 12f, FontStyle.Bold))
            using (SolidBrush numberBrush = new SolidBrush(Color.Red))
            {
                float numberAreaWidth = rectScreen.Width * 0.2f;
                float numberAreaHeight = rectScreen.Height * 0.2f;
                RectangleF numberRect = new RectangleF(
                    rectScreen.Right - numberAreaWidth,
                    rectScreen.Top,
                    numberAreaWidth,
                    numberAreaHeight
                );
                graphics.DrawString(numberText, numberFont, numberBrush, numberRect, farFormat);
            }

            // 绘制内容
            if (!string.IsNullOrEmpty(rectInfo.Content))
            {
                if (rectInfo.ContentType == ("文字"))
                {
                    using (Font contentFont = new Font(rectInfo.FontFamily, rectInfo.FontSize, IsBold))
                    using (SolidBrush contentBrush = new SolidBrush(Color.Black))
                    {
                        StringFormat contentFormat = rectInfo.ContentAlignment == ContentAlignment.Center ? centerFormat : nearFormat;
                        graphics.DrawString(rectInfo.Content, contentFont, contentBrush, rectScreen, contentFormat);
                    }
                }
                else if (rectInfo.ContentType == ("二维码"))
                {
                    Bitmap QrCode = Create_QrCode(rectInfo.Content, 4);
                    DrawQrCode(e.Graphics, QrCode, rectScreen);
                }
                else if (rectInfo.ContentType == ("条形码"))
                {
                    Bitmap barcode = Generate(rectInfo.Content, (int)rectScreen.Width, (int)rectScreen.Height);
                    DrawBarCode(e.Graphics, barcode, rectScreen);
                }
            }
        }

        private void DrawTempDimensions(Graphics g, RectangleF rectMm)
        {
            string dimensionText = $"{rectMm.Width:F2}mm × {rectMm.Height:F2}mm";

            using (Font font = new Font("Arial", DIMENSION_TEXT_SIZE, FontStyle.Regular))
            using (SolidBrush textBrush = new SolidBrush(Color.Blue))
            using (SolidBrush backgroundBrush = new SolidBrush(Color.FromArgb(180, 255, 255, 255)))
            {
                RectangleF rectScreen = MillimetersToScreen(rectMm);
                SizeF textSize = g.MeasureString(dimensionText, font);
                float x = rectScreen.Right - textSize.Width - 5;
                float y = rectScreen.Top + 5;

                if (x < rectScreen.Left) x = rectScreen.Left + 5;
                if (y + textSize.Height > rectScreen.Bottom) y = rectScreen.Bottom - textSize.Height - 5;

                g.FillRectangle(backgroundBrush, x - 2, y - 2, textSize.Width + 4, textSize.Height + 4);
                g.DrawString(dimensionText, font, textBrush, x, y);
            }
        }
        #endregion

        #region DataGridView相关
        private void InitializeDataGridView()
        {
            dataGridView1.Columns.Clear();

            // 添加基础列
            dataGridView1.Columns.Add("Number", "序号");
            dataGridView1.Columns.Add("X", "X坐标(mm)");
            dataGridView1.Columns.Add("Y", "Y坐标(mm)");
            dataGridView1.Columns.Add("Width", "宽度(mm)");
            dataGridView1.Columns.Add("Height", "高度(mm)");
            dataGridView1.Columns.Add("LineWidth", "线宽");
            // 添加边框可见性列
            DataGridViewCheckBoxColumn visiableColumn = new DataGridViewCheckBoxColumn
            {
                Name = "Visiable",
                HeaderText = "边框可见性",
                FalseValue = false,
                TrueValue = true,
                DefaultCellStyle = { NullValue = true }
            };
            dataGridView1.Columns.Add(visiableColumn);

            dataGridView1.Columns.Add("Content", "内容");

            // 添加内容类型下拉列
            DataGridViewComboBoxColumn TypeColumn = new DataGridViewComboBoxColumn
            {
                Name = "ContentType",
                HeaderText = "内容类型",
                Items = { "文字", "二维码", "条形码"},
                DefaultCellStyle = { NullValue = "文字" }
            };
            dataGridView1.Columns.Add(TypeColumn);

            // 添加字体类型下拉列
            DataGridViewComboBoxColumn fontColumn = new DataGridViewComboBoxColumn
            {
                Name = "FontFamily",
                HeaderText = "字体类型",
                Items = { "Arial", "SimSun", "Microsoft YaHei", "SimHei", "Times New Roman", "Courier New" },
                DefaultCellStyle = { NullValue = "Arial" }
            };
            dataGridView1.Columns.Add(fontColumn);

            // 添加字体大小列
            DataGridViewTextBoxColumn fontSizeColumn = new DataGridViewTextBoxColumn
            {
                Name = "FontSize",
                HeaderText = "字体大小",
                DefaultCellStyle = { Format = "N1" }
            };
            dataGridView1.Columns.Add(fontSizeColumn);

            // 添加是否加粗列
            DataGridViewCheckBoxColumn boldColumn = new DataGridViewCheckBoxColumn
            {
                Name = "IsBold",
                HeaderText = "是否加粗",
                FalseValue = false,
                TrueValue = true,
                DefaultCellStyle = { NullValue = false }
            };
            dataGridView1.Columns.Add(boldColumn);

            // 添加内容对齐方式列
            DataGridViewComboBoxColumn alignmentColumn = new DataGridViewComboBoxColumn
            {
                Name = "ContentAlignment",
                HeaderText = "内容对齐",
                Items = { "居左", "居中" },
                DefaultCellStyle = { NullValue = "居左" }
            };
            dataGridView1.Columns.Add(alignmentColumn);

            // 设置列宽和只读属性
            dataGridView1.Columns["Number"].Width = 50;
            dataGridView1.Columns["Number"].ReadOnly = true;
            dataGridView1.Columns["X"].Width = 80;
            dataGridView1.Columns["Y"].Width = 80;
            dataGridView1.Columns["Width"].Width = 80;
            dataGridView1.Columns["Height"].Width = 80;
            dataGridView1.Columns["LineWidth"].Width = 60;
            dataGridView1.Columns["Visiable"].Width = 70;
            dataGridView1.Columns["Content"].Width = 100;
            dataGridView1.Columns["ContentType"].Width = 100;
            dataGridView1.Columns["FontFamily"].Width = 100;
            dataGridView1.Columns["FontSize"].Width = 70;
            dataGridView1.Columns["IsBold"].Width = 70;
            dataGridView1.Columns["ContentAlignment"].Width = 70;

            // 设置标题头居中
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // 设置单元格对齐方式
            dataGridView1.Columns["Number"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["X"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["Y"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["Width"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["Height"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["LineWidth"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["Content"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns["ContentType"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns["FontFamily"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns["FontSize"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["IsBold"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Visiable"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["ContentAlignment"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
        }

        private void UpdateDataGridView()
        {
            dataGridView1.Rows.Clear();

            foreach (var rectInfo in rectanglesArray)
            {
                string alignmentText = rectInfo.ContentAlignment == ContentAlignment.Left ? "居左" : "居中";

                int rowIndex = dataGridView1.Rows.Add(
                    rectInfo.Number,
                    rectInfo.RectangleMm.X,
                    rectInfo.RectangleMm.Y,
                    rectInfo.RectangleMm.Width,
                    rectInfo.RectangleMm.Height,
                    rectInfo.LineWidth,
                    rectInfo.Visiable,
                    rectInfo.Content,
                    rectInfo.ContentType,
                    rectInfo.FontFamily,
                    rectInfo.FontSize,
                    rectInfo.IsBold,
                    alignmentText
                );

                FormatRowCells(rowIndex);
            }
        }

        private void FormatRowCells(int rowIndex)
        {
            dataGridView1.Rows[rowIndex].Cells["X"].Style.Format = "F2";
            dataGridView1.Rows[rowIndex].Cells["Y"].Style.Format = "F2";
            dataGridView1.Rows[rowIndex].Cells["Width"].Style.Format = "F2";
            dataGridView1.Rows[rowIndex].Cells["Height"].Style.Format = "F2";
            dataGridView1.Rows[rowIndex].Cells["LineWidth"].Style.Format = "F1";
            dataGridView1.Rows[rowIndex].Cells["FontSize"].Style.Format = "F1";
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < rectanglesArray.Count)
            {
                var rectInfo = rectanglesArray[e.RowIndex];
                bool dataChanged = false;

                try
                {
                    switch (dataGridView1.Columns[e.ColumnIndex].Name)
                    {
                        case "X":
                            if (float.TryParse(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString(), out float x))
                            {
                                x = Math.Max(0, Math.Min(canvasWidthMm - rectInfo.RectangleMm.Width, x));
                                rectInfo.UpdateRectangle(new RectangleF(x, rectInfo.RectangleMm.Y, rectInfo.RectangleMm.Width, rectInfo.RectangleMm.Height));
                                dataChanged = true;
                            }
                            break;

                        case "Y":
                            if (float.TryParse(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString(), out float y))
                            {
                                y = Math.Max(0, Math.Min(canvasHeightMm - rectInfo.RectangleMm.Height, y));
                                rectInfo.UpdateRectangle(new RectangleF(rectInfo.RectangleMm.X, y, rectInfo.RectangleMm.Width, rectInfo.RectangleMm.Height));
                                dataChanged = true;
                            }
                            break;

                        case "Width":
                            if (float.TryParse(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString(), out float width) && width > 0)
                            {
                                width = Math.Min(width, canvasWidthMm - rectInfo.RectangleMm.X);
                                rectInfo.UpdateRectangle(new RectangleF(rectInfo.RectangleMm.X, rectInfo.RectangleMm.Y, width, rectInfo.RectangleMm.Height));
                                dataChanged = true;
                            }
                            break;

                        case "Height":
                            if (float.TryParse(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString(), out float height) && height > 0)
                            {
                                height = Math.Min(height, canvasHeightMm - rectInfo.RectangleMm.Y);
                                rectInfo.UpdateRectangle(new RectangleF(rectInfo.RectangleMm.X, rectInfo.RectangleMm.Y, rectInfo.RectangleMm.Width, height));
                                dataChanged = true;
                            }
                            break;

                        case "LineWidth":
                            if (float.TryParse(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString(), out float lineWidth) && lineWidth > 0)
                            {
                                rectInfo.SetLineWidth(lineWidth);
                                numericUpDown1.Value = (decimal)lineWidth;
                                currentLineWidth = lineWidth;
                                dataChanged = true;
                            }
                            break;

                        case "Visiable":
                            rectInfo.Visiable = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null && (bool)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                            dataChanged = true;
                            break;

                        case "Content":
                            rectInfo.Content = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";
                            dataChanged = true;
                            break;

                        case "ContentType":
                            rectInfo.ContentType = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "文字";
                            dataChanged = true;
                            break;


                        case "FontFamily":
                            rectInfo.FontFamily = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "Arial";
                            dataChanged = true;
                            break;

                        case "FontSize":
                            if (float.TryParse(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString(), out float fontSize) && fontSize > 0)
                            {
                                rectInfo.FontSize = fontSize;
                                dataChanged = true;
                            }
                            break;

                        case "IsBold":
                            rectInfo.IsBold = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null && (bool)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                            dataChanged = true;
                            break;

                        case "ContentAlignment":
                            string alignmentText = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "居左";
                            rectInfo.ContentAlignment = alignmentText == "居中" ? ContentAlignment.Center : ContentAlignment.Left;
                            dataChanged = true;
                            break;
                    }

                    if (dataChanged)
                    {
                        // 更新单元格显示值
                        dataGridView1.Rows[e.RowIndex].Cells["X"].Value = rectInfo.RectangleMm.X;
                        dataGridView1.Rows[e.RowIndex].Cells["Y"].Value = rectInfo.RectangleMm.Y;
                        dataGridView1.Rows[e.RowIndex].Cells["Width"].Value = rectInfo.RectangleMm.Width;
                        dataGridView1.Rows[e.RowIndex].Cells["Height"].Value = rectInfo.RectangleMm.Height;
                        dataGridView1.Rows[e.RowIndex].Cells["LineWidth"].Value = rectInfo.LineWidth;
                        dataGridView1.Rows[e.RowIndex].Cells["FontFamily"].Value = rectInfo.FontFamily;
                        dataGridView1.Rows[e.RowIndex].Cells["ContentType"].Value = rectInfo.ContentType;
                        dataGridView1.Rows[e.RowIndex].Cells["FontSize"].Value = rectInfo.FontSize;
                        dataGridView1.Rows[e.RowIndex].Cells["IsBold"].Value = rectInfo.IsBold;
                        dataGridView1.Rows[e.RowIndex].Cells["Visiable"].Value = rectInfo.Visiable;
                        dataGridView1.Rows[e.RowIndex].Cells["ContentAlignment"].Value = rectInfo.ContentAlignment == ContentAlignment.Center ? "居中" : "居左";

                        drawingPanel.Invalidate();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"数据输入错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UpdateDataGridView();
                }
            }
        }
        #endregion

        #region 缩放功能
        private void DrawingPanel_MouseEnter(object sender, EventArgs e) => drawingPanel.Focus();

        private void DrawingPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control && canvasWidthMm > 0 && canvasHeightMm > 0)
            {
                if (e.Delta > 0) ZoomIn();
                else ZoomOut();
                ((HandledMouseEventArgs)e).Handled = true;
            }
        }

        private void ZoomIn()
        {
            if (zoomFactor < MaxZoom)
            {
                zoomFactor += ZoomStep;
                ApplyZoom();
            }
        }

        private void ZoomOut()
        {
            if (zoomFactor > MinZoom)
            {
                zoomFactor -= ZoomStep;
                ApplyZoom();
            }
        }

        private void ResetZoom()
        {
            zoomFactor = 1.0f;
            ApplyZoom();
        }

        private void ApplyZoom()
        {
            if (canvasWidthMm > 0 && canvasHeightMm > 0)
            {
                int pixelWidth = (int)Math.Round(MillimetersToPixels(canvasWidthMm));
                int pixelHeight = (int)Math.Round(MillimetersToPixels(canvasHeightMm));
                drawingPanel.Size = new Size(pixelWidth, pixelHeight);
                CenterDrawingPanel();
                UpdateZoomStatus();
                drawingPanel.Invalidate();
            }
        }

        private void UpdateZoomStatus() => Text = $"绘画面板应用程序 - 缩放: {(zoomFactor * 100):F0}%";
        #endregion

        #region 布局和事件处理
        private void SetDrawingPanelSize(float widthMm, float heightMm)
        {
            canvasWidthMm = widthMm;
            canvasHeightMm = heightMm;
            ApplyZoom();
        }

        private void CenterDrawingPanel()
        {
            drawingPanel.Location = new Point(
                (splitContainer1.Panel1.Width - drawingPanel.Width) / 2,
                (splitContainer1.Panel1.Height - drawingPanel.Height) / 2
            );
        }

        private void SetSizeButton_Click(object sender, EventArgs e)
        {
            if (float.TryParse(widthTextBox.Text, out float widthMm) &&
                float.TryParse(heightTextBox.Text, out float heightMm) &&
                widthMm > 0 && heightMm > 0)
            {
                SetDrawingPanelSize(widthMm, heightMm);
                ResetZoom();
                setting_psz((int)widthMm, (int)heightMm);
            }
            else
            {
                MessageBox.Show("请输入有效的宽度和高度（必须大于0）");
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e) => CenterDrawingPanel();

        private void WithDrawBtn_Click(object sender, EventArgs e)
        {
            if (rectanglesArray.Count > 0)
            {
                rectanglesArray.RemoveAt(rectanglesArray.Count - 1);
                for (int i = 0; i < rectanglesArray.Count; i++)
                {
                    rectanglesArray[i].Number = i + 1;
                }
                drawingPanel.Invalidate();
                UpdateDataGridView();
            }
        }

        private void ExportBtn_Click(object sender, EventArgs e)
        {
            Console.WriteLine("===== 表格线框数据 =====");
            Console.WriteLine($"总数量: {rectanglesArray.Count}");
            foreach (var rect in rectanglesArray)
            {
                Console.WriteLine($" RectangleF grid{rect.Number} = new RectangleF({rect.RectangleMm.X:F2}f, {rect.RectangleMm.Y:F2}f, {rect.RectangleMm.Width:F2}f, {rect.RectangleMm.Height:F2}f);");
            }
            MessageBox.Show("数据已导出到控制台", "导出完成");
        }
        #endregion

        #region 打印功能
        private void setting_psz(int w, int h)
        {
            psz = new PaperSize();
            psz.Width = (int)(w / 25.4 * 100);
            psz.Height = (int)(h / 25.4 * 100);
            printDocument_test.DefaultPageSettings.PaperSize = psz;
        }

        private void printDocument_test_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.PageUnit = GraphicsUnit.Millimeter;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            foreach (var rect in rectanglesArray)
            {
                using (Pen borderPen = new Pen(Color.Black, rect.LineWidth / 5))
                using (GraphicsPath path = new GraphicsPath())
                {
                    if (rect.Visiable)
                    {
                        path.AddRectangle(rect.RectangleMm);
                    }
                    e.Graphics.DrawPath(borderPen, path);
                }

                if (!string.IsNullOrEmpty(rect.Content))
                {
                    if (rect.ContentType == ("文字"))
                    {
                        using (Font font = new Font(rect.FontFamily, rect.FontSize, rect.IsBold ? FontStyle.Bold : FontStyle.Regular))
                        {
                            StringFormat contentFormat = rect.ContentAlignment == ContentAlignment.Center ? centerFormat : nearFormat;
                            e.Graphics.DrawString(rect.Content, font, Brushes.Black, rect.RectangleMm, contentFormat);
                        }
                    }
                    else if (rect.ContentType == ("二维码"))
                    {
                        Bitmap QrCode = Create_QrCode(rect.Content, 4);
                        DrawQrCode(e.Graphics, QrCode, rect.RectangleMm);
                    }
                    else if (rect.ContentType == ("条形码"))
                    {
                        Bitmap barcode = Generate(rect.Content, (int)rect.RectangleMm.Width, (int)rect.RectangleMm.Height);
                        DrawBarCode(e.Graphics, barcode, rect.RectangleMm);
                    }
                }

                
            }
        }

        private void PrintBtn_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                printDocument_test.PrinterSettings.PrinterName = comboBox1.SelectedItem.ToString();
            }

            printDocument_test.DefaultPageSettings.PaperSize = psz;
            printDocument_test.Print();
        }
        #endregion

        public static Bitmap Generate(string text, int width, int height)
        {
            BarcodeWriter writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.CODE_128;
            EncodingOptions options = new EncodingOptions()
            {
                Width = width,
                Height = height,
                Margin = 3,
                PureBarcode = true
            };
            writer.Options = options;
            Bitmap map = writer.Write(text);
            return map;
        }
        public Bitmap Create_QrCode(string codeNumber, int size)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = size;
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            System.Drawing.Bitmap image = qrCodeEncoder.Encode(codeNumber);
            return image;
        }
        private void DrawQrCode(Graphics g, Image image, RectangleF destRect, float padding = 0.5f)
        {
            float maxPadding = Math.Min(destRect.Width, destRect.Height) / 2;
            padding = Math.Min(padding, maxPadding);
            RectangleF paddedRect = new RectangleF(
                destRect.X + padding,
                destRect.Y + padding,
                destRect.Width - 2 * padding,
                destRect.Height - 2 * padding
            );
            float widthScale = paddedRect.Width / image.Width;
            float heightScale = paddedRect.Height / image.Height;
            float scale = Math.Min(widthScale, heightScale);
            float scaledWidth = image.Width * scale;
            float scaledHeight = image.Height * scale;
            float x = paddedRect.X + (paddedRect.Width - scaledWidth) / 2;
            float y = paddedRect.Y + (paddedRect.Height - scaledHeight) / 2;
            RectangleF drawRect = new RectangleF(x, y, scaledWidth, scaledHeight);
            g.DrawImage(image, drawRect);
        }
        public static void DrawBarCode(Graphics g, Bitmap barcode, RectangleF targetArea)
        {
            if (barcode == null) return;

            float padding = 0.5f;
            RectangleF paddedRect = new RectangleF(
                targetArea.X + padding,
                targetArea.Y + padding,
                targetArea.Width - 2 * padding,  
                targetArea.Height - 2 * padding  
            );

            g.DrawImage(barcode, paddedRect);
        }
    }

    // 内容对齐方式枚举
    public enum ContentAlignment
    {
        Left,
        Center
    }

    // 矩形信息类
    public class RectangleInfo
    {
        public RectangleF RectangleMm { get; private set; }  
        public float LineWidth { get; private set; }
        public PointF[] CornersMm { get; private set; }      // 角点
        public int Number { get; set; }
        public bool Visiable { get; set; } = true;
        public string Content { get; set; } = "内容";
        public string ContentType { get; set; } = "文字";
        public string FontFamily { get; set; } = "Arial";
        public float FontSize { get; set; } = 10f;
        public bool IsBold { get; set; } = false;
        public ContentAlignment ContentAlignment { get; set; } = ContentAlignment.Left;

        public RectangleInfo(RectangleF rectMm, float lineWidth, int number)
        {
            RectangleMm = rectMm;
            LineWidth = lineWidth;
            Number = number;
            UpdateCorners();
        }

        public void UpdateRectangle(RectangleF newRectMm)
        {
            RectangleMm = newRectMm;
            UpdateCorners();
        }

        public void SetLineWidth(float lineWidth)
        {
            if (lineWidth > 0)
            {
                LineWidth = lineWidth;
            }
        }

        private void UpdateCorners()
        {
            CornersMm = new[]
            {
                new PointF(RectangleMm.Left, RectangleMm.Top),
                new PointF(RectangleMm.Right, RectangleMm.Top),
                new PointF(RectangleMm.Right, RectangleMm.Bottom),
                new PointF(RectangleMm.Left, RectangleMm.Bottom)
            };
        }
    }
}
