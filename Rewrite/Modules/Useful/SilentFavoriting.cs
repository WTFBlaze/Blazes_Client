using Blaze.API.QM;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaze.Modules
{
    public class SilentFavoriting : BModule
    {
        private QMNestedButton Menu;
        private QMScrollMenu Scroll;

        public override void UI()
        {
            Menu = new QMNestedButton(BlazeQM.Selected, "Silent\nFavorite", 2, 2, "Silently favorite this user's avatar to your favorite lists if its public", "Silent Favorites");
            Scroll = new QMScrollMenu(Menu);
            Scroll.SetAction(delegate
            {
                var avi = Main.SelectedPlayer.prop_ApiAvatar_0;
                if (avi.releaseStatus == "private")
                {
                    Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, "Avi Is\n<color=red>Private</color>!", delegate { }, "You cannot add a private avatar to your favorite lists!"));
                }
                else
                {
                    if (Config.AviFavs.AvatarFavorites.FavoriteLists.Count == 0)
                    {
                        Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, "<color=red>You Have\nNo Lists!</color>", delegate { }, "You do not have any avatar lists to add this to!"));
                    }
                    else
                    {
                        foreach (var l in Config.AviFavs.AvatarFavorites.FavoriteLists)
                        {
                            if (l.Avatars.Exists(x => x.ID == avi.id))
                            {
                                Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, "<color=yellow>Avi Exists\nIn List</color>", delegate { }, "This avatar is already added to this favorite list"!));
                            }
                            else
                            {
                                Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, l.name, delegate
                                {
                                    try
                                    {
                                        AvatarFavorites.FavoriteAvatar(avi, l.ID);
                                        Scroll.Refresh();
                                        PopupUtils.InformationAlert($"Successfully added {avi.name} to {l.name}!");
                                    }
                                    catch
                                    {
                                        PopupUtils.InformationAlert("Failed to add avatar to favorites!");
                                    }
                                }, "Click to add this avatar to your favorites"));
                            }
                        }
                    }
                }
            });
        }
    }
}
