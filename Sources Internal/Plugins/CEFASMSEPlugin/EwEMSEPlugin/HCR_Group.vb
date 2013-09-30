Imports EwECore

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

    Public Property GroupName4Biomass As String
    Public Property GroupNumber4Biomass As Integer = cCore.NULL_VALUE
    Public Property LowerLimit As Double = cCore.NULL_VALUE
    Public Property UpperLimit As Double = cCore.NULL_VALUE
    Public Property GroupName4F As String
    Public Property GroupNumber4F As Integer = cCore.NULL_VALUE
    Public Property MaxF As Double = cCore.NULL_VALUE
    Public Property CostFunction As String

    Public Overrides Function ToString() As String

        ' ToDo_JS: globalize this

        Dim tmp As String
        tmp = "Biomass Group = " + GroupName4Biomass
        tmp += Environment.NewLine + "Biomass Index = " + GroupNumber4Biomass.ToString
        tmp += Environment.NewLine + "Fishing Mort. Group = " + GroupName4F
        tmp += Environment.NewLine + "Fishing Mort. Index = " + GroupNumber4F.ToString
        Return tmp

    End Function

#End Region

#Region "Construction"

    Public Sub New(theCore As cCore)
        Me.m_core = theCore
    End Sub

#End Region

#Region "Public Methods"

    Public Shared Function toCostFunctionString(eCostFunctionTypes As eCostFunctionTypes) As String
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
        Dim breturn As Boolean = False
        Dim nl As String = Environment.NewLine
        Debug.Assert(Me.m_core IsNot Nothing, Me.ToString + ".isValid() cCore has not been set. Validation cannot be run.")

        Try
            If Me.isIndexInBounds(Me.GroupNumber4Biomass) Then
                breturn = True
            Else
                ValidationString = "Biomass group number is not valid."
            End If

            If Me.isIndexInBounds(Me.GroupNumber4F) Then
                breturn = breturn And True
            Else
                breturn = breturn And False
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


    Private Function isIndexInBounds(index As Integer) As Boolean
        If index > 0 And index < Me.m_core.nGroups Then
            If Me.m_core.EcoPathGroupInputs(index).IsFished Then
                Return True
            End If
        End If
        Return False
    End Function

#End Region

End Class
