module Session
open System
open DataStructures
    
let private CorrectGuess (letter: char) (word: string) =
    letter |> Option.Some |> Option.filter(fun x -> word |> String.exists (fun y -> x = y))

let private SaveToHistory (history: List<char>) (item: char) =
    (history, [item]) ||> List.append

let private Won word history attempts =
    word |> Some |> Option.filter (fun x -> x |> String.forall (fun x -> history |> List.contains x)) |> Option.map (fun _ -> attempts)

let private Lost maxAttempts attempts =
    attempts |> Some |> Option.filter (fun x -> x >= maxAttempts)

let private CorrectlyGuessedLetters (word: string) history =
    word |> Seq.map Some |> Seq.map (fun x -> x |> Option.filter (fun x -> (x, history) ||> Seq.contains))
let private TryParseInt str =
   match Int32.TryParse(str) with
   | (true, int) -> Some(int)
   | _ -> None
    
let rec Session (config: Config) (stats: List<Stats>) = 
    let ({ 
            ClearWindow = clear
            StringInput = inputString
            CharInput = inputChar
            OutputString = output
            LetterMatcher = letterMatcher
            WordSelector = wordSelector
    }) = config

    let word = wordSelector()

    let rec SetMaxInvalidGuesses() =
        match inputString() |> TryParseInt |> Option.filter (fun x -> x > 0) with 
        | Some x -> x
        | _ -> SetMaxInvalidGuesses()

    let OutputWordProgress word history =
         (word, history) ||> CorrectlyGuessedLetters |> letterMatcher

    let rec Guess (history: List<char>) =
        let res = inputChar()
        clear()
        if (res, history) ||> List.contains then
            res |> sprintf "Letter '%c' has allready been guessed." |> output
            (word, history) ||>  OutputWordProgress
            Guess history
        else res

    let rec Menu (options : List<string>)  = 
        if options.Length > 0 then
            options |> List.mapi (fun i x -> (i + 1, x) ||> sprintf "%i: %s") |> List.iter output
            match inputString() |> TryParseInt |> Option.filter (fun x -> x <= options.Length) |> Option.filter (fun x -> x > 0) with
            | Some x -> x
            | None -> options |> Menu
        else invalidArg "options" "List cannot be empty."
    
    let rec Turn (history: List<char>) (attempts: int) (maxAttempts: int) =
        (word, history) ||> OutputWordProgress
        let letter = history |> Guess
        let history = (history, letter) ||> SaveToHistory
        let correctGuess = (letter, word) ||> CorrectGuess

        let correct letter =
            letter |> sprintf "Letter '%c' is correct!" |> output
            (history, attempts , maxAttempts)
            
        let incorrect() =
            sprintf  "Incorrect Guess" |> output
            (history, attempts + 1, maxAttempts)

        let maybeLost = (maxAttempts, attempts) ||> Lost
        let maybeWon = correctGuess |> Option.bind (fun _ -> (word, history, attempts) |||> Won)
        let gameOver = (maybeLost, maybeWon) ||> Option.orElse

        match gameOver with
        | Some _ -> { Word = word; Attemps = attempts; MaxAttemps = maxAttempts; Guesses = history; GameWon = attempts < maxAttempts }
        | None -> (match correctGuess with
                   | Some x -> x |> correct 
                   | None ->  incorrect()) |||> Turn

    "Set max attempts" |> output
    let maxInvalidGuesses = SetMaxInvalidGuesses()
    maxInvalidGuesses |> sprintf  "Maximum attempts set to '%d'" |> output
    let score = Turn [] 0 maxInvalidGuesses
    ((if score.GameWon then "won" else "lost"), score.Word) ||> sprintf "Game %s, the word was: '%s'" |> output
    score.Attemps |> sprintf "Invalid guesses: '%i'" |> output

    match Menu ["Start Game" ; "Quit"] with
    | 1 -> (config, stats |> List.append [(score)]) ||> Session
    | 2 -> stats