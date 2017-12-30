module Program
open System
open System.IO
open Session
open DataStructures


[<EntryPoint>]
let main argv =
    let output = {
        ScoreHistory = (fun x -> x |> List.iteri (fun i x -> (i, (if x.GameWon then "won" else "lost"), x.Word) |||> sprintf "%i: Game %s, the word was: '%s'" |> Console.WriteLine))
        Menu = (fun x -> x |> List.iteri (fun i x -> (i + 1,x ) ||> printfn "%i: %s"))
        CorrectGuess = (fun x -> x |> printfn "Letter '%c' is correct!")
        IncorrectGuess = (fun x -> x |> printfn "Letter '%c' is incorrect.")
        AllreadyGuessedLetter = (fun x -> x |> sprintf "Letter '%c' has allready been guessed." |> Console.WriteLine)
        AttemptsSet = (fun x -> x |> printfn "Number of attempts is set to '%i'.")
        Score = (fun x -> ((if x.GameWon then "won" else "lost"), x.Word) ||> sprintf "%s %s" |> Console.WriteLine)
        Message = Console.WriteLine
        LetterMatcher = (fun x -> x |> Seq.map (fun x -> match x with | Some x -> x | None -> '-') |> Seq.toArray |> Console.WriteLine)
    }
    let input = {
        Text = Console.ReadLine
        Letter = (fun () -> Console.ReadKey(true)) >> fun cki -> cki.KeyChar
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

