using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Child_Talker
{
    public interface IChildTalkerTile
    {
        bool IsLink();
        void PerformAction();
        string Text { get; set; }
        string ImagePath { get; set; }
        IChildTalkerTile Parent { get; set; }
    }
}

