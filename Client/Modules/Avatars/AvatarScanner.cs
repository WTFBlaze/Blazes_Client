/*using Blaze.API.QM;
using Blaze.Configs;
using Blaze.Utils;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC.Core;

namespace Blaze.Modules
{
    class AvatarScanner : BModule
    {
        private static QMNestedButton Menu;

        public override void QuickMenuUI()
        {
            Menu = new QMNestedButton(BlazeMenu.Settings, "Avatar\nScanner", 4, 0, "Avatar Scanner Settings", "Avatar Scanner");

            new QMToggleButton(Menu, 1, 0, "Scan Self", delegate
            {
                AviScanConfig.Instance.ScanSelf = true;
            }, delegate
            {
                AviScanConfig.Instance.ScanSelf = false;
            }, "Scan your own avatars with the avatar scanner", AviScanConfig.Instance.ScanSelf);

            new QMToggleButton(Menu, 2, 0, "Scan Friends", delegate
            {
                AviScanConfig.Instance.ScanFriends = true;
            }, delegate
            {
                AviScanConfig.Instance.ScanFriends = false;
            }, "Scan your friends avatars with the avatar scanner", AviScanConfig.Instance.ScanFriends);

            new QMToggleButton(Menu, 3, 0, "Scan Others", delegate
            {
                AviScanConfig.Instance.ScanOthers = true;
            }, delegate
            {
                AviScanConfig.Instance.ScanOthers = false;
            }, "Scan non friends with the avatar scanner", AviScanConfig.Instance.ScanOthers);

            #region Game Objects
            var goMenu = new QMNestedButton(Menu, "Game\nObjects", 1, 1, "View GameObject settings", "GameObjects");

            new QMToggleButton(goMenu, 1, 0, "Scan GameObjects", delegate
            {
                AviScanConfig.Instance.ScanGameObjects = true;
            }, delegate
            {
                AviScanConfig.Instance.ScanGameObjects = false;
            }, "Scan GameObjects on Avatars", AviScanConfig.Instance.ScanGameObjects);

            new QMToggleButton(goMenu, 2, 0, "Delete Bad\nNamed Objects", delegate
            {
                AviScanConfig.Instance.DeleteBadObjectNames = true;
            }, delegate
            {
                AviScanConfig.Instance.DeleteBadObjectNames = true;
            }, "Deletes Malicious / Null / Invalid Named Objects", AviScanConfig.Instance.DeleteBadObjectNames);

            new QMToggleButton(goMenu, 3, 0, "Delete\nEmpty Objects", delegate
            {
                AviScanConfig.Instance.DeleteEmptyObjects = true;
            }, delegate
            {
                AviScanConfig.Instance.DeleteEmptyObjects = false;
            }, "Delete Empty Game Objects", AviScanConfig.Instance.DeleteEmptyObjects);

            new QMToggleButton(goMenu, 4, 0, "Delete Unsafe\nChild Count", delegate
            {
                AviScanConfig.Instance.DeleteUnsafeChildCount = true;
            }, delegate
            {
                AviScanConfig.Instance.DeleteUnsafeChildCount = false;
            }, "Delete GameObjects with an unsafe child count", AviScanConfig.Instance.DeleteUnsafeChildCount);
            #endregion

            #region Audio Sources
            var asMenu = new QMNestedButton(Menu, "Audio Sources", 2, 1, "View Audio Sources Settings", "Audio Sources");

            new QMToggleButton(asMenu, 1, 0, "Scan Audio Sources", delegate
            {
                AviScanConfig.Instance.ScanAudioSources = true;
            }, delegate
            {
                AviScanConfig.Instance.ScanAudioSources = false;
            }, "Scan Audio Sources on Avatars", AviScanConfig.Instance.ScanAudioSources);

            new QMToggleButton(asMenu, 2, 0, "Delete Empty Sources", delegate
            {
                AviScanConfig.Instance.DeleteEmptyAudioSources = true;
            }, delegate
            {
                AviScanConfig.Instance.DeleteEmptyAudioSources = false;
            }, "Delete Empty Audio Sources (audio sources that have no clip attached)", AviScanConfig.Instance.DeleteEmptyAudioSources);

            new QMToggleButton(asMenu, 3, 0, "Delete Bad\nClip Names", delegate
            {
                AviScanConfig.Instance.DeleteBadClipNames = true;
            }, delegate
            {
                AviScanConfig.Instance.DeleteBadClipNames = false;
            }, "Delete Audio Sources with Clips that are Malicious / Null / Invalidly named", AviScanConfig.Instance.DeleteBadClipNames);

            new QMToggleButton(asMenu, 4, 0, "Delete\nDuplicate Sources", delegate
            {
                AviScanConfig.Instance.DeleteDuplicateSources = true;
            }, delegate
            {
                AviScanConfig.Instance.DeleteDuplicateSources = false;
            }, "Delete Clones of Audio Sources", AviScanConfig.Instance.DeleteDuplicateSources);

            new QMToggleButton(asMenu, 1, 1, "Change Source\nDistances", delegate
            {
                AviScanConfig.Instance.ChangeAudioSourceDistance = true;
            }, delegate
            {
                AviScanConfig.Instance.ChangeAudioSourceDistance = false;
            }, "Change the distance of Audio Sources to be a safe value (also can be used to block world audios)", AviScanConfig.Instance.ChangeAudioSourceDistance);

            #endregion
        }

        internal static void ScanAvatar(GameObject obj)
        {
            //Logs.Log("[Avatar Scanner] Object Name: " + obj.name, ConsoleColor.Yellow);
            APIUser user = null;
            ApiAvatar avi = null;
            try
            {
                user = Patching.cachedAvatarManager.field_Private_VRCPlayer_0._player.field_Private_APIUser_0;
                avi = Patching.cachedAvatarManager.prop_ApiAvatar_0;
                if (user.displayName == PlayerUtils.CurrentUser().GetAPIUser().displayName && !AviScanConfig.Instance.ScanSelf) return; // Check Scan Self Toggle
                switch (user.isFriend)
                {
                    case true when !AviScanConfig.Instance.ScanFriends: // Check Scan Non Friends
                    case false when !AviScanConfig.Instance.ScanOthers: // Check Scan Friends
                        return;
                }
            }
            catch { }
            if (user == null || avi == null || obj == null) return;

            Logs.Log($"[Avatar Scanner] Scanning {avi.name} [Worn By: {user.displayName}]", ConsoleColor.Blue);
            if (AviScanConfig.Instance.ScanGameObjects) ProcessGameObjects(obj);
            if (AviScanConfig.Instance.ScanAudioSources) ProcessAudioSources(obj);
            if (AviScanConfig.Instance.ScanParticleSystems) ProcessParticleSystems(obj);
            if (AviScanConfig.Instance.ScanSkinnedMeshRenderers) ProcessSkinnedMeshRenderers(obj);
            if (AviScanConfig.Instance.ScanMeshFilters) ProcessMeshFilters(obj);
            if (AviScanConfig.Instance.ScanMeshRenderers) ProcessMeshRenderers(obj);
            if (AviScanConfig.Instance.ScanLineRenderers) ProcessLineRenderers(obj);
        }

        private static void ProcessGameObjects(GameObject obj)
        {
            try
            {
                Transform[] array2 = obj.GetComponentsInChildren<Transform>(true);
                var DestroyedEmptyGameObjects = 0;
                var DestroyedBadNamedObjects = 0;
                var DestroyedUnsafeChildCount = 0;
                foreach (var t in array2)
                {
                    if (t != null)
                    {
                        // Empty Objects
                        if (AviScanConfig.Instance.DeleteEmptyObjects)
                        {
                            Component[] components = t.GetComponents<Component>();
                            if (components.Length == 1 && t.childCount == 0 && t.name.ToLower().Contains("gameobject"))
                            {
                                UnityEngine.Object.DestroyImmediate(t);
                                DestroyedEmptyGameObjects++;
                            }
                        }
                    }

                    if (t != null)
                    {
                        // GameObject Name
                        if (AviScanConfig.Instance.DeleteBadObjectNames)
                        {
                            if (toxicterms.Contains(t.gameObject.name.ToLower()))
                            {
                                UnityEngine.Object.DestroyImmediate(t.gameObject);
                                DestroyedBadNamedObjects++;
                            }
                        }
                    }

                    if (t != null)
                    {
                        // Unsafe Children Count
                        if (AviScanConfig.Instance.DeleteUnsafeChildCount)
                        {
                            if (t.childCount >= 55)
                            {
                                UnityEngine.Object.DestroyImmediate(t.gameObject);
                                DestroyedUnsafeChildCount++;
                            }
                        }
                    }
                }

                if (DestroyedBadNamedObjects != 0)
                    Logs.Log($"x{DestroyedBadNamedObjects} GameObjects (Malicious Name)");
                if (DestroyedEmptyGameObjects != 0)
                    Logs.Log($"x{DestroyedEmptyGameObjects} GameObjects (Empty)");
                if (DestroyedUnsafeChildCount != 0)
                    Logs.Log($"x{DestroyedUnsafeChildCount} GameObjects (Children Count)");
            }
            catch (Exception e)
            {
                Logs.Error("[Avatar Scanner] Failed Scanning GameObjects!", e);
            }
        }

        private static void ProcessAudioSources(GameObject obj)
        {
            try
            {
                AudioSource[] array3 = obj.GetComponentsInChildren<AudioSource>(true);
                List<string> tmpClipNames = new();
                var EmptyAudioSources = 0;
                var BadNamedClips = 0;
                var DuplicateSources = 0;
                var MaxDistances = 0;

                foreach (var t in array3)
                {
                    // Delete Unused Audio Sources
                    if (t.clip == null && AviScanConfig.Instance.DeleteEmptyAudioSources)
                    {
                        UnityEngine.Object.DestroyImmediate(t.gameObject);
                        EmptyAudioSources++;
                    }
                    else
                    {
                        // Audio Source Clip Names
                        if (toxicterms.Contains(t.clip.name.ToLower()) && AviScanConfig.Instance.DeleteBadClipNames)
                        {
                            UnityEngine.Object.DestroyImmediate(t.gameObject);
                            BadNamedClips++;
                        }
                        else
                        {
                            // Duplicate Audio Source Same Clips
                            if (tmpClipNames.Contains(t.clip.name) && AviScanConfig.Instance.DeleteDuplicateSources)
                            {
                                UnityEngine.Object.DestroyImmediate(t.gameObject);
                                DuplicateSources++;
                            }
                            else
                            {
                                tmpClipNames.Add(t.clip.name);
                            }
                        }

                        if (t != null)
                        {
                            // Audio Source Distance
                            if (t.maxDistance > AviScanConfig.Instance.MaxAudioSourceDistance && AviScanConfig.Instance.ChangeAudioSourceDistance)
                            {
                                t.maxDistance = 10;
                                MaxDistances++;
                            }
                        }
                    }
                }

                if (EmptyAudioSources != 0)
                    Logs.Log($"x{EmptyAudioSources} Audio Source (Empty Source)");
                if (BadNamedClips != 0)
                    Logs.Log($"x{BadNamedClips} Audio Source (Malicious Clip Name)");
                if (DuplicateSources != 0)
                    Logs.Log($"x{DuplicateSources} Audio Source (Duplicates)");
                if (MaxDistances != 0)
                    Logs.Log($"x{MaxDistances} Audio Source Distances Changed");
            }
            catch (Exception e)
            {
                Logs.Error("[Avatar Scanner] Failed to Scan Audio Sources!", e);
            }
        }

        private static void ProcessParticleSystems(GameObject obj)
        {
            try
            {
                ParticleSystem[] array4 = obj.GetComponentsInChildren<ParticleSystem>(true);
                var MaxParticleSystems = 0;
                var MaxParticles = 0;

                // Block Particle System Spam
                if (array4.Length >= AviScanConfig.Instance.MaxParticleSystems && AviScanConfig.Instance.DeleteMaxParticleSystems)
                {
                    //DestroyAvatar(obj); // Use this if the method below is laggy
                    foreach (var p in array4)
                    {
                        UnityEngine.Object.DestroyImmediate(p.gameObject);
                        MaxParticleSystems++;
                    }
                    return;
                }

                // Check Particle Count
                foreach (var t in array4)
                {
                    if (t.maxParticles >= AviScanConfig.Instance.MaxParticlesCount)
                    {
                        t.maxParticles = 500;
                        MaxParticles++;
                    }

                    *//*if (t.maxParticles >= AviScannerConfig.Instance.MaxParticlesCount && AviScannerConfig.Instance.DeleteUnsafeParticleCount)
                    {
                        UnityEngine.Object.DestroyImmediate(t.gameObject);
                        MaxParticles++;
                    }*//*
                }

                if (MaxParticleSystems != 0)
                    Logs.Log($"x{MaxParticleSystems} Particle Systems (Over Max Limit)");

                if (MaxParticles != 0)
                    Logs.Log($"x{MaxParticles} Particle Systems Changed (Max Particles)");
            }
            catch (Exception e)
            {
                Logs.Error("[Avatar Scanner] Failed to Scan Particle Systems!", e);
            }
        }

        private static void ProcessSkinnedMeshRenderers(GameObject obj)
        {
            try
            {
                SkinnedMeshRenderer[] array5 = obj.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                var MaxMaterials = 0;
                var StackedMaterials = 0;
                var BadNames = 0;
                var BadNamedMeshes = 0;

                foreach (var t in array5)
                {
                    // Check Materials Amount
                    if (t.materials.Length >= AviScanConfig.Instance.MaxSMRMaterials && AviScanConfig.Instance.CheckMaxSMRMaterials)
                    {
                        UnityEngine.Object.DestroyImmediate(t.gameObject);
                        MaxMaterials++;
                    }
                    else
                    {
                        if (t != null)
                        {
                            // Check for Materials with multiple materials even though there is only 1 sub mesh
                            if (t.sharedMesh.subMeshCount == 1 && t.materials.Length > 1 && AviScanConfig.Instance.DeleteStackedSMRMaterials)
                            {
                                UnityEngine.Object.DestroyImmediate(t.gameObject);
                                StackedMaterials++;
                            }
                            else
                            {
                                // Check Material Names
                                foreach (var t1 in t.materials)
                                {
                                    if (toxicterms.Contains(t1.name.ToLower()) && AviScanConfig.Instance.DeleteBadNamedSMRMaterials)
                                    {
                                        //Instead of destroying the object because of a material name, lets just change that material's shader
                                        //so that if it is a malicious shader we don't detect it catches it anyways

                                        //UnityEngine.Object.DestroyImmediate(array5[j].gameObject);
                                        t1.shader = Shader.Find("VRChat/Mobile/Standard Lite");
                                        BadNames++;
                                    }
                                }
                            }
                        }

                        if (t != null)
                        {
                            // Check Mesh Name
                            if (toxicterms.Contains(t.sharedMesh.name.ToLower()) && t != null && AviScanConfig.Instance.DeleteBadNamedSMRMeshes)
                            {
                                UnityEngine.Object.DestroyImmediate(t.gameObject);
                                BadNamedMeshes++;
                            }
                        }
                    }
                }

                if (MaxMaterials != 0)
                    Logs.Log($"x{MaxMaterials} Skinned Mesh Renderer (Max Materials)");
                if (StackedMaterials != 0)
                    Logs.Log($"x{StackedMaterials} Skinned Mesh Renderer (Stacked Materials)");
                if (BadNames != 0)
                    Logs.Log($"x{BadNames} Material Shaders Changed (Malicious Name)");
                if (BadNamedMeshes != 0)
                    Logs.Log($"x{BadNamedMeshes} Skinned Mesh Renderer (Malicious Named Mesh)");
            }
            catch (Exception e)
            {
                Logs.Error("[Avatar Scanner] Failed to Scan Skinned Mesh Renderers!", e);
            }
        }

        private static void ProcessMeshFilters(GameObject obj)
        {
            try
            {
                MeshFilter[] array7 = obj.GetComponentsInChildren<MeshFilter>(true);
                var EmptyMesh = 0;
                var BadNames = 0;

                foreach (var t in array7)
                {
                    // Check if mesh filter is empty
                    if (t.mesh == null && AviScanConfig.Instance.DeleteBadNamedMFMeshes)
                    {
                        UnityEngine.Object.DestroyImmediate(t.gameObject);
                        EmptyMesh++;
                    }
                    else
                    {
                        if (toxicterms.Contains(t.mesh.name.ToLower()) && AviScanConfig.Instance.DeleteBadNamedMFMeshes)
                        {
                            UnityEngine.Object.DestroyImmediate(t.gameObject);
                            BadNames++;
                        }
                    }
                }
                if (EmptyMesh != 0)
                    Logs.Log($"x{EmptyMesh} Mesh Filters (Empty Filters)");
                if (BadNames != 0)
                    Logs.Log($"x{BadNames} Mesh Filters (Malicious Named Mesh");
            }
            catch (Exception e)
            {
                Logs.Error("[Avatar Scanner] Failed to Scan Mesh Filters!", e);
            }
        }

        private static void ProcessMeshRenderers(GameObject obj)
        {
            try
            {
                MeshRenderer[] array8 = obj.GetComponentsInChildren<MeshRenderer>(true);
                var MaxMaterials = 0;

                foreach (var t in array8)
                {
                    if (t.materials.Length >= AviScanConfig.Instance.MaxMRMaterials)
                    {
                        UnityEngine.Object.DestroyImmediate(t);
                        MaxMaterials++;
                    }
                }

                if (MaxMaterials != 0)
                    Logs.Log($"x{MaxMaterials} Mesh Renderers (Max Materials)");
            }
            catch (Exception e)
            {
                Logs.Error("[Avatar Scanner] Failed to Scan Mesh Renderers!", e);
            }
        }

        private static void ProcessLineRenderers(GameObject obj)
        {
            try
            {
                LineRenderer[] array6 = obj.GetComponentsInChildren<LineRenderer>(true);
                var DeletedCount = 0;

                foreach (var t in array6)
                {
                    UnityEngine.Object.DestroyImmediate(t.gameObject);
                    DeletedCount++;
                }

                if (DeletedCount != 0)
                    Logs.Log($"x{DeletedCount} Line Renderers");
            }
            catch (Exception e)
            {
                Logs.Error("[Avatar Scanner] Failed to Scan Line Renderers!", e);
            }
        }

        internal static void DestroyAvatar(GameObject gameObject)
        {
            if (gameObject != null)
            {
                var transform = gameObject.transform.parent.Find("_AvatarMirrorClone");
                var transform2 = gameObject.transform.parent.Find("_AvatarShadowClone");
                var transform3 = gameObject.transform.parent.Find("IK");
                if (transform3 != null)
                {
                    UnityEngine.Object.DestroyImmediate(transform3, true);
                }
                if (transform2 != null)
                {
                    UnityEngine.Object.DestroyImmediate(transform2, true);
                }
                if (transform != null)
                {
                    UnityEngine.Object.DestroyImmediate(transform, true);
                }
                UnityEngine.Object.DestroyImmediate(gameObject, true);
            }
        }

        public static List<string> toxicterms = new()
        {
            "gang",
            "boom",
            "yeet",
            "torus",
            "die lol",
            "cull-behind",
            "torus.049",
            "bigasstorus",
            "bigouch",
            "ouchbig",
            "049",
            "爪卂丂ㄒ乇尺",
            "几ㄖ尺卂",
            "丂卂几ᗪ乇尺丂",
            "🐯",
            "deleter",
            "ouch",
            "reeee",
            "paralyzer",
            "fuck",
            "cunt",
            "nigger",
            "...",
            "nigga",
            "sex button",
            //"project",
            "cloudie",
            "cloudielovesyou",
            "society",
            "zyklon",
            "brr",
            "crash",
            "smack",
            "smac",
            "rape",
            "killer",
            "clap",
            "entity",
            "toxicity",
            "toxic",
            "critical error",
            "dedsec",
            "watchdogs",
            "wd",
            "iya",
            "stasis",
            "impim legion",
            "cicero",
            "hostile",
            "hack",
            "citizen hack",
            "p.r.o.j.e.c.t",
            "forsaken",
            "murda inc",
            "murda",
            "hellraisers",
            "go outside gang",
            "go outside",
            "cuddle gang",
            "a.b.y.s.s",
            "abyss",
            "ons",
            "nas",
            "neneko squad",
            "teamtoxi",
            "vibez",
            "liner family",
            "aukuto nakama",
            "the boys",
            "blackout",
            "black out",
            "undertaker",
            "vanity",
            "stickman society",
            "hashira",
            "liquid courage",
            "notsi party",
            "natzi",
            "lego gang",
            "laughing coffin",
            "lc",
            "autismo",
            "418",
            "1-800",
            "1-810",
            "1800",
            "1810",
            "1400",
            "ods",
            "entropy",
            "anti-resurrection",
            "anti ressurection",
            "antiressurection",
            "euphoria",
            "fatality",
            "eternal hanahaki",
            "toybox",
            //"inu", pink shadering shiba models
            "glitch",
            "evolution",
            "sinister",
            "headpat gang",
            "headpatgang",
            "hpg",
            "exiled",
            "spectrum",
            "void",
            "atombomb",
            "atom bomb",
            "overwatch",
            "west side",
            "eradication",
            "forever club",
            "ministry",
            "deathflower",
            "death flower",
        };
    }
}
*/