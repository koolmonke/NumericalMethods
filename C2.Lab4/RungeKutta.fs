module C2.Lab4.RungeKutta

open C2.Lab4.Operators

let solve f12 n tau =
    let rec loop i (y1, y2) =
        seq {
            yield y1, y2

            if i < (n - 1) then
                let tN = tau * float i
                let k11, k12 = tau .* f12 tN y1 y2

                let k21, k22 =
                    tau
                    .* f12 (tN + tau / 2.) (y1 + k11 / 2.) (y2 + k12 / 2.)

                let k31, k32 =
                    tau
                    .* f12 (tN + tau / 2.) (y1 + k21 / 2.) (y2 + k22 / 2.)

                let k41, k42 =
                    tau .* f12 (tN + tau) (y1 + k31) (y2 + k32)

                let newY1 =
                    y1 + (k11 + 2. * k21 + 2. * k31 + k41) / 6.

                let newY2 =
                    y2 + (k12 + 2. * k22 + 2. * k32 + k42) / 6.

                yield! loop (i + 1) (newY1, newY2)
        }

    loop 0
