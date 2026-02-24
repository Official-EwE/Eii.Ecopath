' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel
Imports EwEUtils.Utilities

''' ===========================================================================
''' <summary>
''' This class represents a group of Consumers in the Ecost economic model.
''' Consumers form the end of economic flow chains.
''' </summary>
''' ===========================================================================
<TypeConverter(GetType(cPropertySorter)),
    DefaultProperty("Name"),
    Serializable()>
Public Class cConsumerUnit
    Inherits cUnit

    Public Sub New()
        MyBase.New()
    End Sub

#Region " Calculations "

#End Region ' Calculations

#Region " Properties "

#Region " General "

    Public Overrides ReadOnly Property Category() As String
        Get
            Return "Consumer"
        End Get
    End Property

#End Region ' General

    <Browsable(False)>
    Public Overrides ReadOnly Property UnitType() As cUnitFactory.eUnitType
        Get
            Return cUnitFactory.eUnitType.Consumer
        End Get
    End Property

#End Region ' Properties

End Class
