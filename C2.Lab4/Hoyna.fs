module C2.Lab4.Hoyna

let solve f1 f2 n tau =
    let rec loop i (y1, y2) =
        seq {
            yield y1, y2

            if i < (n - 1) then
                let tN = tau * float i
                let tNNext = tau + tN
                let p1 = y1 + tau * f1 tN y1 y2
                let p2 = y2 + tau * f2 tN y1 y2

                let newY1 =
                    y1 + (tau / 2.) * (f1 tN y1 y2 + f1 tNNext p1 p2)

                let newY2 =
                    y2
                    + (tau / 2.) * (f2 tN y1 y2 + f2 tNNext newY1 p2)

                yield! loop (i + 1) (newY1, newY2)
        }

    loop 0
