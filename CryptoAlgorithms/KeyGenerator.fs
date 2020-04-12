namespace CryptoAlgorithms.Helpers

module RandomOrg =
    open FSharp.Data
    open System

    let formatRequest = sprintf "https://www.random.org/integers/?num=%i&min=%i&max=%i&col=1&base=10&format=plain&rnd=new"
    
    let private _next amount (min, max) =
        let nums = Http.RequestString(formatRequest amount min max).Split("\n")
        Seq.take (nums.Length - 1) nums 
        |> Seq.map (fun i -> Int32.Parse(i))

    let next amount (min, max) =
        if amount = 0 
            then Seq.empty<int>            
        elif amount > 10000
        then 
            let amount, remainder = amount / 10000, amount % 10000 in 
            Seq.init (amount) (fun _ -> 10000)
            |> Seq.append ( seq { remainder } )
            |> Seq.map (fun i -> _next i (min,max))
            |> Seq.concat
        else
            _next amount (min,max)

module KeyGenerator =
    open System
    
    let getValues count =
        RandomOrg.next count (1040, 1103)
        |> Seq.map (fun i -> char i) |> Seq.toArray |> String


