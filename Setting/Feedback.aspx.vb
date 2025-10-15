Partial Class Setting_Feedback
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnReset_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            sdsReset.Delete()

            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Call BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Call MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindData(SearchText As String)
        Call MessageError(False, String.Empty)
        Try
            Dim search As String = ""
            If Not SearchText = "" Then
                search = " WHERE Memberships.UserName LIKE '%" + SearchText + "%'"
            End If

            gvList.DataSource = publicCfg.GetListData(String.Format("SELECT Feedbacks.*, Memberships.UserName AS UserName FROM Feedbacks INNER JOIN Memberships ON Feedbacks.UserId = Memberships.UserId {0} ORDER BY Created DESC", search))
            gvList.DataBind()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub
End Class
