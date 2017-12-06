// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System
[<EntryPoint>]
let main argv = 
    let word = "word"

    let Guess (keyFn: Unit -> Char) = 
        let key = keyFn()
        if word |> String.exists (fun x -> key = x) then Some(key) else None
    
    let NewGuess (guesses: List<char>) (guess: char) =
        if guesses |> List.exists (fun x -> x = guess) then None else Some(guess)

    let rec Game (guesses: List<char>) (attempts: int) =
        let (>>=) x s = Option.bind s x

        let guess = Guess ((fun () -> Console.ReadKey(true)) >> (fun cki -> cki.KeyChar))
        let res = guess >>= (fun x -> NewGuess guesses x)
        let output = match res with
                     | Some x -> sprintf "Success char: %c" x
                     | None -> "Fail char"

        printfn "%s" output
        Game guesses attempts + 1

    let score = Game [] 0
    0 // return an integer exit code