namespace Quaver.API.Replays
{
    public class ReplayFrame
    {
        /// <summary>
        ///     The time in the replay since the last frame.
        /// </summary>
        public int Time { get; }

        /// <summary>
        ///     The keys that were pressed during this frame.
        /// </summary>
        public ReplayKeyPressState Keys { get; }

        /// <summary>
        ///     Ctor -
        /// </summary>
        /// <param name="time"></param>
        /// <param name="keys"></param>
        public ReplayFrame(int time, ReplayKeyPressState keys)
        {
            Time = time;
            Keys = keys;
        }

        public override string ToString() => $"{Time}|{(int) Keys}";

        public string ToDebugString() => $"{Time}|{Keys}";
    }
}