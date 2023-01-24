namespace WebApiAuthors.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        private int recordsForPage = 10;
        private readonly int maximumAmountPage = 50;

        public int RecordsForPage
        {
            get
            {
                return recordsForPage;
            }

            set
            {
                recordsForPage = (value > maximumAmountPage) ? maximumAmountPage : value;
            }
        }
    }
}
