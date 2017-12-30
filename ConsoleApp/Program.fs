module Program
open System
open System.IO
open Session
open DataStructures


[<EntryPoint>]
let main argv =
    let rnd = Random();
    let words = Lazy<string[]>.Create (fun () -> "./words.txt" |> File.ReadAllLines |> Array.toList)
    let randomizeListItem  (words: List<string>) = words |> (fun x -> x.Length |> rnd.Next |> (fun i -> x.Item i)) 
    let config = {
        StringInput = Console.ReadLine
        CharInput = (fun () -> Console.ReadKey(true)) >> fun cki -> cki.KeyChar
        OutputString = Console.WriteLine
        ClearWindow = Console.Clear
        LetterMatcher = (fun x -> x |> Seq.map (fun x -> match x with | Some x -> x | None -> '-') |> Seq.toArray |> Console.WriteLine)
        WordSelector = (fun () -> words.Value |> randomizeListItem)
    }
    let hangmanSession = (config, []) ||> Session
    0

