namespace Blog.Notifications
{
    public class Notification<T>
    {
        public T Data { get; private init; }
        private List<string> Error { get; init; } = new List<string>();
        public IReadOnlyCollection<string> Notifications => Error;



        public Notification(T data, List<string> errors)
        {
            Data = data;
            Error = errors;
        }

        public Notification(T data)
        {
            Data = data;
        }

        public Notification(List<string> errors)
        {
            Error = errors;
        }

        public Notification(string error)
        {
            Error.Add(error);
        }
    }
}
