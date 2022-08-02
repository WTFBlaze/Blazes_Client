using Blaze.API.QM;
using Blaze.Configs;
using Blaze.Utils;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC.Core;
using VRC.DataModel;

namespace Blaze.Modules
{
    class GlobalBones : BModule
    {
        private QMNestedButton Menu;
        private QMNestedButton SelfMenu;
        private QMNestedButton WLMenu;
        private QMNestedButton FMenu;
        private QMNestedButton OMenu;
        private QMToggleButton WhitelistUser;
        private QMSingleButton MaxRadiusButton;
        private QMSingleButton OwnCollidersOptionButton;
        private QMSingleButton WLCollidersOptionButton;
        private QMSingleButton FCollidersOptionButton;
        private QMSingleButton OCollidersOptionButton;

        [Flags]
        internal enum CollisionFlag
        {
            Self = (1 << 0),
            Whitelist = (1 << 1),
            Friends = (1 << 2),
            Others = (1 << 3)
        }

        internal enum ColliderOption
        {
            None = 0,
            Hands,
            HandsAndLowerBody, // if you need to kick someone's ears off
            All
        }

        private readonly List<DynamicBone> _ownDynamicBones = new List<DynamicBone>();
        private readonly List<DynamicBoneCollider> _ownDynamicBoneColliders = new List<DynamicBoneCollider>();

        private readonly List<DynamicBone> _whitelistedDynamicBones = new List<DynamicBone>();
        private readonly List<DynamicBoneCollider> _whitelistedBoneColliders = new List<DynamicBoneCollider>();

        private readonly List<DynamicBone> _friendsDynamicBones = new List<DynamicBone>();
        private readonly List<DynamicBoneCollider> _friendsDynamicBoneColliders = new List<DynamicBoneCollider>();

        private readonly List<DynamicBone> _othersDynamicBones = new List<DynamicBone>();
        private readonly List<DynamicBoneCollider> _othersDynamicBoneColliders = new List<DynamicBoneCollider>();

        public override void SelectedUser(IUser user, bool isRemote)
        {
            if (isRemote) return;
            WhitelistUser.SetToggleState(Config.GlobalBones.IsWhitelisted(user.prop_String_0));
        }

        public override void QuickMenuUI()
        {
            Menu = new QMNestedButton(BlazeMenu.Settings, "Global\nBones", 4, 1, "Allow and modify dynamic bones", "Global Bones");
            new QMToggleButton(Menu, 1, 0, "Global Bones", delegate
            {
                ToggleDynamicBones(true);
            }, delegate
            {
                ToggleDynamicBones(false);
            }, "Completely enable or disable Global Bones from working");

            WhitelistUser = new QMToggleButton(BlazeMenu.Selected, 3, 2, "GB Whitelist", delegate
            {
                Config.GlobalBones.WhitelistUser(BlazeInfo.SelectedPlayer.GetUserID());
            }, delegate
            {
                Config.GlobalBones.RemoveWhitelist(BlazeInfo.SelectedPlayer.GetUserID());
            }, "Add or Remove this user from Global Bones whitelisting system");

            MaxRadiusButton = new QMSingleButton(Menu, 2, 0, $"Max Radius: <color=yellow>{Config.GlobalBones.MaxRadius}</color>", delegate
            {
                PopupUtils.NumericPopup("Set Radius", "Enter Max Radius Here...", delegate (string s)
                {
                    if (string.IsNullOrEmpty(s)) return;
                    Config.GlobalBones.MaxRadius = float.Parse(s);
                    MaxRadiusButton.SetButtonText($"Max Radius: <color=yellow>{Config.GlobalBones.MaxRadius}</color>");
                });
            }, "Click to change the max radius for global bones");

            new QMToggleButton(Menu, 3, 0, "Auto Reload Avis", delegate
            {
                Config.GlobalBones.AutoReloadAvatars = true;
                VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
            }, delegate
            {
                Config.GlobalBones.AutoReloadAvatars = false;
            }, "Automatically reload avatars when changing Global Bones Settings", Config.GlobalBones.AutoReloadAvatars);

            new QMSingleButton(Menu, 4, 0, "Reload\nAll Avis", delegate
            {
                VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
            }, "Manually Force reload all avatars. (Required to update Global Bones Settings)");

            #region Self
            SelfMenu = new QMNestedButton(Menu, "Self", 1, 1, "Change Global Bones Options for yourself", "GB - Self");

            OwnCollidersOptionButton = new QMSingleButton(SelfMenu, 1, 0, $"Colliders: <color=yellow>{Config.GlobalBones.OwnColliderOption}</color>", delegate
            {
                var o = Config.GlobalBones.OwnColliderOption;
                CycleColliderOption(OwnCollidersOptionButton, ref o);
                Config.GlobalBones.OwnColliderOption = o;

                _ownDynamicBoneColliders.Clear(); // clear this because we might have colliders we don't want in here
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Change which colliders are applied to yourself");

            new QMToggleButton(SelfMenu, 2, 0, "Always Include Head", delegate
            {
                Config.GlobalBones.AlwaysIncludeOwnHead = true;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.AlwaysIncludeOwnHead = false;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Always include your own head colliders", Config.GlobalBones.AlwaysIncludeOwnHead);

            new QMToggleButton(SelfMenu, 3, 0, "Add To Self", delegate
            {
                Config.GlobalBones.OwnCollisionFlag |= CollisionFlag.Self;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.OwnCollisionFlag &= ~CollisionFlag.Self;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Add colliders to yourself", (Config.GlobalBones.OwnCollisionFlag & CollisionFlag.Self) == CollisionFlag.Self);

            new QMToggleButton(SelfMenu, 4, 0, "Add To Whitelisted", delegate
            {
                Config.GlobalBones.OwnCollisionFlag |= CollisionFlag.Whitelist;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.OwnCollisionFlag &= ~CollisionFlag.Whitelist;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Add colliders to whitelisted users", (Config.GlobalBones.OwnCollisionFlag & CollisionFlag.Whitelist) == CollisionFlag.Whitelist);

            new QMToggleButton(SelfMenu, 1, 1, "Add To Friends", delegate
            {
                Config.GlobalBones.OwnCollisionFlag |= CollisionFlag.Friends;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.OwnCollisionFlag &= ~CollisionFlag.Friends;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Add colliders to your friends", (Config.GlobalBones.OwnCollisionFlag & CollisionFlag.Friends) == CollisionFlag.Friends);

            new QMToggleButton(SelfMenu, 2, 1, "Add To Others", delegate
            {
                Config.GlobalBones.OwnCollisionFlag |= CollisionFlag.Others;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.OwnCollisionFlag &= ~CollisionFlag.Others;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Add colliders to other users", (Config.GlobalBones.OwnCollisionFlag & CollisionFlag.Others) == CollisionFlag.Others);
            #endregion

            #region Whitelisted
            WLMenu = new QMNestedButton(Menu, "Whitelisted", 2, 1, "Change Global Bones for Whitelisted Users", "GB - Whitelisted");

            WLCollidersOptionButton = new QMSingleButton(WLMenu, 1, 0, $"Colliders: <color=yellow>{Config.GlobalBones.WhitelistColliderOption}</color>", delegate
            {
                var o = Config.GlobalBones.WhitelistColliderOption;
                CycleColliderOption(WLCollidersOptionButton, ref o);
                Config.GlobalBones.WhitelistColliderOption = o;
                _whitelistedBoneColliders.Clear(); // clear this because we might have colliders we don't want in here
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Change which colliders are applied to whitelisted users");

            new QMToggleButton(WLMenu, 2, 0, "Always Include Head", delegate
            {
                Config.GlobalBones.AlwaysIncludeWhitelistedHead = true;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.AlwaysIncludeWhitelistedHead = false;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Always include the head collider", Config.GlobalBones.AlwaysIncludeWhitelistedHead);

            new QMToggleButton(WLMenu, 3, 0, "Add To Self", delegate
            {
                Config.GlobalBones.WhitelistCollisionFlag |= CollisionFlag.Self;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.WhitelistCollisionFlag &= ~CollisionFlag.Self;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Add colliders to yourself", (Config.GlobalBones.WhitelistCollisionFlag & CollisionFlag.Self) == CollisionFlag.Self);

            new QMToggleButton(WLMenu, 4, 0, "Add To Whitelisted", delegate
            {
                Config.GlobalBones.WhitelistCollisionFlag |= CollisionFlag.Whitelist;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.WhitelistCollisionFlag &= ~CollisionFlag.Whitelist;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Add colliders to whitelisted users", (Config.GlobalBones.WhitelistCollisionFlag & CollisionFlag.Whitelist) == CollisionFlag.Whitelist);

            new QMToggleButton(WLMenu, 1, 1, "Add To Friends", delegate
            {
                Config.GlobalBones.WhitelistCollisionFlag |= CollisionFlag.Friends;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.WhitelistCollisionFlag &= ~CollisionFlag.Friends;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Add colliders to your friends", (Config.GlobalBones.WhitelistCollisionFlag & CollisionFlag.Friends) == CollisionFlag.Friends);

            new QMToggleButton(WLMenu, 2, 1, "Add To Others", delegate
            {
                Config.GlobalBones.WhitelistCollisionFlag |= CollisionFlag.Others;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.WhitelistCollisionFlag &= ~CollisionFlag.Others;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Add colliders to other users", (Config.GlobalBones.WhitelistCollisionFlag & CollisionFlag.Others) == CollisionFlag.Others);
            #endregion

            #region Friends
            FMenu = new QMNestedButton(Menu, "Friends", 3, 1, "Change Global Bones for Friends", "GB - Friends");

            FCollidersOptionButton = new QMSingleButton(FMenu, 1, 0, $"Colliders: <color=yellow>{Config.GlobalBones.WhitelistColliderOption}</color>", delegate
            {
                var o = Config.GlobalBones.FriendsColliderOption;
                CycleColliderOption(FCollidersOptionButton, ref o);
                Config.GlobalBones.FriendsColliderOption = o;
                _friendsDynamicBoneColliders.Clear(); // clear this because we might have colliders we don't want in here
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Change which colliders are applied to friended users");

            new QMToggleButton(FMenu, 2, 0, "Always Include Head", delegate
            {
                Config.GlobalBones.AlwaysIncludeFriendsHead = true;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.AlwaysIncludeFriendsHead = false;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Always include the head collider", Config.GlobalBones.AlwaysIncludeFriendsHead);

            new QMToggleButton(FMenu, 3, 0, "Add To Self", delegate
            {
                Config.GlobalBones.FriendsCollisionFlag |= CollisionFlag.Self;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.FriendsCollisionFlag &= ~CollisionFlag.Self;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Add colliders to yourself", (Config.GlobalBones.FriendsCollisionFlag & CollisionFlag.Self) == CollisionFlag.Self);

            new QMToggleButton(FMenu, 4, 0, "Add To Whitelisted", delegate
            {
                Config.GlobalBones.FriendsCollisionFlag |= CollisionFlag.Whitelist;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.FriendsCollisionFlag &= ~CollisionFlag.Whitelist;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Add colliders to whitelisted users", (Config.GlobalBones.FriendsCollisionFlag & CollisionFlag.Whitelist) == CollisionFlag.Whitelist);

            new QMToggleButton(FMenu, 1, 1, "Add To Friends", delegate
            {
                Config.GlobalBones.FriendsCollisionFlag |= CollisionFlag.Friends;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.FriendsCollisionFlag &= ~CollisionFlag.Friends;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Add colliders to your friends", (Config.GlobalBones.FriendsCollisionFlag & CollisionFlag.Friends) == CollisionFlag.Friends);

            new QMToggleButton(FMenu, 2, 1, "Add To Others", delegate
            {
                Config.GlobalBones.FriendsCollisionFlag |= CollisionFlag.Others;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.FriendsCollisionFlag &= ~CollisionFlag.Others;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Add colliders to other users", (Config.GlobalBones.FriendsCollisionFlag & CollisionFlag.Others) == CollisionFlag.Others);
            #endregion

            #region Others
            OMenu = new QMNestedButton(Menu, "Others", 4, 1, "Change Global Bones for Friends", "GB - Friends");

            OCollidersOptionButton = new QMSingleButton(OMenu, 1, 0, $"Colliders: <color=yellow>{Config.GlobalBones.WhitelistColliderOption}</color>", delegate
            {
                var o = Config.GlobalBones.OthersColliderOption;
                CycleColliderOption(OCollidersOptionButton, ref o);
                Config.GlobalBones.OthersColliderOption = o;
                _othersDynamicBoneColliders.Clear(); // clear this because we might have colliders we don't want in here
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Change which colliders are applied to other users");

            new QMToggleButton(OMenu, 2, 0, "Always Include Head", delegate
            {
                Config.GlobalBones.AlwaysIncludeFriendsHead = true;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.AlwaysIncludeOthersHead = false;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Always include the head collider", Config.GlobalBones.AlwaysIncludeFriendsHead);

            new QMToggleButton(OMenu, 3, 0, "Add To Self", delegate
            {
                Config.GlobalBones.OthersCollisionFlag |= CollisionFlag.Self;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.OthersCollisionFlag &= ~CollisionFlag.Self;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Add colliders to yourself", (Config.GlobalBones.OthersCollisionFlag & CollisionFlag.Self) == CollisionFlag.Self);

            new QMToggleButton(OMenu, 4, 0, "Add To Whitelisted", delegate
            {
                Config.GlobalBones.OthersCollisionFlag |= CollisionFlag.Whitelist;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.OthersCollisionFlag &= ~CollisionFlag.Whitelist;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Add colliders to whitelisted users", (Config.GlobalBones.OthersCollisionFlag & CollisionFlag.Whitelist) == CollisionFlag.Whitelist);

            new QMToggleButton(OMenu, 1, 1, "Add To Friends", delegate
            {
                Config.GlobalBones.OthersCollisionFlag |= CollisionFlag.Friends;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.OthersCollisionFlag &= ~CollisionFlag.Friends;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Add colliders to your friends", (Config.GlobalBones.OthersCollisionFlag & CollisionFlag.Friends) == CollisionFlag.Friends);

            new QMToggleButton(OMenu, 2, 1, "Add To Others", delegate
            {
                Config.GlobalBones.OthersCollisionFlag |= CollisionFlag.Others;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, delegate
            {
                Config.GlobalBones.OthersCollisionFlag &= ~CollisionFlag.Others;
                if (Config.GlobalBones.AutoReloadAvatars)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
                }
            }, "Add colliders to other users", (Config.GlobalBones.OthersCollisionFlag & CollisionFlag.Others) == CollisionFlag.Others);
            #endregion
        }

        public override void AvatarIsReady(VRCPlayer vrcPlayer, ApiAvatar apiAvatar)
        {
            if (!Config.GlobalBones.Enabled) return;

            var apiUser = vrcPlayer.GetAPIUser();
            if (apiUser == null)return;
            var avatarObject = vrcPlayer.field_Internal_GameObject_0;
            var animator = avatarObject.GetComponentInChildren<Animator>();

            var isSelf = vrcPlayer.gameObject == PlayerUtils.CurrentUser().gameObject;
            var isWhitelisted = !isSelf && Config.GlobalBones.IsWhitelisted(apiUser.id);
            var isFriend = !isSelf && APIUser.IsFriendsWith(apiUser.id);
            var isOther = !isSelf && !isFriend && !isWhitelisted;

            ColliderOption colliderOption;
            CollisionFlag collisionFlag;
            List<DynamicBone> bonesList;
            List<DynamicBoneCollider> collidersList;
            bool alwaysIncludeHead;

            if (isSelf)
            {
                colliderOption = Config.GlobalBones.OwnColliderOption;
                collisionFlag = Config.GlobalBones.OwnCollisionFlag;
                bonesList = _ownDynamicBones;
                collidersList = _ownDynamicBoneColliders;
                alwaysIncludeHead = Config.GlobalBones.AlwaysIncludeOwnHead;
            }
            else if (isWhitelisted)
            {
                colliderOption = Config.GlobalBones.WhitelistColliderOption;
                collisionFlag = Config.GlobalBones.WhitelistCollisionFlag;
                bonesList = _whitelistedDynamicBones;
                collidersList = _whitelistedBoneColliders;
                alwaysIncludeHead = Config.GlobalBones.AlwaysIncludeWhitelistedHead;
            }
            else if (isFriend)
            {
                colliderOption = Config.GlobalBones.FriendsColliderOption;
                collisionFlag = Config.GlobalBones.FriendsCollisionFlag;
                bonesList = _friendsDynamicBones;
                collidersList = _friendsDynamicBoneColliders;
                alwaysIncludeHead = Config.GlobalBones.AlwaysIncludeFriendsHead;
            }
            else
            {
                colliderOption = Config.GlobalBones.OthersColliderOption;
                collisionFlag = Config.GlobalBones.OthersCollisionFlag;
                bonesList = _othersDynamicBones;
                collidersList = _othersDynamicBoneColliders;
                alwaysIncludeHead = Config.GlobalBones.AlwaysIncludeOthersHead;
            }

            HandleColliderOption(collidersList, animator, avatarObject, colliderOption, alwaysIncludeHead);
            bonesList.AddRange(avatarObject.GetComponentsInChildren<DynamicBone>(true));

            if ((collisionFlag & CollisionFlag.Self) == CollisionFlag.Self)
            {
                AddCollidersToDynamicBones(collidersList, _ownDynamicBones);
            }
            if ((collisionFlag & CollisionFlag.Whitelist) == CollisionFlag.Whitelist)
            {
                AddCollidersToDynamicBones(collidersList, _whitelistedDynamicBones);
            }
            if ((collisionFlag & CollisionFlag.Friends) == CollisionFlag.Friends)
            {
                AddCollidersToDynamicBones(collidersList, _friendsDynamicBones);
            }
            if ((collisionFlag & CollisionFlag.Others) == CollisionFlag.Others)
            {
                AddCollidersToDynamicBones(collidersList, _othersDynamicBones);
            }

            if (isSelf)
            {
                if ((Config.GlobalBones.WhitelistCollisionFlag & CollisionFlag.Self) == CollisionFlag.Self)
                {
                    AddCollidersToDynamicBones(_whitelistedBoneColliders, bonesList);
                }
                if ((Config.GlobalBones.FriendsCollisionFlag & CollisionFlag.Self) == CollisionFlag.Self)
                {
                    AddCollidersToDynamicBones(_friendsDynamicBoneColliders, bonesList);
                }
                if ((Config.GlobalBones.OthersCollisionFlag & CollisionFlag.Self) == CollisionFlag.Self)
                {
                    AddCollidersToDynamicBones(_othersDynamicBoneColliders, bonesList);
                }
            }
            else if (isWhitelisted)
            {
                if ((Config.GlobalBones.OwnCollisionFlag & CollisionFlag.Whitelist) == CollisionFlag.Whitelist)
                {
                    AddCollidersToDynamicBones(_ownDynamicBoneColliders, bonesList);
                }
                if ((Config.GlobalBones.FriendsCollisionFlag & CollisionFlag.Whitelist) == CollisionFlag.Whitelist)
                {
                    AddCollidersToDynamicBones(_friendsDynamicBoneColliders, bonesList);
                }
                if ((Config.GlobalBones.OthersCollisionFlag & CollisionFlag.Whitelist) == CollisionFlag.Whitelist)
                {
                    AddCollidersToDynamicBones(_othersDynamicBoneColliders, bonesList);
                }
            }
            else if (isFriend)
            {
                if ((Config.GlobalBones.OwnCollisionFlag & CollisionFlag.Friends) == CollisionFlag.Friends)
                {
                    AddCollidersToDynamicBones(_ownDynamicBoneColliders, bonesList);
                }
                if ((Config.GlobalBones.WhitelistCollisionFlag & CollisionFlag.Friends) == CollisionFlag.Friends)
                {
                    AddCollidersToDynamicBones(_whitelistedBoneColliders, bonesList);
                }
                if ((Config.GlobalBones.OthersCollisionFlag & CollisionFlag.Friends) == CollisionFlag.Friends)
                {
                    AddCollidersToDynamicBones(_othersDynamicBoneColliders, bonesList);
                }
            }
            else
            {
                if ((Config.GlobalBones.OwnCollisionFlag & CollisionFlag.Others) == CollisionFlag.Others)
                {
                    AddCollidersToDynamicBones(_ownDynamicBoneColliders, bonesList);
                }
                if ((Config.GlobalBones.WhitelistCollisionFlag & CollisionFlag.Others) == CollisionFlag.Others)
                {
                    AddCollidersToDynamicBones(_whitelistedBoneColliders, bonesList);
                }
                if ((Config.GlobalBones.FriendsCollisionFlag & CollisionFlag.Others) == CollisionFlag.Others)
                {
                    AddCollidersToDynamicBones(_friendsDynamicBoneColliders, bonesList);
                }
            }
        }

        private void HandleColliderOption(List<DynamicBoneCollider> list, Animator animator, GameObject avatarObject, ColliderOption option, bool alwaysIncludeHead)
        {
            switch (option)
            {
                case ColliderOption.None:
                    break;
                case ColliderOption.All:
                    {
                        foreach (var collider in avatarObject.GetComponentsInChildren<DynamicBoneCollider>(true))
                        {
                            if (collider.m_Bound == DynamicBoneCollider.DynamicBoneColliderBound.Inside)
                                continue;

                            var radius = collider.m_Radius * Math.Abs(collider.transform.lossyScale.y);
                            if (radius > Config.GlobalBones.MaxRadius)
                                continue;

                            list.Add(collider);
                        }

                        return;
                    }
                case ColliderOption.Hands:
                    AddBoneCollider(list, animator, new List<HumanBodyBones> { HumanBodyBones.LeftHand, HumanBodyBones.RightHand });
                    break;
                case ColliderOption.HandsAndLowerBody:
                    AddBoneCollider(list, animator, new List<HumanBodyBones>
                    {
                        HumanBodyBones.LeftHand,
                        HumanBodyBones.RightHand,
                        HumanBodyBones.LeftFoot,
                        HumanBodyBones.RightFoot,
                        HumanBodyBones.LeftUpperLeg,
                        HumanBodyBones.RightUpperLeg,
                        HumanBodyBones.LeftLowerLeg,
                        HumanBodyBones.RightLowerLeg
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (alwaysIncludeHead)
            {
                AddBoneCollider(list, animator, new List<HumanBodyBones> { HumanBodyBones.Head });
            }
        }

        private void AddBoneCollider(List<DynamicBoneCollider> list, Animator animator, List<HumanBodyBones> bones)
        {
            if (animator == null)
                return;

            if (!animator.isHuman)
                return;

            foreach (var bone in bones)
            {
                var boneTransform = animator.GetBoneTransform(bone);
                if (boneTransform == null)
                    continue;

                foreach (var collider in boneTransform.GetComponentsInChildren<DynamicBoneCollider>(true))
                {
                    if (collider.m_Bound == DynamicBoneCollider.DynamicBoneColliderBound.Inside)
                        continue;

                    var radius = collider.m_Radius * Math.Abs(collider.transform.lossyScale.y);
                    if (radius > Config.GlobalBones.MaxRadius)
                        continue;

                    list.Add(collider);
                }
            }
        }

        private void AddCollidersToDynamicBones(List<DynamicBoneCollider> colliders, List<DynamicBone> bones)
        {
            foreach (var bone in bones.ToList())
            {
                if (bone == null)
                {
                    bones.Remove(bone);
                    continue;
                }

                foreach (var collider in colliders.ToList())
                {
                    if (collider == null)
                    {
                        colliders.Remove(collider);
                        continue;
                    }

                    if (bone.m_Colliders.IndexOf(collider) == -1)
                    {
                        bone.m_Colliders.Add(collider);
                    }
                }
            }
        }

        private void ToggleDynamicBones(bool value)
        {
            Config.GlobalBones.Enabled = value;
            if (Config.GlobalBones.AutoReloadAvatars)
            {
                VRCPlayer.field_Internal_Static_VRCPlayer_0.ReloadAllAvatars();
            }
        }

        private void CycleColliderOption(QMSingleButton button, ref ColliderOption option)
        {
            if (++option > ColliderOption.All)
            {
                option = ColliderOption.None;
            }

            button.SetButtonText($"Colliders: <color=yellow>{option}</color>");
        }
    }
}
