﻿using SkiaSharp;
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
            var info = new SKImageInfo(256, 256);
            using (var surface = SKSurface.Create(info))
            {
                SKCanvas canvas = surface.Canvas;

                canvas.Clear(SKColors.White);

                // configure our brush
                var redBrush = new SKPaint
                {
                    Color = new SKColor(0xff, 0, 0),
                    IsStroke = true,
                    IsAntialias = true
                };
                var blueBrush = new SKPaint
                {
                    Color = new SKColor(0, 0, 0xff),
                    IsStroke = true,
                    IsAntialias = true
                };




                for (int j = 0; j < 10000; j++)
                {
                    for (int i = 0; i < 64; i += 8)
                    {
                        var rect = new SKRect(i, i, 256 - i - 1, 256 - i - 1);
                        canvas.DrawRect(rect, (i % 16 == 0) ? redBrush : blueBrush);
                    }
                }

                using (SKPaint textPaint = new SKPaint())
                using (SKTypeface tf = SKTypeface.FromFamilyName("黑"))
                {
                    textPaint.Color = SKColors.Green;
                    textPaint.IsAntialias = true;
                    textPaint.TextSize = 48;
                    textPaint.Typeface = tf;
                    canvas.DrawText("汉字", 50, 50, textPaint);
                }

                PBMain.Image = Surface2Bitmap(surface);


            }
        }
    }
}