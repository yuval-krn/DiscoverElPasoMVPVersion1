#if UNITY_EDITOR
using System.Collections.Generic;

namespace DA_Assets.FCU.Exceptions
{
    public class InvalidSettingsException : FigmaException
    {
        public InvalidSettingsException(List<string> errors)
            : base(string.Join("\n", errors))
        {

        }
    }
}
#endif