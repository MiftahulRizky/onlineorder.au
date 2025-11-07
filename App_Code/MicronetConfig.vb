Imports System.IO
Imports System.Net
Imports WinSCP

Public Class MicronetConfig

    Public Function Connect(key As String) As String
        Dim result As String = String.Empty
        Try
            Dim host As String = "LS-CLOUD-1.lifestyleblinds.com.au"
            Const username As String = "user684207"
            Dim privateKeyFile As String = key
            Dim port As Integer = 22222
            Dim hostKeyFingerprint As String = "ssh-ed25519 255 uGBJef6J0xC1VtTSen/OiYQSAX7NKlb9fzGuH0vRLOw"

            Dim sessionOptions As New SessionOptions With {
                .Protocol = Protocol.Sftp,
                .HostName = host,
                .UserName = username,
                .SshPrivateKeyPath = privateKeyFile,
                .PortNumber = port,
                .SshHostKeyFingerprint = hostKeyFingerprint
            }

            Using session As New Session()
                session.Open(sessionOptions)
                If session.Opened Then
                    result = "SUCCESS. SFTP CONNECTED SUCCESSFULLY."
                Else
                    result = "ERROR. FAILED TO CONNECT TO SFTP SERVER."
                End If
            End Using
        Catch ex As Exception
            result = "ERROR. " & ex.Message & " | " & ex.ToString()
        End Try
        Return result
    End Function

    Public Function Upload(key As String, inv As String) As String
        Dim result As String = String.Empty
        Try
            Dim host As String = "LS-CLOUD-1.lifestyleblinds.com.au"
            Const username As String = "user684207"
            Dim privateKeyFile As String = key
            Dim port As Integer = 22222
            Dim hostKeyFingerprint As String = "ssh-ed25519 255 uGBJef6J0xC1VtTSen/OiYQSAX7NKlb9fzGuH0vRLOw"

            Dim sessionOptions As New SessionOptions With {
                .Protocol = Protocol.Sftp,
                .HostName = host,
                .UserName = username,
                .SshPrivateKeyPath = privateKeyFile,
                .PortNumber = port,
                .SshHostKeyFingerprint = hostKeyFingerprint
            }

            Using session As New Session()
                session.Open(sessionOptions)
                If session.Opened Then
                    Dim localPath As String = Path.GetTempFileName()
                    Using webClient As New WebClient()
                        Try
                            webClient.DownloadFile(inv, localPath)
                        Catch ex As Exception
                            Return "FAILED TO DOWNLOAD FILE FROM URL: " & ex.Message
                        End Try
                    End Using

                    Dim remoteFileName As String = Path.GetFileName(New Uri(inv).LocalPath)
                    Dim remotePath As String = "./" & remoteFileName

                    Dim transferOptions As New TransferOptions With {
                        .TransferMode = TransferMode.Binary
                    }

                    Dim transferResult As TransferOperationResult = session.PutFiles(localPath, remotePath, False, transferOptions)

                    If transferResult.IsSuccess Then
                        result = "SUCCEED PLEASE CHECK AND PROCESS IN MICRONET."
                    Else
                        Dim errorMsg As String = "UPLOAD FAILED: "
                        For Each failure In transferResult.Failures
                            errorMsg &= failure.ToString() & "; "
                        Next
                        result = errorMsg
                    End If

                    Try
                        If File.Exists(localPath) Then File.Delete(localPath)
                    Catch ex As Exception
                        result &= " (WARNING: FAILED TO DELETE TEMP FILE: " & ex.Message & ")"
                    End Try
                Else
                    result = "FAILED TO CONNECT TO SFTP SERVER."
                End If
            End Using
        Catch ex As Exception
            result = "FAILED: " & ex.Message
        End Try
        Return result
    End Function
End Class
