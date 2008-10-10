'==============================================================================
'
' $Log: ucBrushPicker.vb,v $
' Revision 1.2  2008/10/10 20:00:55  jeroens
' Revamped slider
'
' Revision 1.1  2008/06/01 23:45:06  jeroens
' Separated from Scientific Interface
'
' Revision 1.5  2008/04/08 16:04:44  jeroens
' Set nud increment
'
' Revision 1.4  2008/03/28 23:45:09  jeroens
' Added enabling of value field
' Value field shows integer or single based on specified value type
'
' Revision 1.3  2007/09/26 16:22:01  jeroens
' * Resized
' + Responds to style guide
'
' Revision 1.2  2007/09/26 14:36:08  jeroens
' + Fixed comment
'
' Revision 1.1  2007/09/26 14:34:40  jeroens
' Initial version
'
'==============================================================================

#Region " Imports directive "

Option Strict On
Imports System.Drawing.Drawing2D
Imports ScientificInterfaceShared.Style

#End Region ' Imports directive

''' ---------------------------------------------------------------------------
''' <summary>
''' GUI control that provides an interface to select a brush size and a value,
''' intended to be used when drawing.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class ucBrushPicker

#Region " Privates "

    ''' <summary>StyleGuide to track for number of digits formatting.</summary>
    Private m_sg As StyleGuide = Nothing

#End Region ' Privates

#Region " Public properties "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Public event to subscribe to for receiving brush change notifications.
    ''' </summary>
    ''' <param name="iSize">The picked brush size.</param>
    ''' <param name="sValue">The picked brush value.</param>
    ''' -----------------------------------------------------------------------
    Public Event OnBrushPicked(ByVal iSize As Integer, ByVal sValue As Single)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get or set the minimum size for the brush.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BrushMinSize() As Integer
        Get
            Return Me.tbValue.Minimum
        End Get
        Set(ByVal value As Integer)
            Me.tbValue.Minimum = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get or set the maximum size for the brush.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BrushMaxSize() As Integer
        Get
            Return Me.tbValue.Maximum
        End Get
        Set(ByVal value As Integer)
            Me.tbValue.Maximum = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get or set the selected brush size.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BrushSize() As Integer
        Get
            Return Me.tbValue.Value
        End Get
        Set(ByVal value As Integer)
            Me.tbValue.Value = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get or set the minimum value for the brush.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BrushMinValue() As Single
        Get
            Return Convert.ToSingle(Me.nudValue.Minimum)
        End Get
        Set(ByVal value As Single)
            value = CSng(Math.Max(Math.Min(value, CSng(Decimal.MaxValue)), CSng(Decimal.MinValue)))
            Me.nudValue.Minimum = Convert.ToDecimal(value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get or set the maximum value for the brush.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BrushMaxValue() As Single
        Get
            Return Convert.ToSingle(Me.nudValue.Maximum)
        End Get
        Set(ByVal value As Single)
            ' Truncate
            value = CSng(Math.Max(Math.Min(value, Convert.ToSingle(Decimal.MaxValue) / 10), Convert.ToSingle(Decimal.MinValue) / 10))
            Me.nudValue.Maximum = Convert.ToDecimal(value)
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get or set the selected value for the brush.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BrushValue() As Single
        Get
            Return Convert.ToSingle(Me.nudValue.Value)
        End Get
        Set(ByVal value As Single)
            Me.nudValue.Value = Convert.ToDecimal(value)
        End Set
    End Property

    Public Property EnabledValue() As Boolean
        Get
            Return Me.nudValue.Enabled
        End Get
        Set(ByVal value As Boolean)
            Me.nudValue.Visible = value
            Me.lbValue.Visible = value
        End Set
    End Property

    Private m_valueType As Type = GetType(Single)

    Public Property ValueType() As Type
        Get
            Return Me.m_valueType
        End Get
        Set(ByVal value As Type)
            Me.m_valueType = value
            Me.UpdateControls()
        End Set
    End Property

#End Region ' Public properties

#Region " Events "

    Private Sub ucBrushPicker_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.m_sg = StyleGuide.GetInstance()
        If (Me.m_sg IsNot Nothing) Then
            AddHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
        End If
        Me.UpdateControls()
    End Sub

    Private Sub ucBrushPicker_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        If (Me.m_sg IsNot Nothing) Then
            RemoveHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
            Me.m_sg = Nothing
        End If
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler; responds to trackbar changes by raising a brush picked event.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub nudValue_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles nudValue.ValueChanged
        Me.FireBrushPickedEvent()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler; responds to brush value changes by raising a brush picked event.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub tbBrush_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Me.FireBrushPickedEvent()
    End Sub

    Private Sub OnStyleGuideChanged(ByVal changeType As StyleGuide.eChangeType)
        If (changeType And StyleGuide.eChangeType.NumDigits) = StyleGuide.eChangeType.NumDigits Then
            Me.UpdateControls()
        End If
    End Sub

#End Region ' Events 

#Region " Internal implementation "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Raises a brush picked event.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub FireBrushPickedEvent()
        RaiseEvent OnBrushPicked(Me.BrushSize, Me.BrushValue)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update control states.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateControls()
        If Me.m_valueType Is GetType(Integer) Then
            Me.nudValue.DecimalPlaces = 0
            Me.nudValue.Increment = 1
        Else
            Me.nudValue.DecimalPlaces = Me.m_sg.NumDigits
            Me.nudValue.Increment = New Decimal(5 * (10 ^ (1 - Me.m_sg.NumDigits)))
        End If
    End Sub

#End Region ' Internal implementation

End Class
