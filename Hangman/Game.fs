module Game
open System

let rec private SetMaxAttempts (input: unit -> char) (output: string -> unit) (inputs: List<char>) =
    "Input max attempts" |> output
    let userInput = input()
    let isDone = userInput |> Some |> Option.filter (fun x -> x = 'y') |> Option.filter (fun _ -> inputs.Length > 0)
    let inputs = if userInput |> Char.IsNumber then (inputs, [userInput]) ||> List.append else inputs

    match isDone with
    | Some _ -> inputs
    | None -> (input, output, inputs) |||> SetMaxAttempts
    
let private CorrectGuess (letter: char) (word: string) =
    word |> Option.Some |> Option.filter(fun x -> x |> String.exists (fun x -> letter = x) )

let private SaveToHistory (history: List<char>) (item: char) =
    (history, [item]) ||> List.append

let private Won word history attempts =
    if word |> String.forall (fun x -> history |> List.contains x) then Some(attempts) else None

let private Lost maxAttempts attempts =
    attempts |> Some |> Option.filter (fun x -> x >= maxAttempts)

let private CorrectlyGuessedLetters (word: string) history =
    word |> Seq.map Some |> Seq.map (fun x -> x |> Option.filter (fun x -> (x, history) ||> Seq.contains))

let Game (word: string) (input: unit -> char) (output: string -> unit) (clear: unit -> unit) = 
        
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

    let rec Turn (history: List<char>) (attempts: int) (maxAttempts: int)=
        let letter = history |> Guess
        let history = (history, letter) ||> SaveToHistory
        let correctGuess = (letter, word) ||> CorrectGuess

        match correctGuess with
        | Some _ -> letter |> sprintf "Letter '%c' is correct!" |> output
        | None -> letter |> sprintf "Letter '%c' is incorrect!" |> output

        (word, history) ||> OutputWordProgress

        // This is some if game is lost or won otherwise it's none
        match (maxAttempts, attempts) ||> Lost |> (correctGuess |> Option.bind (fun _ -> (word, history, attempts) |||> Won) |> Option.orElse) with
        | Some x -> x
        | None -> match correctGuess with
                  | Some x -> (history, attempts, maxAttempts) |||> Turn 
                  | None -> (history, (attempts + 1), maxAttempts) |||> Turn 

    "Set max attempts" |> output
    let maxAttempts = (input, output, []) |||> SetMaxAttempts |> List.toArray |> String |> int
    maxAttempts |> sprintf  "Maximum attempts set to '%d'" |> output
    let attempts = Turn [] 0 maxAttempts
    attempts |> sprintf "Game finished, attempts required: '%i'" |> output

    if attempts < maxAttempts then Some(attempts) else None