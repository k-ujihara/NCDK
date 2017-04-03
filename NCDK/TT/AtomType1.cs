

// .NET Framework port by Kazuya Ujihara
// Copyright (C) 2016-2017  Kazuya Ujihara

/*
 *  Copyright (C) 2001-2013  Christoph Steinbeck <steinbeck@users.sf.net>
 *                           John May <jwmay@users.sourceforge.net>
 *                           Egon Willighagen <egonw@users.sourceforge.net>
 *                           Rajarshi Guha <rajarshi@users.sourceforge.net>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *  All we ask is that proper credit is given for our work, which includes
 *  - but is not limited to - adding the above copyright notice to the beginning
 *  of your source code files, and to any copyright notice that you may distribute
 *  with programs based on this work.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using System;
using System.Text;

namespace NCDK.Default
{
    [Serializable]
    public class AtomType
        : Isotope, IAtomType
    {
        internal string atomTypeName;
        internal BondOrder maxBondOrder;
        internal double? bondOrderSum;
        internal int? formalCharge;
        internal int? formalNeighbourCount;
        internal Hybridization hybridization;
        internal double? covalentRadius;
        internal int? valency;
        internal bool isHydrogenBondAcceptor;
        internal bool isHydrogenBondDonor;
        internal bool isAliphatic;
        internal bool isAromatic;
        internal bool isInRing;
        internal bool isReactiveCenter;

        public AtomType(string elementSymbol)
            : base(elementSymbol)
        {
            this.formalCharge = 0;
        }

        public AtomType(string identifier, string elementSymbol)
            : base(elementSymbol)
        {
            this.atomTypeName = identifier;
        }

        public AtomType(IElement element)
            : base(element)
        {
            var aa = element as IAtomType;
            if (aa != null)
            {
                maxBondOrder = aa.MaxBondOrder;
                bondOrderSum = aa.BondOrderSum;
                covalentRadius = aa.CovalentRadius;
                formalCharge = aa.FormalCharge;
                hybridization = aa.Hybridization;
                valency = aa.Valency;
                formalNeighbourCount = aa.FormalNeighbourCount;
                atomTypeName = aa.AtomTypeName;

                isHydrogenBondAcceptor = aa.IsHydrogenBondAcceptor;
                isHydrogenBondDonor = aa.IsHydrogenBondDonor;
                isAromatic = aa.IsAromatic;
                isInRing = aa.IsInRing;
            }
        }

        public virtual string AtomTypeName
        {
            get { return atomTypeName; }
            set 
            {
                atomTypeName = value; 
                NotifyChanged();
            }
        }

        public virtual BondOrder MaxBondOrder
        {
            get { return maxBondOrder; }
            set
            {
                maxBondOrder = value; 
                NotifyChanged();
            }
        }

        public virtual double? BondOrderSum
        {
            get { return bondOrderSum; }
            set 
            {
                bondOrderSum = value; 
                NotifyChanged();
            }
        }

        public virtual int? FormalCharge
        {
            get { return formalCharge; }
            set 
            { 
                formalCharge = value; 
                NotifyChanged();
            }
        }

        public virtual int? FormalNeighbourCount
        {
            get { return formalNeighbourCount; }
            set
            { 
                formalNeighbourCount = value; 
                NotifyChanged();
            }

        }

        public virtual Hybridization Hybridization
        {
            get { return hybridization; }
            set { 
                hybridization = value; 
                NotifyChanged();
            }
        }

        public virtual double? CovalentRadius
        {
            get { return covalentRadius; }
            set
            { 
                covalentRadius = value; 
                NotifyChanged();
            }
        }

        public virtual int? Valency
        {
            get { return valency; }
            set 
            {
                valency = value; 
                NotifyChanged();
            }
        }

        public virtual bool IsHydrogenBondAcceptor
        {
            get { return isHydrogenBondAcceptor; }
            set
            {
                isHydrogenBondAcceptor = value; 
                NotifyChanged();
            }
        }

        public virtual bool IsHydrogenBondDonor
        {
            get { return isHydrogenBondDonor; }
            set
            {
                isHydrogenBondDonor = value; 
                NotifyChanged();
            }
        }

        public virtual bool IsAliphatic
        {
            get { return isAliphatic; }
            set
            { 
                isAliphatic = value; 
                NotifyChanged();
            }
        }

        public virtual bool IsAromatic
        {
            get { return isAromatic; }
            set
            {
                isAromatic = value; 
                NotifyChanged();
            }
        }

        public virtual bool IsInRing
        {
            get { return isInRing; }
            set 
            {
                isInRing = value; 
                NotifyChanged();
            }
        }

        public virtual bool IsReactiveCenter
        {
            get { return isReactiveCenter; }
            set 
            {
                isReactiveCenter = value; 
                NotifyChanged();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("AtomType(").Append(GetHashCode());
            if (AtomTypeName != null)
                sb.Append(", N:").Append(AtomTypeName);
            if (MaxBondOrder != BondOrder.Unset)
                sb.Append(", MBO:").Append(MaxBondOrder);
            if (BondOrderSum != null)
                sb.Append(", BOS:").Append(BondOrderSum);
            if (FormalCharge != null)
                sb.Append(", FC:").Append(FormalCharge);
            if (Hybridization != Hybridization.Unset)
                sb.Append(", H:").Append(Hybridization);
            if (FormalNeighbourCount != null)
                sb.Append(", NC:").Append(FormalNeighbourCount);
            if (CovalentRadius != null)
                sb.Append(", CR:").Append(CovalentRadius);
            if (Valency != null)
                sb.Append(", EV:").Append(Valency);
            sb.Append(", ").Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override bool Compare(object obj)
        {
            var o = obj as IAtomType;
            return o != null && base.Compare(obj)
                && AtomTypeName == o.AtomTypeName
                && MaxBondOrder == o.MaxBondOrder
                && BondOrderSum == o.BondOrderSum;
        }
    }
}
namespace NCDK.Silent
{
    [Serializable]
    public class AtomType
        : Isotope, IAtomType
    {
        internal string atomTypeName;
        internal BondOrder maxBondOrder;
        internal double? bondOrderSum;
        internal int? formalCharge;
        internal int? formalNeighbourCount;
        internal Hybridization hybridization;
        internal double? covalentRadius;
        internal int? valency;
        internal bool isHydrogenBondAcceptor;
        internal bool isHydrogenBondDonor;
        internal bool isAliphatic;
        internal bool isAromatic;
        internal bool isInRing;
        internal bool isReactiveCenter;

        public AtomType(string elementSymbol)
            : base(elementSymbol)
        {
            this.formalCharge = 0;
        }

        public AtomType(string identifier, string elementSymbol)
            : base(elementSymbol)
        {
            this.atomTypeName = identifier;
        }

        public AtomType(IElement element)
            : base(element)
        {
            var aa = element as IAtomType;
            if (aa != null)
            {
                maxBondOrder = aa.MaxBondOrder;
                bondOrderSum = aa.BondOrderSum;
                covalentRadius = aa.CovalentRadius;
                formalCharge = aa.FormalCharge;
                hybridization = aa.Hybridization;
                valency = aa.Valency;
                formalNeighbourCount = aa.FormalNeighbourCount;
                atomTypeName = aa.AtomTypeName;

                isHydrogenBondAcceptor = aa.IsHydrogenBondAcceptor;
                isHydrogenBondDonor = aa.IsHydrogenBondDonor;
                isAromatic = aa.IsAromatic;
                isInRing = aa.IsInRing;
            }
        }

        public virtual string AtomTypeName
        {
            get { return atomTypeName; }
            set 
            {
                atomTypeName = value; 
            }
        }

        public virtual BondOrder MaxBondOrder
        {
            get { return maxBondOrder; }
            set
            {
                maxBondOrder = value; 
            }
        }

        public virtual double? BondOrderSum
        {
            get { return bondOrderSum; }
            set 
            {
                bondOrderSum = value; 
            }
        }

        public virtual int? FormalCharge
        {
            get { return formalCharge; }
            set 
            { 
                formalCharge = value; 
            }
        }

        public virtual int? FormalNeighbourCount
        {
            get { return formalNeighbourCount; }
            set
            { 
                formalNeighbourCount = value; 
            }

        }

        public virtual Hybridization Hybridization
        {
            get { return hybridization; }
            set { 
                hybridization = value; 
            }
        }

        public virtual double? CovalentRadius
        {
            get { return covalentRadius; }
            set
            { 
                covalentRadius = value; 
            }
        }

        public virtual int? Valency
        {
            get { return valency; }
            set 
            {
                valency = value; 
            }
        }

        public virtual bool IsHydrogenBondAcceptor
        {
            get { return isHydrogenBondAcceptor; }
            set
            {
                isHydrogenBondAcceptor = value; 
            }
        }

        public virtual bool IsHydrogenBondDonor
        {
            get { return isHydrogenBondDonor; }
            set
            {
                isHydrogenBondDonor = value; 
            }
        }

        public virtual bool IsAliphatic
        {
            get { return isAliphatic; }
            set
            { 
                isAliphatic = value; 
            }
        }

        public virtual bool IsAromatic
        {
            get { return isAromatic; }
            set
            {
                isAromatic = value; 
            }
        }

        public virtual bool IsInRing
        {
            get { return isInRing; }
            set 
            {
                isInRing = value; 
            }
        }

        public virtual bool IsReactiveCenter
        {
            get { return isReactiveCenter; }
            set 
            {
                isReactiveCenter = value; 
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("AtomType(").Append(GetHashCode());
            if (AtomTypeName != null)
                sb.Append(", N:").Append(AtomTypeName);
            if (MaxBondOrder != BondOrder.Unset)
                sb.Append(", MBO:").Append(MaxBondOrder);
            if (BondOrderSum != null)
                sb.Append(", BOS:").Append(BondOrderSum);
            if (FormalCharge != null)
                sb.Append(", FC:").Append(FormalCharge);
            if (Hybridization != Hybridization.Unset)
                sb.Append(", H:").Append(Hybridization);
            if (FormalNeighbourCount != null)
                sb.Append(", NC:").Append(FormalNeighbourCount);
            if (CovalentRadius != null)
                sb.Append(", CR:").Append(CovalentRadius);
            if (Valency != null)
                sb.Append(", EV:").Append(Valency);
            sb.Append(", ").Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }

        public override bool Compare(object obj)
        {
            var o = obj as IAtomType;
            return o != null && base.Compare(obj)
                && AtomTypeName == o.AtomTypeName
                && MaxBondOrder == o.MaxBondOrder
                && BondOrderSum == o.BondOrderSum;
        }
    }
}
