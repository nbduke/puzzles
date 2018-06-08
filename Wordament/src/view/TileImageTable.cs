/*
 * TileImageTable.cs
 * 
 * Nathan Duke
 * 7/29/13
 * WordamentApp
 * 
 * Contains the TileImageTable static class and the ImageType enum.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Wordament.View
{
	/*
	 * The following enum provides a descriptive way to refer to the app's images in the code. 
	 * The names should be fairly self-explanatory.
	 */
	enum ImageType
	{
		North,
		Northeast,
		East,
		Southeast,
		South,
		Southwest,
		West,
		Northwest,
		Start_North,
		Start_Northeast,
		Start_East,
		Start_Southeast,
		Start_South,
		Start_Southwest,
		Start_West,
		Start_Northwest,
		Stop,
		Background,
		NotInPath
	}

	/*
	 * This static class provides static methods for managing a Singleton collection of all images used in the 
	 * WordamentApp GUI.
	 */
	static class TileImageTable
	{
		/*
		 * The collection of images. Maps ImageType to a unique Image instance.
		 */
		private static Dictionary<ImageType, Image> ImageMap = new Dictionary<ImageType, Image>();

		private const string resourcePath = "../../resources/";

		/*
		 * Loads the .png files in the app's resources folder into ImageMap.
		 */
		public static bool LoadImages()
		{
			try
			{
				ImageMap[ImageType.Background] = Resize(Image.FromFile(resourcePath + "background.png"), ShowPathsForm.BACKGROUND_IMAGE_SIZE);
				ImageMap[ImageType.North] = Resize(Image.FromFile(resourcePath + "north.png"), ShowPathsForm.TILE_IMAGE_SIZE);
				ImageMap[ImageType.Northeast] = Resize(Image.FromFile(resourcePath + "northeast.png"), ShowPathsForm.TILE_IMAGE_SIZE);
				ImageMap[ImageType.East] = Resize(Image.FromFile(resourcePath + "east.png"), ShowPathsForm.TILE_IMAGE_SIZE);
				ImageMap[ImageType.Southeast] = Resize(Image.FromFile(resourcePath + "southeast.png"), ShowPathsForm.TILE_IMAGE_SIZE);
				ImageMap[ImageType.South] = Resize(Image.FromFile(resourcePath + "south.png"), ShowPathsForm.TILE_IMAGE_SIZE);
				ImageMap[ImageType.Southwest] = Resize(Image.FromFile(resourcePath + "southwest.png"), ShowPathsForm.TILE_IMAGE_SIZE);
				ImageMap[ImageType.West] = Resize(Image.FromFile(resourcePath + "west.png"), ShowPathsForm.TILE_IMAGE_SIZE);
				ImageMap[ImageType.Northwest] = Resize(Image.FromFile(resourcePath + "northwest.png"), ShowPathsForm.TILE_IMAGE_SIZE);
				ImageMap[ImageType.Start_North] = Resize(Image.FromFile(resourcePath + "start_north.png"), ShowPathsForm.TILE_IMAGE_SIZE);
				ImageMap[ImageType.Start_Northeast] = Resize(Image.FromFile(resourcePath + "start_northeast.png"), ShowPathsForm.TILE_IMAGE_SIZE);
				ImageMap[ImageType.Start_East] = Resize(Image.FromFile(resourcePath + "start_east.png"), ShowPathsForm.TILE_IMAGE_SIZE);
				ImageMap[ImageType.Start_Southeast] = Resize(Image.FromFile(resourcePath + "start_southeast.png"), ShowPathsForm.TILE_IMAGE_SIZE);
				ImageMap[ImageType.Start_South] = Resize(Image.FromFile(resourcePath + "start_south.png"), ShowPathsForm.TILE_IMAGE_SIZE);
				ImageMap[ImageType.Start_Southwest] = Resize(Image.FromFile(resourcePath + "start_southwest.png"), ShowPathsForm.TILE_IMAGE_SIZE);
				ImageMap[ImageType.Start_West] = Resize(Image.FromFile(resourcePath + "start_west.png"), ShowPathsForm.TILE_IMAGE_SIZE);
				ImageMap[ImageType.Start_Northwest] = Resize(Image.FromFile(resourcePath + "start_northwest.png"), ShowPathsForm.TILE_IMAGE_SIZE);
				ImageMap[ImageType.Stop] = Resize(Image.FromFile(resourcePath + "stop.png"), ShowPathsForm.TILE_IMAGE_SIZE);
				ImageMap[ImageType.NotInPath] = Resize(Image.FromFile(resourcePath + "not_in_path.png"), ShowPathsForm.TILE_IMAGE_SIZE);

				return true;
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine("Could not find an image file: {0}", e.Message);
				return false;
			}
		}

		/*
		 * Returns an Image that is the passed-in image re-dimensioned according to newSize.
		 */
		public static Image Resize(Image img, Size newSize)
		{
			return new Bitmap(img, newSize);
		}

		/*
		 * Returns the Image mapped to by type, or null if the mapping does not exist.
		 */
		public static Image GetImage(ImageType type)
		{
			if (ImageMap.ContainsKey(type))
				return ImageMap[type];
			else
				return null;
		}
	}
}
