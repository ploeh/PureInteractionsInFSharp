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
    commandLine {
        do!  CommandLine.writeLine "Please enter your name."
        let! name = CommandLine.readLine
        do!  sprintf "Hello, %s!" name |> CommandLine.writeLine }
    |> interpret
    0 // return an integer exit code
