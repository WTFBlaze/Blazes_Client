using Blaze.API.SM;
using Blaze.Configs;
using Blaze.Utils;
using Blaze.Utils.Attributes;
using Blaze.Utils.Objects;
using Blaze.Utils.VRChat;
using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityEngine;
using VRC.Core;
using VRC.UI;
using static Blaze.Utils.Objects.ModObjects;
using static Blaze.Utils.Objects.OtherObjects;

namespace Blaze.Modules
{
    class AvatarFavorites : BModule
    {
        public static Dictionary<int, SMList> FavlistDictonary = new();
        private static GameObject avatarPage;
        private static PageAvatar currPageAvatar;
        private static GameObject PublicAvatarList;
        public static List<GameObject> TestAvatarButtons = new();
        public static List<UiVRCList> AvatarpageLists = new();

        public override void QuickMenuUI()
        {
            avatarPage = GameObject.Find("UserInterface/MenuContent/Screens/Avatar");
            PublicAvatarList = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/Vertical Scroll View/Viewport/Content/Public Avatar List");
            currPageAvatar = avatarPage.GetComponent<PageAvatar>();
            LoadList();

            var listener = avatarPage.AddComponent<EnableDisableListener>();
            listener.OnEnabled += () =>
            {
                MelonCoroutines.Start(RefreshMenu(1f));
            };

            /*var aviBtn = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickLinks/Button_Avatars");
            aviBtn.GetComponent<UnityEngine.UI.Button>().onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            aviBtn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(new Action(() => 
            {
                SocialMenuUtils.OpenUI();
                VRCUiManager.prop_VRCUiManager_0.ShowScreen(avatarPage.GetComponent<VRCUiPage>());
                QuickMenuUtils.CloseUI();
            }));*/
        }

        public static void LoadList()
        {
            foreach (var list in Config.AvatarFavs.AvatarFavorites.FavoriteLists)
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
                                Config.AvatarFavs.Save();
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

        internal static void AddNewList()
        {
            var newID = Config.AvatarFavs.AvatarFavorites.FavoriteLists.Count;
            Config.AvatarFavs.AvatarFavorites.FavoriteLists.Add(new ModAviFavorites.FavoriteList
            {
                ID = newID,
                Avatars = new List<ModAviFav>(),
                Desciption = "",
                name = "[Blaze] List #" + newID
            });
            LoadList();
        }

        internal static void AddNewList(List<ModAviFav> aviList, string clientName)
        {
            var newID = Config.AvatarFavs.AvatarFavorites.FavoriteLists.Count;
            Config.AvatarFavs.AvatarFavorites.FavoriteLists.Add(new ModAviFavorites.FavoriteList
            {
                ID = newID,
                Avatars = new List<ModAviFav>(),
                Desciption = "",
                name = "[Apollo] List #" + newID
            });
            Config.AvatarFavs.AvatarFavorites.FavoriteLists[newID].Avatars = aviList;
            Config.AvatarFavs.AvatarFavorites.FavoriteLists[newID].name = $"Imported: {clientName}";
            LoadList();
        }

        internal static void RemoveList(int ListID)
        {
            var ConfigList = Config.AvatarFavs.AvatarFavorites.FavoriteLists.FirstOrDefault(list => list.ID == ListID);
            var AvatarList = FavlistDictonary[ListID];
            if (ConfigList != null && AvatarList != null)
            {
                Config.AvatarFavs.AvatarFavorites.FavoriteLists.Remove(ConfigList);
                UnityEngine.Object.Destroy(AvatarList.GetGameObject());
                FavlistDictonary.Remove(ListID);
                Config.AvatarFavs.Save();
            }
        }

        internal static void FavoriteAvatar(ApiAvatar avatar, int ListID)
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
            Config.AvatarFavs.Save();
        }

        internal static void UnfavoriteAvatar(ApiAvatar avatar, int ListID)
        {
            if (GetConfigList(ListID) != null)
            {
                GetConfigList(ListID).Avatars.Remove(GetConfigList(ListID).Avatars.FirstOrDefault(avi => avi.ID == avatar.id));
            }
            MelonCoroutines.Start(RefreshMenu(1f));
            Config.AvatarFavs.Save();
        }

        private static ModAviFavorites.FavoriteList GetConfigList(int ID) => Config.AvatarFavs.AvatarFavorites.FavoriteLists.FirstOrDefault(List => List.ID == ID);

        private static IEnumerator RefreshMenu(float v)
        {
            foreach (var list in Config.AvatarFavs.AvatarFavorites.FavoriteLists)
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
        }
    }
}
