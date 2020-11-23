﻿using osu.Framework.Graphics;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;
using System;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Colour;
using osu.Framework.Allocation;
using System.Collections.Generic;
using osu.Framework.Graphics.Batches;
using osu.Framework.Graphics.OpenGL.Vertices;
using osu.Framework.Lists;
using Piously.Game.Graphics.Primitives;

namespace Piously.Game.Graphics.Backgrounds
{
    public class Hexagons : Drawable
    {
        private const float hexagon_size = 100;
        private const float base_velocity = 50;

        /// <summary>
        /// How many screen-space pixels are smoothed over.
        /// Same behavior as Sprite's EdgeSmoothness.
        /// </summary>
        private const float edge_smoothness = 1;

        private Color4 colourLight = Color4.White;

        public Color4 ColourLight
        {
            get => colourLight;
            set
            {
                if (colourLight == value) return;

                colourLight = value;
                updateColours();
            }
        }

        private Color4 colourDark = Color4.Black;

        public Color4 ColourDark
        {
            get => colourDark;
            set
            {
                if (colourDark == value) return;

                colourDark = value;
                updateColours();
            }
        }

        /// <summary>
        /// Whether we want to expire hexagons as they exit our draw area completely.
        /// </summary>
        protected virtual bool ExpireOffScreenHexagons => true;

        /// <summary>
        /// Whether we should create new hexagons as others expire.
        /// </summary>
        protected virtual bool CreateNewHexagons => true;

        /// <summary>
        /// The amount of hexagons we want compared to the default distribution.
        /// </summary>
        protected virtual float SpawnRatio => 1;

        private float hexagonScale = 1;

        /// <summary>
        /// Whether we should drop-off alpha values of hexagons more quickly to improve
        /// the visual appearance of fading. This defaults to on as it is generally more
        /// aesthetically pleasing, but should be turned off in buffered containers.
        /// </summary>
        public bool HideAlphaDiscrepancies = true;

        /// <summary>
        /// The relative velocity of the hexagons. Default is 1.
        /// </summary>
        public float Velocity = 1;

        private readonly Random stableRandom;

        private float nextRandom() => (float)(stableRandom?.NextDouble() ?? RNG.NextSingle());

        private readonly SortedList<HexagonParticle> parts = new SortedList<HexagonParticle>(Comparer<HexagonParticle>.Default);

        private IShader shader;
        private readonly Texture texture;

        /// <summary>
        /// Construct a new hexagon visualisation.
        /// </summary>
        /// <param name="seed">An optional seed to stabilise random positions / attributes. Note that this does not guarantee stable playback when seeking in time.</param>
        public Hexagons(int? seed = null)
        {
            if (seed != null)
                stableRandom = new Random(seed.Value);

            texture = Texture.WhitePixel;
        }

        [BackgroundDependencyLoader]
        private void load(ShaderManager shaders)
        {
            shader = shaders.Load(VertexShaderDescriptor.TEXTURE_2, FragmentShaderDescriptor.TEXTURE_ROUNDED);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            addHexagons(true);
        }

        public float HexagonScale
        {
            get => hexagonScale;
            set
            {
                float change = value / hexagonScale;
                hexagonScale = value;

                for (int i = 0; i < parts.Count; i++)
                {
                    HexagonParticle newParticle = parts[i];
                    newParticle.Scale *= change;
                    parts[i] = newParticle;
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            Invalidate(Invalidation.DrawNode);

            if (CreateNewHexagons)
                addHexagons(false);

            float adjustedAlpha = HideAlphaDiscrepancies
                // Cubically scale alpha to make it drop off more sharply.
                ? MathF.Pow(DrawColourInfo.Colour.AverageColour.Linear.A, 3)
                : 1;

            float elapsedSeconds = (float)Time.Elapsed / 1000;
            // Since position is relative, the velocity needs to scale inversely with DrawHeight.
            // Since we will later multiply by the scale of individual hexagons we normalize by
            // dividing by hexagonScale.
            float movedDistance = -elapsedSeconds * Velocity * base_velocity / (DrawHeight * hexagonScale);

            for (int i = 0; i < parts.Count; i++)
            {
                HexagonParticle newParticle = parts[i];

                // Scale moved distance by the size of the hexagon. Smaller hexagons should move more slowly.
                newParticle.Position.Y += parts[i].Scale * movedDistance;
                newParticle.Colour.A = adjustedAlpha;

                parts[i] = newParticle;

                float bottomPos = parts[i].Position.Y + hexagon_size * parts[i].Scale * 0.866f / DrawHeight;
                if (bottomPos < 0)
                    parts.RemoveAt(i);
            }
        }

        protected int AimCount;

        private void addHexagons(bool randomY)
        {
            AimCount = (int)(DrawWidth * DrawHeight * 0.002f / (hexagonScale * hexagonScale) * SpawnRatio);

            for (int i = 0; i < AimCount - parts.Count; i++)
                parts.Add(createHexagon(randomY));
        }

        private HexagonParticle createHexagon(bool randomY)
        {
            HexagonParticle particle = CreateHexagon();

            particle.Position = new Vector2(nextRandom(), randomY ? nextRandom() : 1);
            particle.ColourShade = nextRandom();
            particle.Colour = CreateHexagonShade(particle.ColourShade);

            return particle;
        }

        /// <summary>
        /// Creates a hexagon particle with a random scale.
        /// </summary>
        /// <returns>The hexagon particle.</returns>
        protected virtual HexagonParticle CreateHexagon()
        {
            const float std_dev = 0.16f;
            const float mean = 0.5f;

            float u1 = 1 - nextRandom(); //uniform(0,1] random floats
            float u2 = 1 - nextRandom();
            float randStdNormal = (float)(Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2)); // random normal(0,1)
            var scale = Math.Max(hexagonScale * (mean + std_dev * randStdNormal), 0.1f); // random normal(mean,stdDev^2)

            return new HexagonParticle { Scale = scale };
        }

        /// <summary>
        /// Creates a shade of colour for the hexagons.
        /// </summary>
        /// <returns>The colour.</returns>
        protected virtual Color4 CreateHexagonShade(float shade) => Interpolation.ValueAt(shade, colourDark, colourLight, 0, 1);

        private void updateColours()
        {
            for (int i = 0; i < parts.Count; i++)
            {
                HexagonParticle newParticle = parts[i];
                newParticle.Colour = CreateHexagonShade(newParticle.ColourShade);
                parts[i] = newParticle;
            }
        }

        protected override DrawNode CreateDrawNode() => new HexagonsDrawNode(this);

        private class HexagonsDrawNode : DrawNode
        {
            protected new Hexagons Source => (Hexagons)base.Source;

            private IShader shader;
            private Texture texture;

            private readonly List<HexagonParticle> parts = new List<HexagonParticle>();
            private Vector2 size;

            private QuadBatch<TexturedVertex2D> vertexBatch;

            public HexagonsDrawNode(Hexagons source)
                : base(source)
            {
            }

            public override void ApplyState()
            {
                base.ApplyState();

                shader = Source.shader;
                texture = Source.texture;
                size = Source.DrawSize;

                parts.Clear();
                parts.AddRange(Source.parts);
            }

            public override void Draw(Action<TexturedVertex2D> vertexAction)
            {
                base.Draw(vertexAction);

                if (Source.AimCount > 0 && (vertexBatch == null || vertexBatch.Size != Source.AimCount))
                {
                    vertexBatch?.Dispose();
                    vertexBatch = new QuadBatch<TexturedVertex2D>(Source.AimCount, 1);
                }

                shader.Bind();

                Vector2 localInflationAmount = edge_smoothness * DrawInfo.MatrixInverse.ExtractScale().Xy;

                foreach (HexagonParticle particle in parts)
                {
                    var offset = hexagon_size * new Vector2(particle.Scale * 0.5f, 0);

                    var hexagon = new Hexagon(
                        Vector2Extensions.Transform(particle.Position * size + offset, DrawInfo.Matrix),
                        Vector2Extensions.Transform(particle.Position * size + new Vector2(-offset.X, offset.Y), DrawInfo.Matrix)
                    ); ;

                    ColourInfo colourInfo = DrawColourInfo.Colour;
                    colourInfo.ApplyChild(particle.Colour);

                    /*DrawTriangle(
                        texture,
                        hexagon,
                        colourInfo,
                        null,
                        vertexBatch.AddAction,
                        Vector2.Divide(localInflationAmount, new Vector2(2 * offset.X, offset.Y)));
                    */

                    DrawTriangle(texture, hexagon.farUpTriangle, colourInfo, null, vertexBatch.AddAction,
                        Vector2.Divide(localInflationAmount, new Vector2(2 * offset.X, offset.Y)));
                    DrawTriangle(texture, hexagon.nearUpTriangle, colourInfo, null, vertexBatch.AddAction,
                        Vector2.Divide(localInflationAmount, new Vector2(2 * offset.X, offset.Y)));
                    DrawTriangle(texture, hexagon.nearDownTriangle, colourInfo, null, vertexBatch.AddAction,
                        Vector2.Divide(localInflationAmount, new Vector2(2 * offset.X, offset.Y)));
                    DrawTriangle(texture, hexagon.farDownTriangle, colourInfo, null, vertexBatch.AddAction,
                        Vector2.Divide(localInflationAmount, new Vector2(2 * offset.X, offset.Y)));
                }

                shader.Unbind();
            }

            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);

                vertexBatch?.Dispose();
            }
        }

        protected struct HexagonParticle : IComparable<HexagonParticle>
        {
            /// <summary>
            /// The position of the top vertex of the hexagon.
            /// </summary>
            public Vector2 Position;

            /// <summary>
            /// The colour shade of the hexagon.
            /// This is needed for colour recalculation of visible hexagons when <see cref="ColourDark"/> or <see cref="ColourLight"/> is changed.
            /// </summary>
            public float ColourShade;

            /// <summary>
            /// The colour of the hexagon.
            /// </summary>
            public Color4 Colour;

            /// <summary>
            /// The scale of the hexagon.
            /// </summary>
            public float Scale;

            /// <summary>
            /// Compares two <see cref="HexagonParticle"/>s. This is a reverse comparer because when the
            /// hexagons are added to the particles list, they should be drawn from largest to smallest
            /// such that the smaller hexagons appear on top.
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public int CompareTo(HexagonParticle other) => other.Scale.CompareTo(Scale);
        }
    }
}
