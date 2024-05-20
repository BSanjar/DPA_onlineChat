namespace onlineChat2.Models.FliterModels
{
	public class SortViewModel
	{
		public SortState RegDate { get; private set; }   // значение для сортировки по дату регистрации
		public SortState Status { get; private set; }   // значение для сортировки по дату регистрации
		public SortState Current { get; private set; }     // текущее значение сортировки

		public SortViewModel(SortState sortOrder)
		{
			RegDate = sortOrder == SortState.RegDateAsc ? SortState.RegDateDesc : SortState.RegDateAsc;
			Status = sortOrder == SortState.StatusAsc ? SortState.StatusDesc : SortState.StatusAsc;
			Current = sortOrder;
		}
	}
}
