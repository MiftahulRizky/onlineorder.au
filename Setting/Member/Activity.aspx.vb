Partial Class Setting_Member_Activity
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/member", False)
            Exit Sub
        End If

        If Session("memberActivity") = "" Then
            Response.Redirect("~/setting/member", False)
            Exit Sub
        End If

        If Session("memberActivity") = "Only" And Session("memberDetail") = "" Then
            Response.Redirect("~/setting/member", False)
            Exit Sub
        End If

        lblActivity.Text = Session("memberActivity").ToString()
        lblUserId.Text = UCase(Session("memberDetail")).ToString()

        If Not IsPostBack Then
            Call BindData(txtSearch.Text, lblActivity.Text, lblUserId.Text)
        End If
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            sdsPage.Delete()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "RESET MEMBER ACTIVITY. USER ID : " & lblUserId.Text)

            Response.Redirect("~/setting/member/activity", False)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnFinish_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/member", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Call BindData(txtSearch.Text, lblActivity.Text, lblUserId.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Call MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            Call BindData(txtSearch.Text, lblActivity.Text, lblUserId.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindData(SearchText As String, Activity As String, UserId As String)
        Call MessageError(False, String.Empty)
        Try
            Dim search As String = ""
            Dim role As String = ""

            cardTitle.InnerHtml = "Activity All Users"

            If Not SearchText = "" Then
                search = " WHERE Page LIKE '%" + SearchText + "%'"
            End If

            If Activity = "Only" Then
                role = " WHERE UserId = '" + UCase(UserId).ToString() + "'"
                If Not SearchText = "" Then
                    search = " AND Page LIKE '%" + SearchText + "%'"
                End If

                cardTitle.InnerHtml = "Activity " & "<u>" & publicCfg.GetItemData("SELECT UserName FROM Memberships WHERE UserId = '" + UCase(UserId).ToString() + "'") & "</u>"
            End If

            Dim myQuery As String = String.Format("SELECT * FROM MemberActivity {0} {1} ORDER BY Created DESC", role, search)

            gvList.DataSource = publicCfg.GetListData(myQuery)
            gvList.DataBind()

            btnDelete.Visible = False
            If Session("RoleName") = "Administrator" And Activity = "All" Then
                btnDelete.Visible = True
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub
End Class
