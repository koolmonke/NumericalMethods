from typing import Callable

import numpy as np


def k(x: float) -> float:
    return 1 + x**2


def q(_x: float):
    return 1


def f(x: float, a: float, b: float) -> float:
    return (
        np.pi
        * (
            np.pi * (x**2 + 1) * np.sin((np.pi * (a - x)) / (a - b))
            + 2 * x * (a - b) * np.cos((np.pi * (a - x)) / (a - b))
        )
    ) / ((b - a) ** 2) + np.sin(np.pi * (x - a) / (b - a))


def phi_0(x: float, *, x_nodes: list[float]) -> float:
    if x_nodes[0] <= x < x_nodes[1]:
        return (x_nodes[1] - x) / (x_nodes[1] - x_nodes[0])
    return 0


def phi_n(x: float, *, x_nodes: list[float], n: int) -> float:
    if x_nodes[n - 2] <= x <= x_nodes[n - 1]:
        return (x - x_nodes[n - 2]) / (x_nodes[n - 1] - x_nodes[n - 2])
    return 0


def phi_q(x: float, q_idx: int, *, x_nodes: list[float]) -> float:
    if x_nodes[q_idx - 1] <= x <= x_nodes[q_idx]:
        return (x - x_nodes[q_idx - 1]) / (x_nodes[q_idx] - x_nodes[q_idx - 1])
    if x_nodes[q_idx] <= x <= x_nodes[q_idx + 1]:
        return (x_nodes[q_idx + 1] - x) / (x_nodes[q_idx + 1] - x_nodes[q_idx])
    return 0


def a_q1(x: float, *, i: int, x_nodes: list[float]) -> float:
    return k(x) / ((x_nodes[i] - x_nodes[i - 1]) ** 2) + q(x) * (
        ((x - x_nodes[i - 1]) / (x_nodes[i] - x_nodes[i - 1])) ** 2
    )


def a_q2(x: float, *, i: int, x_nodes: list[float]) -> float:
    return k(x) / ((x_nodes[i + 1] - x_nodes[i]) ** 2) + q(x) * (
        ((x_nodes[i + 1] - x) / (x_nodes[i + 1] - x_nodes[i])) ** 2
    )


def a_qj(x: float, *, i: int, x_nodes: list[float]) -> float:
    return (
        -k(x) / ((x_nodes[i + 1] - x_nodes[i]) ** 2)
        + q(x)
        * (x_nodes[i + 1] - x)
        * (x - x_nodes[i])
        / (x_nodes[i + 1] - x_nodes[i]) ** 2
    )


def b_q1(x: float, *, i: int, x_nodes: list[float], a: float, b: float) -> float:
    return f(x, a, b) * (x - x_nodes[i - 1]) / (x_nodes[i] - x_nodes[i - 1])


def b_q2(x: float, *, i: int, x_nodes: list[float], a: float, b: float) -> float:
    return f(x, a, b) * (x_nodes[i + 1] - x) / (x_nodes[i + 1] - x_nodes[i])


def beta_q1(x: float, x_nodes: list[float], mu1: float) -> float:
    return mu1 * (
        q(x) * (x_nodes[1] - x) * (x - x_nodes[0]) / (x_nodes[1] - x_nodes[0]) ** 2
        - k(x) / (x_nodes[1] - x_nodes[0]) ** 2
    )


def beta_q_n(x: float, mu2: float, x_nodes: list[float], n: int) -> float:
    return mu2 * (
        q(x)
        * (x_nodes[n - 1] - x)
        * (x - x_nodes[n - 2])
        / (x_nodes[n - 1] - x_nodes[n - 2]) ** 2
        - k(x) / (x_nodes[n - 1] - x_nodes[n - 2]) ** 2
    )


def gauss_solver(a_matrix: np.ndarray, b_vector: np.ndarray) -> np.ndarray:
    n = len(b_vector)

    for i in range(n):
        p = a_matrix[i][i]

        for ii in range(i + 1, n):
            multiplicative = a_matrix[ii][i] / p
            b_vector[ii] -= multiplicative * b_vector[i]

            for iii in range(i, n):
                a_matrix[ii][iii] -= multiplicative * a_matrix[i][iii]

    x = np.zeros(n)
    for i in range(n - 1, -1, -1):
        x[i] = b_vector[i] / a_matrix[i][i]

        for ii in range(i - 1, -1, -1):
            b_vector[ii] -= a_matrix[ii][i] * x[i]

    return x


def integrate(
    left: float,
    right: float,
    func: Callable,
    nodes: int = 8,
    eps: float = 1e-6,
    **kwargs,
) -> float:
    chebyshev_roots = np.cos([np.pi * (2 * z + 1) / (2 * nodes) for z in range(nodes)])

    def inter(_left, _right, _func, **_kwargs):
        rad = (_right - _left) / 2
        mid = (_right + _left) / 2
        s = sum(
            (np.sqrt(1 - r**2) * _func(rad * r + mid, **_kwargs))
            for r in chebyshev_roots
        )
        return rad * np.pi * s / len(chebyshev_roots)

    def integrate_inner(_left, _right, _func, **_kwargs):
        mid = (_right + _left) / 2
        whole = inter(_left, _right, _func, **_kwargs)
        left_new = inter(_left, mid, _func, **_kwargs)
        right_new = inter(mid, _right, _func, **_kwargs)
        if np.abs(whole - (left_new + right_new)) < eps:
            return whole
        else:
            return integrate_inner(_left, mid, _func, **_kwargs) + integrate_inner(
                mid, _right, _func, **_kwargs
            )

    return integrate_inner(left, right, func, **kwargs)


def calc_a_matrix(x_nodes: list[float], n: int) -> np.ndarray:
    a_mat = np.zeros([n - 1, n - 1])
    for i in range(1, n):
        a_mat[i - 1][i - 1] = integrate(
            x_nodes[i - 1], x_nodes[i], a_q1, i=i, x_nodes=x_nodes
        ) + integrate(x_nodes[i], x_nodes[i + 1], a_q2, i=i, x_nodes=x_nodes)

    for i in range(1, n - 1):
        a_mat[i - 1][i] = integrate(
            x_nodes[i], x_nodes[i + 1], a_qj, i=i, x_nodes=x_nodes
        )
        a_mat[i][i - 1] = a_mat[i - 1][i]
    return a_mat


def calc_b_vector(
    x_nodes: list[float], n: int, mu1: float, mu2: float, a: float, b: float
) -> np.ndarray:
    b_vec = np.zeros(n - 1)
    b_vec[0] = (
        integrate(x_nodes[0], x_nodes[1], b_q1, i=1, x_nodes=x_nodes, a=a, b=b)
        + integrate(x_nodes[1], x_nodes[2], b_q2, i=1, x_nodes=x_nodes, a=a, b=b)
        - integrate(x_nodes[0], x_nodes[1], beta_q1, x_nodes=x_nodes, mu1=mu1)
    )
    b_vec[n - 2] = (
        integrate(
            x_nodes[n - 2], x_nodes[n - 1], b_q1, i=n - 1, x_nodes=x_nodes, a=a, b=b
        )
        + integrate(
            x_nodes[n - 1], x_nodes[n], b_q2, i=n - 1, x_nodes=x_nodes, a=a, b=b
        )
        + integrate(x_nodes[n - 1], x_nodes[n], beta_q_n, x_nodes=x_nodes, n=n, mu2=mu2)
    )

    for i in range(2, n - 1):
        b_vec[i - 1] = integrate(
            x_nodes[i - 1], x_nodes[i], b_q1, i=i, x_nodes=x_nodes, a=a, b=b
        ) + integrate(
            x_nodes[i], x_nodes[i + 1], b_q2, i=i, x_nodes=x_nodes, a=a, b=b
        )
    return b_vec


def calc_y(
    x_nodes: list[float], n: int, mu1: float, mu2: float, c: np.ndarray
) -> np.ndarray:
    y = np.zeros(n + 1)

    for p in range(n):
        y[p] = mu1 * phi_0(x_nodes[p - 1], x_nodes=x_nodes) + mu2 * phi_n(
            x_nodes[p - 1], x_nodes=x_nodes, n=n
        )
        for i in range(1, n):
            y[p] = y[p] + c[i - 1] * phi_q(x_nodes[p], i, x_nodes=x_nodes)
    return y


if __name__ == "__main__":

    def main():
        a, b = 0.5, 1.0
        mu1 = mu2 = 0
        n = 8

        x_nodes = [a + i * (b - a) / n for i in range(n + 1)]

        a_matrix = calc_a_matrix(x_nodes, n)

        b_vector = calc_b_vector(x_nodes, n, mu1, mu2, a, b)

        c_vector = gauss_solver(a_matrix, b_vector)

        y_vector = calc_y(x_nodes, n, mu1, mu2, c_vector)

        u_t = [np.sin((np.pi * (x_nodes[i] - a)) / (b - a)) for i in range(n + 1)]
        r = [abs(u_t[i] - y_vector[i]) for i in range(n + 1)]

        cond_number = max(np.sum(abs(a_matrix), axis=1)) * max(
            np.sum(abs(np.linalg.inv(a_matrix)), axis=1)
        )

        print("Хакимов Артур, вариант 29, метод Ритца:\n")

        print(f"{'x':<8.4} {'u':<8.4} {'r':<8.9}")
        print(
            *(f"{x_nodes[i]:<8.4f} {u_t[i]:<8.4f} {r[i]:<8.9f}" for i in range(n + 1)),
            sep="\n",
        )

        print(f"Число обусловленности матрицы a = {cond_number:.6f}")

    main()
