module Program
open System
open System.IO
open Session
open DataStructures


let private printStat x =  
   ((if x.GameWon then "won" else "lost"), x.Word) ||> sprintf "Game was %s with the word %s."

[<EntryPoint>]
let main argv =

    let output = {
        ScoreHistory = List.iteri (fun i x -> (i ,x |> printStat) ||> printfn "%i: %s")
        MenuItems = List.iteri (fun i x -> (i + 1, x ) ||> printfn "%i: %s")
        CorrectGuess = printfn "Letter '%c' is correct!"
        IncorrectGuess = printfn "Letter '%c' is incorrect."
        AllreadyGuessed = printfn "Letter '%c' has allready been guessed."
        AttemptsSet = printfn "Number of attempts is set to '%i'."
        SetMaxAttempts = (fun () -> printf "Set max attempts: ")
        GameOver =  printStat >> printfn "%s"
        LetterMatcher = (fun x -> x |> Seq.map (fun x -> match x with | Some x -> x | _ -> '-') |> Seq.iter (fun x -> x |> printf "%c"))
    }

    let input = {
        Text = Console.ReadLine
        Letter = (fun () -> true |> Console.ReadKey) >> fun cki -> cki.KeyChar
    }

    let rnd = Random();
    let words = Lazy<string[]>.Create (fun () -> "./words.txt" |> File.ReadAllLines |> Array.toList)
    let randomizeListItem  (words: List<string>) = words |> (fun x -> x.Length |> rnd.Next |> (fun i -> x.Item i)) 
    let config = {
        NewFrame = Console.Clear
        WordSelector = (fun () -> words.Value |> randomizeListItem)
        Output = output         
        Input = input
    }
    let hangmanSession = (config, []) ||> Session
    0

