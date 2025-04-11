
namespace mauiApp;

public partial class MainPage : ContentPage {
	public MainPage()	{
		InitializeComponent();
	}

	private void OnButtonClickedAsync(object sender, EventArgs e) {
		Task<Quote> task = Task.Run(() => DataManager.RandomQuote());
		task.Wait();
		var quote = task.Result;
		editText.Text = "\"{0}\" \n~{1}".FormatWith(quote.QuoteText, quote.Author);
	}
}

