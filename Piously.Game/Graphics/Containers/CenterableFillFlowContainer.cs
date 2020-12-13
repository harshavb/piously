﻿using System;
using System.Collections.Generic;
using osuTK;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;

namespace Piously.Game.Graphics.Containers
{
    public class CenterableFillFlowContainer : CenterableFillFlowContainer<Drawable>
    {
    }

    public class CenterableFillFlowContainer<T> : FlowContainer<T>, IFillFlowContainer where T : Drawable
    {
        private FillDirection direction = FillDirection.Full;

        public FillDirection Direction
        {
            get => direction;
            set
            {
                if (direction == value)
                    return;

                direction = value;
                InvalidateLayout();
            }
        }

        private Vector2 spacing;

        public Vector2 Spacing
        {
            get => spacing;
            set
            {
                if (spacing == value)
                    return;

                spacing = value;
                InvalidateLayout();
            }
        }

        private Vector2 spacingFactor(Drawable c)
        {
            Vector2 result = c.RelativeOriginPosition;
            if (c.Anchor.HasFlag(Anchor.x2))
                result.X = 1 - result.X;
            if (c.Anchor.HasFlag(Anchor.y2))
                result.Y = 1 - result.Y;
            return result;
        }

        protected override IEnumerable<Vector2> ComputeLayoutPositions()
        {
            var max = MaximumSize;

            if (max == Vector2.Zero)
            {
                var s = ChildSize;

                // If we are autosize and haven't specified a maximum size, we should allow infinite expansion.
                // If we are inheriting then we need to use the parent size (our ActualSize).
                max.X = AutoSizeAxes.HasFlag(Axes.X) ? float.MaxValue : s.X;
                max.Y = AutoSizeAxes.HasFlag(Axes.Y) ? float.MaxValue : s.Y;
            }

            var children = FlowingChildren.ToArray();
            if (children.Length == 0)
                return new List<Vector2>();

            // The positions for each child we will return later on.
            Vector2[] result = new Vector2[children.Length];

            // We need to keep track of row widths such that we can compute correct
            // positions for horizontal centre anchor children.
            // We also store for each child to which row it belongs.
            int[] rowIndices = new int[children.Length];
            List<float> rowOffsetsToMiddle = new List<float> { 0 };

            // Variables keeping track of the current state while iterating over children
            // and computing initial flow positions.
            float rowHeight = 0;
            float rowBeginOffset = 0;
            var current = Vector2.Zero;

            // First pass, computing initial flow positions
            Vector2 size = Vector2.Zero;

            //CENTERED FILL FLOW CONTAINER COMPUTATION -> BAD IMPLEMENTATION SHOULD BE FIXED DOWN THE LINE
            if (direction == FillDirection.Centered)
            {
                float lengthOfAllChildren = children.Sum(child => child.BoundingBox.X);

                float startPoint = size.X / 2;

                if (children.Length % 2 == 0)
                {
                    startPoint += spacing.X / 2;
                    for(int i = 0; i < children.Length/2; ++i)
                    {
                        startPoint -= children[i].BoundingBox.X;
                        startPoint -= spacing.X;
                    }
                }
                else
                {
                    startPoint -= children[children.Length / 2].BoundingBox.X / 2;
                    for (int i = 0; i < children.Length / 2; ++i)
                    {
                        startPoint -= spacing.X;
                        startPoint -= children[i].BoundingBox.X;
                    }
                }

                for(int i = 0; i < children.Length; ++i)
                {
                    result[i] = new Vector2(startPoint, size.Y / 2);
                    startPoint += children[i].BoundingBox.X + spacing.X;
                }

                return result;
            }

            for (int i = 0; i < children.Length; ++i)
            {
                Drawable c = children[i];

                static Axes toAxes(FillDirection direction)
                {
                    switch (direction)
                    {
                        case FillDirection.Full:
                            return Axes.Both;

                        case FillDirection.Horizontal:
                            return Axes.X;

                        case FillDirection.Vertical:
                            return Axes.Y;

                        case FillDirection.Centered:
                            return Axes.X;

                        default:
                            throw new ArgumentException($"{direction.ToString()} is not defined");
                    }
                }

                // In some cases (see the right hand side of the conditional) we want to permit relatively sized children
                // in our fill direction; specifically, when children use FillMode.Fit to preserve the aspect ratio.
                // Consider the following use case: A fill flow container has a fixed width but an automatic height, and fills
                // in the vertical direction. Now, we can add relatively sized children with FillMode.Fit to make sure their
                // aspect ratio is preserved while still allowing them to fill vertically. This special case can not result
                // in an autosize-related feedback loop, and we can thus simply allow it.
                if ((c.RelativeSizeAxes & AutoSizeAxes & toAxes(Direction)) != 0 && (c.FillMode != FillMode.Fit || c.RelativeSizeAxes != Axes.Both || c.Size.X > RelativeChildSize.X || c.Size.Y > RelativeChildSize.Y || AutoSizeAxes == Axes.Both))
                {
                    throw new InvalidOperationException(
                        "Drawables inside a fill flow container may not have a relative size axis that the fill flow container is filling in and auto sizing for. " +
                        $"The fill flow container is set to flow in the {Direction} direction and autosize in {AutoSizeAxes} axes and the child is set to relative size in {c.RelativeSizeAxes} axes.");
                }

                // Populate running variables with sane initial values.
                if (i == 0)
                {
                    size = c.BoundingBox.Size;
                    rowBeginOffset = spacingFactor(c).X * size.X;
                }

                float rowWidth = rowBeginOffset + current.X + (1 - spacingFactor(c).X) * size.X;

                //We've exceeded our allowed width, move to a new row
                if ((direction != FillDirection.Horizontal) && (Precision.DefinitelyBigger(rowWidth, max.X) || direction == FillDirection.Vertical || ForceNewRow(c)))
                {
                    current.X = 0;
                    current.Y += rowHeight;

                    result[i] = current;

                    rowOffsetsToMiddle.Add(0);
                    rowBeginOffset = spacingFactor(c).X * size.X;

                    rowHeight = 0;
                }
                else
                {
                    result[i] = current;

                    // Compute offset to the middle of the row, to be applied in case of centre anchor
                    // in a second pass.
                    rowOffsetsToMiddle[^1] = rowBeginOffset - rowWidth / 2;
                }

                rowIndices[i] = rowOffsetsToMiddle.Count - 1;

                Vector2 stride = Vector2.Zero;

                if (i < children.Length - 1)
                {
                    // Compute stride. Note, that the stride depends on the origins of the drawables
                    // on both sides of the step to be taken.
                    stride = (Vector2.One - spacingFactor(c)) * size;

                    c = children[i + 1];
                    size = c.BoundingBox.Size;

                    stride += spacingFactor(c) * size;
                }

                stride += Spacing;

                if (stride.Y > rowHeight)
                    rowHeight = stride.Y;
                current.X += stride.X;
            }

            float height = result.Last().Y;

            Vector2 ourRelativeAnchor = children[0].RelativeAnchorPosition;

            // Second pass, adjusting the positions for anchors of children.
            // Uses rowWidths and height for centre-anchors.
            for (int i = 0; i < children.Length; ++i)
            {
                var c = children[i];

                switch (Direction)
                {
                    case FillDirection.Vertical:
                        if (c.RelativeAnchorPosition.Y != ourRelativeAnchor.Y)
                        {
                            throw new InvalidOperationException(
                                $"All drawables in a {nameof(FillFlowContainer)} must use the same RelativeAnchorPosition for the given {nameof(FillDirection)}({Direction}) ({ourRelativeAnchor.Y} != {c.RelativeAnchorPosition.Y}). "
                                + $"Consider using multiple instances of {nameof(FillFlowContainer)} if this is intentional.");
                        }

                        break;

                    case FillDirection.Horizontal:
                        if (c.RelativeAnchorPosition.X != ourRelativeAnchor.X)
                        {
                            throw new InvalidOperationException(
                                $"All drawables in a {nameof(FillFlowContainer)} must use the same RelativeAnchorPosition for the given {nameof(FillDirection)}({Direction}) ({ourRelativeAnchor.X} != {c.RelativeAnchorPosition.X}). "
                                + $"Consider using multiple instances of {nameof(FillFlowContainer)} if this is intentional.");
                        }

                        break;

                    default:
                        if (c.RelativeAnchorPosition != ourRelativeAnchor)
                        {
                            throw new InvalidOperationException(
                                $"All drawables in a {nameof(FillFlowContainer)} must use the same RelativeAnchorPosition for the given {nameof(FillDirection)}({Direction}) ({ourRelativeAnchor} != {c.RelativeAnchorPosition}). "
                                + $"Consider using multiple instances of {nameof(FillFlowContainer)} if this is intentional.");
                        }

                        break;
                }

                if (c.Anchor.HasFlag(Anchor.x1))
                    // Begin flow at centre of row
                    result[i].X += rowOffsetsToMiddle[rowIndices[i]];
                else if (c.Anchor.HasFlag(Anchor.x2))
                    // Flow right-to-left
                    result[i].X = -result[i].X;

                if (c.Anchor.HasFlag(Anchor.y1))
                    // Begin flow at centre of total height
                    result[i].Y -= height / 2;
                else if (c.Anchor.HasFlag(Anchor.y2))
                    // Flow bottom-to-top
                    result[i].Y = -result[i].Y;
            }

            return result;
        }

        protected virtual bool ForceNewRow(Drawable child) => false;
    }

    public enum FillDirection
    {
        Full,
        Horizontal,
        Vertical,
        Centered
    }
}