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
    Wizard.readReservationRequest
    |> CommandLine.bind (CommandLine.writeLine << (sprintf "%A"))
    |> interpret
    0 // return an integer exit code
