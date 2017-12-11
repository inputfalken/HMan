module Game
open System

let CorrectGuess (key: char) (word: string) =
    if word |> String.exists (fun x -> key = x) then Some(key) else None

let SaveToHistory (history: List<char>) (item: char) =
    (history, [item]) ||> List.append

let Won word history =
    if word |> String.forall (fun x -> history |> List.contains x) then Some(word) else None

let Lost maxAttempts attempts =
    if attempts >= maxAttempts then Some(attempts) else None

let WordProgress (word: string) history =
    word |> Seq.toList |> Seq.map (fun x -> if (x, history) ||> Seq.contains then Some(x) else None)

let Game (word: string) (input: unit -> char) (output: string -> unit) (clear:  unit -> unit) = 
    let OutputWordProgress word history =
         (word, history) ||> WordProgress |> Seq.map (fun x -> match x with | Some x -> x | None -> '_') |> Seq.toArray |> String |> output

    let rec Guess (history: List<char>) =
        let res = input()
        clear()
        if (res, history) ||> List.contains then
            res |> sprintf "Letter '%c' has allready been guessed." |> output
            (word, history) ||>  OutputWordProgress
            Guess history
        else res

    let rec Turn (history: List<char>) (attempts: int) =
        let letter = history |> Guess
        let history = (history, letter) ||> SaveToHistory
        let correctGuess = (letter, word) ||> CorrectGuess

        match correctGuess with
        | Some _ -> letter |> sprintf "Letter '%c' is correct!" |> output
        | None -> letter |> sprintf "Letter '%c' is incorrect!" |> output

        (word, history) ||> OutputWordProgress
        match correctGuess |> Option.bind (fun _ -> (word, history) ||> Won ) with
        | Some _ -> attempts
        | None -> Turn history attempts + 1

    let attempts = Turn [] 1
    attempts |> sprintf "Game finished, attempts required: '%i'" |> output
