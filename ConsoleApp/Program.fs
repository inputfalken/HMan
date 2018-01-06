module Program
open System
open System.IO
open Session
open DataStructures
open Newtonsoft.Json


let private stringFormatStat (x: Stats) =  
   ((if x.GameWon then "won" else "lost"), x.Word) ||> sprintf "Game was %s with the word %s."

let private GetScoreHistory path =
    try 
        path |> File.ReadAllText  |> JsonConvert.DeserializeObject<List<Stats>>
    with
        | :? System.IO.IOException as ex -> printfn "Failed reading score file: %s" ex.Message ; []
        | :? JsonSerializationException as ex -> printfn "Failed creating score history: %s" ex.Message ; []

[<EntryPoint>]
let main argv =
    let scoreFile = "scores.json"

    let output = {
        ScoreHistory = List.map stringFormatStat >> List.iteri (fun i x -> (i, x) ||> printfn "%i: %s")
        MenuItems = Array.iteri (fun i x -> (i + 1, x ) ||> printfn "%i: %A")
        CorrectGuess = printfn "Letter '%c' is correct!"
        IncorrectGuess = printfn "Letter '%c' is incorrect."
        AllreadyGuessed = printfn "Letter '%c' has allready been guessed."
        AttemptsSet = printfn "Number of attempts is set to '%i'."
        SetMaxAttempts = (fun () -> printf "Set max attempts: ")
        GameOver =  stringFormatStat >> printfn "%s"
        LetterMatcher = (fun x -> x |> Seq.map (fun x -> match x.Status with | Guessed  _ ->  x.Char | Unguessed _ -> '-') |> Seq.iter (fun x -> x |> printf "%c"))
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
    let hangmanSession = (config, scoreFile |> GetScoreHistory) ||> Session

    File.WriteAllText(scoreFile, (hangmanSession |> JsonConvert.SerializeObject))
    0

