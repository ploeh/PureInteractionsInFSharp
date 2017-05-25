(* This module communicates with an HTTP API that enables you to make two types
   of requests:
   - get the availability for a particular date
   - post a reservation
   The assumption is that you host this API on your local machine. Modify the
   baseAddress value if you need to.

   This code base doesn't include an HTTP API, so you'll have to create one
   yourself. While you can fairly easily create a fake service in whichever
   language you'd like, if you've participated in my 'Outside-in TDD with F#'
   workshop, you should have a copy of a full code base that matches the API
   used here. That server-side code base includes code for input validation,
   database access, unit testing, and much more. If you haven't been in that
   workshop, but would like to, please contact me.
   See http://blog.ploeh.dk/hire-me for more details. *)

module Ploeh.Samples.ReservationHttpClient

open System
open System.Net.Http
open FSharp.Data

type ReservationJson = JsonProvider<"""
{
    "date": "some date",
    "name": "Mark Seemann",
    "email": "mark@ploeh.dk",
    "quantity": 4
}""">

type AvailabilityJson = JsonProvider<"""
{
    "openings": [
        {
            "date": "some date",
            "seats": 10
        }
    ]
}""">

let private baseAddress = "http://localhost:56268"

let private toSlot (o : AvailabilityJson.Opening) =
    { Date = DateTimeOffset.Parse o.Date; SeatsLeft = o.Seats }

let getSlots (d : DateTimeOffset) = async {
    use  client = new HttpClient ()
    use! response =
        sprintf "%s/availability/%i/%i/%i" baseAddress d.Year d.Month d.Day
        |> client.GetAsync
        |> Async.AwaitTask
    let! content = response.Content.ReadAsStringAsync () |> Async.AwaitTask
    let  a = AvailabilityJson.Parse content
    return a.Openings |> Array.map toSlot |> Array.toList }

let postReservation r = async {
    use  client = new HttpClient ()
    let  json =
        ReservationJson.Root(r.Date.ToString "o", r.Name, r.Email, r.Quantity)
        |> string
    use content = new StringContent (json)
    content.Headers.ContentType.MediaType <- "application/json"
    let! response =
        client.PostAsync (sprintf "%s/reservations" baseAddress, content)
        |> Async.AwaitTask
    printfn "%A" response.StatusCode }
