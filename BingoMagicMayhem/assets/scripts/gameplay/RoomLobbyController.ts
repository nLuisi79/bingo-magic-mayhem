import { _decorator, Color, Component, Graphics, Input, Label, Node, UITransform, Vec3 } from 'cc';
import { BingoPrototypeController } from './BingoPrototypeController';

const { ccclass } = _decorator;

interface IngredientProgress {
  name: string;
  icon: string;
  current: number;
  required: number;
}

interface CardOption {
  count: number;
  ingredients: string;
  regularChance: number;
  specialChance: number;
  jackpotChance: string;
}

const ROOM_DATA = {
  roomName: 'Pollenspire Fountain',
  realmName: 'Sunpetal Conservatory',
  badge: 'Standard Bingo',
  mana: '100,507',
  crystals: '8,667',
  jackpot: '5,000 Mana',
  ingredients: [
    { name: 'Sunpetal Pollen', icon: '*', current: 4, required: 9 },
    { name: 'Glass Dew', icon: 'o', current: 2, required: 6 },
    { name: 'Honeycluster', icon: '@', current: 1, required: 4 },
  ] as IngredientProgress[],
  cards: [
    { count: 1, ingredients: 'Up to 2 Ingredients', regularChance: 100, specialChance: 0, jackpotChance: '0.25%' },
    { count: 2, ingredients: 'Up to 3 Ingredients', regularChance: 92, specialChance: 8, jackpotChance: '0.50%' },
    { count: 4, ingredients: 'Up to 4 Ingredients', regularChance: 80, specialChance: 18, jackpotChance: '0.75%' },
    { count: 6, ingredients: 'Up to 5 Ingredients', regularChance: 68, specialChance: 28, jackpotChance: '1.00%' },
  ] as CardOption[],
};

@ccclass('RoomLobbyController')
export class RoomLobbyController extends Component {
  private selectedCardCount = 4;
  private manaBetPerCard = 25;
  private readonly minBet = 25;
  private readonly maxBet = 250;
  private readonly betStep = 25;
  private cardOptionNodes = new Map<number, Node>();
  private cardCostLabels = new Map<number, Label>();
  private betLabel: Label | null = null;
  private statusLabel: Label | null = null;
  private rewardPreview: Node | null = null;

  public start(): void {
    this.buildLobby();
  }

  private buildLobby(): void {
    this.node.removeAllChildren();

    const root = this.createNode('RoomLobbyRoot', this.node, 0, 0, 1280, 720);
    this.drawBackground(root);
    this.buildTopHeader(root);
    this.buildLeftPanel(root);
    this.buildModeTabs(root);
    this.buildCenterCards(root);
    this.buildRightPanel(root);
    this.buildBetControls(root);
    this.buildStatus(root);
    this.buildRewardPreview(root);
    this.refreshSelection();
  }

  private buildTopHeader(root: Node): void {
    this.drawRoomEmblem(root, -548, 252);
    this.createPanel(root, 'RoomTitlePanel', 0, 278, 575, 100, new Color(255, 245, 219), new Color(214, 151, 48), 16);
    this.createLabel(root, ROOM_DATA.roomName, 0, 298, 42, new Color(52, 24, 86), 560, 50, true);
    this.createLabel(root, ROOM_DATA.realmName, 0, 263, 22, new Color(128, 71, 20), 520, 32, true);

    this.createPanel(root, 'RoomBadge', -464, 286, 230, 56, new Color(71, 29, 121), new Color(255, 212, 92), 18);
    this.createLabel(root, ROOM_DATA.badge, -464, 286, 21, Color.WHITE, 220, 30, true);

    this.createPanel(root, 'ManaCounter', 355, 318, 178, 46, new Color(28, 24, 38), new Color(255, 201, 70), 20);
    this.createLabel(root, `M ${ROOM_DATA.mana}  +`, 355, 318, 23, Color.WHITE, 168, 30, true);
    this.createPanel(root, 'CrystalCounter', 545, 318, 160, 46, new Color(28, 24, 38), new Color(113, 214, 255), 20);
    this.createLabel(root, `C ${ROOM_DATA.crystals}  +`, 545, 318, 23, Color.WHITE, 150, 30, true);

    const mapButton = this.createButton(root, 'MapButton', '< Map', -590, 314, 110, 48, new Color(88, 38, 142));
    mapButton.on(Input.EventType.TOUCH_END, () => this.setStatus('Realm map coming soon.'), this);
  }

  private buildLeftPanel(root: Node): void {
    this.createPanel(root, 'RestorePanel', -310, 151, 610, 136, new Color(255, 244, 216), new Color(214, 151, 48), 14);
    this.createLabel(root, 'Collect Ingredients to Restore', -310, 201, 23, new Color(45, 24, 83), 570, 30, true);

    ROOM_DATA.ingredients.forEach((ingredient, index) => {
      const x = -516 + index * 184;
      this.createPanel(root, `Ingredient_${ingredient.name}`, x, 135, 166, 76, new Color(255, 249, 232), new Color(232, 190, 94), 12);
      this.drawIngredientIcon(root, ingredient.name, x - 55, 139);
      this.createLabel(root, ingredient.name, x + 25, 150, 14, new Color(45, 24, 83), 104, 30, true);
      this.createLabel(root, `${ingredient.current}/${ingredient.required}`, x + 25, 118, 23, new Color(21, 96, 62), 92, 26, true);
    });

    const reward = this.createButton(root, 'RestorationReward', '', 95, 151, 165, 136, new Color(255, 244, 216), new Color(214, 151, 48));
    this.createLabel(root, 'Restore Reward', 95, 190, 18, new Color(45, 24, 83), 135, 24, true);
    this.drawChest(root, 95, 144);
    this.createLabel(root, 'Tap to Preview', 95, 101, 17, new Color(45, 24, 83), 150, 26, true);
    reward.on(Input.EventType.TOUCH_END, () => this.toggleRewardPreview(), this);
  }

  private buildModeTabs(root: Node): void {
    const classic = this.createButton(root, 'ClassicModeButton', '', -452, 54, 132, 58, new Color(89, 31, 143), new Color(255, 201, 70));
    const pattern = this.createButton(root, 'PatternModeButton', '', -306, 54, 132, 58, new Color(56, 24, 86), new Color(255, 201, 70));
    this.createLabel(classic, 'Classic', 0, -10, 17, Color.WHITE, 116, 24, true);
    this.createLabel(classic, '[BINGO]', 0, 13, 16, new Color(255, 238, 190), 116, 22, true);
    this.createLabel(pattern, 'Pattern', 0, -10, 17, Color.WHITE, 116, 24, true);
    this.createLabel(pattern, '[x x]', 0, 13, 16, new Color(255, 238, 190), 116, 22, true);
    classic.on(Input.EventType.TOUCH_END, () => this.setStatus('Classic bingo selected.'), this);
    pattern.on(Input.EventType.TOUCH_END, () => this.setStatus('Pattern rooms coming soon.'), this);
  }

  private buildCenterCards(root: Node): void {
    this.createLabel(root, 'Pick Cards to Play!', 0, 47, 34, new Color(74, 31, 103), 520, 42, true);

    ROOM_DATA.cards.forEach((option, index) => {
      const x = -360 + index * 240;
      const optionNode = this.createButton(root, `CardOption_${option.count}`, '', x, -111, 214, 268, new Color(255, 246, 222), new Color(214, 151, 48));
      this.createPanel(optionNode, 'CardTileGlow', 0, -2, 184, 226, new Color(255, 238, 159, 54), new Color(255, 238, 159, 0), 20, 0);
      this.cardOptionNodes.set(option.count, optionNode);
      optionNode.on(Input.EventType.TOUCH_END, () => this.selectCardCount(option.count), this);

      this.createPanel(optionNode, 'Ribbon', 0, 112, 150, 34, new Color(89, 31, 143), new Color(255, 201, 70), 10);
      this.createLabel(optionNode, `${option.count} ${option.count === 1 ? 'Card' : 'Cards'}`, 0, 112, 21, Color.WHITE, 140, 26, true);
      this.drawMiniCards(optionNode, option.count, 0, 73);
      this.createLabel(optionNode, option.ingredients, 0, 35, 14, new Color(45, 24, 83), 184, 20, true);
      this.createLabel(optionNode, 'Power-Ups       * * *', 0, 11, 13, new Color(45, 24, 83), 180, 19, false);
      this.createLabel(optionNode, 'Special Event   * *', 0, -12, 13, new Color(45, 24, 83), 180, 19, false);
      this.createPanel(optionNode, 'ChanceRule', 0, -30, 170, 1, new Color(214, 151, 48, 120), new Color(214, 151, 48, 0), 0, 0);
      this.createLabel(optionNode, `Regular Card       ${option.regularChance}%`, 0, -43, 12, new Color(26, 26, 52), 176, 18, false);
      this.createLabel(optionNode, `Special Card        ${option.specialChance}%`, 0, -64, 12, new Color(26, 26, 52), 176, 18, false);
      this.createLabel(optionNode, `Jackpot             ${option.jackpotChance}`, 0, -85, 12, new Color(26, 26, 52), 176, 18, false);

      const costButton = this.createButton(optionNode, `Enter_${option.count}_Cards`, '', 0, -116, 170, 40, new Color(39, 137, 30), new Color(255, 201, 70));
      const costLabel = this.createLabel(costButton, '', 0, 0, 21, Color.WHITE, 160, 28, true);
      this.cardCostLabels.set(option.count, costLabel);
      costButton.on(Input.EventType.TOUCH_END, () => this.enterRoom(option.count), this);
    });
  }

  private buildRightPanel(root: Node): void {
    this.createPanel(root, 'JackpotPanel', 460, 151, 330, 158, new Color(78, 21, 110), new Color(255, 201, 70), 18);
    this.drawJackpotWheel(root, 340, 198);
    this.createLabel(root, 'GRAND JACKPOT', 485, 194, 29, Color.WHITE, 260, 34, true);
    this.createLabel(root, `Today's Jackpot: ${ROOM_DATA.jackpot}`, 460, 142, 24, new Color(255, 235, 73), 300, 30, true);
    this.createLabel(root, 'Higher bets improve jackpot chance.', 460, 105, 15, Color.WHITE, 285, 24, true);

    this.createPanel(root, 'EffectsPanel', -562, -102, 142, 368, new Color(62, 21, 103), new Color(255, 201, 70), 18);
    this.createLabel(root, 'ACTIVE\nEFFECTS', -562, 54, 17, Color.WHITE, 118, 46, true);
    this.createLabel(root, 'Called Balls\nVisible', -562, -7, 15, Color.WHITE, 116, 40, true);
    this.createLabel(root, '59m 45s', -562, -52, 17, new Color(255, 240, 180), 116, 26, true);
    this.createLabel(root, '15 MIN\nSUPER\nPOWER-UPS', -562, -124, 15, Color.WHITE, 126, 62, true);
    this.createLabel(root, '14m 59s', -562, -184, 17, new Color(255, 240, 180), 116, 26, true);
    this.createLabel(root, 'CARD VIEW\nAll Visible\nGrouped', -562, -248, 15, Color.WHITE, 126, 66, true);
  }

  private buildBetControls(root: Node): void {
    this.createPanel(root, 'BetPanel', 0, -321, 480, 78, new Color(255, 246, 222), new Color(214, 151, 48), 24);
    this.createLabel(root, 'Mana Bet Per Card', 0, -295, 17, new Color(45, 24, 83), 260, 22, true);
    this.createPanel(root, 'BetTrack', 0, -324, 280, 18, new Color(91, 39, 148), new Color(70, 32, 96), 10);
    this.betLabel = this.createLabel(root, '', 0, -348, 19, new Color(45, 24, 83), 260, 26, true);

    const minus = this.createButton(root, 'DecreaseBet', '-', -203, -326, 58, 58, new Color(94, 38, 146));
    const plus = this.createButton(root, 'IncreaseBet', '+', 203, -326, 58, 58, new Color(94, 38, 146));
    minus.on(Input.EventType.TOUCH_END, () => this.adjustBet(-this.betStep), this);
    plus.on(Input.EventType.TOUCH_END, () => this.adjustBet(this.betStep), this);
  }

  private buildStatus(root: Node): void {
    this.statusLabel = this.createLabel(root, '', 460, 48, 15, Color.WHITE, 310, 24, true);
  }

  private buildRewardPreview(root: Node): void {
    this.rewardPreview = this.createPanel(root, 'RewardPreview', 0, 0, 390, 250, new Color(42, 18, 71), new Color(255, 201, 70), 18);
    this.createLabel(this.rewardPreview, 'Restoration Reward', 0, 78, 27, Color.WHITE, 330, 36, true);
    this.createLabel(this.rewardPreview, 'Preview rewards for restoring Pollenspire Fountain:', 0, 34, 16, new Color(255, 238, 190), 330, 28, true);
    this.createLabel(this.rewardPreview, '+ Sunpetal Conservatory relic\n+ Mana bonus\n+ Grimoire card pack\n+ Portal Surge chance', 0, -34, 19, Color.WHITE, 330, 104, true);
    const close = this.createButton(this.rewardPreview, 'CloseRewardPreview', 'Close', 0, -92, 120, 42, new Color(94, 38, 146));
    close.on(Input.EventType.TOUCH_END, () => this.toggleRewardPreview(false), this);
    this.rewardPreview.active = false;
  }

  private selectCardCount(count: number): void {
    this.selectedCardCount = count;
    this.refreshSelection();
    this.setStatus(`${count} ${count === 1 ? 'card' : 'cards'} selected.`);
  }

  private adjustBet(delta: number): void {
    this.manaBetPerCard = Math.max(this.minBet, Math.min(this.maxBet, this.manaBetPerCard + delta));
    this.refreshSelection();
    this.setStatus(`Mana bet set to ${this.manaBetPerCard} per card.`);
  }

  private refreshSelection(): void {
    this.cardOptionNodes.forEach((node, count) => {
      const graphics = node.getComponent(Graphics);
      if (graphics) {
        this.paintPanel(graphics, 214, 268, count === this.selectedCardCount ? new Color(255, 235, 164) : new Color(255, 246, 222), new Color(214, 151, 48), 14, 4);
      }
    });

    this.cardCostLabels.forEach((label, count) => {
      label.string = `${count * this.manaBetPerCard} Mana`;
    });

    if (this.betLabel) {
      this.betLabel.string = `${this.manaBetPerCard} Mana / Card`;
    }
  }

  private enterRoom(cardCount = this.selectedCardCount): void {
    this.selectedCardCount = cardCount;
    this.setStatus(`Entering: ${this.selectedCardCount} cards, ${this.manaBetPerCard} Mana each.`);
    const gameplay = this.getComponent(BingoPrototypeController);
    gameplay?.beginMatchFromLobby(this.selectedCardCount, this.manaBetPerCard);
  }

  private toggleRewardPreview(force?: boolean): void {
    if (!this.rewardPreview) {
      return;
    }

    this.rewardPreview.active = force ?? !this.rewardPreview.active;
  }

  private setStatus(message: string): void {
    if (this.statusLabel) {
      this.statusLabel.string = message;
    }
  }

  private drawBackground(root: Node): void {
    this.createPanel(root, 'Background', 0, 0, 1280, 720, new Color(18, 82, 73), new Color(18, 82, 73), 0, 0);
    this.createPanel(root, 'SunlitConservatoryWash', 0, 94, 1160, 430, new Color(73, 156, 125, 98), new Color(73, 156, 125, 0), 36, 0);
    this.drawGlassRoof(root);
    this.drawVines(root);
    this.createPanel(root, 'StoneFloorGlow', 0, -274, 940, 92, new Color(111, 100, 68, 138), new Color(236, 190, 88), 32, 1);
    this.createPanel(root, 'CardStage', 116, -156, 860, 132, new Color(86, 78, 53, 120), new Color(225, 176, 70), 24, 1);
  }

  private drawGlassRoof(root: Node): void {
    for (let index = 0; index < 9; index++) {
      const x = -480 + index * 120;
      this.createPanel(root, `GlassRoofPane_${index}`, x, 248, 86, 126, new Color(159, 226, 226, 42), new Color(179, 237, 218, 80), 8, 1);
    }

    this.createCircle(root, 'SunGlow', 0, 214, 190, new Color(255, 239, 165, 42), new Color(255, 239, 165, 0), 0);
  }

  private drawVines(root: Node): void {
    for (let index = 0; index < 12; index++) {
      const y = 265 - index * 42;
      this.createCircle(root, `LeftLeaf_${index}`, -610 + (index % 2) * 16, y, 11, new Color(65, 154, 82, 150), new Color(26, 98, 55, 80), 1);
      this.createCircle(root, `RightLeaf_${index}`, 610 - (index % 2) * 16, y, 11, new Color(65, 154, 82, 150), new Color(26, 98, 55, 80), 1);
    }

    for (let index = 0; index < 7; index++) {
      this.drawFlower(root, -580 + index * 190, -315 + (index % 2) * 24, 9);
    }
  }

  private drawRoomEmblem(root: Node, x: number, y: number): void {
    this.createCircle(root, 'RoomEmblemOuter', x, y, 58, new Color(92, 34, 137), new Color(255, 201, 70), 4);
    this.createCircle(root, 'RoomEmblemInner', x, y, 44, new Color(32, 144, 161), new Color(255, 238, 138), 2);
    this.drawFlower(root, x, y, 23);
  }

  private drawIngredientIcon(root: Node, name: string, x: number, y: number): void {
    if (name === 'Sunpetal Pollen') {
      this.drawFlower(root, x, y, 20);
      return;
    }

    if (name === 'Glass Dew') {
      this.createCircle(root, 'GlassDewIcon', x, y, 24, new Color(73, 213, 255), new Color(255, 255, 255), 3);
      this.createCircle(root, 'GlassDewShine', x - 8, y + 8, 7, new Color(255, 255, 255, 190), new Color(255, 255, 255, 0), 0);
      return;
    }

    this.createCircle(root, 'HoneyclusterA', x - 10, y + 8, 12, new Color(238, 156, 18), new Color(255, 222, 88), 2);
    this.createCircle(root, 'HoneyclusterB', x + 8, y + 8, 12, new Color(238, 156, 18), new Color(255, 222, 88), 2);
    this.createCircle(root, 'HoneyclusterC', x - 1, y - 8, 12, new Color(238, 156, 18), new Color(255, 222, 88), 2);
  }

  private drawChest(root: Node, x: number, y: number): void {
    this.createPanel(root, 'ChestBase', x, y - 8, 76, 38, new Color(147, 72, 26), new Color(255, 201, 70), 7, 3);
    this.createPanel(root, 'ChestLid', x, y + 16, 82, 28, new Color(188, 90, 32), new Color(255, 218, 98), 11, 3);
    this.createPanel(root, 'ChestBand', x, y + 2, 14, 54, new Color(255, 201, 70), new Color(122, 55, 11), 4, 1);
    this.createCircle(root, 'ChestGem', x, y + 8, 8, new Color(199, 72, 210), new Color(255, 238, 138), 2);
  }

  private drawJackpotWheel(root: Node, x: number, y: number): void {
    const colors = [
      new Color(42, 144, 224),
      new Color(126, 67, 214),
      new Color(220, 66, 159),
      new Color(255, 143, 41),
      new Color(255, 218, 61),
      new Color(48, 175, 92),
    ];

    colors.forEach((color, index) => {
      const angle = (Math.PI * 2 * index) / colors.length;
      this.createCircle(root, `JackpotWheelSegment_${index}`, x + Math.cos(angle) * 12, y + Math.sin(angle) * 12, 28, new Color(color.r, color.g, color.b, 92), new Color(255, 226, 120, 60), 1);
    });

    this.createCircle(root, 'JackpotWheelHub', x, y, 10, new Color(255, 218, 61), new Color(255, 255, 255), 2);
  }

  private drawFlower(root: Node, x: number, y: number, size: number): void {
    for (let index = 0; index < 8; index++) {
      const angle = (Math.PI * 2 * index) / 8;
      this.createCircle(root, `FlowerPetal_${x}_${y}_${index}`, x + Math.cos(angle) * size * 0.58, y + Math.sin(angle) * size * 0.58, size * 0.34, new Color(238, 82, 176, 210), new Color(255, 226, 120), 1);
    }

    this.createCircle(root, `FlowerCenter_${x}_${y}`, x, y, size * 0.35, new Color(255, 213, 54), new Color(255, 250, 189), 1);
  }

  private drawMiniCards(parent: Node, count: number, centerX: number, centerY: number): void {
    const cardWidth = count === 1 ? 58 : 28;
    const cardHeight = 36;
    const visibleCount = Math.min(count, 6);
    const spacing = count === 1 ? 0 : 22;
    const startX = centerX - ((visibleCount - 1) * spacing) / 2;

    for (let index = 0; index < visibleCount; index++) {
      const x = startX + index * spacing;
      const card = this.createPanel(parent, `MiniCard_${index + 1}`, x, centerY, cardWidth, cardHeight, new Color(255, 250, 235), new Color(214, 151, 48), 4, 2);
      this.createLabel(card, count === 1 ? 'BINGO' : '', 0, 8, count === 1 ? 10 : 8, new Color(146, 73, 30), cardWidth - 6, 12, true);
      this.createPanel(card, 'MiniDaubA', -8, -5, 8, 8, new Color(126, 67, 214), new Color(126, 67, 214), 2, 0);
      this.createPanel(card, 'MiniDaubB', 8, 5, 8, 8, new Color(255, 212, 92), new Color(255, 212, 92), 2, 0);
    }
  }

  private createButton(parent: Node, name: string, text: string, x: number, y: number, width: number, height: number, fill: Color, stroke = new Color(255, 201, 70)): Node {
    const node = this.createPanel(parent, name, x, y, width, height, fill, stroke, Math.min(20, height / 3));
    if (text) {
      this.createLabel(node, text, 0, 0, Math.min(24, height / 2), Color.WHITE, width - 18, height - 12, true);
    }
    return node;
  }

  private createPanel(parent: Node, name: string, x: number, y: number, width: number, height: number, fill: Color, stroke: Color, radius = 12, lineWidth = 3): Node {
    const node = this.createNode(name, parent, x, y, width, height);
    const graphics = node.addComponent(Graphics);
    this.paintPanel(graphics, width, height, fill, stroke, radius, lineWidth);
    return node;
  }

  private createCircle(parent: Node, name: string, x: number, y: number, radius: number, fill: Color, stroke: Color, lineWidth: number): Node {
    const node = this.createNode(name, parent, x, y, radius * 2, radius * 2);
    const graphics = node.addComponent(Graphics);
    graphics.fillColor = fill;
    graphics.strokeColor = stroke;
    graphics.lineWidth = lineWidth;
    graphics.circle(0, 0, radius);
    graphics.fill();
    if (lineWidth > 0) {
      graphics.stroke();
    }
    return node;
  }

  private paintPanel(graphics: Graphics, width: number, height: number, fill: Color, stroke: Color, radius: number, lineWidth: number): void {
    graphics.clear();
    graphics.fillColor = fill;
    graphics.strokeColor = stroke;
    graphics.lineWidth = lineWidth;
    graphics.roundRect(-width / 2, -height / 2, width, height, radius);
    graphics.fill();
    if (lineWidth > 0) {
      graphics.stroke();
    }
  }

  private createLabel(parent: Node, text: string, x: number, y: number, fontSize: number, color: Color, width: number, height: number, centered: boolean): Label {
    const node = this.createNode(`Label_${text.slice(0, 18)}`, parent, x, y, width, height);
    const label = node.addComponent(Label);
    label.string = text;
    label.fontSize = fontSize;
    label.lineHeight = Math.max(fontSize + 6, Math.min(height, fontSize * 1.35));
    label.color = color;
    label.horizontalAlign = centered ? Label.HorizontalAlign.CENTER : Label.HorizontalAlign.LEFT;
    label.verticalAlign = Label.VerticalAlign.CENTER;
    label.overflow = Label.Overflow.SHRINK;
    return label;
  }

  private createNode(name: string, parent: Node, x: number, y: number, width: number, height: number): Node {
    const node = new Node(name);
    node.setParent(parent);
    node.setPosition(new Vec3(x, y, 0));
    const transform = node.addComponent(UITransform);
    transform.setContentSize(width, height);
    return node;
  }
}
