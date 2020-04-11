namespace CryptoAlgorithms.KeyGenerator

module RandomOrg =
    open FSharp.Data
    open System

    let formatRequest = sprintf "https://www.random.org/integers/?num=%i&min=%i&max=%i&col=1&base=10&format=plain&rnd=new"

    let next amount (min, max) = 
        let nums = Http.RequestString(formatRequest amount min max).Split("\n")
        Seq.take (nums.Length - 1) nums 
        |> Seq.map (fun i -> Int32.Parse(i))

module KeyGenerator =

    open System
    
    let getValues count =
        RandomOrg.next count (1040, 1103)
        |> Seq.map (fun i -> char i) |> Seq.toArray |> String


