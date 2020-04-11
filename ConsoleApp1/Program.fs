// Learn more about F# at http://fsharp.org

open System

[<EntryPoint>]
let main argv =
    Console.OutputEncoding <- System.Text.Encoding.Unicode
    let str = CryptoAlgorithms.KeyGenerator.KeyGenerator.getValues 50
    printfn "%s" str
    0 // return an integer exit code
