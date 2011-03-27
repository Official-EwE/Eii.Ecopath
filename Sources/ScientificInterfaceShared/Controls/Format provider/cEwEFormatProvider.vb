#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Drawing
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Style

#End Region ' Imports 

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add-on class that implements EwEcolour and display feedback on Windows
    ''' controls.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cEwEFormatProvider
        Implements IUIElement

#Region " Private helper classes "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Factory to generate an <see cref="IControlWrapper">IControlWrapper</see>
        ''' for a given Windows control.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Class cControlWrapperFactory

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Factory method; instantiates a <see cref="IControlWrapper">IControlWrapper</see>
            ''' for a given Windows control.
            ''' </summary>
            ''' <param name="ctrl">The <see cref="Control">Windows Control</see> to wrap.</param>
            ''' <param name="provider">The <see cref="cEwEFormatProvider">cEwEFormatProvider</see>
            ''' that requested this wrap.</param>
            ''' <returns>A <see cref="IControlWrapper">IControlWrapper</see> instance if succesful,
            ''' or nothing if an error occurred.</returns>
            ''' -----------------------------------------------------------------------
            Shared Function GetControlWrapper(ByVal uic As cUIContext, _
                                              ByVal ctrl As Control, _
                                              ByVal provider As cEwEFormatProvider, _
                                              Optional ByVal aItems As Object() = Nothing, _
                                              Optional ByVal metadata As cVariableMetaData = Nothing) As IControlWrapper

                Dim wrapper As IControlWrapper = Nothing

                ' Wrapper supported Windows controls
                If TypeOf (ctrl) Is TextBox Or TypeOf (ctrl) Is RichTextBox Then
                    wrapper = New cTextBoxWrapper
                ElseIf TypeOf (ctrl) Is Label Then
                    wrapper = New cLabelWrapper
                ElseIf TypeOf (ctrl) Is CheckBox Then
                    wrapper = New cCheckboxWrapper
                ElseIf TypeOf (ctrl) Is ComboBox Then
                    wrapper = New cComboBoxWrapper
                ElseIf TypeOf (ctrl) Is NumericUpDown Then
                    wrapper = New cNumericUpDownWrapper
                End If

                ' Development time sanity check
                Debug.Assert(wrapper IsNot Nothing, String.Format("ControlWrapperFactory: control {0} not supported", ctrl.GetType().ToString()))

                ' Pass on UI context
                wrapper.UIContext = uic
                ' Try to wrap
                If Not wrapper.Wrap(ctrl, provider, aItems, metadata) Then wrapper = Nothing
                ' Return result
                Return wrapper

            End Function

        End Class

#Region " IndexedCollectionItem "

        Private Class IndexedCollectionItem
            Private m_objItem As Object = Nothing

            Public Sub New(ByVal objItem As Object)
                Debug.Assert(objItem IsNot Nothing)
                Me.m_objItem = objItem
            End Sub

            Public Overrides Function ToString() As String
                If (TypeOf Me.m_objItem Is ICoreInterface) Then
                    If (TypeOf Me.m_objItem Is cForcingFunction) Then
                        Dim obj As cForcingFunction = DirectCast(Me.m_objItem, cForcingFunction)
                        Return String.Format(My.Resources.GENERIC_LABEL_INDEXED, (obj.ID + 1), obj.Name)
                    Else
                        Dim obj As ICoreInterface = DirectCast(Me.m_objItem, ICoreInterface)
                        Return String.Format(My.Resources.GENERIC_LABEL_INDEXED, obj.Index, obj.Name)
                    End If
                End If
                Return Me.m_objItem.ToString()
            End Function

            Public Function CoreIndex() As Integer
                If TypeOf Me.m_objItem Is ICoreInterface Then
                    ' Always return TRUE index
                    Return DirectCast(Me.m_objItem, ICoreInterface).Index
                End If
                Return 0
            End Function
        End Class

#End Region ' IndexedCollectionItem

#Region " Private helper classes "

#Region " Interface IControlWrapper "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Interface for wrapping standard Windows control by an EwEFormatProvider.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Interface IControlWrapper
            Inherits IUIElement

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Wrap a Windows control by attaching it to an EwEFormatProvider.
            ''' </summary>
            ''' <param name="ctrl">The <see cref="Control">Control</see> to interact with.</param>
            ''' <param name="provider">The <see cref="cEwEFormatProvider">cEwEFormatProvider</see>
            ''' that will provide <see cref="cEwEFormatProvider.Value">value</see>, 
            ''' <see cref="cEwEFormatProvider.ValueType">value type</see> and 
            ''' <see cref="cEwEFormatProvider.Style">display style</see> for the control.</param>
            ''' <returns>True if wrapped succesfully.</returns>
            ''' -----------------------------------------------------------------------
            Function Wrap(ByVal ctrl As Control, ByVal provider As cEwEFormatProvider, _
                Optional ByVal aItems As Object() = Nothing, Optional ByVal metadata As cVariableMetaData = Nothing) As Boolean

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Release a wrapped control.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Sub Release()

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Trigger to update the value and display style of the <see cref="Control">Control</see>.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Sub UpdateContent()

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' List of items to reflect in the control. This paramter will only work
            ''' for list-containing controls such as combo boxes and list boxes.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Property Items() As Object()

        End Interface

#End Region ' Interface IControlWrapper

#Region " Class TextBoxWrapper "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Class cTextBoxWrapper
            Implements IControlWrapper

#Region " Private variables "

            ''' <summary>UI context for this wrapper.</summary>
            Private m_uic As cUIContext = Nothing
            ''' <summary>The wrapped text box</summary>
            Private m_tb As TextBoxBase = Nothing
            ''' <summary>The EwEFormatProvider that implements value and colour
            ''' behaviour onto the text box.</summary>
            Private m_provider As cEwEFormatProvider = Nothing

#End Region ' Private variables 

#Region " Implementation "

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get/set the UI context for this wrapper.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Property UIContext() As cUIContext _
                Implements IUIElement.UIContext
                Get
                    Return Me.m_uic
                End Get
                Set(ByVal value As cUIContext)
                    Me.m_uic = value
                End Set
            End Property

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Wrap control as a TextBoxBase class
            ''' </summary>
            ''' <param name="ctrl"></param>
            ''' <param name="provider"></param>
            ''' <returns></returns>
            ''' -----------------------------------------------------------------------
            Public Function Wrap(ByVal ctrl As Control, ByVal provider As cEwEFormatProvider, _
                                 Optional ByVal aItems() As Object = Nothing, _
                                 Optional ByVal metadata As EwECore.cVariableMetaData = Nothing) As Boolean _
                    Implements IControlWrapper.Wrap

                Dim objValueType As Object = provider.ValueType
                Dim bSucces As Boolean = True

                Try
                    ' Store ref to Text box
                    Me.m_tb = DirectCast(ctrl, TextBoxBase)
                    AddHandler Me.m_tb.LostFocus, AddressOf OnControlLostFocus

                    ' Store ref to provider
                    Me.m_provider = provider
                    ' Apply metadata
                    If (metadata IsNot Nothing) Then
                        ' String variable?
                        If (objValueType Is GetType(String)) Then
                            ' #Yes: use metadata length
                            Me.m_tb.MaxLength = metadata.Length
                        End If
                    End If
                Catch e As Exception
                    ' Throw dev. time error
                    Debug.Assert(False, "Failed to wrap text box")
                    ' Report failure
                    bSucces = False
                End Try

                Return bSucces

            End Function

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Release a wrapped control.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Public Sub Release() _
                    Implements IControlWrapper.Release

                If (Me.m_tb IsNot Nothing) Then
                    RemoveHandler Me.m_tb.LostFocus, AddressOf OnControlLostFocus
                    Me.m_tb = Nothing
                End If

            End Sub

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Update value and display style of the text box.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Public Sub UpdateContent() Implements IControlWrapper.UpdateContent

                Dim sg As cStyleGuide = Me.UIContext.StyleGuide
                Dim objValue As Object = Me.m_provider.Value
                Dim objValueType As Object = Me.m_provider.ValueType
                Dim style As cStyleGuide.eStyleFlags = Me.m_provider.Style
                Dim bEditable As Boolean = ((style And cStyleGuide.eStyleFlags.NotEditable) = 0)
                Dim strText As String = ""

                ' Sanity checks
                If objValue Is Nothing Then Return

                ' Get default value
                strText = objValue.ToString()

                ' Interpret as single?
                If objValueType Is GetType(Single) Then
                    ' #Yes: apply format
                    strText = sg.FormatNumber(CSng(objValue), style)
                End If

                ' Interpret as double?
                If objValueType Is GetType(Double) Then
                    ' #Yes: apply format
                    strText = sg.FormatNumber(CDbl(objValue), style)
                End If

                ' Update text box
                ' - Set text
                Me.m_tb.Text = strText
                ' - Set colours
                sg.GetStyleColors(style, Me.m_tb.ForeColor, Me.m_tb.BackColor)
                ' - Set read-only state
                Me.m_tb.ReadOnly = (bEditable = False)
                Me.m_tb.TabStop = (bEditable = True)

                ' Highlight border
                If (style And cStyleGuide.eStyleFlags.Highlight) > 0 Then
                    Me.m_tb.BackColor = sg.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT)
                End If

            End Sub

            Public Property Items() As Object() Implements IControlWrapper.Items
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object())
                End Set
            End Property

#End Region ' Implementation

#Region " TextBox events "

            '''' -----------------------------------------------------------------------
            '''' <summary>
            '''' Event handler, invoked when the Text Box text has lost focus. This will 
            '''' pass the modified text back to the parent 
            '''' <see cref="EwEFormatProvider">EwEFormatProvider</see>.
            '''' </summary>
            '''' -----------------------------------------------------------------------
            Private Sub OnControlLostFocus(ByVal sender As Object, ByVal e As System.EventArgs)
                ' Did anything change?
                If Me.m_tb.Modified Then
                    ' Update internal value
                    Me.m_provider.Value = Me.m_tb.Text
                    ' Clear modified flag
                    Me.m_tb.Modified = False
                End If
            End Sub

#End Region ' TextBox events

        End Class

#End Region ' Class TextBoxWrapper

#Region " Class NumericUpDownWrapper "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Helper class; wraps a NumericUpDown control for interaction with a Property.
        ''' </summary>
        ''' <remarks>
        ''' Note that the up/down control is not truely suitable for handling EwE
        ''' variables; it cannot be emptied (to reflect NULL status values) and its
        ''' value ranges cannot be limited to reflect values such as 'greater than' and
        ''' 'less than' making the control unsuitable for displaying a range of EwE
        ''' values.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Private Class cNumericUpDownWrapper
            Implements IControlWrapper

#Region " Private variables "

            ''' <summary>UI context for this wrapper.</summary>
            Private m_uic As cUIContext = Nothing
            ''' <summary></summary>
            Private m_ud As NumericUpDown = Nothing
            ''' <summary></summary>
            Private m_provider As cEwEFormatProvider = Nothing
            ''' <summary>For trapping number of decimal digits display.</summary>
            Private m_sg As cStyleGuide = Nothing

#End Region ' Private variables 

#Region " Implementation "

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get/set the UI context for this wrapper.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Property UIContext() As cUIContext _
                Implements IUIElement.UIContext
                Get
                    Return Me.m_uic
                End Get
                Set(ByVal value As cUIContext)
                    Me.m_uic = value
                End Set
            End Property

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="ctrl"></param>
            ''' <param name="provider"></param>
            ''' <returns></returns>
            ''' -----------------------------------------------------------------------
            Public Function Wrap(ByVal ctrl As Control, _
                                 ByVal provider As cEwEFormatProvider, _
                                 Optional ByVal aItems() As Object = Nothing, _
                                 Optional ByVal metadata As EwECore.cVariableMetaData = Nothing) As Boolean _
                    Implements IControlWrapper.Wrap

                Dim objValueType As Object = provider.ValueType
                Dim bSucces As Boolean = True

                ' Test for incompatible data types
                ' - Strings
                If objValueType Is GetType(String) Then
                    Debug.Assert(False, "NumericUpDown controls cannot handle string values")
                    Return False
                End If
                ' - Booleans
                If objValueType Is GetType(Boolean) Then
                    Debug.Assert(False, "NumericUpDown controls are unsuitable for handling boolean values - use checkbox instead")
                    Return False
                End If

                Try
                    ' Store ref to control
                    Me.m_ud = DirectCast(ctrl, NumericUpDown)
                    AddHandler Me.m_ud.Validated, AddressOf OnSaveValue
                    AddHandler Me.m_ud.LostFocus, AddressOf OnSaveValue

                    Me.m_sg = Me.UIContext.StyleGuide
                    AddHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged

                    ' Store ref to provider
                    Me.m_provider = provider
                    ' Apply metadata
                    If (metadata IsNot Nothing) Then
                        Me.m_ud.Minimum = CDec(Math.Max(-10000000000, CSng(metadata.Min)))
                        Me.m_ud.Maximum = CDec(Math.Min(10000000000, CSng(metadata.Max)))
                    End If
                    ' Config control
                    Me.OnStyleGuideChanged(cStyleGuide.eChangeType.All)
                Catch e As Exception
                    ' Throw dev. time error
                    Debug.Assert(False, "Failed to wrap numeric up down control")
                    ' Report failure
                    bSucces = False
                End Try

                Return bSucces

            End Function

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Release a wrapped control.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Public Sub Release() _
                    Implements IControlWrapper.Release

                If (Me.m_ud IsNot Nothing) Then
                    RemoveHandler Me.m_ud.Validated, AddressOf OnSaveValue
                    RemoveHandler Me.m_ud.LostFocus, AddressOf OnSaveValue
                    Me.m_ud = Nothing
                End If

                If (Me.m_sg IsNot Nothing) Then
                    RemoveHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
                    Me.m_sg = Nothing
                End If

            End Sub

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Update value and display style of the numeric up down control.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Public Sub UpdateContent() Implements IControlWrapper.UpdateContent

                Dim objValue As Object = Me.m_provider.Value
                Dim style As cStyleGuide.eStyleFlags = Me.m_provider.Style
                Dim bEditable As Boolean = ((style And cStyleGuide.eStyleFlags.NotEditable) = 0)

                ' Sanity checks
                If objValue Is Nothing Then Return

                ' Update control
                ' - Set value truncated to min and max ranges. Note that value_none is not
                '   explicitly supported here!
                Me.m_ud.Value = Math.Max(Me.m_ud.Minimum, Math.Min(Me.m_ud.Maximum, Convert.ToDecimal(objValue)))
                ' - Set colours
                Me.m_sg.GetStyleColors(style, Me.m_ud.ForeColor, Me.m_ud.BackColor)
                ' - Set read-only state
                Me.m_ud.ReadOnly = (bEditable = False)
                Me.m_ud.TabStop = (bEditable = True)

                ' Highlight border
                If (style And cStyleGuide.eStyleFlags.Highlight) > 0 Then
                    Me.m_ud.BackColor = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT)
                End If

            End Sub

            Public Property Items() As Object() Implements IControlWrapper.Items
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object())
                End Set
            End Property

#End Region ' Implementation

#Region " NumericUpDown events "

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Event handler, invoked when the numeric up down control looses focus. This will pass the
            ''' control value back into the parent <see cref="cEwEFormatProvider">cEwEFormatProvider</see>.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Private Sub OnSaveValue(ByVal sender As Object, ByVal e As System.EventArgs)

                ' Update internal value
                If Not Decimal.Equals(Me.m_provider.Value, Me.m_ud.Value) Then
                    Me.m_provider.Value = Me.m_ud.Value
                    Me.UpdateContent()
                End If

            End Sub

#End Region ' NumericUpDown events

#Region " Style guide events "

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Event handler, invoked when the Style Guide has been modified.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Private Sub OnStyleGuideChanged(ByVal changeType As cStyleGuide.eChangeType)

                If (changeType And cStyleGuide.eChangeType.NumberFormatting) = 0 Then Return

                Dim objValueType As Type = Me.m_provider.ValueType

                ' Set number of decimal places for appropriate data types
                If objValueType Is GetType(Single) Or objValueType Is GetType(Double) Then
                    Me.m_ud.DecimalPlaces = Me.m_sg.NumDigits
                Else
                    Me.m_ud.DecimalPlaces = 0
                End If

            End Sub

#End Region ' Style guide events

        End Class

#End Region ' Class NumericUpDownWrapper

#Region " Class ComboBoxWrapper "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, invoked when the Combo Box selected index has changed to
        ''' update the value into the parent <see cref="cEwEFormatProvider">cEwEFormatProvider</see>.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Class cComboBoxWrapper
            Implements IControlWrapper

#Region " Private variables "

            ''' <summary>UI context for this wrapper.</summary>
            Private m_uic As cUIContext = Nothing
            ''' <summary>The wrapped combo box.</summary>
            Private m_cmb As ComboBox = Nothing
            ''' <summary></summary>
            Private m_provider As cEwEFormatProvider = Nothing
            Private m_tValue As Type = Nothing
            ''' <summary>Optional combo box items.</summary>
            Private m_aItems As Object() = Nothing

#End Region ' Private variables 

#Region " Implementation "

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get/set the UI context for this wrapper.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Property UIContext() As cUIContext _
                Implements IUIElement.UIContext
                Get
                    Return Me.m_uic
                End Get
                Set(ByVal value As cUIContext)
                    Me.m_uic = value
                End Set
            End Property

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="ctrl"></param>
            ''' <param name="provider"></param>
            ''' <returns></returns>
            ''' -----------------------------------------------------------------------
            Public Function Wrap(ByVal ctrl As Control, ByVal provider As cEwEFormatProvider, Optional ByVal aItems() As Object = Nothing, Optional ByVal metadata As EwECore.cVariableMetaData = Nothing) As Boolean _
                    Implements IControlWrapper.Wrap

                Dim bSucces As Boolean = True

                Try
                    ' Store ref to combo box
                    Me.m_cmb = DirectCast(ctrl, ComboBox)
                    AddHandler Me.m_cmb.SelectedIndexChanged, AddressOf OnControlValueChanged
                    AddHandler Me.m_cmb.TextChanged, AddressOf OnControlValueChanged

                    ' Store ref to provider
                    Me.m_provider = provider
                    ' Populate combo
                    If Not Object.ReferenceEquals(aItems, Nothing) Then
                        ' Eradicate content
                        Me.Items = aItems
                    End If

                    ' ToDo: apply metadata

                Catch e As Exception
                    ' Throw dev. time error
                    Debug.Assert(False, String.Format("Failed to wrap combo box {0}", ctrl.ToString()))
                    ' Report failure
                    bSucces = False
                End Try

                Return bSucces

            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Release the wrapped control.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Sub Release() _
                    Implements IControlWrapper.Release

                If (Me.m_cmb IsNot Nothing) Then
                    RemoveHandler Me.m_cmb.SelectedIndexChanged, AddressOf OnControlValueChanged
                    RemoveHandler Me.m_cmb.TextChanged, AddressOf OnControlValueChanged
                    Me.m_cmb = Nothing
                End If

            End Sub

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Update value and display style of the combo box.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Public Sub UpdateContent() Implements IControlWrapper.UpdateContent

                Dim objValue As Object = Me.m_provider.Value
                Dim sg As cStyleGuide = Me.UIContext.StyleGuide
                Dim style As cStyleGuide.eStyleFlags = Me.m_provider.Style
                Dim bEditable As Boolean = ((style And cStyleGuide.eStyleFlags.NotEditable) = 0)

                ' Sanity checks
                If (objValue Is Nothing) Then Return

                ' Update control
                ' - Set selection state
                Try
                    Me.SelectItem(objValue)
                Catch ex As Exception

                End Try
                ' - Set colours
                sg.GetStyleColors(style, Me.m_cmb.ForeColor, Me.m_cmb.BackColor)
                ' - Set enabled state
                Me.m_cmb.Enabled = bEditable

            End Sub

            Private Sub SelectItem(ByVal objValue As Object)
                Dim objItem As Object = Nothing
                Dim iValue As Integer = -1

                If Me.m_provider.ValueType Is GetType(Integer) Then
                    For iItem As Integer = 0 To Me.m_cmb.Items.Count - 1
                        objItem = Me.m_cmb.Items(iItem)
                        If (TypeOf objItem Is IndexedCollectionItem) Then
                            If (CInt(objValue) = DirectCast(objItem, IndexedCollectionItem).CoreIndex) Then
                                iValue = iItem
                                Exit For
                            End If
                        Else
                            If (String.Compare(CStr(objValue), CStr(objItem), False) = 0) Then
                                iValue = iItem
                                Exit For
                            End If
                        End If
                    Next
                    ' Truncate
                    Me.m_cmb.SelectedIndex = Math.Max(-1, Math.Min(Me.m_cmb.Items.Count - 1, iValue))
                Else
                    Me.m_cmb.Text = objValue.ToString
                End If
            End Sub

            Private Function SelectedIndex() As Integer
                Dim iIndex As Integer = cCore.NULL_VALUE
                Dim objItem As Object = Nothing

                If (Me.m_cmb.SelectedIndex >= 0) Then
                    objItem = Me.m_cmb.SelectedItem()
                    iIndex = Me.m_cmb.SelectedIndex

                    If (TypeOf objItem Is IndexedCollectionItem) Then
                        iIndex = DirectCast(objItem, IndexedCollectionItem).CoreIndex
                    End If
                End If
                Return iIndex
            End Function

            Public Property Items() As Object() Implements IControlWrapper.Items
                Get
                    Return Me.m_aItems
                End Get
                Set(ByVal aItems As Object())
                    ' Eradicate content
                    Me.m_cmb.Items.Clear()
                    ' Populate if new items given
                    If (Not Object.ReferenceEquals(aItems, Nothing)) Then
                        ' Populate
                        For iItem As Integer = 0 To aItems.Length - 1
                            ' Wrap item
                            Me.m_cmb.Items.Add(New IndexedCollectionItem(aItems(iItem)))
                        Next
                    End If
                    ' Done
                    Me.m_aItems = aItems
                    Me.UpdateContent()
                End Set
            End Property

#End Region ' Implementation 

#Region " ComboBox events "

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Event handler, invoked when the Text Box looses focus. This will pass the
            ''' text box value back into the parent <see cref="cEwEFormatProvider">cEwEFormatProvider</see>.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Private Sub OnControlValueChanged(ByVal sender As Object, ByVal e As System.EventArgs)

                ' Update internal value
                If Me.m_provider.ValueType Is GetType(Integer) Then
                    ' #Integer? Set index
                    Me.m_provider.Value = Me.SelectedIndex()
                ElseIf Me.m_provider.ValueType Is GetType(String) Then
                    ' #String? Set text
                    Me.m_provider.Value = Me.m_cmb.Text
                Else
                    ' #Try to do automatic magic, somehow
                    Try
                        Me.m_provider.Value = Convert.ChangeType(Me.m_cmb.SelectedItem, Me.m_provider.ValueType)
                    Catch ex As Exception
                        Debug.Assert(False, "Unable to convert value type")
                    End Try
                End If

            End Sub

#End Region ' ComboBox events

        End Class

#End Region ' Class ComboBoxWrapper

#Region " Class CheckboxWrapper "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Class cCheckboxWrapper
            Implements IControlWrapper

#Region " Private variables "

            ''' <summary>UI context for this wrapper.</summary>
            Private m_uic As cUIContext = Nothing
            ''' <summary>The wrapped check box.</summary>
            Private m_cb As CheckBox = Nothing
            ''' <summary></summary>
            Private m_provider As cEwEFormatProvider = Nothing

#End Region ' Private variables 

#Region " Implementation "

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get/set the UI context for this wrapper.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Property UIContext() As cUIContext _
                Implements IUIElement.UIContext
                Get
                    Return Me.m_uic
                End Get
                Set(ByVal value As cUIContext)
                    Me.m_uic = value
                End Set
            End Property

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="ctrl"></param>
            ''' <param name="provider"></param>
            ''' <returns></returns>
            ''' -----------------------------------------------------------------------
            Public Function Wrap(ByVal ctrl As Control, ByVal provider As cEwEFormatProvider, Optional ByVal aItems() As Object = Nothing, Optional ByVal metadata As EwECore.cVariableMetaData = Nothing) As Boolean _
                    Implements IControlWrapper.Wrap

                Dim bSucces As Boolean = True

                If Not (provider.ValueType Is GetType(Boolean)) Then
                    Debug.Assert(False, "Checkboxes should only wrap boolean values")
                    Return False
                End If

                Try
                    ' Store ref to Text box
                    Me.m_cb = DirectCast(ctrl, CheckBox)
                    AddHandler Me.m_cb.CheckedChanged, AddressOf OnControlValueChanged

                    ' Store ref to provider
                    Me.m_provider = provider
                Catch e As Exception
                    ' Throw dev. time error
                    Debug.Assert(False, String.Format("Failed to wrap checkbox {0}", ctrl.ToString()))
                    ' Report failure
                    bSucces = False
                End Try

                Return bSucces

            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Release the wrapped control.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Sub Release() _
                    Implements IControlWrapper.Release

                If (Me.m_cb IsNot Nothing) Then
                    RemoveHandler Me.m_cb.CheckedChanged, AddressOf OnControlValueChanged
                    Me.m_cb = Nothing
                End If

            End Sub

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Update value and display style of the check box.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Public Sub UpdateContent() Implements IControlWrapper.UpdateContent

                Dim sg As cStyleGuide = Me.UIContext.StyleGuide
                Dim objValue As Object = Me.m_provider.Value
                Dim objValueType As Object = Me.m_provider.ValueType
                Dim style As cStyleGuide.eStyleFlags = Me.m_provider.Style
                Dim bEditable As Boolean = ((style And cStyleGuide.eStyleFlags.NotEditable) = 0)

                ' Sanity checks
                If objValue Is Nothing Then Return

                ' Update control
                ' - Set checked state
                Me.m_cb.Checked = CBool(objValue)
                ' - Set colours
                ' *** Checkbox special: do not colour background on "OK" or "NotEditable" style
                style = style And Not (cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable)
                Me.m_cb.BackColor = SystemColors.Control
                ' Fetch, boy
                sg.GetStyleColors(style, Me.m_cb.ForeColor, Me.m_cb.BackColor)
                ' - Set enabled state
                Me.m_cb.Enabled = bEditable

                ' Highlight border
                If (style And cStyleGuide.eStyleFlags.Highlight) > 0 Then
                    Me.m_cb.BackColor = sg.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT)
                End If

            End Sub

            Public Property Items() As Object() Implements IControlWrapper.Items
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object())
                End Set
            End Property

#End Region ' Implementation

#Region " Control events "

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Event handler, invoked when the Check Box state has changed. This will 
            ''' pass the check box check state back to the parent 
            ''' <see cref="cEwEFormatProvider">cEwEFormatProvider</see>.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Private Sub OnControlValueChanged(ByVal sender As Object, ByVal e As System.EventArgs)
                ' Update internal value
                Me.m_provider.Value = Me.m_cb.Checked
            End Sub

#End Region ' Control events

        End Class

#End Region ' Class CheckboxWrapper

#Region " Class LabelWrapper "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Class cLabelWrapper
            Implements IControlWrapper

#Region " Private variables "

            ''' <summary>UI context for this wrapper.</summary>
            Private m_uic As cUIContext = Nothing
            ''' <summary>The wrapped label control.</summary>
            Private m_lb As Label = Nothing
            ''' <summary>The EwEFormatProvider that implements value and colour
            ''' behaviour onto the text box.</summary>
            Private m_provider As cEwEFormatProvider = Nothing

#End Region ' Private variables 

#Region " Implementation "

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get/set the UI context for this wrapper.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Property UIContext() As cUIContext _
                Implements IUIElement.UIContext
                Get
                    Return Me.m_uic
                End Get
                Set(ByVal value As cUIContext)
                    Me.m_uic = value
                End Set
            End Property

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="ctrl"></param>
            ''' <param name="provider"></param>
            ''' <returns></returns>
            ''' -----------------------------------------------------------------------
            Public Function Wrap(ByVal ctrl As Control, ByVal provider As cEwEFormatProvider, Optional ByVal aItems() As Object = Nothing, Optional ByVal metadata As EwECore.cVariableMetaData = Nothing) As Boolean _
                    Implements IControlWrapper.Wrap

                Dim bSucces As Boolean = True

                Try
                    ' Store ref to Text box
                    Me.m_lb = DirectCast(ctrl, Label)
                    ' Store ref to provider
                    Me.m_provider = provider
                Catch e As Exception
                    ' Throw dev. time error
                    Debug.Assert(False, "Failed to wrap label")
                    ' Report failure
                    bSucces = False
                End Try

                Return bSucces

            End Function

            Public Sub Release() _
                    Implements IControlWrapper.Release

                Me.m_lb = Nothing

            End Sub

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Update value and display style of the text box.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Public Sub UpdateContent() Implements IControlWrapper.UpdateContent

                Dim sg As cStyleGuide = Me.UIContext.StyleGuide
                Dim objValue As Object = Me.m_provider.Value
                Dim objValueType As Object = Me.m_provider.ValueType
                Dim style As cStyleGuide.eStyleFlags = Me.m_provider.Style
                Dim strText As String = ""

                ' Sanity checks
                If objValue Is Nothing Then Return

                ' Get default value
                strText = objValue.ToString()

                ' Interpret as single?
                If objValueType Is GetType(Single) Then
                    ' #Yes: apply format
                    strText = sg.FormatNumber(CSng(objValue), style)
                End If

                ' Interpret as double?
                If objValueType Is GetType(Double) Then
                    ' #Yes: apply format
                    strText = sg.FormatNumber(CDbl(objValue), style)
                End If

                ' Update text box
                ' - Set text
                Me.m_lb.Text = strText
                ' - Set colours
                sg.GetStyleColors(style And Not cStyleGuide.eStyleFlags.OK, Me.m_lb.ForeColor, Me.m_lb.BackColor)

                ' Highlight border
                If (style And cStyleGuide.eStyleFlags.Highlight) > 0 Then
                    Me.m_lb.BackColor = sg.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT)
                End If

            End Sub

            Public Property Items() As Object() Implements IControlWrapper.Items
                Get
                    Return Nothing
                End Get
                Set(ByVal value As Object())
                End Set
            End Property

#End Region ' Implementation

        End Class

#End Region ' Class LabelWrapper

#End Region ' Control Wrappers

#End Region ' Private helper classes

#Region " Private vars "

        ''' <summary>The UI context serving this provider.</summary>
        Private m_uic As cUIContext = Nothing
        ''' <summary>Value of the control.</summary>
        Private m_objValue As Object = Nothing
        ''' <summary><see cref="Type">Type</see> of the Value.</summary>
        Private m_tValue As Type = Nothing
        ''' <summary>EwE <see cref="cStyleGuide.eStyleFlags">Style</see> of the control.</summary>
        Private m_style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK
        ''' <summary>The wrapper that interacts with the control</summary>
        Private m_ctrlWrapper As IControlWrapper = Nothing

#End Region ' Private vars

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ctrl"></param>
        ''' <param name="tValue"></param>
        ''' <param name="aItems"></param>
        ''' <param name="metadata"></param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext, _
                       ByVal ctrl As Control, _
                       ByVal tValue As Type, _
                       Optional ByVal aItems As Object() = Nothing, _
                       Optional ByVal metadata As cVariableMetaData = Nothing)

            ' Sanity checks
            Debug.Assert(uic IsNot Nothing)
            Debug.Assert(ctrl IsNot Nothing)

            ' Store value type
            Me.m_tValue = tValue
            ' Get wrapper
            Me.m_ctrlWrapper = cControlWrapperFactory.GetControlWrapper(uic, ctrl, Me, aItems, metadata)
            ' Connect to style guide
            Me.UIContext = uic
            ' Respond to styleguide changes
            AddHandler Me.UIContext.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ctrl"></param>
        ''' <param name="tValue"></param>
        ''' <param name="metadata"></param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext, _
                       ByVal ctrl As Control, _
                       ByVal tValue As Type, _
                       ByVal metadata As cVariableMetaData)
            Me.New(uic, ctrl, tValue, Nothing, metadata)
        End Sub

#End Region ' Constructor

#Region " Release "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Release the format provider from the wrapped control.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub Release()
            If Me.m_ctrlWrapper IsNot Nothing Then
                Me.m_ctrlWrapper.Release()
                Me.m_ctrlWrapper = Nothing
            End If

            If Me.UIContext IsNot Nothing Then
                RemoveHandler Me.UIContext.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
                Me.UIContext = Nothing
            End If
        End Sub

#End Region ' Release

#Region " Interface "

        Public Property UIContext() As cUIContext Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Private Set(ByVal uic As cUIContext)
                Me.m_uic = uic
            End Set
        End Property

#End Region ' Interface

#Region " Value "

        ''' -------------------------------------------------------------------
        ''' <summary>Event to notify that a value has changed.</summary>
        ''' <param name="sender">The format provider that sent the event.</param>
        ''' -------------------------------------------------------------------
        Public Event OnValueChanged(ByVal sender As cEwEFormatProvider)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the value of the control.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Property Value() As Object
            Get
                Return Me.m_objValue
            End Get
            Set(ByVal objValue As Object)

                Dim objValueConverted As Object = Nothing

                Try
                    ' First convert value
                    If Me.m_tValue Is GetType(String) Then
                        objValueConverted = CStr(objValue)
                    ElseIf Me.m_tValue Is GetType(Integer) Then
                        objValueConverted = Convert.ToInt32(objValue)
                    ElseIf Me.m_tValue Is GetType(Single) Then
                        objValueConverted = Convert.ToSingle(objValue)
                    ElseIf Me.m_tValue Is GetType(Double) Then
                        objValueConverted = Convert.ToDouble(objValue)
                    Else
                        objValueConverted = objValue
                    End If

                Catch ex As Exception
                    ' Decline!
                    Return
                End Try

                ' Check for changes
                If Not Object.ReferenceEquals(Me.m_objValue, Nothing) Then
                    If Me.m_objValue.Equals(objValueConverted) Then
                        ' No changes: do not set value
                        Return
                    End If
                End If

                ' Set value
                Me.m_objValue = objValueConverted

                ' Update
                Me.UpdateContent()
                Me.RaiseChangeEvent()

            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the <see cref="cStyleGuide.eStyleFlags">Style</see> to reflect 
        ''' in the control.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Property Style() As cStyleGuide.eStyleFlags
            Get
                Return Me.m_style
            End Get
            Set(ByVal s As cStyleGuide.eStyleFlags)
                ' Store style
                Me.m_style = s
                ' Update
                Me.UpdateContent()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the type of the value in the control.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected ReadOnly Property ValueType() As Type
            Get
                Return Me.m_tValue
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the lsit of items to display in the control.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Items() As Object()
            Get
                Return Me.m_ctrlWrapper.Items
            End Get
            Set(ByVal aItems As Object())
                Me.m_ctrlWrapper.Items = aItems
            End Set
        End Property

        Public Property Enabled() As Boolean
            Get
                Return ((Me.m_style And cStyleGuide.eStyleFlags.NotEditable) <> cStyleGuide.eStyleFlags.NotEditable)
            End Get
            Set(ByVal value As Boolean)
                If value = True Then
                    Me.Style = Me.Style And (Not cStyleGuide.eStyleFlags.NotEditable)
                Else
                    Me.Style = Me.Style Or cStyleGuide.eStyleFlags.NotEditable
                End If
            End Set
        End Property

#End Region ' Value

#Region " Updates "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' cStyleGuide change event handler; makes sure cells are redrawn
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub OnStyleGuideChanged(ByVal changeType As cStyleGuide.eChangeType)
            Me.UpdateContent()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Update the attached control
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Sub UpdateContent()
            If Me.m_ctrlWrapper IsNot Nothing Then
                Me.m_ctrlWrapper.UpdateContent()
            End If
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Raises the <see cref="OnValueChanged">OnValueChanged</see> ewvent.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Sub RaiseChangeEvent()
            Try
                RaiseEvent OnValueChanged(Me)
            Catch ex As Exception
                ' Wow
            End Try

        End Sub

#End Region ' Updates

    End Class

End Namespace
