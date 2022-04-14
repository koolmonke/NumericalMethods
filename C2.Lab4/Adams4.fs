module C2.Lab4.Adams4

open System
open MatrixArithmetic
open Operators

let solve f12 n tau rungeInits =
    let rungeKutta =
        RungeKutta.solve f12 n tau rungeInits
        |> Seq.take 5
        |> Seq.toArray

    let y1 = Vector(5)
    let y2 = Vector(5)
    for i in 0 .. 4 do
        y1.[i] <- fst rungeKutta.[i]
        y2.[i] <- snd rungeKutta.[i]

    let adams =
        seq {
            for i in 4 .. (n - 2) do
                let tN = tau * float i

                let newY1, newY2 =
                    (y1.[Index(i % 5)], y2.[Index(i % 5)])
                    ++ (tau
                         .* (
                              ((55. / 24.) .* f12 tN y1.[Index((i - 1) % 5)] y2.[Index((i - 1) % 5)])
                              -- ((59. / 24.) .* f12 tN y1.[Index((i - 2) % 5)] y2.[Index((i - 2) % 5)])
                              ++ ((37. / 24.) .* f12 tN y1.[Index((i - 3) % 5)] y2.[Index((i - 3) % 5)])
                              -- ((3. / 8.) .* f12 tN y1.[Index((i - 4) % 5)] y2.[Index((i - 4) % 5)])
                              )
                         )

                y1.[Index((i + 1) % 5)] <- newY1
                y2.[Index((i + 1) % 5)] <- newY2
                yield newY1, newY2
        }

    Seq.concat [ Array.toSeq rungeKutta 
                 adams ]
