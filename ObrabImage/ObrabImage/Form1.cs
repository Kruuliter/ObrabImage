﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObrabImage
{
    public partial class Form1 : Form
    {
        private List<Bitmap> bitmaps = new List<Bitmap>();
        private Random random = new Random();
        public Form1()
        {
            InitializeComponent();
        }

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                menuStrip1.Enabled = trackBar1.Enabled = false;
                pictureBox1.Image = null;
                bitmaps.Clear();
                var bitmap = new Bitmap(openFileDialog1.FileName);
                await Task.Run(() => { RunProcessing(bitmap); });
                menuStrip1.Enabled = trackBar1.Enabled = true;
            }
        }

        private void RunProcessing(Bitmap bitmap)
        {
            var pixels = GetPixels(bitmap);
            var pixelsInStep = (bitmap.Width * bitmap.Height) / 100;
            var currentPixelsSet = new List<Pixel>(pixels.Count - pixelsInStep);
            for (int i = 1; i < trackBar1.Maximum; i++)
            {
                for (int j = 0; j < pixelsInStep; j++)
                {
                    var index = random.Next(pixels.Count);
                    currentPixelsSet.Add(pixels[index]);
                    pixels.RemoveAt(index);
                }
                var currendBitmap = new Bitmap(bitmap.Width, bitmap.Height);
                foreach(var pixel in currentPixelsSet)
                {
                    currendBitmap.SetPixel(pixel.Point.X, pixel.Point.Y, pixel.Color);
                }
                bitmaps.Add(currendBitmap);
                this.Invoke(new Action(() =>
                {
                    Text = $"{i}%";
                }));
            }
            bitmaps.Add(bitmap);
        }
        private List<Pixel> GetPixels(Bitmap bitmap)
        {
            var pixels = new List<Pixel>(bitmap.Width * bitmap.Height);

            for(int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    pixels.Add(new Pixel()
                    {
                        Color = bitmap.GetPixel(x, y),
                        Point = new Point()
                        {
                            X = x,
                            Y = y
                        }
                    }) ;
                }
            }
            return pixels;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (bitmaps == null || bitmaps.Count == 0)
                return;
            pictureBox1.Image = bitmaps[trackBar1.Value-1];
        }
    }
}
