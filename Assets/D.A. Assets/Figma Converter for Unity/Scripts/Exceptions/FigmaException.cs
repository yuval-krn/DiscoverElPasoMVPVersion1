using System;

namespace DA_Assets.FCU.Exceptions
{
    public class FigmaException : Exception
    {
        public FigmaException(string message) : base(message)
        {

        }

        public FigmaException(Exception ex) : base(ex.Message)
        {

        }
    }
}