module C2.Lab4.Euler
open Operators
let solve f12 n tau =
    let rec loop i (y1, y2) =
        seq {
            yield y1, y2

            if i < (n - 1) then
                let tN = tau * float i
                yield! loop (i + 1) ((y1, y2) ++ tau .* f12 tN y1 y2)
        }

    loop 0
