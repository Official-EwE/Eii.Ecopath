'==============================================================================
'
' $Log: cEwEFormatProvider.vb,v $
' Revision 1.2  2008/12/15 15:33:21  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:20  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.5  2008/08/22 00:37:16  jeroens
' Put at least some restrictions when min/maxing NUD controls
'
' Revision 1.4  2008/08/15 17:21:09  jeroens
' Fixed update issue: misinterpreted change flags
'
' Revision 1.3  2008/08/13 17:34:35  jeroens
' Added sanity check to SetValue
'
' Revision 1.2  2008/08/10 01:36:10  jeroens
' Uses shared tooltip
'
' Revision 1.1  2008/07/04 15:30:36  jeroens
' Moved
' Textbox and NUD tabstop set based on enabled state
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Drawing
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Properties

#End Region ' Imports 

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add-on class that implements EwEcolour and display feedback on Windows
    ''' controls.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cEwEFormatProvider

#Region " Private helper classes "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Factory to generate an <see cref="IControlWrapper">IControlWrapper</see>
        ''' for a given Windows control.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Class ControlWrapperFactory

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Factory method; instantiates a <see cref="IControlWrapper">IControlWrapper</see>
            ''' for a given Windows control.
            ''' </summary>
            ''' <param name="ctrl">The <see cref="Control">Windows control</see> to wrap.</param>
            ''' <param name="provider">The <see cref="cEwEFormatProvider">cEwEFormatProvider</see>
            ''' that requested this wrap.</param>
            ''' <returns>A <see cref="IControlWrapper">IControlWrapper</see> instance if succesful,
            ''' or nothing if an error occurred.</returns>
            ''' -----------------------------------------------------------------------
            Shared Function GetControlWrapper(ByVal ctrl As Control, _
                    ByVal provider As cEwEFormatProvider, _
                    Optional ByVal aItems As Object() = Nothing, _
                    Optional ByVal metadata As cVariableMetaData = Nothing) As IControlWrapper

                Dim wrapper As IControlWrapper = Nothing

                ' Wrapper supported Windows controls
                If TypeOf (ctrl) Is TextBox Then
                    wrapper = New TextBoxWrapper
                ElseIf TypeOf (ctrl) Is Label Then
                    wrapper = New LabelWrapper
                ElseIf TypeOf (ctrl) Is CheckBox Then
                    wrapper = New CheckboxWrapper
                ElseIf TypeOf (ctrl) Is ComboBox Then
                    wrapper = New ComboBoxWrapper
                ElseIf TypeOf (ctrl) Is NumericUpDown Then
                    wrapper = New NumericUpDownWrapper
                End If

                ' Development time sanity check
                Debug.Assert(wrapper IsNot Nothing, String.Format("ControlWrapperFactory: control {0} not supported", ctrl.GetType().ToString()))

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
                        Return String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, (obj.ID + 1), obj.Name)
                    Else
                        Dim obj As ICoreInterface = DirectCast(Me.m_objItem, ICoreInterface)
                        Return String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, obj.Index, obj.Name)
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
        Private Class TextBoxWrapper
            Implements IControlWrapper

#Region " Private variables "

            ''' <summary>The wrapped text box</summary>
            Private WithEvents m_tb As TextBox = Nothing
            ''' <summary>The EwEFormatProvider that implements value and colour
            ''' behaviour onto the text box.</summary>
            Private m_provider As cEwEFormatProvider = Nothing

#End Region ' Private variables 

#Region " Implementation "

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="ctrl"></param>
            ''' <param name="provider"></param>
            ''' <returns></returns>
            ''' -----------------------------------------------------------------------
            Public Function Wrap(ByVal ctrl As System.Windows.Forms.Control, ByVal provider As cEwEFormatProvider, Optional ByVal aItems() As Object = Nothing, Optional ByVal metadata As EwECore.cVariableMetaData = Nothing) As Boolean _
                    Implements IControlWrapper.Wrap

                Dim objValueType As Object = provider.ValueType
                Dim bSucces As Boolean = True

                Try
                    ' Store ref to Text box
                    Me.m_tb = DirectCast(ctrl, TextBox)
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
            ''' Update value and display style of the text box.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Public Sub UpdateContent() Implements IControlWrapper.UpdateContent

                Dim sg As StyleGuide = StyleGuide.GetInstance()
                Dim objValue As Object = Me.m_provider.Value
                Dim objValueType As Object = Me.m_provider.ValueType
                Dim style As StyleGuide.eStyleFlags = Me.m_provider.Style
                Dim bEditable As Boolean = ((style And StyleGuide.eStyleFlags.NotEditable) = 0)
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
                If (style And StyleGuide.eStyleFlags.Highlight) > 0 Then
                    Me.m_tb.BackColor = sg.ApplicationColor(StyleGuide.eApplicationColorType.HIGHLIGHT)
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
            '''' Event handler, invoked when the Text Box text has changed. This will 
            '''' pass the modified text back to the parent 
            '''' <see cref="EwEFormatProvider">EwEFormatProvider</see>.
            '''' </summary>
            '''' -----------------------------------------------------------------------
            'Private Sub m_tb_Changed(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_tb.TextChanged
            '    ' Did anything change?
            '    If Me.m_tb.Modified Then
            '        ' Update internal value
            '        Me.m_provider.Value = Me.m_tb.Text
            '        ' Clear modified flag
            '        Me.m_tb.Modified = False
            '        '' Apply formatting
            '        'Me.UpdateContent()
            '    End If
            'End Sub
            '''' -----------------------------------------------------------------------
            '''' <summary>
            '''' Event handler, invoked when the Text Box text has lost focus. This will 
            '''' pass the modified text back to the parent 
            '''' <see cref="EwEFormatProvider">EwEFormatProvider</see>.
            '''' </summary>
            '''' -----------------------------------------------------------------------
            Private Sub m_tb_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_tb.LostFocus
                ' Did anything change?
                If Me.m_tb.Modified Then
                    ' Update internal value
                    Me.m_provider.Value = Me.m_tb.Text
                    ' Clear modified flag
                    Me.m_tb.Modified = False
                    '' Apply formatting
                    'Me.UpdateContent()
                End If
            End Sub ' TextBox events


#End Region

        End Class

#End Region ' Class TextBoxWrapper

#Region " Class NumericUpDownWrapper "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Helper class; wraps a NumericUpDown control for interaction with a Property.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Class NumericUpDownWrapper
            Implements IControlWrapper

#Region " Private variables "

            ''' <summary></summary>
            Private WithEvents m_ud As NumericUpDown = Nothing
            ''' <summary></summary>
            Private m_provider As cEwEFormatProvider = Nothing
            ''' <summary>For trapping number of decimal digits display.</summary>
            Private WithEvents m_sg As StyleGuide = StyleGuide.GetInstance()

#End Region ' Private variables 

#Region " Implementation "

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="ctrl"></param>
            ''' <param name="provider"></param>
            ''' <returns></returns>
            ''' -----------------------------------------------------------------------
            Public Function Wrap(ByVal ctrl As System.Windows.Forms.Control, ByVal provider As cEwEFormatProvider, Optional ByVal aItems() As Object = Nothing, Optional ByVal metadata As EwECore.cVariableMetaData = Nothing) As Boolean _
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
                    ' Store ref to Text box
                    Me.m_ud = DirectCast(ctrl, NumericUpDown)
                    ' Store ref to provider
                    Me.m_provider = provider
                    ' Apply metadata
                    If (metadata IsNot Nothing) Then
                        Me.m_ud.Minimum = CDec(Math.Max(-10000000000, metadata.Min))
                        Me.m_ud.Maximum = CDec(Math.Min(10000000000, CSng(metadata.Max)))
                    End If
                    ' Config control
                    Me.OnStyleGuideChanged(StyleGuide.eChangeType.All)
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
            ''' Update value and display style of the numeric up down control.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Public Sub UpdateContent() Implements IControlWrapper.UpdateContent

                Dim sg As StyleGuide = StyleGuide.GetInstance()
                Dim objValue As Object = Me.m_provider.Value
                Dim style As StyleGuide.eStyleFlags = Me.m_provider.Style
                Dim bEditable As Boolean = ((style And StyleGuide.eStyleFlags.NotEditable) = 0)

                ' Sanity checks
                If objValue Is Nothing Then Return

                ' Update control
                ' - Set value
                Me.m_ud.Value = Convert.ToDecimal(objValue)
                ' - Set colours
                sg.GetStyleColors(style, Me.m_ud.ForeColor, Me.m_ud.BackColor)
                ' - Set read-only state
                Me.m_ud.ReadOnly = (bEditable = False)
                Me.m_ud.TabStop = (bEditable = True)

                ' Highlight border
                If (style And StyleGuide.eStyleFlags.Highlight) > 0 Then
                    Me.m_ud.BackColor = sg.ApplicationColor(StyleGuide.eApplicationColorType.HIGHLIGHT)
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
            Private Sub m_tb_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_ud.Validated
                ' Update internal value
                Me.m_provider.Value = Me.m_ud.Value
                Me.UpdateContent()
            End Sub

#End Region ' NumericUpDown events

#Region " Style guide events "

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Event handler, invoked when the Style Guide has been modified.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Private Sub OnStyleGuideChanged(ByVal changeType As StyleGuide.eChangeType) Handles m_sg.StyleGuideChanged

                If (changeType And StyleGuide.eChangeType.NumDigits) = 0 Then Return

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
        Private Class ComboBoxWrapper
            Implements IControlWrapper

#Region " Private variables "

            ''' <summary></summary>
            Private WithEvents m_cmb As ComboBox = Nothing
            ''' <summary></summary>
            Private m_provider As cEwEFormatProvider = Nothing
            Private m_tValue As Type = Nothing

#End Region ' Private variables 

#Region " Implementation "

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="ctrl"></param>
            ''' <param name="provider"></param>
            ''' <returns></returns>
            ''' -----------------------------------------------------------------------
            Public Function Wrap(ByVal ctrl As System.Windows.Forms.Control, ByVal provider As cEwEFormatProvider, Optional ByVal aItems() As Object = Nothing, Optional ByVal metadata As EwECore.cVariableMetaData = Nothing) As Boolean _
                    Implements IControlWrapper.Wrap

                Dim bSucces As Boolean = True

                Try
                    ' Store ref to combo box
                    Me.m_cmb = DirectCast(ctrl, ComboBox)
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

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Update value and display style of the combo box.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Public Sub UpdateContent() Implements IControlWrapper.UpdateContent

                Dim objValue As Object = Me.m_provider.Value
                Dim sg As StyleGuide = StyleGuide.GetInstance()
                Dim style As StyleGuide.eStyleFlags = Me.m_provider.Style
                Dim bEditable As Boolean = ((style And StyleGuide.eStyleFlags.NotEditable) = 0)

                ' Sanity checks
                If objValue Is Nothing Then Return

                ' Update control
                ' - Set selection state
                Me.SelectItem(objValue)
                ' - Set colours
                sg.GetStyleColors(style, Me.m_cmb.ForeColor, Me.m_cmb.BackColor)
                ' - Set enabled state
                Me.m_cmb.Enabled = bEditable

            End Sub

            Private Sub SelectItem(ByVal objValue As Object)
                Dim objItem As Object = Nothing
                Dim iValue As Integer = -1

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

            Private m_aItems As Object() = Nothing

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
            Private Sub m_cmb_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                    Handles m_cmb.SelectedIndexChanged, m_cmb.TextChanged

                If Me.m_provider.ValueType Is GetType(Integer) Then
                    ' Update internal value
                    Me.m_provider.Value = Me.SelectedIndex()
                Else
                    Me.m_provider.Value = Me.m_cmb.SelectedItem
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
        Private Class CheckboxWrapper
            Implements IControlWrapper

#Region " Private variables "

            ''' <summary></summary>
            Private WithEvents m_cb As CheckBox = Nothing
            ''' <summary></summary>
            Private m_provider As cEwEFormatProvider = Nothing

#End Region ' Private variables 

#Region " Implementation "

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="ctrl"></param>
            ''' <param name="provider"></param>
            ''' <returns></returns>
            ''' -----------------------------------------------------------------------
            Public Function Wrap(ByVal ctrl As System.Windows.Forms.Control, ByVal provider As cEwEFormatProvider, Optional ByVal aItems() As Object = Nothing, Optional ByVal metadata As EwECore.cVariableMetaData = Nothing) As Boolean _
                    Implements IControlWrapper.Wrap

                Dim bSucces As Boolean = True

                If Not (provider.ValueType Is GetType(Boolean)) Then
                    Debug.Assert(False, "Checkboxes should only wrap boolean values")
                    Return False
                End If

                Try
                    ' Store ref to Text box
                    Me.m_cb = DirectCast(ctrl, CheckBox)
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

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Update value and display style of the check box.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Public Sub UpdateContent() Implements IControlWrapper.UpdateContent

                Dim sg As StyleGuide = StyleGuide.GetInstance()
                Dim objValue As Object = Me.m_provider.Value
                Dim objValueType As Object = Me.m_provider.ValueType
                Dim style As StyleGuide.eStyleFlags = Me.m_provider.Style
                Dim bEditable As Boolean = ((style And StyleGuide.eStyleFlags.NotEditable) = 0)

                ' Sanity checks
                If objValue Is Nothing Then Return

                ' Update control
                ' - Set checked state
                Me.m_cb.Checked = CBool(objValue)
                ' - Set colours
                ' *** Checkbox special: do not colour background on "OK" or "NotEditable" style
                style = style And Not (StyleGuide.eStyleFlags.OK Or StyleGuide.eStyleFlags.NotEditable)
                Me.m_cb.BackColor = Color.FromArgb(0, 0, 0, 0)
                ' Fetch, boy
                sg.GetStyleColors(style, Me.m_cb.ForeColor, Me.m_cb.BackColor)
                ' - Set enabled state
                Me.m_cb.Enabled = bEditable

                ' Highlight border
                If (style And StyleGuide.eStyleFlags.Highlight) > 0 Then
                    Me.m_cb.BackColor = sg.ApplicationColor(StyleGuide.eApplicationColorType.HIGHLIGHT)
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
            Private Sub m_cb_Checked(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_cb.CheckedChanged
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
        Private Class LabelWrapper
            Implements IControlWrapper

#Region " Private variables "

            ''' <summary>The wrapped text box</summary>
            Private WithEvents m_lb As Label = Nothing
            ''' <summary>The EwEFormatProvider that implements value and colour
            ''' behaviour onto the text box.</summary>
            Private m_provider As cEwEFormatProvider = Nothing

#End Region ' Private variables 

#Region " Implementation "

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="ctrl"></param>
            ''' <param name="provider"></param>
            ''' <returns></returns>
            ''' -----------------------------------------------------------------------
            Public Function Wrap(ByVal ctrl As System.Windows.Forms.Control, ByVal provider As cEwEFormatProvider, Optional ByVal aItems() As Object = Nothing, Optional ByVal metadata As EwECore.cVariableMetaData = Nothing) As Boolean _
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

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Update value and display style of the text box.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Public Sub UpdateContent() Implements IControlWrapper.UpdateContent

                Dim sg As StyleGuide = StyleGuide.GetInstance()
                Dim objValue As Object = Me.m_provider.Value
                Dim objValueType As Object = Me.m_provider.ValueType
                Dim style As StyleGuide.eStyleFlags = Me.m_provider.Style
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
                sg.GetStyleColors(style And Not StyleGuide.eStyleFlags.OK, Me.m_lb.ForeColor, Me.m_lb.BackColor)

                ' Highlight border
                If (style And StyleGuide.eStyleFlags.Highlight) > 0 Then
                    Me.m_lb.BackColor = sg.ApplicationColor(StyleGuide.eApplicationColorType.HIGHLIGHT)
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

        ''' <summary>Value of the control.</summary>
        Private m_objValue As Object = Nothing
        ''' <summary><see cref="Type">Type</see> of the Value.</summary>
        Private m_tValue As Type = Nothing
        ''' <summary>EwE <see cref="StyleGuide.eStyleFlags">Style</see> of the control.</summary>
        Private m_style As StyleGuide.eStyleFlags = StyleGuide.eStyleFlags.OK
        ''' <summary>Reference to the global <see cref="StyleGuide">StyleGuide</see>.</summary>
        Private WithEvents m_sg As StyleGuide = StyleGuide.GetInstance()
        ''' <summary>The wrapper that interacts with the control</summary>
        Private m_ctrlWrapper As IControlWrapper = Nothing

        Public Sub New(ByVal ctrl As Control, ByVal tValue As Type, _
                Optional ByVal aItems As Object() = Nothing, Optional ByVal metadata As cVariableMetaData = Nothing)
            ' Store value type
            Me.m_tValue = tValue
            ' Get wrapper
            Me.m_ctrlWrapper = ControlWrapperFactory.GetControlWrapper(ctrl, Me, aItems, metadata)
        End Sub

        Public Sub New(ByVal ctrl As Control, ByVal tValue As Type, ByVal metadata As cVariableMetaData)
            ' Store value type
            Me.m_tValue = tValue
            ' Get wrapper
            Me.m_ctrlWrapper = ControlWrapperFactory.GetControlWrapper(ctrl, Me, Nothing, metadata)
        End Sub

#Region " Value "

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

                ' Optimization: check for changes
                If Not Object.ReferenceEquals(Me.m_objValue, Nothing) Then
                    If Me.m_objValue.Equals(objValue) Then
                        ' No changes: do not set value
                        Return
                    End If
                End If

                Try
                    ' Store new value
                    If Me.m_tValue Is GetType(String) Then
                        Me.m_objValue = CStr(objValue)
                    ElseIf Me.m_tValue Is GetType(Integer) Then
                        Me.m_objValue = Convert.ToInt32(objValue)
                    ElseIf Me.m_tValue Is GetType(Single) Then
                        Me.m_objValue = Convert.ToSingle(objValue)
                    ElseIf Me.m_tValue Is GetType(Double) Then
                        Me.m_objValue = Convert.ToDouble(objValue)
                    Else
                        Me.m_objValue = objValue
                    End If

                Catch ex As Exception
                    ' Decline!
                End Try

                ' Update
                Me.UpdateContent()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the <see cref="StyleGuide.eStyleFlags">Style</see> to reflect 
        ''' in the control.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Property Style() As StyleGuide.eStyleFlags
            Get
                Return Me.m_style
            End Get
            Set(ByVal s As StyleGuide.eStyleFlags)
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
                Return ((Me.m_style And StyleGuide.eStyleFlags.NotEditable) <> StyleGuide.eStyleFlags.NotEditable)
            End Get
            Set(ByVal value As Boolean)
                If value = True Then
                    Me.Style = Me.Style And (Not StyleGuide.eStyleFlags.NotEditable)
                Else
                    Me.Style = Me.Style Or StyleGuide.eStyleFlags.NotEditable
                End If
            End Set
        End Property
#End Region ' Value

#Region " Updates "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' StyleGuide change event handler; makes sure cells are redrawn
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub OnStyleGuideChanged(ByVal changeType As StyleGuide.eChangeType) Handles m_sg.StyleGuideChanged
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

#End Region ' Updates

    End Class

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="cEwEFormatProvider">cEwEFormatProvider</see> that is driven
    ''' by a <see cref="cProperty">cProperty</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cPropertyFormatProvider
        Inherits cEwEFormatProvider

        ''' <summary>Property that serves as data and style source.</summary>
        Private WithEvents m_prop As cProperty = Nothing
        ''' <summary>The wrapped control</summary>
        Private WithEvents m_ctrl As Control = Nothing

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ctrl"></param>
        ''' <param name="source"></param>
        ''' <param name="varName"></param>
        ''' <param name="sourceSec"></param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal ctrl As Control, ByVal source As cCoreInputOutputBase, ByVal varName As eVarNameFlags, Optional ByVal sourceSec As cCoreInputOutputBase = Nothing, Optional ByVal aItems As Object() = Nothing)
            ' Get underlying cProperty for these values
            Me.New(ctrl, cPropertyManager.GetInstance().GetProperty(source, varName, sourceSec), aItems)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ctrl"></param>
        ''' <param name="prop"></param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal ctrl As Control, ByVal prop As cProperty, Optional ByVal aItems As Object() = Nothing)
            MyBase.New(ctrl, prop.GetValueType(), aItems, prop.GetVariableMetadata())
            ' Store relevant bits
            Me.m_prop = prop
            Me.m_ctrl = ctrl
            ' Fire change event manually to immediately show the property value
            Me.OnPropertyChanged(Me.m_prop, cProperty.eChangeFlags.All)
        End Sub

#Region " Data "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the style to reflect in the TextBox, overriding the style
        ''' dictated by the underlying cProperty.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Property Style() As StyleGuide.eStyleFlags
            Get
                Dim eStyle As StyleGuide.eStyleFlags = MyBase.Style()
                If (eStyle <> StyleGuide.eStyleFlags.OK) Then Return eStyle
                If Me.m_prop Is Nothing Then Return Nothing
                Return Me.m_prop.GetStyle()
            End Get
            Set(ByVal eStyle As StyleGuide.eStyleFlags)
                MyBase.Style = eStyle
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the value maintained in the underlying Property.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Property Value() As Object
            Get
                If Me.m_prop Is Nothing Then Return Nothing
                Return Me.m_prop.GetValue()
            End Get
            Set(ByVal value As Object)
                If Me.m_prop Is Nothing Then Return
                Me.m_prop.SetValue(value, TriState.UseDefault)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Refresh()
            Me.m_prop.Refresh()
        End Sub

#End Region ' Data

#Region " Local events "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, invoked when the source Property <see cref="cProperty.PropertyChanged">changes</see>.
        ''' </summary>
        ''' <param name="prop">The property that changed.</param>
        ''' <param name="changeFlags">The type of change.</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnPropertyChanged(ByVal prop As cProperty, ByVal changeFlags As cProperty.eChangeFlags) Handles m_prop.PropertyChanged
            ' Sanity check
            Debug.Assert(Object.ReferenceEquals(prop, Me.m_prop))

            ' Update control
            If (changeFlags And (cProperty.eChangeFlags.CoreStatus Or cProperty.eChangeFlags.Value)) > 0 Then
                ' Get new content
                Me.UpdateContent()
            End If

            ' Update tooltip
            If (changeFlags And cProperty.eChangeFlags.Remarks) > 0 Then
                cToolTipShared.GetInstance().SetToolTip(Me.m_ctrl, prop.GetRemark())
            End If

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the wrapped control receives focus. Handled to fire 
        ''' an application-wide <see cref="PropertySelectionCommand">PropertySelectionCommand</see>.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub OnGotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_ctrl.Enter
            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim dsc As PropertySelectionCommand = DirectCast(cmdh.GetCommand(PropertySelectionCommand.COMMAND_NAME), PropertySelectionCommand)

            If Object.ReferenceEquals(dsc, Nothing) Then Return

            dsc.Invoke(Me.m_prop)
        End Sub

#End Region ' Local events 

    End Class

End Namespace
