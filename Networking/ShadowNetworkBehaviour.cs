using HawkNetworking;

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
