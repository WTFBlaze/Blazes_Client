using Blaze.API.QM;
using Blaze.Utils;

namespace Blaze.Modules
{
    public class PickupsList : BModule
    {
        public QMNestedButton Menu;
        public QMScrollMenu Scroll;

        public override void UI()
        {
            Menu = new QMNestedButton(BlazeQM.Worlds, "Pickups\nList", 1, 2, "View all pickups in the world and teleport selected ones to your right hand", "Pickups List");
            Scroll = new QMScrollMenu(Menu);

            Scroll.SetAction(delegate
            {
                foreach (var p in Main.Pickups)
                {
                    Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, p.gameObject.name, delegate
                    {
                        Functions.TeleportObjectToRightHand(p.gameObject);
                    }, "Click to teleport this pickup to your right hand"));
                }
            });
        }
    }
}
