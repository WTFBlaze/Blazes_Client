using Blaze.API.QM;
using Blaze.Utils.VRChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using UnityEngine;

namespace Blaze.Modules
{
    public class AntiCrash : BModule
    {
        private QMNestedButton Menu;

        public override void UI()
        {
            Menu = new QMNestedButton(BlazeQM.Security, "Anti\nCrash", 2, 2, "Blaze's Client Anti Crash Settings", "Anti Crash");

            new QMToggleButton(Menu, 1, 0, "Check Self", delegate
            {
                Config.AntiCrash.CheckSelf = true;
            }, delegate
            {
                Config.AntiCrash.CheckSelf = false;
            }, "Enable Checking your own Avatars with the Anti Crash", Config.AntiCrash.CheckSelf);

            new QMToggleButton(Menu, 2, 0, "Check Friends", delegate
            {
                Config.AntiCrash.CheckFriends = true;
            }, delegate
            {
                Config.AntiCrash.CheckFriends = false;
            }, "Enable checking your friends Avatars with the Anti Crash", Config.AntiCrash.CheckFriends);

            new QMToggleButton(Menu, 3, 0, "Check Others", delegate
            {
                Config.AntiCrash.CheckOthers = true;
            }, delegate
            {
                Config.AntiCrash.CheckOthers = false;
            }, "Enable Checking non friends Avatars with the Anti Crash", Config.AntiCrash.CheckOthers);

            new QMToggleButton(Menu, 4, 0, "Scan Only In Publics", delegate
            {
                Config.AntiCrash.OnlyCheckInPublicLobbies = true;
            }, delegate
            {
                Config.AntiCrash.OnlyCheckInPublicLobbies = false;
            }, "Enable checking only when you are in public lobbies", Config.AntiCrash.OnlyCheckInPublicLobbies);

            new QMToggleButton(Menu, 1, 1, "Anti Physics", delegate
            {
                Config.AntiCrash.AntiPhysicsCrash = true;
            }, delegate
            {
                Config.AntiCrash.AntiPhysicsCrash = false;
            }, "Stop crashing from physics values", Config.AntiCrash.AntiPhysicsCrash);

            new QMToggleButton(Menu, 2, 1, "Anti Blend Shape", delegate
            {
                Config.AntiCrash.AntiBlendShapeCrash = true;
            }, delegate
            {
                Config.AntiCrash.AntiBlendShapeCrash = false;
            }, "Stop Crashing from Blend Shapes", Config.AntiCrash.AntiBlendShapeCrash);

            new QMToggleButton(Menu, 3, 1, "Anti Audio", delegate
            {
                Config.AntiCrash.AntiAudioCrash = true;
            }, delegate
            {
                Config.AntiCrash.AntiAudioCrash = false;
            }, "Stop Crashing from Audio Sources", Config.AntiCrash.AntiAudioCrash);

            new QMToggleButton(Menu, 4, 1, "Anti Cloth", delegate
            {
                Config.AntiCrash.AntiClothCrash = true;
            }, delegate
            {
                Config.AntiCrash.AntiClothCrash = false;
            }, "Stop Crashing from Cloths", Config.AntiCrash.AntiClothCrash);

            new QMToggleButton(Menu, 1, 2, "Anti Particles", delegate
            {
                Config.AntiCrash.AntiParticleSystemCrash = true;
            }, delegate
            {
                Config.AntiCrash.AntiParticleSystemCrash = false;
            }, "Stop Crashing from Particle Systems", Config.AntiCrash.AntiParticleSystemCrash);

            new QMToggleButton(Menu, 2, 2, "Anti Dynamic Bones", delegate
            {
                Config.AntiCrash.AntiDynamicBoneCrash = true;
            }, delegate
            {
                Config.AntiCrash.AntiDynamicBoneCrash = false;
            }, "Stop Crashing from Dynamic Bones", Config.AntiCrash.AntiDynamicBoneCrash);

            new QMToggleButton(Menu, 3, 2, "Anti Lights", delegate
            {
                Config.AntiCrash.AntiLightSourceCrash = true;
            }, delegate
            {
                Config.AntiCrash.AntiLightSourceCrash = false;
            }, "Stop Crashing from Light Sources", Config.AntiCrash.AntiLightSourceCrash);

            new QMToggleButton(Menu, 4, 2, "Anti Mesh", delegate
            {
                Config.AntiCrash.AntiMeshCrash = true;
            }, delegate
            {
                Config.AntiCrash.AntiMeshCrash = false;
            }, "Stop Crashing from Meshes", Config.AntiCrash.AntiMeshCrash);

            new QMToggleButton(Menu, 1, 3, "Anti Material", delegate
            {
                Config.AntiCrash.AntiMaterialCrash = true;
            }, delegate
            {
                Config.AntiCrash.AntiMaterialCrash = false;
            }, "Stop Crashing from Materials", Config.AntiCrash.AntiMaterialCrash);

            new QMToggleButton(Menu, 2, 3, "Anti Shader", delegate
            {
                Config.AntiCrash.AntiShaderCrash = true;
            }, delegate
            {
                Config.AntiCrash.AntiShaderCrash = false;
            }, "Stop Crashing from Shaders", Config.AntiCrash.AntiShaderCrash);

            new QMToggleButton(Menu, 3, 3, "<size=18>Anti ScreenSpace Shaders</size>", delegate
            {
                Config.AntiCrash.AntiScreenSpace = true;
            }, delegate
            {
                Config.AntiCrash.AntiScreenSpace = false;
            }, "Stop screenspace shaders from loading", Config.AntiCrash.AntiScreenSpace);
        }

        public class AntiCrashRendererPostProcess
        {
            public int nukedMeshes;
            public int polygonCount;

            public int nukedMaterials;
            public int materialCount;

            public int nukedShaders;
            public int shaderCount;
        }

        public class AntiCrashMaterialPostProcess
        {
            public int nukedMaterials;
            public int materialCount;
        }

        public class AntiCrashShaderPostProcess
        {
            public int nukedShaders;
            public int shaderCount;
        }

        public class AntiCrashParticleSystemPostProcess
        {
            public int nukedParticleSystems;
            public int currentParticleCount;
        }

        public class AntiCrashClothPostProcess
        {
            public int nukedCloths;
            public int currentVertexCount;
        }

        public class AntiCrashDynamicBonePostProcess
        {
            public int nukedDynamicBones;
            public int dynamicBoneCount;
        }

        public class AntiCrashDynamicBoneColliderPostProcess
        {
            public int nukedDynamicBoneColliders;
            public int dynamicBoneColiderCount;
        }

        public class AntiCrashLightSourcePostProcess
        {
            public int nukedLightSources;
            public int lightSourceCount;
        }

        public static void ProcessRenderer(Renderer renderer, bool limitPolygons, bool limitMaterials, bool limitShaders, ref AntiCrashRendererPostProcess previousProcess)
        {
            if (limitPolygons == true)
            {
                ProcessMeshPolygons(renderer, ref previousProcess.nukedMeshes, ref previousProcess.polygonCount);
            }

            if (limitMaterials == true)
            {
                AntiCrashMaterialPostProcess postReport = ProcessMaterials(renderer, previousProcess.nukedMaterials, previousProcess.materialCount);

                previousProcess.nukedMaterials = postReport.nukedMaterials;
                previousProcess.materialCount = postReport.materialCount;
            }

            if (limitShaders == true)
            {
                AntiCrashShaderPostProcess postReport = ProcessShaders(renderer, previousProcess.nukedShaders, previousProcess.shaderCount);

                previousProcess.nukedShaders = postReport.nukedShaders;
                previousProcess.shaderCount = postReport.shaderCount;
            }
        }

        public static void ProcessMeshPolygons(Renderer renderer, ref int currentNukedMeshes, ref int currentPolygonCount)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = renderer.TryCast<SkinnedMeshRenderer>();
            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();

            Mesh mesh = skinnedMeshRenderer?.sharedMesh ?? meshFilter?.sharedMesh;

            if (mesh == null)
            {
                return;
            }

            if (mesh.isReadable == false)
            {
                currentNukedMeshes++;

                UnityEngine.Object.DestroyImmediate(mesh, true);

                return;
            }

            if (Config.AntiCrash.AntiBlendShapeCrash == true && mesh.name.ToLower().Equals("body"))
            {
                bool hasRevertedBlendShape = false;
                bool hasPoseToRestBlendShape = false;
                bool hasKey22BlendShape = false;
                bool hasKey56BlendShape = false;
                bool hasSlantBlendShape = false;

                for (int i = 0; i < mesh.blendShapeCount; i++)
                {
                    string blendShapeName = mesh.GetBlendShapeName(i).ToLower();

                    if (blendShapeName.Contains("reverted") == true)
                    {
                        hasRevertedBlendShape = true;
                    }

                    if (blendShapeName.Contains("posetorest") == true)
                    {
                        hasPoseToRestBlendShape = true;
                    }
                    else if (blendShapeName.Contains("key 22") == true)
                    {
                        hasKey22BlendShape = true;
                    }
                    else if (blendShapeName.Contains("key 56") == true)
                    {
                        hasKey56BlendShape = true;
                    }
                    else if (blendShapeName.Contains("slant") == true)
                    {
                        hasSlantBlendShape = true;
                    }
                }

                //"Cyber Popular" avatar detected
                if (hasRevertedBlendShape == true && hasPoseToRestBlendShape == true && hasKey22BlendShape == true && hasKey56BlendShape == true && hasSlantBlendShape == true)
                {
                    //This is the crashing BlendShape key
                    /*int chosenBlendShapeKey = 7;
                    ConsoleUtils.Info("AntiCrash", $"BlendShapes: {mesh.blendShapeCount}");
                    ConsoleUtils.Info("AntiCrash", $"Chosen BlendShape: {mesh.GetBlendShapeName(chosenBlendShapeKey)}");
                    ConsoleUtils.Info("AntiCrash", $"VertexCount: {mesh.vertexCount}");
                    Vector3[] verticies = new Vector3[mesh.vertexCount];
                    Vector3[] normals = new Vector3[mesh.vertexCount];
                    Vector3[] tangents = new Vector3[mesh.vertexCount];
                    MelonLoader.MelonLogger.Msg("1");
                    mesh.GetBlendShapeFrameVertices(chosenBlendShapeKey, 0, verticies, normals, tangents);
                    MelonLoader.MelonLogger.Msg("2");
                    for (int i = 0; i < mesh.vertexCount; i++)
                    {
                        ConsoleUtils.Info("Frame", $"Index: {i} | Vertice: {verticies[i].ToString()} | Normal: {normals[i].ToString()} | Tangent: {tangents[i].ToString()}");
                    }*/

                    mesh.ClearBlendShapes();
                }
            }

            ProcessMesh(mesh, ref currentNukedMeshes, ref currentPolygonCount);
        }

        public static AntiCrashMaterialPostProcess ProcessMaterials(Renderer renderer, int currentNukedMaterials, int currentMaterialCount)
        {
            int newMaterialCount = currentMaterialCount + renderer.GetMaterialCount();

            if (newMaterialCount > Config.AntiCrash.MaxAllowedAvatarMaterials)
            {
                //Grab the current amount of materials
                Il2CppSystem.Collections.Generic.List<Material> materialList = new Il2CppSystem.Collections.Generic.List<Material>();
                renderer.GetSharedMaterials(materialList);

                //Calculate the amount of materials we need to remove
                int startRemoveIndex = (currentMaterialCount < Config.AntiCrash.MaxAllowedAvatarMaterials) ? Config.AntiCrash.MaxAllowedAvatarMaterials : 0;
                int removeMaterialCount = (startRemoveIndex == 0) ? materialList.Count : (newMaterialCount - Config.AntiCrash.MaxAllowedAvatarMaterials);

                //Make sure we don't get out of bounds errors
                if (startRemoveIndex > materialList.Count)
                {
                    startRemoveIndex = materialList.Count;
                }

                //Make sure we don't get out of bounds errors
                int maxRemoveableCount = materialList.Count - startRemoveIndex;

                if (removeMaterialCount > maxRemoveableCount)
                {
                    removeMaterialCount = maxRemoveableCount;
                }

                //Set new values accordingly
                currentNukedMaterials += removeMaterialCount;
                newMaterialCount -= removeMaterialCount;

                //Remove all materials deemed unneccesary
                materialList.RemoveRange(startRemoveIndex, removeMaterialCount);
                renderer.materials = (Il2CppReferenceArray<Material>)materialList.ToArray();
            }

            currentMaterialCount = newMaterialCount;

            return new AntiCrashMaterialPostProcess
            {
                nukedMaterials = currentNukedMaterials,
                materialCount = currentMaterialCount
            };
        }

        public static AntiCrashShaderPostProcess ProcessShaders(Renderer renderer, int currentNukedShaders, int currentShaderCount)
        {
            for (int j = 0; j < renderer.materials.Length; j++)
            {
                //Error Check
                if (renderer.materials[j] == null)
                {
                    continue;
                }
                currentShaderCount++;

                //Blacklist Check
                if (ShaderBlacklist.blockList.Contains(renderer.materials[j].shader.name.ToLower()))
                {
                    renderer.materials[j].shader = ShaderUtils.GetStandardShader();
                    currentNukedShaders++;
                    continue;
                }

                //Engine Check
                if (renderer.materials[j].shader.isSupported == false)
                {
                    renderer.materials[j].shader = ShaderUtils.GetStandardShader();
                    currentNukedShaders++;
                    continue;
                }

                // Too long of a name check
                if (renderer.materials[j].shader.name.Length > 250)
                {
                    renderer.materials[j].shader = ShaderUtils.GetStandardShader();
                    currentNukedShaders++;
                    continue;
                }


                // Check Render Queue
                if (renderer.materials[j].shader.renderQueue > 25000 && Config.AntiCrash.AntiScreenSpace)
                {
                    renderer.materials[j].shader = ShaderUtils.GetStandardShader();
                    currentNukedShaders++;
                    continue;
                }

                //Sanity Check
                switch (renderer.materials[j].shader.name)
                {
                    case "Standard":
                        {
                            renderer.materials[j].shader = ShaderUtils.GetStandardShader();
                            break;
                        }

                    case "Diffuse":
                        {
                            renderer.materials[j].shader = ShaderUtils.GetDiffuseShader();
                            break;
                        }
                }
            }

            return new AntiCrashShaderPostProcess
            {
                nukedShaders = currentNukedShaders,
                shaderCount = currentShaderCount
            };
        }

        public static AntiCrashClothPostProcess ProcessCloth(Cloth cloth, int nukedCloths, int currentVertexCount)
        {
            cloth.clothSolverFrequency = Mathf.Max(cloth.clothSolverFrequency, 300f);

            Mesh skinnedMesh = cloth.GetComponent<SkinnedMeshRenderer>()?.sharedMesh;

            if (skinnedMesh == null)
            {
                nukedCloths++;

                UnityEngine.Object.DestroyImmediate(cloth, true);

                return new AntiCrashClothPostProcess
                {
                    nukedCloths = nukedCloths,
                    currentVertexCount = currentVertexCount
                };
            }

            currentVertexCount += skinnedMesh.vertexCount;

            if (currentVertexCount >= Config.AntiCrash.MaxAllowedAvatarClothVertices)
            {
                currentVertexCount -= skinnedMesh.vertexCount;

                nukedCloths++;

                UnityEngine.Object.DestroyImmediate(cloth, true);
            }

            return new AntiCrashClothPostProcess
            {
                nukedCloths = nukedCloths,
                currentVertexCount = currentVertexCount
            };
        }

        public static void ProcessParticleSystem(ParticleSystem particleSystem, ref AntiCrashParticleSystemPostProcess post)
        {
            ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();

            if (renderer == null)
            {
                post.nukedParticleSystems++;

                UnityEngine.Object.DestroyImmediate(particleSystem, true);

                return;
            }

            particleSystem.main.simulationSpeed = CrashUtils.Clamp(particleSystem.main.simulationSpeed, 0f, Config.AntiCrash.MaxAllowedAvatarParticleSimulationSpeed);
            particleSystem.collision.maxCollisionShapes = CrashUtils.Clamp(particleSystem.collision.maxCollisionShapes, 0, Config.AntiCrash.MaxAllowedAvatarParticleCollisionShapes);
            particleSystem.trails.ribbonCount = CrashUtils.Clamp(particleSystem.trails.ribbonCount, 0, Config.AntiCrash.MaxAllowedAvatarParticleTrails);

            for (int i = 0; i < particleSystem.emission.burstCount; i++)
            {
                ParticleSystem.Burst burst = particleSystem.emission.GetBurst(i);

                burst.maxCount = CrashUtils.Clamp(burst.maxCount, (short)0, (short)125);
                burst.cycleCount = CrashUtils.Clamp(burst.cycleCount, 0, 125);
            }

            int particleLimit = Config.AntiCrash.MaxAllowedAvatarParticleLimit - post.currentParticleCount;

            if (particleSystem.maxParticles > particleLimit)
            {
                particleSystem.maxParticles = particleLimit;
            }

            post.currentParticleCount += particleSystem.maxParticles;

            if (renderer.renderMode == ParticleSystemRenderMode.Mesh)
            {
                Il2CppReferenceArray<Mesh> meshes = new(renderer.meshCount);
                renderer.GetMeshes(meshes);

                int polySum = 0;
                int nukedMeshes = 0;

                foreach (Mesh mesh in meshes)
                {
                    ProcessMesh(mesh, ref nukedMeshes, ref polySum);
                }

                if ((polySum * particleSystem.maxParticles) > Config.AntiCrash.MaxAllowedAvatarParticleMeshVertices)
                {
                    particleSystem.maxParticles = Config.AntiCrash.MaxAllowedAvatarParticleMeshVertices / polySum;
                }
            }

            if (particleSystem.maxParticles == 0)
            {
                post.nukedParticleSystems++;

                UnityEngine.Object.DestroyImmediate(renderer, true);
                UnityEngine.Object.DestroyImmediate(particleSystem, true);
            }
        }

        public static void ProcessMesh(Mesh mesh, ref int currentNukedMeshes, ref int currentPolygonCount)
        {
            int subMeshCount;

            try
            {
                subMeshCount = mesh.subMeshCount;
            }
            catch (System.Exception)
            {
                subMeshCount = 1;
            }

            for (int j = 0; j < subMeshCount; j++)
            {
                try
                {
                    uint polygonsInSubmesh = mesh.GetIndexCount(j);

                    switch (mesh.GetTopology(j))
                    {
                        case MeshTopology.Triangles:
                            {
                                polygonsInSubmesh /= 3;

                                break;
                            }

                        case MeshTopology.Quads:
                            {
                                polygonsInSubmesh /= 4;

                                break;
                            }

                        case MeshTopology.Lines:
                            {
                                polygonsInSubmesh /= 2;

                                break;
                            }
                    }

                    if ((currentPolygonCount + polygonsInSubmesh) > Config.AntiCrash.MaxAllowedAvatarPolygons)
                    {
                        currentNukedMeshes++;

                        UnityEngine.Object.DestroyImmediate(mesh, true);

                        continue;
                    }

                    currentPolygonCount += (int)polygonsInSubmesh;
                }
                catch (System.Exception) { /* It's fine to get an exception here - we just want to be sure we don't skip any meshes */ }
            }

            //Sanity check in case we deleted the mesh in the previous stage
            if (mesh == null)
            {
                return;
            }

            //Mesh Safety
            if (CrashUtils.IsBeyondLimit(mesh.bounds.extents, -100f, 100f) == true)
            {
                UnityEngine.Object.DestroyImmediate(mesh, true);

                return;
            }

            if (CrashUtils.IsBeyondLimit(mesh.bounds.size, -100f, 100f) == true)
            {
                UnityEngine.Object.DestroyImmediate(mesh, true);

                return;
            }

            if (CrashUtils.IsBeyondLimit(mesh.bounds.center, -100f, 100f) == true)
            {
                UnityEngine.Object.DestroyImmediate(mesh, true);

                return;
            }

            if (CrashUtils.IsBeyondLimit(mesh.bounds.min, -100f, 100f) == true)
            {
                UnityEngine.Object.DestroyImmediate(mesh, true);

                return;
            }

            if (CrashUtils.IsBeyondLimit(mesh.bounds.max, -100f, 100f) == true)
            {
                UnityEngine.Object.DestroyImmediate(mesh, true);

                return;
            }

            return;
        }

        public static AntiCrashDynamicBonePostProcess ProcessDynamicBone(DynamicBone dynamicBone, int currentNukedDynamicBones, int currentDynamicBones)
        {
            if (currentDynamicBones >= Config.AntiCrash.MaxAllowedAvatarDynamicBones)
            {
                currentNukedDynamicBones++;

                UnityEngine.Object.DestroyImmediate(dynamicBone, true);

                return new AntiCrashDynamicBonePostProcess
                {
                    nukedDynamicBones = currentNukedDynamicBones,

                    dynamicBoneCount = currentDynamicBones
                };
            }

            currentDynamicBones++;

            //Safety
            dynamicBone.m_UpdateRate = CrashUtils.Clamp(dynamicBone.m_UpdateRate, 0f, 60f);
            dynamicBone.m_Radius = CrashUtils.Clamp(dynamicBone.m_Radius, 0f, 2f);
            dynamicBone.m_EndLength = CrashUtils.Clamp(dynamicBone.m_EndLength, 0f, 10f);
            dynamicBone.m_DistanceToObject = CrashUtils.Clamp(dynamicBone.m_DistanceToObject, 0f, 1f);

            //EndOffset Safety - Start
            Vector3 newEndOffset = dynamicBone.m_EndOffset;

            newEndOffset.x = CrashUtils.Clamp(newEndOffset.x, -5f, 5f);
            newEndOffset.y = CrashUtils.Clamp(newEndOffset.y, -5f, 5f);
            newEndOffset.z = CrashUtils.Clamp(newEndOffset.z, -5f, 5f);

            dynamicBone.m_EndOffset = newEndOffset;
            //EndOffset Safety - End

            //Gravity Safety - Start
            Vector3 newGravity = dynamicBone.m_Gravity;

            newGravity.x = CrashUtils.Clamp(newGravity.x, -5f, 5f);
            newGravity.y = CrashUtils.Clamp(newGravity.y, -5f, 5f);
            newGravity.z = CrashUtils.Clamp(newGravity.z, -5f, 5f);

            dynamicBone.m_Gravity = newGravity;
            //Gravity Safety - End

            //Force Safety - Start
            Vector3 newForce = dynamicBone.m_Force;

            newForce.x = CrashUtils.Clamp(newForce.x, -5f, 5f);
            newForce.y = CrashUtils.Clamp(newForce.y, -5f, 5f);
            newForce.z = CrashUtils.Clamp(newForce.z, -5f, 5f);

            dynamicBone.m_Force = newForce;
            //Force Safety - End

            //Colliders Safety - Start
            Il2CppSystem.Collections.Generic.List<DynamicBoneCollider> dynamicBones = new Il2CppSystem.Collections.Generic.List<DynamicBoneCollider>();

            foreach (DynamicBoneCollider collider in dynamicBone.m_Colliders.ToArray())
            {
                if (collider != null && dynamicBones.Contains(collider) == false)
                {
                    dynamicBones.Add(collider);
                }
            }

            dynamicBone.m_Colliders = dynamicBones;
            //Colliders Safety - End

            return new AntiCrashDynamicBonePostProcess
            {
                nukedDynamicBones = currentNukedDynamicBones,

                dynamicBoneCount = currentDynamicBones
            };
        }

        public static AntiCrashDynamicBoneColliderPostProcess ProcessDynamicBoneCollider(DynamicBoneCollider dynamicBoneCollider, int currentNukedDynamicBoneColliders, int currentDynamicBoneColliders)
        {
            if (currentDynamicBoneColliders >= Config.AntiCrash.MaxAllowedAvatarDynamicBoneColliders)
            {
                currentNukedDynamicBoneColliders++;

                UnityEngine.Object.DestroyImmediate(dynamicBoneCollider, true);

                return new AntiCrashDynamicBoneColliderPostProcess
                {
                    nukedDynamicBoneColliders = currentNukedDynamicBoneColliders,

                    dynamicBoneColiderCount = currentDynamicBoneColliders
                };
            }

            currentDynamicBoneColliders++;

            //Safety
            dynamicBoneCollider.m_Radius = CrashUtils.Clamp(dynamicBoneCollider.m_Radius, 0f, 50f);
            dynamicBoneCollider.m_Height = CrashUtils.Clamp(dynamicBoneCollider.m_Height, 0f, 50f);

            //Center Safety - Start
            Vector3 newCenter = dynamicBoneCollider.m_Center;

            CrashUtils.Clamp(newCenter.x, -50f, 50f);
            CrashUtils.Clamp(newCenter.y, -50f, 50f);
            CrashUtils.Clamp(newCenter.z, -50f, 50f);

            dynamicBoneCollider.m_Center = newCenter;
            //Center Safety - End

            return new AntiCrashDynamicBoneColliderPostProcess
            {
                nukedDynamicBoneColliders = currentNukedDynamicBoneColliders,

                dynamicBoneColiderCount = currentDynamicBoneColliders
            };
        }

        public static AntiCrashLightSourcePostProcess ProcessLight(Light light, int currentNukedLights, int currentLights)
        {
            if (currentLights >= Config.AntiCrash.MaxAllowedAvatarLightSources)
            {
                currentNukedLights++;

                UnityEngine.Object.DestroyImmediate(light, true);

                return new AntiCrashLightSourcePostProcess
                {
                    nukedLightSources = currentNukedLights,

                    lightSourceCount = currentLights
                };
            }

            currentLights++;

            return new AntiCrashLightSourcePostProcess
            {
                nukedLightSources = currentNukedLights,

                lightSourceCount = currentLights
            };
        }

        public static int ProcessTransform(Transform transform, int limitedTransforms)
        {
            bool limitedTransform = false;

            //Rotation Safety
            Quaternion newLocalRotation = transform.localRotation;

            if (CrashUtils.IsInvalid(newLocalRotation) == true)
            {
                //MelonLogger.Msg("Rotation was invalid - lets fix it up");

                newLocalRotation = Quaternion.identity;

                limitedTransform = true;
            }
            else
            {
                newLocalRotation.x = CrashUtils.Clamp(newLocalRotation.x, -Config.AntiCrash.MaxAllowedAvatarTransformScale, Config.AntiCrash.MaxAllowedAvatarTransformScale);
                newLocalRotation.y = CrashUtils.Clamp(newLocalRotation.y, -Config.AntiCrash.MaxAllowedAvatarTransformScale, Config.AntiCrash.MaxAllowedAvatarTransformScale);
                newLocalRotation.z = CrashUtils.Clamp(newLocalRotation.z, -Config.AntiCrash.MaxAllowedAvatarTransformScale, Config.AntiCrash.MaxAllowedAvatarTransformScale);
                newLocalRotation.w = CrashUtils.Clamp(newLocalRotation.w, -Config.AntiCrash.MaxAllowedAvatarTransformScale, Config.AntiCrash.MaxAllowedAvatarTransformScale);

                if (newLocalRotation != transform.localRotation)
                {
                    //MelonLogger.Msg("Rotation was clamped");

                    limitedTransform = true;
                }
            }

            transform.localRotation = newLocalRotation;

            //Scale Safety
            Vector3 newLocalScale = transform.localScale;

            if (CrashUtils.IsInvalid(newLocalScale) == true)
            {
                //MelonLogger.Msg("Scale was invalid - lets fix it up");

                newLocalScale = new Vector3(1f, 1f, 1f);

                limitedTransform = true;
            }
            else
            {
                newLocalScale.x = CrashUtils.Clamp(newLocalScale.x, -Config.AntiCrash.MaxAllowedAvatarTransformScale, Config.AntiCrash.MaxAllowedAvatarTransformScale);
                newLocalScale.y = CrashUtils.Clamp(newLocalScale.y, -Config.AntiCrash.MaxAllowedAvatarTransformScale, Config.AntiCrash.MaxAllowedAvatarTransformScale);
                newLocalScale.z = CrashUtils.Clamp(newLocalScale.z, -Config.AntiCrash.MaxAllowedAvatarTransformScale, Config.AntiCrash.MaxAllowedAvatarTransformScale);

                if (newLocalScale != transform.localScale)
                {
                    //MelonLogger.Msg("Scale was clamped");

                    limitedTransform = true;
                }
            }

            transform.localScale = newLocalScale;

            return limitedTransform ? ++limitedTransforms : limitedTransforms;
        }

        public static void ProcessRigidbody(Rigidbody rigidbody)
        {
            rigidbody.mass = CrashUtils.Clamp(rigidbody.mass, -10000f, 10000f);
            rigidbody.maxAngularVelocity = CrashUtils.Clamp(rigidbody.maxAngularVelocity, -100f, 100f);
            rigidbody.maxDepenetrationVelocity = CrashUtils.Clamp(rigidbody.maxDepenetrationVelocity, -100f, 100f);
        }

        public static bool ProcessCollider(Collider collider)
        {
            if ((collider.bounds.center.x < 100f && collider.bounds.center.x > 100f) ||
                (collider.bounds.center.y < 100f && collider.bounds.center.y > 100f) ||
                (collider.bounds.center.z < 100f && collider.bounds.center.z > 100f) ||
                (collider.bounds.extents.x < 100f && collider.bounds.extents.x > 100f) ||
                (collider.bounds.extents.y < 100f && collider.bounds.extents.y > 100f) ||
                (collider.bounds.extents.z < 100f && collider.bounds.extents.z > 100f))
            {
                UnityEngine.Object.DestroyImmediate(collider, true);

                return true;
            }

            if (collider is BoxCollider boxCollider)
            {
                //Center Safety
                Vector3 newCenter = boxCollider.center;

                newCenter.x = CrashUtils.Clamp(newCenter.x, -100f, 100f);
                newCenter.y = CrashUtils.Clamp(newCenter.y, -100f, 100f);
                newCenter.y = CrashUtils.Clamp(newCenter.y, -100f, 100f);

                boxCollider.center = newCenter;

                //Extents Safety
                Vector3 newExtents = boxCollider.extents;

                newExtents.x = CrashUtils.Clamp(newExtents.x, -100f, 100f);
                newExtents.y = CrashUtils.Clamp(newExtents.y, -100f, 100f);
                newExtents.y = CrashUtils.Clamp(newExtents.y, -100f, 100f);

                boxCollider.extents = newExtents;

                //Size Safety
                Vector3 newSize = boxCollider.size;

                newSize.x = CrashUtils.Clamp(newSize.x, -100f, 100f);
                newSize.y = CrashUtils.Clamp(newSize.y, -100f, 100f);
                newSize.y = CrashUtils.Clamp(newSize.y, -100f, 100f);

                boxCollider.size = newSize;
            }
            else if (collider is CapsuleCollider capsuleCollider)
            {
                capsuleCollider.radius = CrashUtils.Clamp(capsuleCollider.radius, -25f, 25f);
                capsuleCollider.height = CrashUtils.Clamp(capsuleCollider.height, -25f, 25f);

                //Center Safety
                Vector3 newCenter = capsuleCollider.center;

                newCenter.x = CrashUtils.Clamp(newCenter.x, -100f, 100f);
                newCenter.y = CrashUtils.Clamp(newCenter.y, -100f, 100f);
                newCenter.y = CrashUtils.Clamp(newCenter.y, -100f, 100f);

                capsuleCollider.center = newCenter;
            }
            else if (collider is SphereCollider sphereCollider)
            {
                sphereCollider.radius = CrashUtils.Clamp(sphereCollider.radius, -25f, 25f);

                //Center Safety
                Vector3 newCenter = sphereCollider.center;

                newCenter.x = CrashUtils.Clamp(newCenter.x, -100f, 100f);
                newCenter.y = CrashUtils.Clamp(newCenter.y, -100f, 100f);
                newCenter.y = CrashUtils.Clamp(newCenter.y, -100f, 100f);

                sphereCollider.center = newCenter;
            }

            return false;
        }

        public static bool ProcessJoint(Joint joint)
        {
            joint.connectedMassScale = CrashUtils.Clamp(joint.connectedMassScale, -25f, 25f);
            joint.massScale = CrashUtils.Clamp(joint.massScale, -25f, 25f);
            joint.breakTorque = CrashUtils.Clamp(joint.breakTorque, -100f, 100f);
            joint.breakForce = CrashUtils.Clamp(joint.massScale, -100f, 100f);

            if (joint is SpringJoint springJoint)
            {
                springJoint.damper = CrashUtils.Clamp(springJoint.damper, -100f, 100f);
                springJoint.maxDistance = CrashUtils.Clamp(springJoint.maxDistance, -100f, 100f);
                springJoint.minDistance = CrashUtils.Clamp(springJoint.minDistance, -100f, 100f);
                springJoint.spring = CrashUtils.Clamp(springJoint.spring, -100f, 100f);
                springJoint.tolerance = CrashUtils.Clamp(springJoint.tolerance, -100f, 100f);

                //Anchor Safety
                Vector3 newAnchor = springJoint.anchor;

                newAnchor.x = CrashUtils.Clamp(newAnchor.x, -100f, 100f);
                newAnchor.y = CrashUtils.Clamp(newAnchor.y, -100f, 100f);
                newAnchor.z = CrashUtils.Clamp(newAnchor.z, -100f, 100f);

                springJoint.anchor = newAnchor;

                //ConnectedAnchor Safety
                Vector3 newConnectedAnchor = springJoint.connectedAnchor;

                newConnectedAnchor.x = CrashUtils.Clamp(newConnectedAnchor.x, -100f, 100f);
                newConnectedAnchor.y = CrashUtils.Clamp(newConnectedAnchor.y, -100f, 100f);
                newConnectedAnchor.z = CrashUtils.Clamp(newConnectedAnchor.z, -100f, 100f);

                springJoint.connectedAnchor = newConnectedAnchor;
            }

            return false;
        }
    }
}
