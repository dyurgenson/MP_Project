using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace YA_Metro.Drawing
{
    /// <summary>
    /// Класс TextRenderer предоставляет функциональность для рендеринга текста в текстуру OpenGL.
    /// Он использует System.Drawing для отрисовки текста на Bitmap, а затем загружает этот Bitmap в текстуру OpenGL.
    /// </summary>
    public class TextRenderer : IDisposable
    {
        private readonly Bitmap _bitmap;
        private readonly System.Drawing.Graphics _graphics;
        private readonly int _texture;
        private Rectangle _dirtyRegion;

        /// <summary>
        /// Ширина текстуры.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Высота текстуры.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Идентификатор текстуры OpenGL.
        /// </summary>
        public int Texture
        {
            get
            {
                this.UploadBitmap();
                return this._texture;
            }
        }

        /// <summary>
        /// Конструктор класса TextRenderer.
        /// </summary>
        /// <param name="width">Ширина текстуры.</param>
        /// <param name="height">Высота текстуры.</param>
        public TextRenderer(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this._bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            this._graphics = System.Drawing.Graphics.FromImage((Image)this._bitmap);
            this._graphics.Clear(Color.Transparent);
            this._graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            this._dirtyRegion = new Rectangle(0, 0, this._bitmap.Width, this._bitmap.Height);
            this._texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, this._texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 9729);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 9729);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
        }

        /// <summary>
        /// Отрисовывает строку текста на Bitmap.
        /// </summary>
        /// <param name="text">Текст для отрисовки.</param>
        /// <param name="font">Шрифт текста.</param>
        /// <param name="brush">Кисть для отрисовки текста.</param>
        /// <param name="point">Точка, где начнется отрисовка текста.</param>
        public void DrawString(string text, Font font, Brush brush, PointD point)
        {
            this._graphics.DrawString(text, font, brush, new PointF((float)point.X, (float)point.Y));
            SizeF size = this._graphics.MeasureString(text, font);
            this._dirtyRegion = Rectangle.Round(RectangleF.Union((RectangleF)this._dirtyRegion, new RectangleF(PointF.Empty, size)));
            this._dirtyRegion = Rectangle.Intersect(this._dirtyRegion, new Rectangle(0, 0, this._bitmap.Width, this._bitmap.Height));
        }

        /// <summary>
        /// Загружает Bitmap в текстуру OpenGL.
        /// </summary>
        private void UploadBitmap()
        {
            if ((RectangleF)this._dirtyRegion == RectangleF.Empty)
                return;
            BitmapData bitmapdata = this._bitmap.LockBits(this._dirtyRegion, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.BindTexture(TextureTarget.Texture2D, this._texture);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, this._dirtyRegion.X, this._dirtyRegion.Y, this._dirtyRegion.Width, this._dirtyRegion.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapdata.Scan0);
            this._bitmap.UnlockBits(bitmapdata);
            this._dirtyRegion = Rectangle.Empty;
        }

        /// <summary>
        /// Освобождает ресурсы, используемые TextRenderer.
        /// </summary>
        public void Dispose()
        {
            this._bitmap.Dispose();
            this._graphics.Dispose();
            if (GraphicsContext.CurrentContext == null)
                return;
            GL.DeleteTexture(this._texture);
        }
    }
}