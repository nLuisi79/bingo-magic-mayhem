import { BingoCard } from '../core/GameTypes';

export class BingoWinChecker {
  public static hasBingo(card: BingoCard): boolean {
    return this.hasCompletedRow(card) || this.hasCompletedColumn(card) || this.hasCompletedDiagonal(card);
  }

  private static hasCompletedRow(card: BingoCard): boolean {
    for (let row = 0; row < 5; row++) {
      if (card.cells.filter((cell) => cell.row === row).every((cell) => cell.state !== 'open')) {
        return true;
      }
    }

    return false;
  }

  private static hasCompletedColumn(card: BingoCard): boolean {
    for (const column of ['B', 'I', 'N', 'G', 'O']) {
      if (card.cells.filter((cell) => cell.column === column).every((cell) => cell.state !== 'open')) {
        return true;
      }
    }

    return false;
  }

  private static hasCompletedDiagonal(card: BingoCard): boolean {
    const leftToRight = card.cells.filter((cell) => cell.row === ['B', 'I', 'N', 'G', 'O'].indexOf(cell.column));
    const rightToLeft = card.cells.filter((cell) => cell.row === 4 - ['B', 'I', 'N', 'G', 'O'].indexOf(cell.column));

    return leftToRight.every((cell) => cell.state !== 'open') || rightToLeft.every((cell) => cell.state !== 'open');
  }
}
