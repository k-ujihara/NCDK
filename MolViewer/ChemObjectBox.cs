/*
 * Copyright (C) 2018  Kazuya Ujihara <ujihara.kazuya@gmail.com>
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Depict;
using System;
using System.Windows.Media;

namespace NCDK.MolViewer
{
    public partial class ChemObjectBox : System.Windows.Controls.UserControl
    {
        public class ChemObjectChangedEventArgs : EventArgs
        {
            public IChemObject OldObject { get; set; }
            public IChemObject NewObject { get; set; }

            public ChemObjectChangedEventArgs(IChemObject oldObject, IChemObject newObject)
            {
                this.OldObject = oldObject;
                this.NewObject = newObject;
            }
        }

        public delegate void ChemObjectChangedEventHandler(object sender, ChemObjectChangedEventArgs e);

        public event ChemObjectChangedEventHandler ChemObjectChanged;

        private readonly DepictionGenerator generator;
        internal Depiction depiction;
        private IChemObject _ChemObject = null;

        public ChemObjectBox()
        {
            generator = new DepictionGenerator()
            {
                AnnotationFontScale = 0.7,
                AnnotationColor = Colors.Red,
                AlignMappedReaction = true,
                
            }; 
        }

        public DepictionGenerator Generator => generator;

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (depiction != null)
            {
                depiction.Draw(dc);
            }
        }

        private void UpdateVisual()
        {
            switch (_ChemObject)
            {
                case IAtomContainer mol:
                    depiction = Generator.Depict(mol);
                    break;
                case IReaction rxn:
                    depiction = Generator.Depict(rxn);
                    break;
                default:
                    depiction = null;
                    break;
            }

            this.InvalidateVisual();
        }
    }
}
