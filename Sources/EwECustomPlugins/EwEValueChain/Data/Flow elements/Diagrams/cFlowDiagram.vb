' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel
Imports EwECore.Database.cEwEDatabase
Imports EwEUtils.Utilities

''' ===========================================================================
''' <summary>
''' One single flow diagram.
''' </summary>
''' ===========================================================================
<TypeConverter(GetType(cPropertySorter)),
    DefaultProperty("Name"),
    Serializable()>
Public Class cFlowDiagram
    Inherits cOOPStorable

#Region " Properties "

    <Browsable(True),
       DisplayName("Name"),
       Description("Name of this diagram"),
       cPropertySorter.PropertyOrder(1)>
    Public Overridable Property Name() As String = "Default"

#End Region ' Properties

End Class
