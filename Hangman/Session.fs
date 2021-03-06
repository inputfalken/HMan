﻿module Session 
open System
open DataStructures
    
let private CorrectGuess letter word =
    letter |> Option.Some |> Option.filter(fun x -> word |> String.exists (fun y -> x = y))

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
          word |> Seq.map (fun x -> if (x, history) ||> List.contains then { Status = Status.Guessed ; Char = x} else { Status = Status.Unguessed ; Char = x }) |> output.LetterMatcher

    let rec Guess history =
        let letter = input.Letter()
        newFrame()
        if (letter, history) ||> List.contains then
            letter |> output.AllreadyGuessed
            (word, history) ||>  OutputWordProgress
            Guess history
        else letter

    let rec Menu (options: MenuItem[]) = 
        if  options |> Array.isEmpty then
            invalidArg "options" "List cannot be empty."

        else 
            options |> output.MenuItems
            match input.Text() |> TryParseInt |> Option.filter (fun x -> x <= options.Length) |> Option.filter (fun x -> x > 0) |> Option.map (fun x -> options.[x - 1]) with
            | Some x -> x
            | _ -> options |> Menu
    
    let rec Turn history attempts maxAttempts =
        (word, history) ||> OutputWordProgress
        let letter = history |> Guess
        let history = letter :: history  // Add to history
        let correctGuess = (letter, word) ||> CorrectGuess

        let correct letter =
            letter |> output.CorrectGuess
            (history, attempts, maxAttempts)
            
        let incorrect() =
            letter |> output.IncorrectGuess
            (history, attempts + 1, maxAttempts)

        let maybeLost = (maxAttempts, attempts) ||> Lost
        let maybeWon = correctGuess |> Option.bind (fun _ -> (word, history, attempts) |||> Won)
        let gameOver = (maybeLost, maybeWon) ||> Option.orElse

        match gameOver with
        | Some _ -> { Word = word; Attemps = attempts; MaxAttemps = maxAttempts; Guesses = history; }
        | _ -> (match correctGuess with
                   | Some x -> x |> correct 
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

    match Menu [|Start ; Score; Quit|] with
    | Start -> (config, StartGame() :: stats) ||> Session
    | Score -> ShowScoreHistory()
    | Quit -> stats