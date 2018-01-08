/* Copyright (C) 2008-2009  Gilleain Torrance <gilleain@users.sf.net>
 *               2008-2009  Arvid Berg <goglepox@users.sf.net>
 *                    2009  Stefan Kuhn <shk3@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *  */
using NCDK.Events;
using NCDK.Renderers.Generators;
using NCDK.Renderers.Generators.Parameters;
using NCDK.Renderers.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using WPF = System.Windows;

namespace NCDK.Renderers
{
    /// <summary>
    /// Model for <see cref="IRenderer{T}"/> that contains settings for drawing objects.
    /// </summary>
    // @author maclean
    // @cdk.module render
    // @cdk.githash
    [Serializable]
    public class RendererModel
    {
        [NonSerialized]
        private HashSet<ICDKChangeListener> listeners = null;
        private IDictionary<IAtom, string> toolTipTextMap = new Dictionary<IAtom, string>();
        private IAtom highlightedAtom = null;
        private IBond highlightedBond = null;
        private IAtomContainer externalSelectedPart = null;
        private IAtomContainer clipboardContent = null;
        private IChemObjectSelection selection;
        private IDictionary<IAtom, IAtom> merge = new Dictionary<IAtom, IAtom>();

        /// <summary>
        /// Color of a selection.
        /// </summary>
        public class SelectionColor : AbstractGeneratorParameter<Color?>
        {
            public override Color? Default => Color.FromRgb(0x49, 0xdf, 0xff);
        }

        /// <summary>
        /// The color used to highlight external selections.
        /// </summary>
        public class ExternalHighlightColor : AbstractGeneratorParameter<Color?>
        {
            public override Color? Default => WPF.Media.Colors.Gray;
        }

        private IGeneratorParameter<Color?> externalHighlightColor = new ExternalHighlightColor();

        /// <summary>
        /// Padding between molecules in a grid or row.
        /// </summary>
        public class Padding : AbstractGeneratorParameter<double?>
        {
            public override double? Default => 16;
        }

        /// <summary>
        /// The color hash is used to color substructures.
        /// </summary>
        public class ColorHash : AbstractGeneratorParameter<IDictionary<IChemObject, Color>>
        {
            public override IDictionary<IChemObject, Color> Default => new Dictionary<IChemObject, Color>();
        }

        private IGeneratorParameter<IDictionary<IChemObject, Color>> colorHash = new ColorHash();

        /// <summary>
        /// Size of title font relative compared to atom symbols
        /// </summary>
        public class TitleFontScale : AbstractGeneratorParameter<double?>
        {
            public override double? Default => 0.8d;
        }

        /// <summary>
        /// Color of title text.
        /// </summary>
        public class TitleColor : AbstractGeneratorParameter<Color?>
        {
            public override Color? Default => WPF.Media.Colors.Red;
        }

        /// <summary>
        /// If format supports it (e.g. SVG) should marked up elements (id and classes) be output.
        /// </summary>
        public class MarkedOutput : AbstractGeneratorParameter<bool?>
        {
            public override bool? Default => true;
        }

        /// <summary>
        /// A map of <see cref="IGeneratorParameter"/> class names to instances.
        /// </summary>
        private IDictionary<string, IGeneratorParameter> renderingParameters = new Dictionary<string, IGeneratorParameter>();

        /// <summary>
        /// Construct a renderer model with no parameters. To put parameters into
        /// the model, use the registerParameters method.
        /// </summary>
        public RendererModel()
        {
            renderingParameters[colorHash.GetType().FullName] = colorHash;
            renderingParameters[externalHighlightColor.GetType().FullName] = externalHighlightColor;
            renderingParameters[typeof(SelectionColor).FullName] = new SelectionColor();
            renderingParameters[typeof(Padding).FullName] = new Padding();
            renderingParameters[typeof(TitleFontScale).FullName] = new TitleFontScale();
            renderingParameters[typeof(TitleColor).FullName] = new TitleColor();
            renderingParameters[typeof(MarkedOutput).FullName] = new MarkedOutput();
        }

        /// <summary>
        /// Returns all <see cref="IGeneratorParameter"/>s for the current <see cref="RendererModel"/>.
        /// </summary>
        /// <returns>a new List with <see cref="IGeneratorParameter"/>s</returns>
        public List<IGeneratorParameter> GetRenderingParameters()
        {
            List<IGeneratorParameter> parameters = new List<IGeneratorParameter>();
            parameters.AddRange(renderingParameters.Values);
            return parameters;
        }

        /// <summary>
        /// Returns the <see cref="IGeneratorParameter"/> for the active <see cref="IRenderer{T}"/>.
        /// It returns a new instance of it was unregistered.
        /// </summary>
        /// <param name="param"><see cref="IGeneratorParameter"/> to get the value of.</param>
        /// <returns>the <see cref="IGeneratorParameter"/> instance with the active value.</returns>
        /// <typeparam name="T"></typeparam>
        public IGeneratorParameter<T> GetParameter<T>(Type param) 
        {
#if DEBUG
            var f = typeof(IGeneratorParameter<T>).IsAssignableFrom(param);
            if (!f)
                throw new InvalidOperationException();
#endif
            return (IGeneratorParameter<T>)GetParameter(param);
        }

        public IGeneratorParameter GetParameter(Type param)
        {
            if (renderingParameters.TryGetValue(param.FullName, out IGeneratorParameter ret))
                return ret;

            // the parameter was not registered yet, so we throw an exception to
            // indicate that the API is not used correctly.
            throw new TypeAccessException($"You requested the active parameter of type {param.FullName}, but it has not been registered yet. Did you make sure the IGeneratorParameter is registered, by registering the appropriate IGenerator? Alternatively, you can use Default to query the default value for any parameter on the classpath.");
        }

        /// <summary>
        /// Returns the default value for the <see cref="IGeneratorParameter"/> for the
        /// active <see cref="IRenderer{T}"/>.
        /// </summary>
        /// <typeparam name="T"><see cref="IGeneratorParameter"/> to get the value of.</typeparam>
        /// <returns>the default value for which the type is defined by the provided <see cref="IGeneratorParameter"/>-typed <code>param</code> parameter.</returns>
        /// <seealso cref="Get{T}"/>
        public T GetDefault<T>(Type param)
        {
#if DEBUG
            var f = typeof(IGeneratorParameter<T>).IsAssignableFrom(param);
            if (!f)
                throw new InvalidOperationException();
#endif

            if (renderingParameters.TryGetValue(param.FullName, out IGeneratorParameter ret))
            {
                return ((IGeneratorParameter<T>)ret).Default;
            }

            // OK, this parameter is not registered, but that's fine, as we are
            // only to return the default value anyway...
            return ((IGeneratorParameter<T>)param.GetConstructor(Type.EmptyTypes).Invoke(Array.Empty<object>())).Default;
        }

        public T GetDefaultV<T>(Type param) where T : struct
        {
            return GetDefault<T?>(param).Value;
        }

        /// <summary>
        /// The <see cref="IGeneratorParameter"/> for the active <see cref="IRenderer{T}"/>.
        /// </summary>
        /// <param name="paramType"><see cref="IGeneratorParameter"/> to set the value of.</param>
        /// <param name="value">new <see cref="IGeneratorParameter"/> value </param>
        public void Set<T>(Type paramType,  T value) 
        {
#if DEBUG
            {
                var f = typeof(IGeneratorParameter<T>).IsAssignableFrom(paramType);
                if (!f)
                    throw new InvalidOperationException();
            }
#endif
            var parameter = GetParameter<T>(paramType);
            if (!parameter.Value.Equals(value))
            {
                parameter.Value = value;
                FireChange();
            }
        }

        public void Set(Type paramType, object value)
        {
            var parameter = GetParameter(paramType);
            paramType.GetProperty(nameof(IGeneratorParameter<object>.Value)).SetValue(parameter, value);

            FireChange();
        }

        public void SetV<T>(Type paramType, T value) where T : struct
        {
            Set<T?>(paramType, value);
        }
        
        /// <summary>
        /// Returns the <see cref="IGeneratorParameter"/> for the active <see cref="IRenderer{T}"/>.
        /// </summary>
        /// <param name="paramType"><see cref="IGeneratorParameter"/> to get the value of.</param>
        /// <returns>the <see cref="IGeneratorParameter"/> value.</returns>
        /// <see cref="GetParameter{T}"/>
        public T Get<T>(Type paramType) 
        {
            return GetParameter<T>(paramType).Value;
        }

        public T GetV<T>(Type paramType) where T : struct
        {
            return GetParameter<T?>(paramType).Value.Value;
        }

        /// <summary>
        /// Registers rendering parameters from <see cref="IGenerator{T}"/>s with this model.
        /// </summary>
        /// <param name="generator"></param>
        public void RegisterParameters<T>(IGenerator<T> generator) where T : IChemObject
        {
            foreach (var param in generator.Parameters)
            {
                renderingParameters[param.GetType().FullName] = param;
            }
        }

        /// <summary>
        /// Set the selected <see cref="IChemObject"/>s.
        /// </summary>
        /// <param name="selection">an <see cref="IChemObjectSelection"/> with selected <see cref="IChemObject"/>s</param>
        public void SetSelection(IChemObjectSelection selection)
        {
            this.selection = selection;
        }

        /// <summary>
        /// Returns an <see cref="IChemObjectSelection"/> with the currently selected
        /// <see cref="IChemObject"/>s.
        /// </summary>
        /// <returns>the current selected <see cref="IChemObject"/>s</returns>
        public IChemObjectSelection GetSelection()
        {
            return this.selection;
        }

        /// <summary>
        /// This is the central facility for handling "merges" of atoms. A merge occurs if during moving atoms an atom is in Range of another atom.
        /// These atoms are then put into the merge map as a key-value pair. During the move, the atoms are then marked by a circle and on releasing the mouse
        /// they get actually merged, meaning one atom is removed and bonds pointing to this atom are made to point to the atom it has been merged with.
        /// </summary>
        /// <returns>The merged map</returns>
        public IDictionary<IAtom, IAtom> GetMerge()
        {
            return merge;
        }

        /// <summary>
        /// Returns the atom currently highlighted.
        /// </summary>
        /// <returns>the atom currently highlighted</returns>
        public IAtom GetHighlightedAtom()
        {
            return this.highlightedAtom;
        }

        /// <summary>
        /// Sets the atom currently highlighted.
        /// </summary>
        /// <param name="highlightedAtom">The atom to be highlighted</param>
        public void SetHighlightedAtom(IAtom highlightedAtom)
        {
            if ((this.highlightedAtom != null) || (highlightedAtom != null))
            {
                this.highlightedAtom = highlightedAtom;
                FireChange();
            }
        }

        /// <summary>
        /// Returns the Bond currently highlighted.
        /// </summary>
        /// <returns>the Bond currently highlighted</returns>
        public IBond GetHighlightedBond()
        {
            return this.highlightedBond;
        }

        /// <summary>
        /// Sets the Bond currently highlighted.
        /// </summary>
        /// <param name="highlightedBond">The Bond to be currently highlighted</param>
        public void SetHighlightedBond(IBond highlightedBond)
        {
            if ((this.highlightedBond != null) || (highlightedBond != null))
            {
                this.highlightedBond = highlightedBond;
                FireChange();
            }
        }

        /// <summary>
        /// Returns the atoms and bonds on the Renderer2D clipboard. If the clipboard
        /// is empty it returns null. Primarily used for copy/paste.
        /// </summary>
        /// <returns>an atomcontainer with the atoms and bonds on the clipboard.</returns>
        public IAtomContainer GetClipboardContent()
        {
            return clipboardContent;
        }

        /// <summary>
        /// Sets the atoms and bonds on the Renderer2D clipboard. Primarily used for
        /// copy/paste.
        /// </summary>
        /// <param name="content">the new content of the clipboard.</param>
        public void SetClipboardContent(IAtomContainer content)
        {
            this.clipboardContent = content;
        }

        /// <summary>
        /// Change listeners.
        /// </summary>
        public ICollection<ICDKChangeListener> Listeners
        {
            get
            {
                if (listeners == null)
                    listeners = new HashSet<ICDKChangeListener>();
                return listeners;
            }
        }

        /// <summary>
        /// Notifies registered listeners of certain changes that have occurred in
        /// this model.
        /// </summary>
        public void FireChange()
        {
            if (Notification && listeners != null)
            {
                var evt = new ChemObjectChangeEventArgs(this);
                foreach (var listener in listeners)
                {
                    listener.StateChanged(evt);
                }
            }
        }

        /// <summary>
        /// Gets the toolTipText for atom certain atom.
        /// </summary>
        /// <param name="atom">The atom.</param>
        /// <returns>The toolTipText value.</returns>
        public string GetToolTipText(IAtom atom)
        {
            if (toolTipTextMap.TryGetValue(atom, out string text))
                return text;
            return null;
        }

        /// <summary>
        /// Sets the toolTipTextMap.
        /// </summary>
        /// <param name="map">A map containing Atoms of the current molecule as keys and strings to display as values. A line break will be inserted where a \n is in the string.</param>
        public void SetToolTipTextMap(IDictionary<IAtom, string> map)
        {
            toolTipTextMap = map;
            FireChange();
        }

        /// <summary>
        /// Gets the toolTipTextMap.
        /// </summary>
        /// <returns>The toolTipTextValue.</returns>
        public IDictionary<IAtom, string> GetToolTipTextMap()
        {
            return toolTipTextMap;
        }

        /// <summary>
        /// Get externally selected atoms. These are atoms selected externally in e.
        /// g. Bioclipse via the ChemObjectTree, painted in externalSelectedPartColor
        /// </summary>
        /// <returns>the selected part</returns>
        public IAtomContainer GetExternalSelectedPart()
        {
            return externalSelectedPart;
        }

        /// <summary>
        /// Set externally selected atoms. These are atoms selected externally in e.
        /// g. Bioclipse via the ChemObjectTree, painted in externalSelectedPartColor
        /// </summary>
        /// <param name="externalSelectedPart">the selected part</param>
        public void SetExternalSelectedPart(IAtomContainer externalSelectedPart)
        {
            this.externalSelectedPart = externalSelectedPart;
            var colorHash = GetParameter<IDictionary<IChemObject, Color>>(typeof(ColorHash)).Value;
            colorHash.Clear();
            if (externalSelectedPart != null)
            {
                for (int i = 0; i < externalSelectedPart.Atoms.Count; i++)
                {
                    colorHash[externalSelectedPart.Atoms[i]] = GetParameter<Color>(typeof(ExternalHighlightColor)).Value;
                }
                var bonds = externalSelectedPart.Bonds;
                foreach (var bond in bonds)
                {
                    colorHash[bond] = GetParameter<Color>(typeof(ExternalHighlightColor)).Value;
                }
            }
            FireChange();
        }

        /// <summary>
        /// Determines if the model sends around change notifications.
        /// </summary>
        /// <value>true, if notifications are sent around upon changes</value>
        public bool Notification { get; set; } = true;

        /// <summary>
        /// Returns true if the passed <see cref="IGeneratorParameter"/>s has been
        /// registered.
        /// </summary>
        /// <param name="param">parameter for which it is tested if it is registered</param>
        /// <returns>boolean indicating the parameters is registered</returns>
        public bool HasParameter(Type param)
        {
            Debug.Assert(typeof(IGeneratorParameter).IsAssignableFrom(param));
            return renderingParameters.ContainsKey(param.FullName);
        }
    }
}
