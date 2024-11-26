using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WebTvs.Hubs;
using WebTvs.Models;

namespace WebTvs.Services
{
    public class UDPListenerService : BackgroundService
    {
        private readonly ConcurrentBag<TvsCheck> _receivedDataList;
        private readonly UdpClient _udpClient;
        private readonly IHubContext<UdpDataHub> _hubContext;

        private const int Port = 12001;

        public UDPListenerService(ConcurrentBag<TvsCheck> receivedDataList, IHubContext<UdpDataHub> hubContext)
        {
            _receivedDataList = receivedDataList;
            _udpClient = new UdpClient(Port);
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = await _udpClient.ReceiveAsync();
                    string receivedData = Encoding.UTF8.GetString(result.Buffer);

                    var tvsCheck = ParseTvsCheckData(receivedData);

                    if (tvsCheck != null)
                    {
                        if (tvsCheck.Quantity == "FTM")
                        {
                            _receivedDataList.Clear();
                        }
                        else
                        {
                            _receivedDataList.Add(tvsCheck);
                            await _hubContext.Clients.All.SendAsync("ReceiveUdpData", receivedData);
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Veri alma hatası: {ex.Message}");
                }
            }
        }

        private TvsCheck ParseTvsCheckData(string data)
        {
            var parts = data.Split('^');

            //if (parts.Length != 7)
            //{
            //    return null;
            //}

            try
            {
                return new TvsCheck
                {
                    Quantity = parts[0],
                    ItemName = parts[1],
                    ItemPrice = parts[2],
                    ItemType = parts[3],
                    CheckTotal = parts[4],
                    ItemObjectNum = parts[5],
                    IsCombo = parts[6],
                    IsCondiment = parts[7],
                    IsOpenPrice = parts[8]
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veri ayrıştırma hatası: {ex.Message}");
                return null;
            }
        }

        public override void Dispose()
        {
            _udpClient.Dispose();
            base.Dispose();
        }
    }
}
