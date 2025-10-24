Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.Web.UI
Imports System.Web
Imports System.Web.UI.WebControls

Partial Class Account_KeepSessionAlive
    Inherits System.Web.UI.Page
     Dim publicCfg As New PublicConfig
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)

       ' Pastikan request method POST
        If Request.HttpMethod <> "POST" Then
            Response.StatusCode = 405 ' Method Not Allowed
            Response.End()
            Return
        End If

        ' Jika session sudah tidak ada, balas 401 (Unauthorized)
        If Session Is Nothing OrElse Session.IsNewSession Then
            Response.StatusCode = 401
            Response.End()
            Return
        End If

        ' Perbarui session dengan menyentuh semua key (agar timeout di-reset)
        For Each key As String In Session.Keys
            Dim val = Session(key)
            Session(key) = val
        Next

        ' Kirim response sukses (200)
        Response.StatusCode = 200
        Response.Write("Session refreshed")
        Response.End()

    End Sub
End Class
