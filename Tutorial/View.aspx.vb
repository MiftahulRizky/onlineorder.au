Imports System.Data

Partial Class Tutorial_View
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("tutorialDetail") = "" Then
            Response.Redirect("~/tutorial", False)
            Exit Sub
        End If

        lblId.Text = Session("tutorialDetail")
        If Not IsPostBack Then
            Call BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnFinish_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/tutorial", False)
    End Sub

    Private Sub BindData(Id As String)
        Call MessageError(False, String.Empty)
        Try
            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM Tutorials WHERE Id = '" + UCase(Id).ToString() + "'")
            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/tutorial", False)
                Exit Sub
            End If

            cardTitle.InnerText = myData.Tables(0).Rows(0).Item("Title").ToString()
            Dim fileTutorial As String = myData.Tables(0).Rows(0).Item("File").ToString() & "#scrollbar=0&view=Fit"

            Dim thisString As String = String.Format("https://onlineorder.au/File/Tutorial/PDF/{0}", fileTutorial)

            embPdf.Attributes.Add("src", thisString)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindData", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub
End Class
