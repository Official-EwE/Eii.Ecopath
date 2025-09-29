// ===============================================================================
// This file is part of Ecopath with Ecosim (EwE)
//
// EwE is free software: you can redistribute it and/or modify it under the terms
// of the GNU General Public License version 2 as published by the Free Software 
// Foundation.
//
// EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
// PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with EwE.
// If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
//
//
// Copyright 1991- 
//    Ecopath International Initiative, Barcelona, Spain
// ===============================================================================

#region  Imports 

using System;
using System.ComponentModel;
using System.Diagnostics;

#endregion

namespace ValueChain
{


    /// ===========================================================================
    /// <summary>
    /// Base class for holding link information in the flow.
    /// </summary>
    /// <remarks>
    /// Note that this class does not hold the actual references to flow units.
    /// This class is a mere holder of shared behaviour between cUnitLinks and
    /// cLinkDefaults
    /// </remarks>
    /// ===========================================================================
    [TypeConverter(typeof(cPropertySorter))]
    [DefaultProperty("Name")]
    [Serializable()]
    public class cLink : cLinkDefault
    {

        #region  Helper classes 

        /// =======================================================================
    /// <summary>
    /// Helper class; allows the property grid to show a read-only unit name.
    /// </summary>
    /// =======================================================================
        public class cStaticUnitConverter : TypeConverter
        {

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                // Do not show combo
                return false;
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                // Do not edit combo
                return true;
            }

            /// <summary>
        /// Override the GetStandardValues method and return a 
        /// StandardValuesCollection filled with your standard values
        /// </summary>
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(null);
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                // Can only convert FROM unit
                return ReferenceEquals(sourceType, typeof(cUnit));
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                // Can only convert TO unit name
                return ReferenceEquals(destinationType, typeof(string));
            }

            /// <summary>
        /// Convert unit to unit
        /// </summary>
            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                return base.ConvertFrom(context, culture, value);
            }

            /// <summary>
        /// Convert unit to unit name
        /// </summary>
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {

                if (value is cUnit)
                {
                    return ((cUnit)value).Name;
                }

                return base.ConvertTo(context, culture, value, destinationType);

            }

        }

        #endregion

        #region  Private bits 

        /// <summary>Link name.</summary>
        private string m_strName = "";
        private cUnit m_source = null;
        private cUnit m_target = null;

        #endregion

        public cLink() : base()
        {
        }


        [Browsable(true)]
        [Category(cCATEGORY_GENERIC)]
        [DisplayName("Name")]
        [Description("Name of this link")]
        [cPropertySorter.PropertyOrder(1)]
        public override string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(m_strName))
                {
                    try
                    {
                        return string.Format("{0} to {1}", Source.ToString(), Target.ToString());
                    }
                    catch (Exception ex)
                    {
                        return "<unnamed link>";
                    }
                }
                return m_strName;
            }
            set
            {
                m_strName = value;
                SetChanged();
            }
        }

        [Browsable(true)]
        [Category(cCATEGORY_GENERIC)]
        [DisplayName("Source")]
        [Description("Source unit for this link")]
        [cPropertySorter.PropertyOrder(2)]
        [TypeConverter(typeof(cStaticUnitConverter))]
        public cUnit Source
        {
            get
            {
                return m_source;
            }
            set
            {
                Debug.Assert(value is not null);
                m_source = value;
            }
        }

        [Browsable(true)]
        [Category(cCATEGORY_GENERIC)]
        [DisplayName("Target")]
        [Description("Target unit for this link")]
        [cPropertySorter.PropertyOrder(3)]
        [TypeConverter(typeof(cStaticUnitConverter))]
        public cUnit Target
        {
            get
            {
                return m_target;
            }
            set
            {
                Debug.Assert(value is not null);
                m_target = value;
            }
        }

        [Browsable(true)]
        [Category(cCATEGORY_GENERIC)]
        [DisplayName("External")]
        [Description("True when source and target differ in nationality.")]
        [cPropertySorter.PropertyOrder(4)]
        public bool External
        {
            get
            {
                if (Source is null)
                    return false;
                if (Target is null)
                    return false;
                return Source.Nationality != Target.Nationality;
            }
        }


        public virtual bool IsConfigured
        {
            get
            {
                return ValuePerTon != 1f;
            }
        }


        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is cLink))
                return false;
            cLink l = (cLink)obj;
            return Source.DBID == l.Source.DBID & Target.DBID == l.Target.DBID;
        }

        public override string ToString()
        {
            return Name + " " + BiomassRatio.ToString();
        }

        public override bool IsDefault
        {
            get
            {
                return false;
            }
        }

    }
}