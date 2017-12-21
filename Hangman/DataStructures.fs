module DataStructures

type Config = {
    StringInput: unit -> string; 
    CharInput: unit -> char; 
    OutputString: string -> unit; 
    ClearWindow: unit -> unit;
    LetterMatcher: seq<Option<char>> -> string
}

type Stats = {
    Word: string
    Attemps: int
    MaxAttemps: int
    Guesses: List<char>
    GameWon: bool
}
