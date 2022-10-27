using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace Azzandra
{
    public static class Assets
    {
        public static SpriteFont Medifont, Gridfont;
        public static Texture2D Rectangle;

        public static Texture2D TileBasic, TileBasicCorner, TilePillar;
        public static Texture2D FloorTextureSparse, FloorTextureDense, PlanksTexture;

        public static Texture2D Hit;
        public static Texture2D Arrow;

        public static Texture2D Target12, Target16, Target20, Target24, Target32, Target48;

        public static Texture2D MenuArch, CoALogo;
        public static Texture2D[] MenuBackgrounds;

        // Game Animation
        //private static Dictionary<string, Animation> Animations;
        //public static Animation GetAnimation(string assetID)
        //{
        //    if (assetID != null)
        //    {
        //        Animations.TryGetValue(assetID, out var anim);
        //        if (anim != null) return anim;
        //    }

        //    return new Animation(UnknownSprite);
        //}


        //Game Sprites
        private static Dictionary<string, Texture2D> Sprites;
        private static Dictionary<string, Texture2D> EntitySprites;
        public static Texture2D UnknownSprite;
        public static Texture2D GetSprite(string assetID)
        {
            if (assetID == null) return UnknownSprite;
            
            Sprites.TryGetValue(assetID, out var img);
            if (img != null) return img;
            
            EntitySprites.TryGetValue(assetID, out img);
            if (img != null) return img;

            return UnknownSprite;
        }

        ////items
        //private static Dictionary<string, Texture2D> ItemSprites;
        //public static Texture2D UnknownItem;

        //public static Texture2D GetSprite(string assetID)
        //{
        //    if (assetID == null) return UnknownItem;
        //    ItemSprites.TryGetValue(assetID, out var img);
        //    return img ?? UnknownItem;
        //}

        //eq slots
        public static Texture2D[] EquipmentIcons;
        

        public static void LoadAssets(ContentManager content, GraphicsDevice graphicsDevice)
        {
            Rectangle = new Texture2D(graphicsDevice, 1, 1);
            Rectangle.SetData(new[] { Color.White });

            Medifont = content.Load<SpriteFont>("fonts/medifont");
            Gridfont = content.Load<SpriteFont>("fonts/dungeonscript");

            TileBasic = content.Load<Texture2D>("textures/empty_tile");
            //TileBasicCorner = content.Load<Texture2D>("textures/empty_corner");
            //TilePillar = content.Load<Texture2D>("textures/pillar");
            //FloorTextureSparse = content.Load<Texture2D>("textures/floor_texture_sparse");
            //FloorTextureDense = content.Load<Texture2D>("textures/floor_texture_dense");
            //PlanksTexture = content.Load<Texture2D>("textures/planks_texture");


            Hit = content.Load<Texture2D>("interface/hit");
            Arrow = content.Load<Texture2D>("textures/arrow");

            Target12 = content.Load<Texture2D>("interface/target12");
            Target16 = content.Load<Texture2D>("interface/target16");
            Target20 = content.Load<Texture2D>("interface/target20");
            Target24 = content.Load<Texture2D>("interface/target24");
            Target32 = content.Load<Texture2D>("interface/target32");
            Target48 = content.Load<Texture2D>("interface/target48");

            MenuArch = content.Load<Texture2D>("interface/menu_arch");
            CoALogo = content.Load<Texture2D>("interface/logo");
            LoadMenuBackgrounds(content);


            Sprites = LoadAll(content, "Content\\textures");
            EntitySprites = LoadAll(content, "Content\\textures\\entity");

            UnknownSprite = Sprites["unknown"];
            //LoadItemSprites(content);
            LoadEquipmentIcons(content);
        }

        public static Texture2D GetTargetSprite(int size)
        {
            switch (size)
            {
                case 12: return Target12;
                case 16: default: return Target16;
                case 20: return Target20;
                case 24: return Target24;
                case 32: return Target32;
                case 48: return Target48;
            }
        }

        private static void LoadMenuBackgrounds(ContentManager content)
        {
            string root = "interface\\menu_backgrounds";
            int amt = 5;

            MenuBackgrounds = new Texture2D[amt];

            for (int i = 0; i < amt; i++)
            {
                MenuBackgrounds[i] = content.Load<Texture2D>(root + "\\" + i);
            }
        }

        public static Texture2D GetRandomMenuBackground(Texture2D current)
        {
            return MenuBackgrounds[Util.Random.Next(MenuBackgrounds.Length)];
        }

        private static Dictionary<string, Texture2D> LoadAll(ContentManager content, string root)
        {
            string[] files = Directory.GetFiles(root);
            var images = new Dictionary<string, Texture2D>(files.Length);

            int rootLength = root.Length + 1;
            content.RootDirectory = root.Replace('\\', '/');

            for (int i = 0; i < files.Length; i++)
            {
                var fileName = files[i].Remove(0, rootLength);
                fileName = fileName.Remove(fileName.Length - 4, 4);
                images.Add(fileName, content.Load<Texture2D>(fileName));
            }

            return images;
        }

        //private static void LoadItemSprites(ContentManager content)
        //{
        //    string root = "Content\\items";
        //    string[] files = Directory.GetFiles(root);
        //    ItemSprites = new Dictionary<string, Texture2D>(files.Length);

        //    int rootLength = "Content\\items\\".Length;
        //    content.RootDirectory = "Content/items";

        //    for (int i = 0; i < files.Length; i++)
        //    {
        //        var fileName = files[i].Remove(0, rootLength);
        //        fileName = fileName.Remove(fileName.Length - 4, 4);
        //        ItemSprites.Add(fileName, content.Load<Texture2D>(fileName));
        //    }

        //    UnknownItem = content.Load<Texture2D>("unknown");
        //}

        private static void LoadEquipmentIcons(ContentManager content)
        {
            int amtOfSlots = 12;
            EquipmentIcons = new Texture2D[amtOfSlots];
            content.RootDirectory = "Content/eqslots";

            for (int i = 0; i < amtOfSlots; i++)
            {
                EquipmentIcons[i] = content.Load<Texture2D>("eqslot" + i);
            }
        }
    }
}
