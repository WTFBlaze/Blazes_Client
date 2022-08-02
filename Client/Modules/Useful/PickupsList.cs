using Blaze.API.QM;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaze.Modules
{
    class PickupsList : BModule
    {
        private QMNestedButton Menu;
        private QMScrollMenu Scroll;

        public override void QuickMenuUI()
        {
            Menu = new QMNestedButton(BlazeMenu.Worlds, "Pickups\nList", 2, 3, "Click to view all pickups in this world", "Pickups List");
            Scroll = new QMScrollMenu(Menu);

            Scroll.SetAction(delegate
            {
                if (WorldUtils.GetPickups().Length == 0)
                {
                    Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, "<color=red>0 Pickups!</color>", delegate { }, "There are no pickups in this world!"));
                }
                else
                {
                    foreach (var p in WorldUtils.GetPickups())
                    {
                        Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, p.name, delegate
                        {
                            p.gameObject.transform.position = PlayerUtils.CurrentUser().GetVRCPlayerApi().GetBonePosition(UnityEngine.HumanBodyBones.RightHand);
                        }, "Click to teleport this pickup to your right hand!"));
                    }
                }
            });
        }
    }
}
