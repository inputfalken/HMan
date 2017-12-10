open System
[<EntryPoint>]
let main argv =
    let CorrectGuess (key: char) (word: string) =
        if word |> String.exists (fun x -> key = x) then Some(key) else None

    let SaveToHistory (history: List<char>) (item: char) =
        (history, [item]) ||> List.append

    let GameWon word history =
        if word |> String.forall (fun x -> history |> List.contains x) then Some(word) else None
    
    let WordProgress (word: string) history =
        word |> Seq.toList |> List.map (fun x -> if (x, history) ||> List.contains then Some(x) else None)

    let Game (word: string) (input: Unit -> char) (output: string -> Unit) (clear:  Unit -> Unit) = 
        let OutputWordProgress word history =
            (word, history) ||> WordProgress |> List.map (fun x -> match x with | Some x -> x | None -> '_') |> List.toArray |> (fun s -> s |> String) |> output

        let rec Guess (history: list<char>) =
            let res = input()
            clear()
            if (res, history) ||> List.contains then
                sprintf "Letter %c has allready been guessed." res |> output
                (word, history) ||>  OutputWordProgress
                Guess history
            else res

        let rec Turn (history: List<char>) (attempts: int) =
            let letter = history |> Guess
            let history = (history, letter) ||> SaveToHistory
            let correctGuess = (letter, word) ||> CorrectGuess

            match correctGuess with
            | Some letter -> sprintf "Letter '%c' is correct!" letter |> output
            | None -> sprintf "Letter '%c' is incorrect!" letter |> output

            (word, history) ||> OutputWordProgress
            match correctGuess |> Option.bind (fun _ -> GameWon word history) with
            | Some _ -> attempts
            | None -> Turn history attempts + 1

        let attempts = Turn [] 1
        sprintf "Game finished, attempts required: '%i'" attempts |> output
    
    Game "woooord" ((fun () -> Console.ReadKey(true)) >> fun cki -> cki.KeyChar) Console.WriteLine Console.Clear
    0