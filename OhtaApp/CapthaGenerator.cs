using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;
using Brushes = System.Drawing.Brushes;
using System.Drawing;

namespace OhtaApp
{
    public class CapthaGenerator
    {
        public string text; // Поле класса
        // Алфавит цветов
        Brush[] colors = {
                     Brushes.Black,
                     Brushes.Red,
                     Brushes.BlueViolet,
                     Brushes.Yellow,
                     Brushes.RoyalBlue,
                     Brushes.Green,
                     Brushes.Magenta,
                     Brushes.Black,
                     Brushes.Azure
            };
        #region Captha Maker
        // Создание капчи
        public ImageSource CreateImageHard(int Width, int Height) // Создание сложного изображения
        {
            Random rnd = new Random();
            // Иницализация изображения
            Bitmap result = new Bitmap(Width, Height);

            //Вычислим позицию текста
            int Xpos = rnd.Next(0, Width - 50);
            int Ypos = rnd.Next(15, Height - 15);

            // Укажем где рисовать
            Graphics g = Graphics.FromImage((Image)result);
            Color ImageBackColor = new Color();
            // Фон картинки
            g.Clear(ImageBackColor = Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255)));

            // Генерируем текст
            text = String.Empty;
            string ALF = "1234567890QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm";
            for (int i = 0; i < 5; ++i)
                text += ALF[rnd.Next(ALF.Length)];

            // Генерируем угол поворота текста
            text += ALF[rnd.Next(ALF.Length)];
            Int16[] rotate = { 1, -1, 2, -2, 3, -3, 4, -4, 5, -5, 6, -6 };
            g.RotateTransform(rnd.Next(rotate.Length));

            // Рисуем сгенирируемый текст
            g.DrawString(text,
                         new Font("Arial", 120),
                         colors[rnd.Next(colors.Length)],
                         new PointF(Xpos, Ypos));

            #region Noise
            // Линии из углов
            g.DrawLine(Pens.Black,
                       new System.Drawing.Point(rnd.Next(255), 0),
                       new System.Drawing.Point(Width - 1, Height - 1));
            g.DrawLine(Pens.Black,
                       new System.Drawing.Point(rnd.Next(255), Height - 1),
                       new System.Drawing.Point(Width - 1, 0));
            // Белые точки
            Color DotsColor = new Color();
            DotsColor = Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255));
            for (int i = 0; i < Width; ++i)
                for (int j = 0; j < Height; ++j)
                    if (rnd.Next() % 20 == 0)
                        result.SetPixel(i, j, DotsColor);
            #endregion
            return BitmapToImageSource(result);
        }
        #endregion
        // Переводчик для конечного изображения капчи
        private BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }
        // Проверка текста на соотвествие капче
        public bool CapthaChecker(string str)
        {
            if (str == text)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
