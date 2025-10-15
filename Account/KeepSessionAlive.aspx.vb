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
        ' Cek apakah session masih ada
        If Session("IsLoggedIn") IsNot Nothing AndAlso Session("IsLoggedIn") = True Then
            ' Perbarui waktu aktif
            Session("LastPing") = Now()

            ' (Opsional) bisa isi ulang session yang penting dari database, mirip login
            ' Contoh ringan:

            Dim memberData As DataSet = publicCfg.GetListData("SELECT * FROM view_memberships WHERE UserId = '" + Session("UserId") + "'")

            If memberData.Tables(0).Rows.Count > 0 Then
                Session("ApplicationId") = memberData.Tables(0).Rows(0).Item("ApplicationId").ToString()
                Session("UserName") = memberData.Tables(0).Rows(0).Item("UserName").ToString()
                Session("UserId") = memberData.Tables(0).Rows(0).Item("UserId").ToString()
            End If

            If Session("rollName") = "Administrator" Then
                Response.StatusCode = 200
            End If
        Else
            Response.StatusCode = 401 ' Unauthorized
        End If

        Response.End()
    End Sub
End Class
