import { Matrix } from "./matrix";

export function computeLaplacian({ data: U }: Matrix, h: number) {
  const U_padded = [];
  for (let i = 0; i < U.length + 2; i++) {
    U_padded[i] = Array(U[0].length + 2).fill(0);
  }

  for (let i = 0; i < U.length; i++) {
    for (let j = 0; j < U[0].length; j++) {
      U_padded[i + 1][j + 1] = U[i][j];
    }
  }

  const laplacian = [];
  for (let i = 1; i < U.length + 1; i++) {
    const row = [];
    for (let j = 1; j < U[0].length + 1; j++) {
      const value =
        (U_padded[i - 1][j] +
          U_padded[i + 1][j] +
          U_padded[i][j - 1] +
          U_padded[i][j + 1] -
          4 * U_padded[i][j]) /
        h ** 2;
      row.push(value);
    }
    laplacian.push(row);
  }

  return new Matrix(laplacian);
}
