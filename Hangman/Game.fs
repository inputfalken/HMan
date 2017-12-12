module Game
open System

let Game (word: string) (input: unit -> char) (output: string -> unit) (clear:  unit -> unit) = 

    let CorrectGuess (key: char) (word: string) =
        if word |> String.exists (fun x -> key = x) then Some(key) else None

    let SaveToHistory (history: List<char>) (item: char) =
        (history, [item]) ||> List.append

    let Won word history =
        if word |> String.forall (fun x -> history |> List.contains x) then Some(2) else None

    let Lost maxAttempts attempts =
        if attempts >= maxAttempts then Some(attempts) else None

    let WordProgress (word: string) history =
        word |> Seq.toList |> Seq.map (fun x -> if (x, history) ||> Seq.contains then Some(x) else None)

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

    let rec Turn (history: List<char>) (attempts: int) (maxAttempts: int)=
        let letter = history |> Guess
        let history = (history, letter) ||> SaveToHistory
        let correctGuess = (letter, word) ||> CorrectGuess

        match correctGuess with
        | Some _ -> letter |> sprintf "Letter '%c' is correct!" |> output
        | None -> letter |> sprintf "Letter '%c' is incorrect!" |> output

        (word, history) ||> OutputWordProgress

        // This is some if game is lost or won otherwise it's none
        match (maxAttempts, attempts) ||> Lost |> (correctGuess |> Option.bind (fun _ -> (word, history) ||> Won) |> Option.orElse) with
        | Some x -> x
        | None -> Turn history (attempts + 1) maxAttempts
         
    let maxAttempts = 5
    maxAttempts |> sprintf  "Maximum attempts set to '%d'" |> output
    let attempts = Turn [] 1 maxAttempts
    attempts |> sprintf "Game finished, attempts required: '%i'" |> output

    if attempts < maxAttempts then Some(attempts) else None
