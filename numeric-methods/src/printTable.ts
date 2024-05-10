import { Matrix } from "./utils";

export function printTable({ data }: Matrix) {
  const n = data.length - 1;

  const header = `        | ${data[0]
    .map((_, i) => ` ${(i / n).toFixed(5)}`)
    .join(" | ")}`;
  const totalWidth = header.length;

  console.log(`${"─".repeat(totalWidth)}`);
  console.log(header);
  console.log(`${"─".repeat(totalWidth)}`);

  for (let i = data.length - 1; i >= 0; i--) {
    const row = `  ${(i / n).toFixed(2)}  | ${data[i]
      .map((j) => (Math.sign(j) !== -1 ? ` ${j.toFixed(5)}` : j.toFixed(5)))
      .join(" | ")}`;
    console.log(row);
    if (i >= n) {
      console.log(`${"─".repeat(totalWidth)}`);
    } else {
      console.log(`${"─".repeat(totalWidth)}`);
    }
  }
}
