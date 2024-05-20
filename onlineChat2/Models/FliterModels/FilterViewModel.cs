namespace onlineChat2.Models.FliterModels
{
	public class FilterViewModel
	{
		public FilterViewModel(string? SelectedSource, string SelectedTypeTheme, string? searchInput)
		{
			_SelectedSource = SelectedSource;
			_SelectedTypeTheme = SelectedTypeTheme;
			_searchInput = searchInput;
		}
		public string? _SelectedSource { get; private set; }   // выбранный источник обращения
		public string? _SelectedTypeTheme { get; private set; }   // выбранный тип вопроса обращения (юридичесский или техничесский)
		public string? _searchInput { get; private set; }   // введенный текст
	}
}
