using Blaze.API.QM;
using Blaze.Utils;
using Blaze.Utils.VRChat;

namespace Blaze.Modules
{
    public class SelectUser : BModule
    {
        public QMNestedButton Menu;
        public QMScrollMenu Scroll;

        public override void UI()
        {
            Menu = new QMNestedButton(BlazeQM.MainMenu, "Select\nUser", 4, 2, "View all users in the world and select them in your quickmenu", "Select User");
            Scroll = new QMScrollMenu(Menu);
            Scroll.SetAction(delegate
            {
                foreach (var p in PhotonUtils.GetAllPhotonPlayers())
                {
                    if (p.GetPlayer() == null)
                    {
                        Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, $"<color=red><b>[INVIS]</b></color>\n{p.GetDisplayName()}", delegate
                        {
                            PopupUtils.AskConfirmOpenURL("https://vrchat.com/home/user/" + p.GetUserID(), "VRChat");
                        }, "Click to select this user!"));
                    }
                    else
                    {
                        Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, $"<color={p.GetPlayer().GetAPIUser().GetTrueRankColor()}>{p.GetDisplayName()}</color>", delegate
                        {
                            Functions.SelectPlayer(p.GetPlayer());
                        }, "Click to select this user!"));
                    }
                }
            });
        }
    }
}
