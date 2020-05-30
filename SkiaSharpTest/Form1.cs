using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SkiaSharp.Views.Desktop;
using System.IO;

namespace SkiaSharpTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        internal Bitmap Surface2Bitmap(SKSurface surface)
        {
            using (SKImage image = surface.Snapshot())
            using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (MemoryStream mStream = new MemoryStream(data.ToArray()))
            {
                Bitmap bm = new Bitmap(mStream, false);
                return bm;
            }
        }

        
        private void Button1_Click(object sender, EventArgs e)
        {
            var info = new SKImageInfo(1024, 1024);
            using (var surface = SKSurface.Create(info))
            {
                //now ,can draw sth with canvas
                SKCanvas canvas = surface.Canvas;
                canvas.Clear(SKColors.White);

                //using DrawTextOnPath to draw sth
                var paint1 = new SKPaint
                {
                    TextSize = 32.0f,
                    IsAntialias = true,
                    Color = new SKColor(255, 0, 0),
                    Style = SKPaintStyle.Fill
                };
                var paint2 = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 10
                   // PathEffect=SKPathEffect.CreateDiscrete(3,1)
                };

                var innerpaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = new SKColor(0, 255, 0),
                    // PathEffect=SKPathEffect.CreateDiscrete(3,1)
                };

                int[] temp = new int[] { 2, 1, 0, 2, 1, 2, 2, 2,0, 0,1,0,2,0,0,1 };
                var pts = DrawTools.Tools.ints2Pts(temp);
                var ptsPath = DrawTools.Tools.Points2Path(ref pts,150);

                var path = new SKPath();
                path.AddArc(new SKRect(100, 100, 300, 500), 225, 90);
                
                canvas.DrawPath(ptsPath, paint2);
                canvas.DrawPath(ptsPath, innerpaint);
                canvas.DrawTextOnPath("11111111111111111111111111111111111111111"+
                    "222222222222222222222222222222222222222222"+
                    "333333333333333333333333333333333333333333"+
                    "444444444444444444444444444444444444444444"+
                    "555555555555555555555555555555555555555555",
                    ptsPath, 0, -2, paint1);

                //// configure our brush
                //var redBrush = new SKPaint
                //{
                //    Color = new SKColor(0xff, 0, 0),
                //    IsStroke = true,
                //    IsAntialias = true
                //};
                //var blueBrush = new SKPaint
                //{
                //    Color = new SKColor(0, 0, 0xff),
                //    IsStroke = true,
                //    IsAntialias = true
                //};
                //for (int j = 0; j < 10000; j++)
                //{
                //    for (int i = 0; i < 64; i += 8)
                //    {
                //        var rect = new SKRect(i, i, 256 - i - 1, 256 - i - 1);
                //        canvas.DrawRect(rect, (i % 16 == 0) ? redBrush : blueBrush);
                //    }
                //}

                ////draw text
                //using (SKPaint textPaint = new SKPaint())
                //using (SKTypeface tf = SKTypeface.FromFamilyName("黑"))
                //{
                //    textPaint.Color = SKColors.Green;
                //    textPaint.IsAntialias = true;
                //    textPaint.TextSize = 48;
                //    textPaint.Typeface = tf;
                //    canvas.DrawText("汉字", 50, 50, textPaint);
                //}

                PBMain.Image = Surface2Bitmap(surface);


            }
        }
    }
}
