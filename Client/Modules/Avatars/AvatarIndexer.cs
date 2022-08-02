using Blaze.API.QM;
using Blaze.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blaze.Modules
{
    class AvatarIndexer : BModule
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

        public override void QuickMenuUI()
        {
            // Menus
            Menu = new QMNestedButton(BlazeMenu.Selected, "Avatar\nIndexer", 2, 1, "View information about this users avatar (avatar must be shown)", "Avatar Indexer");
            ShadersMenu = new QMNestedButton(Menu, "Shaders", 1, 0, "View The Avatar's Shaders", "Shaders");
            AudiosMenu = new QMNestedButton(Menu, "Audios", 2, 0, "Views The Avatar's Audio Files", "Audios");
            SelectedAudioMenu = new QMNestedButton(AudiosMenu, "", 0, 0, "", "Selected Audio");
            AudioScroll = new QMScrollMenu(AudiosMenu);
            ShaderScroll = new QMScrollMenu(ShadersMenu);
            // Lists
            shaders = new();
            SourceList = new();
            // Local Audio Source
            LocalAudioSource = BlazeInfo.BlazesComponents.AddComponent<AudioSource>();
            LocalAudioSource.loop = false;
            LocalAudioSource.priority = -1000;

            SelectedAudioMenu.GetMainButton().SetActive(false);
            /*ShadersMenu.GetMainButton().SetAction(delegate
            {
                foreach (var label in shaderLabels)
                {
                    label.DestroyMe();
                }
                shaderLabels.Clear();
                shaders.Clear();
                Renderer[] array = BlazeInfo.SelectedPlayer.gameObject.GetComponentsInChildren<Renderer>(true);
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
                    var tmpLabel = new QMLabel(ShadersMenu.GetMenuObject().transform.Find("ScrollRect/Viewport/VerticalLayoutGroup"), 0, 0, tmp, Config.ShaderBlacklist.list.Contains(tmp) ? Color.red : Color.green);
                    var click = tmpLabel.GetGameObject().AddComponent<UnityEngine.UI.Button>();
                    click.onClick.AddListener(new Action(() => 
                    {
                        if (Config.ShaderBlacklist.list.Contains(tmpLabel.GetText().text))
                        {
                            Config.ShaderBlacklist.list.Remove(tmpLabel.GetText().text);
                            Config.ShaderBlacklist.Save();
                            tmpLabel.GetText().color = Color.green;
                        }
                        else
                        {
                            Config.ShaderBlacklist.list.Add(tmpLabel.GetText().text);
                            Config.ShaderBlacklist.Save();
                            tmpLabel.GetText().color = Color.red;
                        }
                    }));
                    shaderLabels.Add(tmpLabel);
                }
                *//*foreach (var tmp in shaders.Select(s => new QMLabel(ShadersMenu.GetMenuObject().transform.Find("ScrollRect/Viewport/VerticalLayoutGroup"), 0, 0, s, Color.white)))
                {
                    shaderLabels.Add(tmp);
                }*//*
                ShadersMenu.OpenMe();
            });*/

            ShaderScroll.SetAction(delegate
            {
                shaders.Clear();
                Renderer[] array = BlazeInfo.SelectedPlayer.gameObject.GetComponentsInChildren<Renderer>(true);
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
                    ShaderScroll.Add(new QMSingleButton(ShaderScroll.BaseMenu, 0, 0, BlacklistedShaders.blockList.Contains(tmp) ? $"<color=red>{tmp}</color>" : $"<color=green>{tmp}</color>", delegate
                    {
                        BlacklistedShaders.AddOrRemoveFromList(tmp);
                        ShaderScroll.Refresh();
                    }, "Click to toggle whether this shader is blacklisted or not"));
                }
            });

            AudioScroll.SetAction(delegate
            {
                SourceList.Clear();
                AudioSource[] array = BlazeInfo.SelectedPlayer.gameObject.GetComponentsInChildren<AudioSource>(true);
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
