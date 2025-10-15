Partial Class Setting_Member_Default
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("RoleName") <> "Administrator" AndAlso Session("RoleName") <> "Account" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call MessageError(False, String.Empty)

            txtSearch.Text = Session("memberSearch")
            Call BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Call BindData(txtSearch.Text)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("memberSearch") = txtSearch.Text
        Response.Redirect("~/setting/member/add", False)
    End Sub

    Protected Sub linkOnline_Click(sender As Object, e As EventArgs)
        Session("memberSearch") = txtSearch.Text
        Response.Redirect("~/setting/member/online", False)
    End Sub

    Protected Sub linkActivityAll_Click(sender As Object, e As EventArgs)
        Session("memberSearch") = txtSearch.Text
        Session("memberActivity") = "All"
        Session("memberDetail") = String.Empty

        Response.Redirect("~/setting/member/activity", False)
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

    Protected Sub linkDetail_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Session("memberDetail") = UCase(row.Cells(1).Text)
            Session("memberSearch") = txtSearch.Text

            Response.Redirect("~/setting/member/detail", False)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            lblUserId.Text = txtIdDelete.Text

            sdsMemberhips.Delete()
            sdsUsers.Delete()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "DELETE MEMBERSHIPS. USER ID : " & lblUserId.Text)
            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub linkActive_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Dim active As String = row.Cells(6).Text

            Dim newActive As Integer = 1
            If active = "True" Then : newActive = 0 : End If

            lblUserId.Text = UCase(row.Cells(1).Text)
            lblLockoutEnabled.Text = newActive

            sdsMemberhips.Update()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "UPDATE ACTIVE USERS. USER ID : " & lblUserId.Text)

            Call BindData(txtSearch.Text)

        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnReset_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            lblUserId.Text = txtIdReset.Text

            lblPassword.Text = "123456"
            lblPasswordHash.Text = publicCfg.Encrypt(lblPassword.Text)
            sdsResetPassword.Update()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "RESET PASSWORD. USER ID : " & lblUserId.Text)

            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub linkActivity_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Session("memberSearch") = txtSearch.Text
            Session("memberActivity") = "Only"
            Session("memberDetail") = UCase(row.Cells(1).Text)

            Response.Redirect("~/setting/member/activity", False)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindData(SearchText As String)
        Session("memberDetail") = "" : Session("memberSearch") = "" : Session("memberActivity") = ""
        Try
            Dim search As String = ""

            If Not SearchText = "" Then
                search = " AND (StoreId LIKE '%" + SearchText + "%' OR  StoreName LIKE '%" + SearchText + "%' OR UserName LIKE '%" + SearchText + "%' OR StoreName LIKE '%" + SearchText + "%' OR Access LIKE '%" + SearchText + "%')"
            End If

            gvList.DataSource = publicCfg.GetListData(String.Format("SELECT * FROM view_memberships WHERE AppActive=1 {0} ORDER BY UserName ASC", search))
            gvList.DataBind()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub

    Protected Function IconActiveOnNames(LockoutEnabled As String, Username As String) As String
        Dim result As String = "<i class='fa-regular text-green fa-circle-check' style='margin-right: 0.3rem;'></i> " + Username
        If LockoutEnabled = "True" Then : Return "<i class='fa-regular text-danger fa-circle-xmark' style='margin-right: 0.3rem;'></i> " + Username : End If
        Return result
    End Function

    Protected Function CssActive(Active As Boolean) As String
        Dim result As String = "dropdown-item"
        ' Dim result As String = "btn btn-green"
        ' If Active = True Then : result = "btn btn-secondary" : End If
        Return result
    End Function

    Protected Function TextActive(Active As String) As String
        Dim result As String = "<i class='bi bi-arrow-repeat fs-3 me-2 opacity-50'></i>Swicth Activated"
        ' Dim result As String = "<i class='bi bi-arrow-repeat fs-3'></i>"
        ' If Active = True Then : Return "<i class='bi bi-arrow-repeat fs-3'></i>" : End If
        Return result
    End Function

    Protected Function VisibleUsername(UserName As String) As String
        Dim result As Boolean = False
         If Session("RoleName") = "Administrator" Or Session("RoleName") = "Account" Then 
            result = True
            If Username = "admin" AND Session("RoleName") = "Account" Then 
                result = false 
            End If
        End If
        Return result
    End Function

    Protected Function VisibleDelete(UserName As String) As String
        Dim result As Boolean = False
        'If Session("RoleName") = "Administrator" Or Session("RoleName") = "Account" Then : result = True : End If
        If Session("RoleName") = "Administrator" Or Session("RoleName") = "Account" Then 
            result = True
            If Username = "admin" AND Session("RoleName") = "Account" Then 
                result = false 
            End If
        End If
        Return result
    End Function

    Protected Function VisibleActive(Username As String) As String
        Dim result As Boolean = False
        'If Session("RoleName") = "Administrator" Or Session("RoleName") = "Account" Then : result = True : End If
         If Session("RoleName") = "Administrator" Or Session("RoleName") = "Account" Then 
            result = True
            If Username = "admin" AND Session("RoleName") = "Account" Then 
                result = false 
            End If
        End If
        Return result
    End Function

    Protected Function VisibleResetPass(Username As String) As String
        Dim result As Boolean = False
        If Session("RoleName") = "Administrator" Or Session("RoleName") = "Account" Then
            result = True 
            If Username = "admin" AND Session("RoleName") = "Account" Then 
                result = false 
            End If
        End If
        If Session("RoleName") = "Customer" And Session("LevelName") = "Leader" Then
            result = True
             If Username = "admin" Then 
                result = false 
            End If
        End If
        Return result
    End Function

    Protected Function VisibleActivity() As Boolean
        Dim result As Boolean = False
        If Session("RoleName") = "Administrator" Then : result = True : End If
        Return result
    End Function

    Protected Function DencryptPassword(Password As String) As String
        Dim result As String = publicCfg.Decrypt(Password)
        Return result
    End Function
End Class
