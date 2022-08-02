using System.Collections.Generic;
using VRC.Core;
using static Blaze.Utils.Objects.ModObjects;

namespace Blaze.Utils.Objects
{
    public class NetworkObjects
    {
        public enum PayloadType
        {
            FetchedTags,
            SearchResult,
            AvatarResponse,
            MessageAll,
            FoundBlazeUsers,
            ReceivedUserInfo,
            OnUserOnline,
            OnUserAttemptBanned,
            OnUserAttemptDuplicate,
            OnUserAttemptUnAuthorised,
            OnUserAttemptIncorrectHash,
            OnUserOffline,
            SendOnline
        }

        public class PayloadPart1
        {
            public PayloadPart2 payload { get; set; }
        }

        public class PayloadPart2
        {
            public PayloadType type { get; set; }
            public object data { get; set; }
        }

        public class FetchTagsResults
        {
            public List<ModTag> tags { get; set; }
        }

        public class AviSearchResults
        {
            public List<BlazeAvatar> results { get; set; }
        }

        public class FindUsersResults
        {
            public List<ModBlazeTag> Users { get; set; }
        }

        public class SendOnlineResults
        {
            public Dictionary<string, ModSocketUser> data { get; set; }
        }

        public class OnUserState
        {
            public string name { get; set; }
            public string level { get; set; }
        }

        public class MessageAllResults
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
