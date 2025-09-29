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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace ValueChain
{

    #endregion

    /// ===========================================================================
/// <summary>
/// Code taken from "Ordering Items in the Property Grid" by
/// Paul T (http://www.codeproject.com/script/Articles/MemberArticles.aspx?amid=126190)
/// url: http://www.codeproject.com/KB/cpp/orderedpropertygrid.aspx
/// </summary>
/// <remarks>
/// Usage:
/// 
/// [TypeConverter(TypeOf(PropertySorter))]
/// [DefaultProperty("Name")]
/// Public Class Person
/// {
///     [cPropertySorter.PropertyOrder(1)}
///     Public Property Test
///     ..
/// }
/// </remarks>
/// ===========================================================================
    public class cPropertySorter : ExpandableObjectConverter
    {

        #region  Helper classes 

        #region  PropertyOrderAttribute 

        [AttributeUsage(AttributeTargets.Property)]
        public class PropertyOrderAttribute : Attribute
        {

            /// <summary>Simple attribute to allow the order of a property to be specified.</summary>
            private int m_iOrder = 0;

            public PropertyOrderAttribute(int iOrder)
            {
                m_iOrder = iOrder;
            }

            public int Order
            {
                get
                {
                    return m_iOrder;
                }
            }
        }

        #endregion

        #region  PropertyOrderComparer 

        private class PropertyOrderComparer : IComparable
        {

            private string m_strPropertyName = "";
            private string m_strCategory = "";
            private string m_strDisplayName = "";
            private int m_iOrder = 0;

            /// ---------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strPropertyName">Property name</param>
        /// <param name="strCategory">Category attribute</param>
        /// <param name="strDisplayName">Name attribute</param>
        /// <param name="iOrder">Order attribute</param>
        /// ---------------------------------------------------------------
            public PropertyOrderComparer(string strPropertyName, string strCategory, string strDisplayName, int iOrder)
            {
                m_strPropertyName = strPropertyName;
                m_strCategory = strCategory;
                m_strDisplayName = strDisplayName;
                m_iOrder = iOrder;
            }

            public string PropertyName
            {
                get
                {
                    return m_strPropertyName;
                }
            }

            public string Category
            {
                get
                {
                    return m_strCategory;
                }
            }

            public string DisplayName
            {
                get
                {
                    return m_strDisplayName;
                }
            }

            public int Order
            {
                get
                {
                    return m_iOrder;
                }
            }

            public int CompareTo(object obj)
            {

                // Get object to compare to
                PropertyOrderComparer cmp = (PropertyOrderComparer)obj;
                // Sort by category first
                int iSort = string.Compare(m_strCategory, cmp.Category);

                // Categories match?
                if (iSort == 0)
                {
                    // #Yes: sort by order
                    // Orders match?
                    if (cmp.Order == m_iOrder)
                    {
                        // #Yes: sort by name 
                        iSort = string.Compare(m_strDisplayName, cmp.DisplayName);
                    }
                    // #No: sort by order
                    else if (cmp.Order > m_iOrder)
                    {
                        iSort = -1;
                    }
                    else
                    {
                        iSort = 1;
                    }
                }

                return iSort;

            }

        }

        #endregion

        #endregion

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
    /// This override returns a list of properties in order.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="value"></param>
    /// <param name="attributes"></param>
    /// <returns></returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {

            var pdc = TypeDescriptor.GetProperties(value, attributes);
            Attribute attribute = null;
            PropertyOrderAttribute poa = null;
            string strName = "";
            var alPropsOrdered = new ArrayList();
            var lstrNames = new List<string>();

            foreach (PropertyDescriptor pd in pdc)
            {

                // Get appropriate name
                if (!string.IsNullOrEmpty(pd.DisplayName))
                {
                    strName = pd.DisplayName;
                }
                else
                {
                    strName = pd.Name;
                }

                // Get order attribute, if any
                attribute = pd.Attributes[typeof(PropertyOrderAttribute)];
                // Has an order specifier attribute?
                if (attribute is not null)
                {
                    // #Yes: create an pair object to hold it
                    poa = (PropertyOrderAttribute)attribute;
                    alPropsOrdered.Add(new PropertyOrderComparer(pd.Name, pd.Category, strName, poa.Order));
                }
                else
                {
                    // #No: create dummy pair object with a default order of 0
                    alPropsOrdered.Add(new PropertyOrderComparer(pd.Name, pd.Category, strName, 0));
                }
            }

            // Perform the actual order using the value PropertyOrderPair classes
            // implementation of IComparable to sort
            alPropsOrdered.Sort();

            // Build a string list of the ordered names
            foreach (PropertyOrderComparer pop in alPropsOrdered)
                lstrNames.Add(pop.PropertyName);

            // Pass in the ordered list for the PropertyDescriptorCollection to sort by
            return pdc.Sort(lstrNames.ToArray());
        }

    }

    /// =======================================================================
/// <summary>
/// Property conversion utility class.
/// </summary>
/// =======================================================================
    public class cPropertyConverter
    {

        /// -------------------------------------------------------------------
    /// <summary>
    /// Find a <see cref="PropertyDescriptor">PropertyDescriptor</see> for
    /// a given <see cref="PropertyInfo">PropertyInfo</see> instance.
    /// </summary>
    /// <param name="pi">The property info instance to find a 
    /// property descriptor for.</param>
    /// <returns>A <see cref="PropertyDescriptor">PropertyDescriptor</see>
    /// instance, or nothing if an error occurred.</returns>
    /// -------------------------------------------------------------------
        public static PropertyDescriptor FindOrigPropertyDescriptor(PropertyInfo pi)
        {
            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(pi.DeclaringType))
            {
                if (pd.Name.Equals(pi.Name))
                {
                    return pd;
                }
            }
            return null;
        }

        /// -------------------------------------------------------------------
    /// <summary>
    /// Find a  for <see cref="PropertyInfo">PropertyInfo</see> a given 
    /// <see cref="PropertyDescriptor">PropertyDescriptor</see> instance.
    /// </summary>
    /// <param name="t">The type to search.</param>
    /// <param name="pd">The property descriptor instance to find a 
    /// property descriptor for.</param>
    /// <returns>A <see cref="PropertyInfo">PropertyInfo</see> instance,
    /// or nothing if an error occurred.</returns>
    /// -------------------------------------------------------------------
        public static PropertyInfo FindOrigPropertyInfo(Type t, PropertyDescriptor pd)
        {
            foreach (PropertyInfo pi in t.GetProperties())
            {
                if (pd.Name.Equals(pi.Name))
                {
                    return pi;
                }
            }
            return null;
        }

    }

    public class cPropertyUtils
    {

        public static bool IsWritableElemental(PropertyInfo pi)
        {
            if (!pi.CanWrite)
                return false;
            var t = pi.PropertyType;
            if (t.IsEnum)
                return true;
            if (t.IsValueType)
                return true;
            if (ReferenceEquals(t, typeof(string)))
                return true;
            return false;
        }

    }
}