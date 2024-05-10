/*
Решение задачи Неймана для уравнения Пуассона методом скорейшего спуска
Вариант 29
Хакимов Артур
*/

import { printTable } from "./printTable";
import { computeLaplacian, linspace, Matrix, meshGrid } from "./utils";

type IterData = {
  k: number;
  data: Matrix;
};

const initGrid = (size: number) => {
  const x = linspace(0, 1, size + 1);
  const y = linspace(0, 1, size + 1);
  const [X, Y] = meshGrid(x, y, "ij");
  return X.sub(Y).mul(2);
};

const solvePoisson = (
  size: number,
  step: number,
  f: Matrix,
  maxIter: number = 1000,
  correctionInterval: number = 10,
  tolerance: number = 1e-4
): { U: Matrix; iterData: IterData[] } => {
  let U = Matrix.fill(size + 1, size + 1);

  let previousU: Matrix | null = null;
  const iterData: IterData[] = [];

  for (let k = 0; k < maxIter; k++) {
    const laplacian = computeLaplacian(U, step);
    const residual = laplacian.sub(f);
    const AResidual = computeLaplacian(residual, step);

    let UNew = U.sub(
      residual.mul(residual.pow(2).sum() / residual.mul(AResidual).sum())
    );

    if (previousU !== null && previousU.isClose(UNew, tolerance) && k > 50) {
      iterData.push({ k, data: previousU });
      iterData.push({ k: k + 1, data: UNew });
      break;
    }

    if (k % correctionInterval === 0) {
      UNew = UNew.sub(UNew.mean());
    }

    previousU = UNew;
    U = UNew;

    if (UNew.sub(U).norm() < tolerance) {
      if (iterData.length === 0) {
        iterData.push({ k, data: previousU });
        iterData.push({ k: k + 1, data: UNew });
      }
      break;
    }
  }

  return { U, iterData };
};

export const lab2 = () => {
  const size = 10;
  const step = 0.1;
  const grid = initGrid(size);
  const { U, iterData } = solvePoisson(size, step, grid);

  for (const iter of iterData) {
    console.log(`Итерация ${iter.k + 1}`);
    printTable(U);
  }

  console.log("Точное решение");
  printTable(U);
};
