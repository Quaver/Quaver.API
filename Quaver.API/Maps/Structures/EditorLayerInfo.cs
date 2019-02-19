using System;
using System.Drawing;
using YamlDotNet.Serialization;

namespace Quaver.API.Maps.Structures
{
    [Serializable]
    public class EditorLayerInfo
    {
        /// <summary>
        ///     The name of the layer
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Is the layer hidden in the editor?
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        ///     The color of the layer (default is white)
        /// </summary>
        public string ColorRgb { get; set; }

        /// <summary>
        ///     Converts the stringified color to a System.Drawing color
        /// </summary>
        /// <returns></returns>
        public Color GetColor()
        {
            if (ColorRgb == null)
                return Color.White;

            var split = ColorRgb.Split(',');

            try
            {
                return Color.FromArgb(byte.Parse(split[0]), byte.Parse(split[1]), byte.Parse(split[2]));
            }
            catch (Exception)
            {
                return Color.White;
            }
        }
    }
}