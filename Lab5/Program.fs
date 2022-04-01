open System.Collections.Generic
open System
open Lab1
open Lab5.Integrate

open Functions.Expressions
open MatrixArithmetic

let variant = 19

let x = Var "x"

let n = if variant % 2 = 0 then 4u else 3u

let xLeft = 0.

let xRight = 1.5 - float variant / 25.

let muLeft = float variant / 2. - 1.
let muRight = 0.

let xLenght = xRight - xLeft

let k = x ** Val 0.5 + Val(float variant / 3.)

let p =
    (x + Val 4)
    / (x ** Val 2 + Val(float variant / 3.))

let f = x * Val(2. / (float variant + 1.))

let memoize func =
    let cache = Dictionary()

    fun v ->
        match cache.TryGetValue(v) with
        | true, value -> value
        | false, _ ->
            let tmp = func v
            cache.Add(v, tmp)
            tmp

let phi =
    let phi1 _k =
        if _k = 0. then
            let xCoeff = muLeft - muRight
            let freeCoeff = xLeft * muRight - xRight * muLeft
            ((Val xCoeff) * x + Val freeCoeff) / Val xLenght
        else
            (Val xRight - x) * (x - Val xLeft) ** Val _k

    memoize phi1



let lagrange i j =
    let inIntegral =
        k * diff (phi i) "x" * diff (phi j) "x"
        + p * phi i * phi j

    integral (xLeft, xRight, n) (evalf inIntegral "x")

module NoSymbols =
    let f x = x * (2. / (float variant + 1.))

    let phi0 x =
        muLeft
        + (muRight - muLeft)
          * sin (Math.PI * (x - xLeft) / 2. / (xRight - xLeft))

    let d_phi0 x =
        (muRight - muLeft)
        * cos (Math.PI * (x - xLeft) / 2. / (xRight - xLeft))
        * (Math.PI / 2. / (xRight - xLeft))

    let phi k x =
        sin (k * Math.PI * (x - xLeft) / (xRight - xLeft))

    let d_phi k x =
        cos (k * Math.PI * (x - xLeft) / (xRight - xLeft))
        * (k * Math.PI / (xRight - xLeft))

    let fn_k x = x ** 0.5 + (float variant / 3.)

    let fn_q x =
        (x + 5.) / (x ** 2. + 0.9 * float variant)

    let inter_b i x =
        f x * phi i x
        - fn_k (x) * d_phi0 (x) * d_phi i x
        - fn_q x * phi0 (x) * phi i x


let b q =
    integral (xLeft, xRight, n) (NoSymbols.inter_b q)


let a =
    Matrix(int n, int n)
        .From(
            seq {
                for i in 1 .. int n do
                    for j in 1 .. int n do
                        yield lagrange j i
            }
        )

printfn $"{a.[0, 0]}"

let bVector =
    Vector(seq { 1.0 .. (float n) } |> Seq.map b)


let gaussSolution = GaussSolver(a, bVector).SolutionVector


printfn $"{gaussSolution}"

let solution =
    (seq {
        for i in 1 .. (int n) ->
            phi (float i) * Val gaussSolution.[int (i - 1)]
            + phi 0
     })
    |> Seq.toList

let sol =
    solution.[0] + solution.[1] + solution.[2]

printfn "%A" (solution |> Seq.map show)

for i in xLeft .. xLenght / 6.0 .. xRight do
    printfn "%A %A" i (evalf sol "x" i)
