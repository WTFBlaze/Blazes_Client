using Blaze.API;
using Blaze.API.QM;
using Blaze.Utils;
using Blaze.Utils.Managers;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Blaze.Modules
{
    public class EmojiSpammer : BModule
    {
        public static QMNestedButton Menu;
        public static QMToggleButton ToggleButton;
        public static List<int> selectedEmojis = new();

        public override void Start()
        {
            ComponentManager.RegisterIl2Cpp<BlazeEmojiSpammer>();
        }

        public override void UI()
        {
            Menu = new QMNestedButton(BlazeQM.Exploits, "Emoji\nSpammer", 3, 2, "Click to view options for emoji spammer", "Emoji Spammer");

            ToggleButton = new QMToggleButton(Menu, 1, 0, "Emoji Spammer", delegate
            {
                Main.BlazesComponents.AddComponent<BlazeEmojiSpammer>();
            }, delegate
            {
                UnityEngine.Object.Destroy(Main.BlazesComponents.GetComponent<BlazeEmojiSpammer>());
            }, "Toggle spamming emojis");

            new QMToggleButton(Menu, 2, 0, "Laughing", delegate
            {
                Config.Main.EmojiLaughing = true;
                selectedEmojis.Add(9);
            }, delegate
            {
                Config.Main.EmojiLaughing = false;
                selectedEmojis.Remove(9);
            }, "Enables the laughing emoji to be used in the spammer", Config.Main.EmojiLaughing);
            if (Config.Main.EmojiLaughing) selectedEmojis.Add(9);

            new QMToggleButton(Menu, 3, 0, "Smiles", delegate
            {
                Config.Main.EmojiSmiles = true;
                selectedEmojis.Add(11);
            }, delegate
            {
                Config.Main.EmojiSmiles = false;
                selectedEmojis.Remove(11);
            }, "Enable the smiles emoji to be used in the spammer", Config.Main.EmojiSmiles);
            if (Config.Main.EmojiSmiles) selectedEmojis.Add(11);

            new QMToggleButton(Menu, 4, 0, "Ghosts", delegate
            {
                Config.Main.EmojiGhosts = true;
                selectedEmojis.Add(12);
            }, delegate
            {
                Config.Main.EmojiGhosts = false;
                selectedEmojis.Remove(12);
            }, "Enable the ghosts emoji to be used in the spammer", Config.Main.EmojiGhosts);
            if (Config.Main.EmojiGhosts) selectedEmojis.Add(12);

            new QMToggleButton(Menu, 1, 1, "Thumbs Up", delegate
            {
                Config.Main.EmojiThumbsUp = true;
                selectedEmojis.Add(17);
            }, delegate
            {
                Config.Main.EmojiThumbsUp = false;
                selectedEmojis.Remove(17);
            }, "Enable the thumbs up emoji to be used in the spammer", Config.Main.EmojiThumbsUp);
            if (Config.Main.EmojiThumbsUp) selectedEmojis.Add(17);

            new QMToggleButton(Menu, 2, 1, "Thumbs Down", delegate
            {
                Config.Main.EmojiThumbsDown = true;
                selectedEmojis.Add(16);
            }, delegate
            {
                Config.Main.EmojiThumbsDown = false;
                selectedEmojis.Remove(16);
            }, "Enable the thumbs down emoji to be used in the spammer", Config.Main.EmojiThumbsDown);
            if (Config.Main.EmojiThumbsDown) selectedEmojis.Add(16);

            new QMToggleButton(Menu, 3, 1, "Clouds", delegate
            {
                Config.Main.EmojiClouds = true;
                selectedEmojis.Add(21);
            }, delegate
            {
                Config.Main.EmojiClouds = false;
                selectedEmojis.Remove(21);
            }, "Enable the clouds emoji to be used in the spammer", Config.Main.EmojiClouds);
            if (Config.Main.EmojiClouds) selectedEmojis.Add(21);

            new QMToggleButton(Menu, 4, 1, "Snowflakes", delegate
            {
                Config.Main.EmojiSnowflakes = true;
                selectedEmojis.Add(23);
            }, delegate
            {
                Config.Main.EmojiSnowflakes = false;
                selectedEmojis.Remove(23);
            }, "Enable the snowflakes emoji to be used in the spammer", Config.Main.EmojiSnowflakes);
            if (Config.Main.EmojiSnowflakes) selectedEmojis.Add(23);

            new QMToggleButton(Menu, 1, 2, "Candy", delegate
            {
                Config.Main.EmojiCandy = true;
                selectedEmojis.Add(25);
            }, delegate
            {
                Config.Main.EmojiCandy = false;
                selectedEmojis.Remove(25);
            }, "Enable the candy emoji to be used in the spammer", Config.Main.EmojiCandy);
            if (Config.Main.EmojiCandy) selectedEmojis.Add(25);

            new QMToggleButton(Menu, 2, 2, "Candy Corn", delegate
            {
                Config.Main.EmojiCandyCorn = true;
                selectedEmojis.Add(30);
            }, delegate
            {
                Config.Main.EmojiCandyCorn = false;
                selectedEmojis.Remove(30);
            }, "Enable the candy corn emoji to be used in the spammer", Config.Main.EmojiCandyCorn);
            if (Config.Main.EmojiCandyCorn) selectedEmojis.Add(30);

            new QMToggleButton(Menu, 3, 2, "Pizza", delegate
            {
                Config.Main.EmojiPizza = true;
                selectedEmojis.Add(36);
            }, delegate
            {
                Config.Main.EmojiPizza = false;
                selectedEmojis.Remove(36);
            }, "Enable the pizza emoji to be used in the spammer", Config.Main.EmojiPizza);
            if (Config.Main.EmojiPizza) selectedEmojis.Add(36);

            new QMToggleButton(Menu, 4, 2, "Confetti", delegate
            {
                Config.Main.EmojiConfetti = true;
                selectedEmojis.Add(40);
            }, delegate
            {
                Config.Main.EmojiConfetti = false;
                selectedEmojis.Remove(40);
            }, "Enable the confetti emoji to be used in the spammer", Config.Main.EmojiConfetti);
            if (Config.Main.EmojiConfetti) selectedEmojis.Add(40);

            new QMToggleButton(Menu, 1, 3, "Gifts", delegate
            {
                Config.Main.EmojiGifts = true;
                selectedEmojis.Add(42);
            }, delegate
            {
                Config.Main.EmojiGifts = false;
                selectedEmojis.Remove(42);
            }, "Enable the gifts emoji to be used in the spammer", Config.Main.EmojiGifts);
            if (Config.Main.EmojiGifts) selectedEmojis.Add(42);

            new QMToggleButton(Menu, 2, 3, "Money", delegate
            {
                Config.Main.EmojiMoney = true;
                selectedEmojis.Add(45);
            }, delegate
            {
                Config.Main.EmojiMoney = false;
                selectedEmojis.Remove(45);
            }, "Enable the money emoji to be used in the spammer", Config.Main.EmojiMoney);
            if (Config.Main.EmojiMoney) selectedEmojis.Add(45);

            new QMToggleButton(Menu, 3, 3, "ZZZ", delegate
            {
                Config.Main.EmojiZ = true;
                selectedEmojis.Add(56);
            }, delegate
            {
                Config.Main.EmojiZ = false;
                selectedEmojis.Remove(56);
            }, "Enable the z emoji to be used in the spammer", Config.Main.EmojiZ);
            if (Config.Main.EmojiZ) selectedEmojis.Add(56);

            new QMToggleButton(Menu, 4, 3, "Pineapple", delegate
            {
                Config.Main.EmojiPineapple = true;
                selectedEmojis.Add(35);
            }, delegate
            {
                Config.Main.EmojiPineapple = false;
                selectedEmojis.Remove(35);
            }, "Enable the pineapple emoji to be used in the spammer", Config.Main.EmojiPineapple);
            if (Config.Main.EmojiPineapple) selectedEmojis.Add(35);
        }
    }

    public class BlazeEmojiSpammer : MonoBehaviour
    {
        public BlazeEmojiSpammer(IntPtr id) : base(id) { }
        public static float targetTime = 1f;

        public void Awake() => targetTime = 0;

        public void Update()
        {
            targetTime -= Time.deltaTime;
            if (targetTime <= 0)
            {
                if (WorldUtils.IsInRoom() && EmojiSpammer.selectedEmojis.Count > 0)
                {
                    Functions.EmojiRPC(EmojiSpammer.selectedEmojis[APIStuff.rnd.Next(0, EmojiSpammer.selectedEmojis.Count)]);
                }
                targetTime = 1f;
            }
        }

        public void OnDestroy()
        {
            EmojiSpammer.ToggleButton.SetToggleState(false, true);
        }
    }
}
