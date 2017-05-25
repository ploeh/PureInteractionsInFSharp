namespace Ploeh.Samples

type CommandLineReservationsApiT<'a> =
| Run of CommandLineProgram<ReservationsApiProgram<'a>>

type CommandLineReservationsApiProgram<'a> =
| Free of CommandLineReservationsApiT<CommandLineReservationsApiProgram<'a>>
| Pure of 'a

module CommandLineReservationsApi =
    // Underlying functor
    let private mapStack f x = commandLine {
        let! x' = x
        return ReservationsApi.map f x' }

    let private mapT f (Run p) = mapStack f p |> Run

    let rec bind f = function
        | Free instruction -> instruction |> mapT (bind f) |> Free
        | Pure x -> f x

    let map f = bind (f >> Pure)

    let private wrap x = x |> Run |> mapT Pure |> Free
    let liftCL x = wrap <| CommandLine.map ReservationsApiProgram.Pure x
    let liftRA x = wrap <| CommandLineProgram.Pure x

type CommandLineReservationsApiBuilder () =
    member this.Bind (x, f) = CommandLineReservationsApi.bind f x
    member this.Return x = Pure x
    member this.ReturnFrom x = x
    member this.Zero () = Pure ()

[<AutoOpen>]
module CommandLineReservationsApiComputationExpression =
    let commandLineReservationsApi = CommandLineReservationsApiBuilder ()
