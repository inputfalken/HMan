module Config

type Config = {
        StringInput: unit -> string; 
        CharInput: unit -> char; 
        OutputString: string -> unit; 
        ClearWindow: unit -> unit;
        LetterMatcher: seq<Option<char>> -> string
}