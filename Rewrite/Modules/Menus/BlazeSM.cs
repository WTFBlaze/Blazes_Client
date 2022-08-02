using Blaze.API.SM;
using Blaze.Utils;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using VRC.Core;
using VRC.UI;
using static Blaze.Utils.Objects.ModObjects;
using static Blaze.Utils.Objects.OtherObjects;

namespace Blaze.Modules
{
    public class BlazeSM : BModule
    {
        public static SMPopup AvatarsPopup;
        public static SMPopup SocialPopup;
        public static SMPopup UserInfoPopup;
        public static SMPopup WorldInfoPopup;

        #region User Info Variables
        public static SMButton SMTP;
        public static SMButton SMForceClone;
        public static SMButton SMPortalToRoom;
        public static SMButton SMDownloadVRCA;
        public static SMButton SMForceInvite;
        #endregion

        #region Social Menu Variables
        public static int refreshCooldown;
        #endregion

        public override void Start()
        {
            ClassInjector.RegisterTypeInIl2Cpp<BlazeUserInfo>();
            ClassInjector.RegisterTypeInIl2Cpp<BlazeSocialMenu>();
        }

        public override void UI()
        {
            // Delete the Early Access image
            UnityEngine.Object.Destroy(GameObject.Find("UserInterface/MenuContent/Backdrop/Backdrop/Image"));

            // Avatars Page text
            GameObject.Find("UserInterface/MenuContent/Screens/Avatar/TitlePanel (1)/TitleText").GetComponent<Text>().text = $"<b><color={Colors.AquaHex}>Blaze's</color> <color={Colors.MagentaHex}>Client</color> - <color=yellow>v4.3</color></b>";

            #region Avatars Popup
            var avatarPage = GameObject.Find("UserInterface/MenuContent/Screens/Avatar").transform;
            var currPageAvatar = avatarPage.GetComponent<PageAvatar>();
            AvatarsPopup = new SMPopup(avatarPage, 860, 375, "Avatar Functions", "Blaze's Avatar Functions", 1.5f, 1, new Color(1, 0.6f, 0.015f));

            // Misc 
            new SMText(AvatarsPopup, -300, 150, 350, 75, "Misc", Color.white);

            new SMButton(SMButton.SMButtonType.ChangeAvatar, AvatarsPopup, -300, 95, "Change By ID", delegate
            {
                PopupUtils.InputPopup("Change Avi", "Enter Avatar ID...", delegate (string s)
                {
                    if (!RegexManager.IsValidAvatarID(s))
                    {
                        PopupUtils.InformationAlert("That is not a valid Avatar ID!");
                        return;
                    }
                    AvatarUtils.ChangeToAvatar(s);
                    AvatarsPopup.CloseMe();
                });
            }, 1.5f, 1);

            new SMButton(SMButton.SMButtonType.ChangeAvatar, AvatarsPopup, -300, 30, "Download VRCA", delegate
            {
                DownloadManager.DownloadVRCA(avatarPage.GetComponent<PageAvatar>().field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0);
            }, 1.5f, 1);

            new SMButton(SMButton.SMButtonType.ChangeAvatar, AvatarsPopup, -300, -35, "Import LNC", delegate
            {
                if (File.Exists(ModFiles.LNCImportFile))
                {
                    var lncList = JsonConvert.DeserializeObject<LNCAviFav>(File.ReadAllText(ModFiles.LNCImportFile));
                    PopupUtils.AlertV2($"Are you sure you want to import your Late Night Favorites Containing ({lncList.Avis.Count}) Avatars?", "Import", delegate
                    {
                        var newList = lncList.Avis.Select(avi => avi.ToModFavAvi()).ToList();
                        AvatarFavorites.AddNewList(newList, "Late Night");
                        Config.AviFavs.Save();
                        PopupUtils.HideCurrentPopUp();
                    }, "Cancel", PopupUtils.HideCurrentPopUp);
                    return;
                }
                PopupUtils.InformationAlert("You are missing Late Night's Avatar Favorite File in the Imports folder!\nVRChat/Blaze's Client/Imports/Late-Night-Avi-Favs.json");
            }, 1.5f, 1);

            new SMButton(SMButton.SMButtonType.ChangeAvatar, AvatarsPopup, -300, -100, "Import Apollo", delegate
            {
                if (File.Exists(ModFiles.ApolloImportFile))
                {
                    var apolloList = JsonConvert.DeserializeObject<ApolloAviFav>(File.ReadAllText(ModFiles.ApolloImportFile));
                    var count = 0;
                    foreach (var l in apolloList.AvatarFavorites.FavoriteLists)
                    {
                        count += l.Avatars.Count;
                    }
                    PopupUtils.AlertV2($"Are you sure you want to import your Apollo Favorites containing ({count}) Avatars?", "Import", delegate
                    {
                        List<ModAviFav> newList = new();
                        foreach (var l in apolloList.AvatarFavorites.FavoriteLists)
                        {
                            foreach (var a in l.Avatars)
                            {
                                newList.Add(a);
                            }
                        }
                        AvatarFavorites.AddNewList(newList, "Apollo");
                        Config.AviFavs.Save();
                        PopupUtils.HideCurrentPopUp();
                    }, "Cancel", PopupUtils.HideCurrentPopUp);
                    return;
                }
                PopupUtils.InformationAlert("You are missing Apollo's Avatar Favorite File in the Imports folder!\nVRChat/Blaze's Client/Imports/Apollo-Avi-Favs.json");
            }, 1.5f, 1);

            new SMButton(SMButton.SMButtonType.ChangeAvatar, AvatarsPopup, -300, -165, "EZRip Avi", delegate
            {
                EZRip.ProcessRip(avatarPage.GetComponent<PageAvatar>().field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0);
                AvatarsPopup.CloseMe();
            }, 1.5f, 1);

            new SMButton(SMButton.SMButtonType.ChangeAvatar, AvatarsPopup, -300, -230, "Clear Seen", delegate
            {
                AvatarFavorites.RecentlySeenAvatars.RenderElement(new Il2CppSystem.Collections.Generic.List<ApiAvatar>());
                AvatarsPopup.CloseMe();
            }, 1.6f, 1);

            new SMButton(SMButton.SMButtonType.ChangeAvatar, AvatarsPopup, -300, -295, "Clear Used", delegate
            {
                AvatarFavorites.RecentlyUsedAvatars.RenderElement(new Il2CppSystem.Collections.Generic.List<ApiAvatar>());
                AvatarsPopup.CloseMe();
            }, 1.6f, 1);

            // Avatar Favorites
            new SMText(AvatarsPopup, 50, 150, 350, 75, "Avi Favs", Color.white);
            new SMButton(SMButton.SMButtonType.ChangeAvatar, AvatarsPopup, 50, 95, "New List", AvatarFavorites.AddNewList, 1.5f, 1);
            new SMButton(SMButton.SMButtonType.ChangeAvatar, AvatarsPopup, 50, 30, "Copy Avi ID", delegate
            {
                Clipboard.SetText(currPageAvatar.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0.id);
            }, 1.5f, 1);

            // Avatar Search
            new SMText(AvatarsPopup, 350, 150, 350, 75, "Avi Search", Color.white);

            new SMButton(SMButton.SMButtonType.ChangeAvatar, AvatarsPopup, 350, 95, "Avatar Name", delegate
            {
                if (BlazeNetwork.IsConnected)
                {
                    PopupUtils.InputPopup("Search", "Enter Search Query", delegate (string s)
                    {
                        BlazeNetwork.ws.SendAsync(JsonConvert.SerializeObject(new
                        {
                            payload = new
                            {
                                type = "AvatarSearch",
                                data = new
                                {
                                    searchtype = "AvatarName",
                                    query = s
                                }
                            }
                        }), null);
                        AvatarsPopup.CloseMe();
                    });
                    return;
                }
                PopupUtils.InformationAlert("You must be connected to Blaze Networks to use that feature!");
            }, 1.6f, 1);

            new SMButton(SMButton.SMButtonType.ChangeAvatar, AvatarsPopup, 350, 30, "Author Name", delegate
            {
                if (BlazeNetwork.IsConnected)
                {
                    PopupUtils.InputPopup("Search", "Enter Search Query", delegate (string s)
                    {
                        BlazeNetwork.ws.SendAsync(JsonConvert.SerializeObject(new
                        {
                            payload = new
                            {
                                type = "AvatarSearch",
                                data = new
                                {
                                    searchtype = "AuthorName",
                                    query = s
                                }
                            }
                        }), null);
                        AvatarsPopup.CloseMe();
                    });
                    return;
                }
                PopupUtils.InformationAlert("You must be connected to Blaze Networks to use that feature!");
            }, 1.6f, 1);

            new SMButton(SMButton.SMButtonType.ChangeAvatar, AvatarsPopup, 350, -35, "Clear Search", delegate
            {
                AvatarSearch.list.RenderElement(new Il2CppSystem.Collections.Generic.List<ApiAvatar>());
                AvatarSearch.list.GetText().text = "Blaze's Search Results";
                AvatarSearch.list.GetGameObject().SetActive(false);
                AvatarsPopup.CloseMe();
            }, 1.6f, 1);
            #endregion

            #region User Info Popup
            var UserInfoScreen = GameObject.Find("UserInterface/MenuContent/Screens/UserInfo").transform;
            UserInfoPopup = new SMPopup(UserInfoScreen, -860, 335, "Blaze Options", "Blaze's User Info Functions", 1.5f, 1, new Color(0, 0.45f, 0));

            SMTP = new SMButton(SMButton.SMButtonType.ChangeAvatar, UserInfoPopup, -300, 145, "Teleport To", delegate
            {
                Functions.TeleportByUserID(SocialMenuUtils.GetAPIUser().id);
            }, 1.5f, 1);
            SMTP.SetInteractable(false);

            SMForceClone = new SMButton(SMButton.SMButtonType.ChangeAvatar, UserInfoPopup, -300, 80, "Force Clone", delegate
            {
                AvatarUtils.ChangeToAvatar(Functions.GetPlayerByUserID(SocialMenuUtils.GetAPIUser().id).prop_ApiAvatar_0.id);
            }, 1.5f, 1);
            SMForceClone.SetInteractable(false);

            new SMButton(SMButton.SMButtonType.ChangeAvatar, UserInfoPopup, -300, 15, "View Avatars", delegate
            {
                if (BlazeNetwork.IsConnected)
                {
                    var userID = SocialMenuUtils.GetAPIUser().id;
                    VRCUiManager.prop_VRCUiManager_0.ShowScreen(GameObject.Find("UserInterface/MenuContent/Screens/Avatar").GetComponent<PageAvatar>());
                    BlazeNetwork.ws.SendAsync(JsonConvert.SerializeObject(new
                    {
                        payload = new
                        {
                            type = "AvatarSearch",
                            data = new
                            {
                                searchtype = "AuthorID",
                                query = userID
                            }
                        }
                    }), null);
                    return;
                }
                PopupUtils.InformationAlert("You must be connected to Blaze's Networks to use that feature!");
            }, 1.5f, 1);

            new SMButton(SMButton.SMButtonType.ChangeAvatar, UserInfoPopup, -300, -50, "Copy Info", delegate
            {
                var sb = new StringBuilder();
                var user = SocialMenuUtils.GetAPIUser();
                sb.AppendLine("======[Blaze's Client]======");
                sb.AppendLine("Registered Name: " + user.username);
                sb.AppendLine("Display Name: " + user.displayName);
                sb.AppendLine("ID: " + user.id);
                sb.AppendLine("============================");
                Clipboard.SetText(sb.ToString());
                PopupUtils.InformationAlert("Sucessfully copied user's information to your clipboard!");
            }, 1.5f, 1);

            SMPortalToRoom = new SMButton(SMButton.SMButtonType.ChangeAvatar, UserInfoPopup, -300, -115, "Portal To Room", delegate
            {
                Functions.DropPortal(SocialMenuUtils.GetAPIUser().location);
            }, 1.5f, 1);
            SMPortalToRoom.SetInteractable(false);

            SMDownloadVRCA = new SMButton(SMButton.SMButtonType.ChangeAvatar, UserInfoPopup, -300, -180, "Download VRCA", delegate
            {
                DownloadManager.DownloadVRCA(Functions.GetPlayerByUserID(SocialMenuUtils.GetAPIUser().id).prop_ApiAvatar_0);
            }, 1.5f, 1);
            SMDownloadVRCA.SetInteractable(false);

            SMForceInvite = new SMButton(SMButton.SMButtonType.ChangeAvatar, UserInfoPopup, -300, -245, "Force Invite", delegate
            {
                NotificationUtils.SendInvite(WorldUtils.GetJoinID(), WorldUtils.CurrentWorld().name, SocialMenuUtils.GetAPIUser().id);
            }, 1.5f, 1);

            UserInfoPopup.GetPanelObject().AddComponent<BlazeUserInfo>();
            #endregion

            #region Social Popup
            var SocialScreen = GameObject.Find("UserInterface/MenuContent/Screens/Social").transform;
            SocialPopup = new SMPopup(SocialScreen, -870, 335, "Blaze's Options", "Blaze's Social Functions", 1.5f, 1, Color.black);

            new SMButton(SMButton.SMButtonType.ChangeAvatar, SocialPopup, -300, 145, "Reload Social", delegate
            {
                if (refreshCooldown <= 0)
                {
                    using var enumerator = Resources.FindObjectsOfTypeAll<UiUserList>().GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var uiUserList = enumerator.Current;
                        try
                        {
                            uiUserList.Method_Public_Void_0();
                            uiUserList.Method_Public_Void_1();
                        }
                        catch (Exception ex)
                        {
                            Logs.Error("Social Menu Refresh", ex);
                        }
                        refreshCooldown = 10;
                        MelonCoroutines.Start(CooldownTimer());
                    }
                    return;
                }
                PopupUtils.InformationAlert($"You must wait {refreshCooldown} seconds before refreshing again!", 10f);
            }, 1.5f, 1);

            new SMButton(SMButton.SMButtonType.ChangeAvatar, SocialPopup, -300, 80, "Export Friends", delegate
            {
                if (!File.Exists(ModFiles.FriendsExportFile))
                    FileManager.CreateFile(ModFiles.FriendsExportFile);
                else
                {
                    if (new FileInfo(ModFiles.FriendsExportFile).Length > 0)
                        FileManager.WipeTextFromFile(ModFiles.FriendsExportFile);
                }

                var list = APIUser.CurrentUser.friendIDs;
                foreach (var id in list)
                {
                    FileManager.AppendLineToFile(ModFiles.FriendsExportFile, id);
                }
                Logs.Debug($"<color=yellow>[EXPORTS]</color> Exported Friends List with (<color=yellow>{list.Count}</color>) ids!");
                Logs.Log($"[EXPORTS] Exported Friends List with ({list.Count}) ids!", ConsoleColor.Yellow);
            }, 1.5f, 1);

            new SMButton(SMButton.SMButtonType.ChangeAvatar, SocialPopup, -300, 15, "Import Friends", delegate
            {
                MelonCoroutines.Start(Functions.ProcessFriendsImport());
            }, 1.5f, 1);
            #endregion

            #region World Info Popup
            var WorldInfoScreen = GameObject.Find("UserInterface/MenuContent/Screens/WorldInfo").transform;
            WorldInfoPopup = new SMPopup(WorldInfoScreen, 605, -275, "Blaze's Options", "Blaze's World Info Functions", 1.5f, 1, Color.gray);

            new SMButton(SMButton.SMButtonType.ChangeAvatar, WorldInfoPopup, -300, 145, "Download VRCW", delegate
            {
                var api = WorldInfoScreen.GetComponent<PageWorldInfo>().field_Private_ApiWorld_0;
                DownloadManager.DownloadVRCW(api);
            }, 1.5f, 1);

            new SMButton(SMButton.SMButtonType.ChangeAvatar, WorldInfoPopup, -300, 80, "Custom Portal", delegate
            {
                PopupUtils.InputPopup("Drop Portal", "Enter Custom Portal Text...", delegate (string s)
                {
                    var api = WorldInfoScreen.GetComponent<PageWorldInfo>().field_Private_ApiWorld_0;
                    Functions.DropPortal($"{api.id}:{s.Replace(' ', '_')}");
                    SocialMenuUtils.CloseUI();
                });
            }, 1.5f, 1);
            #endregion

            GameObject.Find("UserInterface/MenuContent/Screens/Social").AddComponent<BlazeSocialMenu>();
        }

        private static IEnumerator CooldownTimer()
        {
            yield return new WaitForSecondsRealtime(refreshCooldown);
            refreshCooldown = 0;
        }
    }

    public class BlazeUserInfo : MonoBehaviour
    {
        public BlazeUserInfo(IntPtr id) : base(id) { }
        public static float targetTime = 0.5f;
        public void Awake() => targetTime = 0f;
        public void Update()
        {
            targetTime -= Time.deltaTime;
            if (targetTime <= 0)
            {
                var user = Functions.GetPlayerByUserID(SocialMenuUtils.GetAPIUser().id);
                if (user == null)
                {
                    BlazeSM.SMDownloadVRCA.SetInteractable(false);
                    BlazeSM.SMTP.SetInteractable(false);
                    BlazeSM.SMForceClone.SetInteractable(false);
                    BlazeSM.SMPortalToRoom.SetInteractable(SocialMenuUtils.GetAPIUser().location != "private");
                    BlazeSM.SMForceInvite.SetInteractable(true);
                }
                else
                {
                    BlazeSM.SMDownloadVRCA.SetInteractable(true);
                    BlazeSM.SMTP.SetInteractable(user.prop_APIUser_0.id != PlayerUtils.CurrentUser().GetAPIUser().id);
                    BlazeSM.SMForceClone.SetInteractable(user.prop_ApiAvatar_0.releaseStatus != "private");
                    BlazeSM.SMPortalToRoom.SetInteractable(false);
                    BlazeSM.SMForceInvite.SetInteractable(false);
                }

                var trustObj = GameObject.Find("UserInterface/MenuContent/Screens/UserInfo/User Panel/TrustLevel");
                trustObj.transform.Find("TrustText").gameObject.GetComponent<Text>().text = SocialMenuUtils.GetAPIUser().GetTrueRank();
                trustObj.transform.Find("TrustIcon").gameObject.GetComponent<RawImage>().color = SocialMenuUtils.GetAPIUser().GetTrueRankUnityColor();
            }
        }
    }

    public class BlazeSocialMenu : MonoBehaviour
    {
        public BlazeSocialMenu(IntPtr id) : base(id) { }

        private GameObject onlineFriendsObj;
        private GameObject inRoomObj;

        public void Start()
        {
            inRoomObj = GameObject.Find("UserInterface/MenuContent/Screens/Social/Vertical Scroll View/Viewport/Content/InRoom/Button/TitleText");
            inRoomObj.GetComponent<Text>().supportRichText = true;

            onlineFriendsObj = GameObject.Find("UserInterface/MenuContent/Screens/Social/Vertical Scroll View/Viewport/Content/OnlineFriends/Button/TitleText");
            onlineFriendsObj.GetComponent<Text>().supportRichText = true;
        }

        public void Update()
        {
            try
            {
                // Online Friends Label
                var onlineCount = 0;
                foreach (var p in GameObject.Find("UserInterface/MenuContent/Screens/Social/Vertical Scroll View/Viewport/Content/OnlineFriends").GetComponent<UiUserList>().pickers)
                {
                    if (p.gameObject.active)
                    {
                        onlineCount++;
                    }
                }
                onlineFriendsObj.GetComponent<Text>().text = $"<color=yellow>Online Friends</color> <color=white>[</color><color={Colors.AquaHex}>{onlineCount}</color><color=white>/</color><color={Colors.MagentaHex}>{PlayerUtils.CurrentUser().GetAPIUser().friendIDs.Count}</color><color=white>]</color>";
            }
            catch { }

            /*try
            {
                
            }
            catch 
            {
                
            }*/

            // In Room Label
            var roomCount = 0;
            foreach (var p in GameObject.Find("UserInterface/MenuContent/Screens/Social/Vertical Scroll View/Viewport/Content/InRoom").GetComponent<UiUserList>().pickers)
            {
                if (p.field_Public_String_0 == null) continue;
                /*var user = Functions.GetPlayerByUserID(p.field_Public_String_0);
                if (user.IsFriend() && user.GetUserID() != PlayerUtils.CurrentUser().GetUserID())
                {
                    p.field_Public_Text_0.text = $"<color=yellow><b>[F]</b></color>" + p.field_Public_Text_0.text;
                }*/

                if (p.gameObject.active)
                {
                    roomCount++;
                }
            }
            inRoomObj.GetComponent<Text>().text = $"<color=yellow>In Room</color> <color=white>[</color><color={Colors.AquaHex}>{roomCount}</color><color=white>/</color><color={Colors.MagentaHex}>{WorldUtils.CurrentWorld().capacity}</color><color=white>]</color>";
        }
    }
}
