export class Matrix {
  constructor(public readonly data: number[][]) {}

  public mul(scalar: number | Matrix) {
    if (typeof scalar === "number") {
      return new Matrix(this.data.map((row) => row.map((val) => val * scalar)));
    }

    return new Matrix(
      this.data.map((row, i) => row.map((val, j) => val * scalar.data[i][j]))
    );
  }

  public matmul(scalar: Matrix) {
    const result: number[][] = structuredClone(this.data);
    for (let i = 0; i < this.data.length; i++) {
      for (let j = 0; j < scalar.data[0].length; j++) {
        let sum = 0;
        for (let k = 0; k < this.data[0].length; k++) {
          sum += this.data[i][k] * scalar.data[k][j];
        }
        result[i][j] = sum;
      }
    }

    return new Matrix(result);
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

    return new Matrix(
      this.data.map((row, i) => row.map((val, j) => val - b.data[i][j]))
    );
  }

  public static fill(i: number, j: number, value = 0) {
    return new Matrix(
      Array.from({ length: i }, () => Array.from({ length: j }, () => value))
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
    return this.data.flat().reduce((prev, current) => prev + current, 0);
  }

  public mean() {
    return (
      this.sum() / this.data.map((row) => row.length).reduce((a, b) => a + b, 0)
    );
  }

  public isClose(b: Matrix, tolerance: number = 1e-6) {
    return this.data.every((row, i) =>
      row.every((val, j) => Math.abs(val - b.data[i][j]) < tolerance)
    );
  }

  public norm() {
    return this.data
      .flat()
      .map(Math.abs)
      .reduce((a, b) => a + b, 0);
  }

  public transpose() {
    const result: number[][] = structuredClone(this.data);

    for (let i = 0; i < this.data.length; i++) {
      for (let j = 0; j < this.data[i].length; j++) {
        result[i][j] = this.data[j][i];
      }
    }

    return new Matrix(result);
  }

  public copy() {
    return new Matrix(structuredClone(this.data));
  }

  public map(f: (arg: number, i: number, j: number) => number) {
    return new Matrix(
      this.data.map((row, i) => row.map((val, j) => f(val, i, j)))
    );
  }
}
