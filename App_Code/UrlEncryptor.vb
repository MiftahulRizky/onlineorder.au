

Imports System
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text

Public Class UrlEncryptor

    ' Key sementara untuk pengujian.
    ' Harus sama untuk proses Encrypt dan Decrypt.
    Private Shared ReadOnly EncryptionKey As Byte() =
        Encoding.UTF8.GetBytes("12345678901234567890123456789012")

    Public Function Encrypt(ByVal plainText As String) As String

        If plainText Is Nothing Then
            Throw New ArgumentNullException("plainText")
        End If

        Using aes As Aes = Aes.Create()

            aes.Key = EncryptionKey
            aes.Mode = CipherMode.CBC
            aes.Padding = PaddingMode.PKCS7
            aes.GenerateIV()

            Using output As New MemoryStream()

                ' Simpan IV di awal data terenkripsi.
                output.Write(aes.IV, 0, aes.IV.Length)

                Using crypto As New CryptoStream(
                    output,
                    aes.CreateEncryptor(),
                    CryptoStreamMode.Write
                )
                    Dim plainBytes As Byte() =
                        Encoding.UTF8.GetBytes(plainText)

                    crypto.Write(plainBytes, 0, plainBytes.Length)
                    crypto.FlushFinalBlock()
                End Using

                ' Konversi ke Base64Url.
                Return Convert.ToBase64String(output.ToArray()) _
                    .TrimEnd("="c) _
                    .Replace("+", "-") _
                    .Replace("/", "_")

            End Using
        End Using

    End Function

    Public Function Decrypt(ByVal encryptedText As String) As String

        If String.IsNullOrWhiteSpace(encryptedText) Then
            Throw New ArgumentException(
                "Encrypted value cannot be empty.",
                "encryptedText"
            )
        End If

        Dim base64 As String =
            encryptedText.Replace("-", "+").Replace("_", "/")

        Select Case base64.Length Mod 4
            Case 0
                ' Tidak perlu menambahkan padding.
            Case 2
                base64 &= "=="
            Case 3
                base64 &= "="
            Case Else
                Throw New FormatException(
                    "Invalid Base64Url length."
                )
        End Select

        Dim encryptedBytes As Byte() =
            Convert.FromBase64String(base64)

        ' Minimal 16 byte IV + 16 byte ciphertext.
        If encryptedBytes.Length < 32 OrElse
           (encryptedBytes.Length - 16) Mod 16 <> 0 Then

            Throw New FormatException(
                "Invalid AES ciphertext length."
            )
        End If

        Using aes As Aes = Aes.Create()

            aes.Key = EncryptionKey
            aes.Mode = CipherMode.CBC
            aes.Padding = PaddingMode.PKCS7

            ' Ambil 16 byte pertama sebagai IV.
            Dim iv(15) As Byte
            Array.Copy(encryptedBytes, 0, iv, 0, 16)
            aes.IV = iv

            Using input As New MemoryStream(
                encryptedBytes, 16, encryptedBytes.Length - 16
            )
                Using crypto As New CryptoStream(
                    input,
                    aes.CreateDecryptor(),
                    CryptoStreamMode.Read
                )
                    Using reader As New StreamReader(
                        crypto, Encoding.UTF8
                    )
                        Return reader.ReadToEnd()
                    End Using
                End Using
            End Using

        End Using

    End Function

End Class

