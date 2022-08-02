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

namespace Blaze.Modules
{
    class Waypoints : BModule
    {
        private QMNestedButton Menu;
        private QMScrollMenu Scroll;

        public override void QuickMenuUI()
        {
            Menu = new QMNestedButton(BlazeMenu.Worlds, "World\nWaypoints", 3, 3, "Click to view or add world specific waypoints", "World Waypoints");
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

            Scroll.SetAction(delegate
            {
                if (Config.Waypoints.list.Exists(x => x.WorldID == WorldUtils.CurrentWorld().id))
                {
                    var world = Config.Waypoints.list.Find(x => x.WorldID == WorldUtils.CurrentWorld().id);
                    foreach (var w in world.Waypoints)
                    {
                        Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, $"X: {w.X}\nY: {w.Y}\nZ: {w.Z}", delegate
                        {
                            PlayerUtils.CurrentUser().transform.position = new Vector3(w.X, w.Y, w.Z);
                            Logs.Debug($"<color=#649e91>[Waypoints]</color> Teleported to: (<color=yellow>{w.X}</color>, <color=yellow>{w.Y}</color>, <color=yellow>{w.Z}</color>)");
                            Logs.Log($"[Waypoints] Telepoted to: ({w.X}, {w.Y}, {w.Z})", ConsoleColor.Green);
                        }, "Click to teleport to this waypoint"));
                    }
                }
            });
        }
    }
}
