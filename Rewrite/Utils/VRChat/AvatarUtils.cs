using Blaze.API;
using Blaze.Utils.Objects;
using System;
using System.Globalization;
using VRC.Core;
using VRC.UI;
using static Blaze.Utils.Objects.OtherObjects;
using static Blaze.Utils.Objects.VRChatObjects;

namespace Blaze.Utils.VRChat
{
    public static class AvatarUtils
    {
        public static void ChangeToAvatar(string AvatarID)
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

        public static void ChangeToAvatar(this ApiAvatar instance)
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

        public static void ReloadAvatar(this VRCPlayer instance)
        {
            BlazesXRefs.LoadAvatarMethod.Invoke(instance, new object[] { true }); // parameter is forceLoad and has to be true
        }

        public static void ReloadAllAvatars(this VRCPlayer instance, bool ignoreSelf = false)
        {
            BlazesXRefs.ReloadAllAvatarsMethod.Invoke(instance, new object[] { ignoreSelf });
        }

        public static ApiAvatar ToApiAvatar(this VRCAvatar a)
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

        public static ApiAvatar ToApiAvatar(this ModObjects.ModAviFav a)
        {
            return new ApiAvatar
            {
                name = a.Name,
                id = a.ID,
                thumbnailImageUrl = a.ThumbnailImageURL
            };
        }

        public static VRCAvatar ToVRCAvatar(this ApiAvatar a)
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

        public static VRCAvatar ToVRCAvatar(this ModObjects.ModAviFav a)
        {
            return new VRCAvatar
            {
                name = a.Name,
                id = a.ID,
                thumbnailImageUrl = a.ThumbnailImageURL
            };
        }

        public static ModObjects.ModAviFav ToModFavAvi(this LNCAvatar a)
        {
            return new ModObjects.ModAviFav
            {
                Name = a.name,
                ID = a.id,
                ThumbnailImageURL = a.thumbnailImageUrl
            };
        }

        public static ModObjects.ModAviFav ToModFavAvi(this ApiAvatar a)
        {
            return new ModObjects.ModAviFav
            {
                Name = a.name,
                ID = a.id,
                ThumbnailImageURL = a.thumbnailImageUrl
            };
        }
    }
}
