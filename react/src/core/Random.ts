export default class Random {
  public readonly seed: number;
  private lastValue: number;

  constructor(seed: number) {
    this.lastValue = seed;
  }

  /**
   * Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.
   * @returns
   */
  nextDouble() {
    this.lastValue = Random.hash(this.lastValue);
    return (this.lastValue & 0xfffffff) / 0x10000000;
  }

  /**
   * Returns a non-negative random integer that is less than the specified maximum.
   * @param maxValue
   * @returns
   */
  nextInt(maxValue: number) {
    return Math.floor(this.nextDouble() * maxValue);
  }

  /**
   * Robert Jenkins' 32 bit integer hash function.
   * https://healeycodes.com/creating-randomness
   * @param seed
   * @returns
   */
  static hash(seed: number) {
    seed = seed & 0xffffffff;
    seed = (seed + 0x7ed55d16 + (seed << 12)) & 0xffffffff;
    seed = (seed ^ 0xc761c23c ^ (seed >>> 19)) & 0xffffffff;
    seed = (seed + 0x165667b1 + (seed << 5)) & 0xffffffff;
    seed = ((seed + 0xd3a2646c) ^ (seed << 9)) & 0xffffffff;
    seed = (seed + 0xfd7046c5 + (seed << 3)) & 0xffffffff;
    seed = (seed ^ 0xb55a4f09 ^ (seed >>> 16)) & 0xffffffff;
    return seed;
  }

  static getHashCode(str: string) {
    return Array.from(str).reduce(
      (s, c) => (Math.imul(31, s) + c.charCodeAt(0)) | 0,
      0
    );
  }

  public static shuffle<T>(array: T[], random: Random) {
    let i = array.length;
    while (i > 1) {
      i--;
      const j = random.nextInt(i + 1);
      const value = array[j];
      array[j] = array[i];
      array[i] = value;
    }
    return array;
  }
}
