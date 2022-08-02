using Blaze.API;
using Blaze.Utils.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRC.Core;
using VRC.UI;
using static Blaze.Utils.Objects.OtherObjects;
using static Blaze.Utils.Objects.VRChatObjects;

namespace Blaze.Utils.VRChat
{
    internal static class AvatarUtils
    {
        internal static ApiAvatar ToApiAvatar(this VRCAvatar a)
        {
            return new ApiAvatar
            {
                id = a.id,
                name = a.name,
                description = a.description,
                authorId = a.authorId,
                authorName = a.authorName,
                assetUrl = a.assetUrl,
                imageUrl = a.imageUrl,
                thumbnailImageUrl = a.thumbnailImageUrl,
                releaseStatus = a.releaseStatus,
                version = a.version,
                featured = a.featured,
                created_at = Il2CppSystem.DateTime.Parse(a.created_at.ToString(CultureInfo.CurrentCulture)),
                updated_at = Il2CppSystem.DateTime.Parse(a.updated_at.ToString(CultureInfo.CurrentCulture))
            };
        }

        internal static ApiAvatar ToApiAvatar(this ModObjects.ModAviFav a)
        {
            return new ApiAvatar
            {
                name = a.Name,
                id = a.ID,
                thumbnailImageUrl = a.ThumbnailImageURL
            };
        }

        internal static VRCAvatar ToVRCAvatar(this ApiAvatar a)
        {
            return new VRCAvatar
            {
                id = a.id,
                name = a.name,
                description = a.description,
                authorId = a.authorId,
                authorName = a.authorName,
                assetUrl = a.assetUrl,
                imageUrl = a.imageUrl,
                thumbnailImageUrl = a.thumbnailImageUrl,
                releaseStatus = a.releaseStatus,
                version = a.version,
                featured = a.featured,
                created_at = DateTime.Parse(a.created_at.ToString()),
                updated_at = DateTime.Parse(a.updated_at.ToString())
            };
        }

        internal static VRCAvatar ToVRCAvatar(this ModObjects.ModAviFav a)
        {
            return new VRCAvatar
            {
                name = a.Name,
                id = a.ID,
                thumbnailImageUrl = a.ThumbnailImageURL
            };
        }

        internal static ModObjects.ModAviFav ToModFavAvi(this LNCAvatar a)
        {
            return new ModObjects.ModAviFav
            {
                Name = a.name,
                ID = a.id,
                ThumbnailImageURL = a.thumbnailImageUrl
            };
        }

        internal static ModObjects.ModAviFav ToModFavAvi(this ApiAvatar a)
        {
            return new ModObjects.ModAviFav
            {
                Name = a.name,
                ID = a.id,
                ThumbnailImageURL = a.thumbnailImageUrl
            };
        }

        internal static void ChangeToAvatar(string AvatarID)
        {
            new ApiAvatar { id = AvatarID }.Get(new Action<ApiContainer>(x =>
            {
                APIStuff.GetSocialMenuInstance().transform.Find("Avatar").GetComponent<PageAvatar>().field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0 = x.Model.Cast<ApiAvatar>();
                APIStuff.GetSocialMenuInstance().transform.Find("Avatar").GetComponent<PageAvatar>().ChangeToSelectedAvatar();
            }), new Action<ApiContainer>(x =>
            {
                Logs.Error($"Failed to change to avatar: {AvatarID} | Error Message: {x.Error}");
            }));
        }

        internal static void ChangeToAvatar(this ApiAvatar instance)
        {
            new ApiAvatar { id = instance.id }.Get(new Action<ApiContainer>(x =>
            {
                APIStuff.GetSocialMenuInstance().transform.Find("Avatar").GetComponent<PageAvatar>().field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0 = x.Model.Cast<ApiAvatar>();
                APIStuff.GetSocialMenuInstance().transform.Find("Avatar").GetComponent<PageAvatar>().ChangeToSelectedAvatar();
            }), new Action<ApiContainer>(x =>
            {
                Logs.Error($"Failed to change to avatar: {instance.id} | Error Message: {x.Error}");
            }));
        }

        /*internal static void ReloadAvatar(this VRCPlayer Instance)
        {
            VRCPlayer.Method_Public_Static_Void_APIUser_0(Instance.GetAPIUser());
        }

        internal static void ReloadAvatar(this VRC.Player Instance)
        {
            VRCPlayer.Method_Public_Static_Void_APIUser_0(Instance.GetAPIUser());
        }*/

        internal static void ReloadAvatar(this VRCPlayer instance)
        {
            BlazesXRefs.LoadAvatarMethod.Invoke(instance, new object[] { true }); // parameter is forceLoad and has to be true
        }

        internal static void ReloadAllAvatars(this VRCPlayer instance, bool ignoreSelf = false)
        {
            BlazesXRefs.ReloadAllAvatarsMethod.Invoke(instance, new object[] { ignoreSelf });
        }
    }
}
