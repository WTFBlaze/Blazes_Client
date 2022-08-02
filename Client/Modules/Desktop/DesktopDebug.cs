using Blaze.API.QM;
using Blaze.Configs;
using Blaze.Utils;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.XR;
using VRC.SDKBase;

namespace Blaze.Modules
{
    class DesktopDebug : BModule
    {
        public override void Start()
        {
            ClassInjector.RegisterTypeInIl2Cpp<BlazesIMGUIDebug>();
        }

        public override void QuickMenuUI()
        {
            new QMToggleButton(BlazeMenu.BCUI, 2, 1, "Desktop Debug", delegate
            {
                Config.Main.DesktopDebug = true;
            }, delegate
            {
                Config.Main.DesktopDebug = false;
            }, "Toggle the On Screen Debug Panel", Config.Main.DesktopDebug);

            if (XRDevice.isPresent) return;
            BlazeInfo.BlazesComponents.AddComponent<BlazesIMGUIDebug>();
        }
    }

    public class BlazesIMGUIDebug : MonoBehaviour
    {
        public BlazesIMGUIDebug(IntPtr id) : base(id) { }
        private struct DebugLog { public string message; }
        public static GUIStyle DBStylePanel;
        public static GUIStyle DBStyleLog;
        public static Rect dbRect = new Rect(Screen.width - 505, Screen.height - 404, 500f, 400f);
        public static Vector2 dbScrollPos;
        private static List<DebugLog> dbLogs = new List<DebugLog>();
        private static string dbLastMessage;
        private static int dbDuplicateCount = 1;
        private static Texture2D dbBackground = null;

        public void Start()
        {
            dbBackground = ImageManager.ColorToTexture(new Color(0, 0, 0, 0.6f));
            DBStylePanel = new GUIStyle();
            DBStylePanel.normal.background = dbBackground;
            DBStylePanel.wordWrap = true;
            DBStylePanel.padding.top = 6;
            DBStylePanel.normal.textColor = Color.white;
            DBStylePanel.fontSize = 15;
            DBStylePanel.alignment = TextAnchor.UpperCenter;
            
        }

        public void OnGUI()
        {
            try
            {
                if (Config.Main.DesktopDebug && !BlazeInfo.SMIsOpened)
                {
                    DBStyleLog = new GUIStyle(GUI.skin.box)
                    {
                        alignment = TextAnchor.UpperLeft,
                        wordWrap = true
                    };
                    DBStyleLog.normal.textColor = Color.white;
                    dbRect = GUILayout.Window(123456, dbRect, (GUI.WindowFunction)DebugWindow, $"<b><color={BlazeInfo.ModColor1}>Blaze's</color> <color={BlazeInfo.ModColor2}>Client</color> <color=red>Debug <3</color></b>", DBStylePanel, new GUILayoutOption[0]);
                }

                /*GUI.Label(new Rect(Screen.width - 130f, 10f, 500f, 500f), string.Concat(new object[]
                {
                    string.Format($"<b><color={BlazeInfo.ModColor1}>Blaze's</color> <color={BlazeInfo.ModColor2}>Client</color></b>"),
                    string.Format("\n<b>In Room: {0}</b>", WorldUtils.GetPlayerCount()),
                    string.Format("\n<b>X: {0}</b>", PlayerUtils.CurrentUser().transform.position.x),
                    string.Format("\n<b>Y: {0}</b>", PlayerUtils.CurrentUser().transform.position.y),
                    string.Format("\n<b>Z: {0}</b>", PlayerUtils.CurrentUser().transform.position.z),
                }), GUI.skin.label);*/
            }
            catch {}
        }

        [HideFromIl2Cpp]
        private void DebugWindow(int windowID)
        {
            GUILayout.Label("", new GUILayoutOption[0]);
            dbScrollPos = GUILayout.BeginScrollView(dbScrollPos, new GUILayoutOption[0]);
            for (int i = 0; i < dbLogs.Count; i++)
            {
                DebugLog debugLog = dbLogs[i];
                GUI.contentColor = Color.white;
                GUILayout.Box(debugLog.message, DBStyleLog, new GUILayoutOption[0]);
            }
            GUILayout.EndScrollView();
            GUI.contentColor = Color.white;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
        }

        [HideFromIl2Cpp]
        public static void DebugPrint(string msg)
        {
            if (dbLastMessage == msg)
            {
                dbLogs.RemoveAt(dbLogs.Count - 1);
                dbDuplicateCount++;
                DebugLog item;
                item.message = $"<b>[<color=#34eba8>{DateTime.Now.ToString("hh:mm tt")}</color>] {msg} <color=red><i>x{dbDuplicateCount}</i></color></b>";
                dbLogs.Add(item);
                dbScrollPos.y = dbScrollPos.y + 10000f;
            }
            else
            {
                dbLastMessage = msg;
                DebugLog item;
                item.message = $"<b>[<color=#34eba8>{DateTime.Now:hh:mm tt}</color>] {msg}</b>";
                dbLogs.Add(item);
                dbScrollPos.y = dbScrollPos.y + 10000f;
                if (dbLogs.Count > 100)
                {
                    dbLogs.RemoveAt(0);
                }
            }
        }
    }
}
