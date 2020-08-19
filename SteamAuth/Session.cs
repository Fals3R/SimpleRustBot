using System.Runtime.InteropServices;

namespace RustBotnet.SteamAuth
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Session
    {
        public uint Length;
        public uint Unknown0x1C;
        public uint Unknown0x20;
        public uint Unknown0x24;
        public uint Unknown0x28;
        public uint SessionID;
        public uint ConnectNumber;
    }
}
