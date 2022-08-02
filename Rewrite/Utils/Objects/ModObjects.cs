using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using VRC.Core;

namespace Blaze.Utils.Objects
{
    public class ModObjects
    {
        public class ModUser
        {
            [JsonProperty("_id")]
            public string Key { get; set; }
            public string HWID { get; set; }
            public string DiscordName { get; set; }
            public ulong DiscordID { get; set; }
            public string UserHash { get; set; }
            public bool IsBanned { get; set; }
            public string BanReason { get; set; }
            public string AccessType { get; set; }
            public ModAviFavorites Favs { get; set; }
        }

        public class ModSocketUser
        {
            public string WebSocketID { get; set; }
            public string DiscordName { get; set; }
            public ulong DiscordID { get; set; }
            public string AccessType { get; set; }
            public string UserHash { get; set; }
            public string UserID { get; set; }
            public string WorldID { get; set; }
        }

        public class ColorJson
        {
            public float R { get; set; } = 0;
            public float G { get; set; } = 0;
            public float B { get; set; } = 0;
            public float A { get; set; } = 0;

            public ColorJson(Color color)
            {
                R = color.r;
                G = color.g;
                B = color.b;
                A = color.a;
            }

            public Color GetColor()
            {
                return new Color(R, G, B, A);
            }
        }

        public class ModBlazeTag
        {
            public string UserID { get; set; }
            public string AccessType { get; set; }
        }

        public class ModAviFavorites
        {
            public class FavoriteList
            {
                public int ID { get; set; }
                public string name { get; set; }
                public string Desciption { get; set; }
                public int Rows { get; set; } = 2;
                public List<ModAviFav> Avatars { get; set; }
            }
            public List<FavoriteList> FavoriteLists = new();
        }

        public class ModAviFav
        {
            public string Name { get; set; }
            public string ID { get; set; }
            public string ThumbnailImageURL { get; set; }
        }

        public class ModLocalBlock
        {
            public string DisplayName { get; set; }
            public string UserID { get; set; }
            public DateTime BlockDate { get; set; }
        }

        public class ModInstanceHistory
        {
            public string Name { get; set; }
            public string JoinID { get; set; }
            public InstanceAccessType Type { get; set; }
        }

        public class ModEventLocker
        {
            public string DisplayName { get; set; }
            public string UserID { get; set; }
            public List<byte> BlockedEvents { get; set; }
        }

        public class ModTag
        {
            public string userid { get; set; }
            public List<string> tags { get; set; }
        }

        public class ModPortal
        {
            public string DisplayName { get; set; }
            public string UserID { get; set; }
            public string ObjectID { get; set; }
        }

        public class ModKeybind
        {
            public ModFeature Target { get; set; }
            public KeyCode FirstKey { get; set; }
            public KeyCode SecondKey { get; set; }
            public bool MultipleKeys;

            public ModKeybind(ModFeature feature, KeyCode first, KeyCode second, bool multiple)
            {
                Target = feature;
                FirstKey = first;
                SecondKey = second;
                MultipleKeys = multiple;
            }
        }

        public enum ModFeature
        {
            Flight,
            ESP,
        }
    }
}
