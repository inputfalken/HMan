module Program
open System
open System.IO
open Game
open DataStructures


[<EntryPoint>]
let main argv =
    let rnd = Random();
    let words = Lazy<string[]>.Create (fun () -> "./words.txt" |> File.ReadAllLines)
    let randomizeListItem  (words: string[]) = words |> (fun x -> x.Length |> rnd.Next |> (fun i -> (x, i) ||> Array.get))
    let config = {
        StringInput = Console.ReadLine
        CharInput = (fun () -> Console.ReadKey(true)) >> fun cki -> cki.KeyChar
        OutputString = Console.WriteLine
        ClearWindow = Console.Clear
        LetterMatcher = (fun x -> x |> Seq.map (fun x -> match x with | Some x -> x | None -> '-') |> Seq.toArray |> Console.WriteLine)
    }

    let stats = ((words.Value |> randomizeListItem), config) ||> Game
    ((if stats.GameWon then "won" else "lost"), stats.Word) ||> sprintf "Game %s, the word was: '%s'" |> Console.WriteLine
    stats.Attemps |> sprintf "Invalid guesses: '%i'" |> Console.WriteLine
    0

