namespace Ploeh.Samples

type CommandLineInstruction<'a> =
| ReadLine of (string -> 'a)
| WriteLine of string * 'a

type CommandLineProgram<'a> =
| Free of CommandLineInstruction<CommandLineProgram<'a>>
| Pure of 'a

module CommandLine =
    // Underlying functor
    let private mapI f = function
        | ReadLine next -> ReadLine (next >> f)
        | WriteLine (x, next) -> WriteLine (x, next |> f)

    let rec bind f = function
        | Free instruction -> instruction |> mapI (bind f) |> Free
        | Pure x -> f x

    let map f = bind (f >> Pure)

    let readLine = Free (ReadLine Pure)

    let writeLine s = Free (WriteLine (s, Pure ()))

type CommandLineBuilder () =
    member this.Bind (x, f) = CommandLine.bind f x
    member this.Return x = Pure x
    member this.ReturnFrom x = x
    member this.Zero () = Pure ()

[<AutoOpen>]
module CommandLineComputationExpression =
    let commandLine = CommandLineBuilder ()
