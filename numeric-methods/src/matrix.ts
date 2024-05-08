function subtractMatrices(a: number[][], b: number[][]): number[][] {
  return a.map((row, i) => row.map((val, j) => val - b[i][j]));
}

function multiplyMatrix(a: number[][], scalar: number): number[][] {
  return a.map((row) => row.map((val) => val * scalar));
}

function isClose(a: number[][], b: number[][], tolerance: number): boolean {
  return a.every((row, i) =>
    row.every((val, j) => Math.abs(val - b[i][j]) < tolerance)
  );
}

export class Matrix {
  constructor(public readonly data: number[][]) {}

  public multiply(scalar: number | Matrix) {
    if (typeof scalar === "number") {
      return new Matrix(multiplyMatrix(this.data, scalar));
    } else {
      let result: number[][] = [];
      const m1 = this.data;
      const m2 = scalar.data;
      for (let i = 0; i < m1.length; i++) {
        result[i] = [];
        for (let j = 0; j < m2[0].length; j++) {
          let sum = 0;
          for (let k = 0; k < m1[0].length; k++) {
            sum += m1[i][k] * m2[k][j];
          }
          result[i][j] = sum;
        }
      }
      return new Matrix(result);
    }
  }

  public sub(b: Matrix | number) {
    if (typeof b === "number") {
      const clone = structuredClone(this.data);
      for (let i = 0; i < clone.length; i++) {
        for (let j = 0; j < clone[i].length; j++) {
          clone[i][j] -= b;
        }
      }

      return new Matrix(clone);
    }
    return new Matrix(subtractMatrices(this.data, b.data));
  }

  public static zeros(i: number, j: number) {
    return new Matrix(
      Array.from({ length: i }, () => Array.from({ length: j }, () => 0))
    );
  }

  public pow(scalar: number) {
    const copy = structuredClone(this.data);

    for (let i = 0; i < copy.length; i++) {
      for (let j = 0; j < copy[i].length; j++) {
        copy[i][j] = copy[i][j] ** scalar;
      }
    }

    return new Matrix(copy);
  }

  public sum() {
    return this.data.reduce(
      (prev, current) =>
        prev + current.reduce((prev, current) => prev + current, 0),
      0
    );
  }

  public mean() {
    const sum = this.sum();
    return sum / (this.data.length * this.data[0].length);
  }

  public isClose(b: Matrix, tolerance: number = 1e-6) {
    return isClose(this.data, b.data, tolerance);
  }

  public norm() {
    return this.transpose()
      .data.map((v) => v.map(Math.abs).reduce((a, b) => a + b, 0))
      .reduce((a, b) => a + b, 0);
  }
  public transpose() {
    const result: number[][] = Array.from(this.data, () => []);

    for (let i = 0; i < this.data.length; i++) {
      for (let j = 0; j < this.data[i].length; j++) {
        result[i][j] = this.data[j][i];
      }
    }

    return new Matrix(result);
  }
}
