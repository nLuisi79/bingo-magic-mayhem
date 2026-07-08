import { _decorator, Color, Component, Graphics, Input, Label, Node, UITransform, Vec3 } from 'cc';
import { BingoCardGenerator } from './BingoCardGenerator';
import { BingoWinChecker } from './BingoWinChecker';
import { RoomLobbyController } from './RoomLobbyController';
import { BingoCard, BingoCell, CalledNumber } from '../core/GameTypes';

const { ccclass, property } = _decorator;

@ccclass('BingoPrototypeController')
export class BingoPrototypeController extends Component {
  @property(Label)
  public statusLabel: Label | null = null;

  @property(Node)
  public cardRoot: Node | null = null;

  private card: BingoCard | null = null;
  private calledNumbers: CalledNumber[] = [];
  private cellLabels = new Map<string, Label>();
  private cellBackgrounds = new Map<string, Graphics>();
  private selectedCardCount = 1;
  private manaBetPerCard = 25;

  public start(): void {
    this.hideLegacySceneControls();
    const lobby = this.getComponent(RoomLobbyController) ?? this.addComponent(RoomLobbyController);
    lobby.start();
  }

  public beginMatchFromLobby(cardCount: number, manaBetPerCard: number): void {
    this.selectedCardCount = cardCount;
    this.manaBetPerCard = manaBetPerCard;
    this.node.removeAllChildren();
    this.setupGameplayHud();
    this.startNewMatch();
  }

  public startNewMatch(): void {
    this.card = BingoCardGenerator.createCard();
    this.calledNumbers = [];
    this.drawCard();
    this.updateStatus(`Pollenspire Fountain started: ${this.selectedCardCount} cards, ${this.manaBetPerCard} Mana per card.`);
  }

  public callNextNumber(): void {
    if (!this.card) {
      this.startNewMatch();
    }

    if (this.calledNumbers.length >= 75) {
      this.updateStatus('All numbers have been called. Start a new match to play again.');
      return;
    }

    const call = this.createUniqueCall();
    this.calledNumbers.push(call);

    const prefix = `${call.column}-${call.value}`;
    this.updateStatus(`${prefix} called. Tap the matching square if it is on your card.`);
  }

  private createUniqueCall(): CalledNumber {
    const columns = [
      { column: 'B' as const, min: 1, max: 15 },
      { column: 'I' as const, min: 16, max: 30 },
      { column: 'N' as const, min: 31, max: 45 },
      { column: 'G' as const, min: 46, max: 60 },
      { column: 'O' as const, min: 61, max: 75 },
    ];

    let call: CalledNumber;

    do {
      const selected = columns[Math.floor(Math.random() * columns.length)];
      call = {
        column: selected.column,
        value: selected.min + Math.floor(Math.random() * (selected.max - selected.min + 1)),
      };
    } while (this.calledNumbers.some((existing) => existing.value === call.value));

    return call;
  }

  private daubCell(cell: BingoCell): void {
    if (!this.card) {
      return;
    }

    if (cell.value === null) {
      this.updateStatus('The FREE space is already yours.');
      return;
    }

    const wasCalled = this.calledNumbers.some((call) => call.value === cell.value);

    if (!wasCalled) {
      this.updateStatus(`${cell.column}-${cell.value} has not been called yet.`);
      return;
    }

    cell.state = 'daubed';
    this.refreshCardView();

    if (BingoWinChecker.hasBingo(this.card)) {
      this.updateStatus('Bingo! Whispering Fen rewards are ready.');
      return;
    }

    this.updateStatus(`${cell.column}-${cell.value} daubed. Keep going.`);
  }

  private drawCard(): void {
    const root = this.getCardRoot();
    root.removeAllChildren();
    this.cellLabels.clear();
    this.cellBackgrounds.clear();

    const columns = ['B', 'I', 'N', 'G', 'O'];
    const cellSize = 68;
    const gap = 5;
    const gridWidth = columns.length * cellSize + (columns.length - 1) * gap;
    const boardCenterX = -270;
    const startX = boardCenterX - gridWidth / 2 + cellSize / 2;
    const startY = 115;

    columns.forEach((column, index) => {
      this.createGridLabel(root, column, startX + index * (cellSize + gap), startY, cellSize, true);
    });

    if (!this.card) {
      return;
    }

    this.card.cells.forEach((cell) => {
      const columnIndex = columns.indexOf(cell.column);
      const x = startX + columnIndex * (cellSize + gap);
      const y = startY - (cell.row + 1) * (cellSize + gap);
      const label = this.createGridLabel(root, this.getCellText(cell), x, y, cellSize, false);
      const graphics = label.node.parent?.getComponent(Graphics);
      const cellNode = label.node.parent;

      this.cellLabels.set(this.getCellKey(cell), label);

      if (graphics) {
        this.cellBackgrounds.set(this.getCellKey(cell), graphics);
      }

      cellNode?.on(Input.EventType.TOUCH_END, () => this.daubCell(cell), this);
    });

    this.refreshCardView();
  }

  private setupGameplayHud(): void {
    const root = new Node('GameplayRoot');
    root.setParent(this.node);
    root.setPosition(Vec3.ZERO);
    root.addComponent(UITransform).setContentSize(1280, 720);

    const statusNode = new Node('GameplayStatusLabel');
    statusNode.setParent(root);
    statusNode.setPosition(new Vec3(260, 230, 0));
    statusNode.addComponent(UITransform).setContentSize(520, 48);
    const gameplayStatus = statusNode.addComponent(Label);
    gameplayStatus.fontSize = 22;
    gameplayStatus.lineHeight = 30;
    gameplayStatus.color = Color.WHITE;
    gameplayStatus.horizontalAlign = Label.HorizontalAlign.CENTER;
    gameplayStatus.verticalAlign = Label.VerticalAlign.CENTER;
    this.statusLabel = gameplayStatus;

    const callButton = this.createGameplayButton(root, 'CallNumberButton', 'Call Number', 260, -230, 210, 64);
    callButton.on(Input.EventType.TOUCH_END, () => this.callNextNumber(), this);

    const lobbyButton = this.createGameplayButton(root, 'BackToLobbyButton', '< Lobby', 530, 300, 140, 48);
    lobbyButton.on(Input.EventType.TOUCH_END, () => this.showLobby(), this);

    const cardRoot = new Node('GameplayCardRoot');
    cardRoot.setParent(root);
    cardRoot.setPosition(Vec3.ZERO);
    cardRoot.addComponent(UITransform).setContentSize(1280, 720);
    this.cardRoot = cardRoot;
  }

  private createGameplayButton(parent: Node, name: string, text: string, x: number, y: number, width: number, height: number): Node {
    const node = new Node(name);
    node.setParent(parent);
    node.setPosition(new Vec3(x, y, 0));
    node.addComponent(UITransform).setContentSize(width, height);

    const graphics = node.addComponent(Graphics);
    graphics.fillColor = new Color(48, 137, 34);
    graphics.strokeColor = new Color(255, 214, 118);
    graphics.lineWidth = 3;
    graphics.roundRect(-width / 2, -height / 2, width, height, 16);
    graphics.fill();
    graphics.stroke();

    const labelNode = new Node(`${name}_Label`);
    labelNode.setParent(node);
    labelNode.addComponent(UITransform).setContentSize(width, height);
    const label = labelNode.addComponent(Label);
    label.string = text;
    label.fontSize = 24;
    label.lineHeight = height;
    label.color = Color.WHITE;
    label.horizontalAlign = Label.HorizontalAlign.CENTER;
    label.verticalAlign = Label.VerticalAlign.CENTER;

    return node;
  }

  private showLobby(): void {
    this.node.removeAllChildren();
    this.cardRoot = null;
    this.card = null;
    this.calledNumbers = [];
    const lobby = this.getComponent(RoomLobbyController) ?? this.addComponent(RoomLobbyController);
    lobby.start();
  }

  private refreshCardView(): void {
    if (!this.card) {
      return;
    }

    this.card.cells.forEach((cell) => {
      const label = this.cellLabels.get(this.getCellKey(cell));
      const graphics = this.cellBackgrounds.get(this.getCellKey(cell));

      if (!label || !graphics) {
        return;
      }

      label.string = this.getCellText(cell);
      this.paintCell(graphics, cell.state !== 'open');
      label.color = cell.state === 'open' ? new Color(55, 31, 84) : new Color(255, 255, 255);
    });
  }

  private createGridLabel(root: Node, text: string, x: number, y: number, size: number, isHeader: boolean): Label {
    const cellNode = new Node(isHeader ? `Header_${text}` : `Cell_${text}`);
    cellNode.setParent(root);
    cellNode.setPosition(new Vec3(x, y, 0));

    const transform = cellNode.addComponent(UITransform);
    transform.setContentSize(size, size);

    const graphics = cellNode.addComponent(Graphics);
    this.paintCell(graphics, isHeader);

    const labelNode = new Node(`${cellNode.name}_Label`);
    labelNode.setParent(cellNode);
    labelNode.setPosition(Vec3.ZERO);

    const labelTransform = labelNode.addComponent(UITransform);
    labelTransform.setContentSize(size, size);

    const label = labelNode.addComponent(Label);
    label.string = text;
    label.fontSize = isHeader ? 36 : 26;
    label.lineHeight = size;
    label.color = isHeader ? new Color(255, 255, 255) : new Color(55, 31, 84);

    return label;
  }

  private paintCell(graphics: Graphics, isActive: boolean): void {
    const transform = graphics.node.getComponent(UITransform);
    const size = transform?.contentSize.width ?? 82;
    const half = size / 2;

    graphics.clear();
    graphics.fillColor = isActive ? new Color(124, 73, 214) : new Color(247, 239, 255);
    graphics.strokeColor = new Color(255, 214, 118);
    graphics.lineWidth = 3;
    graphics.roundRect(-half, -half, size, size, 10);
    graphics.fill();
    graphics.stroke();
  }

  private getCardRoot(): Node {
    if (this.cardRoot) {
      return this.cardRoot;
    }

    return this.node;
  }

  private getCellKey(cell: BingoCell): string {
    return `${cell.column}-${cell.row}`;
  }

  private getCellText(cell: BingoCell): string {
    return cell.value === null ? 'FREE' : cell.value.toString();
  }

  private updateStatus(message: string): void {
    if (this.statusLabel) {
      this.statusLabel.string = message;
    }

    console.log(`[BingoMagicMayhem] ${message}`);
  }

  private hideLegacySceneControls(): void {
    if (!this.node.parent) {
      return;
    }

    for (const child of this.node.parent.children) {
      if (child !== this.node && (child.name === 'CallNumberButton' || child.name === 'Whispering Fen room ready')) {
        child.active = false;
      }
    }
  }
}
