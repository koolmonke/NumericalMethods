﻿module C2Lab1.Chebyshev

open System


let Roots degree =
    seq {
        for item in 0u .. (degree - 1u) ->
            cos (
                Math.PI * (2. * float item + 1.)
                / (2. * float degree)
            )
    }
    |> Seq.cache
