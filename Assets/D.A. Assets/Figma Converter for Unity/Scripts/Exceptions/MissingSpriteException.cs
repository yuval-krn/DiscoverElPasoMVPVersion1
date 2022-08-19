#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA_Assets.FCU.Exceptions
{
    public class MissingSpriteException : FigmaException
    {
        public MissingSpriteException(string spriteName)
            : base($"'{spriteName.TextBold()}' sprite missing. Re-download required.")
        {

        }
    }
}
#endif