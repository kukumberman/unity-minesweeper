export default class Grid2D {
  public readonly Width: number;
  public readonly Height: number;

  public constructor(width: number, height: number) {
    this.Width = width;
    this.Height = height;
  }

  public GetNeighbours(x: number, y: number): number[] {
    const list = [];
    this.GetNeighboursNonAlloc(x, y, list);
    return list;
  }

  public GetNeighboursNonAlloc(x: number, y: number, indexes: number[]) {
    for (let i = -1; i <= 1; i++) {
      for (let j = -1; j <= 1; j++) {
        if (i === 0 && j === 0) {
          continue;
        }
        var xx = x + i;
        var yy = y + j;
        if (this.IsInside(xx, yy)) {
          const index = this.ConvertTo1D(xx, yy);
          indexes.push(index);
        }
      }
    }
  }

  public IsInside(x: number, y: number) {
    const xx = x >= 0 && x < this.Width;
    const yy = y >= 0 && y < this.Height;
    return xx && yy;
  }

  public ConvertTo1D(x: number, y: number) {
    return y * this.Width + x;
  }

  public ConvertTo2D(index: number) {
    const x = index % this.Width;
    const y = Math.floor(index / this.Width);
    return {
      x,
      y,
      inside: this.IsInside(x, y),
    };
  }
}
