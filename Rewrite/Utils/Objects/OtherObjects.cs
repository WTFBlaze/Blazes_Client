using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaze.Utils.Objects
{
    public class OtherObjects
    {
        public class LNCAviFav
        {
            public List<LNCAvatar> Avis { get; set; }
        }

        public class LNCAvatar
        {
            public string name { get; set; }
            public string id { get; set; }
            public string authorId { get; set; }
            public string authorName { get; set; }
            public string assetUrl { get; set; }
            public string thumbnailImageUrl { get; set; }
            public string supportedPlatforms { get; set; }
            public string releaseStatus { get; set; }
            public string description { get; set; }
            public int version { get; set; }
        }

        public class ApolloAviFav
        {
            public ModObjects.ModAviFavorites AvatarFavorites = new();
        }
    }
}
