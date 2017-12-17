module Program
open Game
open Config
open System


[<EntryPoint>]
let main argv =
    let config = {
        StringInput = Console.ReadLine; 
        CharInput = (fun () -> Console.ReadKey(true)) >> fun cki -> cki.KeyChar; 
        OutputString = Console.WriteLine; 
        ClearWindow = Console.Clear; 
        LetterMatcher = (fun x -> x |> Seq.map (fun x -> match x with | Some x -> x | None -> '-') |> Seq.toArray |> String)
    }

    let game = Game "woooord" config
    0
