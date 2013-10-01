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

Option Strict On
Imports EwECore
Imports System.Text
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Enum HCRType
    Target = 0
    Conservation = 1
End Enum


''' <summary>
''' Harvest Control Rules and Strategies all need to be public so they can be accessed in the frmTFMpolicy interface.
''' </summary>
Public Class HCR_Group

#Region "Private variables"
    Private m_core As cCore
#End Region

#Region "Public variables and Properties"

    Public Property GroupB As cEcoPathGroupInput = Nothing
    Public Property GroupF As cEcoPathGroupInput = Nothing

    Public Property LowerLimit As Double = cCore.NULL_VALUE
    Public Property UpperLimit As Double = cCore.NULL_VALUE
    Public Property MaxF As Double = cCore.NULL_VALUE

    Public Property CostFunction As eCostFunctionTypes

    <Obsolete("Use GroupB instead")> _
    Public Property GroupName4Biomass As String
    <Obsolete("Use GroupB instead")> _
    Public Property GroupNumber4Biomass As Integer = cCore.NULL_VALUE
    <Obsolete("Use GroupF instead")> _
    Public Property GroupName4F As String
    <Obsolete("Use GroupF instead")> _
    Public Property GroupNumber4F As Integer = cCore.NULL_VALUE
    <Obsolete("Use CostFunction instead")> _
    Public Property CostFunctionOrg As String

    Public Overrides Function ToString() As String

        ' JS 01Oct13: StringBuilder is better at handling newlines on different OS-es
        Dim sb As New StringBuilder()
        Dim fmt As New cCoreInterfaceFormatter()

        sb.Append(String.Format(My.Resources.HCR_GROUP_BIOMASS, fmt.GetDescriptor(GroupB)))
        sb.AppendLine(String.Format(My.Resources.HCR_GROUP_FISHMORT, fmt.GetDescriptor(GroupF)))

        Return sb.ToString

    End Function

#End Region

#Region "Construction"

    Public Sub New(theCore As cCore)
        Me.m_core = theCore
    End Sub

#End Region

#Region "Public Methods"

    Public Shared Function toCostFunctionString(eCostFunctionTypes As eCostFunctionTypes) As String

        ' ToDo_JS: Globalize this method

        Select Case eCostFunctionTypes
            Case EwEMSEPlugin.eCostFunctionTypes.Target
                Return "Target"
            Case EwEMSEPlugin.eCostFunctionTypes.Conservation
                Return "Conservation"
        End Select
        Return "Target"
    End Function

    Public Shared Function toCostFunctionEnum(CostFunctionString As String) As eCostFunctionTypes
        'ToDo this should be handled by the HarvestRule
        If String.Compare(CostFunctionString, "Target") = 0 Then
            Return eCostFunctionTypes.Target
        ElseIf String.Compare(CostFunctionString, "Conservation") = 0 Then
            Return eCostFunctionTypes.Conservation
        End If
        Return eCostFunctionTypes.Target
    End Function

    ''' <summary>
    ''' Validate the Harvest Control Rule against the core group indexes
    ''' </summary>
    ''' <returns>True if this rule is valid. False otherwise.</returns>
    ''' <remarks></remarks>
    Public Function isValid(ByRef ValidationString As String) As Boolean

        ' ToDo_JS: Globalize this method

        Dim breturn As Boolean = False
        Dim nl As String = Environment.NewLine
        Debug.Assert(Me.m_core IsNot Nothing, Me.ToString + ".isValid() cCore has not been set. Validation cannot be run.")

        Try
            If Me.isIndexInBounds(Me.GroupB) Then
                breturn = True
            Else
                ValidationString = "Biomass group number is not valid."
            End If

            If Me.isIndexInBounds(Me.GroupF) Then
                breturn = breturn And True
            Else
                breturn = False
                Dim tmp As String = "Fishing Mortality group number is not valid."
                If String.IsNullOrEmpty(ValidationString) Then
                    ValidationString = tmp
                Else
                    ValidationString += nl + tmp
                End If
            End If

        Catch ex As Exception
            breturn = False
            Debug.Assert(False, Me.ToString + ".isValid() Exception: " + ex.Message)
        End Try

        Return breturn

    End Function


    Private Function isIndexInBounds(group As cEcoPathGroupInput) As Boolean
        If (group Is Nothing) Then Return False
        Return group.IsFished
    End Function

#End Region

End Class
