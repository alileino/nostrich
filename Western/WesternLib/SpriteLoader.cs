using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WesternLib
{
    public class SpriteCollection
    {
        private Sprite _default;

        public string Name { get; set; }
        public string Filename { get; set; }
        public int SpriteWidth { get; set; }
        public int SpriteHeight { get; set; }
        public Dictionary<string, Sprite> Sprites { get; internal set; }

        public Sprite this[string name]
        {
            get 
            {
                if (name == "default")
                {
                    if(_default == null)
                        InitializeDefaultSprite();
                    return _default;
                }
                return Sprites[name]; 
            }
            set { Sprites[name] = value; }
        }

        private void InitializeDefaultSprite()
        {
            _default = Sprites.Values.First<Sprite>();
        }

        public SpriteCollection Copy()
        {
            SpriteCollection col = new SpriteCollection();
            col._default = _default;
            col.Name = Name;
            col.Filename = Filename;
            col.SpriteWidth = SpriteWidth;
            col.SpriteHeight = SpriteHeight;
            col.Sprites = new Dictionary<string, Sprite>();
            foreach (var spr in Sprites)
            {
                col.Sprites.Add(spr.Key, spr.Value.Copy());
            }
            return col;
        }
    }

    public class SpriteLoader : XmlHelper
    {
        private static readonly string FILENAME = "data/sprites.xml";

        public static Dictionary<string,SpriteCollection> Load(ResourceManager resourceManager)
        {
            var spriteCollections = new Dictionary<string,SpriteCollection>();
            XDocument doc = XDocument.Load(FILENAME);
            
          

            foreach (XElement spriteCollectionElement in doc.Root.Elements("SpriteCollection"))
            {
                string filename = spriteCollectionElement.GetAttribute("Filename");
                Texture texture = resourceManager.GetTexture(filename);
                int defaultWidth = spriteCollectionElement.GetAttribute("DefaultSpriteWidth", 32);
                int defaultHeight = spriteCollectionElement.GetAttribute("DefaultSpriteHeight", 32);
                int defaultOriginX = spriteCollectionElement.GetAttribute("OriginX", -1);
                int defaultOriginY = spriteCollectionElement.GetAttribute("OriginY", -1);
                var spriteCollection = new SpriteCollection()
                {
                    Filename = filename,
                    Name = spriteCollectionElement.GetAttribute("Name"),
                    SpriteWidth = defaultWidth,
                    SpriteHeight = defaultHeight,
                    Sprites = (from x in spriteCollectionElement.Elements("Sprite") select x).ToDictionary(x=> x.GetAttribute("Name"),x=>CreateSprite(x, texture, defaultWidth, defaultHeight, defaultOriginX, defaultOriginY))
                };
                spriteCollections.Add(spriteCollection.Name, spriteCollection);
            }


            return spriteCollections;
        }
        private static Sprite CreateSprite(XElement spriteElement, Texture texture, int defaultWidth, int defaultHeight, int defaultOriginX, int defaultOriginY)
        {
            TileData tileData = new TileData()
            {
                Name = spriteElement.GetAttribute("Name"), 
                OffsetX = spriteElement.GetAttribute("OffsetX",0), 
                OffsetY = spriteElement.GetAttribute("OffsetY",0),
                Width = spriteElement.GetAttribute("Width", defaultWidth),
                Height = spriteElement.GetAttribute("Height", defaultHeight),
                AnimNum = spriteElement.GetAttribute("AnimNum", 1),
                AnimSpeed = spriteElement.GetAttribute("AnimSpeed",1.0f)
            };
            defaultOriginX = defaultOriginX == -1 ? tileData.Width / 2 : defaultOriginX;
            defaultOriginY = defaultOriginY == -1 ? tileData.Height / 2 : defaultOriginY;
            return new Sprite(
                texture,
                tileData,
                spriteElement.GetAttribute("OriginX", tileData.Width/2),
                spriteElement.GetAttribute("OriginY", tileData.Height/2));
        }

    }

    
}
