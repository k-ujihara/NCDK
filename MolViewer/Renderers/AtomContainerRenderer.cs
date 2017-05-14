/*  Copyright (C) 2008-2009  Gilleain Torrance <gilleain.torrance@gmail.com>
 *                2008-2009  Arvid Berg <goglepox@users.sf.net>
 *                     2009  Egon Willighagen <egonw@users.sf.net>
 *
 *  Contact: cdk-devel@list.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Geometries;
using NCDK.Numerics;
using NCDK.Renderers;
using NCDK.Renderers.Elements;
using NCDK.Renderers.Fonts;
using NCDK.Renderers.Generators;
using NCDK.Renderers.Visitors;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace NCDK.MolViewer.Renderers
{
    /// <summary>
    /// A general renderer for <see cref="IAtomContainer"/>s. The chem object
    /// is converted into a 'diagram' made up of <see cref="IRenderingElement"/>s. It takes
    /// an <see cref="IDrawVisitor"/> to do the drawing of the generated diagram. Various
    /// display properties can be set using the <see cref="JChemPaintRendererModel"/>.
    /// </summary>
    /// <example>
    /// This class has several usage patterns. For just painting fit-to-screen do:
    /// <code>
    ///   renderer.paintMolecule(molecule, visitor, drawArea)
    /// </code>
    /// for painting at a scale determined by the bond length in the RendererModel:
    /// <code>
    ///   if (moleculeIsNew) {
    ///     renderer.setup(molecule, drawArea);
    ///   }
    ///   Rectangle diagramSize = renderer.paintMolecule(molecule, visitor);
    ///   // ...update scroll bars here
    /// </code>
    /// to paint at full screen size, but not resize with each change:
    /// <code>
    ///   if (moleculeIsNew) {
    ///     renderer.setScale(molecule);
    ///     Rectangle diagramBounds = renderer.calculateDiagramBounds(molecule);
    ///     renderer.setZoomToFit(diagramBounds, drawArea);
    ///     renderer.paintMolecule(molecule, visitor);
    ///   } else {
    ///     Rectangle diagramSize = renderer.paintMolecule(molecule, visitor);
    ///   // ...update scroll bars here
    ///   }
    /// </code>
    /// finally, if you are scrolling, and have not changed the diagram:
    /// <code>
    ///   renderer.repaint(visitor)
    /// </code>
    /// will just repaint the previously generated diagram, at the same scale.
    /// </example>
    /// <remarks>
    /// <para>
    /// There are two sets of methods for painting IChemObjects - those that take
    /// a Rectangle that represents the desired draw area, and those that return a
    /// Rectangle that represents the actual draw area. The first are intended for
    /// drawing molecules fitted to the screen (where 'screen' means any drawing
    /// area) while the second type of method are for drawing bonds at the length
    /// defined by the <see cref="JChemPaintRendererModel"/> parameter bondLength.
    /// </para>
    /// <para>
    /// There are two numbers used to transform the model so that it fits on screen.
    /// The first is <c>scale</c>, which is used to map model coordinates to
    /// screen coordinates. The second is <c>zoom</c> which is used to, well,
    /// zoom the on screen coordinates. If the diagram is fit-to-screen, then the
    /// ratio of the bounds when drawn using bondLength and the bounds of
    /// the screen is used as the zoom.
    /// </para>
    /// <para>
    /// So, if the bond length on screen is set to 40, and the average bond length
    /// of the model is 2 (unitless, but roughly Ångström scale) then the
    /// scale will be 20. If the model is 10 units wide, then the diagram drawn at
    /// 100% zoom will be 10 * 20 = 200 in width on screen. If the screen is 400
    /// pixels wide, then fitting it to the screen will make the zoom 200%. Since the
    /// zoom is just a floating point number, 100% = 1 and 200% = 2.
    /// </para>
    /// </remarks>
    // @author maclean
    // @cdk.module renderbasic 
    public class AtomContainerRenderer : IRenderer
    {
        /// <summary>
        /// The default scale is used when the model is empty.
        /// </summary>
        public const double DefaultScale = 30.0;

        protected IFontManager fontManager;

        /// <summary>
        /// The renderer model is final as it is not intended to be replaced.
        /// </summary>
        protected readonly JChemPaintRendererModel rendererModel;

        protected IList<IGenerator<IAtomContainer>> generators;

        protected Transform transform;

        protected Vector2 modelCenter = new Vector2(0, 0); // model

        protected Vector2 drawCenter = new Vector2(150, 200); //diagram on screen

        protected double scale = DefaultScale;

        protected double zoom = 1.0;

        protected IRenderingElement cachedDiagram;

        /// <summary>
        /// A renderer that generates diagrams using the specified
        /// generators and manages fonts with the supplied font manager.
        /// </summary>
        /// <param name="generators">a list of classes that implement the IGenerator interface</param>
        /// <param name="fontManager">a class that manages mappings between zoom and font sizes</param>
        public AtomContainerRenderer(IList<IGenerator<IAtomContainer>> generators,
                                     IFontManager fontManager)
        {
            rendererModel = new JChemPaintRendererModel();
            this.generators = generators;
            this.fontManager = fontManager;
            foreach (IGenerator<IAtomContainer> generator in generators)
            {
                if (generator.Parameters != null)
                    rendererModel.RegisterParameters(generator);
            }
        }

        /// <summary>
        /// Setup the transformations necessary to draw this Atom Container.
        /// </summary>
        /// <param name="atomContainer"></param>
        /// <param name="screen"></param>
        public void Setup(IAtomContainer atomContainer, Rect screen)
        {
            this.SetScale(atomContainer);
            var bounds = CalculateBounds(atomContainer);
            this.modelCenter = new Vector2(bounds.GetCenterX(), bounds.GetCenterY());
            this.drawCenter = new Vector2(screen.GetCenterX(), screen.GetCenterY());
            this.Setup();
        }

        public void Reset()
        {
            modelCenter = new Vector2(0, 0);
            drawCenter = new Vector2(200, 200);
            zoom = 1.0;
            Setup();
        }

        /// <summary>
        /// Set the scale for an IAtomContainer. It calculates the average bond
        /// length of the model and calculates the multiplication factor to transform
        /// this to the bond length that is set in the RendererModel.
        /// </summary>
        /// <param name="atomContainer"></param>
        public void SetScale(IAtomContainer atomContainer)
        {
            double bondLength = GeometryUtil.GetBondLengthAverage(atomContainer);
            this.scale = this.CalculateScaleForBondLength(bondLength);

            // store the scale so that other components can access it
            this.rendererModel.SetScale(scale);
        }

        /*
        public Rectangle paint(
                IAtomContainer atomContainer,IDrawVisitor drawVisitor) {
            // the bounds of the model
            Rectangle2D modelBounds = calculateBounds(atomContainer);

            // setup and draw
            this.setupTransformNatural(modelBounds);
            IRenderingElement diagram = this.generateDiagram(atomContainer);
            this.paint(drawVisitor, diagram);

            return this.convertToDiagramBounds(modelBounds);
        }
        */

        /// <summary>
        /// Paint a molecule (an IAtomContainer).
        /// </summary>
        /// <param name="atomContainer">the molecule to paint</param>
        /// <param name="drawVisitor">the visitor that does the drawing</param>
        /// <param name="bounds">the bounds on the screen</param>
        /// <param name="resetCenter">if true, set the draw center to be the center of bounds</param>
        public void PaintMolecule(IAtomContainer atomContainer,
                                  IDrawVisitor drawVisitor, Rect bounds, bool resetCenter)
        {
            // the bounds of the model
            Rect modelBounds = CalculateBounds(atomContainer);

            this.SetupTransformToFit(bounds, modelBounds,
                                     GeometryUtil.GetBondLengthAverage(atomContainer), resetCenter);

            // the diagram to draw
            IRenderingElement diagram = this.GenerateDiagram(atomContainer);

            this.Paint(drawVisitor, diagram);
        }

        /// <summary>
        /// Repaint using the cached diagram
        /// </summary>
        /// <param name="drawVisitor">the wrapper for the graphics object that draws</param>
        public void Repaint(IDrawVisitor drawVisitor)
        {
            this.Paint(drawVisitor, cachedDiagram);
        }

        public Rect CalculateDiagramBounds(IAtomContainer atomContainer)
        {
            return this.CalculateScreenBounds(
                    CalculateBounds(atomContainer));
        }

        public Rect CalculateScreenBounds(Rect modelBounds)
        {
            double margin = this.rendererModel.GetMargin();
            var modelScreenCenter
                    = this.ToScreenCoordinates(modelBounds.GetCenterX(),
                                               modelBounds.GetCenterY());
            double w = (scale * zoom * modelBounds.Width) + (2 * margin);
            double h = (scale * zoom * modelBounds.Height) + (2 * margin);
            return new Rect((int)(modelScreenCenter.X - w / 2),
                                 (int)(modelScreenCenter.Y - h / 2),
                                 (int)w,
                                 (int)h);
        }

        public static Rect CalculateBounds(IAtomContainer ac)
        {
            // this is essential, otherwise a rectangle
            // of (+INF, -INF, +INF, -INF) is returned!
            if (ac.Atoms.Count == 0)
            {
                return new Rect();
            }
            else if (ac.Atoms.Count == 1)
            {
                Vector2 p = ac.Atoms[0].Point2D.Value;
                return new Rect(p.X, p.Y, 0, 0);
            }

            double xmin = double.PositiveInfinity;
            double xmax = double.NegativeInfinity;
            double ymin = double.PositiveInfinity;
            double ymax = double.NegativeInfinity;

            foreach (IAtom atom in ac.Atoms)
            {
                Vector2 p = atom.Point2D.Value;
                xmin = Math.Min(xmin, p.X);
                xmax = Math.Max(xmax, p.X);
                ymin = Math.Min(ymin, p.Y);
                ymax = Math.Max(ymax, p.Y);
            }
            double w = xmax - xmin;
            double h = ymax - ymin;
            return new Rect(xmin, ymin, w, h);
        }

        public JChemPaintRendererModel GetRenderer2DModel()
        {
            return this.rendererModel;
        }

        public Point ToModelCoordinates(double screenX, double screenY)
        {
            Matrix inv = transform.Value;
            if (inv.HasInverse)
            {
                inv.Invert();
                var src = new Point(screenX, screenY);
                var dest = inv.Transform(src);
                return dest;
            }
            else
            {
                return new Point(0, 0);
            }
        }

        public Point ToScreenCoordinates(double modelX, double modelY)
        {
            var dest = transform.Transform(new Point(modelX, modelY));
            return dest;
        }

        public void SetModelCenter(double x, double y)
        {
            this.modelCenter = new Vector2(x, y);
            Setup();
        }

        public void SetDrawCenter(double x, double y)
        {
            this.drawCenter = new Vector2(x, y);
            Setup();
        }

        public void SetZoom(double z)
        {
            GetRenderer2DModel().SetZoomFactor(z);
            zoom = z;
            Setup();
        }

        /// <summary>
        /// Move the draw center by dx and dy.
        /// </summary>
        /// <param name="dx">the x shift</param>
        /// <param name="dy">the y shift</param>
        public void ShiftDrawCenter(double dx, double dy)
        {
            this.drawCenter = new Vector2(this.drawCenter.X + dx, this.drawCenter.Y + dy);
            Setup();
        }

        public Vector2 GetDrawCenter()
        {
            return drawCenter;
        }

        public Vector2 GetModelCenter()
        {
            return modelCenter;
        }

        /// <summary>
        /// Calculate and set the zoom factor needed to completely fit the diagram
        /// onto the screen bounds.
        /// </summary>
        /// <param name="diagramBounds"></param>
        /// <param name="drawBounds"></param>
        public void SetZoomToFit(double drawWidth,
                                 double drawHeight,
                                 double diagramWidth,
                                 double diagramHeight)
        {
            double m = this.rendererModel.GetMargin();

            // determine the zoom needed to fit the diagram to the screen
            double widthRatio = drawWidth / (diagramWidth + (2 * m));
            double heightRatio = drawHeight / (diagramHeight + (2 * m));

            this.zoom = Math.Min(widthRatio, heightRatio);

            this.fontManager.Zoom = zoom;

            // record the zoom in the model, so that generators can use it
            this.rendererModel.SetZoomFactor(zoom);
        }

        /// <summary>
        /// The target method for paintChemModel, paintReaction, and paintMolecule.
        /// </summary>
        /// <param name="drawVisitor">the visitor to draw with</param>
        /// <param name="diagram">the IRenderingElement tree to render</param>
        private void Paint(IDrawVisitor drawVisitor,
                           IRenderingElement diagram)
        {
            if (diagram == null) return;

            // cache the diagram for quick-redraw
            this.cachedDiagram = diagram;

            this.fontManager.FontName = this.rendererModel.GetFontName();
            this.fontManager.FontStyle = this.rendererModel.GetFontStyle();

            drawVisitor.SetFontManager(this.fontManager);
            drawVisitor.SetTransform(this.transform);
            drawVisitor.SetRendererModel(this.rendererModel);
            diagram.Accept(drawVisitor);
        }

        /// <summary>
        /// Set the transform for a non-fit to screen paint.
        /// </summary>
        /// <param name="modelBounds">the bounding box of the model</param>
        private void SetupTransformNatural(Rect modelBounds)
        {
            this.zoom = this.rendererModel.GetZoomFactor();
            this.fontManager.Zoom = zoom;
            this.Setup();
        }

        /// <summary>
        /// Sets the transformation needed to draw the model on the canvas when
        /// the diagram needs to fit the screen.
        /// </summary>
        /// <param name="screenBounds">the bounding box of the draw area</param>
        /// <param name="modelBounds">the bounding box of the model</param>
        /// <param name="bondLength">the average bond length of the model</param>
        /// <param name="reset">if true, model center will be set to the modelBounds center            and the scale will be re-calculated</param>
        private void SetupTransformToFit(Rect screenBounds,
                                    Rect modelBounds,
                                    double bondLength,
                                    bool reset)
        {
            if (screenBounds == null) return;

            this.SetDrawCenter(
                    screenBounds.GetCenterX(), screenBounds.GetCenterY());

            this.scale = this.CalculateScaleForBondLength(bondLength);

            double drawWidth = screenBounds.Width;
            double drawHeight = screenBounds.Height;

            double diagramWidth = modelBounds.Width * scale;
            double diagramHeight = modelBounds.Height * scale;

            this.SetZoomToFit(drawWidth, drawHeight, diagramWidth, diagramHeight);

            // this controls whether editing a molecule causes it to re-center
            // with each change or not
            if (reset || rendererModel.IsFitToScreen())
            {
                this.SetModelCenter(
                        modelBounds.GetCenterX(), modelBounds.GetCenterY());
            }

            // set the scale in the renderer model for the generators
            if (reset)
            {
                this.rendererModel.SetScale(scale);
            }

            this.Setup();
        }

        /// <summary>
        /// Given a bond length for a model, calculate the scale that will transform
        /// this length to the on screen bond length in RendererModel.
        /// </summary>
        /// <param name="modelBondLength"></param>
        /// <param name="reset"></param>
        /// <returns></returns>
        private double CalculateScaleForBondLength(double modelBondLength)
        {
            if (Double.IsNaN(modelBondLength) || modelBondLength == 0)
            {
                return DefaultScale;
            }
            else
            {
                return this.rendererModel.GetBondLength() / modelBondLength;
            }
        }

        /// <summary>
        /// Calculate the bounds of the diagram on screen, given the current scale,
        /// zoom, and margin.
        /// </summary>
        /// <param name="modelBounds">the bounds in model space of the chem object</param>
        /// <returns>the bounds in screen space of the drawn diagram</returns>
        private Rect ConvertToDiagramBounds(Rect modelBounds)
        {
            double cx = modelBounds.GetCenterX();
            double cy = modelBounds.GetCenterY();
            double mw = modelBounds.Width;
            double mh = modelBounds.Height;

            var mc = this.ToScreenCoordinates(cx, cy);

            // special case for 0 or 1 atoms
            if (mw == 0 && mh == 0)
            {
                return new Rect((int)mc.X, (int)mc.Y, 0, 0);
            }

            double margin = this.rendererModel.GetMargin();
            int w = (int)((scale * zoom * mw) + (2 * margin));
            int h = (int)((scale * zoom * mh) + (2 * margin));
            int x = (int)(mc.X - w / 2);
            int y = (int)(mc.Y - h / 2);

            return new Rect(x, y, w, h);
        }

        private void Setup()
        {
            // set the transform
            try
            {
                Matrix m = Matrix.Identity;
                m.Translate(-this.modelCenter.X, -this.modelCenter.Y);
                //this.transform.Scale(1, -1); // Converts between CDK Y-up & Java2D Y-down coordinate-systems
                m.Scale(scale, scale);
                m.Scale(zoom, zoom);
                m.Translate(this.drawCenter.X, this.drawCenter.Y);
                this.transform = new MatrixTransform(m);
            }
            catch (Exception)
            {
                Console.Error.WriteLine(
                        $"null pointer when setting transform: " +
                        $"drawCenter={this.drawCenter} scale={this.scale} zoom={this.zoom} modelCenter={this.modelCenter}");
            }
        }

        public IRenderingElement GenerateDiagram(IAtomContainer ac)
        {
            ElementGroup diagram = new ElementGroup();
            foreach (var generator in this.generators)
            {
                IRenderingElement element = generator.Generate(ac, this.rendererModel);
                if (!(element is TextGroupElement) || ((TextGroupElement)element).children.Count > 0)
                    diagram.Add(element);
            }
            //Rgroup stuff (not contained in the ac)

            return diagram;
        }

        public IList<IGenerator<IAtomContainer>> GetGenerators()
        {
            return new List<IGenerator<IAtomContainer>>(generators);
        }

        /// <summary>
        /// Gets the currently used FontManager.
        /// </summary>
        /// <returns>The currently used FontManager.</returns>
        public IFontManager GetFontManager()
        {
            return fontManager;
        }
    }
}
