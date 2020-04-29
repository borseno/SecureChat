// cd ..\SecureChat\ConsoleApp1\bin\Debug\netcoreapp3.1
// KeyGenerator_Sample.exe length=100 path=..\SecureChat\SecureChat.Client\bin\Debug\netcoreapp3.1\result.txt

open Microsoft.Extensions.Configuration
open System.IO

let (|Int|_|) (str:string) =
    match System.Int32.TryParse str with
    | true,int -> Some int
    | _ -> None
    
let validateKeyLength length =
    match length with
    | Int a -> a > 0 && a <= 250000
    | _ -> false
    
[<EntryPoint>]
let main argv =
    let config = ConfigurationBuilder().AddCommandLine(argv).Build()
    let length = config.["length"]
    let path = config.["path"]
    
    if validateKeyLength length then
        let str = Async.RunSynchronously (CryptoAlgorithms.Helpers.KeyGenerator.getValues (int length) )
        File.AppendAllText(path, str)
                               
    0