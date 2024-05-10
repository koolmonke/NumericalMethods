import { Matrix } from "./matrix";

export const computeLaplacian = ({ data: U }: Matrix, step: number): Matrix => {
  const rows = U.length + 2;
  const cols = U[0].length + 2;
  const U_padded: number[][] = Array(rows);
  for (let i = 0; i < rows; i++) {
    U_padded[i] = Array(cols).fill(0);
  }
  for (let i = 0; i < U.length; i++) {
    for (let j = 0; j < U[i].length; j++) {
      U_padded[i + 1][j + 1] = U[i][j];
    }
  }
  const laplacian: number[][] = Array(U.length);
  for (let i = 0; i < U.length; i++) {
    laplacian[i] = Array(U[i].length);
    for (let j = 0; j < U[i].length; j++) {
      laplacian[i][j] =
        (U_padded[i][j] +
          U_padded[i + 2][j] +
          U_padded[i][j + 2] +
          U_padded[i + 2][j + 2] -
          4 * U_padded[i + 1][j + 1]) /
        step ** 2;
    }
  }
  return new Matrix(laplacian);
};
