module Lab5.Integrate

open System
open MatrixArithmetic
open Microsoft.FSharp.Core


let rec Process degree =
    match degree with
    | 0 -> Polynom([ 1. ])
    | 1 -> Polynom([ 0.; 1. ])
    | n ->
        2. * Polynom([ 0.; 1. ]) * (Process(n - 1))
        - Process(n - 2)

let integrate (left, right, nodes: uint) func =
    let roots = Chebyshev.Roots nodes
    let len = (right - left) / 2.
    let mid = (right + left) / 2.

    let sum =
        seq { for r in roots -> sqrt (1. - r ** 2.) * func (len * r + mid) }
        |> Seq.sum

    len * Math.PI * sum / float nodes


let rec integral (l, r, nodes) func =
    let mid = (l + r) / 2.
    let whole = integrate (l, r, nodes) func
    let left = integrate (l, mid, nodes) func
    let right = integrate (mid, r, nodes) func

    if abs (whole - (left + right)) < 1e-5 then
        whole
    else
        integral (l, mid, nodes) func
        + integral (mid, r, nodes) func
