using Blaze.API.QM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaze.Modules
{
    public class BlazeStaff : BModule
    {
        private QMNestedButton Menu;
        private QMNestedButton OnlineList;
        public static QMScrollMenu OnlineScroll;

        public override void UI()
        {
            if (Main.CurrentUser.AccessType != "Developer") return;
            Menu = new QMNestedButton(BlazeQM.MainMenu, "Staff", 3, 2, "Blaze's Client Staff Menu", "Staff Menu");

            OnlineList = new QMNestedButton(Menu, "View\nOnline\nUsers", 1, 0, "Click to view all online Blaze's Client Users", "Online Users");
            OnlineScroll = new QMScrollMenu(OnlineList);
            OnlineScroll.SetAction(delegate
            {
                BlazeNetwork.ws.SendAsync(JsonConvert.SerializeObject(new 
                {
                    payload = new
                    {
                        type = "FetchOnline"
                    }
                }), null);
            });
        }
    }
}
