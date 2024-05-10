import { Matrix } from "./matrix";

export const meshGrid = (
  x: number[],
  y: number[],
  indexing: "ij" | "xy" = "ij"
): [Matrix, Matrix] => {
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
};
