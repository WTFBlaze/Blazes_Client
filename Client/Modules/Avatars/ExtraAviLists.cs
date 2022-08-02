using Blaze.API;
using Blaze.API.SM;
using Blaze.Utils.Attributes;
using Blaze.Utils.Managers;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VRC.Core;

namespace Blaze.Modules
{
    class ExtraAviLists : BModule
    {
        private static GameObject avatarPage;
        internal static SMList RecentlyUsedAvatars;
        internal static SMList RecentlySeenAvatars;
        private static List<ApiAvatar> usedAvis = new();
        private static List<ApiAvatar> seenAvis = new();
        private static UiAvatarList selfUploads;
        private static Text selfUploadsText;

        public override void QuickMenuUI()
        {
            avatarPage = GameObject.Find("UserInterface/MenuContent/Screens/Avatar");
            RecentlyUsedAvatars = new SMList(APIStuff.GetSocialMenuInstance().transform.Find("Avatar/Vertical Scroll View/Viewport/Content"), "<color=yellow>Recently Used Avis</color>");
            RecentlyUsedAvatars.GetText().supportRichText = true;
            RecentlySeenAvatars = new SMList(APIStuff.GetSocialMenuInstance().transform.Find("Avatar/Vertical Scroll View/Viewport/Content"), "<color=magenta>Recently Seen Avis</color>");
            RecentlySeenAvatars.GetText().supportRichText = true;

            var listener = avatarPage.GetComponent<EnableDisableListener>();
            listener.OnEnabled += () =>
            {
                MelonCoroutines.Start(RefreshMenu(1.5f));
            };
        }

        private static IEnumerator RefreshMenu(float v)
        {
            RecentlyUsedAvatars.RenderElement(new Il2CppSystem.Collections.Generic.List<ApiAvatar>());
            RecentlySeenAvatars.RenderElement(new Il2CppSystem.Collections.Generic.List<ApiAvatar>());
            yield return new WaitForSeconds(v);
            Il2CppSystem.Collections.Generic.List<ApiAvatar> tmpList = new();
            foreach (var a in usedAvis)
            {
                tmpList.Add(a);
            }
            foreach (var a in tmpList)
            {
                if (a.releaseStatus == "private")
                {
                    if (!a.name.StartsWith("<color=red>[P]</color>"))
                    {
                        a.name = $"<color=red>[P]</color> {a.name}";
                    }
                }
            }
            tmpList.Reverse();
            RecentlyUsedAvatars.RenderElement(tmpList);
            List<string> tmpPickers = new();
            foreach (var p in RecentlyUsedAvatars.GetUiVRCList().pickers)
            {
                if (!tmpPickers.Contains(p.field_Public_String_0))
                {
                    tmpPickers.Add(p.field_Public_String_0);
                }
                else
                {
                    p.gameObject.SetActive(false);
                }
            }
            tmpList.Clear();

            Il2CppSystem.Collections.Generic.List<ApiAvatar> tmpList2 = new();
            foreach (var a in seenAvis)
            {
                tmpList2.Add(a);
            }
            foreach (var a in tmpList2)
            {
                if (a.releaseStatus == "private")
                {
                    if (!a.name.StartsWith("<color=red>[P]</color>"))
                    {
                        a.name = $"<color=red>[P]</color> {a.name}";
                    }
                }
            }
            tmpList2.Reverse();
            RecentlySeenAvatars.RenderElement(tmpList2);
            List<string> tmpPickers2 = new();
            foreach (var p in RecentlySeenAvatars.GetUiVRCList().pickers)
            {
                if (!tmpPickers2.Contains(p.field_Public_String_0))
                {
                    tmpPickers2.Add(p.field_Public_String_0);
                }
                else
                {
                    p.gameObject.SetActive(false);
                }
            }
            tmpList2.Clear();

            int uploadsCount = 0;
            foreach (var p in GetSelfList().pickers)
            {
                if (p.gameObject.active)
                {
                    uploadsCount++;
                }
            }
            GetSelfText().supportRichText = true;
            GetSelfText().text = $"<color=yellow>My Uploads</color> <color=white>[</color><color={BlazeInfo.ModColor1}>{uploadsCount}</color><color=white>]</color>";
        }

        private static UiAvatarList GetSelfList()
        {
            if (selfUploads == null)
            {
                selfUploads = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Vertical Scroll View/Viewport/Content/Personal Avatar List").GetComponent<UiAvatarList>();
            }
            return selfUploads;
        }

        private static Text GetSelfText()
        {
            if (selfUploadsText == null)
            {
                selfUploadsText = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Vertical Scroll View/Viewport/Content/Personal Avatar List/Button/TitleText").GetComponent<Text>();
            }
            return selfUploadsText;
        }

        public static void AddToRecentlyUsed(ApiAvatar avi)
        {
            if (usedAvis.Contains(avi))
            {
                usedAvis.MoveItemAtIndexToFront(usedAvis.IndexOf(avi));
            }
            else
            {
                usedAvis.Add(avi);
            }
        }

        public static void AddToRecentlySeen(ApiAvatar avi)
        {
            if (seenAvis.Contains(avi))
            {
                seenAvis.MoveItemAtIndexToFront(seenAvis.IndexOf(avi));
            }
            else
            {
                seenAvis.Add(avi);
            }
        }
    }
}
