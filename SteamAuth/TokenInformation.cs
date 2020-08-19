using System.Runtime.InteropServices;

namespace RustBotnet.SteamAuth
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TokenInformation
    {
        public int Length;
        public int Unknown0x38;
        public int Unknown0x3C;
        public ulong UserID;
        public int AppID;
        public int Unknown0x4C;
        public byte Unknown0x50;
        public byte Unknown0x51;
        public byte Unknown0x52;
        public byte Unknown0x53;
        public uint Unknown0x54;
        public uint StartTime;
        public uint EndedTime;
        public byte Unknown0x60;
        public byte Unknown0x61;
        public byte Unknown0x62;
        public byte Unknown0x63;
        public short Unknown0x64;
        public short Unknown0x66;
        public short Unknown0x68;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x80)]
        public byte[] SHA128;
    }
}
