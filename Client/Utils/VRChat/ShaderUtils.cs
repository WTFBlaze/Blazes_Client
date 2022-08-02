using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Blaze.Utils.VRChat
{
    internal static class ShaderUtils
    {
        private static Shader standardShader;
        private static Shader diffuseShader;

        internal static Shader GetStandardShader()
        {
            if (standardShader == null)
            {
                standardShader = Shader.Find("Standard");
            }

            return standardShader;
        }

        internal static Shader GetDiffuseShader()
        {
            if (diffuseShader == null)
            {
                diffuseShader = Shader.Find("Diffuse");
            }

            return diffuseShader;
        }
    }
}
