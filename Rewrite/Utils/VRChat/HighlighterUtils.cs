using System.Collections.Generic;
using UnityEngine;

namespace Blaze.Utils.VRChat
{
    public static class HighlighterUtils
    {
        public static List<HighlightsFXStandalone> SpawnedESPsHolders = new();

        private static void RemoveRendFromUnlistedHighLighter(Renderer rend)
        {
            HighlightsFX.prop_HighlightsFX_0.field_Protected_HashSet_1_Renderer_0.Remove(rend);
        }

        private static void RemoveRendFromUnlistedHighLighter(MeshRenderer rend)
        {
            HighlightsFX.prop_HighlightsFX_0.field_Protected_HashSet_1_Renderer_0.Remove(rend);
        }

        public static void SetHighLighter(this HighlightsFXStandalone item, Renderer rend, bool status)
        {
            if (item != null)
            {
                RemoveRendFromUnlistedHighLighter(rend);
                item.field_Protected_HashSet_1_Renderer_0.AddIfNotPresent(rend);
                item.Method_Public_Void_Renderer_Boolean_0(rend, status);
            }
        }

        public static void SetHighLighter(this HighlightsFXStandalone item, MeshRenderer rend, bool status)
        {
            if (item != null)
            {
                RemoveRendFromUnlistedHighLighter(rend);
                item.field_Protected_HashSet_1_Renderer_0.AddIfNotPresent(rend);
                item.Method_Public_Void_Renderer_Boolean_0(rend, status);
            }
        }

        public static void SetHighLighter(this HighlightsFXStandalone item, Renderer rend, Color color, bool status)
        {
            if (item != null)
            {
                RemoveRendFromUnlistedHighLighter(rend);
                item.field_Protected_HashSet_1_Renderer_0.AddIfNotPresent(rend);
                item.highlightColor = color;
                item.Method_Public_Void_Renderer_Boolean_0(rend, status);
            }
        }

        public static HighlightsFXStandalone AddHighlighter(this GameObject obj)
        {
            var item = obj.AddComponent<HighlightsFXStandalone>();
            if (item != null)
            {
                if (!SpawnedESPsHolders.Contains(item))
                {
                    SpawnedESPsHolders.Add(item);
                }
            }

            return item;
        }

        public static void DestroyHighlighter(this HighlightsFXStandalone item)
        {
            if (item != null)
            {
                if (SpawnedESPsHolders.Contains(item))
                {
                    SpawnedESPsHolders.Remove(item);
                }
            }
            UnityEngine.Object.DestroyImmediate(item);
        }

        public static void SetHighLighter(this HighlightsFXStandalone item, MeshRenderer rend, Color color, bool status)
        {
            if (item != null)
            {
                RemoveRendFromUnlistedHighLighter(rend);
                item.field_Protected_HashSet_1_Renderer_0.AddIfNotPresent(rend);
                item.highlightColor = color;
                item.Method_Public_Void_Renderer_Boolean_0(rend, status);
            }
        }

        public static void SetHighLighterColor(this HighlightsFXStandalone item, Color color)
        {
            if (item != null && item.field_Protected_Material_0 != null)
            {
                item.highlightColor = color;
            }
        }

        public static void ResetHighlighterColor(this HighlightsFXStandalone item)
        {
            if (item != null && item.highlightColor != null)
            {
                item.highlightColor = new Color(0.5019608f, 0.5019608f, 0.5019608f, 1f);
            }
        }
    }
}
