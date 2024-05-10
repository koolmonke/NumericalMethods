/*
Решение задачи Дирихле для линейного эллиптического уравнения с переменными коэффициентами
Вариант 29
Хакимов Артур
*/

import { Matrix } from "./utils";

const k1 = (x1: number, _x2: number) => 1 + x1 / 2;

const k2 = (_x1: number, x2: number) => 1 + x2 / 2;

const q = (x1: number, _x2: number) => 1 + x1;

const f = (x1: number, x2: number) =>
  8 * x1 * (x1 - x2 ** 2) +
  8 * x2 * (x1 - x1 ** 2) -
  (1 + x1) * (x2 - x2 ** 2) * (x1 - x1 ** 2);

const initCondition = (n: number) => Matrix.fill(n + 1, n + 1);

const boundary = ({ data }: Matrix) => {
  const y = structuredClone(data);
  y[0] = y[y.length - 1];
  for (let i = 0; i < y[0].length; i++) {
    y[0][i] = 0;
    y[y.length - 1][i] = 0;
  }
  y[y.length - 1][y[0].length - 1] = 0;
  return new Matrix(y);
};

const getParameters = (n: number) => {
  const h = 1 / n;
  const tau =
    h ** 2 /
    (2 *
      Math.max(
        ...Array.from(
          (function* (N: number) {
            for (let i = 0; i < N + 1; i++) {
              for (let j = 0; j < N + 1; j++) {
                yield k1(i * h, j * h);
              }
            }
          })(n)
        )
      ));
  return { tau, h };
};

function* solvePde(n: number) {
  const { h, tau } = getParameters(n);

  let y = boundary(initCondition(n));

  for (let i = 0; i < n + 1; i++) {
    const yOld = y.copy();

    for (let m = 1; m < n; m++) {
      for (let k = 1; k < n; k++) {
        const a1 = k1(k * h - h / 2, m * h);
        const a2 = k1((k + 1) * h - h / 2, m * h);
        const Am = (a1 * tau) / (2 * h ** 2);
        const Cm = 1 + ((a1 + a2) * tau) / (2 * h ** 2);
        const Bm = (a2 * tau) / (2 * h ** 2);
        const Fm =
          (-tau / 2) * (q(k * h, m * h) * y.data[k][m] - f(k * h, m * h));
        const b = Bm / (Cm - Am);
        const v = (Fm + Am * y.data[k - 1][m]) / (Cm - Am);
        y.data[k][m] = b * y.data[k + 1][m] + v;
      }
    }
    y = boundary(y);

    for (let m = 1; m < n; m++) {
      for (let k = 1; k < n; k++) {
        const a1 = k2(k * h, m * h - h / 2);
        const a2 = k2(k * h, (m + 1) * h - h / 2);
        const Am = (a1 * tau) / (2 * h ** 2);
        const Cm = 1 + ((a1 + a2) * tau) / (2 * h ** 2);
        const Bm = (a2 * tau) / (2 * h ** 2);
        const Fm =
          (-tau / 2) * (q(k * h, m * h) * y.data[k][m] - f(k * h, m * h));
        const b = Bm / (Cm - Am);
        const v = (Fm + Am * y.data[k][m - 1]) / (Cm - Am);
        y.data[k][m] = b * y.data[k][m + 1] + v;
      }
    }
    y = boundary(y);
    if (y.sub(yOld).norm() < 1e-4) {
      break;
    }
    yield y.copy();
  }
  return y;
}

function printTable({ data }: Matrix) {
  const n = data.length - 1;
  const colWidths = [...Array(n + 1).keys()].map((i) => {
    return Math.max(
      (i / n).toFixed(5).length,
      Math.max(...data[i].map((j) => j.toFixed(5).length))
    );
  });
  const headerWidth = Math.max("x1\\x2".length, 5);

  const header = `│ ${"x1\\x2".padStart(headerWidth)} │ ${data[0]
    .map((_, i) => (i / n).toFixed(5).padStart(colWidths[i], " "))
    .join(" │ ")} │`;
  const totalWidth = header.length - 2;

  console.log(`┌${"─".repeat(totalWidth)}┐`);
  console.log(header);
  console.log(`├${"─".repeat(totalWidth)}┤`);

  for (let i = 0; i < data.length; i++) {
    const row = `│ ${(i / n).toFixed(2).padStart(headerWidth, " ")} │ ${data[i]
      .map((j, k) => j.toFixed(5).padStart(colWidths[k], " "))
      .join(" │ ")} │`;
    console.log(row);
    if (i < n) {
      console.log(`├${"─".repeat(totalWidth)}┤`);
    } else {
      console.log(`└${"─".repeat(totalWidth)}┘`);
    }
  }
}

export const lab4 = () => {
  const N = 10;
  const iters = Array.from(solvePde(N));

  console.log("Предпоследняя итерация");
  printTable(iters[iters.length - 3]);

  console.log("Последняя итерация");
  printTable(iters[iters.length - 2]);

  console.log("Результат решения PDE");
  printTable(iters[iters.length - 1]);
};
