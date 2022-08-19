#if UNITY_EDITOR
namespace DA_Assets.FCU.Exceptions
{
    public class NoSelectedPageException : FigmaException
    {
        public NoSelectedPageException() 
            : base(string.Format("The page for exporting frames is not selected."))
        {

        }
    }
}
#endif