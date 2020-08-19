using System;
using System.Runtime.InteropServices;

namespace RustBotnet.SteamAuth
{
    [StructLayout(LayoutKind.Sequential, Size = 0xea, Pack = 1)]
    public struct Token
    {
        public uint Length;
        public ulong ID;
        public ulong SteamID;
        public uint ConnectionTime;
        public Session Session;
        public TokenInformation Information;

        public static Token Parse(byte[] buffer)
        {
            try
            {
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                Token local = (Token)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Token));
                handle.Free();
                return local;
            }
            catch
            {
            }
            return default(Token);
        }

        public static byte[] getBytes(Token str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public static byte[] CreateToken(ulong steamid)
        {
            Byte[] steam_token = Token.getBytes(new Token
            {
                Information = new TokenInformation
                {
                    Length = 182,
                    Unknown0x38 = 0,
                    Unknown0x50 = 0,
                    Unknown0x51 = 0,
                    Unknown0x52 = 0,
                    Unknown0x53 = 0,
                    Unknown0x54 = 0,
                    Unknown0x60 = 0,
                    Unknown0x61 = 0,
                    Unknown0x62 = 0,
                    Unknown0x63 = 0,
                    Unknown0x64 = 0,
                    Unknown0x66 = 0,
                    Unknown0x68 = 0,
                    EndedTime = 0,
                    StartTime = 0,
                    Unknown0x3C = 0,
                    Unknown0x4C = 0,
                    AppID = 480,
                    SHA128 = new byte[128],
                    UserID = steamid
                },
                Session = new Session
                {
                    Length = 28,
                    Unknown0x20 = 0,
                    Unknown0x24 = 0,
                    Unknown0x28 = 0,
                    ConnectNumber = 0,
                    Unknown0x1C = 0,
                    SessionID = 0
                },
                Length = 234,
                ConnectionTime = 0,
                ID = 0,
                SteamID = steamid
            });

            return steam_token;
        }
    }
}
