using Blaze.API;
using Blaze.API.SM;
using Blaze.Utils;
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
using VRC.Core;
using static Blaze.Utils.Objects.NetworkObjects;

namespace Blaze.Modules
{
    public class AvatarSearch : BModule
    {
        public static List<ApiAvatar> avatars = new();
        public static List<List<ApiAvatar>> SortedLists = new();
        public static SMList list;
        public static SMButton backPage;
        public static SMButton nextPage;
        public static SMText currentLabel;
        public static int currentPage;

        public override void UI()
        {
            var avatarPage = GameObject.Find("UserInterface/MenuContent/Screens/Avatar");
            list = new SMList(APIStuff.GetSocialMenuInstance().transform.Find("Avatar/Vertical Scroll View/Viewport/Content"), "Blaze's Search Results");
            list.GetText().supportRichText = true;
            list.GetUiVRCList().hideWhenEmpty = true;
            list.GetUiVRCList().clearUnseenListOnCollapse = false;
            list.GetGameObject().GetComponent<UiAvatarList>().clearUnseenListOnCollapse = false;

            backPage = new SMButton(SMButton.SMButtonType.ChangeAvatar, list.GetUiVRCList().expandButton.gameObject.transform, 650, 0, "<-", delegate
            {
                if (currentPage == 0) return;
                currentPage--;
                currentLabel.SetText($"Page {currentPage + 1}/{SortedLists.Count}");
                Il2CppSystem.Collections.Generic.List<ApiAvatar> newList = new();
                foreach (var avi in SortedLists[currentPage])
                {
                    newList.Add(avi);
                }
                list.RenderElement(newList);
            }, 3, 1);

            currentLabel = new SMText(list.GetUiVRCList().expandButton.gameObject.transform, 800, 0, 150, 75, "Page 0/0");

            nextPage = new SMButton(SMButton.SMButtonType.ChangeAvatar, list.GetUiVRCList().expandButton.gameObject.transform, 950, 0, "->", delegate
            {
                if (currentPage + 1 == SortedLists.Count) return;
                currentPage++;
                currentLabel.SetText($"Page {currentPage + 1}/{SortedLists.Count}");
                Il2CppSystem.Collections.Generic.List<ApiAvatar> newList = new();
                foreach (var avi in SortedLists[currentPage])
                {
                    newList.Add(avi);
                }
                list.RenderElement(newList);
            }, 3, 1);

            var listener = avatarPage.gameObject.GetComponent<EnableDisableListener>();
            listener.OnEnabled += () =>
            {
                if (list.GetText().text == "Blaze's Search Results")
                {
                    list.GetGameObject().transform.SetSiblingIndex(0);
                    list.GetGameObject().SetActive(false);
                }
            };
        }

        public static void ProcessAvatars(AviSearchResults results)
        {
            MelonCoroutines.Start(DoShit(results));
        }

        private static IEnumerator DoShit(AviSearchResults results)
        {
            yield return new WaitForSeconds(1);
            avatars.Clear();
            foreach (var a in results.results)
            {
                avatars.Add(a.ToApiAvatar());
            }
            list.GetText().supportRichText = true;
            list.GetUiVRCList().expandedHeight *= 2f;
            list.GetUiVRCList().extendRows = 4;
            list.GetUiVRCList().startExpanded = false;

            SortedLists = avatars.ChunkBy(250);
            Il2CppSystem.Collections.Generic.List<ApiAvatar> firstList = new();
            foreach (var avi in SortedLists[0])
            {
                firstList.Add(avi);
            }

            list.GetText().supportRichText = true;
            foreach (var avatar in avatars.ToArray().Where(a => a.releaseStatus.ToLower().Equals("private")))
            {
                avatar.name = $"<color=red>[P]</color> {avatar.name}";
            }

            list.GetGameObject().SetActive(true);
            list.RenderElement(firstList);
            currentPage = 0;
            currentLabel.SetText($"Page {currentPage + 1}/{SortedLists.Count}");
            list.GetText().text = $"<color=blue>Blaze's Search</color> Found: <color=cyan>{avatars.Count}</color>";
            Logs.Log($"[Avatar Search] Found {avatars.Count}!", ConsoleColor.Magenta);
        }
    }
}
