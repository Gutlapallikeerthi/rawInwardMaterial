namespace ERP_API.Moduls
{
    public class StoreAddReadOnlyDto
    {
        public int? StoreAddId { get; set; }

        public int? StoreAddNo { get; set; }

        public DateTime? StoreAddDate { get; set; }

        public int? InMatId { get; set; }

        public int? Source { get; set; }

        public int? RefDocId { get; set; }

        public int? StoreId { get; set; }

        public List<StoreAddSubReadOnlyDto> Storeaddsubs { get; set; }
    }
}
