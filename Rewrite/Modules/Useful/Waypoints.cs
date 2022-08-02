using Blaze.API.QM;
using Blaze.Utils;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Blaze.Config.WaypointsConfig;

namespace Blaze.Modules
{
    public class Waypoints : BModule
    {
        private QMNestedButton Menu;
        private QMScrollMenu Scroll;
        private bool DeleteMode;

        public override void UI()
        {
            Menu = new QMNestedButton(BlazeQM.Self, "World\nWaypoints", 3, 0, "Click to view or add world specific waypoints", "World Waypoints");
            Scroll = new QMScrollMenu(Menu);

            new QMSingleButton(Menu, 4, 0, "Add\nWaypoint", delegate
            {
                if (Config.Waypoints.list.Exists(x => x.WorldID == WorldUtils.CurrentWorld().id))
                {
                    var world = Config.Waypoints.list.Find(x => x.WorldID == WorldUtils.CurrentWorld().id);
                    var point = PlayerUtils.CurrentUser().transform.position;
                    world.Waypoints.Add(new WaypointObject
                    {
                        X = point.x,
                        Y = point.y,
                        Z = point.z
                    });
                    Config.Waypoints.Save();
                    Scroll.Refresh();
                }
                else
                {
                    var world = new WaypointWorldObject
                    {
                        WorldID = WorldUtils.CurrentWorld().id,
                        Waypoints = new List<WaypointObject>()
                    };
                    var point = PlayerUtils.CurrentUser().transform.position;
                    world.Waypoints.Add(new WaypointObject
                    {
                        X = point.x,
                        Y = point.y,
                        Z = point.z
                    });
                    Config.Waypoints.list.Add(world);
                    Config.Waypoints.Save();
                    Scroll.Refresh();
                }
            }, "Click to add where you are currently standing as a new waypoint");

            new QMToggleButton(Menu, 4, 3, "Deletion Mode", delegate
            {
                DeleteMode = true;
            }, delegate
            {
                DeleteMode = false;
            }, "Toggles deleting waypoints on click instead of teleporting to them");

            Scroll.SetAction(delegate
            {
                if (Config.Waypoints.list.Exists(x => x.WorldID == WorldUtils.CurrentWorld().id))
                {
                    var world = Config.Waypoints.list.Find(x => x.WorldID == WorldUtils.CurrentWorld().id);
                    foreach (var w in world.Waypoints)
                    {
                        Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, $"X: {w.X}\nY: {w.Y}\nZ: {w.Z}", delegate
                        {
                            if (DeleteMode)
                            {
                                world.Waypoints.Remove(w);
                                Config.Waypoints.Save();
                            }
                            else
                            {
                                PlayerUtils.CurrentUser().transform.position = new Vector3(w.X, w.Y, w.Z);
                                Logs.Debug($"<color=#649e91>[Waypoints]</color> Teleported to: (<color=yellow>{w.X}</color>, <color=yellow>{w.Y}</color>, <color=yellow>{w.Z}</color>)");
                                Logs.Log($"[Waypoints] Telepoted to: ({w.X}, {w.Y}, {w.Z})", ConsoleColor.Green);
                            }
                        }, DeleteMode ? "Click to <color=red>DELETE</color> this waypoint" : "Click to teleport to this waypoint"));
                    }
                }
            });
        }
    }
}
