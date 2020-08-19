using System;
using System.Collections.Generic;

namespace RustBotnet
{
    public class General
    {
        public static void Main(string[] args)
        {
            List<FakeClient> clients = new List<FakeClient>();

            for(int i = 0; i < Settings.FakeClients_Count; i++)
            {
                clients.Add(new FakeClient(Settings.GameVersion, (uint)i, String.Concat("notbot-", i), 76561198014350000 + (ulong)i));
            }

            long lastConnectedTime = 0;

            while (true)
            {
                try
                {
                    foreach (FakeClient client in clients)
                    {
                        if (client.GetConnectionStatus() == EConnectionStatus.DISCONNECTED)
                        {
                            long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                            long passTime = lastConnectedTime + Settings.Connect_Delay_MS;
                            if (passTime < currentTime)
                            {
                                lastConnectedTime = currentTime;
                                client.ConnectToServer(Settings.Server_IP, Settings.Server_Port);
                            }
                        }
                        else
                        {
                            client.CycleUpdate();

                            if (client.GetConnectionStatus() == EConnectionStatus.CONNECTED)
                            {
                                long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                                long passTime = client.GetLastMessageMillis() + Settings.Message_Delay_MS;
                                if (passTime < currentTime)
                                {
                                    client.SetLastMessageMillis(currentTime);
                                    client.SendChatMessage("\"I am real human!\"");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Oops, catched exception: " + ex.Message);
                }
            }
        }
    }
}
