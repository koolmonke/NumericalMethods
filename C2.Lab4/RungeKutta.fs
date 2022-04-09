module C2.Lab4.RungeKutta

let solve f12 n tau =
    let rec loop i (y1, y2) =
        seq {
            yield y1, y2

            if i < (n - 1) then
                let tN = tau * float i
                let k11, k12 = f12 tN y1 y2

                let k21, k22 =
                    f12 (tN + tau / 2.) (y1 + tau / 2. * k11) (y2 + tau / 2. * k12)

                let k31, k32 =
                    f12 (tN + tau) (y1 - tau * k11 + 2. * tau * k21) (y2 - tau * k12 + 2. * tau * k22)

                let newY1 = y1 + tau * (k11 + 4. * k21 + k31) / 6.
                let newY2 = y2 + tau * (k12 + 4. * k22 + k32) / 6.
                yield! loop (i + 1) (newY1, newY2)
        }

    loop 0
