namespace DA_Assets.FCU.Exceptions
{
    public class InvalidAuthException : FigmaException
    {
        public InvalidAuthException() 
            : base(string.Format("Authentication aborted or failed."))
        {

        }
    }
}