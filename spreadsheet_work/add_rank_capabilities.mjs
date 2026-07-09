import fs from "node:fs/promises";
import { FileBlob, SpreadsheetFile } from "@oai/artifact-tool";

const inputPath = "C:/Users/nLuis/OneDrive/Desktop/nLuisi Laptop/Bingo Magic Mayhem/_BMM Potions 2.xlsx";
const outputDir = "outputs/rank_capabilities";
const outputPath = `${outputDir}/_BMM Potions 2 - Rank Capabilities.xlsx`;
await fs.mkdir(outputDir, { recursive: true });

const input = await FileBlob.load(inputPath);
const workbook = await SpreadsheetFile.importXlsx(input);
const sheet = workbook.worksheets.add("Rank Capabilities");
sheet.showGridLines = false;

const ranks = [
  ["TBD", "Novice", "Nov", 0, 3, 0, "TBD", "TBD", "None", "Starter profile identity"],
  ["TBD", "Apprentice", "App", 0.02, 4, 0, "TBD", "TBD", "Small", "Basic frame"],
  ["TBD", "Spellbinder", "Spellbinder", 0.04, 5, 1, "TBD", "TBD", "Small+", "First daub unlock"],
  ["TBD", "Mage", "Mage", 0.05, 6, 1, "TBD", "TBD", "Medium", "Rank badge or title"],
  ["TBD", "Thaumaturge", "Thaum", 0.08, 7, 1, "TBD", "TBD", "Medium+", "Avatar option"],
  ["TBD", "Mystic", "Mystic", 0.10, 8, 2, "TBD", "TBD", "Strong", "Premium frame"],
  ["TBD", "Enchanter", "Enchant", 0.12, 9, 2, "TBD", "TBD", "Strong", "Animated daub"],
  ["TBD", "Wizard", "Wizard", 0.15, 10, 2, "TBD", "TBD", "Very Strong", "Prestige frame"],
  ["TBD", "Spellmaster", "Spellmaster", 0.18, 11, 3, "TBD", "TBD", "Very Strong", "Advanced daub"],
  ["TBD", "Archmage", "Archmage", 0.20, 12, 3, "TBD", "TBD", "Prestige", "Elite title/frame"],
  ["TBD", "Grand Archmage", "Grand Arch", 0.22, 13, 3, "TBD", "TBD", "Prestige+", "Rare avatar/robe"],
  ["TBD", "Paragon", "Paragon", 0.25, 14, 4, "TBD", "TBD", "Premium", "Paragon visual set"],
  ["TBD", "Ascendant", "Ascend", 0.30, 15, 4, "TBD", "TBD", "Major", "Ascendant visual set"],
  ["TBD", "Sorcerer Supreme", "Supreme", 0.35, 16, 5, "TBD", "TBD", "Supreme", "Exclusive treatment"],
];

sheet.getRange("A1:I1").merge();
sheet.getRange("A1").values = [["Rank Capabilities Working Draft"]];
sheet.getRange("A1").format = {
  fill: "#2B0E43",
  font: { bold: true, color: "#FFE869", size: 18 },
  horizontalAlignment: "center",
};
sheet.getRange("A2:I2").merge();
sheet.getRange("A2").values = [["Status: review surface only. Rank is Aura-derived, not Level-derived. Aura thresholds and formula are TBD."]];
sheet.getRange("A2").format = {
  fill: "#F5D99B",
  font: { bold: true, color: "#3B2350" },
  horizontalAlignment: "center",
  wrapText: true,
};

sheet.getRange("A4:I4").values = [[
  "Aura Band",
  "Rank",
  "Comfort Bonus",
  "Ingredient Sends",
  "Card Gift/Trade",
  "Friend Mana Send",
  "Friend Mana Receive",
  "Rank-Up Chest",
  "Identity Unlock Direction",
]];
sheet.getRange("A5:I18").values = ranks.map((row) => [row[0], row[1], row[3], row[4], row[5], row[6], row[7], row[8], row[9]]);
const tableRange = sheet.getRange("A4:I18");
tableRange.format.borders = { preset: "inside", style: "thin", color: "#D8C58D" };
sheet.getRange("A4:I4").format = {
  fill: "#4B1674",
  font: { bold: true, color: "#FFFFFF" },
  horizontalAlignment: "center",
  wrapText: true,
};
sheet.getRange("A5:I18").format = {
  fill: "#FFF2CC",
  font: { color: "#2D193B" },
  verticalAlignment: "center",
  wrapText: true,
};
sheet.getRange("C5:C18").format.numberFormat = "0%";
sheet.getRange("D5:E18").format.numberFormat = "#,##0";
sheet.getRange("F5:G18").format = {
  fill: "#F3E7FF",
  font: { color: "#5B2A73", italic: true },
  horizontalAlignment: "center",
};
sheet.getRange("A4:I18").format.borders = { preset: "all", style: "thin", color: "#D8C58D" };

sheet.getRange("K4:N4").values = [["Visual Summary", "Value", "Scope", "Notes"]];
sheet.getRange("K4:N4").format = {
  fill: "#4B1674",
  font: { bold: true, color: "#FFFFFF" },
  horizontalAlignment: "center",
};
sheet.getRange("K5:N10").values = [
  ["Current prototype rank", "Apprentice", "Level 37 shown in prototype", "Card gifts/trades are 0/day under this draft."],
  ["First card gift/trade unlock", "Spellbinder", "Aura threshold TBD", "Starts at 1/day."],
  ["Max comfort bonus", "35%", "Sorcerer Supreme", "Only for approved daily/account comfort sources."],
  ["Max ingredient sends/day", 16, "Sorcerer Supreme", "Request amount remains 10 total per 48-hour wish list."],
  ["Max card gifts/trades/day", 5, "Sorcerer Supreme", "Shared vs separate send/receive limit is still open."],
  ["Purchases and Aura", "Capped", "Aura source", "Purchases cannot independently cause rank advancement."],
  ["Friend mana send/receive caps", "TBD", "Friends system", "User requested Aura-rank based caps; values not locked yet."],
];
sheet.getRange("K5:N11").format = {
  fill: "#FFF2CC",
  font: { color: "#2D193B" },
  wrapText: true,
};
sheet.getRange("K4:N11").format.borders = { preset: "all", style: "thin", color: "#D8C58D" };

sheet.getRange("K12:N12").values = [["Comfort Bonus Applies To", "", "Comfort Bonus Must Not Apply To", ""]];
sheet.getRange("K12:N12").format = {
  fill: "#2B0E43",
  font: { bold: true, color: "#FFE869" },
  horizontalAlignment: "center",
};
sheet.getRange("K13:L18").values = [
  ["Daily Bonus mana", ""],
  ["Daily Spin common mana", ""],
  ["Mana Cauldron refill/capacity", ""],
  ["Level-up currency rewards", ""],
  ["Enchanted Trail free-path currency", ""],
  ["Only if approved per reward source", ""],
];
sheet.getRange("M13:N23").values = [
  ["Bingo odds", ""],
  ["Jackpot odds or values", ""],
  ["Rare/Gilded/Ancient card odds", ""],
  ["Ingredient drop odds", ""],
  ["Trade or duplicate market value", ""],
  ["Coven/event competitive scoring", ""],
  ["Room/realm progression requirements", ""],
  ["Inbox gifts unless reward source is eligible", ""],
  ["Any hidden broad reward multiplier", ""],
  ["Purchase-only rank advancement", ""],
  ["Purchase bypass of gameplay progress", ""],
];
sheet.getRange("K13:L18").format = { fill: "#E7F4D8", font: { color: "#244015" }, wrapText: true };
sheet.getRange("M13:N23").format = { fill: "#F8D7DA", font: { color: "#5B1D25" }, wrapText: true };
sheet.getRange("K12:N23").format.borders = { preset: "all", style: "thin", color: "#D8C58D" };

sheet.getRange("A21:B35").values = [["Rank", "Comfort Bonus"], ...ranks.map((row) => [row[2], row[3]])];
sheet.getRange("D21:E35").values = [["Rank", "Ingredient Sends"], ...ranks.map((row) => [row[2], row[4]])];
sheet.getRange("G21:H35").values = [["Rank", "Card Gift/Trade"], ...ranks.map((row) => [row[2], row[5]])];
sheet.getRange("A21:H21").format = {
  fill: "#4B1674",
  font: { bold: true, color: "#FFFFFF" },
};
sheet.getRange("B22:B35").format.numberFormat = "0%";
sheet.getRange("A21:H35").format.borders = { preset: "all", style: "thin", color: "#D8C58D" };

const comfortChart = sheet.charts.add("bar", sheet.getRange("A21:B35"));
comfortChart.title = "Comfort Bonus by Rank";
comfortChart.hasLegend = false;
comfortChart.xAxis = { axisType: "textAxis", textStyle: { fontSize: 8 } };
comfortChart.yAxis = { numberFormatCode: "0%" };
comfortChart.setPosition("K23", "N39");

const socialChart = sheet.charts.add("bar", sheet.getRange("D21:E35"));
socialChart.title = "Ingredient Sends by Rank";
socialChart.hasLegend = false;
socialChart.xAxis = { axisType: "textAxis", textStyle: { fontSize: 8 } };
socialChart.yAxis = { numberFormatCode: "#,##0" };
socialChart.setPosition("A38", "E54");

const cardChart = sheet.charts.add("bar", sheet.getRange("G21:H35"));
cardChart.title = "Card Gift/Trade Capacity by Rank";
cardChart.hasLegend = false;
cardChart.xAxis = { axisType: "textAxis", textStyle: { fontSize: 8 } };
cardChart.yAxis = { numberFormatCode: "#,##0" };
cardChart.setPosition("F38", "J54");

sheet.getRange("A1:N54").format.font.name = "Aptos";
sheet.getRange("A1:N54").format.autofitColumns();
sheet.getRange("A1:N54").format.autofitRows();
sheet.getRange("A:A").format.columnWidth = 13;
sheet.getRange("B:B").format.columnWidth = 21;
sheet.getRange("C:C").format.columnWidth = 15;
sheet.getRange("D:E").format.columnWidth = 18;
sheet.getRange("F:G").format.columnWidth = 19;
sheet.getRange("H:H").format.columnWidth = 18;
sheet.getRange("I:I").format.columnWidth = 28;
sheet.getRange("K:M").format.columnWidth = 24;
sheet.getRange("N:N").format.columnWidth = 30;
sheet.getRange("A4:I4").format.rowHeight = 38;
sheet.getRange("K5:N11").format.rowHeight = 34;
sheet.getRange("K13:N23").format.rowHeight = 28;
sheet.freezePanes.freezeRows(4);

const table = sheet.tables.add("A4:I18", true, "RankCapabilitiesTable");
table.style = "TableStyleMedium4";

const inspect = await workbook.inspect({
  kind: "table",
  sheetId: "Rank Capabilities",
  range: "A4:I18",
  include: "values,formulas",
  tableMaxRows: 20,
  tableMaxCols: 10,
  maxChars: 8000,
});
console.log(inspect.ndjson);

const errors = await workbook.inspect({
  kind: "match",
  searchTerm: "#REF!|#DIV/0!|#VALUE!|#NAME\\?|#N/A",
  options: { useRegex: true, maxResults: 300 },
  summary: "final formula error scan",
});
console.log(errors.ndjson);

for (const renderSheetName of ["Sheet1", "Rank Capabilities"]) {
  const preview = await workbook.render({
    sheetName: renderSheetName,
    autoCrop: "all",
    scale: 1,
    format: "png",
  });
  await fs.writeFile(`${outputDir}/${renderSheetName.replaceAll(" ", "_")}.png`, new Uint8Array(await preview.arrayBuffer()));
}

const output = await SpreadsheetFile.exportXlsx(workbook);
await output.save(outputPath);
console.log(outputPath);
