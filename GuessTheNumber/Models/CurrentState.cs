namespace GuessTheNumber.Models
{
    public class CurrentState
    {
        public string id { get; set; }
        public string name { get; set; }
        public int randomNumber { get; set; }
        public string difficulty { get; set; } = "easy";
        public int noOfGuesses { get; set; }
        public List<int> guessesList { get; set; } = [];
        public int score { get; set; }
        public int range { get; set; }
        public bool isNumberGuessed { get; set; }
    }
}
