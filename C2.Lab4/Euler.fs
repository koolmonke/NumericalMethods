module C2.Lab4.Euler

let solve f1 f2 n tau =
    let rec loop i (y1, y2) =
        seq {
            yield y1, y2

            if i < (n - 1) then
                let tN = tau * float i
                let nextY1 = y1 + tau * f1 tN y1 y2
                let nextY2 = y2 + tau * f2 tN y1 y2
                yield! loop (i + 1) (nextY1, nextY2)
        }

    loop 0
