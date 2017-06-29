using System.Collections.Generic;
using System.IO;
using Avatars.TileEngine;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Avatars.MapComponents
{
    public static class MapManager
    {
        #region Field Region

        private static Dictionary<string, TileMap> mapList = new Dictionary<string, TileMap>();

        #endregion

        #region Property Region

        public static Dictionary<string, TileMap> MapList
        {
            get { return mapList; }
        }

        #endregion

        #region Constructor Region

        #endregion

        #region Method Region

        public static void AddMap(string name, TileMap avatar)
        {
            if (!mapList.ContainsKey(name.ToLowerInvariant()))
                mapList.Add(name.ToLowerInvariant(), avatar);
        }

        public static TileMap GetMap(string name)
        {
            if (mapList.ContainsKey(name.ToLowerInvariant()))
                return mapList[name.ToLowerInvariant()];

            return null;
        }

        /*public static void FromCSVFile(string fileName, ContentManager content)
        {
            using (Stream stream = new FileStream(@".\Data\" + fileName + ".csv", FileMode.Open, FileAccess.Read))
            {
                try
                {
                    using (TextReader reader = new StreamReader(stream))
                    {
                        try
                        {
                            string lineIn = "";

                            do
                            {
                                lineIn = reader.ReadLine();
                                if (lineIn != null)
                                {
                                    string[] parts = lineIn.Split(',');
                                    TileMap map = null;
                                    Texture2D tiles = content.Load<Texture2D>(@"Tiles\" + parts[0]);

                                    lineIn = reader.ReadLine();
                                    parts = lineIn.Split(',');
                                    TileSet set = new TileSet(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]));
                                    set.Texture = tiles;

                                    TileLayer background = new TileLayer(int.Parse(parts[4]), int.Parse(parts[5]));
                                    TileLayer edge = new TileLayer(int.Parse(parts[4]), int.Parse(parts[5]));
                                    TileLayer building = new TileLayer(int.Parse(parts[4]), int.Parse(parts[5]));
                                    TileLayer decor = new TileLayer(int.Parse(parts[4]), int.Parse(parts[5]));

                                    map = new TileMap(set, background, edge, building, decor, fileName);

                                    map.FillEdges();
                                    map.FillBuilding();
                                    map.FillDecoration();


                                    int h = int.Parse(parts[5]);

                                    for (int j = 0; j < h; j++)
                                    {
                                        lineIn = reader.ReadLine();
                                        parts = lineIn.Split(',');
                                        for (int i = 0; i < parts.Length; i++)
                                        {
                                            background.SetTile(i, j, int.Parse(parts[i]));
                                        }
                                    }

                                    if (!mapList.ContainsKey(map.MapName.ToLowerInvariant()))
                                        mapList.Add(map.MapName.ToLowerInvariant(), map);
                                }
                            } while (lineIn != null);
                        }
                        catch
                        {
                        }
                        finally
                        {
                            if (reader != null)
                                reader.Close();
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }
        }*/

        public static void FromBinFile(string fileName, ContentManager content)
        {
            using (Stream stream = new FileStream(@".\Data\" + fileName + ".bin", FileMode.Open, FileAccess.Read))
            {
                try
                {
                    using (BinaryReader reader = new BinaryReader(stream)) // expects .bin file
                    {
                        try
                        {
                            int length = (int)reader.BaseStream.Length;

                            if (length > 0)
                            {
                                string tilesetName = reader.ReadString();
                                Texture2D tiles = content.Load<Texture2D>(@"Tiles\" + tilesetName);

                                TileSet set = new TileSet(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                                set.TextureName = tilesetName;
                                set.Texture = tiles;

                                int w = 0;
                                int h = 0;
                                TileLayer background = new TileLayer(w = reader.ReadInt32(), h = reader.ReadInt32());
                                TileLayer edge = new TileLayer(w, h);
                                TileLayer buildings = new TileLayer(w, h);
                                TileLayer decorations = new TileLayer(w, h);

                                TileMap map = new TileMap(set, background, edge, buildings, decorations, fileName);
                                map.FillEdges();
                                map.FillBuilding();
                                map.FillDecoration();

                                for (int j = 0; j < h; j++)
                                {
                                    for (int i = 0; i < w; i++)
                                    {
                                        map.SetGroundTile(i, j, reader.ReadInt32());
                                        map.SetEdgeTile(i, j, reader.ReadInt32());
                                        map.SetBuildingTile(i, j, reader.ReadInt32());
                                        map.SetDecorationTile(i, j, reader.ReadInt32());
                                    }
                                }

                                if (!mapList.ContainsKey(map.MapName.ToLowerInvariant()))
                                    mapList.Add(map.MapName.ToLowerInvariant(), map);
                            }
                        }
                        finally
                        {
                            if (reader != null) reader.Close();
                        }
                    }
                }
                finally
                {
                    if (stream != null) stream.Close();
                }
            }
        }
        #endregion
    }
}
