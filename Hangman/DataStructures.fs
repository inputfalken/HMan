module DataStructures

type Stats = {
    Word: string
    Attemps: int
    MaxAttemps: int
    Guesses: List<char>
    GameWon: bool
}

type Output = {
    ScoreHistory: List<Stats> -> unit
    Menu: List<string> -> unit
    CorrectGuess: char -> unit
    IncorrectGuess: char -> unit
    AllreadyGuessedLetter: char -> unit
    AttemptsSet: int -> unit
    Score: Stats -> unit
    Message: string -> unit
    LetterMatcher: seq<Option<char>> -> unit
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