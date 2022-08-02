using Blaze.Utils.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib.XrefScans;
using VRC;
using VRC.UI.Elements.Menus;

namespace Blaze.Utils
{
    internal static class BlazesXRefs
    {
        private static MethodInfo _FriendlyNameTargetMethod;
        private static List<MethodInfo> _ColorForRankMethods;
        private static MethodInfo _OnPhotonPlayerJoinMethod;
        private static MethodInfo _OnPhotonPlayerLeftMethod;
        private static MethodInfo _OnMenuOpened;
        private static MethodInfo _OnMenuClosed;
        private static MethodInfo _placeUi;
        private static MethodInfo _ActionWheelMethod;
        private static List<MethodInfo> _OnPlayerSelectedMethod;
        private static MethodInfo _reloadAllAvatarsMethod;
        private static MethodInfo _reloadAvatarMethod;
        private static MethodInfo _applyPlayerMotionMethod;

        internal static MethodInfo FriendNameTargetMethod
        {
            get
            {
                if (_FriendlyNameTargetMethod != null)
                {
                    return _FriendlyNameTargetMethod;
                }
                return _FriendlyNameTargetMethod = typeof(VRCPlayer).GetMethods()
                    .Where(it => !it.Name.Contains("PDM") && 
                    it.ReturnType.ToString().Equals("System.String") && 
                    it.GetParameters().Length == 1 && 
                    it.GetParameters()[0].ParameterType.ToString().Equals("VRC.Core.APIUser")).FirstOrDefault();
            }
        }

        internal static List<MethodInfo> ColorForRankMethods
        {
            get
            {
                if (_ColorForRankMethods != null)
                {
                    return _ColorForRankMethods;
                }
                return _ColorForRankMethods = typeof(VRCPlayer).GetMethods()
                    .Where(it => it.ReturnType.ToString().Equals("UnityEngine.Color") && 
                    it.GetParameters().Length == 1 && 
                    it.GetParameters()[0].ParameterType.ToString().Equals("VRC.Core.APIUser")).ToList();
            }
        }

        internal static MethodInfo OnPhotonPlayerJoinMethod
        {
            get
            {
                if (_OnPhotonPlayerJoinMethod != null)
                {
                    return _OnPhotonPlayerJoinMethod;
                }
                return _OnPhotonPlayerJoinMethod = typeof(NetworkManager).GetMethods().Single(delegate (MethodInfo it)
                {
                    if (it.ReturnType == typeof(void) && it.GetParameters().Length == 1 && it.GetParameters()[0].ParameterType == typeof(Photon.Realtime.Player))
                    {
                        return XrefScanner.XrefScan(it).Any(jt =>
                        {
                            if (jt.Type == XrefType.Global)
                            {
                                Il2CppSystem.Object @object = jt.ReadAsObject();
                                if (@object != null)
                                {
                                    if (@object.ToString().Contains("Enter"))
                                    {
                                        //Logs.Log($"[XREF] Found [JOIN] Method! [{it.Name} with {@object.ToString()}]");
                                        _OnPhotonPlayerJoinMethod = it;
                                        return true;
                                    }
                                }
                                return false;
                            }
                            return false;
                        });
                    }
                    return false;
                });
            }
        }
        internal static MethodInfo OnPhotonPlayerLeftMethod
        {
            get
            {
                if (_OnPhotonPlayerLeftMethod != null)
                {
                    return _OnPhotonPlayerLeftMethod;
                }
                return _OnPhotonPlayerLeftMethod = typeof(NetworkManager).GetMethods().Single(delegate (MethodInfo it)
                {
                    if (it.ReturnType == typeof(void) && it.GetParameters().Length == 1 && it.GetParameters()[0].ParameterType == typeof(Photon.Realtime.Player))
                    {
                        return XrefScanner.XrefScan(it).Any(jt =>
                        {
                            if (jt.Type == XrefType.Global
                            )
                            {
                                Il2CppSystem.Object @object = jt.ReadAsObject();
                                if (@object != null)
                                {
                                    if (@object.ToString().Contains("Left"))
                                    {
                                        //Logs.Log($"[XREF] Found [Left] Method! [{it.Name} with {@object.ToString()}]");
                                        _OnPhotonPlayerLeftMethod = it;
                                        return true;
                                    }
                                }
                                return false;
                            }
                            return false;
                        });
                    }
                    return false;
                });
            }
        }

        internal static MethodInfo OnMenuOpened
        {
            get
            {
                if (_OnMenuOpened == null)
                {
                    _OnMenuOpened = (from m in typeof(QuickMenu).GetMethods()
                                     where m.Name.StartsWith("Method_Private_Void_")
                                     where m.Name.Length <= 22
                                     where (from s in (from x in XrefScanner.XrefScan(m)
                                                       where x.Type == 0
                                                       select x).Select(delegate (XrefInstance x)
                                                       {
                                                           Object @object = x.ReadAsObject();
                                                           if (@object == null)
                                                           {
                                                               return null;
                                                           }
                                                           return @object.ToString();
                                                       })
                                            where s.StartsWith("Mic")
                                            select s).Count() == 3
                                     select m).FirstOrDefault();
                }
                return _OnMenuOpened;
            }
        }

        internal static MethodInfo OnMenuClosed
        {
            get
            {
                if (_OnMenuClosed == null)
                {
                    _OnMenuClosed = (from m in typeof(QuickMenu).GetMethods()
                                     where m.Name.StartsWith("Method_Public_Void_Boolean_")
                                     where m.Name.Length <= 29
                                     orderby XrefScanner.XrefScan(m).Count(x => x.Type == (XrefType)1)
                                     select m).ElementAt(3);
                }
                return _OnMenuClosed;
            }
        }

        internal static MethodInfo PlaceUiMethod
        {
            get
            {
                if (_placeUi == null)
                {
                    try
                    {
                        var xrefs = XrefScanner.XrefScan(typeof(VRCUiManager).GetMethod(nameof(VRCUiManager.LateUpdate)));
                        foreach (var x in xrefs)
                        {
                            if (x.Type == XrefType.Method && x.TryResolve() != null &&
                                x.TryResolve().GetParameters().Length == 2 &&
                                x.TryResolve().GetParameters().All(a => a.ParameterType == typeof(bool)))
                            {
                                _placeUi = (MethodInfo)x.TryResolve();
                                break;
                            }
                        };
                    }
                    catch
                    {
                    }
                }
                return _placeUi;
            }
        }

        internal static MethodInfo ActionWheelMethod
        {
            get
            {
                if (_ActionWheelMethod == null)
                {
                    _ActionWheelMethod = typeof(ActionMenu).GetMethods().FirstOrDefault(it => XrefScanner
                    .XrefScan(it).Any(delegate (XrefInstance jt)
                    {
                        if (jt.Type == XrefType.Global)
                        {
                            Il2CppSystem.Object someObject = jt.ReadAsObject();
                            return (someObject?.ToString()) == "Emojis";
                        }

                        return false;
                    }));
                }
                return _ActionWheelMethod;
            }
        }

        internal static List<MethodInfo> OnPlayerSelectedMethod
        {
            get
            {
                if (_OnPlayerSelectedMethod == null)
                {
                    List<MethodInfo> tmpList = new();
                    foreach (var method in typeof(SelectedUserMenuQM).GetMethods())
                    {
                        if (!method.Name.StartsWith("Method_Private_Void_IUser_PDM_")) continue;
                        if (XrefScanner.XrefScan(method).Count() < 3) continue;
                        tmpList.Add(method);
                    }
                    _OnPlayerSelectedMethod = tmpList;
                }
                return _OnPlayerSelectedMethod;
            }
        }

        internal static MethodInfo LoadAvatarMethod
        {
            get
            {
                if (_reloadAvatarMethod == null)
                {
                    _reloadAvatarMethod = typeof(VRCPlayer).GetMethods().First(mi => mi.Name.StartsWith("Method_Private_Void_Boolean_") && mi.Name.Length < 31 && mi.GetParameters().Any(pi => pi.IsOptional) && XRefManager.CheckUsedBy(mi, "ReloadAvatarNetworkedRPC"));
                }
                return _reloadAvatarMethod;
            }
        }

        internal static MethodInfo ReloadAllAvatarsMethod
        {
            get
            {
                if (_reloadAllAvatarsMethod == null)
                {
                    _reloadAllAvatarsMethod = typeof(VRCPlayer).GetMethods().First(mi => mi.Name.StartsWith("Method_Public_Void_Boolean_") && mi.Name.Length < 30 && mi.GetParameters().All(pi => pi.IsOptional) && XRefManager.CheckUsedBy(mi, "Method_Public_Void_", typeof(FeaturePermissionManager)));// Both methods seem to do the same thing;
                }

                return _reloadAllAvatarsMethod;
            }
        }

        internal static MethodInfo ApplyPlayerMotionMethod
        {
            get
            {
                if (_applyPlayerMotionMethod == null)
                {
                    _applyPlayerMotionMethod = typeof(VRCTrackingManager).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(m => m.Name.StartsWith("Method_Public_Static_Void_Vector3_Quaternion")
                    && !m.Name.Contains("_PDM_")).First(m => XrefScanner.UsedBy(m).Any(xrefInstance => xrefInstance.Type == XrefType.Method 
                    && xrefInstance.TryResolve()?.ReflectedType?.Equals(typeof(VRC_StationInternal)) == true));
                }
                return _applyPlayerMotionMethod;
            }
        }
    }
}
