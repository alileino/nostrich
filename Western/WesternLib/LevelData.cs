using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace WesternLib
{

    public class LevelData : XmlHelper
    {
        private string _filename; // you should never set this directly, use the property Filename
        protected List<Tile[]> _tiles;
        protected int[][] _collision;
        protected List<Trigger> _triggers = new List<Trigger>();
        protected Dictionary<string, Point> _locations = new Dictionary<string,Point>();
        private static string CUR_FILE_FORMAT_VERSION = "0.2";
        public string Name { get; set; }
        public string Filename 
        { 
            get 
            { 
                return _filename; 
            }
            protected set
            {
                _filename = value;
                Name = Path.GetFileNameWithoutExtension(value);
            }
        }
        public int Layers { get; set; }
        public List<Tile[]> Tiles { get { return _tiles; } }
        public int Width { get; set; }
        public int Height { get; set; }
        protected ResourceManager _resourceMgr;
        private Dictionary<string, Tileset> _autoTilesets = new Dictionary<string,Tileset>();
        public Dictionary<string, Point> Locations { get { return _locations; } }
        public Dictionary<string, Tileset> AutoTilesets
        {
            get { return _autoTilesets; }
        }

        internal List<Trigger> Triggers { get { return _triggers; } }

        public Tile[] TilesAt(int x, int y)
        {
            Tile[] row = _tiles[y];
            Tile[] tiles = new Tile[Layers];
            Array.Copy(row, x * Layers, tiles, 0, Layers);
            return tiles;
        }

        public Point GetLocationOrDefault(string location)
        {
            if(_locations.ContainsKey(location))
                return _locations[location];
            return _locations["default"];
        }

        private bool IsValidCoordinate(int x, int y, int layer)
        {
            return !(x < 0 || y < 0 || x >= Width || y >= Height || layer >= Layers || layer < 0);
        }

        public Tile GetTile(int x, int y, int layer)
        {
            if (IsValidCoordinate(x,y,layer))
                return _tiles[y][x * Layers + layer];
            return null;
        }

        public void SetTile(Tile t, int x, int y, int layer)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return;
            _tiles[y][x * Layers + layer] = t;
        }

        public void SetCollision(int colNum, int x, int y)
        {
            if (IsValidCoordinate(x, y, 0))
            {
                _collision[y][x] = colNum;
                
            }
            
        }

        public int GetCollision(int x, int y)
        {
            if (IsValidCoordinate(x, y, 0))
                return _collision[y][x];
            return 0;
        }

        public int GetCollision(System.Drawing.Point p)
        {
            return GetCollision(p.X, p.Y);
        }


        public void Pad(int left, int top,int right, int bottom)
        {
            int newWidth = Width+left+right;
            int newHeight = Height+top+bottom;
            List<Tile[]> newTiles = GetTileArray(newWidth, newHeight, Layers);
            for (int y = top; y < newHeight-bottom; y++)
            {
                for (int xl = left*Layers; xl < newWidth*Layers - right*Layers; xl++)
                {
                    newTiles[y][xl] = _tiles[y - top][xl - left * Layers];
                }
            }
            _tiles = newTiles;

            Height = _tiles.Count;
            Width = _tiles.Count > 0 ? _tiles[0].Length / Layers : 0;

            #region collision
            int[][] newCol = GetCollisionArray(Width, Height);
            for (int y = top; y < Height-bottom; y++)
            {
                for (int x = left; x < Width-right; x++) // todo: arraycopy would be better here
                {
                    newCol[y][x] = _collision[y - top][x - left];
                }
            }
            _collision = newCol;
            foreach (Trigger trigger in _triggers)
            {
                trigger.MoveTrigger(left, top);
            }

            List<string> locationNames = new List<string>(_locations.Keys);
            foreach (string locName in locationNames)
            {
                Point loc = _locations[locName];
                loc.Offset(left, top);
                _locations[locName] = loc;
            }
            #endregion
        }


        public void Pad(Rectangle r)
        {
            Pad(r.Left, r.Top, r.Right, r.Bottom);
        }


        public Tileset GetAutoTileset(string name)
        {
            Tileset tileset;
            if (_autoTilesets.ContainsKey(name))
                tileset = _autoTilesets[name];
            else
            {
                tileset = TileLoader.LoadAutoTileset(name);
                AutoTilesets.Add(tileset.Filename, tileset);
            }
            return tileset;
        }

        public LevelData(ResourceManager resourceManager)
        {
            Enemy.DoesCollide = new Enemy.CollisionHandler(GetCollision);
            _resourceMgr = resourceManager;
            Layers=2;
        }

        public void Unload()
        {
            _resourceMgr.UnloadTilesets(AutoTilesets);
            foreach (Trigger trigger in _triggers)
            {
                trigger.Unload();
            }
        }

        protected virtual void LevelLoaded()
        {
            Console.WriteLine("Level loaded: " + Name);
        }

        private List<Tile[]> GetTileArray(int width, int height, int layers)
        {
            List<Tile[]> tiles = new List<Tile[]>();
            for (int y = 0; y < height; y++)
            {
                Tile[] row = new Tile[width * layers];
                tiles.Add(row);
            }
            return tiles;
        }

        public void New()
        {
            int width = 100, height = 100, layers = 3;
            Width = width;
            Height = height;
            Layers = layers;
            _tiles = GetTileArray(width, height, layers);
            Filename = "levels/unnamed.xml";
            _collision = GetCollisionArray(width, height);
            
        }

        private string ResolveFilename(string filename)
        {
            //string fname = Path.Combine("levels", filename);
            filename = Path.GetFileName(filename);
            DirectoryInfo di = new DirectoryInfo("levels");
            FileInfo fi = di.GetFiles(filename + "*").FirstOrDefault();
            return fi.FullName;
        }

        public void Load(string filename)
        {
            try
            {
                filename = ResolveFilename(filename);
                Filename = filename;
                XDocument doc = XDocument.Load(filename);
                
                Width = doc.Root.GetAttribute("Width", 10);
                Height = doc.Root.GetAttribute("Height", 10);
                Layers = doc.Root.GetAttribute("Layers", 3);

                LoadTriggers(doc.Root);
                LoadProperties(doc.Root);
                _autoTilesets = TileLoader.LoadTilesets(doc.Root);
                _resourceMgr.Load(_autoTilesets);

                _tiles = GetTileArray(Width, Height, Layers);
                _collision = LoadCollision(doc.Root);

                foreach (XElement tileElement in doc.Root.Elements("Tile"))
                    LoadTile(tileElement);
                LevelLoaded();
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load " + filename);
            }
        }

        private void LoadProperties(XElement rootElement)
        {
            foreach (XElement propertyElement in rootElement.Elements("Location"))
            {
                string name = propertyElement.GetAttribute("Name");
                string pointString = propertyElement.Value;
                Point tile = PointExtensions.Parse(pointString);
                _locations.Add(name, tile);
            }
            if (!_locations.ContainsKey("default"))
                _locations["default"] = new Point(0, 0);
        }

        private void SaveProperties(XmlWriter writer)
        {
            foreach (var pair in _locations)
            {
                writer.WriteStartElement("Location");
                writer.WriteAttribute("Name", pair.Key);
                writer.WriteString(pair.Value.ToString(','));
                writer.WriteEndElement();
            }
        }

        private int[][] GetCollisionArray(int width, int height)
        {
            int[][] col = new int[height][];
            for (int i = 0; i < height; i++)
                col[i] = new int[width];
            return col;
        }

        private int[][] LoadCollision(XElement rootElement)
        {
            int[][] col = GetCollisionArray(Width, Height);
            XElement collisionElement = rootElement.Element("Collision");
            if (collisionElement == null)
                return col;
            string collisionData = collisionElement.Value;
            if (collisionData == null || collisionData == "")
                return col;
            List<int[]> coordinates = LoadTriplets(collisionData);
            foreach (int[] coord in coordinates)
            {
                col[coord[1]][coord[0]] = coord[2];
            }
            return col;

        }

        private void LoadTriggers(XElement rootElement)
        {
            
            foreach (XElement triggerElement in rootElement.Elements("Trigger"))
            {
                Trigger trigger;
                string type = triggerElement.GetAttribute("Type", "area").ToLower();
                string content = triggerElement.Value;
                switch (type)
                {
                    case "area":
                        trigger = new AreaTrigger();
                        (trigger as AreaTrigger).Area = RectangleExtensions.Parse(content);
                        break;
                    default:
                        Console.WriteLine("Trigger type: " + type + " is undefined.");
                        continue;
                }

                trigger.Quest = triggerElement.GetAttribute("Quest");
                trigger.Event = triggerElement.GetAttribute("Event");
                trigger.TotalTimes = triggerElement.GetAttribute("Times", 1);
                trigger.SetParameters(triggerElement.GetAttribute("Parameters",""));
                trigger.Type = type;
                _triggers.Add(trigger);
            }
        }

        private void SaveTriggers(XmlWriter writer)
        {

            foreach (Trigger trigger in _triggers)
            {
                writer.WriteStartElement("Trigger");
                writer.WriteAttribute("Quest", trigger.Quest);
                writer.WriteAttribute("Event", trigger.Event);
                writer.WriteAttribute("Type", trigger.Type);
                writer.WriteAttribute("Times", trigger.TotalTimes);
                writer.WriteAttribute("Parameters", trigger.GetParameterString());
                writer.WriteString(trigger.ToXmlString());
                writer.WriteEndElement();
            }
        }

        private void LoadTile(XElement tileElement)
        {
            string tileName = (string)tileElement.Attribute("Name");
            string tileData = tileElement.Value;
            try
            {
                Tile tile = _resourceMgr.Tiles[tileName];

                List<int[]> coordinates = LoadTriplets(tileData);
            foreach (int[] coord in coordinates)
                _tiles[coord[1]][coord[0] * Layers + coord[2]] = tile;
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("Error: " + " tile \"" + tileName + "\" could not be found but was present in the map file. Map partially loaded. Saving will permanently remove this tile.");
            }
        }

        private List<int[]> LoadTriplets(string data)
        {
            data = data.Trim().TrimEnd(new[] { ';' });
            return (from x in data.Split(';')
                    select (
                           from w in x.Split(',')
                           select Int32.Parse(w.Trim())
                           ).ToArray()).ToList();
        }

        public void Save()
        {
            Save(Filename);
        }

        public void Save(string filename)
        {
            using (XmlWriter writer = getXmlWriter(filename))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Level");
                writer.WriteAttribute("FileFormatVersion", CUR_FILE_FORMAT_VERSION);
                writer.WriteAttribute("Width", Width);
                writer.WriteAttribute("Height", Height);
                writer.WriteAttribute("Layers", Layers);

                SaveTriggers(writer);
                SaveProperties(writer);
                foreach (Tileset tileset in _autoTilesets.Values)
                {
                    if (tileset.Tiles.Count == 0)
                        continue;
                    writer.WriteStartElement("Tileset");
                    writer.WriteAttribute("Filename", tileset.Filename);
                    foreach (TileData tile in tileset.Tiles)
                    {
                        writer.WriteStartElement("Tile");
                        writer.WriteAttribute("Name", tile.Name);
                        writer.WriteAttribute("OffsetX", tile.OffsetX);
                        writer.WriteAttribute("OffsetY", tile.OffsetY);
                        writer.WriteAttribute("Width", tile.Width);
                        writer.WriteAttribute("Height", tile.Height);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }

                WriteCollisionElement(writer);

                Dictionary<string, List<int[]>> tileData = new Dictionary<string, List<int[]>>();
                for (int y = 0; y < Height; y++)
                {
                    Tile[] row = _tiles[y];
                    for (int xw = 0; xw < Width * Layers; xw++)
                    {
                        int x = xw / Layers;
                        Tile tile = row[xw];
                        if (tile != null)
                        {
                            if(!tileData.ContainsKey(tile.Name))
                                tileData.Add(tile.Name, new List<int[]>());
                            tileData[tile.Name].Add(new int[] { x, y, xw % Layers });
                        }
                    }
                }

                foreach (var tile in tileData)
                {
                    writer.WriteStartElement("Tile");
                    writer.WriteAttribute("Name", tile.Key);
                    StringBuilder sb = new StringBuilder();
                    foreach(int[] coord in tile.Value)
                    {
                        sb.Append(coord[0] + "," + coord[1] + "," + coord[2] + ";");
                        
                    }
                    writer.WriteString(sb.ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

        }

        private void WriteCollisionElement(XmlWriter writer)
        {
            writer.WriteStartElement("Collision");
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    if(_collision[y][x] != 0)
                        sb.Append(x + "," + y + "," + _collision[y][x] + ";");
            writer.WriteString(sb.ToString());
            writer.WriteEndElement();

        }
    }
}
