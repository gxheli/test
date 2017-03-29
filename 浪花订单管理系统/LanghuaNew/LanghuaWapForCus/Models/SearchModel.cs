namespace LanghuaWapForCus.Models
{
    public class SearchModel
    {
        public int draw { get; set; }
        public int length { get; set; }
        public int start { get; set; }
        public OrderByModel OrderBy { get; set; }
        public OrderSearchModel OrderSearch { get; set; }
    }
    public class OrderByModel
    {
        public string PropertyName { get; set; }
        public int OrderBy { get; set; }
    }
    public class OrderSearchModel
    {
        public string status { get; set; }
        public string NoShowCancel { get; set; }
    }
}