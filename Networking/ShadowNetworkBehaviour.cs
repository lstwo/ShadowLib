using HawkNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowLib.Networking
{
    public class ShadowNetworkBehaviour : HawkNetworkBehaviour
    {
        public ShadowNetworkBehaviour()
        {
            PluginNetworkManager.AddNetworkBehaviour(this);
        }
    }
}
