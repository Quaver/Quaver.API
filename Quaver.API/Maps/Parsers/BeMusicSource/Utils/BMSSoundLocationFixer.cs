using System.IO;

namespace Quaver.API.Maps.Parsers.BeMusicSource.Utils
{
    public class BMSSoundEffectFixer
    {
        /// <summary>
        ///     It's pretty common for a file to be defined in a BMS file as #WAVXX file.wav,
        ///     but actually be something like file.ogg. This will attempt to correct that by
        ///     renaming the file to the correct extension when it's moved into the .qua file.
        /// </summary>
        /// <returns></returns>
        public static string SearchForSoundFile(string originalPath, string pathToSoundFile)
        {
            var pathToSoundFileNoExt = pathToSoundFile.TrimEnd(Path.GetExtension(pathToSoundFile).ToCharArray());
            foreach (var possibleExtension in new[]{".wav", ".mp3", ".ogg", ".3gp"})
            {
                if (File.Exists(Path.Combine(originalPath, pathToSoundFileNoExt) + possibleExtension))
                {
                    return pathToSoundFileNoExt + possibleExtension;
                }
            }

            return null;
        }
    }
}