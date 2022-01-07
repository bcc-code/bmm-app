using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using BMM.UI.iOS.Extensions;
using CoreGraphics;
using UIKit;

namespace BMM.UI.iOS.Utils.ColorPalette
{
	public class ColorPaletteGenerator
	{
		private const double Range = 0.075;
		private const float MinContrastDistance = 0.5f;
		private const float VariantColorDistance = 0.3f;
		private const int MaxImageSize = 40;

		public UIColor MutedColor { get; private set; }
		public UIColor VibrantColor { get; private set; }
		public UIColor LightMutedColor { get; private set; }
		public UIColor LightVibrantColor { get; private set; }
		public UIColor DarkMutedColor { get; private set; }
		public UIColor DarkVibrantColor { get; private set; }
		private List<PixelColor> listOfColors = new List<PixelColor>();

		public void Generate (UIImage image)
		{
			listOfColors.Clear();

			image = ScaleImageDown(image);

			for (int y = 1; y < image.Size.Height - 1; y++)
			{
				for (int x = 1; x < image.Size.Width - 1; x++)
				{
					var color = GetPixelColor (new CGPoint (x, y), image);
					var index = IndexOfItemWithColorFromRange(listOfColors, color.ToYUV(), Range);

					if(index == -1)
					{
						listOfColors.Add(new PixelColor {Color = color.ToYUV(), NumberOfOccurrences = 0});
					}
					else
					{
						listOfColors [index].NumberOfOccurrences++;
					}
				}
			}

			FindColorPalette ((float)(image.Size.Width * image.Size.Height));
		}

		private UIImage ScaleImageDown(UIImage sourceImage)
		{
			var sourceSize = sourceImage.Size;
			var maxResizeFactor = Math.Min(MaxImageSize / sourceSize.Width, MaxImageSize / sourceSize.Height);

			if (maxResizeFactor > 1) 
				return sourceImage;
			
			var width = maxResizeFactor * sourceSize.Width;
			var height = maxResizeFactor * sourceSize.Height;

			UIGraphics.BeginImageContext(new CGSize(width, height));
			sourceImage.Draw(new CGRect(0, 0, width, height));

			var resultImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			return resultImage;
		}

		private UIColor GetPixelColor(CGPoint point, UIImage image)
		{
			var rawData = new byte[4];
			var handle = GCHandle.Alloc(rawData);
			UIColor resultColor = null;
			try
			{
				using (var colorSpace = CGColorSpace.CreateDeviceRGB())
				{
					using (var context = new CGBitmapContext(rawData, 1, 1, 8, 4, colorSpace, CGImageAlphaInfo.PremultipliedLast))
					{
						context.DrawImage(new CGRect(-point.X, point.Y - image.Size.Height, image.Size.Width, image.Size.Height), image.CGImage);
						float red   = (rawData[0]) / 255.0f;
						float green = (rawData[1]) / 255.0f;
						float blue  = (rawData[2]) / 255.0f;
						float alpha = (rawData[3]) / 255.0f;
						resultColor = UIColor.FromRGBA(red, green, blue, alpha);
					}
				}
			}
			finally
			{
				handle.Free();
			}

			return resultColor;
		}

		private void FindColorPalette(float numberOfPixels)
		{
			YUVColor dominantColor;
			YUVColor accentColor;

			YUVColor mutedColor;
			YUVColor vibrantColor;
			YUVColor lightMutedColor;
			YUVColor lightVibrantColor;
			YUVColor darkMutedColor;
			YUVColor darkVibrantColor;

			int count = listOfColors.Count;

			if(count >= 2)
			{
				bool isInOrder = false;

				while (isInOrder == false)
				{
					isInOrder = true;

					for (int i = 1; i < count; i++)
					{
						if (listOfColors[i - 1].NumberOfOccurrences >= listOfColors[i].NumberOfOccurrences)
							continue;
						
						(listOfColors [i - 1], listOfColors [i]) = (listOfColors [i], listOfColors [i - 1]);
						isInOrder = false;
					}
				}
			}

			dominantColor = listOfColors[0].Color;

			double biggestDifference = 0;
			int accentColorIndex = 1;

			if (count >= 2)
			{
				for (int i = 1; i < count; i++)
				{
					if(listOfColors[i].Color.DistanceTo(listOfColors[0].Color) > biggestDifference)
					{
						biggestDifference = listOfColors [i].Color.DistanceTo (listOfColors [0].Color);
						accentColorIndex = i;
					}
				}

				accentColor = listOfColors [accentColorIndex].Color;
			}
			else
			{
				accentColor = listOfColors[0].Color;
			}

			if(accentColor.DistanceTo(dominantColor) < MinContrastDistance)
			{
				accentColor = accentColor.ColorAtDistanceFrom(dominantColor, MinContrastDistance);
			}

			if(dominantColor.Y < accentColor.Y)
			{
				mutedColor = dominantColor;
				vibrantColor = accentColor;
			}
			else
			{
				mutedColor = accentColor;
				vibrantColor = dominantColor;
			}

			lightMutedColor = mutedColor.LighttenByDistane (VariantColorDistance);
			lightVibrantColor = vibrantColor.LighttenByDistane (VariantColorDistance);

			darkMutedColor = mutedColor.DarkenByDistane (VariantColorDistance);
			darkVibrantColor = vibrantColor.DarkenByDistane (VariantColorDistance);

			MutedColor = mutedColor.ToRGBUIColor ();
			VibrantColor = vibrantColor.ToRGBUIColor ();

			LightMutedColor = lightMutedColor.ToRGBUIColor ();
			LightVibrantColor = lightVibrantColor.ToRGBUIColor ();

			DarkMutedColor = darkMutedColor.ToRGBUIColor ();
			DarkVibrantColor = darkVibrantColor.ToRGBUIColor ();
		}
		
		private static int IndexOfItemWithColorFromRange(IReadOnlyList<PixelColor> list, YUVColor color, double range)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if(list[i].Color.DistanceTo(color) <= range)
					return i;
			}

			return -1;
		}
	}
}