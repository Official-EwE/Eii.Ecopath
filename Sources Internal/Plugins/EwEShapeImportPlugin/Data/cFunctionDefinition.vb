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
Imports EwEUtils.Core

#End Region ' Imports

Public Class cFunctionDefinition

#Region " Private vars "

    Private m_strName As String
    Private m_fn As IShapeFunction
    Private m_parms(5) As Single

#End Region ' Private vars

    Public Sub New(strName As String, fn As IShapeFunction, p1 As Single, p2 As Single, p3 As Single, p4 As Single, p5 As Single)
        Me.m_strName = strName
        Me.m_fn = fn
        Me.m_parms(1) = p1
        Me.m_parms(2) = p2
        Me.m_parms(3) = p3
        Me.m_parms(4) = p4
        Me.m_parms(5) = p5
    End Sub

    Public ReadOnly Property Name As String
        Get
            Return Me.m_strName
        End Get
    End Property

    Public ReadOnly Property ShapeFunction As IShapeFunction
        Get
            Return Me.m_fn
        End Get
    End Property

    Public ReadOnly Property Parms(i As Integer) As Single
        Get
            If (i < 1 Or i > Me.m_fn.nParameters) Then Return cCore.NULL_VALUE
            Return Me.m_parms(i)
        End Get
    End Property

End Class