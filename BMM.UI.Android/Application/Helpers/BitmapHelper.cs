using System;
using Android.Content;
using Android.Graphics;
using Android.Renderscripts;
using Android.Views;
using AndroidX.Core.Content;
using AndroidX.Palette.Graphics;

namespace BMM.UI.Droid.Application.Helpers
{

    public static class BitmapHelper
    {
        private static float BLUR_RADIUS = 25f;
        private static float BITMAP_SCALE = 0.2f;

        public static Bitmap BlurImage(Context context, Bitmap img)
        {
            try
            {
                var width = Math.Round(img.Width*BITMAP_SCALE);
                var height = Math.Round(img.Height*BITMAP_SCALE);

                var input = Bitmap.CreateScaledBitmap(img, Convert.ToInt32(width), Convert.ToInt32(height), false);
                var output = Bitmap.CreateBitmap(input);

                var c = new Canvas(output);
                Paint paint = new Paint();
                var filter = new LightingColorFilter(0x7F727272, 0x00000000);
                paint.SetColorFilter(filter);

                var script = RenderScript.Create(context);
                var intrisinc = ScriptIntrinsicBlur.Create(script, Element.U8_4(script));
                var tmpIn = Allocation.CreateFromBitmap(script, input);
                var tmpOut = Allocation.CreateFromBitmap(script, output);
                intrisinc.SetRadius(BLUR_RADIUS);
                intrisinc.SetInput(tmpIn);
                intrisinc.ForEach(tmpOut);
                tmpOut.CopyTo(output);
                c.DrawBitmap(output, 0, 0, paint);
                output = DarkenImage(output);
                return output;
            }
            catch (Exception ex)
            {
                return img;
            }
        }

        public static Color GetColor(Bitmap bitmap)
        {
            try
            {
                return new Color(bitmap.GetPixel(10, 10));
            }
            catch (Exception)
            {
                return Color.Black;
            }
        }

        public static Color GetDominantColor(Bitmap bitmap)
        {
            var palette = Palette
                .From(bitmap)
                .Generate();

            return new Color(palette.GetDominantColor(Color.Black));
        }

        public static Color Darken(Color color)
        {
            // This basically takes the primaryColor and converts it into primaryColorDark using a generic algorithm which can be used for others colors as well
            var hsv = new float[3];
            Color.ColorToHSV(color, hsv);
            hsv[2] -= 0.16f;
            return Color.HSVToColor(hsv);
        }

        public static bool BackgroundColorRequiresDarkText(Color color)
        {
            var hsv = new float[3];
            Color.ColorToHSV(color, hsv);
            return hsv[2] > 0.8f;
        }

        public static StatusBarVisibility AddFlag(this StatusBarVisibility visibility, SystemUiFlags flag)
        {
            var current = (SystemUiFlags)visibility;
            return (StatusBarVisibility)(current | flag);
        }

        public static StatusBarVisibility RemoveFlag(this StatusBarVisibility visibility, SystemUiFlags flag)
        {
            var current = (SystemUiFlags)visibility;
            return (StatusBarVisibility)(current & ~flag);
        }

        public static Bitmap DarkenImage(Bitmap img)
        {
            var canvas = new Canvas(img);
            var paint = new Paint();
            var filter = new LightingColorFilter(0x7F7F7F7F, 0x000000);
            var color = Android.Graphics.Color.Transparent;
            var gardiant = new LinearGradient(0, img.Height, 0, 0, color, Android.Graphics.Color.Black, Shader.TileMode.Clamp);
            paint.SetColorFilter(filter);
            paint.SetShader(gardiant);
            canvas.DrawBitmap(img, new Matrix(), paint);
            return img;
        }
    }
}