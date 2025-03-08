
using System.Threading.Tasks;

var runLoop = true;
ShowMenu();
while(runLoop) {
  var inputValue = Console.ReadLine();
  switch(inputValue){
    case "a":
      AddNewQuoteAsync();
      break;
    case "r":
      RandomQuoteAsync();
      break;
    case "l":
      ListAllQuotesAsync();
      break;
    case "q":    
      runLoop = false;
      break;
  }
}

Console.WriteLine("\nDone. Press enter.");
Console.ReadLine();

void ShowMenu() {
  Console.WriteLine(string.Empty);
  Console.WriteLine("|== Quotes Main Menu ==|");
  Console.WriteLine(string.Empty);
  Console.WriteLine("a: Add New Quote");
  Console.WriteLine("r: Get Random Quote");
  Console.WriteLine("l: List All Quotes");
  Console.WriteLine("q: Quit");
  Console.WriteLine(string.Empty);
  Console.Write("=> Option: ");
}

async void RandomQuoteAsync() {
  var quote = await DataManager.RandomQuote();  
  Console.WriteLine(string.Empty);
  Console.WriteLine("== Todays Quote ==");
  Console.WriteLine(string.Empty);
  Console.WriteLine($"\"{quote.QuoteText}\"");
  Console.WriteLine($"-{quote.Author}");
  Console.WriteLine(string.Empty);
  ShowMenu();
}

async void ListAllQuotesAsync() {
  try {
    Console.WriteLine(string.Empty);
    Console.WriteLine("== Listing Quotes ==");
    var quotes = await DataManager.QuoteListAsync();
    quotes.ForEach(q => {
      Console.WriteLine(string.Empty);
      Console.WriteLine($"\"{q.QuoteText}\"");
      Console.WriteLine($"-{q.Author}");
      Console.WriteLine(string.Empty);
    });
  } catch (Exception e) {
    Console.WriteLine(e.ToString());
  }
  ShowMenu();
}

void AddNewQuoteAsync() {
  try {
    Console.WriteLine(string.Empty);
    Console.WriteLine("== Adding new Quote ==");
    var quote = new Quote();
    Console.WriteLine("> Author:");
    var input = Console.ReadLine();
    quote.Author = string.IsNullOrEmpty(input) ? "" : input;
    Console.WriteLine();
    Console.WriteLine("> Quote:");
    input = Console.ReadLine();
    quote.QuoteText = string.IsNullOrEmpty(input) ? "" : input;
    _ = DataManager.SaveQuoteAsync(quote);
    Console.WriteLine(string.Empty);
  } catch (Exception e) {
    Console.WriteLine(e.ToString());
  }
  ShowMenu();
}
