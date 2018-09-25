namespace Quaver.API.Replays.Virtual
{
    public class VirtualReplayKeyBinding
    {
        /// <summary>
        ///     The virtual replay key.
        /// </summary>
        public ReplayKeyPressState Key { get; }

        /// <summary>
        ///     If the key is currently pressed.
        /// </summary>
        public bool Pressed { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        public VirtualReplayKeyBinding(ReplayKeyPressState key) => Key = key;
    }
}