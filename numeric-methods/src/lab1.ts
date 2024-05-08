const NUM_POINTS = 10;
const STEP_SIZE = 1.0 / NUM_POINTS;
const TOLERANCE = 1e-4;

const xCoords = Array.from({ length: NUM_POINTS + 1 }, (_, i) => i * STEP_SIZE);
const yCoords = Array.from({ length: NUM_POINTS + 1 }, (_, i) => i * STEP_SIZE);

function functionF(x: number, y: number) {
  return 2 * (x + y - x ** 2 - y ** 2);
}

function exactSolution(x: number, y: number) {
  return (x - x ** 2) * (y - y ** 2);
}

const F = xCoords.map((xi) => yCoords.map((yj) => functionF(xi, yj)));
const UExact = xCoords.map((xi) => yCoords.map((yj) => exactSolution(xi, yj)));
const U = UExact.slice();

const initialTerm = STEP_SIZE ** 2 / 4;
const penaltyFactor = 1 - Math.tan((STEP_SIZE * Math.PI) / 2) ** 2;
const T = Array.from(
  { length: NUM_POINTS },
  (_, k) =>
    initialTerm /
    (1 + penaltyFactor * Math.cos((Math.PI * (2 * k - 1)) / (2 * NUM_POINTS)))
);

export function laplaceOperator(U: number[][], step = STEP_SIZE) {
  const UTemp = U.map((row) => row.slice());
  for (let i = 1; i < U.length - 1; i++) {
    for (let j = 1; j < U[i].length - 1; j++) {
      UTemp[i][j] =
        (-U[i + 1][j] - U[i - 1][j] - U[i][j + 1] - U[i][j - 1] + 4 * U[i][j]) /
        step ** 2;
    }
  }
  return UTemp;
}

const previousIterations: number[][][] = [];
for (let k = 0; k < NUM_POINTS; k++) {
  const UPrev = U.map((row) => row.slice());
  const delta = laplaceOperator(UPrev);
  for (let i = 1; i < U.length - 1; i++) {
    for (let j = 1; j < U[i].length - 1; j++) {
      U[i][j] += T[k] * (F[i][j] - delta[i][j]);
    }
  }
  if (Math.max(...U.flatMap((row) => row.map(Math.abs))) < TOLERANCE) {
    previousIterations.push(UPrev);
    break;
  }
  if (k === NUM_POINTS - 1) {
    previousIterations.push(UPrev);
  }
}

function printMatrixWithIndices(matrix: number[][], label: string) {
  console.log(label);
  const header = `         ${xCoords.map((x) => x.toFixed(4)).join(" ")}`;
  console.log(header);
  for (let i = matrix.length - 1; i >= 0; i--) {
    const row = matrix[i];
    const rowOutput = row.map((val) => val.toFixed(4)).join(" ");
    console.log(`${yCoords[i].toFixed(4)} | ${rowOutput}`);
  }
}

export const lab1 = () => {
  printMatrixWithIndices(
    previousIterations[previousIterations.length - 1],
    "Предпоследняя итерация:"
  );
  printMatrixWithIndices(U, "Последняя итерация:");
  printMatrixWithIndices(UExact, "Точное решение:");
};
