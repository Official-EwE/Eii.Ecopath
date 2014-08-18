' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On

Imports EwECore

#End Region ' Imports

Public MustInherit Class cSketchedShapeFunction
    Inherits cShapeFunction

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Sub Defaults()
        ' NOP
    End Sub

    Public Overrides Function IsRelevantDataType(datatype As EwEUtils.Core.eDataTypes) As Boolean
        Return (datatype = EwEUtils.Core.eDataTypes.Forcing) Or _
               (datatype = EwEUtils.Core.eDataTypes.Mediation) Or _
               (datatype = EwEUtils.Core.eDataTypes.PriceMediation)
    End Function

    Public Overrides ReadOnly Property nParameters As Integer
        Get
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property ParamName(iParam As Integer) As String
        Get
            Return "?"
        End Get
    End Property

    Public Overrides Function CalculateShape(Optional ByVal nPoints As Integer = 1200) As Single()
        Return Me.m_sPoints
    End Function

End Class

