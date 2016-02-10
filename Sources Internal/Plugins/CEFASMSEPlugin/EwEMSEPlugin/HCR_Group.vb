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
Imports System.Text
Imports EwECore
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Public Enum HCRType
    Target = 0
    Conservation = 1
End Enum

Public Class cCostFunctionTypeFormatter
    Implements ITypeFormatter

    Public Function GetDescribedType() As System.Type Implements ITypeFormatter.GetDescribedType
        Return GetType(HCRType)
    End Function

    Public Function GetDescriptor(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
        Implements ITypeFormatter.GetDescriptor

        Select Case DirectCast(value, HCRType)
            Case HCRType.Target
                Return My.Resources.COSTFUNCTION_TARGET
            Case HCRType.Conservation
                Return My.Resources.COSTFUNCTION_CONSERVATION
        End Select
        Return "?"
    End Function

End Class

''' <summary>
''' Harvest Control Rules and Strategies all need to be public so they can be accessed in the frmTFMpolicy interface.
''' </summary>
Public Class HCR_Group

#Region "Private variables"
    Private m_core As cCore
    Private m_MSE As cMSE
#End Region

#Region "Public variables and Properties"

    ''' <summary>
    ''' Biomass group.
    ''' </summary>
    Public Property GroupB As cEcoPathGroupInput = Nothing

    ''' <summary>
    ''' Fishing mortality group.
    ''' </summary>
    Public Property GroupF As cEcoPathGroupInput = Nothing

    Public Property LowerLimit As Single = cCore.NULL_VALUE
    Public Property UpperLimit As Single = cCore.NULL_VALUE
    Public Property MaxF As Single = cCore.NULL_VALUE

    Public Property TypeOfHCR As HCRType = HCRType.Target

    Public Property TimeFrameRule As cTimeFrameRule

    Public Overrides Function ToString() As String

        ' JS 01Oct13: StringBuilder is better at handling newlines on different OS-es
        Dim sb As New StringBuilder()
        Dim fmt As New cCoreInterfaceFormatter()
        Dim fmtC As New cCostFunctionTypeFormatter()

        sb.AppendLine(String.Format(My.Resources.HCR_GROUP_BIOMASS, fmt.GetDescriptor(Me.GroupB)))
        sb.AppendLine(String.Format(My.Resources.HCR_GROUP_FISHMORT, fmt.GetDescriptor(Me.GroupF)))
        sb.AppendLine(String.Format(My.Resources.HCR_GROUP_FUNCTION, fmtC.GetDescriptor(Me.TypeOfHCR)))

        Return sb.ToString

    End Function

#End Region

#Region "Construction"

    Public Sub New(theCore As cCore, MSE As cMSE)
        Me.m_core = theCore
        Me.m_MSE = MSE
        TimeFrameRule = New cTimeFrameRule(MSE.EcosimData, Me, MSE)
    End Sub

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Validate the Harvest Control Rule against the core group indexes
    ''' </summary>
    ''' <returns>True if this rule is valid. False otherwise.</returns>
    ''' <remarks></remarks>
    Public Function isValid(ByRef ValidationString As String) As Boolean

        ' ToDo_JS: Globalize this method
        Dim sb As New StringBuilder()
        Dim breturn As Boolean = True
        Debug.Assert(Me.m_core IsNot Nothing, Me.ToString + ".isValid() cCore has not been set. Validation cannot be run.")

        Try
            If Not Me.isIndexInBounds(Me.GroupB) Then
                breturn = False
                sb.AppendLine("Biomass group number is not valid.")
            End If

            If Not Me.isIndexInBounds(Me.GroupF) Then
                breturn = False
                sb.AppendLine("Fishing Mortality group number is not valid.")
            End If

        Catch ex As Exception
            breturn = False
            Debug.Assert(False, Me.ToString + ".isValid() Exception: " + ex.Message)
        End Try

        ValidationString = sb.ToString()

        Return breturn

    End Function

    Public Function CalcF(ByRef Biomass As Single(), ByRef iYearProjecting As Integer, ByVal iTimeStep As Integer) As Single

        Dim HCR_F As Single = CalcFfromHCR(Biomass, iYearProjecting)

        If TimeFrameRule.CheckValidRule(iYearProjecting, HCR_F, iTimeStep) And Me.TypeOfHCR = HCRType.Target Then 'Use a time frame rule

#If DEBUG Then
            Console.WriteLine("(HCR_Group.CalcFfromHCR) Model = " & m_MSE.CurrentModelID & "   Strategy = " & m_MSE.currentStrategy.Name & "   Group = " & Me.GroupF.Name)
#End If

            Return TimeFrameRule.F(iTimeStep, iYearProjecting, HCR_F)
        Else
            Return HCR_F
        End If


    End Function

    Public Function CalcFfromHCR(ByRef Biomass As Single(), ByRef iYearProjecting As Integer) As Single

        If Biomass(Me.GroupB.Index) > UpperLimit Then 'otherwise use the standard HCR
            Return MaxF
        ElseIf Biomass(Me.GroupB.Index) < LowerLimit Then
            Return 0
        Else
            Return ((Biomass(Me.GroupB.Index) - LowerLimit) / (UpperLimit - LowerLimit)) * MaxF
        End If

    End Function

#End Region

#Region "Private Methods"

    Private Function isIndexInBounds(group As cEcoPathGroupInput) As Boolean
        If (group Is Nothing) Then Return False
        Return group.IsFished
    End Function

#End Region

End Class
