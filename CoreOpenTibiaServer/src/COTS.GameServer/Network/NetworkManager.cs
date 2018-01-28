using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using COTS.Infra.CrossCutting.Security;

namespace COTS.GameServer.Network
{
    public class NetworkManager
    {
        private List<NetworkService> _services;

        public NetworkManager()
        {
            _services = new List<NetworkService>();

            Rsa.SetKey("14299623962416399520070177382898895550795403345466153217470516082934737582776038882967213386204600674145392845853859217990626450972452084065728686565928113",
                           "7630979195970404721891201847792002125535401292779123937207447574596692788513647179235335529307251350570728407373705564708871762033017096809910315212884101");
        }

        public void Start()
        {
            Console.WriteLine("Starting NetworkManager");
            AddService<ProtocolLogin>(7171);
            AddService<ProtocolGame>(7172);
        }

        public bool AddService<ProtocolType>(UInt16 port) where ProtocolType : Protocol, new()
        {
            if(port == 0)
                return false;

            NetworkService service = new NetworkService(new ProtocolType(), port);
            _services.Add(service);

            service.Start();
            Console.WriteLine("Aaaa");

            return true;
        }

        public void Close()
        {
            Console.WriteLine("Closing NetworkManager");
            _services.ForEach( s =>
            {
                s.Close();
            });
        }
    }
}