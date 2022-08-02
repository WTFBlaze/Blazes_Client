using Blaze.Utils;
using Blaze.Utils.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Blaze.Modules.GlobalBones;

namespace Blaze.Configs
{
    class BonesConfig
    {
        public CollisionFlag OwnCollisionFlag
        {
            get => _ownCollisionFlag;
            set
            {
                _ownCollisionFlag = value;
            }
        }

        public CollisionFlag WhitelistCollisionFlag
        {
            get => _whitelistCollisionFlag;
            set
            {
                _whitelistCollisionFlag = value;
            }
        }

        public CollisionFlag FriendsCollisionFlag
        {
            get => _friendsCollisionFlag;
            set
            {
                _friendsCollisionFlag = value;
            }
        }

        public CollisionFlag OthersCollisionFlag
        {
            get => _othersCollisionFlag;
            set
            {
                _othersCollisionFlag = value;
            }
        }

        public ColliderOption OwnColliderOption
        {
            get => _ownColliderOption;
            set
            {
                _ownColliderOption = value;
            }
        }

        public ColliderOption WhitelistColliderOption
        {
            get => _whitelistColliderOption;
            set
            {
                _whitelistColliderOption = value;
            }
        }

        public ColliderOption FriendsColliderOption
        {
            get => _friendsColliderOption;
            set
            {
                _friendsColliderOption = value;
            }
        }

        public ColliderOption OthersColliderOption
        {
            get => _othersColliderOption;
            set
            {
                _othersColliderOption = value;
            }
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
            }
        }

        public bool AutoReloadAvatars
        {
            get => _autoReloadAvatars;
            set
            {
                _autoReloadAvatars = value;
            }
        }

        public float MaxRadius
        {
            get => _maxRadius;
            set
            {
                _maxRadius = value;
            }
        }

        public List<string> WhitelistedUsers => _whitelistedUsers;

        public bool AlwaysIncludeOwnHead
        {
            get => _alwaysIncludeOwnHead;
            set
            {
                _alwaysIncludeOwnHead = value;
            }
        }

        public bool AlwaysIncludeWhitelistedHead
        {
            get => _alwaysIncludeWhitelistedHead;
            set
            {
                _alwaysIncludeWhitelistedHead = value;
            }
        }

        public bool AlwaysIncludeFriendsHead
        {
            get => _alwaysIncludeFriendsHead;
            set
            {
                _alwaysIncludeFriendsHead = value;
            }
        }

        public bool AlwaysIncludeOthersHead
        {
            get => _alwaysIncludeOthersHead;
            set
            {
                _alwaysIncludeOthersHead = value;
            }
        }

        public void WhitelistUser(string userId)
        {
            if (!IsWhitelisted(userId))
            {
                _whitelistedUsers.Add(userId);
                Logs.Log($"[GlobalBones] Added {userId} to whitelist!", ConsoleColor.Yellow);
                Logs.Debug($"[<color=yellow>GlobalBones</color>] <color=green>Added</color> <color=yellow>{userId}</color> to whitelist!");
                Save();
            }
        }

        public void RemoveWhitelist(string userId)
        {
            if (_whitelistedUsers.Remove(userId))
            {
                Logs.Log($"[GlobalBones] Removed {userId} to whitelist!", ConsoleColor.Yellow);
                Logs.Debug($"[<color=yellow>GlobalBones</color>] <color=red>Removed</color> <color=yellow>{userId}</color> to whitelist!");
                Save();
            }
        }

        public bool IsWhitelisted(string userId)
        {
            return _whitelistedUsers.Contains(userId);
        }


        private bool _enabled = false;
        private float _maxRadius = 1f;
        private bool _autoReloadAvatars = true;
        private List<string> _whitelistedUsers = new()
        {
            "usr_00000000-0000-0000-0000-000000000000"
        };

        private CollisionFlag _ownCollisionFlag = CollisionFlag.Self | CollisionFlag.Friends | CollisionFlag.Whitelist;
        private CollisionFlag _whitelistCollisionFlag = CollisionFlag.Self;
        private CollisionFlag _friendsCollisionFlag = CollisionFlag.Self | CollisionFlag.Friends;
        private CollisionFlag _othersCollisionFlag = CollisionFlag.Self;

        private ColliderOption _ownColliderOption = ColliderOption.HandsAndLowerBody;
        private ColliderOption _whitelistColliderOption = ColliderOption.HandsAndLowerBody;
        private ColliderOption _friendsColliderOption = ColliderOption.HandsAndLowerBody;
        private ColliderOption _othersColliderOption = ColliderOption.Hands;

        private bool _alwaysIncludeOwnHead = false;
        private bool _alwaysIncludeWhitelistedHead = false;
        private bool _alwaysIncludeFriendsHead = false;
        private bool _alwaysIncludeOthersHead = false;

        public static BonesConfig Instance;
        public static void Load()
        {
            if (!File.Exists(ModFiles.DynamicBonesFile))
            {
                JsonManager.WriteToJsonFile(ModFiles.DynamicBonesFile, new BonesConfig());
            }
            Instance = JsonManager.ReadFromJsonFile<BonesConfig>(ModFiles.DynamicBonesFile);
        }

        public void Save()
        {
            JsonManager.WriteToJsonFile(ModFiles.DynamicBonesFile, Instance);
        }
    }
}
