using PHC.Assets.Scripts.Art;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PHC.Shaders.Editor
{
    public class GameboiColorEditor : ShaderGUI
    {
        private static readonly List<string> s_colorProps =
            new List<string>(4)
            {
                "_GameboiColor1",
                "_GameboiColor2",
                "_GameboiColor3",
                "_GameboiColor4",
            };



        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            foreach (MaterialProperty eachProp in properties)
            {
                if (s_colorProps.Contains(eachProp.name))
                {
                    GColor gcolor = (long)eachProp.floatValue;
                    Color color = gcolor;
                    Color newColor = EditorGUILayout.ColorField(eachProp.displayName, color);
                    if(newColor != color)
                    {
                        gcolor = newColor;
                        eachProp.floatValue = gcolor.m_value;
                    }
                }
            }
        }
    }
}
