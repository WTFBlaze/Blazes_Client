using Blaze.API;
using Blaze.API.SM;
using Blaze.Utils;
using Blaze.Utils.Attributes;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
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
using VRC.UI;
using static Blaze.Utils.Objects.ModObjects;

namespace Blaze.Modules
{
    public class AvatarFavorites : BModule
    {
        public static Dictionary<int, SMList> FavlistDictonary = new();
        private static GameObject avatarPage;
        private static PageAvatar currPageAvatar;
        private static GameObject PublicAvatarList;
        public static List<GameObject> TestAvatarButtons = new();
        public static List<UiVRCList> AvatarpageLists = new();
        public static SMList RecentlyUsedAvatars;
        public static SMList RecentlySeenAvatars;
        private static List<ApiAvatar> usedAvis = new();
        private static List<ApiAvatar> seenAvis = new();
        private static UiAvatarList selfUploads;
        private static Text selfUploadsText;

        public override void UI()
        {
            avatarPage = GameObject.Find("UserInterface/MenuContent/Screens/Avatar");
            PublicAvatarList = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/Vertical Scroll View/Viewport/Content/Public Avatar List");
            currPageAvatar = avatarPage.GetComponent<PageAvatar>();
            LoadList();

            RecentlyUsedAvatars = new SMList(APIStuff.GetSocialMenuInstance().transform.Find("Avatar/Vertical Scroll View/Viewport/Content"), "<color=yellow>Recently Used Avis</color>");
            RecentlyUsedAvatars.GetText().supportRichText = true;
            RecentlySeenAvatars = new SMList(APIStuff.GetSocialMenuInstance().transform.Find("Avatar/Vertical Scroll View/Viewport/Content"), "<color=magenta>Recently Seen Avis</color>");
            RecentlySeenAvatars.GetText().supportRichText = true;

            var listener = avatarPage.AddComponent<EnableDisableListener>();
            listener.OnEnabled += () =>
            {
                MelonCoroutines.Start(RefreshMenu(1f));
            };
        }

        public static void LoadList()
        {
            foreach (var list in Config.AviFavs.AvatarFavorites.FavoriteLists)
            {
                if (!FavlistDictonary.ContainsKey(list.ID))
                {
                    var newlist = new SMList(PublicAvatarList.transform.parent, list.name, list.ID);
                    var listofbuttons = new List<SMButton>
                    {
                        new(SMButton.SMButtonType.ChangeAvatar, newlist.GetUiVRCList().expandButton.gameObject.transform, 600, 0, "Fav/UnFav", delegate {
                            if (!list.Avatars.Exists(avi => avi.ID == currPageAvatar.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0.id))
                                FavoriteAvatar(currPageAvatar.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0, list.ID);
                            else
                                UnfavoriteAvatar(currPageAvatar.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0, list.ID);
                        }, 2, 1),
                        new(SMButton.SMButtonType.ChangeAvatar, newlist.GetUiVRCList().expandButton.gameObject.transform, 750, 0, "Destroy", delegate
                        {
                            PopupUtils.AlertV2($"Woah There! By Destroying this list you are deleting it forever! Are you sure you want to do delete [{list.name}]?", "Delete", delegate
                            {
                                RemoveList(list.ID);
                                PopupUtils.HideCurrentPopUp();
                            }, "Cancel", PopupUtils.HideCurrentPopUp);
                        }, 2, 1),
                        new(SMButton.SMButtonType.ChangeAvatar, newlist.GetUiVRCList().expandButton.gameObject.transform, 900, 0, "Rename", delegate
                        {
                            PopupUtils.InputPopup("Set Name", "ERP Avatars", delegate (string text)
                            {
                                list.name = text;
                                newlist.GetText().text = text;
                                Config.AviFavs.Save();
                                PopupUtils.HideCurrentPopUp();
                            });
                        }, 2, 1)
                    };
                    listofbuttons.ForEach(b => b.SetActive(false));
                    newlist.GetUiVRCList().expandButton.onClick.AddListener(new Action(() =>
                    {
                        listofbuttons.ForEach(b => b.SetActive(!b.button.activeSelf));
                    }));
                    newlist.GetUiVRCList().extendRows = list.Rows;
                    newlist.GetUiVRCList().expandedHeight += 300 * (list.Rows - 2);
                    FavlistDictonary.Add(list.ID, newlist);
                }
            }
            MelonCoroutines.Start(RefreshMenu(1f));
        }

        public static void AddNewList()
        {
            var newID = Config.AviFavs.AvatarFavorites.FavoriteLists.Count;
            Config.AviFavs.AvatarFavorites.FavoriteLists.Add(new ModAviFavorites.FavoriteList
            {
                ID = newID,
                Avatars = new List<ModAviFav>(),
                Desciption = "",
                name = "[Blaze] List #" + newID
            });
            LoadList();
        }

        public static void AddNewList(List<ModAviFav> aviList, string clientName)
        {
            var newID = Config.AviFavs.AvatarFavorites.FavoriteLists.Count;
            Config.AviFavs.AvatarFavorites.FavoriteLists.Add(new ModAviFavorites.FavoriteList
            {
                ID = newID,
                Avatars = new List<ModAviFav>(),
                Desciption = "",
                name = "[Blaze] List #" + newID
            });
            Config.AviFavs.AvatarFavorites.FavoriteLists[newID].Avatars = aviList;
            Config.AviFavs.AvatarFavorites.FavoriteLists[newID].name = $"Imported: {clientName}";
            LoadList();
        }

        public static void RemoveList(int ListID)
        {
            var ConfigList = Config.AviFavs.AvatarFavorites.FavoriteLists.FirstOrDefault(list => list.ID == ListID);
            var AvatarList = FavlistDictonary[ListID];
            if (ConfigList != null && AvatarList != null)
            {
                Config.AviFavs.AvatarFavorites.FavoriteLists.Remove(ConfigList);
                UnityEngine.Object.Destroy(AvatarList.GetGameObject());
                FavlistDictonary.Remove(ListID);
                Config.AviFavs.Save();
            }
        }

        public static void FavoriteAvatar(ApiAvatar avatar, int ListID)
        {
            var avatarobject = avatar.ToModFavAvi();
            if (GetConfigList(ListID) != null)
            {
                if (!GetConfigList(ListID).Avatars.Exists(avi => avi.ID == avatar.id))
                {
                    GetConfigList(ListID).Avatars.Insert(0, avatarobject);
                }
            }
            MelonCoroutines.Start(RefreshMenu(1f));
            Config.AviFavs.Save();
        }

        public static void UnfavoriteAvatar(ApiAvatar avatar, int ListID)
        {
            if (GetConfigList(ListID) != null)
            {
                GetConfigList(ListID).Avatars.Remove(GetConfigList(ListID).Avatars.FirstOrDefault(avi => avi.ID == avatar.id));
            }
            MelonCoroutines.Start(RefreshMenu(1f));
            Config.AviFavs.Save();
        }

        private static ModAviFavorites.FavoriteList GetConfigList(int ID) => Config.AviFavs.AvatarFavorites.FavoriteLists.FirstOrDefault(List => List.ID == ID);

        private static IEnumerator RefreshMenu(float v)
        {
            if (PlayerUtils.CurrentUser() == null || RecentlyUsedAvatars == null || RecentlySeenAvatars == null) yield break;
            foreach (var list in Config.AviFavs.AvatarFavorites.FavoriteLists)
            {
                yield return new WaitForSeconds(v);
                if (FavlistDictonary[list.ID] != null)
                {
                    Il2CppSystem.Collections.Generic.List<ApiAvatar> AvatarList = new();
                    list.Avatars.ForEach(avi => AvatarList.Add(avi.ToApiAvatar()));
                    FavlistDictonary[list.ID].RenderElement(AvatarList);
                    FavlistDictonary[list.ID].GetText().text = $"{list.name} [{AvatarList.Count}] {list.Desciption}";
                }
            }

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
            GetSelfText().text = $"<color=yellow>My Uploads</color> <color=white>[</color><color={Colors.AquaHex}>{uploadsCount}</color><color=white>]</color>";
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
