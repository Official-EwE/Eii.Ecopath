' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports System.Reflection
Imports EwEUtils

Namespace My

    ''' <summary>
    ''' Settings class that uses a custom <see cref="System.Configuration.SettingsProvider"/>.
    ''' </summary>
    ''' <remarks>
    ''' For details about the overridden settings behaviour refer to <see cref="cEwESettingsProvider"/>.
    ''' </remarks>
    Partial Friend NotInheritable Class MySettings

        ''' <summary>Custom <see cref="cEwESettingsProvider">settings provider</see>.</summary>
        Private m_provider As cEwESettingsProvider = Nothing

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New()

            MyBase.New()

            Dim asm As Assembly = Assembly.GetAssembly(GetType(MySettings))
            Me.m_provider = New cEwESettingsProvider(Path.GetFileNameWithoutExtension(asm.Location), Me)

        End Sub

    End Class

End Namespace
