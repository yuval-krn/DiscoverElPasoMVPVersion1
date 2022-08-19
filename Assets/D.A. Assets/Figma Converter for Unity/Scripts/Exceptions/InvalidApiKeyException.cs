namespace DA_Assets.FCU.Exceptions
{
    public class InvalidApiKeyException : FigmaException
    {
        public InvalidApiKeyException() 
            : base(string.Format("Need new authentication."))
        {

        }
    }
}
