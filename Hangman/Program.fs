module Program
open Game
open Config
open System
open System.IO


[<EntryPoint>]
let main argv =
    let rnd = Random();
    let words = "./words.txt" |> File.ReadAllLines 
    let randomizeListItem  (words: string[]) = words |> (fun x -> x.Length |> rnd.Next |> (fun i -> (x, i) ||> Array.get))
    
    let config = {
        StringInput = Console.ReadLine; 
        CharInput = (fun () -> Console.ReadKey(true)) >> fun cki -> cki.KeyChar; 
        OutputString = Console.WriteLine; 
        ClearWindow = Console.Clear; 
        LetterMatcher = (fun x -> x |> Seq.map (fun x -> match x with | Some x -> x | None -> '-') |> Seq.toArray |> String)
    }

    let game = ((words |> randomizeListItem), config) ||> Game 
    0
