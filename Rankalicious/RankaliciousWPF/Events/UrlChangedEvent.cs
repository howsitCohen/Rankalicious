namespace RankaliciousWPF.Events
{
    public class UrlChangedEvent
    {
        public UrlChangedEvent(string url)
        {
            Url = url;
        }

        public string Url { get; private set; }

    }
}
