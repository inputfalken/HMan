module Game
open Stats
open System
open Config
    
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

let Game (word: string) (config: Config) = 
    let ({ 
            ClearWindow = clear
            StringInput = inputString
            CharInput = inputChar
            OutputString = output
            LetterMatcher = letterMatcher
    }) = config

    let rec SetMaxInvalidGuesses() =
        let tryParseInt str =
           match Int32.TryParse(str) with
           | (true, int) -> Some(int)
           | _ -> None

        match inputString() |> tryParseInt |> Option.filter (fun x -> x > 0) with 
        | Some x -> x
        | _ -> SetMaxInvalidGuesses()

    let OutputWordProgress word history =
         (word, history) ||> CorrectlyGuessedLetters |> letterMatcher |> output

    let rec Guess (history: List<char>) =
        let res = inputChar()
        clear()
        if (res, history) ||> List.contains then
            res |> sprintf "Letter '%c' has allready been guessed." |> output
            (word, history) ||>  OutputWordProgress
            Guess history
        else res
    
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
        | Some _ -> { Word = word; Attemps = attempts; MaxAttemps = maxAttempts; Guesses = history }
        | None -> (match correctGuess with
                   | Some x -> x |> correct 
                   | None ->  incorrect()) |||> Turn

    "Set max attempts" |> output
    let maxInvalidGuesses = SetMaxInvalidGuesses()
    maxInvalidGuesses |> sprintf  "Maximum attempts set to '%d'" |> output
    let stats = Turn [] 0 maxInvalidGuesses
    word |> sprintf "Game over, the word was: '%s'" |> output
    stats.Attemps |> sprintf "Invalid guesses: '%i'" |> output
    stats