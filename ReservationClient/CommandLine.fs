namespace Ploeh.Samples

type CommandLineInstruction<'a> =
| ReadLine of (string -> 'a)
| WriteLine of string * 'a

type CommandLineProgram<'a> =
| Free of CommandLineInstruction<CommandLineProgram<'a>>
| Pure of 'a
