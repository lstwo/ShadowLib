using HawkNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShadowLib.Networking
{
    public abstract class ShadowNetworkBehaviour : HawkNetworkBehaviour
    {
        public ShadowNetworkBehaviour()
        {
            PluginNetworkManager.AddNetworkBehaviour(this);
        }
    }
}
