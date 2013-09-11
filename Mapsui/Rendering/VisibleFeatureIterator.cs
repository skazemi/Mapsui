﻿using System;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using System.Collections.Generic;
using System.Linq;

namespace Mapsui.Rendering
{
    public class VisibleFeatureIterator
    {
        public static void Render(IViewport viewport, IEnumerable<ILayer> layers,
            Action<IViewport, IStyle, IFeature> renderFeature)
        {
            foreach (var layer in layers)
            {
                if (layer.Enabled &&
                    layer.MinVisible <= viewport.Resolution &&
                    layer.MaxVisible >= viewport.Resolution)
                {
                    RenderLayer(viewport, layer, renderFeature);
                }
            }
        }

        private static void RenderLayer(IViewport viewport, ILayer layer,
            Action<IViewport, IStyle, IFeature> renderFeature)
        {
            if (layer.Enabled == false) return;

            if (layer is LabelLayer)
            {
                var labelLayer = layer as LabelLayer;
                //!!!labelLayer.UseLabelStacking ? LabelRenderer.RenderStackedLabelLayer(viewport, labelLayer) : LabelRenderer.RenderLabelLayer(viewport, labelLayer));
            }
            else
            {
                RenderVectorLayer(viewport, layer, renderFeature);
            }
        }

        private static void RenderVectorLayer(IViewport viewport, ILayer layer,
            Action<IViewport, IStyle, IFeature> renderFeature)
        {
            var features = layer.GetFeaturesInView(viewport.Extent, viewport.Resolution).ToList();

            foreach (var layerStyle in layer.Styles)
            {
                var style = layerStyle; // This is the default that could be overridden by an IThemeStyle

                foreach (var feature in features)
                {
                    if (layerStyle is IThemeStyle) style = (layerStyle as IThemeStyle).GetStyle(feature);
                    if ((style == null) || (style.Enabled == false) || (style.MinVisible > viewport.Resolution) || (style.MaxVisible < viewport.Resolution)) continue;

                    renderFeature(viewport, style, feature);
                }
            }

            foreach (var feature in features)
            {
                var styles = feature.Styles ?? Enumerable.Empty<IStyle>();
                foreach (var style in styles)
                {
                    if (feature.Styles != null && style.Enabled)
                    {
                        renderFeature(viewport, style, feature);
                    }
                }
            }
        }
    }
}
