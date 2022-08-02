using UnityEngine;

namespace Blaze.Utils.VRChat
{
    public static class ShaderUtils
    {
        private static Shader standardShader;
        private static Shader diffuseShader;

        public static Shader GetStandardShader()
        {
            if (standardShader == null)
            {
                standardShader = Shader.Find("Standard");
            }

            return standardShader;
        }

        public static Shader GetDiffuseShader()
        {
            if (diffuseShader == null)
            {
                diffuseShader = Shader.Find("Diffuse");
            }

            return diffuseShader;
        }
    }
}
