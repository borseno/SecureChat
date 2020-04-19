namespace CryptoAlgorithms.Helpers


module RandomOrg =
    open FSharp.Data
    open System

    let formatRequest = sprintf "https://www.random.org/integers/?num=%i&min=%i&max=%i&col=1&base=10&format=plain&rnd=new"
    
    let private _next amount (min, max) =
        async {
            let! nums = Http.AsyncRequestString(formatRequest amount min max)
            let nums = nums.Split(Seq.toArray (seq {"\n"}), StringSplitOptions.RemoveEmptyEntries) in
            return Seq.take (nums.Length - 1) nums 
            |> Seq.map (fun i -> Int32.Parse(i))
        }

    let private splitNumberBy maxnum num = 
        seq {
            let amount, remainder = num / maxnum, num % maxnum in
            for i = 1 to amount do
                10000
            if remainder > 0 then 
                remainder
        }

    let next amount (min, max) = 
        async {
            if amount = 0 
                then return Seq.empty<int>            
            elif amount > 10000 then             
                let! p = splitNumberBy 10000 amount             
                        |> Seq.map (fun i -> _next i (min,max))
                        |> Async.Parallel
                return Seq.concat p
            else
                return! ( _next amount (min,max) )
        }
module KeyGenerator =
    open System
    
    let getValues count =
        async {
            let! randomSeq = RandomOrg.next count (1040, 1103) in
            return randomSeq |> Seq.map (fun i -> char i) |> Seq.toArray |> String
        }