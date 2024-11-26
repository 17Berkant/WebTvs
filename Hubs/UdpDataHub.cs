using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace WebTvs.Hubs
{
    public class UdpDataHub : Hub
    {
        public async Task SendUdpData(string message)
        {
            // İstemcilere gelen veriyi gönderir
            await Clients.All.SendAsync("ReceiveUdpData", message);
        }
    }
}
