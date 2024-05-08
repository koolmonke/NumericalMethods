import { laplaceOperator } from "./lab1";
import { Matrix } from "./matrix";

interface Grid {
  x: number[];
  y: number[];
  f: Matrix;
}

interface IterData {
  k: number;
  data: Matrix;
}

function initGrid(size: number): Grid {
  const x = linspace(0, 1, size + 1);
  const y = linspace(0, 1, size + 1);
  const [X, Y] = meshgrid(x, y, "ij");
  const f = X.sub(Y).multiply(2);
  return { x, y, f };
}

function meshgrid(
  x: number[],
  y: number[],
  indexing: "ij" | "xy" = "ij"
): [Matrix, Matrix] {
  const X: number[][] = [];
  const Y: number[][] = [];

  if (indexing === "ij") {
    for (let i = 0; i < y.length; i++) {
      const xRow: number[] = [];
      const yRow: number[] = [];
      for (let j = 0; j < x.length; j++) {
        xRow.push(x[j]);
        yRow.push(y[i]);
      }
      X.push(xRow);
      Y.push(yRow);
    }
  } else {
    for (let j = 0; j < x.length; j++) {
      const xRow: number[] = [];
      const yRow: number[] = [];
      for (let i = 0; i < y.length; i++) {
        xRow.push(x[j]);
        yRow.push(y[i]);
      }
      X.push(xRow);
      Y.push(yRow);
    }
  }

  return [new Matrix(X).transpose(), new Matrix(Y).transpose()];
}

function computeLaplacian(U: number[][], step: number): Matrix {
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
}

// function computeLaplacian(U: number[][], step: number): Matrix {
//   const rows = U.length + 2;
//   const cols = U[0].length + 2;
//   const U_padded: number[][] = Array(rows);
//   for (let i = 0; i < rows; i++) {
//     U_padded[i] = Array(cols).fill(0);
//   }
//   for (let i = 0; i < U.length; i++) {
//     for (let j = 0; j < U[i].length; j++) {
//       U_padded[i + 1][j + 1] = U[i][j];
//     }
//   }
//   const laplacian: number[][] = Array(U.length);
//   for (let i = 0; i < U.length; i++) {
//     laplacian[i] = Array(U[i].length);
//     for (let j = 0; j < U[i].length; j++) {
//       laplacian[i][j] =
//         (U_padded[i][j] +
//           U_padded[i + 2][j] +
//           U_padded[i][j + 2] +
//           U_padded[i][j] -
//           4 * U_padded[i + 1][j + 1]) /
//         step;
//     }
//   }
//   return new Matrix(laplacian);
// }

function solvePoisson(
  size: number,
  step: number,
  f: Matrix,
  maxIter: number = 1000,
  correctionInterval: number = 10,
  tolerance: number = 1e-4
): { U: Matrix; iterData: IterData[] } {
  let U = Matrix.zeros(size + 1, size + 1);

  let previousU: Matrix | null = null;
  const iterData: IterData[] = [];
  let saveIter = false;

  for (let k = 0; k < maxIter; k++) {
    const laplacian = computeLaplacian(U.data, step);
    const residual = laplacian.sub(f);
    const AResidual = computeLaplacian(residual.data, step);

    const numerator = residual.pow(2).sum();
    const denominator = residual.multiply(AResidual).sum() + 1e-6;
    const T_k = numerator / denominator;
    let UNew = U.sub(residual.multiply(T_k));

    if (previousU !== null && UNew.isClose(previousU, tolerance) && saveIter) {
      iterData.push({ k, data: previousU });
      iterData.push({ k: k + 1, data: UNew });
      break;
    }

    if (k % correctionInterval === 0) {
      const alpha = UNew.mean();
      UNew = UNew.sub(alpha);
    }

    if (k > 50) {
      saveIter = true;
    }

    previousU = new Matrix(structuredClone(UNew.data));

    U = new Matrix(structuredClone(UNew.data));

    if (UNew.sub(U).norm() < tolerance) {
      if (iterData.length === 0) {
        iterData.push({ k, data: previousU });
        iterData.push({ k: k + 1, data: UNew });
      }
      break;
    }
  }

  return { U, iterData };
}

function linspace(start: number, stop: number, num: number, endpoint = true) {
  const div = endpoint ? num - 1 : num;
  const step = (stop - start) / div;
  return Array.from({ length: num }, (_, i) => start + step * i);
}

function printMatrixWithIndices(matrix: Matrix, label: string, size: number) {
  const matrixData = matrix.transpose().data;
  const x = linspace(0, 1, size + 1);
  const y = linspace(0, 1, size + 1);
  console.log(label);
  const footer = `          ${x.map((x) => x.toFixed(4)).join("  ")}`;
  for (let i = matrixData.length - 1; i >= 0; i--) {
    const row = matrixData[i];
    const rowOutput = row
      .map((val) =>
        Math.sign(val) !== -1 ? " " + val.toFixed(4) : val.toFixed(4)
      )
      .join(" ");
    console.log(`${y[i].toFixed(4)} | ${rowOutput}`);
  }
  console.log(footer);
}

export const lab2 = () => {
  const size = 10;
  const step = 0.1;
  const { f } = initGrid(size);
  const { U, iterData } = solvePoisson(size, step, f);

  for (const iter of iterData) {
    printMatrixWithIndices(U, `Итерация ${iter.k}`, size);
  }

  printMatrixWithIndices(U, "Точное решение", size);
};
