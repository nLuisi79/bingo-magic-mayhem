export type BingoCellState = 'free' | 'open' | 'daubed';

export interface BingoCell {
  column: 'B' | 'I' | 'N' | 'G' | 'O';
  row: number;
  value: number | null;
  state: BingoCellState;
}

export interface BingoCard {
  id: string;
  cells: BingoCell[];
}

export interface CalledNumber {
  column: 'B' | 'I' | 'N' | 'G' | 'O';
  value: number;
}

export interface BingoMatchState {
  card: BingoCard;
  calledNumbers: CalledNumber[];
  hasBingo: boolean;
}
