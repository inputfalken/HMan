module DataStructures

type MenuItem = Start | Score | Quit

type Stats = {
    Word: string
    Attemps: int
    MaxAttemps: int
    Guesses: List<char>
} with member this.GameWon = this.Attemps < this.MaxAttemps

type Status =
     | Guessed
     | Unguessed

type Letter = {
    Status: Status
    Char: char
}

type Output = {
    /// Occurs when the 'Show Scores' option is choosen.
    ScoreHistory: List<Stats> -> unit
    /// Occurs first with game options, each list member is one option.
    MenuItems: MenuItem[] -> unit
    /// Occurs when guessed letter is a correct guess.
    CorrectGuess: char -> unit
    /// Occurs when guessed letter is an incorrect guess.
    IncorrectGuess: char -> unit
    /// Occurs when guessed letter has allready been guessed.
    AllreadyGuessed: char -> unit
    /// Occurs when the input input of max attempts is approved.
    AttemptsSet: int -> unit
    /// Occurs before program expects an input of max attempts.
    SetMaxAttempts: unit -> unit
    /// Occurs when a game has ended.
    GameOver: Stats -> unit
    /// Occurs every time an guess has been evaluated.
    LetterMatcher: seq<Letter> -> unit
}

type Input = {
    Text: unit -> string
    Letter: unit -> char
}

type Config = {
    NewFrame: unit -> unit
    WordSelector: unit -> string
    Output: Output
    Input: Input
}