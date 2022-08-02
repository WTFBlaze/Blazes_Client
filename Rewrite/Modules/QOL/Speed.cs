/*using Blaze.API.QM;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaze.Modules
{
    public class Speed : BModule
    {
        private QMNestedButton menu;
        private QMSingleButton Label;
        private float defaultWalk;
        private float defaultSprint;
        private float defaultStrafe;
        private float newWalk;
        private float newSprint;
        private float newStrafe;

        public override void UI()
        {
            menu = new QMNestedButton(BlazeQM.Movement, "Speed", 2, 1, "Player Speed Settings", "Speed");

        }

        public override void LocalPlayerLoaded()
        {
            defaultWalk = PlayerUtils.CurrentUser().GetVRCPlayerApi().GetWalkSpeed();
            defaultSprint = PlayerUtils.CurrentUser().GetVRCPlayerApi().GetRunSpeed();
            defaultStrafe = PlayerUtils.CurrentUser().GetVRCPlayerApi().GetStrafeSpeed();

        }
    }
}
*/