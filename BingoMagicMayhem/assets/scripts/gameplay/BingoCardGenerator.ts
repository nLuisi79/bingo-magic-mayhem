import { BingoCard, BingoCell } from '../core/GameTypes';

const BINGO_COLUMNS = [
  { label: 'B' as const, min: 1, max: 15 },
  { label: 'I' as const, min: 16, max: 30 },
  { label: 'N' as const, min: 31, max: 45 },
  { label: 'G' as const, min: 46, max: 60 },
  { label: 'O' as const, min: 61, max: 75 },
];

export class BingoCardGenerator {
  public static createCard(id = 'card-1'): BingoCard {
    const cells: BingoCell[] = [];

    for (const column of BINGO_COLUMNS) {
      const values = this.pickUniqueNumbers(column.min, column.max, 5);

      values.forEach((value, row) => {
        const isFreeSpace = column.label === 'N' && row === 2;

        cells.push({
          column: column.label,
          row,
          value: isFreeSpace ? null : value,
          state: isFreeSpace ? 'free' : 'open',
        });
      });
    }

    return { id, cells };
  }

  private static pickUniqueNumbers(min: number, max: number, count: number): number[] {
    const pool = Array.from({ length: max - min + 1 }, (_, index) => min + index);
    const picked: number[] = [];

    while (picked.length < count) {
      const index = Math.floor(Math.random() * pool.length);
      picked.push(pool.splice(index, 1)[0]);
    }

    return picked;
  }
}
