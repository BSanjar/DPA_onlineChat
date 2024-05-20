using onlineChat2.Models.DB_Models;

namespace onlineChat2.Models.FliterModels
{
	public class IndexViewModel
	{
		public PaginatedList<Chat> Registers { get; set; }
		public FilterViewModel FilterViewModel { get; set; }
		public SortViewModel SortViewModel { get; set; }
		public int TotalRecords { get; set; }
	}
}
