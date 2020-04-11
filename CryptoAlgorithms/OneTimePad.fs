namespace CryptoAlgorithms

module OneTimePad =
    let validateInput key text = Seq.length key = Seq.length text

    let encrypt (key:#seq<char>) (text:#seq<char>) = 
        Seq.zip key text |> 
        Seq.map (fun (k, t) -> (int k) ^^^ (int t) |> char)