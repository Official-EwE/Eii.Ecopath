' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.SystemUtilities



''' ---------------------------------------------------------------------------
''' <summary>
''' Storage class for system-wide, model independent EwE core settings.
''' </summary>
''' ---------------------------------------------------------------------------
Friend Class cCoreSettings

#Region " Private vars "

    ''' <summary>Autosave flags</summary>
    Private m_bAutosave() As Boolean

#End Region ' Private vars

#Region " Constructor "

    Public Sub New()
        ReDim Me.m_bAutosave([Enum].GetValues(GetType(eAutosaveTypes)).Length)
        Me.ThreatCount = cSystemUtils.ProcessorCount
    End Sub

#End Region ' Constructor

#Region " Accessors "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether a component is allowed to auto-save.
    ''' </summary>
    ''' <param name="t">The <see cref="eAutosaveTypes">auto-save enabled component</see>
    ''' to enable or disable.</param>
    ''' -----------------------------------------------------------------------
    Public Property Autosave(t As eAutosaveTypes) As Boolean
        Get
            Return Me.m_bAutosave(t)
        End Get
        Set(value As Boolean)
            Me.m_bAutosave(t) = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether a component is allowed to auto-save.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property AutosaveHeaders() As Boolean = True

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the core output path.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property OutputPath As String = ""

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the model backup path mask.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BackupFileMask As String = ""

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the default EwE author.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Author As String = ""

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the default EwE author contact.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Contact As String = ""

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the number of threads that can be used by computations.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property ThreatCount As Integer = 1

#End Region ' Accessors

End Class
