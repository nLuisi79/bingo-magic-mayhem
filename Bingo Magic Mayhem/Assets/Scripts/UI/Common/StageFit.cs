using UnityEngine;

namespace BingoMagicMayhem.UI.Common
{
    // Pure math for fitting the fixed-size stage (1600x900 by default) onto any screen inside the device safe area.
    // BingoPrototype.ApplyStageScale uses this at runtime; StageFitTests checks it against common phone/tablet sizes.
    // See 07_UI_VISUAL_DIRECTION.md for the layout rules.
    public static class StageFit
    {
        public const float MinScale = 0.55f;
        public const float MaxScale = 1.15f;

        // Wide "bleed" background envelope in stage units: the 1600x900 composition sits unchanged in the centre and the
        // scene extends 300 left/right and 225 top/bottom. Covers 21:9 phones (incl. side camera cutouts) and 4:3 /
        // near-square tablets and foldables. Delivered at 1.2 px per unit = 2640x1620 px (Docs/REALM_HOME_WIDE_ART_SPEC.md).
        public static readonly Vector2 WideArtSize = new Vector2(2200f, 1350f);
        public const string WideArtResourceSuffix = "-wide";

        // Visible screen area, in stage units, around a stage fitted with the given result.
        public static Vector2 VisibleStageUnits(Vector2 rootSize, Result fit)
        {
            return rootSize / Mathf.Max(0.0001f, fit.Scale);
        }

        public readonly struct Insets
        {
            public Insets(float left, float right, float bottom, float top)
            {
                Left = left;
                Right = right;
                Bottom = bottom;
                Top = top;
            }

            public float Left { get; }
            public float Right { get; }
            public float Bottom { get; }
            public float Top { get; }
        }

        public readonly struct Result
        {
            public Result(float scale, Vector2 offset, Insets insets)
            {
                Scale = scale;
                Offset = offset;
                Insets = insets;
            }

            public float Scale { get; }
            public Vector2 Offset { get; }
            public Insets Insets { get; }
        }

        // Canvas size produced by a ScaleWithScreenSize CanvasScaler in MatchWidthOrHeight mode.
        public static Vector2 CanvasSizeFor(Vector2 screenSize, Vector2 referenceResolution, float matchWidthOrHeight)
        {
            float logWidth = Mathf.Log(screenSize.x / referenceResolution.x, 2f);
            float logHeight = Mathf.Log(screenSize.y / referenceResolution.y, 2f);
            float scaleFactor = Mathf.Pow(2f, Mathf.Lerp(logWidth, logHeight, matchWidthOrHeight));
            return screenSize / scaleFactor;
        }

        // Converts the screen-pixel safe area into canvas-unit insets on each edge.
        public static Insets GetSafeAreaInsets(Vector2 rootSize, Vector2 screenSize, Rect safeArea)
        {
            if (screenSize.x <= 0f || screenSize.y <= 0f || safeArea.width <= 0f || safeArea.height <= 0f)
            {
                return new Insets(0f, 0f, 0f, 0f);
            }

            float toCanvasX = rootSize.x / screenSize.x;
            float toCanvasY = rootSize.y / screenSize.y;
            return new Insets(
                Mathf.Max(0f, safeArea.xMin) * toCanvasX,
                Mathf.Max(0f, screenSize.x - safeArea.xMax) * toCanvasX,
                Mathf.Max(0f, safeArea.yMin) * toCanvasY,
                Mathf.Max(0f, screenSize.y - safeArea.yMax) * toCanvasY);
        }

        // Full-bleed stages (realm home and others that fill the viewport) skip the padding and the upper size limit.
        public static Result Compute(Vector2 rootSize, Vector2 screenSize, Rect safeArea, Vector2 stageSize, float safePadding, bool fillsViewport)
        {
            Insets insets = GetSafeAreaInsets(rootSize, screenSize, safeArea);
            float padding = fillsViewport ? 0f : safePadding;
            float availableWidth = Mathf.Max(1f, rootSize.x - insets.Left - insets.Right - padding * 2f);
            float availableHeight = Mathf.Max(1f, rootSize.y - insets.Bottom - insets.Top - padding * 2f);
            float scale = Mathf.Min(availableWidth / stageSize.x, availableHeight / stageSize.y);
            scale = fillsViewport ? Mathf.Max(MinScale, scale) : Mathf.Clamp(scale, MinScale, MaxScale);
            Vector2 offset = new Vector2((insets.Left - insets.Right) * 0.5f, (insets.Bottom - insets.Top) * 0.5f);
            return new Result(scale, offset, insets);
        }
    }
}
