using System;
using System.Text;

namespace BingoMagicMayhem.Infrastructure
{
    public sealed class DisplayNameValidationResult
    {
        public bool IsValid;
        public string NormalizedName = "";
        public string Message = "";
    }

    /// <summary>
    /// Conservative local Beta/test validation only. Final naming, moderation,
    /// localization, uniqueness, and reserved-word policy require backend approval.
    /// </summary>
    public static class ProfileDisplayNameValidator
    {
        public const int BetaMinimumLength = 3;
        public const int BetaMaximumLength = 24;

        public static DisplayNameValidationResult ValidateBeta(string candidate)
        {
            string normalized = CollapseWhitespace(candidate);
            if (normalized.Length < BetaMinimumLength || normalized.Length > BetaMaximumLength)
            {
                return Invalid($"Use {BetaMinimumLength}-{BetaMaximumLength} characters for this Beta placeholder.");
            }

            foreach (char character in normalized)
            {
                if (!char.IsLetterOrDigit(character) && character != ' ' && character != '-' && character != '\'')
                {
                    return Invalid("Use letters, numbers, spaces, apostrophes, or hyphens for this Beta placeholder.");
                }
            }

            return new DisplayNameValidationResult
            {
                IsValid = true,
                NormalizedName = normalized,
                Message = "Saved locally. Production moderation and uniqueness are not active."
            };
        }

        private static string CollapseWhitespace(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }

            StringBuilder builder = new StringBuilder(value.Length);
            bool previousWasSpace = false;
            foreach (char character in value.Trim())
            {
                if (char.IsWhiteSpace(character))
                {
                    if (!previousWasSpace)
                    {
                        builder.Append(' ');
                        previousWasSpace = true;
                    }
                }
                else
                {
                    builder.Append(character);
                    previousWasSpace = false;
                }
            }

            return builder.ToString();
        }

        private static DisplayNameValidationResult Invalid(string message)
        {
            return new DisplayNameValidationResult { Message = message };
        }
    }
}
