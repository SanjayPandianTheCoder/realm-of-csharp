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

        private IWebHostEnvironment _env;
        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            levels = ["easy", "intermediate", "hard"];
            ranges = [50, 100, 500];
            _env = env;
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
            saveScore();
            return Page();

        }

        private int getRandomRumber(int uptoRange)
        {
            return Random.Shared.Next(1, uptoRange + 1);
        }

        private async void saveScore()
        {
            var currentPath = Path.Combine(_env.ContentRootPath, "Files", "scorecard.json");
            var fileContent = await System.IO.File.ReadAllTextAsync(currentPath);

            var scorecards = new List<CurrentState>();
            if(fileContent != "")
            {
                scorecards = JsonSerializer.Deserialize<List<CurrentState>>(fileContent) ?? new List<CurrentState>();
            }

            var existingScoreCard = scorecards.FirstOrDefault(x => x.id == currentState.id);
            if(existingScoreCard != null)
            {
                existingScoreCard.guessesList = currentState.guessesList;
                existingScoreCard.noOfGuesses = currentState.noOfGuesses;
                existingScoreCard.isNumberGuessed = currentState.isNumberGuessed;
                existingScoreCard.score = currentState.score;
            }

            else
            {
                scorecards.Add(currentState);
            }
            
            var serializedScoreCard = JsonSerializer.Serialize(scorecards);
            await System.IO.File.WriteAllTextAsync(currentPath, serializedScoreCard);

        }
    }
}
