' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel
Imports EwEUtils.Utilities



''' ===========================================================================
''' <summary>
''' This class holds defaults for <see cref="cRetailerUnit">retailer units</see>
''' in the Ecost model. Defaults are used as blueprints for spawning their base 
''' class objects.
''' </summary>
''' ===========================================================================
<TypeConverter(GetType(cPropertySorter)),
    Serializable()>
Public Class cRetailerUnitDefault
    Inherits cRetailerUnit

    <Browsable(False)>
    Public Overrides Property Name() As String
        Get
            Return "Default"
        End Get
        Set(value As String)
        End Set
    End Property

    <Browsable(False)>
    Public Overrides ReadOnly Property Category() As String
        Get
            Return ""
        End Get
    End Property

End Class
