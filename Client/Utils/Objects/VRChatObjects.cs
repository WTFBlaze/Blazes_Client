using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaze.Utils.Objects
{
    internal class VRChatObjects
    {
        public class VRCAvatar
        {
            public string id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public bool featured { get; set; }
            public string authorId { get; set; }
            public string authorName { get; set; }
            public string[] tags { get; set; }
            public string releaseStatus { get; set; }
            public string imageUrl { get; set; }
            public string thumbnailImageUrl { get; set; }
            public string supportedPlatforms { get; set; }
            public string assetUrl { get; set; }
            public string assetUrlObject { get; set; }
            public string pluginUrlObject { get; set; }
            public string unityPackageUrlObject { get; set; }
            public VRCUnityPackage[] unityPackages { get; set; }
            public int version { get; set; }
            public string organization { get; set; }
            public string previewYoutubeId { get; set; }
            public int favorites { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
        }

        public class VRCWorld
        {
            public string id { get; set; }
            public string name { get; set; }
            public string authorId { get; set; }
            public string authorName { get; set; }
            public int capacity { get; set; }
            public string imageUrl { get; set; }
            public string thumbnailImageUrl { get; set; }
            public string releaseStatus { get; set; }
            public string organization { get; set; }
            public string[] tags { get; set; }
            public int favorites { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public DateTime publicationDate { get; set; }
            public DateTime labsPublicationDate { get; set; }
            //public VRCUnityPackage[] unityPackages { get; set; }
            public int popularity { get; set; }
            public int heat { get; set; }
            public int occupants { get; set; }
        }

        public class VRCInstance
        {
            public string id { get; set; }
            public string location { get; set; }
            public string instanceId { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string worldId { get; set; }
            public string[] tags { get; set; }
            public bool active { get; set; }
            public bool full { get; set; }
            public int n_users { get; set; }
            public int capacity { get; set; }
            public VRCPlatformsCount platforms { get; set; }
            public string shortname { get; set; }
            public string clientNumber { get; set; }
            public string photonRegion { get; set; }
            public string region { get; set; }
            public bool canRequestInvite { get; set; }
            public bool permanent { get; set; }
            public string hidden { get; set; }
        }

        public class VRCPlatformsCount
        {
            public int standalonewindows { get; set; }
            public int android { get; set; }
        }

        public class VRCUser
        {
            public string id { get; set; }
            public string username { get; set; }
            public string displayName { get; set; }
            public string userIcon { get; set; }
            public string bio { get; set; }
            public string[] bioLinks { get; set; }
            public string profilePicOverride { get; set; }
            public string statusDescription { get; set; }
            public string currentAvatarImageUrl { get; set; }
            public string currentAvatarThumbnailImageUrl { get; set; }
            public string fallbackAvatar { get; set; }
            public string state { get; set; }
            public string[] tags { get; set; }
            public string developerType { get; set; }
            public DateTime last_login { get; set; }
            public string last_platform { get; set; }
            public bool allowAvatarCopying { get; set; }
            public string status { get; set; }
            public DateTime date_joined { get; set; }
            public string friendKey { get; set; }
            public string worldId { get; set; }
            public string instanceId { get; set; }
            public string location { get; set; }
        }

        public class VRCUnityPackage
        {
            public string id { get; set; }
            public DateTime created_at { get; set; }
            public string assetUrl { get; set; }
            public string impostorUrl { get; set; }
            public string unityVersion { get; set; }
            public int unitySortNumber { get; set; }
            public int assetVersion { get; set; }
            public string platform { get; set; }
        }

        public class VRCNotification
        {
            public string id { get; set; }
            public string senderUserId { get; set; }
            public string senderUsername { get; set; }
            public string type { get; set; }
            public string message { get; set; }
            public string details { get; set; }
            public bool seen { get; set; }
            public DateTime created_at { get; set; }
        }

        public class VRCPlayerModeration
        {
            public string id { get; set; }
            public string type { get; set; }
            public string sourceUserId { get; set; }
            public string sourceDisplayName { get; set; }
            public string targetUserId { get; set; }
            public string targetDisplayName { get; set; }
            public DateTime created { get; set; }
        }

        public class VRCFetchInstanceResponse
        {
            public string id { get; set; }
            public string location { get; set; }
            public string instanceId { get; set; }
            public string name { get; set; }
            public string worldId { get; set; }
            public string type { get; set; }
            public string ownerId { get; set; }
            public string[] tags { get; set; }
            public bool active { get; set; }
            public int n_users { get; set; }
            public int capacity { get; set; }
            public VRCPlatformsCount platforms { get; set; }
            public string shortName { get; set; }
            public string clientNumber { get; set; }
            public string photonRegion { get; set; }
            public string region { get; set; }
            public bool canRequestInvite { get; set; }
            public bool permanent { get; set; }
        }

        public class VRCConfig
        {
            public int ps_max_particles { get; set; }
            public int ps_max_systems { get; set; }
            public int ps_max_emission { get; set; }
            public int ps_mesh_particle_divider { get; set; }
            public int ps_mesh_particle_poly_limit { get; set; }
            public int ps_collision_penalty_high { get; set; }
            public int ps_collision_penalty_med { get; set; }
            public int ps_collision_penalty_low { get; set; }
            public int ps_trails_penalty { get; set; }
            public int dynamic_bone_max_affected_transform_count { get; set; }
            public int dynamic_bone_max_collider_check_count { get; set; }
            public bool disableRichPresence { get; set; }
        }
    }
}
