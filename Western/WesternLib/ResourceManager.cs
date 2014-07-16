using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WesternLib
{

    /// <summary>
    /// Keeps textures loaded and keeps track of the GL texture ID's for them. Stores all tiles and sprites.
    /// </summary>
    public class ResourceManager
    {
        private static readonly string DEFAULT_TEXTURE = Path.Combine("graphics", "spritetest.png");

        private Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();
        private Dictionary<string, Tile> _tiles = new Dictionary<string, Tile>();
        private readonly Color TRANSPARENT_COLOR = Color.Magenta;
        private Dictionary<string, SpriteCollection> _sprites;
        private Dictionary<string, Tileset> _tilesets;


        public Dictionary<string, Tileset> Tilesets
        {
            get { return _tilesets; }
        }

        public Dictionary<string, Tile> Tiles
        {
            get { return _tiles; }
        }

        public Dictionary<string, SpriteCollection> Sprites
        {
            get { return _sprites; }
        }

        public Texture GetTexture(string filename)
        {
            if (!_textures.ContainsKey(filename))
                LoadTexture(filename);
            return _textures[filename];
        }

        public void Unload()
        {
            foreach (Texture texture in _textures.Values)
            {
                DeleteTexture(texture);
            }
        }

        private void UnloadTile(string name)
        {
            if(!_tiles.ContainsKey(name))
            {
                //TODO: warning
                return;
            }
            Tile tile = _tiles[name];
            _tiles.Remove(name);
        }

        private void DeleteTexture(Texture texture)
        {
            int idCopy = texture.TextureId;
            GL.DeleteTextures(1, ref idCopy);
        }

        public Tile GetAutoTile(Tileset tileset, int offsetX, int offsetY, int width, int height)
        {
            string name = String.Format("{4}{0:0000}{1:0000}{2:0000}{3:0000}", offsetX, offsetY, width, height, tileset.Filename);
            Tile tile;
            if (Tiles.ContainsKey(name))
                tile = Tiles[name];
            else
            {
                Texture texture = GetTexture(tileset.Filename);
                TileData tileData = new TileData(name, offsetX, offsetY, width, height);
                tile = new Tile(texture, tileData);
                tileset.Tiles.Add(tileData);
                Tiles.Add(name, tile);
            }
            return tile;
        }

        public void UnloadTilesets(Dictionary<string, Tileset> tilesets)
        {
            foreach (Tileset tileset in tilesets.Values)
            {
                foreach (TileData tileData in tileset.Tiles)
                {
                    UnloadTile(tileData.Name);
                }
                Texture texture = _textures[tileset.Filename];
                DeleteTexture(texture);
                _textures.Remove(tileset.Filename);

            }
        }



        public Tile GetFullTile(Tileset tileset)
        {
            Texture texture = GetTexture(tileset.Filename);
            return GetAutoTile(tileset, 0, 0, texture.Width, texture.Height);
        }

        internal void Load(Dictionary<string, Tileset> tilesets)
        {
            foreach (Tileset tileset in tilesets.Values)
            {
                Texture texture = GetTexture(tileset.Filename);
                foreach (TileData tileData in tileset.Tiles)
                {
                    Tile tile = new Tile(texture, tileData);
                    try
                    {
                        _tiles.Add(tileData.Name, tile);
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("Tile by the name " + tile.Name + " already exists. Duplicate tile was not added.");
                    }

                }

            }
        }

        // This code could be moved to the Texture class, probably
        private Texture LoadTexture(string filename)
        {
            string key = filename;
            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists)
            {
                filename = DEFAULT_TEXTURE;
            }
            try
            {
                Bitmap bitmap = new Bitmap(filename);

                bitmap.MakeTransparent(TRANSPARENT_COLOR);
                GL.Enable(EnableCap.Texture2D);

                int textureId = GL.GenTexture();
                //GL.GenTextures(1, out textureId);
                GL.BindTexture(TextureTarget.Texture2D, textureId);

                Texture texture = new Texture(textureId, bitmap.Width, bitmap.Height,filename);

                BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);


                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                bitmap.UnlockBits(data);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                _textures.Add(key, texture);

                return texture;
            }
            catch (Exception ex)
            {
                Console.WriteLine("File " + filename + " could not be loaded");
                Console.ReadKey();
                throw ex;
            }
        }

        internal void LoadResources()
        {
            _tilesets = TileLoader.Load();
            
            Load(_tilesets);
            _sprites = SpriteLoader.Load(this);

        }
    }
}
