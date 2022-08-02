using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRC.Core;
using static Blaze.Utils.Objects.ModObjects;

namespace Blaze.Utils.Objects
{
    internal class NetworkObjects
    {
        internal enum PayloadType
        {
            FetchedTags,
            SearchResult,
            AvatarResponse,
            MessageAll,
            FoundBlazeUsers,
            ReceivedUserInfo
        }

        internal class PayloadPart1
        {
            public PayloadPart2 payload { get; set; }
        }

        internal class PayloadPart2
        {
            public PayloadType type { get; set; }
            public object data { get; set; }
        }

        internal class FetchTagsResults 
        {
            public List<ModTag> tags { get; set; }
        }

        internal class AviSearchResults
        {
            public List<BlazeAvatar> results { get; set; }
        }

        internal class FindUsersResults
        {
            public List<ModBlazeTag> Users { get; set; }
        }

        internal class MessageAllResults
        {
            public string devname { get; set; }
            public string message { get; set; }
            public string message_type { get; set; }
        }

        public class BlazeAvatar
        {
            public string _id { get; set; }
            public string AssetURL { get; set; }
            public string AuthorID { get; set; }
            public string AuthorName { get; set; }
            public string AvatarName { get; set; }
            public string Description { get; set; }
            public bool Featured { get; set; }
            public string ImageURL { get; set; }
            //public string SupportedPlatforms { get; set; }
            public string ReleaseStatus { get; set; }
            public string ThumbnailImageURL { get; set; }
            public int Version { get; set; }
            public ApiAvatar ToApiAvatar()
            {
                return new ApiAvatar
                {
                    name = AvatarName,
                    id = _id,
                    authorId = AuthorID,
                    authorName = AuthorName,
                    assetUrl = AssetURL,
                    thumbnailImageUrl = ThumbnailImageURL,
                    supportedPlatforms = ApiModel.SupportedPlatforms.StandaloneWindows,
                    description = Description,
                    releaseStatus = ReleaseStatus,
                    version = Version,
                    //unityVersion = unityVersion,
                    //assetVersion = new AssetVersion(unityVersion, 0),
                };
            }
        }
    }
}
