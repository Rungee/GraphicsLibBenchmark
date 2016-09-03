using System.Drawing;
using System.IO;

using BenchmarkDotNet.Attributes;

using SkiaSharp;

namespace GraphicsLibBenchmark
{
  public class Benchmarks
  {
    private class LineSegment
    {
      public float X0;
      public float Y0;
      public float X1;
      public float Y1;

      public LineSegment(float x0, float y0, float x1, float y1)
      {
        X0 = x0;
        Y0 = y0;
        X1 = x1;
        Y1 = y1;
      }
    }

    private float STROKE_WIDTH = 2;

    [Params(true, false)]
    public bool AntiAlias = true;

    private readonly SKSurface skSurface;
    private readonly SKCanvas skCanvas;
    private readonly SKPaint skPaintSolidThick;
    private readonly SKPaint skPaintSolidThin;
    private readonly SKPaint skPaintDashedThick;
    private readonly SKPaint skPaintDashedThin;

    private readonly Graphics gdiGraphics;
    private readonly Pen gdiPenThick;
    private readonly Pen gdiPenThin;
    private readonly Pen gdiPenDashedThin;
    private readonly Pen gdiPenDashedThick;

    private readonly LineSegment[] lineSegments;

    public Benchmarks()
    {
      skSurface = SKSurface.Create(width: 1024, height: 1024, colorType: SKImageInfo.PlatformColorType, alphaType: SKAlphaType.Premul);
      skCanvas = skSurface.Canvas;
      skCanvas.ClipRect(new SKRect(0, 0, 1024, 1024));

      skPaintSolidThick = new SKPaint();
      skPaintSolidThick.StrokeWidth = STROKE_WIDTH;
      skPaintSolidThick.IsAntialias = AntiAlias;
      skPaintSolidThick.Style = SKPaintStyle.Fill;

      skPaintSolidThin = new SKPaint();
      skPaintSolidThin.StrokeWidth = 1;
      skPaintSolidThin.IsAntialias = AntiAlias;

      SKPathEffect dashEffect = SKPathEffect.CreateDash(new float[] { 2F, 6F }, 1F);

      skPaintDashedThick = new SKPaint();
      skPaintDashedThick.StrokeWidth = STROKE_WIDTH;
      skPaintDashedThick.IsAntialias = AntiAlias;
      skPaintDashedThick.PathEffect = dashEffect;

      skPaintDashedThin = new SKPaint();
      skPaintDashedThin.StrokeWidth = 1;
      skPaintDashedThin.IsAntialias = AntiAlias;
      skPaintDashedThin.PathEffect = dashEffect;


      Bitmap bmp = new Bitmap(1024, 1024);
      gdiGraphics = Graphics.FromImage(bmp);
      gdiPenThick = new Pen(Color.Red, STROKE_WIDTH);
      gdiPenThin = new Pen(Color.Red, 1F);
      gdiPenDashedThin = new Pen(Color.Red, 1F);
      gdiPenDashedThin.DashPattern = new float[] { 2F, 6F };
      gdiPenDashedThin.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;

      gdiPenDashedThick = new Pen(Color.Red, STROKE_WIDTH);
      gdiPenDashedThick.DashPattern = new float[] { 2F, 6F };
      gdiPenDashedThick.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;

      lineSegments = new LineSegment[1];
      lineSegments[0] = new LineSegment(-10F, -10F, 1200F, 1200F);
    }

    [Setup]
    public void Setup()
    {
      skPaintSolidThick.IsAntialias = AntiAlias;
      skPaintSolidThin.IsAntialias = AntiAlias;
      skPaintDashedThick.IsAntialias = AntiAlias;
      skPaintDashedThin.IsAntialias = AntiAlias;
      
      if (AntiAlias)
      {
        gdiGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //gdiGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
      }
      else
      {
        gdiGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
      }
    }

    [Benchmark]
    public void Skia_Clear()
    {
      skCanvas.Clear(new SKColor(145, 145, 156));
    }

    [Benchmark]
    public void GDI_Clear()
    {
      gdiGraphics.Clear(Color.FromArgb(145, 145, 156));
    }

    [Benchmark]
    public void Skia_DrawLines_SolidThick()
    {
      foreach (LineSegment ls in lineSegments)
        skCanvas.DrawLine(ls.X0, ls.Y0, ls.X1, ls.Y1, skPaintSolidThick);
    }

    [Benchmark]
    public void GDI_DrawLines_SolidThick()
    {
      foreach (LineSegment ls in lineSegments)
        gdiGraphics.DrawLine(gdiPenThick, ls.X0, ls.Y0, ls.X1, ls.Y1);
    }

    [Benchmark]
    public void Skia_DrawLines_SolidThin()
    {
      foreach (LineSegment ls in lineSegments)
        skCanvas.DrawLine(ls.X0, ls.Y0, ls.X1, ls.Y1, skPaintSolidThin);
    }

    [Benchmark]
    public void GDI_DrawLines_SolidThin()
    {
      foreach (LineSegment ls in lineSegments)
        gdiGraphics.DrawLine(gdiPenThin, ls.X0, ls.Y0, ls.X1, ls.Y1);
    }

    [Benchmark]
    public void Skia_DrawLines_DashedThick()
    {
      foreach (LineSegment ls in lineSegments)
        skCanvas.DrawLine(ls.X0, ls.Y0, ls.X1, ls.Y1, skPaintDashedThick);
    }

    [Benchmark]
    public void GDI_DrawLines_DashedThick()
    {
      foreach (LineSegment ls in lineSegments)
        gdiGraphics.DrawLine(gdiPenDashedThick, ls.X0, ls.Y0, ls.X1, ls.Y1);
    }

    [Benchmark]
    public void Skia_DrawLines_DashedThin()
    {
      foreach (LineSegment ls in lineSegments)
        skCanvas.DrawLine(ls.X0, ls.Y0, ls.X1, ls.Y1, skPaintDashedThin);
    }

    [Benchmark]
    public void GDI_DrawLines_DashedThin()
    {
      foreach (LineSegment ls in lineSegments)
        gdiGraphics.DrawLine(gdiPenDashedThin, ls.X0, ls.Y0, ls.X1, ls.Y1);
    }
  }
}
