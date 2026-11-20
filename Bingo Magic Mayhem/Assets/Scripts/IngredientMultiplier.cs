// Card potion (ingredient) multiplier from the mana bet, per the locked rule in LOCKED_DECISIONS.md:
// 1x by default, 2x past 50% of the room's max bet, 3x past 75%, 4x at the max bet. Thresholds come from
// EconomyTuning.IngredientMultiplier so they can be tuned without code changes.
public static class IngredientMultiplier
{
    public static int ForBet(int manaBetPerCard, int maxManaBet)
    {
        return ForBet(manaBetPerCard, maxManaBet, EconomyTuning.Current.IngredientMultiplier);
    }

    public static int ForBet(int manaBetPerCard, int maxManaBet, EconomyTuning.IngredientMultiplierSettings settings)
    {
        if (maxManaBet <= 0 || manaBetPerCard <= 0)
        {
            return 1;
        }

        if (manaBetPerCard >= maxManaBet)
        {
            return settings.MaxMultiplier;
        }

        float fraction = (float)manaBetPerCard / maxManaBet;
        if (fraction > settings.TripleAboveBetFraction)
        {
            return 3;
        }

        return fraction > settings.DoubleAboveBetFraction ? 2 : 1;
    }
}
