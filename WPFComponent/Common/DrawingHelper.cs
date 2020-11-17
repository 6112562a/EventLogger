using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Path = System.Windows.Shapes.Path;

namespace WPFComponent.Common
{
    /// <summary>
    /// Path对象转换为图片帮助类，支持图片组合
    /// </summary>
    public static class DrawingHelper
    {
        public static RenderTargetBitmap GetImage(string strId)
        {
            //todo 对象未初始化无法呈现视觉
            var path = Application.Current.TryFindResource(strId) as FrameworkElement;
            double width = path.ActualWidth;
            double height = path.ActualHeight;
            RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 100, 100, PixelFormats.Default);
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                VisualBrush visualBrush = new VisualBrush(path);
                drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(), new Size(width, height)));
            }
            bmpCopied.Render(drawingVisual);
            return bmpCopied;
        }
        /// <summary>
        /// 将指定Key的Path对象转换为DrawingImage
        /// </summary>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static DrawingImage Convert2Image(string strKey)
        {
            return new DrawingImage(Convert2GeometryDrawing(strKey));
        }
        /// <summary>
        /// 将指定Path对象转换为DrawingImage
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static DrawingImage Convert2Image(Path path)
        {
            return new DrawingImage(Convert2GeometryDrawing(path));
        }
        /// <summary>
        /// 将Path转化为GeometryDrawing
        /// </summary>
        /// <param name="strKey">资源Key</param>
        /// <returns></returns>
        private static GeometryDrawing Convert2GeometryDrawing(string strKey)
        {
            var path = Application.Current.TryFindResource(strKey) as Path;
            var drawing = Convert2GeometryDrawing(path);
            return drawing;
        }
        /// <summary>
        /// 将Path转化为GeometryDrawing
        /// </summary>
        /// <param name="path">Path源</param>
        /// <returns></returns>
        public static GeometryDrawing Convert2GeometryDrawing(Path path)
        {
            var drawing = new GeometryDrawing();
            if (path != null)
            {
                drawing.Brush = path.Fill;
                var pen = new Pen(path.Stroke, path.StrokeThickness)
                {//转换画笔实例
                    LineJoin = path.StrokeLineJoin,
                    MiterLimit = path.StrokeMiterLimit,
                    StartLineCap = path.StrokeStartLineCap,
                    EndLineCap = path.StrokeEndLineCap,
                    DashCap = path.StrokeDashCap,
                    DashStyle = new DashStyle(path.StrokeDashArray, path.StrokeDashOffset)
                };
                drawing.Pen = pen;
                drawing.Geometry = path.Data;
            }
            return drawing;
        }
        /// <summary>
        /// 批量转换指定Key的Path对象为DrawingImage
        /// </summary>
        /// <param name="strKeys"></param>
        /// <returns></returns>
        public static DrawingImage Convert2Image(params string[] strKeys)
        {
            DrawingGroup group = new DrawingGroup();
            foreach (var strKey in strKeys)
            {
                group.Children.Add(Convert2GeometryDrawing(strKey));
            }
            return new DrawingImage(group);
        }
        /// <summary>
        /// 将字符串集合转化为GeometryGroup
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public static GeometryGroup Convert2GeometryGroup(List<string> dataList)
        {
            GeometryGroup group = new GeometryGroup();
            foreach (var data in dataList)
            {
                group.Children.Add(Geometry.Parse(data));
            }
            return group;
        }


        /// <summary>
        /// 保存控件为图片格式
        /// </summary>
        /// <param name="control"></param>
        public static void SaveImage(FrameworkElement control)
        {
            //对话框
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "PNG文件(*.png)|*.png|JPG文件(*.jpg)|*.jpg|BMP文件(*.bmp)|*.bmp|GIF文件(*.gif)|*.gif|TIF文件(*.tif)|*.tif";
            if (dialog.ShowDialog() == true)
            {
                var fileName = dialog.FileName;

                //第一步：建立DrawingVisual,将Control转换为DrawingVisual
                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext context = drawingVisual.RenderOpen())
                {
                    VisualBrush brush = new VisualBrush(control) { Stretch = Stretch.None };
                    context.DrawRectangle(brush, null, new Rect(0, 0, control.ActualWidth, control.ActualHeight));
                    context.Close();
                }
                //第二步:建立RenderTargetBitmap，将DrawingVisual转化为RenderTargetBitmap
                //--获取Dpi-- 
                float dpiX, dpiY;
                using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
                {
                    dpiX = graphics.DpiX;
                    dpiY = graphics.DpiY;
                }
                RenderTargetBitmap bitmap = new RenderTargetBitmap((int)control.ActualWidth + 1, (int)control.ActualHeight + 1, dpiX, dpiY, PixelFormats.Pbgra32);
                bitmap.Render(drawingVisual);

                string extensionString = System.IO.Path.GetExtension(fileName);
                BitmapEncoder encoder = null;
                switch (extensionString)
                {
                    case ".jpg":
                        encoder = new JpegBitmapEncoder();
                        break;
                    case ".png":
                        encoder = new PngBitmapEncoder();
                        break;
                    case ".bmp":
                        encoder = new BmpBitmapEncoder();
                        break;
                    case ".gif":
                        encoder = new GifBitmapEncoder();
                        break;
                    case ".tif":
                        encoder = new TiffBitmapEncoder();
                        break;
                    default:
                        throw new InvalidOperationException();


                }
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                using (Stream stm = File.Create(fileName))
                {
                    encoder.Save(stm);
                }
                MessageBox.Show(string.Format("成功保存到{0}!", fileName), "提示", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }


        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="serverImage">图片</param>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片高度</param>
        public static System.Drawing.Image GetThumbnail(System.Drawing.Image serverImage, int width, int height)
        {
            //画板大小
            int towidth = width;
            int toheight = height;
            //缩略图矩形框的像素点
            int ow = serverImage.Width;
            int oh = serverImage.Height;

            if (ow > oh)
            {
                toheight = serverImage.Height * width / serverImage.Width;
            }
            else
            {
                towidth = serverImage.Width * height / serverImage.Height;
            }
            //新建一个bmp图片
            System.Drawing.Image bm = new System.Drawing.Bitmap(width, height);
            //新建一个画板
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bm);
            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充
            g.Clear(System.Drawing.Color.White);
            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(serverImage, new System.Drawing.Rectangle((width - towidth) / 2, (height - toheight) / 2, towidth, toheight),
                0, 0, ow, oh,
                System.Drawing.GraphicsUnit.Pixel);

            try
            {
                return bm;
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                g.Dispose();
            }
        }
    }

}
