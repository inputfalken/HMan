module Session
open System
open DataStructures
    
let private CorrectGuess letter word =
    letter |> Option.Some |> Option.filter(fun x -> word |> String.exists (fun y -> x = y))

let private SaveToHistory history item =
    (history, [item]) ||> List.append

let private Won word history attempts =
    word |> Some |> Option.filter (fun x -> x |> String.forall (fun x -> history |> List.contains x)) |> Option.map (fun _ -> attempts)

let private Lost maxAttempts attempts =
    attempts |> Some |> Option.filter (fun x -> x >= maxAttempts)

let private CorrectlyGuessedLetters word history =
    word |> Seq.map Some |> Seq.map (fun x -> x |> Option.filter (fun x -> (x, history) ||> Seq.contains))

let private TryParseInt str =
   match Int32.TryParse(str) with
   | (true, int) -> Some(int)
   | _ -> None
    
let rec Session config stats = 
    let ({ 
            NewFrame = newFrame
            WordSelector = wordSelector
            Output = output
            Input = input
    }) = config

    let word = wordSelector()

    let rec SetMaxInvalidGuesses() =
        match input.Text() |> TryParseInt |> Option.filter (fun x -> x > 0) with 
        | Some x -> x
        | _ -> SetMaxInvalidGuesses()

    let OutputWordProgress word history =
         (word, history) ||> CorrectlyGuessedLetters |> output.LetterMatcher

    let rec Guess history =
        let letter = input.Letter()
        newFrame()
        if (letter, history) ||> List.contains then
            letter |> output.AllreadyGuessed
            (word, history) ||>  OutputWordProgress
            Guess history
        else letter

    let rec Menu options = 
        if  options |> List.isEmpty then
            invalidArg "options" "List cannot be empty."
        else 
            options |> output.MenuItems
            match input.Text() |> TryParseInt |> Option.filter (fun x -> x <= options.Length) |> Option.filter (fun x -> x > 0) with
            | Some x -> x
            | _ -> options |> Menu
           
    
    let rec Turn history (attempts: GuessCount) maxAttempts =
        (word, history) ||> OutputWordProgress
        let letter = history |> Guess
        let history = (history, letter) ||> SaveToHistory
        let correctGuess = (letter, word) ||> CorrectGuess

        let correct letter =
            letter |> output.CorrectGuess
            (history, attempts, maxAttempts)
            
        let incorrect() =
            (letter, attempts) |> output.IncorrectGuess
            (history, attempts + 1, maxAttempts)

        let maybeLost = (maxAttempts, attempts) ||> Lost
        let maybeWon = correctGuess |> Option.bind (fun _ -> (word, history, attempts) |||> Won)
        let gameOver = (maybeLost, maybeWon) ||> Option.orElse

        match gameOver with
        | Some _ -> { Word = word; GuessCount = attempts; GuessLimit = maxAttempts; Guesses = history; GameOver = (attempts,  maxAttempts) ||> GameOver }
        | _ -> (match correctGuess with
                   | Some x -> (x, attempts) |> correct 
                   | _ ->  incorrect()) |||> Turn

    let StartGame() = 
        output.SetMaxAttempts()
        let maxInvalidGuesses = SetMaxInvalidGuesses()
        newFrame()
        maxInvalidGuesses |> output.AttemptsSet
        let score = Turn [] 0 maxInvalidGuesses
        score |> output.GameOver
        score

    let ShowScoreHistory() =
        newFrame()
        stats |> output.ScoreHistory
        (config, stats) ||> Session

    match Menu ["Start Game" ; "Show scores" ; "Quit"] with
    | 1 -> (config, stats |> List.append [(StartGame())]) ||> Session
    | 2 -> ShowScoreHistory()
    | 3 -> stats