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
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore.Core
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities

#End Region ' Imports

''' <summary>
''' Base layer providing access to Ecospace data as cells of single values.
''' </summary>
Public Class cEcospaceLayerVelocity
    Inherits cEcospaceLayerSingle

    Private m_source As cEcospaceLayerVector = Nothing
    Private m_iFieldIndex As Integer = 0

    Public Sub New(core As cCore, source As cEcospaceLayerVector, iFieldIndex As Integer)
        MyBase.New(core, Nothing, source.Name & cSystemUtils.IIF(iFieldIndex = 0, " (X velocity)", " (Y velocity)"))
        Me.m_source = source
        Me.m_iFieldIndex = iFieldIndex
    End Sub

    Public Overrides Property Cell(iRow As Integer, iCol As Integer) As Object
        Get
            Select Case Me.m_iFieldIndex
                Case 0 : Return Me.m_source.XVelocity(iRow, iCol)
                Case 1 : Return Me.m_source.YVelocity(iRow, iCol)
            End Select
            Return cCore.NULL_VALUE
        End Get
        Set(value As Object)
            Select Case Me.m_iFieldIndex
                Case 0 : Me.m_source.XVelocity(iRow, iCol) = CSng(value)
                Case 1 : Me.m_source.YVelocity(iRow, iCol) = CSng(value)
            End Select
        End Set
    End Property

End Class
