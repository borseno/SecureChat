// Learn more about F# at http://fsharp.org

open System

let (|Int|_|) (str:string) =
    match System.Int32.TryParse str with
    | true,int -> Some int
    | _ -> None

let printKey length = 
        let str = Async.RunSynchronously (CryptoAlgorithms.Helpers.KeyGenerator.getValues length)
        printfn "%s" str

[<EntryPoint>]
let main argv =
    Console.OutputEncoding <- System.Text.Encoding.Unicode
    
    let invalidInput = true
    while invalidInput do
        printfn "enter a length of key to generate"
        let input = Console.ReadLine()
        match input with
        | Int num -> 
            printKey num
            printfn "-----------------------"
        | _ -> 
            printfn "error, try again"

    0 // return an integer exit code
