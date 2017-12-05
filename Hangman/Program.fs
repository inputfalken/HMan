// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System
[<EntryPoint>]
let main argv = 
    let word = "word"
    let Guess (keyFn: Unit -> Char) = 
        let key = keyFn()
        if String.exists (fun x -> key = x) word then Some(key)
        else None

    let  CheckHistory (letter: Char Option) = 
        ()


    let guess = Guess ((fun () -> Console.ReadKey(true)) >> (fun cki -> cki.KeyChar))


    0 // return an integer exit code