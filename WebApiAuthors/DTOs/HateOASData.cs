namespace WebApiAuthors.DTOs
{
    public class HateOASData
    {
        public string Link { get; private set; }
        public string Description { get; private set; }
        public string Method { get; private set; }

        public HateOASData(string link, string description, string method)
        {
            Link = link;
            Description = description;
            Method = method;
        }
    }
}
