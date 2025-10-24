Partial Class Setting_Store_Default
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call MessageError(False, String.Empty)

            txtSearch.Text = Session("storeSearch")
            Call BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Call BindData(txtSearch.Text)
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("storeSearch") = txtSearch.Text
        Response.Redirect("~/setting/store/add", False)
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

            Session("storeDetail") = row.Cells(1).Text
            Session("storeSearch") = txtSearch.Text

            Response.Redirect("~/setting/store/detail", False)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdDelete.Text
            sdsPage.Delete()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "DELETE STORE. ID : " & lblId.Text)

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

            Dim newActive As Integer = 0
            If active = "False" Then : newActive = 1 : End If

            lblId.Text = row.Cells(1).Text
            lblActive.Text = newActive

            sdsPage.Update()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "ACTIVE / DEACTIVATE STORES. ID : " & lblId.Text)

            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindData(SearchText As String)
        Session("storeDetail") = "" : Session("storeSearch") = ""
        Try
            Dim search As String = String.Empty

            If Not SearchText = "" Then
                search = " WHERE Stores.Id LIKE '%" + SearchText + "%' OR Stores.Name LIKE '%" + SearchText + "%' OR Stores.Company LIKE '%" + SearchText + "%' OR Stores.Type LIKE '%" + SearchText + "%' OR Regions.AliasName LIKE '%" + SearchText + "%'"
            End If

            gvList.DataSource = publicCfg.GetListData(String.Format("SELECT Stores.*, Regions.AliasName AS RegionName FROM Stores LEFT JOIN Regions ON Stores.RegionId = Regions.Id {0} ORDER BY Stores.Name ASC", search))
            gvList.DataBind()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub

    Protected Function IconActiveOnNames(Active As String, Name As String) As String
        Dim result As String = "<i class='fa-regular text-danger fa-circle-xmark'></i> " + Name
        If Active = "True" Then : Return "<i class='fa-regular text-green fa-circle-check'></i> " + Name : End If
        Return result
    End Function

    Protected Function TextActive(Active As String) As String
        Dim result As String = "<i class='bi bi-arrow-repeat me-2 opacity-50 fs-3'></i> Switch Activated"
        ' Dim result As String = "<i class='fa-solid fa-arrows-rotate'></i>"
        ' If Active = "True" Then : Return "<i class='fa-solid fa-arrows-rotate'></i>" : End If
        Return result
    End Function

    Protected Function CssActive(Active As Boolean) As String
        Dim result As String = "dropdown-item"
        ' Dim result As String = "btn btn-green"
        ' If Active = False Then : result = "btn btn-secondary" : End If
        Return result
    End Function
End Class
