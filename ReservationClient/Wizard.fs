namespace Ploeh.Samples

open System
open CommandLineReservationsApi

#nowarn "40"
module Wizard =

    let rec readQuantity = commandLine {
        do!  CommandLine.writeLine "Please enter number of diners:"
        let! l = CommandLine.readLine
        match Int32.TryParse l with
        | true, dinerCount -> return dinerCount
        | _ ->
            do! CommandLine.writeLine "Not an integer."
            return! readQuantity }

    let rec readDate = commandLine {
        do!  CommandLine.writeLine "Please enter your desired date:"
        let! l = CommandLine.readLine
        match DateTimeOffset.TryParse l with
        | true, dt -> return dt
        | _ ->
            do! CommandLine.writeLine "Not a date."
            return! readDate }

    let readName = commandLine {
        do! CommandLine.writeLine "Please enter your name:"
        return! CommandLine.readLine }

    let readEmail = commandLine {
        do! CommandLine.writeLine "Please enter your email address:"
        return! CommandLine.readLine }

    let tryReserve = commandLineReservationsApi {
        let! count = liftCL readQuantity
        let! date = liftCL readDate
        let! availableSeats =
            ReservationsApi.getSlots date
            |> ReservationsApi.map (List.sumBy (fun slot -> slot.SeatsLeft))
            |> liftRA
        if availableSeats < count
        then do!
            sprintf "Only %i remaining seats." availableSeats
            |> CommandLine.writeLine
            |> liftCL
        else
            let! name = liftCL readName
            let! email = liftCL readEmail
            do! { Date = date; Name = name; Email = email; Quantity = count }
                |> ReservationsApi.postReservation 
                |> liftRA
        }
