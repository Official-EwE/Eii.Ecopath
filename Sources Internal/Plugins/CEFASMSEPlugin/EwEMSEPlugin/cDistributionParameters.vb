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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright: 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' Cefas MSE plug-in copyright: 2013- Cefas, Lowestoft, UK.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Option Explicit On

#End Region ' Imports

Public MustInherit Class cDistributionParamsData
    
End Class

''' <summary>
''' This holds one item in the list of any Ecopath parameters. 
''' Later in the code the entire list is grouped into a list of EcopathParam
''' </summary>
Public Class EcopathParam
    Inherits cDistributionParamsData

    Public Sub New(ByVal GroupNumber As Integer, ByVal GroupName As String, ByVal Mean As Single, ByVal CV As Double, ByVal LowerBound As Double, ByVal UpperBound As Double)
        Me.GroupNo = GroupNumber
        Me.GroupName = GroupName
        Me.Mean = Mean
        Me.CV = CV
        Me.LowerBound = LowerBound
        Me.UpperBound = UpperBound
    End Sub

    Public Property CV() As Double
    Public Property LowerBound() As Double
    Public Property UpperBound() As Double
    Public Property GroupNo As Integer
    Public Property GroupName As String
    Public Property Mean As Double

End Class

''' <summary>
''' Similar to <see cref="EcopathParam"/>, this holds one item 
''' in the list of any Ecosim parameters
''' </summary>
Public Class EcosimParam
    Inherits cDistributionParamsData

    Public Sub New(ByVal GroupNumber As Integer, _
                   ByVal GroupName As String, _
                   ByVal DistributionType As cMSE.DistributionType, _
                   ByVal LowerBound As Double, _
                   ByVal UpperBound As Double, _
                   ByVal MidPoint As Double)
        Me.GroupNo = GroupNumber
        Me.GroupName = GroupName
        Me.DistributionType = DistributionType
        Me.LowerBound = LowerBound
        Me.UpperBound = UpperBound
        Me.MidPoint = MidPoint
    End Sub

    Public Property GroupNo As Integer
    Public Property GroupName As String
    Public Property DistributionType As cMSE.DistributionType
    Public Property LowerBound As Double
    Public Property UpperBound As Double
    Public Property MidPoint As Double

End Class