'==============================================================================
'
' $Log: cPropertyManager.vb,v $
' Revision 1.3  2009/01/19 18:07:25  jeroens
' MessageHandlers, CoreStateMonitor have sync objects
'
' Revision 1.2  2009/01/16 18:30:34  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:31:22  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2008/08/02 03:04:13  jeroens
' Renamed resources
'
' Revision 1.2  2008/06/15 12:46:19  jeroens
' Added safety checks when accessing unsupported varnames
'
' Revision 1.1  2008/06/01 23:45:46  jeroens
' Separated from Scientific Interface
'
' Revision 1.38  2008/05/29 22:22:50  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.37  2008/05/16 15:27:23  jeroens
' Cleaned-up in code comments
'
' Revision 1.36  2008/05/08 17:51:48  jeroens
' Dear prudence
'
' Revision 1.35  2008/04/08 15:45:37  jeroens
' Updates ancient code to determine variable type
'
' Revision 1.34  2008/04/07 02:31:10  jeroens
' Cleaning up resources
'
' Revision 1.33  2008/03/19 00:25:48  jeroens
' Added crash test in ExtractProperty
'
' Revision 1.32  2008/02/27 18:46:08  jeroens
' Fixed crash on updating property indexed by nMonths
'
' Revision 1.31  2008/02/19 13:51:42  jeroens
' Fixed bug 32
'
' Revision 1.30  2008/01/24 14:43:09  jeroens
' Added SecundaryIndexOffset
'
' Revision 1.29  2008/01/15 13:43:05  jeroens
' Fixed long-standing bug in determining secundary source of variables
'
' Revision 1.28  2007/12/21 16:01:20  jeroens
' * Build tracer refresh support
'
' Revision 1.27  2007/09/25 16:54:30  jeroens
' * iGroup -> iIndex
'
' Revision 1.26  2007/08/27 02:28:51  jeroens
' * Fixed bug. No I didn't. Did I? Erm.. no, neatified code instead
'
' Revision 1.25  2007/07/11 00:38:28  jeroens
' + Introduced selective refresh
'
' Revision 1.24  2007/07/06 20:11:18  jeroens
' * Core stanza group list no longer exposed
'
' Revision 1.23  2007/05/31 13:11:20  jeroens
' * Renamed StyleGuide StyleFlags to eStyleFlags
'
' Revision 1.22  2006/11/23 04:25:09  jeroens
' + Added ExtractProperty
'
' Revision 1.21  2006/11/19 04:06:11  jeroens
' + Ecopath data added or removed message will clear the properties cache
'
' Revision 1.20  2006/10/16 01:15:48  jeroens
' * Do not update on irrelevant message types
'
' Revision 1.19  2006/09/21 00:58:42  jeroens
' - Removed ToDo, we're all good here
'
' Revision 1.18  2006/07/12 16:34:46  jeroens
' - Reverted silly property secundary index ID. Secundary object was solid and that will stay, other interfaces just muddle the design.
'
' Revision 1.17  2006/07/10 18:43:32  jeroens
' + Some values require an Index number rather than an object. This is true for for instance Fleet.FixedCost.
'
' Revision 1.16  2006/07/06 15:51:24  jeroens
' + Added base logic to resolve ICoreInterface object for messages with an arrayIndex, but without the secundary object properly filled
'
' Revision 1.15  2006/07/03 04:19:10  jeroens
' + Added support for cIntegerProperty, cBooleanProperty
'
' Revision 1.14  2006/06/28 13:59:28  jeroens
' * Renamed iGroup member vars, properties to Index
' * Renamed GroupName vartype and usage to Name where applicable
' * Merged usage of varName Name (fleet) with GroupName
'
' Revision 1.13  2006/06/21 03:01:08  jeroens
' Fixed m_Status VarName vs. VarType confusion
'
' Revision 1.12  2006/06/19 23:35:37  jeroens
' + Added Clear()
'
' Revision 1.11  2006/06/14 04:15:22  cvsuser
' + JS: Added default 'No Data' property to return if a property is requested for an undefined source
'
' Revision 1.10  2006/06/13 08:29:01  cvsuser
' * Secundary index now an object, not an integer. The secundary index object will resolve its iGroup at runtime to allow for dynamic object creation and destruction
'
' Revision 1.9  2006/06/07 15:37:26  jeroens
' + Set up JS homework
'
' Revision 1.8  2006/06/07 03:40:57  jeroens
' + Updated to cCoreInputOutput / ICoreInterface changes
'
' Revision 1.7  2006/06/06 15:01:15  jeroens
' + Added work-around to accept new ICoreInterface baseclass. This needs to be changed structurally in the near future!
'
' Revision 1.6  2006/05/03 13:33:47  cvsuser
' * Changed ProperytIDGenerator to cCore.cValueID
'
' Revision 1.5  2006/04/11 16:28:16  cvsuser
' - Removed MakeProperty
' + Property no longer generated via Type; was too slow.
'
' Revision 1.4  2006/03/21 03:05:54  cvsuser
' + Fixed crash when attempting to get a non-existing property from the internal storage
'
' Revision 1.3  2006/03/17 05:12:20  cvsuser
' * N -> n :(
'
' Revision 1.2  2006/03/17 04:45:47  cvsuser
' * Moved to Other.Properties - reprise
'
' Revision 1.1  2006/03/17 04:43:33  cvsuser
' * Moved to Other.Properties
'
' Revision 1.7  2006/03/16 19:06:02  cvsuser
' cVariableStatus now has a iArrayIndex property
'
' Revision 1.6  2006/03/15 19:23:48  cvsuser
' * Option Strict ON again
'
' Revision 1.5  2006/03/15 19:20:03  cvsuser
' + Responds to EcoPath messages
' * Fixed crash on attempt to load non-existant property
'
' Revision 1.4  2006/03/15 02:48:31  cvsuser
' * Simplified message variable resolution with new core information
' + Uses cCore.VALUE_NULL instead of -1
'
' Revision 1.3  2006/03/14 16:41:26  cvsuser
' + Properties should not be created in response to Core messages
'
' Revision 1.2  2006/03/14 16:34:19  cvsuser
' + Added Core message handling to trigger property updates
'
' Revision 1.1  2006/03/13 05:43:28  cvsuser
' Initial version
'
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports eweCore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Style
Imports System.ComponentModel

#End Region ' Imports

Namespace Properties

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Property factory and storage
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cPropertyManager

        ''' <summary>Message handler synchronizer.</summary>
        Private m_sync As ISynchronizeInvoke = Nothing

        ''' <summary>Error property</summary>
        Private m_propNoData As cStringProperty = Nothing

        ''' <summary>Quick property lookup tables</summary>
        Private m_htGeneric As New Dictionary(Of String, cProperty)
        Private m_htEcopath As New Dictionary(Of String, cProperty)
        Private m_htEcosim As New Dictionary(Of String, cProperty)
        Private m_htEcospace As New Dictionary(Of String, cProperty)
        Private m_htEcotracer As New Dictionary(Of String, cProperty)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub New()
            ' Create No Data property
            Me.m_propNoData = New cStringProperty("")
            Me.m_propNoData.SetStyle(StyleGuide.eStyleFlags.ErrorEncountered Or StyleGuide.eStyleFlags.NotEditable)
            Me.m_propNoData.SetValue(My.Resources.GENERIC_TEXT_NODATA)
            ' Start listening to core messages
            Me.InitializeMessageHandlers()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Clears the properties cache, useful when loading new models.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Clear(ByVal msgSource As eCoreComponentType)

            Select Case msgSource
                Case eCoreComponentType.EcoPath
                    Me.m_htGeneric.Clear()
                    Me.m_htEcopath.Clear()
                    Me.m_htEcosim.Clear()
                    Me.m_htEcospace.Clear()
                    Me.m_htEcotracer.Clear()

                Case eCoreComponentType.EcoSim
                    Me.m_htEcosim.Clear()
                    Me.m_htEcospace.Clear()
                    Me.m_htEcotracer.Clear()

                Case eCoreComponentType.EcoSpace
                    Me.m_htEcospace.Clear()
                    Me.m_htEcotracer.Clear()

                Case eCoreComponentType.Ecotracer
                    Me.m_htEcotracer.Clear()

            End Select

        End Sub

#Region " Singleton "

        ''' <summary>Singleton instance</summary>
        Private Shared __inst__ As cPropertyManager = Nothing

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Singleton access
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Shared Function GetInstance() As cPropertyManager
            If (cPropertyManager.__inst__ Is Nothing) Then
                cPropertyManager.__inst__ = New cPropertyManager()
            End If
            Return cPropertyManager.__inst__
        End Function

#End Region ' Singleton

#Region " Config "

        Public Property SyncObject() As ISynchronizeInvoke
            Get
                Return Me.m_sync
            End Get
            Set(ByVal value As ISynchronizeInvoke)
                Me.m_sync = value
            End Set
        End Property

#End Region ' Config

#Region " Public property access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a manually made property
        ''' </summary>
        ''' <param name="strID">The ID of the property</param>
        ''' -------------------------------------------------------------------
        Public Function GetProperty(ByVal strID As String) As cProperty

            ' Return a property from the internal storage
            If (Not Me.m_htGeneric.ContainsKey(strID)) Then
                Return Nothing
            End If
            Return Me.m_htGeneric(strID)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a property for specific core data
        ''' </summary>
        ''' <param name="Source">The <see cref="cCoreInputOutputBase">cCoreInputOutputBase</see> instance to generate the property for</param>
        ''' <param name="VarName">The <see cref="eVarNameFlags">Variable Name</see> within the indicated Source to generate the property for</param>
        ''' <param name="SourceSec">Optional <see cref="cCoreInputOutputBase">secundary cCoreInputOutputBase data source</see> in case the variable name indicates an indexed variable.</param>
        ''' <param name="bAllowedToCreate">States that the property should be created if it does not exist</param>
        ''' <remarks>The property is generated if it does not exist yet</remarks>
        ''' -------------------------------------------------------------------
        Public Function GetProperty(ByVal Source As cCoreInputOutputBase, ByVal VarName As eVarNameFlags, Optional ByVal SourceSec As cCoreInputOutputBase = Nothing, _
                Optional ByVal bAllowedToCreate As Boolean = True, _
                Optional ByVal iSecundaryIndexOffset As Integer = 0) As cProperty

            Dim strID As String = Nothing
            Dim prop As cProperty = Nothing
            Dim iIndex As Integer = cCore.NULL_VALUE
            Dim ValTest As ValueWrapper.cValue = Nothing
            Dim t As Type = Nothing
            Dim ht As Dictionary(Of String, cProperty) = Nothing

            ' Does not source exist?
            If (Source Is Nothing) Then
                ' #Yes: return system wide 'No Data' property to prevent
                ' code that expects a property from crashing.
                Return Me.m_propNoData
            End If

            ' Get an ID for this property
            strID = cValueID.Generate(Source, VarName, SourceSec)
            Select Case Source.CoreComponent
                Case eCoreComponentType.EcoPath : ht = Me.m_htEcopath
                Case eCoreComponentType.EcoSim : ht = Me.m_htEcosim
                Case eCoreComponentType.EcoSpace : ht = Me.m_htEcospace
                Case eCoreComponentType.Ecotracer : ht = Me.m_htEcotracer
                Case Else : ht = Me.m_htGeneric
            End Select

            ' Has property been used already?
            If ht.ContainsKey(strID) Then
                ' #Yes: return it
                Return ht(strID)
            End If

            ' Property does not exist. Allowed to create it?
            If Not bAllowedToCreate Then
                ' #No: abort
                Return Nothing
            End If

            ' Determine source data type
            If SourceSec IsNot Nothing Then iIndex = SourceSec.Index - iSecundaryIndexOffset

            ValTest = Source.ValueDescriptor(VarName)

            If ValTest Is Nothing Then
                Debug.Assert(False, String.Format("Source {0} does not support varname {1}", Source.Name, VarName.ToString()))
            Else
                Select Case ValTest.varType
                    Case ValueWrapper.eValueTypes.Bool, ValueWrapper.eValueTypes.BoolArray
                        prop = New cBooleanProperty(Source, VarName, SourceSec, iSecundaryIndexOffset)
                    Case ValueWrapper.eValueTypes.Int, ValueWrapper.eValueTypes.IntArray
                        prop = New cIntegerProperty(Source, VarName, SourceSec, iSecundaryIndexOffset)
                    Case ValueWrapper.eValueTypes.Sng, ValueWrapper.eValueTypes.SingleArray
                        prop = New cSingleProperty(Source, VarName, SourceSec, iSecundaryIndexOffset)
                    Case ValueWrapper.eValueTypes.Str
                        prop = New cStringProperty(Source, VarName, SourceSec, iSecundaryIndexOffset)
                    Case Else
                        Debug.Assert(False, String.Format("Cannot generate property {0} for cValue type {1}", strID, ValTest.varType))
                End Select
            End If

            If prop Is Nothing Then Return Nothing

            ' Store property
            ht(strID) = prop
            ' Make sure property is up to date
            prop.Refresh()

            Return prop
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Extract a <see cref="cProperty">Property</see> from a given 
        ''' <see cref="cVariableStatus">VariableStatus</see>.
        ''' </summary>
        ''' <param name="vs">VariableStatus to find the Property instance for.</param>
        ''' <returns>A cProperty instance, or Nothing if unsuccesful.</returns>
        ''' -------------------------------------------------------------------
        Public Function ExtractProperty(ByVal vs As cVariableStatus) As cProperty
            Dim prop As cProperty = Nothing
            Dim source As cCoreInputOutputBase = Nothing
            Dim sourceSec As cCoreInputOutputBase = Nothing

            ' Attempt to find an existing property for this variable
            source = DirectCast(vs.CoreDataObject, cCoreInputOutputBase)
            sourceSec = DirectCast(vs.CoreDataObjectSecundary, cCoreInputOutputBase)

            ' Does the message contain an accompanying object for a valid secundary index?
            If (vs.iArrayIndex >= 0 And vs.CoreDataObjectSecundary Is Nothing) Then
                ' #No: oops!

                ' Try to figure out the type of the secundary index via core counters
                Dim io As cCoreInputOutputBase = DirectCast(source, cCoreInputOutputBase)
                Dim va As ValueWrapper.cValueArray = DirectCast(io.ValueDescriptor(vs.VarName), ValueWrapper.cValueArray)
                Dim core As cCore = cCore.GetInstance()

                If va IsNot Nothing Then
                    Select Case va.CoreCounterType
                        Case eCoreCounterTypes.nGroups, eCoreCounterTypes.nDetritus, eCoreCounterTypes.nLivingGroups
                            sourceSec = core.EcoPathGroupInputs(vs.iArrayIndex)
                        Case eCoreCounterTypes.nFleets
                            sourceSec = core.FleetInputs(vs.iArrayIndex)
                        Case eCoreCounterTypes.nHabitats
                            sourceSec = core.EcospaceHabitats(vs.iArrayIndex)
                        Case eCoreCounterTypes.nRegions
                            sourceSec = core.EcospaceRegions(vs.iArrayIndex)
                        Case eCoreCounterTypes.nMPAs
                            sourceSec = core.EcospaceMPAs(vs.iArrayIndex)
                        Case eCoreCounterTypes.nMonths, _
                             eCoreCounterTypes.nEcosimYears, eCoreCounterTypes.nEcosimTimeSteps, _
                             eCoreCounterTypes.nEcospaceYears, eCoreCounterTypes.nEcospaceTimeSteps
                            sourceSec = Nothing
                        Case Else
                            Debug.Assert(False, String.Format("Core counter type {0} not supported in property manager", va.CoreCounterType))
                    End Select
                Else
                    ' Hmm?
                End If
            End If

            ' js 07/jun/06 type check to cast to cCoreInputOutputBase
            If TypeOf source Is cCoreInputOutputBase Then
                prop = Me.GetProperty(source, vs.VarName, sourceSec, False)
            End If

            Return prop
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Extract all properties from a core message
        ''' </summary>
        ''' <param name="Message">Core message to analyze</param>
        ''' <returns>A strong-typed cProperty list</returns>
        ''' -------------------------------------------------------------------
        Public Function ExtractProperties(ByRef Message As cMessage) As List(Of cProperty)

            Dim lProps As New List(Of cProperty)
            Dim prop As cProperty = Nothing

            ' Validate message
            If Message Is Nothing Then Return lProps

            ' For all variables in the message
            For Each vs As cVariableStatus In Message.Variables

                ' Resolve property for vs
                prop = Me.ExtractProperty(vs)
                ' Add to list of props if resolved
                If (prop IsNot Nothing) Then lProps.Add(prop)

            Next
            Return lProps

        End Function

#End Region ' Public property access

#Region " Refresh management "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Refresh the core values of all properties
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Refresh(ByVal msgSource As eCoreComponentType)

            Select Case msgSource
                Case eCoreComponentType.EcoPath
                    For Each prop As cProperty In Me.m_htGeneric.Values
                        ' Refresh yourself
                        prop.Refresh()
                    Next
                    For Each prop As cProperty In Me.m_htEcopath.Values
                        ' Refresh yourself
                        prop.Refresh()
                    Next
                    For Each prop As cProperty In Me.m_htEcosim.Values
                        ' Refresh yourself
                        prop.Refresh()
                    Next
                    For Each prop As cProperty In Me.m_htEcospace.Values
                        ' Refresh yourself
                        prop.Refresh()
                    Next
                    For Each prop As cProperty In Me.m_htEcotracer.Values
                        ' Refresh yourself
                        prop.Refresh()
                    Next

                Case eCoreComponentType.EcoSim
                    For Each prop As cProperty In Me.m_htGeneric.Values
                        ' Refresh yourself
                        prop.Refresh()
                    Next
                    For Each prop As cProperty In Me.m_htEcosim.Values
                        ' Refresh yourself
                        prop.Refresh()
                    Next
                    For Each prop As cProperty In Me.m_htEcospace.Values
                        ' Refresh yourself
                        prop.Refresh()
                    Next
                    For Each prop As cProperty In Me.m_htEcotracer.Values
                        ' Refresh yourself
                        prop.Refresh()
                    Next

                Case eCoreComponentType.EcoSpace
                    For Each prop As cProperty In Me.m_htGeneric.Values
                        ' Refresh yourself
                        prop.Refresh()
                    Next
                    For Each prop As cProperty In Me.m_htEcospace.Values
                        ' Refresh yourself
                        prop.Refresh()
                    Next
                    For Each prop As cProperty In Me.m_htEcotracer.Values
                        ' Refresh yourself
                        prop.Refresh()
                    Next

                Case eCoreComponentType.Ecotracer
                    For Each prop As cProperty In Me.m_htGeneric.Values
                        ' Refresh yourself
                        prop.Refresh()
                    Next
                    For Each prop As cProperty In Me.m_htEcotracer.Values
                        ' Refresh yourself
                        prop.Refresh()
                    Next

            End Select
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Hook up to core messages
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub InitializeMessageHandlers()

            Dim core As cCore = cCore.GetInstance()

            'core.Messages.AddMessageHandler(New cMessageHandler(AddressOf Me.AllMessagesHandler, eCoreComponentType.ShapesManager, eMessageType.Any))
            core.Messages.AddMessageHandler(New cMessageHandler(AddressOf Me.AllMessagesHandler, eCoreComponentType.EcoPath, eMessageType.Any, Me.m_sync))
            core.Messages.AddMessageHandler(New cMessageHandler(AddressOf Me.AllMessagesHandler, eCoreComponentType.EcoSim, eMessageType.Any, Me.m_sync))
            core.Messages.AddMessageHandler(New cMessageHandler(AddressOf Me.AllMessagesHandler, eCoreComponentType.EcoSpace, eMessageType.Any, Me.m_sync))
            core.Messages.AddMessageHandler(New cMessageHandler(AddressOf Me.AllMessagesHandler, eCoreComponentType.Ecotracer, eMessageType.Any, Me.m_sync))

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Standard Core messages handler where all property updates are triggered
        ''' </summary>
        ''' <param name="msg">An arriving message</param>
        ''' -------------------------------------------------------------------
        Private Sub AllMessagesHandler(ByRef msg As cMessage)

            ' Get properties related to message
            Dim lProps As List(Of cProperty) = Nothing
            Dim prop As cProperty = Nothing

            ' Respond to major events
            If (msg.Type = eMessageType.DataAddedOrRemoved) And (msg.Source = eCoreComponentType.EcoPath) Then
                ' Clear existing properties when number of ecopath groups has changed
                Me.Clear(msg.Source)
                ' No need to proceed since all Properties are gone
                Return
            End If

            ' Ignore irrelevant messages
            If msg.Type = eMessageType.DataImport Then Return

            lProps = Me.ExtractProperties(msg)

            If lProps.Count = 0 Then
                ' Update everything (ouch)
                Me.Refresh(msg.Source)
            Else
                ' Update each property in this message
                For Each prop In lProps
                    prop.Refresh()
                Next
            End If

        End Sub

#End Region ' Refresh management

    End Class

End Namespace
