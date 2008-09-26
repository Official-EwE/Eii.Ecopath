'==============================================================================
'
' $Log: cProperty.vb,v $
' Revision 1.1  2008/09/26 07:31:22  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/06/01 23:45:45  jeroens
' Separated from Scientific Interface
'
' Revision 1.50  2008/05/29 22:22:50  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.49  2008/05/16 16:16:40  jeroens
' SetValue sends event by default
'
' Revision 1.48  2008/05/06 00:42:21  jeroens
' Remarks inaccessible for properties without ID
'
' Revision 1.47  2008/05/04 12:51:19  jeroens
' Hmm, cached remark was a good idea after all to detect updates
'
' Revision 1.46  2008/05/04 01:47:40  jeroens
' Removed buffered remark value
'
' Revision 1.45  2008/04/08 16:24:49  jeroens
' Added Custom change event for unconventional use
'
' Revision 1.44  2008/04/07 20:17:32  jeroens
' Secundary index not applied correctly when setting values?! Wow!!!!!!
'
' Revision 1.43  2008/01/24 14:43:08  jeroens
' Added SecundaryIndexOffset
'
' Revision 1.42  2008/01/11 12:23:49  jeroens
' Fixed possible value assignment issue on Single properties
'
'==============================================================================

Option Strict On
Imports EwECore
Imports EwECore.ValueWrapper
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Style

Namespace Properties

#Region " cProperty "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The one and only property change delegeate
    ''' </summary>
    ''' <param name="prop">The property that fired off the change enent</param>
    ''' <param name="changeFlags">A bit flag pattern that indicates which aspects of the property changed</param>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)> _
    Public Delegate Sub PropertyChangeEventHandler(ByVal prop As cProperty, ByVal changeFlags As cProperty.eChangeFlags)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' A property wraps a core variable in a strong-typed object which will broadcast
    ''' change events whenever its value and/or Style changes!
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(True)> _
    Public MustInherit Class cProperty

#Region " Private parts "

        ''' <summary>ID of the property</summary>
        Private m_strID As String = ""
        ''' <summary>cCoreInputOutputBase object that is the source of this property's data</summary>
        Private m_Source As cCoreInputOutputBase = Nothing
        ''' <summary>VarName within Source</summary>
        Private m_VarName As eVarNameFlags = eVarNameFlags.NotSet
        ''' <summary>Secundary index within VarName, in case this is an object</summary>
        Private m_SourceSec As cCoreInputOutputBase = Nothing
        ''' <summary>Secundary index within VarName, in case this is an enumerated value.</summary>
        Private m_iSecIndex As Integer = cCore.NULL_VALUE
        ''' <summary>Offset for secundary index, for instance to use when the first detritus group should provide index 1.</summary>
        Private m_iSecIndexOffset As Integer = 0
        ''' <summary>Buffered remark value.</summary>
        Private m_strRemark As String = ""

#End Region ' Private parts

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes the property
        ''' </summary>
        ''' <param name="id">Manually created ID to assign to the property. This id should be unique.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal id As String)
            Me.m_strID = id
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes the property
        ''' </summary>
        ''' <param name="src">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> instance that is the data source for this property.</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">Variable name</see> in <paramref name="Source">Source</paramref> that is the data source for this property.</param>
        ''' <param name="srcSec">The object acting as index on <paramref name="VarName">VarName</paramref> in case this is an indexed variable.</param>
        ''' <param name="iSecIndexOffset">An optional offset that defines the diffence between the index provided by
        ''' <paramref name="srcSec">srcSec</paramref> and the actual storage position in the underlying arrays.</param>
        ''' <remarks>
        ''' <para>The <paramref name="iSecIndexOffset">iSecIndexOffset</paramref> parameter is useful in cases where secundary
        ''' objects represent array indices other than their ID value.</para>
        ''' <para>A typical example would be the use of groups as secundary indexes to access Detritus fate information.
        ''' The Core detritus fate arrays are indexed by [1, {numdetritusgroups}], while the actual detritus groups that act as
        ''' secundary indexes have an <see cref="cCoreInputOutputBase.Index">Index</see> value that is most likely higher than
        ''' the the detritus fate array index range. To compensate for this difference, a 
        ''' <paramref name="iSecIndexOffset">iSecIndexOffset</paramref> value of {<see cref="cCore.nGroups">numgroups</see>} -
        ''' {<see cref="cCore.nDetritusGroups">numdetritusgroups</see>} will ensure that the <paramref name="srcSec">srcSec</paramref>
        ''' object is used to correctly access the underlying array.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal src As EwECore.cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                Optional ByVal srcSec As EwECore.cCoreInputOutputBase = Nothing, _
                Optional ByVal iSecIndexOffset As Integer = 0)

            Me.m_strID = cValueID.Generate(DirectCast(src, cCoreInputOutputBase), VarName, srcSec)

            ' Store link to core
            Me.m_Source = src
            Me.m_VarName = VarName
            Me.m_SourceSec = srcSec
            Me.m_iSecIndex = cCore.NULL_VALUE
            Me.m_iSecIndexOffset = iSecIndexOffset
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Refresh a property from its related core variable
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Refresh()

            Dim newValue As Object = Nothing
            Dim strNewRemark As String = ""
            Dim coreStatus As eStatusFlags = 0
            Dim guiStyle As StyleGuide.eStyleFlags = 0
            Dim changeFlags As eChangeFlags = 0
            Dim iIndex As Integer = cCore.NULL_VALUE

            If (Me.m_SourceSec IsNot Nothing) Then
                iIndex = Me.m_SourceSec.Index
            End If
            If (Me.m_iSecIndex <> cCore.NULL_VALUE) Then
                iIndex = Me.m_iSecIndex
            End If
            iIndex -= Me.m_iSecIndexOffset

            If (Me.m_Source IsNot Nothing) Then
                ' Get the variable
                newValue = m_Source.GetVariable(Me.m_VarName, iIndex)

                ' Did this entail a change?
                If Not Me.IsValue(newValue) Then
                    ' # Yes: flag as changed
                    changeFlags = eChangeFlags.Value
                    ' Write the new value
                    Me.Value = newValue
                End If

                ' Get the core status
                coreStatus = m_Source.GetStatus(Me.m_VarName, iIndex)
                ' Hard-copy only the core status bits. All other flags are GUI flags and are preserved
                guiStyle = DirectCast(CInt(coreStatus And StyleGuide.eStyleFlags.CoreStatusFlagsMask) Or _
                                  CInt(Me.Style And (Not StyleGuide.eStyleFlags.CoreStatusFlagsMask)), StyleGuide.eStyleFlags)
                ' Did Style change?
                If Not Me.IsStyle(guiStyle) Then
                    ' # Yes: flag as changed
                    changeFlags = changeFlags Or eChangeFlags.CoreStatus
                    ' Write the new value
                    Me.Style = guiStyle
                End If

                ' Get new remark text
                strNewRemark = Me.Remark()
                If String.Compare(strNewRemark, Me.m_strRemark, False) <> 0 Then
                    changeFlags = changeFlags Or eChangeFlags.Remarks
                    Me.Remark = strNewRemark
                End If
            End If

            ' Get remarks
            Me.UpdateRemarksStyle(TriState.False)

            ' Get references

            ' Anything changed?
            If (changeFlags <> 0) Then
                ' #Yes: fire away
                Me.FireChangeNotification(changeFlags)
            End If

        End Sub

#End Region ' Construction 

#Region " Properties of this property (are you confused yet?)"

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the ID for this property
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property ID() As String
            Get
                Return Me.m_strID
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see>-derived
        ''' source for the property
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Source() As cCoreInputOutputBase
            Get
                Return Me.m_Source
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="eVarNameFlags">Variable Name</see> within the
        ''' <see cref="cProperty.Source">source</see> for the property
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property VarName() As eVarNameFlags
            Get
                Return Me.m_VarName
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the secundary index within <see cref="cProperty.VarName">Variable name</see>
        ''' in case this is an indexed property
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property SourceSec() As cCoreInputOutputBase
            Get
                Return Me.m_SourceSec
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cValue">Value description</see> of the variable
        ''' in <see cref="Source">data source</see> for the property.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property ValueDescriptor() As cValue
            Get
                If Me.m_Source IsNot Nothing Then
                    Return Me.m_Source.ValueDescriptor(Me.m_VarName)
                End If
                Return Nothing
            End Get
        End Property

#End Region '  Properties

#Region " Value "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the type of the value
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public MustOverride Function GetValueType() As Type

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/Set the actual value for this property
        ''' </summary>
        ''' <param name="bHonourNull">Flag stating whether NULL status flags 
        ''' should return a NULL value.</param>
        ''' -------------------------------------------------------------------
        Protected MustOverride Property Value(Optional ByVal bHonourNull As Boolean = True) As Object

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the value maintained in the property
        ''' </summary>
        ''' <param name="bHonourNull">Flag stating whether NULL status flags 
        ''' should return a NULL value.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function GetValue(Optional ByVal bHonourNull As Boolean = True) As Object
            Return Me.Value(bHonourNull)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Set a value in the property and commit it to the EwE core.
        ''' </summary>
        ''' <param name="newValue">The value to set</param>
        ''' <param name="notify">Flag that states whether a change notification must be
        ''' sent out. Possible values are:
        ''' <list type="bullet">
        ''' <item>
        ''' <term>TriState.True</term>
        ''' <description>Broadcast a change notification, regardless if a value and/or Style change has occured</description>
        ''' </item>
        ''' <item>
        ''' <term>TriState.False</term>
        ''' <description>Do not broadcast a change notification, regardless if a value and/or Style change has occured</description>
        ''' </item>
        ''' <item>
        ''' <term>TriState.UseDefault</term>
        ''' <description>Broadcast a change notification when a value or Style change has occured</description>
        ''' </item>
        ''' </list>
        ''' </param>
        ''' <returns>True if this request resulted in a value and/or Style change.</returns>
        ''' <remarks>
        ''' By default, the Property is left to determine whether an event needs
        ''' to be sent.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Function SetValue(ByVal newValue As Object, _
                    Optional ByVal notify As TriState = TriState.UseDefault) As Boolean

            Dim vs As cVariableStatus = Nothing
            Dim changeFlags As eChangeFlags = 0
            Dim iIndex As Integer = cCore.NULL_VALUE

            ' Is this a property associated with core data?
            If (Me.m_Source IsNot Nothing) Then

                ' #Yes: try to set the variable in the core
                If (Me.m_SourceSec IsNot Nothing) Then
                    iIndex = Me.m_SourceSec.Index
                Else
                    iIndex = Me.m_iSecIndex
                End If

                ' Correct for secundary offset
                iIndex -= Me.m_iSecIndexOffset

                'jb 16/mar/06 setVariable() now returns boolean so get the Style object from CurrentStyle
                ' Set new value
                m_Source.SetVariable(Me.m_VarName, newValue, iIndex)
                ' Get the status of this operation
                vs = m_Source.ValidationStatus

                ' Did the core accept this value?
                If ((vs.Status And eStatusFlags.FailedValidation) = 0) Then
                    ' #Yes
                    ' Turn f.v. Style flag off. Do not notify, but check if a change occurred
                    If (Me.SetStyle(StyleGuide.eStyleFlags.FailedValidation, TriState.False, eBitSetMode.BitwiseOff)) Then
                        changeFlags = eChangeFlags.CoreStatus
                    End If
                Else
                    ' #No
                    ' Turn f.v. Style flag on. Do not notify, but check if a change occurred
                    If (Me.SetStyle(StyleGuide.eStyleFlags.FailedValidation, TriState.False, eBitSetMode.BitwiseOn)) Then
                        changeFlags = eChangeFlags.CoreStatus
                    End If
                    ' Fetch value corrected by the Core
                    newValue = m_Source.GetVariable(Me.m_VarName, iIndex)
                End If

            End If

            ' Will the value change?
            If (Me.IsValue(newValue) = False) Then
                ' # Yes: flag as changed
                changeFlags = changeFlags Or eChangeFlags.Value
                ' Write the new value
                Me.Value = newValue
            End If

            ' Check whether to send change notification
            If (notify = TriState.True) Or ((notify = TriState.UseDefault) And (changeFlags <> 0)) Then
                ' #Yes: fire away
                Me.FireChangeNotification(changeFlags)
            End If

            ' Return changed state
            Return (changeFlags <> 0)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether a given value is equal to the value maintained in the property
        ''' </summary>
        ''' <param name="value">Value to compare</param>
        ''' <returns>True if the values are considered equal</returns>
        ''' -------------------------------------------------------------------
        Public MustOverride Function IsValue(ByVal value As Object) As Boolean

        Public Overridable Function GetVariableMetadata() As cVariableMetaData
            ' Santiy checks
            If Object.ReferenceEquals(Me.m_Source, Nothing) Then
                Return Nothing
            End If

            Return Me.m_Source.GetVariableMetadata(Me.m_VarName)
        End Function

#End Region ' Value

#Region " Style "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Enumerated type, used for setting and clearing bitflags
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Enum eBitSetMode
            ' Set an etire bitpattern
            All = 0
            ' Set all '1' bits in a bit pattern
            BitwiseOn
            ' Clear all '1' bits in a bit pattern
            BitwiseOff
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/Set the Style of the property. This method must be implemented by
        ''' inheriting classes.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected MustOverride Property Style() As StyleGuide.eStyleFlags

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the <see cref="StyleGuide.eStyleFlags">Style</see> of the property
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function GetStyle() As StyleGuide.eStyleFlags
            Return Me.Style
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Set the style of a Property.
        ''' </summary>
        ''' <param name="newStyle"></param>
        ''' <param name="BitSetMode"></param>
        ''' <param name="notify">
        ''' Flag that states whether a change notification needs to be broadcasted. Possible values are:
        ''' <list type="table">
        ''' <item><term>True</term><description>Always broadcasts a change notification, even when the Style has changed</description></item>
        ''' <item><term>False</term><description>Never broadcasts a change notification</description></item>
        ''' <item><term>UseDefault</term><description>Only broadcasts a change notification the Style has changed</description></item>
        ''' </list>
        ''' </param>
        ''' <returns>True if the Style changed</returns>
        ''' <remarks>Be aware that Style flags set here are not passed down to the Core. Core status bits are exclusively
        ''' managed by the core itself. Rather, this method allows </remarks>
        ''' -------------------------------------------------------------------
        Public Function SetStyle(ByVal newStyle As StyleGuide.eStyleFlags, _
                    Optional ByVal notify As TriState = TriState.False, _
                    Optional ByVal BitSetMode As eBitSetMode = eBitSetMode.All) As Boolean

            ' Get the current style
            Dim style As StyleGuide.eStyleFlags = Me.Style
            ' Change flag
            Dim bChanged As Boolean = False

            ' Calc what new Style flag will become
            Select Case BitSetMode
                Case eBitSetMode.All
                    style = newStyle
                Case eBitSetMode.BitwiseOn
                    style = style Or newStyle
                Case eBitSetMode.BitwiseOff
                    style = style And (Not newStyle)
            End Select

            ' Will the style change?
            If (Not IsStyle(newStyle)) Then
                ' #Yes: update the style
                Me.Style = style
                ' Remember that things have changed
                bChanged = True
            End If

            ' Check if notification has to sent out
            If (notify = TriState.True Or (notify = TriState.UseDefault And bChanged = True)) Then
                Me.FireChangeNotification(eChangeFlags.CoreStatus)
            End If

            Return bChanged
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether two Styles equal. This method must be implemented by
        ''' inheriting classes
        ''' </summary>
        ''' <param name="Style">Style to compare</param>
        ''' <returns>True if the Stylees are considered equal</returns>
        ''' -------------------------------------------------------------------
        Protected MustOverride Function IsStyle(ByVal Style As StyleGuide.eStyleFlags) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update the Remarks style bit based on available remarks and references for this property.
        ''' </summary>
        ''' <param name="notify">
        ''' Flag that states whether a change notification needs to be broadcasted. Possible values are:
        ''' <list type="table">
        ''' <item><term>True</term><description>Always broadcasts a change notification, even when the Style has changed</description></item>
        ''' <item><term>False</term><description>Never broadcasts a change notification</description></item>
        ''' <item><term>UseDefault</term><description>Only broadcasts a change notification the Style has changed</description></item>
        ''' </list>
        ''' </param>
        ''' -------------------------------------------------------------------
        Protected Overridable Sub UpdateRemarksStyle(Optional ByVal notify As TriState = TriState.False)

            Dim nRemarksStyle As eBitSetMode = eBitSetMode.BitwiseOff

            If (Me.HasRemark() Or Me.HasReferences()) Then
                nRemarksStyle = eBitSetMode.BitwiseOn
            End If

            Me.SetStyle(StyleGuide.eStyleFlags.Remarks, notify, nRemarksStyle)
        End Sub

#End Region ' Style

#Region " Remarks "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Set the remarks for this property.
        ''' </summary>
        ''' <param name="strRemark">The remarks text to set.</param>
        ''' <param name="notify">
        ''' Flag that states whether a change notification needs to be broadcasted. Possible values are:
        ''' <list type="table">
        ''' <item><term>True</term><description>Always broadcasts a change notification, even when the Style has changed</description></item>
        ''' <item><term>False</term><description>Never broadcasts a change notification</description></item>
        ''' <item><term>UseDefault</term><description>Only broadcasts a change notification the Style has changed</description></item>
        ''' </list>
        ''' </param>
        ''' <returns>True when Remarks have changed, False otherwise.</returns>
        ''' -------------------------------------------------------------------
        Public Function SetRemark(ByVal strRemark As String, Optional ByVal notify As TriState = TriState.UseDefault) As Boolean

            Dim bChanged As Boolean = False

            ' Prepare remarks
            If String.IsNullOrEmpty(strRemark) Then
                strRemark = ""
            Else
                strRemark = strRemark.Trim()
            End If

            ' Check if this entails a change
            If (String.Compare(strRemark, Me.Remark, False) <> 0) Then
                ' Store remarks
                Me.Remark = strRemark
                ' Update style but do not send out a notification
                Me.UpdateRemarksStyle(TriState.False)
                ' Remember change
                bChanged = True
            End If

            ' Check if remarks notification has to be sent out
            If (notify = TriState.True Or (notify = TriState.UseDefault And bChanged = True)) Then
                Me.FireChangeNotification(eChangeFlags.Remarks)
            End If

            Return bChanged
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the remarks for this property.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function GetRemark() As String
            Return Me.Remark
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Performs the actual getting/setting of the remarks for this property.
        ''' Remarks are not stored in the property itself, but should be obtained from the Core.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overridable Property Remark() As String
            Get
                If String.IsNullOrEmpty(Me.ID) Then Return ""
                Return cCore.GetInstance().Remark(Me.ID)
            End Get
            Set(ByVal strRemark As String)
                If Not String.IsNullOrEmpty(Me.ID) Then
                    cCore.GetInstance().Remark(Me.ID) = strRemark
                    Me.m_strRemark = strRemark
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether this property has associated remarks.
        ''' </summary>
        ''' <returns>True when remarks are present.</returns>
        ''' -------------------------------------------------------------------
        Public Function HasRemark() As Boolean
            Return (Not String.IsNullOrEmpty(Me.Remark))
        End Function

#End Region ' Remarks

#Region " References "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether this property has associated references
        ''' </summary>
        ''' <returns>True when references are present.</returns>
        ''' -------------------------------------------------------------------
        Public Function HasReferences() As Boolean
            Return False
        End Function

#End Region ' References

#Region " Event "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Enumerated type that provides information what section of a property
        ''' has changed: its value, its CoreStatus (and thus most likely its Style)
        ''' or its associated remarks and references.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Enum eChangeFlags As Byte
            Value = 1
            CoreStatus = 2
            Remarks = 4
            Custom = 8 ' Custom data has changed
            All = 255
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Property change notification event
        ''' </summary>
        ''' <param name="prop">Me</param>
        ''' -------------------------------------------------------------------
        Public Event PropertyChanged As PropertyChangeEventHandler

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Broadcast the property change event.
        ''' </summary>
        ''' <param name="changeFlags">Flags that indicate which aspect of the property has changed</param>
        ''' -------------------------------------------------------------------
        Public Sub FireChangeNotification(Optional ByVal changeFlags As eChangeFlags = eChangeFlags.All)
            RaiseEvent PropertyChanged(Me, changeFlags)
        End Sub

#End Region ' Event

    End Class

#End Region ' cProperty

#Region " cSingleProperty "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' cProperty derived class providing access to a strong-typed 
    ''' <see cref="Single">Single</see> value.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cSingleProperty
        : Inherits cProperty

        ''' <summary></summary>
        Private m_sValue As Single = 0.0
        Private m_Style As StyleGuide.eStyleFlags = StyleGuide.eStyleFlags.OK

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes the property.
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see>
        ''' instance that is the data source for this property.</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">Variable name</see>
        ''' in <paramref name="Source">Source</paramref> that is the data source
        ''' for this property.</param>
        ''' <param name="SourceSec">The object acting as index on
        ''' <paramref name="VarName">VarName</paramref> in case this is an indexed
        ''' variable.</param>
        ''' <param name="iSecIndexOffset">
        ''' <para>An optional offset that defines the diffence between the index provided by
        ''' <paramref name="srcSec">srcSec</paramref> and the actual storage position in the underlying arrays.
        ''' </para>
        ''' <para>For a detailed description of this variable refer to the constructor description of
        ''' <see cref="cProperty">cProperty</see>
        ''' </para>
        ''' </param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As EwECore.cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                Optional ByVal SourceSec As EwECore.cCoreInputOutputBase = Nothing, _
                Optional ByVal iSecIndexOffset As Integer = 0)
            MyBase.New(Source, VarName, SourceSec, iSecIndexOffset)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes the property.
        ''' </summary>
        ''' <param name="id">The ID to assign to the property.</param>
        ''' <remarks>This Constructor is provided to allow for manual creation.</remarks>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal id As String)
            MyBase.New(id)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns <see cref="Type">type Single</see>, the fixed type of this property.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetValueType() As System.Type
            Return GetType(Single)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the property value.
        ''' </summary>
        ''' <param name="bHonourNull">Flag stating whether NULL status flags 
        ''' should return a NULL value.</param>
        ''' -------------------------------------------------------------------
        Protected Overrides Property Value(Optional ByVal bHonourNull As Boolean = True) As Object
            Get
                ' Is this a NULL value?
                If bHonourNull And ((Me.m_Style And StyleGuide.eStyleFlags.Null) = StyleGuide.eStyleFlags.Null) Then
                    ' #Yes: return nothing (NOT 0.0)
                    Return Nothing
                End If
                ' Yes: return true value
                Return Me.m_sValue
            End Get
            Set(ByVal value As Object)
                Dim s As Single = 0.0
                Try
                    ' Try to convert to single
                    s = Convert.ToSingle(value)
                Catch ex As Exception
                    'Debug.Assert(False, "Unable to convert value to Single")
                End Try
                Me.m_sValue = s
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compares a given value to the the property value.
        ''' </summary>
        ''' <param name="value">The value to compare against the value in the property.</param>
        ''' <returns>True if the values can be considered equal.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function IsValue(ByVal value As Object) As Boolean
            Try
                Return m_sValue = CSng(value)
            Catch ex As Exception
                Return False
            End Try
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the Style for the property
        ''' </summary>
        ''' <remarks>This will need to change to StyleGuide.DisplayStyle</remarks>
        ''' -------------------------------------------------------------------
        Protected Overrides Property Style() As StyleGuide.eStyleFlags
            Get
                Return m_Style
            End Get
            Set(ByVal val As StyleGuide.eStyleFlags)
                Me.m_Style = val
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compares the <see cref="StyleGuide.eStyleFlags">Style</see> maintained
        ''' in the prothe Style mainta the property Style.
        ''' </summary>
        ''' <param name="Style">The Style to compare.</param>
        ''' <returns>True if the Style equal</returns>
        ''' <remarks>This will need to change to StyleGuide.DisplayStyle</remarks>
        ''' -------------------------------------------------------------------
        Protected Overrides Function IsStyle(ByVal Style As StyleGuide.eStyleFlags) As Boolean
            Return Me.m_Style = Style
        End Function

    End Class

#End Region ' cSingleProperty 

#Region " cIntegerProperty "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' cProperty derived class providing access to a strong-typed 
    ''' <see cref="Integer">Integer</see> value.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cIntegerProperty
        : Inherits cProperty

        Private m_value As Integer = 0
        Private m_Style As StyleGuide.eStyleFlags = StyleGuide.eStyleFlags.OK

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes the property
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see>
        ''' instance that is the data source for this property.</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">Variable name</see> in
        ''' <paramref name="Source">Source</paramref> that is the data source for this
        ''' property.</param>
        ''' <param name="SourceSec">The object acting as index on
        ''' <paramref name="VarName">VarName</paramref> in case this is an
        ''' indexed variable.</param>
        ''' <param name="iSecIndexOffset">
        ''' <para>An optional offset that defines the diffence between the index provided by
        ''' <paramref name="srcSec">srcSec</paramref> and the actual storage position in the underlying arrays.
        ''' </para>
        ''' <para>For a detailed description of this variable refer to the constructor description of
        ''' <see cref="cProperty">cProperty</see>
        ''' </para>
        ''' </param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As EwECore.cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                Optional ByVal SourceSec As EwECore.cCoreInputOutputBase = Nothing, _
                Optional ByVal iSecIndexOffset As Integer = 0)
            MyBase.New(Source, VarName, SourceSec, iSecIndexOffset)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes the property.
        ''' </summary>
        ''' <param name="id">The ID to assign to the property.</param>
        ''' <remarks>This Constructor is provided to allow for manual creation.</remarks>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal id As String)
            MyBase.New(id)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns <see cref="Type">type Integer</see>, the fixed type of this property.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetValueType() As System.Type
            Return GetType(Integer)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the property value.
        ''' </summary>
        ''' <param name="bHonourNull">Flag stating whether NULL status flags 
        ''' should return a NULL value.</param>
        ''' -------------------------------------------------------------------
        Protected Overrides Property Value(Optional ByVal bHonourNull As Boolean = True) As Object
            Get
                ' Is this a NULL value?
                If bHonourNull And ((Me.m_Style And StyleGuide.eStyleFlags.Null) = StyleGuide.eStyleFlags.Null) Then
                    ' #Yes: return nothing
                    Return Nothing
                End If
                ' Yes: return true value
                Return Me.m_value
            End Get
            Set(ByVal value As Object)
                Try
                    ' Try to convert to integer
                    Me.m_value = Convert.ToInt32(value)
                Catch ex As Exception
                    Debug.Assert(False, "Unable to convert value to Integer")
                End Try
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compares a given value to the the property value.
        ''' </summary>
        ''' <param name="value">The value to compare against the value in the property.</param>
        ''' <returns>True if the values can be considered equal.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function IsValue(ByVal value As Object) As Boolean
            Return Object.Equals(m_value, CInt(value))
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the Style for the property.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Property Style() As StyleGuide.eStyleFlags
            Get
                Return m_Style
            End Get
            Set(ByVal Style As StyleGuide.eStyleFlags)
                Me.m_Style = Style
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compares a given Style to the property Style.
        ''' </summary>
        ''' <param name="Style">The Style to compare.</param>
        ''' <returns>True if the Styles equal.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function IsStyle(ByVal Style As StyleGuide.eStyleFlags) As Boolean
            Return Me.m_Style = Style
        End Function

    End Class

#End Region ' cIntegerProperty 

#Region " cBooleanProperty "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' cProperty derived class providing access to a strong-typed 
    ''' <see cref="Boolean">Boolean</see> value.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cBooleanProperty
        : Inherits cProperty

        Private m_value As Boolean = False
        Private m_Style As StyleGuide.eStyleFlags = StyleGuide.eStyleFlags.OK

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes the property
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see>
        ''' instance that is the data source for this property.</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">Variable name</see> in
        ''' <paramref name="Source">Source</paramref> that is the data source for this
        ''' property.</param>
        ''' <param name="SourceSec">The object acting as index on
        ''' <paramref name="VarName">VarName</paramref> in case this is an
        ''' indexed variable.</param>
        ''' <param name="iSecIndexOffset">
        ''' <para>An optional offset that defines the diffence between the index provided by
        ''' <paramref name="srcSec">srcSec</paramref> and the actual storage position in the underlying arrays.
        ''' </para>
        ''' <para>For a detailed description of this variable refer to the constructor description of
        ''' <see cref="cProperty">cProperty</see>
        ''' </para>
        ''' </param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As EwECore.cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                Optional ByVal SourceSec As EwECore.cCoreInputOutputBase = Nothing, _
                Optional ByVal iSecIndexOffset As Integer = 0)
            MyBase.New(Source, VarName, SourceSec, iSecIndexOffset)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes the property.
        ''' </summary>
        ''' <param name="id">The ID to assign to the property.</param>
        ''' <remarks>This Constructor is provided to allow for manual creation.</remarks>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal id As String)
            MyBase.New(id)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns <see cref="Type">type Boolean</see>, the fixed type of this property.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetValueType() As System.Type
            Return GetType(Boolean)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the property value.
        ''' </summary>
        ''' <param name="bHonourNull">Flag stating whether NULL status flags 
        ''' should return a NULL value.</param>
        ''' -------------------------------------------------------------------
        Protected Overrides Property Value(Optional ByVal bHonourNull As Boolean = True) As Object
            Get
                ' Is this a NULL value?
                ' Is this a NULL value?
                If bHonourNull And ((Me.m_Style And StyleGuide.eStyleFlags.Null) = StyleGuide.eStyleFlags.Null) Then
                    ' #Yes: return nothing (NOT 0.0)
                    Return Nothing
                End If
                Return m_value
            End Get
            Set(ByVal value As Object)
                Try
                    ' Try to convert to boolean
                    Me.m_value = Convert.ToBoolean(value)
                Catch ex As Exception
                    Debug.Assert(False, "Unable to convert value to Boolean")
                End Try
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compares a given value to the the property value.
        ''' </summary>
        ''' <param name="value">The value to compare against the value in the property.</param>
        ''' <returns>True if the values can be considered equal.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function IsValue(ByVal value As Object) As Boolean
            Return Object.Equals(m_value, CBool(value))
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the Style for the property.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Property Style() As StyleGuide.eStyleFlags
            Get
                Return m_Style
            End Get
            Set(ByVal Style As StyleGuide.eStyleFlags)
                Me.m_Style = Style
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compares a given Style to the property Style.
        ''' </summary>
        ''' <param name="Style">The Style to compare.</param>
        ''' <returns>True if the Styles equal.</returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function IsStyle(ByVal Style As StyleGuide.eStyleFlags) As Boolean
            Return Me.m_Style = Style
        End Function

    End Class

#End Region ' cIntegerProperty 

#Region " cStringProperty "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' cProperty derived class providing access to a strong-typed 
    ''' <see cref="String">String</see> value.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cStringProperty
        : Inherits cProperty

        Private m_value As String = ""
        Private m_Style As StyleGuide.eStyleFlags = StyleGuide.eStyleFlags.OK

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes the property
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see>
        ''' instance that is the data source for this property.</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">Variable name</see> in
        ''' <paramref name="Source">Source</paramref> that is the data source for this
        ''' property.</param>
        ''' <param name="SourceSec">The object acting as index on <paramref name="VarName">VarName</paramref> in case this is an indexed variable.</param>
        ''' <param name="iSecIndexOffset">
        ''' <para>An optional offset that defines the diffence between the index provided by
        ''' <paramref name="srcSec">srcSec</paramref> and the actual storage position in the underlying arrays.
        ''' </para>
        ''' <para>For a detailed description of this variable refer to the constructor description of
        ''' <see cref="cProperty">cProperty</see>
        ''' </para>
        ''' </param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal Source As EwECore.cCoreInputOutputBase, ByVal VarName As eVarNameFlags, _
                Optional ByVal SourceSec As EwECore.cCoreInputOutputBase = Nothing, _
                Optional ByVal iSecIndexOffset As Integer = 0)
            MyBase.New(Source, VarName, SourceSec, iSecIndexOffset)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes the property
        ''' </summary>
        ''' <param name="id">The ID to assign to the property</param>
        ''' <remarks>This Constructor is provided to allow for manual creation</remarks>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal id As String)
            MyBase.New(id)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Strings anyone? Fresh strings! Going for the gentleman in the blue hat. Going once, going twice...
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetValueType() As System.Type
            Return GetType(String)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the property value
        ''' </summary>
        ''' <param name="bHonourNull">Flag stating whether NULL status flags 
        ''' should return a NULL value.</param>
        ''' -------------------------------------------------------------------
        Protected Overrides Property Value(Optional ByVal bHonourNull As Boolean = True) As Object
            Get
                ' Is this a NULL value?
                If bHonourNull And ((Me.m_Style And StyleGuide.eStyleFlags.Null) = StyleGuide.eStyleFlags.Null) Then
                    ' #Yes: return empty string
                    Return ""
                End If
                Return Me.m_value
            End Get
            Set(ByVal value As Object)
                Dim str As String = ""
                Try
                    ' Try to convert to string
                    str = Convert.ToString(value)
                    Me.m_value = str
                Catch ex As Exception
                    Debug.Assert(False, "Unable to convert value to String")
                End Try
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compares a given value to the the property value
        ''' </summary>
        ''' <param name="value">The value to compare against the value in the property</param>
        ''' <returns>True if the values can be considered equal</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function IsValue(ByVal value As Object) As Boolean
            Return (String.Compare(Me.m_value, CStr(value), StringComparison.Ordinal) = 0)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the Style for the property
        ''' </summary>
        ''' <remarks>This will need to change to StyleGuide.DisplayStyle</remarks>
        ''' -------------------------------------------------------------------
        Protected Overrides Property Style() As StyleGuide.eStyleFlags
            Get
                Return m_Style
            End Get
            Set(ByVal Style As StyleGuide.eStyleFlags)
                Me.m_Style = Style
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compares a given Style to the property Style
        ''' </summary>
        ''' <param name="Style">The Style to compare</param>
        ''' <returns>True if the Style equal</returns>
        ''' <remarks>This will need to change to StyleGuide.DisplayStyle</remarks>
        ''' -------------------------------------------------------------------
        Protected Overrides Function IsStyle(ByVal Style As StyleGuide.eStyleFlags) As Boolean
            Return Me.m_Style = Style
        End Function

    End Class

#End Region ' cStringProperty 

#Region " cCheckedProperty "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Specialized cProperty class, designed to behave like a check box or radio button
    ''' by observing a particular value in another cProperty.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cCheckedProperty
        Inherits cStringProperty

        ''' <summary>A property to observe.</summary>
        Private m_prop As cProperty = Nothing
        ''' <summary>The value of m_prop that this property represents.</summary>
        Private m_value As Object = Nothing

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes the property.
        ''' </summary>
        ''' <param name="prop">The <see cref="cProperty">Property</see> to observe.</param>
        ''' <param name="value">The value of <paramref name="prop">prop</paramref> that 
        ''' this instance represents.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal prop As cProperty, ByVal value As Object)
            MyBase.New("")

            ' Sanity check
            Debug.Assert(prop IsNot Nothing)

            Me.m_prop = prop
            Me.m_value = CObj(value)

            ' Listen for property changes
            AddHandler m_prop.PropertyChanged, AddressOf OnPropertyChanged
            Me.OnPropertyChanged(m_prop, eChangeFlags.All)

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the observed property has changed.
        ''' </summary>
        ''' <param name="prop">The changed <see cref="cProperty">property</see>.</param>
        ''' <param name="changeFlags">Bitwise <see cref="cProperty.eChangeFlags">flag</see> 
        ''' describing what aspect of the property changed.</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnPropertyChanged(ByVal prop As cProperty, ByVal changeFlags As cProperty.eChangeFlags)

            Dim bIsMyValue As Boolean = False
            Dim value As Object = prop.GetValue()
            Dim sf As StyleGuide.eStyleFlags = prop.GetStyle()
            Dim strValue As String = ""

            ' Value has not changed?
            If (changeFlags And (eChangeFlags.Value Or eChangeFlags.CoreStatus)) = 0 Then
                ' #No relevant changes: abort
                Return
            End If

            ' Check property value against instance value
            If prop.IsValue(Me.m_value) Then
                strValue = "X"
                sf = (sf Or StyleGuide.eStyleFlags.Checked)
            Else
                strValue = ""
            End If

            Me.Value = strValue
            Me.FireChangeNotification(eChangeFlags.Value)

            Me.SetStyle(sf, TriState.UseDefault, eBitSetMode.All)

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' User edit entry point. An edit operation resulted in a change of the value in the cell. This will
        ''' set the value of the underlying property to the value observed by this instance.
        ''' </summary>
        ''' <param name="newValue">The edited value.</param>
        ''' <param name="notify">States whether a notification must be broadcasted.</param>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------
        Public Overrides Function SetValue(ByVal newValue As Object, Optional ByVal notify As TriState = TriState.UseDefault) As Boolean

            Dim bResult As Boolean = True

            ' Is any value set?
            If newValue IsNot Nothing Then
                ' #Yes: Update the underlying property with our instance value.
                bResult = Me.m_prop.SetValue(Me.m_value, TriState.UseDefault)
            End If
            Return bResult

        End Function

    End Class

#End Region ' cCheckedProperty

End Namespace
