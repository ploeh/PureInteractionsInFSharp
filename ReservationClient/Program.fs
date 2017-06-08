module Ploeh.Samples.Program

open System

let rec interpret = function
    | Pure x -> x
    | Free (ReadLine  next) -> Console.ReadLine () |> next |> interpret
    | Free (WriteLine (s, next)) ->
        Console.WriteLine s
        next |> interpret

[<EntryPoint>]
let main _ =
    let program =
        Free (WriteLine (
                "Please enter your name.",
                Free (ReadLine (
                        fun s -> Free (WriteLine (
                                        sprintf "Hello, %s!" s,
                                        Pure ()))))))
    interpret program
    0 // return an integer exit code
