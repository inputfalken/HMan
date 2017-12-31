﻿module DataStructures

type Stats = {
    Word: string
    Attemps: int
    MaxAttemps: int
    Guesses: List<char>
    GameWon: bool
}

type Output = {
    /// Occurs when the 'Show Scores' option is choosen.
    ScoreHistory: List<Stats> -> unit
    /// Occurs first with game options, each list member is one option.
    MenuItems: List<string> -> unit
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