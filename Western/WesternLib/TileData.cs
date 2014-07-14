using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace WesternLib
{
  

    public class Tileset
    {
        private List<TileData> _tiles = new List<TileData>();

        [Obsolete]
        public Tile FullTile { get; internal set; }
        public string Filename { get; set;}
        public int DefaultTileWidth { get; set; }
        public int DefaultTileHeight { get; set; }
        public List<TileData> Tiles { get { return _tiles; } set { _tiles = value; } }

        public bool AutoTile { get; set; }
    }

    public class TileLoader : XmlHelper
    {
        private static string FILE_FORMAT_VERSION = "0.2";
        private static string FILENAME = "data/TileData.xml";

        public static Dictionary<string, Tileset> LoadTilesets(XElement rootElement)
        {
            var tilesets = new Dictionary<string, Tileset>();
            foreach (XElement tilesetElement in rootElement.Elements("Tileset"))
            {
                string filename = tilesetElement.GetAttribute("Filename");
                var tileset = new Tileset()
                {
                    DefaultTileWidth = tilesetElement.GetAttribute("DefaultTileWidth", 32),
                    DefaultTileHeight = tilesetElement.GetAttribute("DefaultTileHeight", 32),
                    AutoTile = tilesetElement.GetAttribute("AutoTile", false),
                    Filename = filename
                };

                IEnumerable<TileData> data = from w in tilesetElement.Elements("Tile")
                                             select new TileData()
                                             {
                                                 Width = (int)w.GetAttribute("Width", tileset.DefaultTileWidth),
                                                 Height = (int)w.GetAttribute("Height", tileset.DefaultTileHeight),
                                                 Name = (string)w.Attribute("Name"),
                                                 OffsetX = (int)w.GetAttribute("OffsetX", 0),
                                                 OffsetY = (int)w.GetAttribute("OffsetY", 0),
                                                 AnimNum = w.GetAttribute("AnimNum", 1),
                                                 AnimSpeed = w.GetAttribute("AnimSpeed", 0.5f)
                                             };
                tileset.Tiles = data.ToList();
                if (tilesets.ContainsKey(tileset.Filename))
                    Console.WriteLine("Warning: file" + tileset.Filename + " was added twice.");
                else
                    tilesets.Add(tileset.Filename, tileset);
            }
            return tilesets;
        }

        public static Dictionary<string, Tileset> Load()
        {
            var tilesets = new Dictionary<string, Tileset>();
            XDocument doc = XDocument.Load(FILENAME);
            tilesets = LoadTilesets(doc.Root);

            return tilesets;
        }

        public static Dictionary<string, Tileset> LoadAutoTilesets()
        {
            var tilesets = new Dictionary<string, Tileset>();
            DirectoryInfo autoTileDir = new DirectoryInfo("graphics/tilesets");

            foreach (FileInfo file in autoTileDir.EnumerateFiles("*.png"))
            {
                string filename = Path.Combine("graphics", Path.Combine("tilesets", file.Name));
                var tileset = LoadAutoTileset(filename);
                tilesets.Add(tileset.Filename, tileset);
            }
            return tilesets;
        }

        public static Tileset LoadAutoTileset(string filename)
        {
            var tileset = new Tileset()
            {
                AutoTile = true,
                DefaultTileHeight = GameConstants.TILE_SIZE,
                DefaultTileWidth = GameConstants.TILE_SIZE,
                Filename = filename
            };
            return tileset;
        }
    }

}
