open System
[<EntryPoint>]
let main argv = 
    let Hangman  (word: string) (inputFn: Unit -> char) (outputFn: string -> Unit) = 
        let CorrectGuess (key: char, word: string) = 
            if word |> String.exists (fun x -> key = x) then Some(key) else None

        let SaveToHistory (history: List<char>) (item: char) =
            history |> List.append [item]

        let GameWon word history =
            if word |> String.forall (fun x ->  history |> List.contains x)  then Some(word) else None
        
        let rec Guess (history: list<char>) = 
            let res = inputFn()
            if history |> List.contains res then 
                sprintf "Letter %c has allready been guessed." res |> outputFn
                Guess history
            else
                res 

        let rec Game (history: List<char>) (attempts: int) =
            let letter = Guess history
            let history = SaveToHistory history letter
            let correctGuess = CorrectGuess(letter, word) 

            match correctGuess with
            | Some letter -> sprintf "Letter '%c' is correct!" letter |> outputFn
            | None -> sprintf "Letter '%c' is incorrect!" letter |> outputFn

            match correctGuess |> Option.bind (fun _ -> GameWon word history) with
            | Some _ -> attempts
            | None -> Game history attempts + 1

        

        let attempts = Game [] 1
        sprintf "Game finished, attempts required: '%i'" attempts |> outputFn
    
    Hangman "word" ((fun () -> Console.ReadKey(true)) >> fun cki -> cki.KeyChar) Console.WriteLine
    0