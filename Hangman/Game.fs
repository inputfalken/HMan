module Game
open System
    
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

let Game (word: string) (input: unit -> char) (output: string -> unit) (clear: unit -> unit) = 
        
    let rec SetMaxInvalidGuesses (inputs: List<char>) =
        clear()
        sprintf "Max attempts: %s" (inputs |> List.toArray |> String) |> output
        let userInput = input()
        let isDone = userInput |> Some |> Option.filter (fun x -> x = 'y') |> Option.filter (fun _ -> inputs.Length > 0)
        let inputs = if userInput |> Char.IsNumber then (inputs, [userInput]) ||> List.append else inputs

        match isDone with
        | Some _ -> inputs
        | None ->  inputs |> SetMaxInvalidGuesses

    let OutputWordProgress word history =
         (word, history) ||> CorrectlyGuessedLetters |> Seq.map (fun x -> match x with | Some x -> x | None -> '_') |> Seq.toArray |> String |> output

    let rec Guess (history: List<char>) =
        let res = input()
        clear()
        if (res, history) ||> List.contains then
            res |> sprintf "Letter '%c' has allready been guessed." |> output
            (word, history) ||>  OutputWordProgress
            Guess history
        else res
    
    let rec Turn (history: List<char>) (attempts: int) (maxAttempts: int) =
        let letter = history |> Guess
        let history = (history, letter) ||> SaveToHistory
        let correctGuess = (letter, word) ||> CorrectGuess

        let correct letter =
            letter |> sprintf "Letter '%c' is correct!" |> output
            (history, attempts , maxAttempts)
            
        let incorrect() =
            sprintf  "Incorrect Guess" |> output
            (history, attempts + 1, maxAttempts)

        (word, history) ||> OutputWordProgress

        let maybeLost = (maxAttempts, attempts) ||> Lost
        let maybeWon = correctGuess |> Option.bind (fun _ -> (word, history, attempts) |||> Won)
        let gameOver = (maybeLost, maybeWon) ||> Option.orElse

        match gameOver with
        | Some x -> x
        | None -> (match correctGuess with
                   | Some x -> x |> correct 
                   | None ->  incorrect()) |||> Turn

    "Set max attempts" |> output
    let maxInvalidGuesses = [] |> SetMaxInvalidGuesses |> List.toArray |> String |> int
    maxInvalidGuesses |> sprintf  "Maximum attempts set to '%d'" |> output
    let score = Turn [] 0 maxInvalidGuesses
    score |> sprintf "Game finished, wrong guesses: '%i'" |> output

    if score < maxInvalidGuesses then Some(score) else None