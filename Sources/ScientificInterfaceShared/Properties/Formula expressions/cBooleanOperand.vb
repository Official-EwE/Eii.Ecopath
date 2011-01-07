#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Properties

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Implements an expression as a boolean test of two single operands,
    ''' compared via a given operator.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class cBooleanOperand
        : Inherits cExpression

        ''' <summary>Operator to perform</summary>
        Private m_nOperator As EwECore.cOperatorBase = Nothing
        ''' <summary>First operand</summary>
        Private WithEvents m_operand1 As cExpression = Nothing
        ''' <summary>Second operand</summary>
        Private WithEvents m_operand2 As cExpression = Nothing
        ''' <summary>Cached value</summary>
        Private m_sValue As Single = 0.0
        ''' <summary>Cached style</summary>
        Private m_style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new cBooleanOperand. 
        ''' </summary>
        ''' <param name="op"><see cref="cOperatorBase">Operator</see> to perform.</param>
        ''' <param name="operand1">First operand (left side of operator).</param>
        ''' <param name="operand2">Second operand (right side of operator).</param>
        ''' <remarks>For supported operand types, see <see cref="cFormulaProperty.GetExpression">cFormulaProperty.GetExpression</see>.</remarks>
        ''' ---------------------------------------------------------------
        Public Sub New(ByVal op As cOperatorBase, ByVal operand1 As Object, ByVal operand2 As Object)
            ' Remember. Remember, my son!
            Me.m_nOperator = op
            Me.m_operand1 = cFormulaProperty.GetExpression(operand1)
            Me.m_operand2 = cFormulaProperty.GetExpression(operand2)
            ' Initialize cached content
            Me.CalcValueAndStyle(Me.m_sValue, Me.m_style)
            ' Start listening for operand changes
            AddHandler Me.m_operand1.OnValueChanged, AddressOf OnOperandValueChanged
            AddHandler Me.m_operand2.OnValueChanged, AddressOf OnOperandValueChanged
        End Sub

        Protected Overrides Sub Dispose(ByVal bDisposing As Boolean)
            ' Stop listening for operand changes
            RemoveHandler Me.m_operand1.OnValueChanged, AddressOf OnOperandValueChanged
            RemoveHandler Me.m_operand2.OnValueChanged, AddressOf OnOperandValueChanged
            ' Clean up
            Me.m_operand1.Dispose()
            Me.m_operand2.Dispose()
            Me.m_operand1 = Nothing
            Me.m_operand2 = Nothing
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the outcome of the operation.
        ''' </summary>
        ''' <remarks>
        ''' The single value returned here contains the boolean outcome
        ''' ([Operand1] [Operator] [Operand2]) = True
        ''' </remarks>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetValue() As Single
            Return Me.m_sValue
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the <see cref="cStyleGuide.eStyleFlags">style</see>
        ''' of the operation.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetStyle() As cStyleGuide.eStyleFlags
            Return Me.m_style
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Event handler, responds to operand change events by recalculating
        ''' the outcome of the operation.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Private Sub OnOperandValueChanged(ByVal exp As cExpression)
            ' Calc new value and style
            Dim sVal As Single = 0.0
            Dim style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK

            Me.CalcValueAndStyle(sVal, style)

            ' Changes?
            If ((sVal <> Me.m_sValue) Or (Me.m_style <> style)) Then
                ' #Yes: set new value and style
                Me.m_sValue = sVal
                Me.m_style = style

                ' Broadcast change notification
                FireChangeNotification()
            End If
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Recalculate the outcome of the operation.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Private Sub CalcValueAndStyle(ByRef sVal As Single, ByRef style As cStyleGuide.eStyleFlags)
            Dim s1 As Single = Me.m_operand1.GetValue()
            Dim s2 As Single = Me.m_operand2.GetValue()

            Try
                sVal = CSng(Me.m_nOperator.Compare(s1, s2))
                style = cStyleGuide.eStyleFlags.OK
            Catch ex As Exception
                style = cStyleGuide.eStyleFlags.Null
            End Try
        End Sub

    End Class

End Namespace
