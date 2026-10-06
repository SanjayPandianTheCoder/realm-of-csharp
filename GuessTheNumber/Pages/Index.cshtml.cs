using GuessTheNumber.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace GuessTheNumber.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public List<string> levels;
        public List<int> ranges;
        public CurrentState currentState = new CurrentState();

        [BindProperty]
        public GameStartBind gameStart { get; set; }

        [BindProperty]
        public int Guess { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            levels = ["easy", "intermediate", "hard"];
            ranges = [50, 100, 500];
        }

        public void OnGet()
        {
            
        }

        public void OnPost()
        {
            Console.WriteLine("GAME STARTED");
        }

        public IActionResult OnPostGameStart()
        {
            var range = ranges[levels.IndexOf(levels.First(x => x == gameStart.difficulty))];
            var randomNumber = getRandomRumber(range);

            currentState = new CurrentState();
            currentState.id = Guid.NewGuid().ToString();
            currentState.noOfGuesses = 0;
            currentState.randomNumber = randomNumber;
            currentState.name = gameStart.name;
            currentState.difficulty = gameStart.difficulty;
            currentState.score = 0;
            currentState.range = range;
            currentState.guessesList = [];
            currentState.isNumberGuessed = false;

            var serializedCurrentState = JsonSerializer.Serialize(currentState);
            HttpContext.Session.SetString("CurrentState", serializedCurrentState);

            Console.WriteLine(serializedCurrentState);

            return Page();
        }

        public IActionResult OnPostSubmitGuess()
        {
            var serializedJson = HttpContext.Session.GetString("CurrentState") ?? "";
            if (serializedJson == "") return Page();

            currentState = JsonSerializer.Deserialize<CurrentState>(serializedJson) ?? new CurrentState();
            if(currentState.randomNumber == 0) return Page();

            var isMatch = Guess == currentState.randomNumber;

            currentState.noOfGuesses += 1;
            currentState.guessesList.Add(Guess);
            if (isMatch)
            {
                currentState.score = 100 / currentState.noOfGuesses;
                currentState.isNumberGuessed = true;
            }

            var serializedCurrentState = JsonSerializer.Serialize(currentState);
            HttpContext.Session.SetString("CurrentState", serializedCurrentState);

            Console.WriteLine(JsonSerializer.Serialize(currentState));

            return Page();

        }

        private int getRandomRumber(int uptoRange)
        {
            return Random.Shared.Next(1, uptoRange + 1);
        }
    }
}
