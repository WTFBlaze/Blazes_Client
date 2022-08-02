using Blaze.API.QM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blaze.Modules
{
    public class AvatarIndexer : BModule
    {
        private static QMNestedButton Menu;
        private static QMNestedButton ShadersMenu;
        private static QMNestedButton AudiosMenu;
        private static QMNestedButton SelectedAudioMenu;
        private static QMScrollMenu AudioScroll;
        private static QMScrollMenu ShaderScroll;
        private static List<string> shaders;
        private static AudioSource SelectedSource;
        private static AudioSource LocalAudioSource;
        private static List<AudioSource> SourceList;

        public override void UI()
        {
            // Menus
            Menu = new QMNestedButton(BlazeQM.Selected, "Avatar\nIndexer", 4, 1, "View information about this users avatar (avatar must be shown)", "Avatar Indexer");
            ShadersMenu = new QMNestedButton(Menu, "Shaders", 1, 0, "View The Avatar's Shaders", "Shaders");
            AudiosMenu = new QMNestedButton(Menu, "Audios", 2, 0, "Views The Avatar's Audio Files", "Audios");
            SelectedAudioMenu = new QMNestedButton(AudiosMenu, "", 0, 0, "", "Selected Audio");
            AudioScroll = new QMScrollMenu(AudiosMenu);
            ShaderScroll = new QMScrollMenu(ShadersMenu);
            // Lists
            shaders = new();
            SourceList = new();
            // Local Audio Source
            LocalAudioSource = Main.BlazesComponents.AddComponent<AudioSource>();
            LocalAudioSource.loop = false;
            LocalAudioSource.priority = -1000;
            SelectedAudioMenu.GetMainButton().SetActive(false);

            ShaderScroll.SetAction(delegate
            {
                shaders.Clear();
                Renderer[] array = Main.SelectedPlayer.gameObject.GetComponentsInChildren<Renderer>(true);
                foreach (var t in array)
                {
                    Material[] array2 = t.materials;
                    foreach (var t1 in array2)
                    {
                        if (!shaders.Contains(t1.shader.name))
                        {
                            shaders.Add(t1.shader.name);
                        }
                    }
                }
                foreach (var tmp in shaders)
                {
                    ShaderScroll.Add(new QMSingleButton(ShaderScroll.BaseMenu, 0, 0, ShaderBlacklist.blockList.Contains(tmp) ? $"<color=red>{tmp}</color>" : $"<color=green>{tmp}</color>", delegate
                    {
                        ShaderBlacklist.AddOrRemoveFromList(tmp);
                        ShaderScroll.Refresh();
                    }, "Click to toggle whether this shader is blacklisted or not"));
                }
            });

            AudioScroll.SetAction(delegate
            {
                SourceList.Clear();
                AudioSource[] array = Main.SelectedPlayer.gameObject.GetComponentsInChildren<AudioSource>(true);
                foreach (var source in array)
                {
                    if (source.name is "Speaker" or "USpeak" or "USpeaker") return;
                    if (SourceList.Contains(source)) return;
                    else SourceList.Add(source);
                    AudioScroll.Add(new QMSingleButton(AudioScroll.BaseMenu, 0, 0, source.name, delegate
                    {
                        SelectedSource = source;
                        SelectedAudioMenu.OpenMe();
                    }, "Click to view more options"));
                }
            });

            new QMSingleButton(SelectedAudioMenu, 1, 0, "<color=green>Play Audio</color>", delegate
            {
                LocalAudioSource.clip = SelectedSource.clip;
                LocalAudioSource.Play();
            }, "Locally plays the audio clip");

            new QMSingleButton(SelectedAudioMenu, 2, 0, "<color=red>Stop Audio</color>", delegate
            {
                LocalAudioSource.Stop();
                LocalAudioSource.clip = null;
            }, "Stops playing the audio clip if its playing");
        }
    }
}
