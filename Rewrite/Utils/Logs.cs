using Blaze.Modules;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blaze.Utils
{
    public static class Logs
    {
        private static string lastMsg = string.Empty;
        private static int duplicateCount = 1;
        public static List<string> TotalLogs = new();
        private static List<string> lines = new();

        public static void Log(string message) => HandleLog(message, ConsoleColor.Gray);
        public static void Log(string message, ConsoleColor color) => HandleLog(message, color);
        public static void Error(string itemName, Exception errorMessage) => HandleLog($"[Error Item: {itemName}] Error Message:\n{errorMessage.Message}", ConsoleColor.Red);
        public static void Error(string message) => HandleLog(message, ConsoleColor.Red);
        public static void Warning(string message) => HandleLog(message, ConsoleColor.Yellow);
        public static void Success(string message) => HandleLog(message, ConsoleColor.Green);
        public static void Debug(string message) => HandleDebug(message);
        public static void RawHUD(string message, float duration) => MelonCoroutines.Start(HandleHud(message, duration));
        public static void HUD(string message, float duration) => MelonCoroutines.Start(HandleHud($"[<color={Colors.AquaHex}>B</color><color={Colors.MagentaHex}>C</color>]: " + message, duration));
        public static void ClearHUD()
        {
            try
            {
                lines.Clear();
                BlazeQM.hudLog.text = "";
            }
            catch { }
        }

        private static void HandleLog(string message, ConsoleColor color)
        {
            var time = DateTime.Now.ToString("HH:mm:ss.fff");
            WriteConsolePrefix(time, ConsoleColor.Cyan);
            WriteConsolePrefix("Blaze's ", ConsoleColor.Cyan, "Client", ConsoleColor.Magenta);
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
            //ApolloUtils.AppendLineToFile(FileManager.LatestLogFile, $"[{time}] {message}\n");
            //ApolloUtils.AppendLineToFile(LogFileName, $"[{time}] {message}\n");
        }

        private static void WriteConsolePrefix(string text, ConsoleColor color)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write('[');
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
            Console.ResetColor();
        }

        private static void WriteConsolePrefix(string text, ConsoleColor color, string text2, ConsoleColor color2)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write('[');
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = color2;
            Console.Write(text2);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
            Console.ResetColor();
        }

        private static void HandleDebug(string message)
        {
            if (BlazeQM.DebugPanel == null) return;
            if (message == lastMsg)
            {
                TotalLogs.RemoveAt(TotalLogs.Count - 1);
                duplicateCount++;
                TotalLogs.Add($"[<color={Colors.MagentaHex}>{DateTime.Now:hh:mm tt}</color>] {message} <color=red><i>x{duplicateCount}</i></color>");
            }
            else
            {
                lastMsg = message;
                duplicateCount = 1;
                TotalLogs.Add($"[<color={Colors.MagentaHex}>{DateTime.Now:hh:mm tt}</color>] {message}");
                if (TotalLogs.Count == 22)
                {
                    TotalLogs.RemoveAt(0);
                }
            }
            BlazeQM.DebugPanel.SetText(string.Join("\n", TotalLogs.Take(22)));
            //if (BlazeInfo.BlazesComponents.GetComponent<BlazesIMGUIDebug>() != null)
            //    BlazesIMGUIDebug.DebugPrint(message);
        }

        private static IEnumerator HandleHud(string text, float duration)
        {
            if (BlazeQM.hudLog is null)
            {
                Log("hudlog is null!");
                yield break;
            }
            lines.Add(text);
            BlazeQM.hudLog.text = string.Join("\n", lines);
            yield return new WaitForSecondsRealtime(duration);
            lines.Remove(text);
            BlazeQM.hudLog.text = string.Join("\n", lines);
        }
    }
}
