﻿module Program
open Game
open System

[<EntryPoint>]
let main argv =
    let game = Game "woooord" ((fun () -> Console.ReadKey(true)) >> fun cki -> cki.KeyChar) Console.WriteLine Console.Clear 
    0