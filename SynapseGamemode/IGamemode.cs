using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynapseGamemode
{
    public interface IGamemode
    {
        void Init();
        void Start();
        void End();
    }
}
