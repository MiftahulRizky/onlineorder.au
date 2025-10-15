Partial Class Setting_Member_Online
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Call BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnFinish_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/member", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Call BindData(txtSearch.Text)
    End Sub

    Private Sub BindData(SearchText As String)
        Call MessageError(False, String.Empty)
        Try
            Dim search As String = ""
            If Not SearchText = "" Then
                search = " AND UserName LIKE '%" + SearchText + "%'"
            End If

            Dim TimeThreshold As String = String.Format("{0:yyyy-MM-dd hh:mm:ss}", Now().AddSeconds(-30))

            Dim myQuery As String = String.Format("SELECT * FROM view_memberships WHERE LastActivityDate > '" + TimeThreshold + "' {0} ORDER BY UserName ASC", search)

            lvMember.DataSource = publicCfg.GetListData(myQuery)
            lvMember.DataBind()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub
End Class
