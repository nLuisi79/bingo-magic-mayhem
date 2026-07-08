import fs from "node:fs/promises";
import path from "node:path";
import { FileBlob, SpreadsheetFile, Workbook } from "@oai/artifact-tool";

const sourcePath = "C:/Users/nLuis/OneDrive/Desktop/nLuisi Laptop/Bingo Magic Mayhem/_BMM Potions 2.xlsx";
const outputDir = "outputs/potions-tuning";
const outputPath = path.join(outputDir, "BMM Potions Tuned v1.xlsx");

const source = await FileBlob.load(sourcePath);
const sourceWorkbook = await SpreadsheetFile.importXlsx(source);
const sourceSheet = sourceWorkbook.worksheets.getItemAt(0);
const used = sourceSheet.getUsedRange();
const rows = used.values;

function cleanText(value) {
  if (value === null || value === undefined) {
    return "";
  }

  return String(value)
    .replace(/\uFFFD/g, "'")
    .replace(/\s+/g, " ")
    .trim();
}

function toNumber(value) {
  const number = Number(value);
  return Number.isFinite(number) ? number : null;
}

function getRealmBand(realmIndex) {
  if (realmIndex <= 2) return { min: 3, max: 8 };
  if (realmIndex <= 4) return { min: 4, max: 9 };
  if (realmIndex <= 6) return { min: 5, max: 10 };
  if (realmIndex <= 8) return { min: 6, max: 11 };
  return { min: 7, max: 11 };
}

function clamp(value, min, max) {
  return Math.max(min, Math.min(max, value));
}

function tuneCount(rawCount, rarity, realmIndex, roomIndex, ingredientIndex, ingredientTotal) {
  const band = getRealmBand(realmIndex);
  const numeric = rawCount ?? band.min + ((roomIndex + ingredientIndex) % Math.max(1, band.max - band.min + 1));
  let tuned = Math.round(numeric);

  tuned = clamp(tuned, 3, 11);
  tuned = clamp(tuned, band.min, band.max);

  if (rarity === "Common") {
    tuned = clamp(tuned + 1, band.min, band.max);
  } else if (rarity === "Rare" || rarity === "Key") {
    tuned = clamp(tuned - 1, 3, band.max);
  }

  const variation = (roomIndex + ingredientIndex + ingredientTotal) % 3;
  if (rarity === "Common" && variation === 0) {
    tuned = clamp(tuned - 1, band.min, band.max);
  }

  if (rarity !== "Common" && variation === 2) {
    tuned = clamp(tuned + 1, 3, band.max);
  }

  return clamp(tuned, 3, 11);
}

function rarityMix(totalIngredients, isSpecialRoom) {
  if (isSpecialRoom) {
    if (totalIngredients <= 5) return ["Common", "Common", "Uncommon", "Uncommon", "Key"];
    if (totalIngredients === 6) return ["Common", "Common", "Uncommon", "Uncommon", "Uncommon", "Key"];
    return ["Common", "Common", "Uncommon", "Uncommon", "Uncommon", "Rare", "Key"];
  }

  if (totalIngredients <= 5) return ["Common", "Common", "Common", "Uncommon", "Key"];
  if (totalIngredients === 6) return ["Common", "Common", "Common", "Uncommon", "Uncommon", "Key"];
  return ["Common", "Common", "Common", "Common", "Uncommon", "Uncommon", "Key"];
}

const realms = [];
let currentRealm = null;

for (let rowIndex = 1; rowIndex < rows.length; rowIndex++) {
  const row = rows[rowIndex] ?? [];
  const first = cleanText(row[0]);
  const potion = cleanText(row[1]);
  const hasAny = row.some((cell) => cleanText(cell) !== "");

  if (!hasAny) {
    continue;
  }

  if (first && !potion) {
    currentRealm = { name: first, rooms: [] };
    realms.push(currentRealm);
    continue;
  }

  if (!currentRealm || !first || !potion) {
    continue;
  }

  const ingredients = [];
  for (let col = 2; col < row.length; col += 3) {
    const ingredientName = cleanText(row[col]);
    const rawCount = toNumber(row[col + 1]);
    if (!ingredientName) {
      continue;
    }

    ingredients.push({
      name: ingredientName,
      sourceCount: rawCount,
      sourceRarity: cleanText(row[col + 2]),
    });
  }

  currentRealm.rooms.push({
    name: first,
    potion,
    ingredients,
  });
}

const tunedRows = [];
const issueRows = [];
const summaryRows = [];

for (let realmIndex = 0; realmIndex < realms.length; realmIndex++) {
  const realm = realms[realmIndex];
  let roomCount = 0;
  let ingredientCount = 0;
  let specialCount = 0;

  for (let roomIndex = 0; roomIndex < realm.rooms.length; roomIndex++) {
    const room = realm.rooms[roomIndex];
    const isSpecialRoom = roomIndex === 3;
    const mix = rarityMix(room.ingredients.length, isSpecialRoom);
    roomCount++;
    ingredientCount += room.ingredients.length;
    if (isSpecialRoom) specialCount++;

    if (room.ingredients.length < 5 || room.ingredients.length > 7) {
      issueRows.push([
        realm.name,
        room.name,
        "Ingredient count outside 5-7",
        room.ingredients.length,
        "Review ingredient list",
      ]);
    }

    for (let ingredientIndex = 0; ingredientIndex < room.ingredients.length; ingredientIndex++) {
      const ingredient = room.ingredients[ingredientIndex];
      const rarity = ingredient.sourceRarity || mix[Math.min(ingredientIndex, mix.length - 1)] || "Common";
      const tunedCount = tuneCount(
        ingredient.sourceCount,
        rarity,
        realmIndex + 1,
        roomIndex + 1,
        ingredientIndex + 1,
        room.ingredients.length,
      );

      tunedRows.push([
        realmIndex + 1,
        realm.name,
        roomIndex + 1,
        room.name,
        isSpecialRoom ? "Special" : "Standard",
        room.potion,
        ingredientIndex + 1,
        ingredient.name,
        ingredient.sourceCount ?? "",
        tunedCount,
        rarity,
        ingredient.sourceRarity ? "Kept source rarity" : "Assigned by room mix",
      ]);
    }
  }

  summaryRows.push([
    realmIndex + 1,
    realm.name,
    roomCount,
    specialCount,
    ingredientCount,
    getRealmBand(realmIndex + 1).min,
    getRealmBand(realmIndex + 1).max,
  ]);
}

const workbook = Workbook.create();
const catalog = workbook.worksheets.add("Tuned Catalog");
const summary = workbook.worksheets.add("Realm Summary");
const rules = workbook.worksheets.add("Tuning Rules");
const issues = workbook.worksheets.add("Review Notes");

catalog.showGridLines = false;
summary.showGridLines = false;
rules.showGridLines = false;
issues.showGridLines = false;

catalog.getRange("A1:L1").values = [[
  "Realm #",
  "Realm",
  "Room #",
  "Room",
  "Room Type",
  "Potion",
  "Ingredient #",
  "Ingredient",
  "Source Count",
  "Tuned Count",
  "Rarity",
  "Rarity Source",
]];
catalog.getRangeByIndexes(1, 0, tunedRows.length, 12).values = tunedRows;

summary.getRange("A1:G1").values = [[
  "Realm #",
  "Realm",
  "Rooms",
  "Special Rooms",
  "Ingredient Types",
  "Min Count Band",
  "Max Count Band",
]];
summary.getRangeByIndexes(1, 0, summaryRows.length, 7).values = summaryRows;

rules.getRange("A1:B13").values = [
  ["Rule", "Value"],
  ["First content batch", "Realms 1-10"],
  ["Ingredient types per room", "5-7"],
  ["Tuned count floor", "3"],
  ["Tuned count ceiling for first 10 realms", "11"],
  ["Standard 5-ingredient mix", "3 Common, 1 Uncommon, 1 Key"],
  ["Standard 6-ingredient mix", "3 Common, 2 Uncommon, 1 Key"],
  ["Standard 7-ingredient mix", "4 Common, 2 Uncommon, 1 Key"],
  ["Special 5-ingredient mix", "2 Common, 2 Uncommon, 1 Key"],
  ["Special 6-ingredient mix", "2 Common, 3 Uncommon, 1 Key"],
  ["Special 7-ingredient mix", "2 Common, 3 Uncommon, 1 Rare, 1 Key"],
  ["Special room assumption", "4th room in each realm"],
  ["Gameplay assumption", "Bingos guarantee ingredient quantity; rarity decides ingredient mix"],
];

issues.getRange("A1:E1").values = [["Realm", "Room", "Issue", "Value", "Recommendation"]];
if (issueRows.length > 0) {
  issues.getRangeByIndexes(1, 0, issueRows.length, 5).values = issueRows;
} else {
  issues.getRange("A2:E2").values = [["None", "None", "No structural issues found", "", ""]];
}

for (const sheet of [catalog, summary, rules, issues]) {
  const usedRange = sheet.getUsedRange();
  usedRange.format.font = { name: "Aptos", size: 10, color: "#1F1733" };
  usedRange.format.wrapText = true;
  const header = sheet.getRangeByIndexes(0, 0, 1, usedRange.columnCount);
  header.format = {
    fill: "#2B1248",
    font: { bold: true, color: "#FFFFFF" },
  };
  header.format.rowHeightPx = 30;
  usedRange.format.borders = { preset: "inside", style: "thin", color: "#E2D8F0" };
  sheet.freezePanes.freezeRows(1);
}

catalog.getRange("A:L").format.autofitColumns();
summary.getRange("A:G").format.autofitColumns();
rules.getRange("A:B").format.autofitColumns();
issues.getRange("A:E").format.autofitColumns();

catalog.getRange("I:J").format.numberFormat = "#,##0";
summary.getRange("A:G").format.numberFormat = "#,##0";

await fs.mkdir(outputDir, { recursive: true });
const out = await SpreadsheetFile.exportXlsx(workbook);
await out.save(outputPath);

const inspect = await workbook.inspect({
  kind: "sheet,table",
  maxChars: 4000,
  tableMaxRows: 6,
  tableMaxCols: 8,
});

console.log(inspect.ndjson);
console.log(`Saved ${outputPath}`);
