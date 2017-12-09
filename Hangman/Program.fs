﻿open System
[<EntryPoint>]
let main argv = 
    let word = "word"
    let CorrectGuess (key: char, word: string) = 
        if word |> String.exists (fun x -> key = x) then Some(key) else None

    let SaveToHistory (history: List<char>) (item: char) =
        history |> List.append [item]

    let GameWon word history =
        if word |> String.forall (fun x ->  history |> List.contains x)  then Some(word) else None
    
    let rec Guess (history: list<char>) (guessFn: Unit -> Char) = 
        let res = guessFn()
        if history |> List.contains res then 
            printf "Letter %c has allready been guessed.\n" res
            Guess history guessFn
        else
            res 

    let rec Game (history: List<char>) (attempts: int) =
        let letter = Guess history ((fun () -> Console.ReadKey(true)) >> fun cki -> cki.KeyChar)
        let history = SaveToHistory history letter
        let correctGuess = CorrectGuess(letter, word) 

        printf "%s\n" (match correctGuess with
                       | Some letter -> sprintf "Letter '%c' is correct!" letter
                       | None -> sprintf "Letter '%c' is incorrect!" letter)

        match correctGuess |> Option.bind (fun _ -> GameWon word history) with
        | Some _ -> attempts
        | None -> Game history attempts + 1


    let attempts = Game [] 1
    printf "Game finished, attempts required: '%i'" attempts
    0