namespace Core.Rest
{
    public abstract class ResultBase<TData>
    {
        //       public string RawData { get; set; }
        public TData Data { get; set; }
        public Error Error { get; set; }
        public bool HasError => Error != null;

        protected ResultBase(string code, string message)
        {
            Error = new Error(code, message);
        }

        protected ResultBase(string message) 
            : this(null, message)
        {
        }

        protected ResultBase()
        {
        }
    }
}