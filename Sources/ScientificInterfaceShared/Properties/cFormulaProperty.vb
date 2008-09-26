'==============================================================================
'
' $Log: cFormulaProperty.vb,v $
' Revision 1.1  2008/09/26 07:31:22  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/06/01 23:45:45  jeroens
' Separated from Scientific Interface
'
' Revision 1.15  2008/01/15 12:08:01  jeroens
' * Eased-up multi-operand formula to handle less than one operand
'
' Revision 1.14  2007/10/22 01:02:08  jeroens
' * ValueCalculated style suppressed in Formula results
'
' Revision 1.13  2007/06/05 14:01:01  jeroens
' + Added cMultiOperation operands Avg, AvgNonZero
'
' Revision 1.12  2007/06/05 02:44:27  jeroens
' * Fixed cMultiOperation comments
' * Renamed cMultiOperation Add ->Sum
'
' Revision 1.11  2007/06/04 14:51:00  jeroens
' * Debugged new formula bits
'
' Revision 1.10  2007/06/04 01:54:41  jeroens
' + Introduced cBooleanOperand, cConditionalOperation
'
' Revision 1.9  2007/05/31 13:11:19  jeroens
' * Renamed StyleGuide StyleFlags to eStyleFlags
'
' Revision 1.8  2006/10/03 03:18:52  jeroens
' + Added option to construct with a valid custom ID
'
' Revision 1.7  2006/08/19 09:47:54  jeroens
' * Fixed byref / byval confusion
'
' Revision 1.6  2006/06/21 03:04:14  jeroens
' * Changed CType to DirectCast to gain a bit of performance
' * cFormulaProperty now excludes the style NULL since NULL expressions are already properly propagated. A NULL property won't show...
'
' Revision 1.5  2006/06/20 23:42:55  fgao
' binaryOperation bug..
'
' Revision 1.4  2006/06/14 04:17:48  cvsuser
' + JS: Style now cascades through formula
'
' Revision 1.3  2006/05/23 01:05:19  jeroens
' + Fixed Property enum access build error
'
' Revision 1.2  2006/05/22 03:23:12  jeroens
' + Added cMultiOperation
' + Changed cExpression Event to Delegate
' + Tested and ok
'
' Revision 1.1  2006/05/21 18:47:55  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports EwECore
Imports System.Globalization
Imports ScientificInterfaceShared.Style

Namespace Properties

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Implements a property that is capable of calculating simple formulas.
    ''' </summary>
    ''' <remarks>
    ''' <para>The fomulas in cFormulaProperty accept <see cref="cProperty">Properties</see>
    ''' as operands, making this contracption extremely powerful for performing spreadsheet-
    ''' like calculations. Whenever an operand in the formula changes, this property will 
    ''' automatically recalculate the formula result. The formula result is available 
    ''' through <see cref="cProperty.GetValue">cProperty.GetValue()</see>. Additionally, 
    ''' formula updates will be broadcasted through the cProperty 
    ''' <see cref="cProperty.PropertyChanged">change event</see>.</para>
    ''' </remarks>
    ''' <example>
    ''' <para>To calculate 1 / (prop1 + prop2):</para>
    ''' <code>
    ''' Dim opAdd As New cBinaryOperation(eOperatorType.Add, prop1, prop2)
    ''' Dim opDiv As New cBinaryOperation(eOperatorType.Divide, 1, opAdd)
    ''' Dim propF As New cFormulaProperty(opDiv)
    ''' </code>
    ''' <para>To calculate Sqrt((propB^2) - (4*propA*propC)) / (2*propA):</para>
    ''' <code>
    ''' Dim opB2 As New cBinaryOperation(eOperatorType.Pow, propB, 2)
    ''' Dim opAC As New cBinaryOperation(eOperatorType.Multiply, propA, propC)
    ''' Dim op4AC As New cBinaryOperation(eOperatorType.Multiply, 4, opAC)
    ''' Dim opB2_4AC As New cBinaryOperation(eOperatorType.Substract, opB2, op4AC)
    ''' Dim op2A As New cBinaryOperation(eOperatorType.Multiply, 2, propA)
    ''' Dim formula As new cBinaryOperation(eOperatorType.Divide, opB2_4AC, op2A)
    ''' Dim propF as new cFormulaProperty(formula)
    ''' </code>
    ''' </example>
    ''' -----------------------------------------------------------------------
    Public Class cFormulaProperty
        : Inherits cSingleProperty

        ''' <summary>The formula</summary>
        Private WithEvents m_formula As cExpression = Nothing

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new cFormulaProperty instance.
        ''' </summary>
        ''' <param name="formula">The formula that will feed the value and
        ''' status of this <see cref="cProperty">Property</see>.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal formula As cExpression)
            Me.New(String.Empty, formula)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new cFormulaProperty instance.
        ''' </summary>
        ''' <param name="strID">The ID to assign to this property.</param>
        ''' <param name="formula">The formula that will feed the value and
        ''' status of this <see cref="cProperty">Property</see>.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal strID As String, ByVal formula As cExpression)
            MyBase.New(strID)
            ' Sanity check
            Debug.Assert(formula IsNot Nothing, "Need valid formula")
            ' Store formula
            Me.m_formula = formula
            ' Listen to formula changes
            AddHandler m_formula.OnValueChanged, AddressOf OnFormulaChanged
            ' Initialize value
            Me.Calculate()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Attempt to get a valid <see cref="cExpression">cExpression</see> 
        ''' for a provided operand.
        ''' </summary>
        ''' <param name="operand">The operand to analyze.</param>
        ''' <returns>A valid <see cref="cExpression">cExpression</see>, or 
        ''' Nothing if the conversion could not be made.</returns>
        ''' <remarks>Accepted operand types are numerical values, 
        ''' <see cref="cSingleProperty">cSingleProperty</see> instances,
        ''' or <see cref="cExpression">cExpression-derived</see> objects.</remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function GetExpression(ByVal operand As Object) As cExpression

            Dim s As Single = 0.0

            Debug.Assert(operand IsNot Nothing, "Operand is NULL, cannot proceed")

            ' Is operand already an Expression?
            If TypeOf (operand) Is cExpression Then
                ' #Yes: return type-casted operand
                Return DirectCast(operand, cExpression)
            End If

            ' Is operand a SingleProperty?
            If (TypeOf (operand) Is cSingleProperty) Then
                ' #Yes: wrap operand in a cPropertyOperand
                Return New cPropertyOperand(DirectCast(operand, cSingleProperty))
            End If

            ' Is operand a BooleanProperty?
            If (TypeOf (operand) Is cBooleanProperty) Then
                ' #Yes: wrap operand in a cPropertyOperand
                Return New cPropertyOperand(DirectCast(operand, cBooleanProperty))
            End If

            ' Is operand a boolean?
            If (TypeOf (operand) Is Boolean) Then
                Return New cStaticOperand(CSng(operand))
            End If

            ' Is operand convertable into a Single value?
            If Single.TryParse(operand.ToString(), Globalization.NumberStyles.Any, NumberFormatInfo.InvariantInfo, s) Then
                ' #Yes: wrap operand in a cStaticOperand
                Return New cStaticOperand(s)
            End If

            ' Unable to convert or wrap operand into an Expression: we're out of options
            Debug.Assert(False, String.Format("Unable to wrap or convert operand {0} of type {1}", operand.ToString(), operand.GetType()))

            ' Return failure
            Return Nothing

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Calculates the formula result.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub Calculate()

            Dim styleSum As StyleGuide.eStyleFlags = (StyleGuide.eStyleFlags.Sum Or StyleGuide.eStyleFlags.NotEditable)
            Dim cf As cProperty.eChangeFlags = 0
            Dim sValue As Single = 0.0
            Dim style As StyleGuide.eStyleFlags = StyleGuide.eStyleFlags.OK

            Try
                ' Try to calculate formula outcome
                sValue = Me.m_formula.GetValue()
                ' Try to calculate formula style
                styleSum = (styleSum Or Me.m_formula.GetStyle())

            Catch ex As Exception
                ' Woops, something went wrong. For now, do not try to discover the error, just flag
                ' the Property value as erroneous
                styleSum = styleSum Or StyleGuide.eStyleFlags.ErrorEncountered
                ' Reset the value
                sValue = 0.0
            End Try

            ' Update style without notifying anyone
            ' - Some core states are suppressed, such as NULL, Remarks, and ValueComputed
            If (Me.SetStyle(styleSum And Not (StyleGuide.eStyleFlags.Remarks Or StyleGuide.eStyleFlags.Null Or StyleGuide.eStyleFlags.ValueComputed), TriState.False)) Then
                cf = cf Or eChangeFlags.CoreStatus
            End If

            ' Update value without notifying anyone
            If (Me.SetValue(sValue, TriState.False)) Then
                cf = cf Or eChangeFlags.Value
            End If

            ' Anything changed?
            If (cf = 0) Then
                ' #No: done
                Return
            End If

            ' Fire change notification
            Me.FireChangeNotification(cf)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, responds to operand value change events by recalculating
        ''' the formula result.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnFormulaChanged(ByVal formula As cExpression)
            Me.Calculate()
        End Sub

    End Class

#Region " Expressions "

#Region " cExpression (base class) "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Base class for a cFormulaProperty formula
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public MustInherit Class cExpression

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the value of this expression
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public MustOverride Function GetValue() As Single
        Public MustOverride Function GetStyle() As StyleGuide.eStyleFlags

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Change notification event that must be fired when
        ''' the value of this expression has changed.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Delegate Sub ValueChangedEventHandler(ByVal exp As cExpression)
        Public Event OnValueChanged As ValueChangedEventHandler

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Fire the change event for this expression.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Protected Sub FireChangeNotification()
            RaiseEvent OnValueChanged(Me)
        End Sub

    End Class

#End Region ' cExpression (base class) 

#Region " cStaticOperand "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' A numerical expression that does not change value
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class cStaticOperand
        : Inherits cExpression

        ''' <summary>The constant value of this expression</summary>
        Private m_sValue As Single

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="s">The value of this expression</param>
        ''' ---------------------------------------------------------------
        Public Sub New(ByVal s As Single)
            Me.m_sValue = s
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the value of this expression
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetValue() As Single
            Return Me.m_sValue
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the static <see cref="StyleGuide.eStyleFlags">style</see>
        ''' of this expression.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetStyle() As StyleGuide.eStyleFlags
            Return StyleGuide.eStyleFlags.OK
        End Function

    End Class

#End Region ' cConstantValue 

#Region " cPropertyOperand "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' A numerical expression that derives its value from a <see cref="cSingleProperty">cSingleProperty</see>.
    ''' </summary>
    ''' <remarks>
    ''' This expression monitors its property for value changes, and will broadcast a change if such an
    ''' event occurs.
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Public Class cPropertyOperand
        : Inherits cExpression

        ''' <summary>The <see cref="cProperty">cProperty</see> to observe.</summary>
        Private m_prop As cProperty = Nothing

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new cPropertyOperand
        ''' </summary>
        ''' <param name="prop">The <see cref="cSingleProperty">cSingleProperty</see> to observe.</param>
        ''' ---------------------------------------------------------------
        Public Sub New(ByVal prop As cSingleProperty)
            ' Store property
            Me.m_prop = prop
            ' Start listening to property events
            AddHandler prop.PropertyChanged, AddressOf onPropertyChanged
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new cPropertyOperand
        ''' </summary>
        ''' <param name="prop">The <see cref="cBooleanProperty">cBooleanProperty</see> to observe.</param>
        ''' ---------------------------------------------------------------
        Public Sub New(ByVal prop As cBooleanProperty)
            ' Store property
            Me.m_prop = prop
            ' Start listening to property events
            AddHandler prop.PropertyChanged, AddressOf onPropertyChanged
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Destructor
        ''' </summary>
        ''' ---------------------------------------------------------------
        Protected Overrides Sub Finalize()
            ' Stop listening to property events
            RemoveHandler Me.m_prop.PropertyChanged, AddressOf onPropertyChanged
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the value of the <see cref="cSingleProperty">cSingleProperty</see>.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetValue() As Single
            Return CSng(Me.m_prop.GetValue())
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the <see cref="StyleGuide.eStyleFlags">style</see>
        ''' of the <see cref="cSingleProperty">cSingleProperty</see>.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetStyle() As StyleGuide.eStyleFlags
            Return Me.m_prop.GetStyle()
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Event handler; filters property change events for value changes.
        ''' </summary>
        ''' <param name="prop">The property that changed.</param>
        ''' <param name="changeFlag">Information on what changed.</param>
        ''' ---------------------------------------------------------------
        Public Sub onPropertyChanged(ByVal prop As cProperty, ByVal changeFlag As cProperty.eChangeFlags)
            ' Is this a value or status change?
            If (changeFlag And (cProperty.eChangeFlags.Value Or cProperty.eChangeFlags.CoreStatus)) <> 0 Then
                ' #Yes: that's for us. Fire a change.
                Me.FireChangeNotification()
            End If
        End Sub

    End Class

#End Region ' cPropertyOperand 

#Region " cUnaryOperation "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Implements an expression as an unary operation, i.e. an
    ''' arithmetical computation on one numeral.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class cUnaryOperation
        : Inherits cExpression

        ''' ---------------------------------------------------------------
        ''' <summary>Supported arithmetical operations.</summary>
        ''' <remarks>Extend this enumberated type with any Unary operation you need. 
        ''' Just don't forget to add the implementation in <see cref="CalcValue">CalcValue</see>.
        ''' </remarks>
        ''' ---------------------------------------------------------------
        Public Enum eOperatorType
            ''' <summary>Returns the quare root of the operand.</summary>
            Sqrt
            ''' <summary>Returns the sine of the angle specified in operand.</summary>
            Sin
            ''' <summary>Returns the inverse sine of the number [-1, 1] specified in operand.</summary>
            Asin
            ''' <summary>Returns the cosine of the angle specified in operand.</summary>
            Cos
            ''' <summary>Returns the inverse cosine of the number [-1, 1] specified in operand.</summary>
            Acos
            ''' <summary>Returns the tangent of the angle specified in operand.</summary>
            Tan
            ''' <summary>Returns the inverse tangent of the number [-1, 1] specified in operand.</summary>
            Atan
            ''' <summary>Returns operand rounded-up.</summary>
            Ceil
            ''' <summary>Returns operand rounded-down.</summary>
            Floor
            ''' <summary>Returns the operand rounded to the nearest whole number.</summary>
            Round
            ''' <summary>Returns the sign of operand.</summary>
            Sign
            ''' <summary>Returns the absolute value of operand.</summary>
            Abs
            ''' <summary>Returns the natural (base-e) logarithm of operand.</summary>
            Log
            ''' <summary>Returns the base-10 logarithm of operand.</summary>
            Log10

        End Enum

        ''' <summary>Unary operator to perform.</summary>
        Private m_nOperator As eOperatorType = 0
        ''' <summary>Operand to perform operator onto.</summary>
        Private WithEvents m_operand As cExpression = Nothing
        ''' <summary>Cached calcuated value</summary>
        Private m_sValue As Single = 0.0
        ''' <summary>Cached style</summary>
        Private m_style As StyleGuide.eStyleFlags = StyleGuide.eStyleFlags.OK

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new cUnaryOperation
        ''' </summary>
        ''' <param name="nOperator"><see cref="eOperatorType">Operator</see> to perform.</param>
        ''' <param name="operand">Operand to perform operator onto.</param>
        ''' <remarks>For supported operand types, see <see cref="cFormulaProperty.GetExpression">cFormulaProperty.GetExpression</see>.</remarks>
        ''' ---------------------------------------------------------------
        Public Sub New(ByVal nOperator As eOperatorType, ByVal operand As Object)
            Me.m_nOperator = nOperator
            Me.m_operand = cFormulaProperty.GetExpression(operand)
            Me.m_sValue = Me.CalcValue()
            Me.m_style = Me.CalcStyle()
            ' Start listening for operand changes
            AddHandler Me.m_operand.OnValueChanged, AddressOf OnOperandValueChanged
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the outcome of the operation.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetValue() As Single
            Return Me.m_sValue
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the <see cref="StyleGuide.eStyleFlags">style</see>
        ''' of the operation.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetStyle() As StyleGuide.eStyleFlags
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
            Dim sVal As Single = Me.CalcValue()
            Dim style As StyleGuide.eStyleFlags = Me.CalcStyle()
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
        Private Function CalcValue() As Single
            Dim s1 As Single = Me.m_operand.GetValue()
            Dim s As Single = 0

            Me.m_style = Me.m_operand.GetStyle()

            Select Case Me.m_nOperator
                Case eOperatorType.Sqrt
                    s = CSng(Math.Sqrt(s1))
                Case eOperatorType.Sin
                    s = CSng(Math.Sin(s1))
                Case eOperatorType.Asin
                    s = CSng(Math.Asin(s1))
                Case eOperatorType.Cos
                    s = CSng(Math.Cos(s1))
                Case eOperatorType.Acos
                    s = CSng(Math.Acos(s1))
                Case eOperatorType.Tan
                    s = CSng(Math.Tan(s1))
                Case eOperatorType.Atan
                    s = CSng(Math.Atan(s1))
                Case eOperatorType.Ceil
                    s = CSng(Math.Ceiling(s1))
                Case eOperatorType.Floor
                    s = CSng(Math.Floor(s1))
                Case eOperatorType.Round
                    s = CSng(Math.Round(s1))
                Case eOperatorType.Sign
                    s = CSng(Math.Sign(s1))
                Case eOperatorType.Abs
                    s = CSng(Math.Abs(s1))
                Case eOperatorType.Log
                    s = CSng(Math.Log(s1))
                Case eOperatorType.Log10
                    s = CSng(Math.Log10(s1))
                Case Else
                    Debug.Assert(False, String.Format("Operator {0} not implemented", Me.m_nOperator))
            End Select
            Return s
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Recalculate the style of the operation.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Private Function CalcStyle() As StyleGuide.eStyleFlags
            Return Me.m_operand.GetStyle()
        End Function

    End Class

#End Region ' cUnaryOperation 

#Region " cBinaryOperation "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Implements an expression as a binary operation, i.e. an
    ''' arithmetical computation on two numerals.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class cBinaryOperation
        : Inherits cExpression

        ''' ---------------------------------------------------------------
        ''' <summary>Supported arithmetical operations</summary>
        ''' ---------------------------------------------------------------
        Public Enum eOperatorType
            ''' <summary>Adds operand 1 to operand 2</summary>
            Add
            ''' <summary>Substracts operand 2 from operand 1</summary>
            Substract
            ''' <summary>Multiplies operand 1 with operand 2</summary>
            Multiply
            ''' <summary>Divides operand 1 by operand 2</summary>
            Divide
            ''' <summary>Raises operand 1 to the power of operand 2</summary>
            Pow
            ''' <summary>Returns the larger of 2 operands</summary>
            Max
            ''' <summary>Returns the lower of 2 operands</summary>
            Min
        End Enum

        ''' <summary>Operator to perform</summary>
        Private m_nOperator As eOperatorType = 0
        ''' <summary>First operand</summary>
        Private WithEvents m_operand1 As cExpression = Nothing
        ''' <summary>Second operand</summary>
        Private WithEvents m_operand2 As cExpression = Nothing
        ''' <summary>Cached value</summary>
        Private m_sValue As Single = 0.0
        ''' <summary>Cached style</summary>
        Private m_style As StyleGuide.eStyleFlags = StyleGuide.eStyleFlags.OK

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new cBinaryOperation. 
        ''' </summary>
        ''' <param name="nOperator"><see cref="eOperatorType">Operator</see> to perform.</param>
        ''' <param name="operand1">First operand (left side of operator).</param>
        ''' <param name="operand2">Second operand (right side of operator).</param>
        ''' <remarks>For supported operand types, see <see cref="cFormulaProperty.GetExpression">cFormulaProperty.GetExpression</see>.</remarks>
        ''' ---------------------------------------------------------------
        Public Sub New(ByVal nOperator As eOperatorType, ByVal operand1 As Object, ByVal operand2 As Object)
            Me.m_nOperator = nOperator
            Me.m_operand1 = cFormulaProperty.GetExpression(operand1)
            Me.m_operand2 = cFormulaProperty.GetExpression(operand2)
            Me.m_sValue = Me.CalcValue()
            Me.m_style = Me.CalcStyle()
            ' Start listening for operand changes
            AddHandler Me.m_operand1.OnValueChanged, AddressOf OnOperandValueChanged
            AddHandler Me.m_operand2.OnValueChanged, AddressOf OnOperandValueChanged
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the outcome of the operation.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetValue() As Single
            Return Me.m_sValue
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the <see cref="StyleGuide.eStyleFlags">style</see>
        ''' of the operation.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetStyle() As StyleGuide.eStyleFlags
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
            Dim sVal As Single = Me.CalcValue()
            Dim style As StyleGuide.eStyleFlags = Me.CalcStyle()
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
        Private Function CalcValue() As Single
            Dim s1 As Single = Me.m_operand1.GetValue()
            Dim s2 As Single = Me.m_operand2.GetValue()
            Dim s As Single = 0

            Select Case Me.m_nOperator
                Case eOperatorType.Add
                    s = s1 + s2
                Case eOperatorType.Substract
                    s = s1 - s2
                Case eOperatorType.Divide
                    s = s1 / s2
                Case eOperatorType.Multiply
                    s = s1 * s2
                Case eOperatorType.Pow
                    s = CSng(Math.Pow(s1, s2))
                Case eOperatorType.Max
                    s = CSng(Math.Max(1, s2))
                Case eOperatorType.Min
                    s = CSng(Math.Min(s1, s2))
                Case Else
                    Debug.Assert(False, String.Format("Operator {0} not implemented", Me.m_nOperator))
            End Select

            Return s
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Recalculate the <see cref="StyleGuide.eStyleFlags">style</see>
        ''' of the operation.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Private Function CalcStyle() As StyleGuide.eStyleFlags
            Return (Me.m_operand1.GetStyle() Or Me.m_operand2.GetStyle())
        End Function

    End Class

#End Region ' cBinaryOperation 

#Region " cMultiOperation "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Implements an expression as a K-ary operation, i.e. an
    ''' arithmetical computation on any K number of parameters where K >= 1.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class cMultiOperation
        : Inherits cExpression

        ''' ---------------------------------------------------------------
        ''' <summary>Supported arithmetical operations.</summary>
        ''' ---------------------------------------------------------------
        Public Enum eOperatorType
            ''' <summary>Sums all operands.</summary>
            Sum
            ''' <summary>Multiplies all operands.</summary>
            Multiply
            ''' <summary>Returns the larger of all operands.</summary>
            Max
            ''' <summary>Returns the lower of all operands.</summary>
            Min
            ''' <summary>Returns the average of all operands.</summary>
            Avg
            ''' <summary>Returns the average of all operands that are not 0.</summary>
            AvgNonZero
        End Enum

        ''' <summary>Operator to perform</summary>
        Private m_nOperator As eOperatorType = 0
        ''' <summary>Operands</summary>
        Private m_lOperands As New List(Of cExpression)
        ''' <summary>Cached value</summary>
        Private m_sValue As Single = 0.0
        ''' <summary>Cached style</summary>
        Private m_style As StyleGuide.eStyleFlags = StyleGuide.eStyleFlags.OK

        Private Delegate Sub OnChanged()

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new cMultiOperation. 
        ''' </summary>
        ''' <param name="nOperator"><see cref="eOperatorType">Operator</see> to perform.</param>
        ''' <param name="aOperands">Array of operands.</param>
        ''' <remarks>For supported operand types, see <see cref="cFormulaProperty.GetExpression">cFormulaProperty.GetExpression</see>.</remarks>
        ''' ---------------------------------------------------------------
        Public Sub New(ByVal nOperator As eOperatorType, ByVal aOperands() As Object)
            ' Store operator
            Me.m_nOperator = nOperator
            ' For each operand
            For nOperand As Integer = 0 To aOperands.Length - 1
                ' Resolve expression
                Dim operand As cExpression = cFormulaProperty.GetExpression(aOperands(nOperand))
                ' Add to private list of operands
                Me.m_lOperands.Add(operand)
                ' Listen to event
                AddHandler operand.OnValueChanged, AddressOf Me.OnOperandValueChanged
            Next
            ' Update value
            Me.m_sValue = Me.CalcValue()
            ' Update style
            Me.m_style = Me.CalcStyle()
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Destructor
        ''' </summary>
        ''' ---------------------------------------------------------------
        Protected Overrides Sub Finalize()
            ' For each operand
            For nOperand As Integer = 0 To Me.m_lOperands.Count - 1
                ' Get it
                Dim operand As cExpression = Me.m_lOperands(nOperand)
                ' Stop listening to its events
                RemoveHandler operand.OnValueChanged, AddressOf OnOperandValueChanged
            Next
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the outcome of the operation.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetValue() As Single
            Return Me.m_sValue
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the <see cref="StyleGuide.eStyleFlags">style</see>
        ''' of the operation.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetStyle() As StyleGuide.eStyleFlags
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
            Dim sVal As Single = Me.CalcValue()
            Dim style As StyleGuide.eStyleFlags = Me.CalcStyle()
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
        Private Function CalcValue() As Single
            Dim operand As cExpression = Nothing
            Dim iHitCount As Integer = 0
            Dim s As Single = 0.0

            For nOperand As Integer = 0 To Me.m_lOperands.Count - 1
                ' Get operand
                operand = Me.m_lOperands(nOperand)
                ' Apply operator
                Select Case Me.m_nOperator
                    Case eOperatorType.Sum
                        s += operand.GetValue()
                    Case eOperatorType.Multiply
                        If nOperand = 0 Then s = operand.GetValue() Else s *= operand.GetValue()
                    Case eOperatorType.Max
                        If nOperand = 0 Then s = operand.GetValue() Else s = CSng(Math.Max(s, operand.GetValue()))
                    Case eOperatorType.Min
                        If nOperand = 0 Then s = operand.GetValue() Else s = CSng(Math.Min(s, operand.GetValue()))
                    Case eOperatorType.Avg
                        s += operand.GetValue() : iHitCount += 1
                    Case eOperatorType.AvgNonZero
                        If operand.GetValue() <> 0.0 Then s += operand.GetValue() : iHitCount += 1
                    Case Else
                        Debug.Assert(False, String.Format("Operator {0} not implemented", Me.m_nOperator))
                End Select
            Next

            ' Post-process
            Select Case Me.m_nOperator
                Case eOperatorType.Avg, eOperatorType.AvgNonZero
                    ' JS 15jan08: prevent crash
                    If (iHitCount <> 0) Then s /= iHitCount
            End Select

            Return s
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Combine the style of all operands.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Private Function CalcStyle() As StyleGuide.eStyleFlags

            Dim operand As cExpression = Nothing
            Dim s As StyleGuide.eStyleFlags = StyleGuide.eStyleFlags.OK

            For nOperand As Integer = 0 To Me.m_lOperands.Count - 1
                ' Get operand
                operand = Me.m_lOperands(nOperand)
                ' Is this the first operand?
                If nOperand = 0 Then
                    ' #Yes: copy operand style
                    s = operand.GetStyle()
                Else
                    ' #No: combine with operand style
                    s = s Or operand.GetStyle()
                End If
            Next
            Return s

        End Function

    End Class

#End Region ' cMultiOperation

#Region " cBooleanOperand "

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
        Private m_style As StyleGuide.eStyleFlags = StyleGuide.eStyleFlags.OK

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
        ''' Returns the <see cref="StyleGuide.eStyleFlags">style</see>
        ''' of the operation.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetStyle() As StyleGuide.eStyleFlags
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
            Dim style As StyleGuide.eStyleFlags = StyleGuide.eStyleFlags.OK

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
        Private Sub CalcValueAndStyle(ByRef sVal As Single, ByRef style As StyleGuide.eStyleFlags)
            Dim s1 As Single = Me.m_operand1.GetValue()
            Dim s2 As Single = Me.m_operand2.GetValue()

            Try
                sVal = CSng(Me.m_nOperator.Compare(s1, s2))
                style = StyleGuide.eStyleFlags.OK
            Catch ex As Exception
                style = StyleGuide.eStyleFlags.Null
            End Try
        End Sub

    End Class

#End Region ' cBooleanOperand

#Region " cConditionalOperation "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' <para>
    ''' Returns one of two operands, depending on the evaluation of a test operand.
    ''' </para>
    ''' <para>This operation behaves exactly like <see cref="IIf">IIf</see>, other 
    ''' than expression responds to live data changes of all three parameters.</para>
    ''' </summary>
    ''' <remarks>
    ''' This expression monitors its property for value changes, and will broadcast 
    ''' a change event if this occurs.
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Public Class cConditionalOperation
        : Inherits cExpression

        ''' <summary>The boolean test operand that will determine the outcome of this operation.</summary>
        Private WithEvents m_opTest As cBooleanOperand = Nothing
        ''' <summary>The operand that will be returned when the expression evaluates to True.</summary>
        Private WithEvents m_opTrue As cExpression = Nothing
        ''' <summary>The operand that will be returned when the expression evaluates to False.</summary>
        Private WithEvents m_opFalse As cExpression = Nothing
        ''' <summary>Cached calcuated value</summary>
        Private m_sValue As Single = 0.0
        ''' <summary>Cached style</summary>
        Private m_style As StyleGuide.eStyleFlags = StyleGuide.eStyleFlags.OK

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new cConditionalOperation.
        ''' </summary>
        ''' <param name="opTest">The <see cref="cBooleanOperand">test operand</see>
        ''' that determines which operand will be returned.
        ''' </param>
        ''' <param name="opTrue">The operand that will be returned when the expression evaluates to True.</param>
        ''' <param name="opFalse">The operand that will be returned when the expression evaluates to False.</param>
        ''' <remarks>
        ''' <para>Evaluation of <paramref name="opTest">opTest</paramref> will 
        ''' result in the following:</para>
        ''' <list type="bullet">
        ''' <item>When True, <paramref name="opTrue">opTrue</paramref> will deliver the
        ''' value for this expression.</item>
        ''' <item>When False, <paramref name="opFalse">opFalse</paramref> will deliver the
        ''' value for this expression.</item>
        ''' </list>
        ''' </remarks>
        ''' ---------------------------------------------------------------
        Public Sub New(ByVal opTest As cBooleanOperand, ByVal opTrue As Object, ByVal opFalse As Object)
            ' Store bits
            Me.m_opTest = opTest
            Me.m_opTrue = cFormulaProperty.GetExpression(opTrue)
            Me.m_opFalse = cFormulaProperty.GetExpression(opFalse)
            ' Initialize contents
            Me.m_sValue = Me.CalcValue()
            Me.m_style = Me.CalcStyle()
            ' Start listening for operand changes
            AddHandler Me.m_opTest.OnValueChanged, AddressOf OnOperandValueChanged
            AddHandler Me.m_opTrue.OnValueChanged, AddressOf OnOperandValueChanged
            AddHandler Me.m_opFalse.OnValueChanged, AddressOf OnOperandValueChanged
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the outcome of the operation.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetValue() As Single
            Return Me.m_sValue
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns the <see cref="StyleGuide.eStyleFlags">style</see>
        ''' of the operation.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetStyle() As StyleGuide.eStyleFlags
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
            Dim sVal As Single = Me.CalcValue()
            Dim style As StyleGuide.eStyleFlags = Me.CalcStyle()
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
        Private Function CalcValue() As Single
            If Me.m_opTest.GetValue() <> CSng(False) Then
                Return Me.m_opTrue.GetValue()
            Else
                Return Me.m_opFalse.GetValue()
            End If
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Recalculate the style of the operation.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Private Function CalcStyle() As StyleGuide.eStyleFlags
            If CBool(Me.m_opTest.GetValue) Then
                Return Me.m_opTrue.GetStyle()
            Else
                Return Me.m_opFalse.GetStyle()
            End If
        End Function

    End Class

#End Region ' cConditionalOperation

#End Region ' Expressions

End Namespace
