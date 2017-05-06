#r "packages/FAKE/tools/FakeLib.dll"

open Fake

Target "Build" <| fun _ ->
    !! "**/InteractiveDependencies.sln"
    |> MSBuildRelease "" "Rebuild"
    |> ignore

RunTargetOrDefault "Build"