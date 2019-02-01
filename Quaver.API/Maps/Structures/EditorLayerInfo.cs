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
    }
}